#if !NET461
#else

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using ApiController = System.Web.Http.ApiController;

namespace Never.Web.WebApi.Results
{
    /// <summary>
    /// actionresult
    /// </summary>
    /// <seealso cref="Never.Web.WebApi.IHttpActionResult" />
    public abstract class ActionResult : IHttpActionResult
    {
        /// <summary>
        /// Api控制器
        /// </summary>
        protected ApiController ApiController { get; set; }

        /// <summary>
        /// 执行结果
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public virtual HttpResponseMessage Execute(CancellationToken cancellationToken)
        {
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// 执行结果
        /// </summary>
        /// <param name="cancellationToken">传播有关应取消操作的通知</param>
        /// <returns></returns>
        public virtual Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.Execute(cancellationToken));
        }
    }
}

#endif