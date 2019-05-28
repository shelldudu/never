#if !NET461
#else

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
    /// 文件结果
    /// </summary>
    /// <seealso cref="Never.Web.WebApi.Results.ActionResult" />
    /// <seealso cref="Never.Web.WebApi.IHttpActionResult" />
    public abstract class FileResult : ActionResult, IHttpActionResult
    {
        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// 下载文件名
        /// </summary>
        public string FileDownloadName { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="contentType"></param>
        public FileResult(string contentType)
        {
            this.ContentType = contentType;
        }

        /// <summary>
        /// 执行结果
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public override HttpResponseMessage Execute(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage();
            var contentType = new MediaTypeHeaderValue(this.ContentType);
            this.WriteFile(response);
            response.Content.Headers.ContentType = contentType;
            response.RequestMessage = new HttpRequestMessage(this.ApiController.Request.Method, this.ApiController.Request.RequestUri);
            if (!string.IsNullOrEmpty(this.FileDownloadName))
                response.Headers.Add("Content-Disposition", this.FileDownloadName);

            return base.Execute(cancellationToken);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="response"></param>
        protected virtual void WriteFile(HttpResponseMessage response)
        {
        }
    }
}

#endif