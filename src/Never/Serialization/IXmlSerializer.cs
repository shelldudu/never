using System;

namespace Never.Serialization
{
    /// <summary>
    /// xml序列化接口
    /// </summary>
    public interface IXmlSerializer
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="object">源对象</param>
        /// <returns></returns>
        string SerializeObject(object @object);

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <param name="object">源对象</param>
        /// <returns></returns>
        string Serialize<T>(T @object);

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="xml">源字符串</param>
        /// <returns></returns>
        T Deserialize<T>(string xml);

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="xml">源字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        object DeserializeObject(string xml, Type targetType);
    }
}