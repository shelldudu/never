using Never.Serialization;
using System.Threading.Tasks;

#if !NET461
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
#else

using System.Web.Mvc;

#endif

namespace Never.Web.Mvc.Results
{
    /// <summary>
    /// JsonResult
    /// </summary>
    public class MyJsonResult<T> : ActionResult, IHttpActionResult
    {
        #region field

        /// <summary>
        /// 结果
        /// </summary>
        private readonly T content = default(T);

        /// <summary>
        /// 序列化接口
        /// </summary>
        private readonly IJsonSerializer jsonSerializer = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="content"></param>
        /// <param name="jsonSerializer"></param>
        public MyJsonResult(T content, IJsonSerializer jsonSerializer)
        {
            this.content = content;
            this.jsonSerializer = jsonSerializer;
        }

        #endregion ctor

        #region 重新渲染

#if !NET461

        public override Task ExecuteResultAsync(ActionContext context)
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
        ///
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            var ser = this.jsonSerializer == null ? Never.Serialization.SerializeEnvironment.JsonSerializer : this.jsonSerializer;
            var ouput = ser.Serialize<T>(content);

            var jsoncallback = context.Controller.ValueProvider.GetValue("callback");
            if (jsoncallback != null && !string.IsNullOrEmpty(jsoncallback.AttemptedValue))
            {
                context.HttpContext.Response.ContentType = "application/x-javascript";
                context.HttpContext.Response.Write(string.Format("{0}({1})", jsoncallback.AttemptedValue, ouput));
                return;
            }
            else
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.Write(string.Format("{0}", ouput));
            }
        }

#endif

        #endregion 重新渲染

        #region new

        /// <summary>
        /// 创建一个新的结果集
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        internal static MyJsonResult<TKey> New<TKey>(TKey content)
        {
            return new MyJsonResult<TKey>(content, Never.Serialization.SerializeEnvironment.JsonSerializer);
        }

        /// <summary>
        /// 创建一个新的结果集
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="content">The content.</param>
        /// <param name="jsonSerializer"></param>
        /// <returns></returns>
        internal static MyJsonResult<TKey> New<TKey>(TKey content, IJsonSerializer jsonSerializer)
        {
            return new MyJsonResult<TKey>(content, jsonSerializer);
        }

        #endregion new
    }
}