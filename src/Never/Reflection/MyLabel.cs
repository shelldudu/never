using System.Reflection.Emit;

namespace Never.Reflection
{
    /// <summary>
    /// 标签
    /// </summary>
    public struct MyLabel : ILabel, IOwner
    {
        #region prop

        /// <summary>
        /// 标签
        /// </summary>
        public Label Label { get; set; }

        /// <summary>
        /// 所拥有者
        /// </summary>
        public object Owner { get; set; }

        #endregion prop
    }
}