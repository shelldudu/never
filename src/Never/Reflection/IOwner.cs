namespace Never.Reflection
{
    /// <summary>
    /// 所属者属性
    /// </summary>
    public interface IOwner
    {
        /// <summary>
        /// 所拥有者
        /// </summary>
        object Owner { get; }
    }
}