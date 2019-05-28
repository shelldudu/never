using System;

namespace Never.Security
{
    /// <summary>
    /// 当前会员
    /// </summary>
    public interface IUser : IEquatable<IUser>
    {
        #region property

        /// <summary>
        /// 用户注册Id
        /// </summary>
        long UserId { get; }

        /// <summary>
        /// 用户名
        /// </summary>
        string UserName { get; }

        #endregion property
    }
}