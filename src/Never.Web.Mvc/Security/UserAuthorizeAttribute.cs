using Never.Security;
using System.Threading;

#if !NET461
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Never.Web.Mvc.Security
{
    /// <summary>
    /// 授权行为
    /// </summary>
    public sealed class UserAuthorizeAttribute : UserPrincipalAttribute
    {
        /// <summary>
        /// 当验证完成后怎么做
        /// </summary>
        /// <param name="filterContext"></param>
        /// <param name="user">用户</param>
        protected override void OnAuthorizeCompleted(AuthorizationFilterContext filterContext, IUser user)
        {
            if (!this.IsAuthorizable(filterContext))
            {
                this.SetUnauthorized(filterContext);
            }
        }
    }
}

#else

using System.Web.Mvc;
using System.Web.Security;

namespace Never.Web.Mvc.Security
{
    /// <summary>
    /// 授权行为
    /// </summary>
    public sealed class UserAuthorizeAttribute : UserPrincipalAttribute
    {
        /// <summary>
        /// 当验证完成后怎么做
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="user">用户</param>
        protected override void OnAuthorizeCompleted(AuthorizationContext actionContext, IUser user)
        {
            if (!this.IsAuthorizable(actionContext))
            {
                this.SetUnauthorized(actionContext);
            }
        }
    }
}

#endif