namespace Never.Serialization.Json.Deserialize
{
    /// <summary>
    /// json 节点
    /// </summary>
    public interface IObjectContentNode : IContentNode
    {
        /// <summary>
        /// 节点内容，当前有Object和数组两种
        /// </summary>
        object Node { get; }

        /// <summary>
        /// json当前节点类型
        /// </summary>
        ContentNodeType NodeType { get; }

        /// <summary>
        /// 原始json 串
        /// </summary>
        string Original { get; set; }
    }
}