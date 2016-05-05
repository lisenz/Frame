namespace Frame.Service.Server
{
    /// <summary>
    /// 提供解析执行服务请求的操作方法对象的方法。
    /// </summary>
    public interface IServiceCommandResolver
    {
        /// <summary>
        /// 解析出与指定SQL指令源的键相关联的执行服务请求操作对象。
        /// </summary>
        /// <param name="name">SQL指令源对应SQL语句的键。</param>
        /// <param name="context">服务上下文对象。</param>
        /// <returns>返回执行服务请求操作对象。</returns>
        IServiceCommand Resolve(string name, IServiceContext context);
    }
}
