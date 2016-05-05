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
    /// IPageContainer的默认实现,装载和提供服务的容器对象。
    /// </summary>
    public class PageContainer : IPageContainer
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
        /// 对页面对象的缓存列表。
        /// </summary>
        private readonly IDictionary<string, IPage> _pages = new Dictionary<string, IPage>();

        /// <summary>
        /// 对页面对象进行读写操作时使用的锁对象。
        /// </summary>
        private readonly ReaderWriterLockSlim _pagesLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 页面对象中进行指定的路由URL模式规则列表。
        /// </summary>
        private readonly IList<PageRoute> _routes = new List<PageRoute>();

        /// <summary>
        /// 对路由进行读写操作时使用的锁对象。
        /// </summary>
        private readonly ReaderWriterLockSlim _routesLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 提供遍历服务的路由信息对象列表。
        /// </summary>
        public IEnumerable<PageRoute> Routes
        {
            get { return _routes; }
        }

        /// <summary>
        /// 页面对象的数量。
        /// </summary>
        protected int PageCount
        {
            get
            {
                _pagesLock.EnterReadLock();
                try
                {
                    return _pages.Count;
                }
                finally
                {
                    _pagesLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// 页面加载。
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
        /// 初始化页面以及相关信息。
        /// </summary>
        protected virtual void Initialize()
        {
            LoadServicesFromAssemblies();
        }

        /// <summary>
        /// 从相关作为页面的程序集中加载页面对象到服务缓存列表中。
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
                            if ((attrs = type.GetCustomAttributes(typeof(PageAttribute), true)).Count() > 0)
                            {
                                IPage page = CreatePage(type, (PageAttribute)attrs[0]);
                                Register(page.Name, page);
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

        /// <summary>
        /// 与当前类型进行比较，根据不同情况创建对应类型的页面对象。
        /// </summary>
        /// <param name="type">要与IPage接口类型进行比较的类型。</param>
        /// <param name="attribute">页面特性对象。</param>
        /// <returns>返回一个页面对象。</returns>
        protected virtual IPage CreatePage(Type type, PageAttribute attribute)
        {
            //若该服务对象是继承IService接口
            if (typeof(IPage).IsAssignableFrom(type))
            {
                IPage page;
                page = type.CreateInstance<IPage>();
                page.Name = attribute.Name;

                if (string.IsNullOrEmpty(page.Path))
                {
                    page.Path = attribute.Path;
                }

                return page;
            }
            else
            {
                return new Frame.Service.Server.Core.Page()
                {
                    Name = attribute.Name,
                    Path = attribute.Path,
                    DefaultAction = attribute.DefaultAction,
                    Template = attribute.Template,
                    Type = type
                };
            }
        }

        /// <summary>
        /// 将页面对象装载到缓存列表中，并注册路由URL模式规则到路由URL模式规则列表。
        /// </summary>
        /// <param name="name">页面对象在缓存列表中的键。</param>
        /// <param name="page">页面对象。</param>
        public void Register(string name, IPage page)
        {
            if (null == name || null == page)
            {
                throw new ArgumentNullException();
            }
            AddPage(name, page);
            if (!string.IsNullOrEmpty(page.Path))
            {
                string path = page.Path.StartsWith("/") ? page.Path.Substring(1) : page.Path;

                //注册路由
                AddRoute(new PageRoute(path, page, new Dictionary<string, object>()
                                                           {
                                                               {Constants.PageRouteKey,name}
                                                           }));
            }
        }

        /// <summary>
        /// 将页面对象添加入缓存列表。
        /// </summary>
        /// <param name="name">页面对象在缓存列表中的键。</param>
        /// <param name="page">页面对象。</param>
        /// <param name="replace">当缓存列表中存在该页面时，指定是否进行替换。该值默认为false。</param>
        protected virtual void AddPage(string name, IPage page, bool replace = false)
        {
            _pagesLock.EnterWriteLock();
            try
            {
                if (!replace && _pages.ContainsKey(name))
                {
                    throw new InvalidOperationException(string.Format("页面'{0}'已存在!", name));
                }
                _pages[name] = page;
            }
            finally
            {
                _pagesLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 注册路由，将页面路由对象添加到路由URL模式规则列表中。
        /// </summary>
        /// <param name="route">页面路由对象。</param>
        protected virtual void AddRoute(PageRoute route)
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
        /// 获取与指定页面名称相关联的页面对象。
        /// </summary>
        /// <param name="name">页面对象在缓存列表中的键。一般为页面名称。</param>
        /// <returns>如果找到指定键，则返回与该键相关联的页面对象；否则，将返回null。</returns>
        public IPage Resolve(string name)
        {
            return GetPage(name);
        }

        /// <summary>
        /// 从页面缓存列表中获取与指定页面名称相关联的页面对象。
        /// </summary>
        /// <param name="name">页面对象在缓存列表中的键。一般为页面名称。</param>
        /// <returns>如果找到指定键，则返回与该键相关联的页面对象；否则，将返回null。</returns>
        protected IPage GetPage(string name)
        {
            _pagesLock.EnterReadLock();
            try
            {
                IPage page;
                return _pages.TryGetValue(name, out page) ? page : null;
            }
            finally
            {
                _pagesLock.ExitReadLock();
            }
        }

    }
}
