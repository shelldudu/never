#if !NET461
#else

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace Never.Web.Fakes
{
    /// <summary>
    /// 伪造httpcontext请求
    /// </summary>
    public class HttpContextWrapper : System.Web.HttpContextWrapper
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContextWrapper"/> class.
        /// </summary>
        public HttpContextWrapper()
            : base(HttpContext.Current != null ? HttpContext.Current : new HttpContext(new HttpRequest("", "http://localhost", ""), new HttpResponse(new StringWriter())))
        {
        }

        #endregion ctor
    }
}

#endif