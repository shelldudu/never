using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Never.Web.Mvc
{
#if !NET461

    /// <summary>
    /// 方法结果
    /// </summary>
    public interface IHttpActionResult : Microsoft.AspNetCore.Mvc.IActionResult
    {
    }
#else

    using System.Web.Mvc;

    /// <summary>
    /// 方法结果
    /// </summary>
    public interface IHttpActionResult
    {
        /// <summary>
        /// 启用对操作方法结果的处理
        /// </summary>
        /// <param name="context">用于执行结果的上下文。上下文信息包括控制器、HTTP 内容、请求上下文和路由数据</param>
        void ExecuteResult(ControllerContext context);
    }

#endif
}