using System;
using System.Reflection.Emit;

namespace Never.Reflection
{
    /// <summary>
    /// 局部变量
    /// </summary>
    public struct MyLocal : ILocal, IOwner
    {
        #region porp

        /// <summary>
        /// 变量对象
        /// </summary>
        public Type LocalType { get; set; }

        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 局部变量
        /// </summary>
        public LocalBuilder LocalBuilder { get; set; }

        /// <summary>
        /// 所拥有者
        /// </summary>
        public object Owner { get; set; }

        #endregion porp
    }
}