using System.Collections.Generic;

namespace Frame.Service.Server
{
    /// <summary>
    /// IPage对象的容器，加载页面并提供注册和查询接口
    /// </summary>
    public interface IPageContainer
    {
        /// <summary>
        /// 在引擎启动时被调用，实现类可在此方法中进行初始化并加载页面
        /// </summary>
        void Load();

        /// <summary>
        /// 注册一个页面
        /// </summary>
        /// <param name="name">关联页面的键。</param>
        /// <param name="page">一个页面对象。</param>
        void Register(string name, IPage page);

        /// <summary>
        /// 返回页面路由列表，默认情况下页面使用默认的路由规则，此属性返回自定义的路由规则。
        /// </summary>
        IEnumerable<PageRoute> Routes { get; }

        /// <summary>
        /// 根据页面的键找到页面对象。
        /// </summary>
        /// <param name="name">页面的键。</param>
        /// <returns>返回一个与指定键相关联的页面对象。</returns>
        IPage Resolve(string name);
    }
}
