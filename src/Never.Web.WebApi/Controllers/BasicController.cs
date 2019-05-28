using Never.IoC;
using Never.Logging;
using Never.Mappers;
using Never.Security;
using Never.Serialization;
using Never.Web;
using Never.Net;
using Never.Web.WebApi.Results;
using Never.Web.WebApi.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Never.Commands;
using Never.Attributes;

#if !NET461

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
#else

using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Controller = System.Web.Http.ApiController;
#endif

namespace Never.Web.WebApi.Controllers
{
    /// <summary>
    /// 抽象控制器类
    /// </summary>
    public abstract class BasicController : Controller
    {
        #region resule

        /// <summary>
        /// 请求接口数据结果，通常用于对外接口
        /// </summary>
        /// <typeparam name="TResult">结果对象</typeparam>
        [Serializable]
        [System.Runtime.Serialization.DataContract]
        public sealed class ResponseResult<TResult>
        {
            #region property

            /// <summary>
            /// 代号，也是状态值
            /// </summary>
            [DefaultValue(Value = "code")]
            [System.Runtime.Serialization.DataMember(Name = "code")]
            [Never.Serialization.Json.DataMember(Name = "code")]
            public string Code { get; set; }

            /// <summary>
            /// 附带信息
            /// </summary>
            [DefaultValue(Value = "string.Empty")]
            [System.Runtime.Serialization.DataMember(Name = "message")]
            [Never.Serialization.Json.DataMember(Name = "message")]
            public string Message { get; set; }

            /// <summary>
            /// 结果
            /// </summary>
            [DefaultValue(Value = "default(data)")]
            [System.Runtime.Serialization.DataMember(Name = "data")]
            [Never.Serialization.Json.DataMember(Name = "data")]
            public TResult Data { get; set; }

            #endregion property

            #region ctor

            /// <summary>
            /// Initializes a new instance of the <see cref="ResponseResult{TResult}"/> class.
            /// </summary>
            public ResponseResult()
                : this(string.Empty, default(TResult), string.Empty)
            {
                /*保留这个是因为在Wcf中用到契约的话，没这个构造则提示契约不可用*/
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ResponseResult{TResult}"/> class.
            /// </summary>
            /// <param name="code">The code.</param>
            /// <param name="result">The result.</param>
            public ResponseResult(string code, TResult result)
                : this(code, result, string.Empty)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ResponseResult{TResult}"/> class.
            /// </summary>
            /// <param name="code">The code.</param>
            /// <param name="result">The result.</param>
            /// <param name="message">The message.</param>
            public ResponseResult(string code, TResult result, string message)
            {
                this.Code = code;
                this.Data = result;
                this.Message = message;
            }

            #endregion ctor
        }

        #endregion

        #region error

        /// <summary>
        /// model验证错误信息
        /// </summary>
        public struct ModelStateError
        {
            /// <summary>
            /// 异常信息
            /// </summary>
            public Exception Exception { get; set; }

            /// <summary>
            /// 错误信息
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// 成员名称
            /// </summary>
            public string MemberName { get; set; }
        }

        /// <summary>
        /// model验证错误信息
        /// </summary>
        public IEnumerable<ModelStateError> ModelError
        {
            get
            {
                if (this.ModelState.IsValid)
                {
                    yield break;
                }

                if (this.ModelState.Any() && this.ModelState.Keys.Any())
                {
                    foreach (var key in this.ModelState.Keys)
                    {
                        var errors = this.ModelState[key];
                        if (errors != null && errors.Errors != null && errors.Errors.Any())
                        {
                            foreach (var e in errors.Errors)
                            {
                                yield return new ModelStateError()
                                {
                                    Exception = e.Exception,
                                    Message = e.ErrorMessage,
                                    MemberName = key
                                };
                            }
                        }
                    }
                }

                yield break;
            }
        }

        /// <summary>
        ///model验证错误信息
        /// </summary>
        public string ModelErrorMessage
        {
            get
            {
                var first = this.ModelError?.FirstOrDefault();
                return first?.Message;
            }
        }

        #endregion error

        #region prop

        /// <summary>
        /// response
        /// </summary>
        internal static readonly string HttpResponseTag = "IHttpResponseMessage";

        /// <summary>
        /// Gets the object serializer.
        /// </summary>
        protected IJsonSerializer JsonSerializer { get; private set; }

