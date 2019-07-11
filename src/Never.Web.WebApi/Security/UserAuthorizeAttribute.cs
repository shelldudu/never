using Never.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#if !NET461
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Never.Web.WebApi.Security
{
    /// <summary>
    /// 授权行为
    /// </summary>
    public sealed class UserAuthorizeAttribute : UserPrincipalAttribute
    {
        /// <summary>
        /// 当验证完成后怎么做
        /// </summary>
        /// <param name="context"></param>
        /// <param name="user">用户</param>
        protected override void OnAuthorizeCompleted(AuthorizationFilterContext context, IUser user)
        {
            if (!this.IsAuthorizable(context))
            {
                this.SetUnauthorized(context);
            }       
        }
    }
}

#else

using System.Web.Http.Controllers;

namespace Never.Web.WebApi.Security
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
        protected override void OnAuthorizeCompleted(HttpActionContext actionContext, IUser user)
        {
            if (!this.IsAuthorizable(actionContext))
            {
                this.SetUnauthorized(actionContext);
            }      
        }
    }
}

#endif