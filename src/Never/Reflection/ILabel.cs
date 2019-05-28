using System.Reflection.Emit;

namespace Never.Reflection
{
    /// <summary>
    /// 标签
    /// </summary>
    public interface ILabel : IOwner
    {
        #region prop

        /// <summary>
        /// 标签
        /// </summary>
        Label Label { get; }

        #endregion prop
    }
}