using System;

namespace Never.Serialization.Json.Deserialize
{
    /// <summary>
    /// 每一个key与value节点
    /// </summary>
    public class JsonContentNode : IObjectContentNode
    {
        #region prop

        /// <summary>
        /// 当前层次
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 序列化的名字
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 节点内容，当前有Object和数组两种
        /// </summary>
        public object Node { get; set; }

        /// <summary>
        /// json当前节点类型
        /// </summary>
        public ContentNodeType NodeType { get; set; }

        /// <summary>
        /// 序列化的内容
        /// </summary>
        public ArraySegment<char> Segment { get; set; }

        /// <summary>
        /// 原始json 串
        /// </summary>
        public string Original { get; set; }

        /// <summary>
        /// 指示该节点是否含有编码
        /// </summary>
        public bool Escaping { get; set; }

        /// <summary>
        /// 该节点key值使用的引号
        /// </summary>
        public ValueQuoteSignal ValueQuote { get; set; }

        /// <summary>
        /// 该数组节点key[[[]]这样的层数，分别为0，1，2，
        /// </summary>
        public int ArrayLevel { get; set; }

        #endregion prop

        #region ctor

        /// <summary>
        /// 
        /// </summary>
        public JsonContentNode()
        {
        }

        /// <summary>
        ///
        /// </summary>
        public JsonContentNode(ArraySegment<char> segment)
        {
            this.Segment = segment;
        }

        /// <summary>
        ///
        /// </summary>
        public JsonContentNode(char[] value, int start, int endintex) : this(new ArraySegment<char>(value, start, endintex - start))
        {
        }

        #endregion ctor

        /// <summary>
        /// 序列化的内容
        /// </summary>
        /// <returns></returns>
        public ArraySegmentValue GetValue()
        {
            if (this.Segment == null)
                return ArraySegmentValue.Empty;

            return new ArraySegmentValue(this.Segment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Segment == null)
                return string.Empty;

            switch (this.NodeType)
            {
                case ContentNodeType.String:
                    return this.Original.Substring(this.Segment.Offset, this.Segment.Count);
                case ContentNodeType.Object:
                    return string.Concat("{", this.Original.Substring(this.Segment.Offset, this.Segment.Count), "}");
                case ContentNodeType.Array:
                    return string.Concat("[", this.Original.Substring(this.Segment.Offset, this.Segment.Count), "]");
            }

            return this.Original.Substring(this.Segment.Offset, this.Segment.Count);
        }
    }
}