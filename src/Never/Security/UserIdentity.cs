namespace Never.Security
{
    /// <summary>
    /// 用户身份
    /// </summary>
    public class UserIdentity : System.Security.Principal.IIdentity
    {
        #region

        /// <summary>
        /// 当前用户
        /// </summary>
        internal readonly IUser user;

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdentity"/> class.
        /// </summary>
        /// <param name="user">当前用户</param>
        public UserIdentity(IUser user)
        {
            this.user = user;
        }

        #endregion

        #region IIdentity成员

        /// <summary>
        /// 获取所使用的身份验证的类型。
        /// </summary>
        public virtual string AuthenticationType
        {
            get { return "User"; }
        }

        /// <summary>
        /// 获取一个值，该值指示是否验证了用户。
        /// </summary>
        public virtual bool IsAuthenticated
        {
            get { return this.user != null; }
        }

        /// <summary>
        /// 获取当前用户的名称。
        /// </summary>
        public virtual string Name
        {
            get
            {
                return string.IsNullOrEmpty(this.user.UserName) ? this.user.UserId.ToString() : this.user.UserName;
            }
        }

        #endregion
    }
}