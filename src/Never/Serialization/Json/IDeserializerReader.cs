using Never.Serialization.Json.Deserialize;
using System;

namespace Never.Serialization.Json
{
    /// <summary>
    /// 反序列化读取
    /// </summary>
    public interface IDeserializerReader
    {
        /// <summary>
        /// 读取一个节点值
        /// </summary>
        /// <param name="key">节点key</param>
        /// <returns></returns>
        IObjectContentNode Read(string key);

        /// <summary>
        /// 读取一个节点值
        /// </summary>
        /// <param name="key">节点key</param>
        /// <param name="comparison">排序规则</param>
        /// <returns></returns>
        IObjectContentNode Read(string key, StringComparison comparison);

        /// <summary>
        /// 当前的条数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 重新组织节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IDeserializerReader Parse(IObjectContentNode node);

        /// <summary>
        /// 读取下一个节点
        /// </summary>
        /// <returns></returns>
        IObjectContentNode MoveNext();

        /// <summary>
        /// 当前内容的容器结构类型
        /// </summary>
        ContainerSignal ContainerSignal { get; }

        /// <summary>
        /// 重置读取器
        /// </summary>
        void Reset();
    }
}