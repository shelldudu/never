namespace Never.Serialization.Json
{
    /// <summary>
    /// value 标签引号类型
    /// </summary>
    public enum ValueQuoteSignal : byte
    {
        /// <summary>
        /// 无，默认值
        /// </summary>
        No = 0,

        /// <summary>
        /// 单引号
        /// </summary>
        Single = 1,

        /// <summary>
        /// 双引号
        /// </summary>
        Double = 2,
    }

    /// <summary>
    /// 当前内容的容器结构类型
    /// </summary>
    public enum ContainerSignal : byte
    {
        /// <summary>
        /// 空的
        /// </summary>
        Empty = 0,

        /// <summary>
        /// 是{对象，表示当前内容是在{的容器中
        /// </summary>
        Object = 1,

        /// <summary>
        /// 是[对象，表示当前内容是在[的容器中
        /// </summary>
        Array = 2,
    }
}