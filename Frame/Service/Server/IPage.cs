using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.Service.Server
{
    public interface IPage
    {
        /// <summary>
        /// 页面名称，全局唯一。
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 页面的URL路径，此值为空表示使用全局默认的路由规则。
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// 页面对应的View模版文件名称。
        /// </summary>
        string Template { get; set; }

        /// <summary>
        /// 执行页面的访问并返回值。
        /// </summary>
        /// <param name="context">一次页面请求的上下文对象。</param>
        /// <returns>调用请求后返回的执行结果。</returns>
        object Visit(IPageContext context);
    }
}
