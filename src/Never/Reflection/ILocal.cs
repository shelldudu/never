using System;
using System.Reflection.Emit;

namespace Never.Reflection
{
    /// <summary>
    /// 局部变量
    /// </summary>
    public interface ILocal : IOwner
    {
        #region porp

        /// <summary>
        /// 变量对象
        /// </summary>
        Type LocalType { get; }

        /// <summary>
        /// 索引
        /// </summary>
        int Index { get; }

        /// <summary>
        /// 局部变量
        /// </summary>
        LocalBuilder LocalBuilder { get; }

        #endregion porp
    }
}