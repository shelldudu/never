using Never.Serialization;
using Never.Web.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
    /// json 结果信息
    /// </summary>
    public class MyJsonResult<T> : ActionResult, IHttpActionResult
#else

    /// <summary>
    /// json 结果信息
    /// </summary>
    public class MyJsonResult<T> : ActionResult, IHttpActionResult
#endif

    {
        #region field

        private readonly HttpRequestMessage request = null;
        private readonly Encoding encoding = null;
        private readonly T content = default(T);
        private readonly IJsonSerializer jsonSerializer = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MyJsonResult{T}"/> class.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="controller">The controller.</param>
        public MyJsonResult(T content, ApiController controller)
            : this(content, controller, Never.Serialization.SerializeEnvironment.JsonSerializer, new UTF8Encoding(false, true))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyJsonResult{T}"/> class.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="controller">The controller.</param>
        /// <param name="jsonSerializer"></param>
        public MyJsonResult(T content, ApiController controller, IJsonSerializer jsonSerializer)
            : this(content, controller, jsonSerializer, new UTF8Encoding(false, true))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyJsonResult{T}"/> class.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="controller">The controller.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="jsonSerializer"></param>
        public MyJsonResult(T content, ApiController controller, IJsonSerializer jsonSerializer, Encoding encoding)
        {
#if !NET461
            this.request = new HttpRequestMessage(new HttpMethod(controller.Request.Method), controller.Request.PathBase.Value);
            this.encoding = encoding;
            this.content = content;
            this.jsonSerializer = jsonSerializer;
#else
            this.ApiController = controller;
            this.request = new HttpRequestMessage(controller.Request.Method, controller.Request.RequestUri);
            this.encoding = encoding;
            this.content = content;
            this.jsonSerializer = jsonSerializer;
            controller.ControllerContext.ControllerDescriptor.Properties[BasicController.HttpResponseTag] = this;
#endif
        }

        #endregion ctor

#if !NET461
        public override Task ExecuteResultAsync(Microsoft.AspNetCore.Mvc.ActionContext context)
        {
            var ser = this.jsonSerializer == null ? Never.Serialization.SerializeEnvironment.JsonSerializer : this.jsonSerializer;
            var ouput = ser.Serialize<T>(content);

            var jsoncallback = context.ActionDescriptor.RouteValues.ContainsKey("callback") ? context.ActionDescriptor.RouteValues["callback"] : null;
            if (jsoncallback != null && !string.IsNullOrEmpty(jsoncallback))
            {
                context.HttpContext.Response.ContentType = "application/x-javascript";
                using (var writer = new System.IO.StreamWriter(context.HttpContext.Response.Body))
                {
                    writer.Write(string.Format("{0}({1})", jsoncallback, ouput));
                }
            }
            else
            {
                context.HttpContext.Response.ContentType = "application/json";
                using (var writer = new System.IO.StreamWriter(context.HttpContext.Response.Body))
                {
                    writer.Write(string.Format("{0}", ouput));
                }
            }

            return Task.CompletedTask;
        }
#else

        /// <summary>
        /// 执行结果
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public override HttpResponseMessage Execute(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var buffer = this.encoding.GetBytes(this.jsonSerializer.Serialize(content));
            response.Content = new ByteArrayContent(buffer, 0, buffer.Length);
            MediaTypeHeaderValue contentType = new MediaTypeHeaderValue("application/json");
            contentType.CharSet = this.encoding.WebName;
            response.Content.Headers.ContentType = contentType;
            response.RequestMessage = this.request;
            return response;
        }

#endif
    }
}