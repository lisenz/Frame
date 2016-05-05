using System;
using System.Web.Routing;
using Frame.Service.Server.Core;

namespace Frame.Service.Server
{
    /// <summary>
    /// 通用页面引擎主入口类，需要在Global.aspx中的Application_Start事件中调用Run()方法来启动服务引擎，
    /// 或者在Web.Config中配置HttpModule进行启动。
    /// </summary>
    public class PageEngine
    {
        /// <summary>
        /// 表示默认服务的路由URL模式规则。
        /// </summary>
        public const string DefaultPageRoute = "{path}/{" + Constants.PageRouteKey + "}";

        /// <summary>
        /// 表示默认服务+方法的路由URL模式规则。
        /// </summary>
        public const string DefaultPageActionRoute = "{path}/{" + Constants.PageRouteKey + "}/{" + Constants.ActionRouteKey + "}";

        /// <summary>
        /// 创建一个服务请求时的锁对象。
        /// </summary>
        private readonly static object _syncRoot = new object();

        /// <summary>
        /// 标识服务引擎是否已启动。
        /// </summary>
        private static bool _runned;

        /// <summary>
        /// 表示一个页面容器对象。
        /// </summary>
        private static IPageContainer _pageContainer;

        /// <summary>
        /// 表示一个页面请求对象。
        /// </summary>
        private static IPageHandler _pageHandler;

        /// <summary>
        /// 在页面的路由URL模式规则中特定的标识页面的路径。
        /// </summary>
        private static string _pagePath = Constants.PageRoutePath;

        /// <summary>
        /// 获取创建一个页面容器对象。
        /// </summary>
        public static IPageContainer PageContainer
        {
            get
            {
                if (null == _pageContainer)
                {
                    lock (_syncRoot)
                    {
                        if (null == _pageContainer)
                        {
                            GetOrCreateInstanceFor<IPageContainer>(out _pageContainer, () => new PageContainer());
                        }
                    }
                }
                return _pageContainer;
            }
            set
            {
                CheckNullAndRunnedOnSetValue("PageContainer", value);
                _pageContainer = value;
            }
        }

        /// <summary>
        /// 获取创建一个页面请求。
        /// </summary>
        public static IPageHandler PageHandler
        {
            get
            {
                if (null == _pageHandler)
                {
                    lock (_syncRoot)
                    {
                        if (null == _pageHandler)
                        {
                            GetOrCreateInstanceFor<IPageHandler>(out _pageHandler, () => new PageHandler());
                        }
                    }
                }
                return _pageHandler;
            }
            set
            {
                CheckNullAndRunnedOnSetValue("PageHandler", value);
                _pageHandler = value;
            }
        }

        /// <summary>
        /// 获取或设置服务的路径。这里指服务的文件夹名称。
        /// </summary>
        public static string PagePath
        {
            get { return _pagePath; }
            set
            {
                CheckNullAndRunnedOnSetValue("PagePath", value);
                _pagePath = value.StartsWith("/") ? value.Substring(1) : value;
            }
        }

        /// <summary>
        /// 启动页面引擎。
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
        /// 启动页面引擎所进行的加载服务以及注册路由等操作。
        /// </summary>
        private static void DoRun()
        {
            LoadPages();

            RegisterRoutes();
        }

        /// <summary>
        /// 加载页面到页面容器对象中。
        /// </summary>
        private static void LoadPages()
        {
            PageContainer.Load();
        }

        /// <summary>
        /// 注册路由到路由表中。
        /// </summary>
        private static void RegisterRoutes()
        {
            PageHandler.PageContainer = PageContainer;

            //注册默认路由URL模式规则
            string path1 = DefaultPageRoute.Replace("{path}", PagePath);
            string path2 = DefaultPageActionRoute.Replace("{path}", PagePath);

            RouteTable.Routes.Add(new Route(path1, PageHandler));
            RouteTable.Routes.Add(new Route(path2, PageHandler));

            foreach (PageRoute route in PageContainer.Routes)
            {
                RouteTable.Routes.Add(new Route(route.Path, new RouteValueDictionary(route.Defaults), PageHandler));
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
                    throw new InvalidOperationException(string.Format("页面引擎已经启动,禁止设置'{0}'的值。", name));
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
