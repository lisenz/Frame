namespace Frame.Service.Server
{
    /// <summary>
    /// 服务对象接口。
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// 服务名称，全局唯一。
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 服务路由的URL路径，此值为空表示使用全局默认的路由规则。
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// 执行服务的调用并返回值。
        /// </summary>
        /// <param name="context">一次服务请求的上下文对象。</param>
        /// <returns>调用服务后返回的执行结果。</returns>
        object Execute(IServiceContext context);
    }
}
