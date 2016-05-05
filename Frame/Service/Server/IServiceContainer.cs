using System.Collections.Generic;

namespace Frame.Service.Server
{
    /// <summary>
    /// IService对象的容器，加载服务并提供注册和查询接口
    /// </summary>
    public interface IServiceContainer
    {

        /// <summary>
        /// 在引擎启动时被调用，实现类可在此方法中进行初始化并加载服务
        /// </summary>
        void Load();

        /// <summary>
        /// 注册一个服务
        /// </summary>
        /// <param name="name">关联服务的键。</param>
        /// <param name="service">一个服务对象。</param>
        void Register(string name, IService service);

        /// <summary>
        /// 返回服务路由列表，默认情况下服务使用默认的路由规则，此属性返回自定义的路由规则。
        /// </summary>
        IEnumerable<ServiceRoute> Routes { get; }

        /// <summary>
        /// 根据服务的键找到服务对象。
        /// </summary>
        /// <param name="name">服务的键。</param>
        /// <returns>返回一个与指定键相关联的服务对象。</returns>
        IService Resolve(string name);
    }
}
