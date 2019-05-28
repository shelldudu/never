using System;

namespace Never.Serialization.Json
{
    /// <summary>
    /// 反序列化构建
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeserialierBuilder<T>
    {
        /// <summary>
        /// 进行构建，该回调方法第一个参数是读取器，第二个参数是配置，第三个参数：下一个数组层次，如果为1，则表示数组连续，比如2维数据，通常为0
        /// </summary>
        /// <returns></returns>
        Func<IDeserializerReader, JsonDeserializeSetting, int, T> Build(JsonDeserializeSetting setting);
    }
}