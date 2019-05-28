using System;

namespace Never.Serialization.Json
{
    /// <summary>
    /// 每一个key与value节点
    /// </summary>
    public interface IContentNode
    {
        /// <summary>
        /// 当前层次
        /// </summary>
        int Level { get; }

        /// <summary>
        /// 序列化的名字
        /// </summary>
        string Key { get; }

        /// <summary>
        /// 序列化的内容
        /// </summary>
        ArraySegment<char> Segment { get; }

        /// <summary>
        /// 序列化的内容
        /// </summary>
        /// <returns></returns>
        ArraySegmentValue GetValue();

        /// <summary>
        /// 指示该节点是否含有编码
        /// </summary>
        bool Escaping { get; }

        /// <summary>
        /// 该节点key值使用的引号
        /// </summary>
        ValueQuoteSignal ValueQuote { get; }

        /// <summary>
        /// 该数组节点key[[[]]这样的层数，分别为0，1，2，
        /// </summary>
        int ArrayLevel { get; }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}