        /// <summary>
        /// 下载器
        /// </summary>
        protected IHttpDownloader HttpDownloader { get; set; }

        /// <summary>
        /// 当前用户信息
        /// </summary>
        protected IUser CurrentUser
        {
            get
            {
                if (!this.User.Identity.IsAuthenticated)
                {
                    return null;
                }

                var principal = this.User as UserPrincipal;
                if (principal == null)
                {
                    return null;
                }

                return principal.CurrentUser;
            }
        }

        #endregion prop

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicController"/> class.
        /// </summary>
        protected BasicController()
            : this(Never.Serialization.SerializeEnvironment.JsonSerializer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicController"/> class.
        /// </summary>
        /// <param name="jsonSerializer">The object serializer.</param>
        protected BasicController(IJsonSerializer jsonSerializer)
        {
            this.JsonSerializer = jsonSerializer;
            this.HttpDownloader = new WebRequestDownloader();
        }

        #endregion ctor

        #region json

        /// <summary>
        /// 空对象
        /// </summary>
        public struct EmptyObject
        {

        }

        /// <summary>
        /// 生成JsonResult
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [NonAction]
        public virtual IHttpActionResult Json(ApiStatus status)
        {
            return this.Json(status, string.Empty);
        }

        /// <summary>
        /// 生成JsonResult
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        public virtual IHttpActionResult Json(ApiStatus status, string message)
        {
            return this.Json(status, new EmptyObject(), message);
        }

        /// <summary>
        /// 生成JsonResult
        /// </summary>
        /// <param name="status"></param>
        /// <param name="object"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        public virtual IHttpActionResult Json(ApiStatus status, object @object, string message)
        {
            return new MyJsonResult<ApiResult<object>>(new ApiResult<object>(status, @object, message), this, this.JsonSerializer);
        }

        /// <summary>
        /// 生成JsonResult
        /// </summary>
        /// <param name="status"></param>
        /// <param name="object"></param>
        /// <returns></returns>
        [NonAction]
        public virtual IHttpActionResult Json<T>(ApiStatus status, T @object)
        {
            return this.Json<T>(status, @object, string.Empty);
        }

        /// <summary>
        /// 生成JsonResult
        /// </summary>
        /// <param name="status"></param>
        /// <param name="object"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        public virtual IHttpActionResult Json<T>(ApiStatus status, T @object, string message)
        {
            return new MyJsonResult<ApiResult<T>>(new ApiResult<T>(status, @object, message), this, this.JsonSerializer);
        }

        /// <summary>
        /// 生成JsonResult
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [NonAction]
        public virtual ActionResult Json(string code)
        {
            return this.Json(code, string.Empty);
        }

        /// <summary>
        /// 生成JsonResult
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        public virtual ActionResult Json(string code, string message)
        {
            return new MyJsonResult<ResponseResult<EmptyObject>>(new ResponseResult<EmptyObject>(code, new EmptyObject(), message), this, this.JsonSerializer);
        }

        /// <summary>
        /// 生成JsonResult
        /// </summary>
        /// <param name="code"></param>
        /// <param name="object"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        public virtual ActionResult Json(string code, object @object, string message)
        {
            return new MyJsonResult<ResponseResult<object>>(new ResponseResult<object>(code, @object, message), this, this.JsonSerializer);
        }

        /// <summary>
        /// 生成JsonResult
        /// </summary>
        /// <param name="code"></param>
        /// <param name="object"></param>
        /// <returns></returns>
        [NonAction]
        public virtual ActionResult Json<T>(string code, T @object)
        {
            return this.Json<T>(code, @object, string.Empty);
        }

        /// <summary>
        /// 生成JsonResult
        /// </summary>
        /// <param name="code"></param>
        /// <param name="object"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        public virtual ActionResult Json<T>(string code, T @object, string message)
        {
            return new MyJsonResult<ResponseResult<T>>(new ResponseResult<T>(code, @object, message), this, this.JsonSerializer);
        }

        #endregion json

        #region mapper

        /// <summary>
        /// 自动映射对象
        /// </summary>
        /// <typeparam name="From">源来对象类型</typeparam>
        /// <typeparam name="To">目标对象类型</typeparam>
        /// <param name="from">源来</param>
        /// <returns></returns>
        protected virtual To Map<From, To>(From from)
        {
            return this.Map(from, (Action<From, To>)null);
        }

        /// <summary>
        /// 自动映射对象
        /// </summary>
        /// <typeparam name="From">源来对象类型</typeparam>
        /// <typeparam name="To">目标对象类型</typeparam>
        /// <param name="from">源来</param>
        /// <param name="callBack">回调</param>
        /// <returns></returns>
        protected virtual To Map<From, To>(From from, Action<From, To> callBack)
        {
            return EasyMapper.Map(from, callBack);
        }

        /// <summary>
        /// 自动映射对象
        /// </summary>
        /// <typeparam name="From">源来对象类型</typeparam>
        /// <typeparam name="To">目标对象类型</typeparam>
        /// <param name="from">源来</param>
        /// <param name="target">目标</param>
        /// <returns></returns>
        protected virtual To Map<From, To>(From from, To target)
        {
            return this.Map(from, target, null);
        }

        /// <summary>
        /// 自动映射对象
        /// </summary>
        /// <typeparam name="From">源来对象类型</typeparam>
        /// <typeparam name="To">目标对象类型</typeparam>
        /// <param name="from">源来</param>
        /// <param name="target">目标</param>
        /// <param name="callBack">回调</param>
        /// <returns></returns>
        protected virtual To Map<From, To>(From from, To target, Action<From, To> callBack)
        {
            return EasyMapper.Map(from, target, callBack);
        }

        #endregion mapper

        #region validate and execute

#if !NET461

        /// <summary>
        /// 对model进行验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override bool TryValidateModel(object model)
        {
            try
            {
                if (!this.ModelState.IsValid)
                    return this.ModelState.IsValid;

                if (model == null)
                    return base.TryValidateModel(model);

                var modelType = model.GetType();
                var validator = ValidatorProvider.GetValidator(modelType);
                if (validator == null)
                    return base.TryValidateModel(model);

                var modelValidator = new ValidationModelValidator(validator);
                var array = modelValidator.ValidateModel(model);
                if (array.IsNullOrEmpty())
                    return base.TryValidateModel(model);

                if (array.Any())
                {
                    foreach (var a in array)
                        this.ModelState.AddModelError(a.MemberName, a.Message);

                    return false;
                }

                return base.TryValidateModel(model);
            }
            catch
            {
            }

            return base.TryValidateModel(model);
        }

#else

        /// <summary>
        /// 对model进行验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool TryValidateModel(object model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.ModelState.IsValid;
            }

