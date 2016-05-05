namespace Frame.Service.Server
{
    /// <summary>
    /// 提供执行服务请求的操作方法。
    /// </summary>
    public interface IServiceCommand
    {
        /// <summary>
        /// 执行操作服务上下文对象提供的信息，默认为执行服务类中的指定方法。
        /// </summary>
        /// <param name="context">服务上下文对象。</param>
        /// <returns>执行服务方法返回的结果。</returns>
        object Execute(IServiceContext context);
    }
}
