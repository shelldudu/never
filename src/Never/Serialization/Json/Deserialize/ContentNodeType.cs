namespace Never.Serialization.Json.Deserialize
{
    /// <summary>
    /// json当前节点类型
    /// </summary>
    public enum ContentNodeType
    {
        /// <summary>
        /// 是一个字符串
        /// </summary>
        String = 0,

        /// <summary>
        /// 是一个数组
        /// </summary>
        Array = 1,

        /// <summary>
        /// 是一个对象
        /// </summary>
        Object = 2
    }
}