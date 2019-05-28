using Never.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if !NET461
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Never.Web.WebApi.Security
{
    /// <summary>
    /// 用户身份验证
    /// </summary>
    public class UserPrincipalAttribute : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// 开始验证
        /// </summary>
        /// <param name="context"></param>
        public virtual void OnAuthorization(AuthorizationFilterContext context)
        {
            if (this.IsAuthorizable(context))
            {
                return;
            }

            var user = this.GetUser(context, context.HttpContext.Request);
            if (user != null)
            {
                var principal = new UserPrincipal(new UserIdentity(user));
                context.HttpContext.User = principal;
            }

            this.OnAuthorizeCompleted(context, user);
        }

        /// <summary>
        /// 当验证完成后怎么做
        /// </summary>
        /// <param name="context"></param>
        /// <param name="user">用户</param>
        protected virtual void OnAuthorizeCompleted(AuthorizationFilterContext context, IUser user)
        {

        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="filterContext"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public virtual IUser GetUser(AuthorizationFilterContext context, Microsoft.AspNetCore.Http.HttpRequest request)
        {
            return null;
        }

        /// <summary>
        /// 可验证的
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected bool IsAuthorizable(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                return true;
            }

            //不再验证了
            if (context.HttpContext.User != null && context.HttpContext.User is UserPrincipal && context.HttpContext.User.Identity.IsAuthenticated)
            {
                return true;
            }

            //if (Thread.CurrentPrincipal != null && Thread.CurrentPrincipal is UserPrincipal && Thread.CurrentPrincipal.Identity.IsAuthenticated)
            //{
            //    return true;
            //}

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        protected void SetUnauthorized(AuthorizationFilterContext filterContext)
        {
            filterContext.Result = new UnauthorizedResult();
        }

        /// <summary>
        /// 将当前请求结果重新描绘到新的内容结果中
        /// </summary>
        /// <param name="filterContext">The action context.</param>
        /// <param name="status">The status.</param>
        protected void SetHttpStatusCode(AuthorizationFilterContext filterContext, HttpStatusCode status)
        {
            filterContext.Result = new StatusCodeResult((int)status);
        }
    }
}

#else

using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Security;

namespace Never.Web.WebApi.Security
{
    /// <summary>
    /// 用户身份验证
    /// </summary>
    public class UserPrincipalAttribute : System.Web.Http.Filters.FilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// 开始验证
        /// </summary>
        /// <param name="actionContext"></param>
        public virtual void OnAuthorization(HttpActionContext actionContext)
        {
            if (this.IsAuthorizable(actionContext))
            {
                return;
            }

            IUser user = this.GetUser(actionContext, this.DiscloseTicket(actionContext));
            if (user != null)
            {
                IPrincipal principal = new UserPrincipal(new UserIdentity(user));
                actionContext.RequestContext.Principal = principal;
            }
            this.OnAuthorizeCompleted(actionContext, user);
        }


        /// <summary>
        /// 当验证完成后怎么做
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="user"></param>
        protected virtual void OnAuthorizeCompleted(HttpActionContext actionContext, IUser user)
        {

        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        protected virtual IUser GetUser(HttpActionContext actionContext, FormsAuthenticationTicket ticket)
        {
            return default(IUser);
        }

        /// <summary>
        /// 可验证的
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected bool IsAuthorizable(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                return false;
            }

            //不再验证了
            if (actionContext.RequestContext.Principal != null && actionContext.RequestContext.Principal is UserPrincipal && actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                return true;
            }

            //if (Thread.CurrentPrincipal != null && Thread.CurrentPrincipal is UserPrincipal && Thread.CurrentPrincipal.Identity.IsAuthenticated)
            //{
            //    return true;
            //}

            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected FormsAuthenticationTicket DiscloseTicket(HttpActionContext actionContext)
        {
            if (actionContext == null || actionContext.Request.Headers.Authorization == null || string.IsNullOrEmpty(actionContext.Request.Headers.Authorization.Parameter))
            {
                return null;
            }

            return FormsAuthentication.Decrypt(actionContext.Request.Headers.Authorization.Parameter);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, System.Func<Task<HttpResponseMessage>> continuation)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            if (continuation == null)
            {
                throw new ArgumentNullException("continuation");
            }

            try
            {
                this.OnAuthorization(actionContext);
            }
            catch (Exception exception)
            {
                var task = new TaskCompletionSource<HttpResponseMessage>();
                task.SetException(exception);
                return task.Task;
            }

            if (actionContext.Response != null)
            {
                var task = new TaskCompletionSource<HttpResponseMessage>();
                task.SetResult(actionContext.Response);
                return task.Task;
            }

            return continuation();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected void SetUnauthorized(HttpActionContext actionContext, string message = "无权限访问")
        {
            actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, message);
        }

        /// <summary>
        /// 将当前请求标识为未授权
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected void SetUnauthorized(HttpActionContext actionContext, Exception ex)
        {
            actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized, ex);
        }

        /// <summary>
        /// 将当前请求结果重新描绘到新的内容结果中
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <param name="status">The status.</param>
        /// <param name="ex">The ex.</param>
        protected void SetHttpStatusCode(HttpActionContext actionContext, HttpStatusCode status, Exception ex)
        {
            actionContext.Response = actionContext.Request.CreateErrorResponse(status, ex);
        }

        /// <summary>
        /// 将当前请求结果重新描绘到新的内容结果中
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <param name="status">The status.</param>
        /// <param name="message">The message.</param>
        protected void SetHttpStatusCode(HttpActionContext actionContext, HttpStatusCode status, string message)
        {
            actionContext.Response = actionContext.Request.CreateErrorResponse(status, message);
        }
    }
}

#endif