            if (model == null)
            {
                this.ModelState.AddModelError("parameter", new ArgumentNullException());
                return this.ModelState.IsValid;
            }

            var modelType = model.GetType();
            var validator = ValidatorProvider.GetValidator(modelType);
            if (validator == null)
            {
                return this.ModelState.IsValid;
            }

            var modelValidator = new ValidationModelValidator(new System.Web.Http.Validation.ModelValidatorProvider[0], validator);
            var array = modelValidator.ValidateModel(model);
            if (array.IsNullOrEmpty())
            {
                return this.ModelState.IsValid;
            }

            if (array.Any())
            {
                foreach (var a in array)
                {
                    this.ModelState.AddModelError(a.MemberName, a.Message);
                }

                return false;
            }

            return this.ModelState.IsValid;
        }

        /// <summary>
        /// 异步执行单个 HTTP 操作。
        /// </summary>
        /// <param name="controllerContext">单个 HTTP 操作的控制器上下文。</param>
        /// <param name="cancellationToken">为 HTTP 操作分配的取消标记。</param>
        /// <returns>新启动的任务。</returns>
        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            var task = base.ExecuteAsync(controllerContext, cancellationToken);
            object @actionResult = null;
            controllerContext.ControllerDescriptor.Properties.TryRemove(BasicController.HttpResponseTag, out @actionResult);
            if (@actionResult == null)
            {
                return task;
            }

            var myjson = @actionResult as IHttpActionResult;
            if (myjson == null)
            {
                return task;
            }

            return myjson.ExecuteAsync(cancellationToken);
        }

#endif

        /// <summary>
        /// 获取处理结果的消息内容
        /// </summary>
        /// <returns></returns>
        protected string HandlerMerssage(ICommandHandlerResult result, string message = null)
        {
            if (result == null || result.Message.IsNullOrEmpty())
            {
                return message;
            }

            return result.Message;
        }

        #endregion validate and execute
    }
}