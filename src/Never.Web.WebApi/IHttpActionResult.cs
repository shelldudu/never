using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Never.Web.WebApi
{
#if !NET461

    /// <summary>
    /// 表示是一个执行结果信息
    /// </summary>
    public interface IHttpActionResult : Microsoft.AspNetCore.Mvc.IActionResult
    {
    }
#else

    /// <summary>
    /// 表示是一个执行结果信息
    /// </summary>
    public interface IHttpActionResult : System.Web.Http.IHttpActionResult
    {

    }

#endif
}