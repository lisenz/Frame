using System;

namespace Frame.Service.Client
{
    /// <summary>
    /// 表示一个Ajax请求目前所处的阶段。
    /// </summary>
    public enum AjaxStage
    {
        /// <summary>
        /// 请求被创建，但尚未设置参数，这是最初始的状态
        /// </summary>
        Created,

        /// <summary>
        /// 请求参数已经初始化正确，但尚未发送请求
        /// </summary>
        Prepared,

        /// <summary>
        /// 已经成功发送请求
        /// </summary>
        Requested,

        /// <summary>
        /// 已经收到服务器端的响应
        /// </summary>
        Responsed
    }
}
