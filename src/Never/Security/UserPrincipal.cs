using System.Security.Principal;

namespace Never.Security
{
    /// <summary>
    /// 用户凭证
    /// </summary>
    public class UserPrincipal : System.Security.Claims.ClaimsPrincipal, System.Security.Principal.IPrincipal
    {
        #region field

        /// <summary>
        /// 用户身份
        /// </summary>
        private readonly UserIdentity identity = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="UserPrincipal"/> class.
        /// </summary>
        /// <param name="identity">用户身份.</param>
        public UserPrincipal(UserIdentity identity)
        {
            this.identity = identity;
        }

        #endregion ctor

        #region IPrincipal成员

        /// <summary>
        /// 当前用户身份
        /// </summary>
        public override IIdentity Identity
        {
            get { return this.identity; }
        }

        /// <summary>
        /// 确定当前用户是否属于指定的角色。
        /// </summary>
        /// <param name="role">要检查其成员资格的角色的名称。</param>
        /// <returns>
        /// 如果当前用户是指定角色的成员，则为 true；否则为 false。
        /// </returns>
        public override bool IsInRole(string role)
        {
            if (this.CurrentUser == null)
                return false;

            return base.IsInRole(role);
        }

        #endregion IPrincipal成员

        #region 用户信息

        /// <summary>
        /// 当前用户信息
        /// </summary>
        public virtual IUser CurrentUser
        {
            get
            {
                var customerIdentity = this.identity as UserIdentity;
                if (customerIdentity == null)
                    return null;

                return customerIdentity.user;
            }
        }

        #endregion 用户信息
    }
}