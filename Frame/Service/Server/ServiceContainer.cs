using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using Frame.Core.Extensions;
using Frame.Service.Server.Attributes;

namespace Frame.Service.Server
{
    /// <summary>
    /// IServiceContainer的默认实现,装载和提供服务的容器对象。
    /// </summary>
    public class ServiceContainer : IServiceContainer
    {
        /// <summary>
        /// 不进行装载的程序集列表。
        /// </summary>
        private static readonly string[] excludesAssemblies = new string[]
        {
            "Microsoft","Microsoft.", "System", "System.", "mscorlib", "log4net",
            "Frame.Core","Frame.Data","Frame.DataStore"
        };

        /// <summary>
        /// 标识是否已进行初始化。
        /// </summary>
        private bool _initialized = false;

        /// <summary>
        /// 进行服务加载时候的锁对象。
        /// </summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        /// 对服务对象的缓存列表。
        /// </summary>
        private readonly IDictionary<string, IService> _services = new Dictionary<string, IService>();

        /// <summary>
        /// 对服务对象进行读写操作时使用的锁对象。
        /// </summary>
        private readonly ReaderWriterLockSlim _servicesLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 服务对象中进行指定的路由URL模式规则列表。
        /// </summary>
        private readonly IList<ServiceRoute> _routes = new List<ServiceRoute>();

        /// <summary>
        /// 对路由进行读写操作时使用的锁对象。
        /// </summary>
        private readonly ReaderWriterLockSlim _routesLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 提供遍历服务的路由信息对象列表。
        /// </summary>
        public IEnumerable<ServiceRoute> Routes
        {
            get { return _routes; }
        }

        /// <summary>
        /// 服务对象的数量。
        /// </summary>
        protected int ServiceCount
        {
            get
            {
                _servicesLock.EnterReadLock();
                try
                {
                    return _services.Count;
                }
                finally
                {
                    _servicesLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// 服务加载。
        /// </summary>
        public void Load()
        {
            if (!_initialized)
            {
                lock (_syncRoot)
                {
                    this.Initialize();
                    this._initialized = true;
                }
            }
        }

        /// <summary>
        /// 将服务对象装载到缓存列表中，并注册路由URL模式规则到路由URL模式规则列表。
        /// </summary>
        /// <param name="name">服务对象在缓存列表中的键。</param>
        /// <param name="service">服务对象。</param>
        public void Register(string name, IService service)
        {
            if (null == name || null == service)
            {
                throw new ArgumentNullException();
            }
            AddService(name, service);
            if (!string.IsNullOrEmpty(service.Path))
            {
                string path = service.Path.StartsWith("/") ? service.Path.Substring(1) : service.Path;

                //注册路由
                AddRoute(new ServiceRoute(path, service, new Dictionary<string, object>()
                                                           {
                                                               {Constants.ServiceRouteKey,name}
                                                           }));
            }
        }

        /// <summary>
        /// 获取与指定服务名称相关联的服务对象。
        /// </summary>
        /// <param name="name">服务对象在缓存列表中的键。一般为服务名称。</param>
        /// <returns>如果找到指定键，则返回与该键相关联的服务对象；否则，将返回null。</returns>
        public IService Resolve(string name)
        {
            return GetService(name);
        }

        /// <summary>
        /// 从服务缓存列表中获取与指定服务名称相关联的服务对象。
        /// </summary>
        /// <param name="name">服务对象在缓存列表中的键。一般为服务名称。</param>
        /// <returns>如果找到指定键，则返回与该键相关联的服务对象；否则，将返回null。</returns>
        protected IService GetService(string name)
        {
            _servicesLock.EnterReadLock();
            try
            {
                IService service;
                return _services.TryGetValue(name, out service) ? service : null;
            }
            finally
            {
                _servicesLock.ExitReadLock();
            }
        }

        /// <summary>
        /// 注册路由，将服务路由对象添加到路由URL模式规则列表中。
        /// </summary>
        /// <param name="route">服务路由对象。</param>
        protected virtual void AddRoute(ServiceRoute route)
        {
            _routesLock.EnterWriteLock();
            try
            {
                _routes.Add(route);
            }
            finally
            {
                _routesLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 将服务对象添加入缓存列表。
        /// </summary>
        /// <param name="name">服务对象在缓存列表中的键。</param>
        /// <param name="service">服务对象。</param>
        /// <param name="replace">当缓存列表中存在该服务时，指定是否进行替换。该值默认为false。</param>
        protected virtual void AddService(string name, IService service, bool replace = false)
        {
            _servicesLock.EnterWriteLock();
            try
            {
                if (!replace && _services.ContainsKey(name))
                {
                    throw new InvalidOperationException(string.Format("服务'{0}'已存在!", name));
                }
                _services[name] = service;
            }
            finally
            {
                _servicesLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 初始化服务以及相关信息。
        /// </summary>
        protected virtual void Initialize()
        {
            LoadServicesFromAssemblies();
        }

        /// <summary>
        /// 与当前类型进行比较，根据不同情况创建对应类型的服务对象。
        /// </summary>
        /// <param name="type">要与IService接口类型进行比较的类型。</param>
        /// <param name="attribute">服务特性对象。</param>
        /// <returns>返回一个服务对象。</returns>
        protected virtual IService CreateService(Type type, ServiceAttribute attribute)
        {
            //若该服务对象是继承IService接口
            if (typeof(IService).IsAssignableFrom(type))
            {
                IService service;
                service = type.CreateInstance<IService>();
                service.Name = attribute.Name;

                if (string.IsNullOrEmpty(service.Path))
                {
                    service.Path = attribute.Path;
                }

                return service;
            }
            else
            {
                return new Frame.Service.Server.Core.Service()
                {
                    Name = attribute.Name,
                    Path = attribute.Path,
                    DefaultMethod = attribute.DefaultMethod,
                    Type = type
                };
            }
        }

        /// <summary>
        /// 从相关作为服务的程序集中加载服务对象到服务缓存列表中。
        /// </summary>
        protected virtual void LoadServicesFromAssemblies()
        {
            var assemblies = LoadAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    string assemblyName = assembly.GetName().Name;
                    if (excludesAssemblies.Any(name => assemblyName.Equals(name) || (name.EndsWith(".") && assemblyName.StartsWith(name))))
                    {
                        continue;
                    }

                    foreach (Type type in assembly.GetTypes())
                    {
                        if (!type.IsInterface && !type.IsAbstract)
                        {
                            object[] attrs;
                            if ((attrs = type.GetCustomAttributes(typeof(ServiceAttribute), true)).Count() > 0)
                            {
                                IService service = CreateService(type, (ServiceAttribute)attrs[0]);
                                Register(service.Name, service);
                            }
                        }
                    }
                }
                catch
                {
                    //(ReflectionTypeLoadException e)
                    continue;
                }
            }
        }

        /// <summary>
        /// 获取已加载到此应用程序域的执行上下文中的程序集。
        /// </summary>
        /// <returns>返回一个可以遍历程序集列表的迭代器。</returns>
        protected virtual IEnumerable<Assembly> LoadAssemblies()
        {
            HashSet<Assembly> assemblies = new HashSet<Assembly>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                assemblies.Add(assembly);
            }
            assemblies.Add(Assembly.GetCallingAssembly());
            return assemblies;
        }
    }
}
