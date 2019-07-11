using Never.Security;
using System;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Web;

#if !NET461
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Never.Web.Mvc.Security
{
    /// <summary>
    /// 自定义身份验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class UserPrincipalAttribute : Attribute, IAuthorizationFilter
    {
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
        /// <param name="user"></param>
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

using System.Web.Mvc;
using System.Web.Security;

namespace Never.Web.Mvc.Security
{
    /// <summary>
    /// 自定义身份验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class UserPrincipalAttribute : System.Web.Mvc.FilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// 验证核心
        /// </summary>
        /// <param name="filterContext"></param>
        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (this.IsAuthorizable(filterContext))
            {
                return;
            }

            var user = this.GetUser(filterContext, this.DiscloseTicket(filterContext.HttpContext.Request));
            if (user != null)
            {
                IPrincipal principal = new UserPrincipal(new UserIdentity(user));
                filterContext.HttpContext.User = principal;
            }

            this.OnAuthorizeCompleted(filterContext, user);
        }

        /// <summary>
        /// 当验证完成后怎么做
        /// </summary>
        /// <param name="filterContext"></param>
        /// <param name="user"></param>
        protected virtual void OnAuthorizeCompleted(AuthorizationContext filterContext, IUser user)
        {

        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="filterContext"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public virtual IUser GetUser(AuthorizationContext filterContext, FormsAuthenticationTicket ticket)
        {
            return null;
        }

        /// <summary>
        /// 可验证的
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        protected bool IsAuthorizable(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                return false;
            }

            //不再验证了
            if (filterContext.HttpContext.User != null && !filterContext.HttpContext.User.Identity.IsAuthenticated && filterContext.HttpContext.User is UserPrincipal)
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
        /// 从cookie中解密
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected FormsAuthenticationTicket DiscloseTicket(HttpRequestBase request)
        {
            var value = this.QueryCookieValue(request);
            if (value == null)
            {
                return null;
            }

            return FormsAuthentication.Decrypt(value);
        }

        /// <summary>
        /// 查询Cookie
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected HttpCookie QueryCookie(HttpRequestBase request)
        {
            if (request == null || request.Cookies == null)
            {
                return null;
            }

            HttpCookie cookie = request.Cookies.Get(FormsAuthentication.FormsCookieName);
            if (cookie == null)
            {
                return null;
            }

            return cookie;
        }

        /// <summary>
        /// 查询Cookie的值
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected string QueryCookieValue(HttpRequestBase request)
        {
            var cookie = this.QueryCookie(request);
            return cookie == null ? null : cookie.Value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        protected void SetUnauthorized(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }

        /// <summary>
        /// 将当前请求结果重新描绘到新的内容结果中
        /// </summary>
        /// <param name="filterContext">The action context.</param>
        /// <param name="status">The status.</param>
        /// <param name="statusDescription">The message.</param>
        protected void SetHttpStatusCode(AuthorizationContext filterContext, HttpStatusCode status, string statusDescription)
        {
            filterContext.Result = new HttpStatusCodeResult(status, statusDescription);
        }
    }
}

#endif