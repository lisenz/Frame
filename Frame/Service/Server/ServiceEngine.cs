using System;
using System.Web.Routing;
using Frame.Service.Server.Core;

namespace Frame.Service.Server
{
    /// <summary>
    /// 通用服务引擎主入口类，需要在Global.aspx中的Application_Start事件中调用Run()方法来启动服务引擎，
    /// 或者在Web.Config中配置HttpModule进行启动。
    /// </summary>
    public class ServiceEngine
    {
        /// <summary>
        /// 表示默认服务的路由URL模式规则。
        /// </summary>
        public const string DefaultServiceRoute = "{path}/{" + Constants.ServiceRouteKey + "}";

        /// <summary>
        /// 表示默认服务+方法的路由URL模式规则。
        /// </summary>
        public const string DefaultServiceMethodRoute = "{path}/{" + Constants.ServiceRouteKey + "}/{" + Constants.MethodRouteKey + "}";

        /// <summary>
        /// 创建一个服务请求时的锁对象。
        /// </summary>
        private readonly static object _syncRoot = new object();

        /// <summary>
        /// 标识服务引擎是否已启动。
        /// </summary>
        private static bool _runned;

        /// <summary>
        /// 表示一个服务容器对象。
        /// </summary>
        private static IServiceContainer _serviceContainer;

        /// <summary>
        /// 表示一个服务请求对象。
        /// </summary>
        private static IServiceHandler _serviceHandler;

        /// <summary>
        /// 在服务的路由URL模式规则中特定的标识服务的路径。
        /// </summary>
        private static string _servicePath = Constants.ServiceRoutePath;

        /// <summary>
        /// 获取创建一个服务容器对象。
        /// </summary>
        public static IServiceContainer ServiceContainer
        {
            get
            {
                if (null == _serviceContainer)
                {
                    lock (_syncRoot)
                    {
                        if (null == _serviceContainer)
                        {
                            GetOrCreateInstanceFor<IServiceContainer>(out _serviceContainer, () => new ServiceContainer());
                        }
                    }
                }
                return _serviceContainer;
            }
            set
            {
                CheckNullAndRunnedOnSetValue("ServiceContainer", value);
                _serviceContainer = value;
            }
        }

        /// <summary>
        /// 获取创建一个服务请求。
        /// </summary>
        public static IServiceHandler ServiceHandler
        {
            get
            {
                if (null == _serviceHandler)
                {
                    lock (_syncRoot)
                    {
                        if (null == _serviceHandler)
                        {
                            GetOrCreateInstanceFor<IServiceHandler>(out _serviceHandler, () => new ServiceHandler());
                        }
                    }
                }
                return _serviceHandler;
            }
            set
            {
                CheckNullAndRunnedOnSetValue("ServiceHandler", value);
                _serviceHandler = value;
            }
        }

        /// <summary>
        /// 获取或设置服务的路径。这里指服务的文件夹名称。
        /// </summary>
        public static string ServicePath
        {
            get { return _servicePath; }
            set
            {
                CheckNullAndRunnedOnSetValue("ServicePath", value);
                _servicePath = value.StartsWith("/") ? value.Substring(1) : value;
            }
        }

        /// <summary>
        /// 启动服务引擎。
        /// </summary>
        public static void Run()
        {
            if (!_runned)
            {
                lock (_syncRoot)
                {
                    DoRun();

                    _runned = true;
                }
            }
        }

        /// <summary>
        /// 启动服务引擎所进行的加载服务以及注册路由等操作。
        /// </summary>
        private static void DoRun()
        {
            LoadServices();

            RegisterRoutes();
        }

        /// <summary>
        /// 加载服务到服务容器对象中。
        /// </summary>
        private static void LoadServices()
        {
            ServiceContainer.Load();
        }

        /// <summary>
        /// 注册路由到路由表中。
        /// </summary>
        private static void RegisterRoutes()
        {
            ServiceHandler.ServiceContainer = ServiceContainer;

            //注册默认路由URL模式规则
            string path1 = DefaultServiceRoute.Replace("{path}", ServicePath);
            string path2 = DefaultServiceMethodRoute.Replace("{path}", ServicePath);

            RouteTable.Routes.Add(new Route(path1, ServiceHandler));
            RouteTable.Routes.Add(new Route(path2, ServiceHandler));

            foreach (ServiceRoute route in ServiceContainer.Routes)
            {
                RouteTable.Routes.Add(new Route(route.Path, new RouteValueDictionary(route.Defaults), ServiceHandler));
            }
        }

        /// <summary>
        /// 检测设置的指定对象的值是否为空且服务引擎是否启动。
        /// </summary>
        /// <param name="name">要设置值的对象名称。</param>
        /// <param name="value">要设置的值。</param>
        private static void CheckNullAndRunnedOnSetValue(string name, object value)
        {
            if (null == value || (value is string && string.IsNullOrEmpty((string)value)))
            {
                if (null == value)
                {
                    throw new ArgumentNullException(string.Format("{0}的值不能为空。", name), name);
                }
                if (_runned)
                {
                    throw new InvalidOperationException(string.Format("服务引擎已经启动,禁止设置'{0}'的值。", name));
                }
            }
        }

        /// <summary>
        /// 获取或创建泛型类型参数指定的类型的对象。
        /// </summary>
        /// <typeparam name="T">要创建的对象的类型。</typeparam>
        /// <param name="instance">要创建的对象。</param>
        /// <param name="CreateDefault">一个委托。</param>
        private static void GetOrCreateInstanceFor<T>(out T instance, Func<T> CreateDefault)
        {
            instance = CreateDefault();
        }
    }
}
