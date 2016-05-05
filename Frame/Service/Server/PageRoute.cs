using System.Collections.Generic;

namespace Frame.Service.Server
{
    public class PageRoute
    {
        /// <summary>
        /// 路由的URL模式。
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// 路由的服务对象。
        /// </summary>
        private readonly IPage _page;

        /// <summary>
        /// 默认情况下服务使用默认的路由规则列表。
        /// </summary>
        private readonly IDictionary<string, object> _defaults;


        /// <summary>
        /// 获取该服务路由的URL模式。
        /// </summary>
        public string Path
        {
            get { return _path; }
        }

        /// <summary>
        /// 获取默认路由规则列表。
        /// </summary>
        public IDictionary<string, object> Defaults
        {
            get { return _defaults; }
        }

        /// <summary>
        /// 获取关联的服务对象。
        /// </summary>
        public IPage Page
        {
            get { return _page; }
        }

        /// <summary>
        /// 初始化路由的URL模式以及对应关联的服务对象。
        /// </summary>
        /// <param name="path">路由的URL模式。</param>
        /// <param name="service">关联的服务对象。</param>
        public PageRoute(string path, IPage page)
            : this(path, page, null)
        {
        }

        /// <summary>
        /// 初始化路由的URL模式、对应关联的服务对象以及默认的路由规则列表。
        /// </summary>
        /// <param name="path">路由的URL模式。</param>
        /// <param name="service">关联的服务对象。</param>
        /// <param name="defaults">服务使用默认的路由规则列表。</param>
        public PageRoute(string path, IPage page, IDictionary<string, object> defaults)
        {
            _path = path;
            _page = page;
            _defaults = defaults ?? new Dictionary<string, object>();
        }
    }
}
