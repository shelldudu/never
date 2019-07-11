using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Threading;
using System.Net.Http;

#if !NET461
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ApiController = Microsoft.AspNetCore.Mvc.Controller;

#else

using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using ApiController = System.Web.Http.ApiController;

#endif

namespace Never.Web.WebApi.Results
{
#if !NET461
    /// <summary>
    /// PDF Result
    /// </summary>
    public class MyPDFResult : FileResult, IHttpActionResult
#else

    /// <summary>
    /// json 结果信息
    /// </summary>
    public class MyPDFResult : FileResult, IHttpActionResult
#endif
    {
        #region property

        /// <summary>
        /// 序列化对象
        /// </summary>
        public Stream Stream { get; set; }

        #endregion property

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="controller"></param>
        public MyPDFResult(Stream stream, ApiController controller)
            : base("application/pdf")
        {
            this.Stream = stream;

#if !NET461
#else
            this.ApiController = controller;
            controller.ControllerContext.ControllerDescriptor.Properties[Controllers.BasicController.HttpResponseTag] = this;
#endif
        }

        #endregion ctor

        #region 重新渲染

#if !NET461

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ExecuteResultAsync(ActionContext context)
        {
            this.FileDownloadName = TrimExtension(!string.IsNullOrEmpty(FileDownloadName) ? FileDownloadName : DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            return base.ExecuteResultAsync(context).ContinueWith(t =>
            {
                if (this.Stream == null)
                    this.Stream = new MemoryStream();

                this.Stream.Position = 0;
                this.Stream.CopyTo(context.HttpContext.Response.Body);
            });
        }
#else

        /// <summary>
        /// 执行结果
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public override HttpResponseMessage Execute(CancellationToken cancellationToken)
        {
            this.FileDownloadName = TrimExtension(!string.IsNullOrEmpty(FileDownloadName) ? FileDownloadName : DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            return base.Execute(cancellationToken);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="response"></param>
        protected override void WriteFile(HttpResponseMessage response)
        {
            if (this.Stream == null)
                this.Stream = new MemoryStream();

            this.Stream.Position = 0;
            response.Content = new System.Net.Http.StreamContent(this.Stream);
        }

#endif

        #endregion 重新渲染

        #region 去掉扩展名

        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string TrimExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return fileName;

            if (string.IsNullOrEmpty(System.IO.Path.GetExtension(fileName)))
            {
                var newFile = fileName.Sub(0, fileName.LastIndexOf('.'));
                return string.Concat(newFile, ".pdf");
            }

            return fileName;
        }

        #endregion 去掉扩展名
    }
}