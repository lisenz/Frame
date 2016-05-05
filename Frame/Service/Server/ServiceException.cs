using System;
using Frame.Service.Server.Core;

namespace Frame.Service.Server
{
    public class ServiceException : Exception
    {
        /// <summary>
        /// 获取异常信息状态码。
        /// </summary>
        public int Code { get; private set; }

        /// <summary>
        /// 获取异常信息的状态描述。
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// 获取异常信息的错误描述。
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 设置服务异常信息。
        /// </summary>
        /// <param name="message">解释异常原因的错误消息。</param>
        /// <param name="innerException">导致当前异常的异常；如果未指定内部异常，则是一个null引用。</param>
        /// <param name="code">异常信息对应的状态码。</param>
        /// <param name="status">异常信息对应的状态描述。</param>
        /// <param name="desc">异常信息的错误描述。</param>
        public ServiceException(string message = "", Exception innerException = null, int code = Result.ServerError, string status = "", string desc = Result.ServerErrorDesc)
            : base(message, innerException)
        {
            this.Code = code;
            this.Status = status;
            this.Description = desc;
        }

        /// <summary>
        /// 设置服务请求过程中发生的异常错误消息，该消息状态码一般指定为500。
        /// </summary>
        /// <param name="message">错误消息的内容。</param>
        /// <returns>返回一个状态码为500的服务异常信息对象。</returns>
        public static ServiceException Error(string message)
        {
            return new ServiceException(code: Result.ServerError, desc: message);
        }

        /// <summary>
        /// 设置服务请求过程中发生的异常错误消息，该消息错误码一般指定为500。
        /// 导致这个问题的原因一般为服务器的内部错误
        /// </summary>
        /// <param name="message">错误消息的内容。</param>
        /// <param name="e">执行服务过程中发生的错误。</param>
        /// <returns>返回一个状态码为500的服务异常信息对象。</returns>
        public static ServiceException Error(string message, Exception e)
        {
            return new ServiceException(code: Result.ServerError, desc: message, innerException: e);
        }

        /// <summary>
        /// 设置服务请求过程中发生的异常错误消息，该消息状态码一般指定为404。
        /// TODO：
        /// 导致这个错误的原因一般来说，有三种：
        /// 1、无法在所请求的端口上访问Web站点。
        /// 2、Web服务扩展锁定策略阻止本请求。
        /// 3、MIME映射策略阻止本请求。
        /// </summary>
        /// <param name="message">错误消息的内容，一般意味着链接指向的网页不存在，即原始网页的URL失效。</param>
        /// <returns>返回一个状态码为404的服务异常信息对象。</returns>
        public static ServiceException NotFound(string message)
        {
            return new ServiceException(code: Result.NotFound, desc: message);
        }

        /// <summary>
        /// 设置服务请求过程中发生的异常错误消息，该消息状态码一般指定为400。
        /// 导致该问题的一般原因为语法格式有误，服务器无法理解此请求。
        /// </summary>
        /// <param name="message">错误消息的内容。</param>
        /// <returns>返回一个状态码为400的服务异常信息对象。</returns>
        public static ServiceException BadRequest(string message)
        {
            return new ServiceException(code: Result.BadRequest, desc: message);
        }
    }
}
