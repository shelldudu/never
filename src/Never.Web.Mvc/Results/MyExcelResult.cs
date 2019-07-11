using System;
using System.IO;
using System.Threading.Tasks;

#if !NET461
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
#else

using System.ServiceModel.Syndication;
using System.Web.Mvc;

#endif

namespace Never.Web.Mvc.Results
{
    /// <summary>
    /// ExcelResult
    /// </summary>
    public class MyExcelResult : FileResult, IHttpActionResult
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
        public MyExcelResult(Stream stream)
            : base("text/xls")
        {
            this.Stream = stream;
        }

        #endregion ctor

        #region 重新渲染

#if !NET461

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
        ///
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            this.FileDownloadName = TrimExtension(!string.IsNullOrEmpty(FileDownloadName) ? FileDownloadName : DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            base.ExecuteResult(context);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="response"></param>
        protected override void WriteFile(System.Web.HttpResponseBase response)
        {
            if (this.Stream == null)
                this.Stream = new MemoryStream();

            this.Stream.Position = 0;
            this.Stream.CopyTo(response.OutputStream);
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
                return string.Concat(newFile, ".xlsx");
            }

            return fileName;
        }

        #endregion 去掉扩展名
    }
}