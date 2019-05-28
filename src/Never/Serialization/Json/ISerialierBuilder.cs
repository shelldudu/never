using System;

namespace Never.Serialization.Json
{
    /// <summary>
    /// Interface ISerialierBuilder
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISerialierBuilder<T>
    {
        /// <summary>
        /// 进行构建，该回调方法第一个参数是写入器，第二个参数是配置，第三个参数是数据类型，第四个参数：如果上层也是复合对象，当前对象也是复合对象，则调用的时候层次会加1，类似递归
        /// </summary>
        Action<ISerializerWriter, JsonSerializeSetting, T, byte> Build(JsonSerializeSetting setting);
    }
}