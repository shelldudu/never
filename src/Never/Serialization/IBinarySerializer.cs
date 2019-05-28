using System;

namespace Never.Serialization
{
    /// <summary>
    /// binary序列化接口
    /// </summary>
    public interface IBinarySerializer
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="graph">要序列化的对象或对象图形的根。将自动序列化此根对象的所有子对象。</param>
        /// <returns></returns>
        byte[] SerializeObject(object graph);

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <param name="graph">要序列化的对象或对象图形的根。将自动序列化此根对象的所有子对象。</param>
        /// <returns></returns>
        byte[] Serialize<T>(T graph);

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="buffer">源字符串</param>
        /// <returns></returns>
        T Deserialize<T>(byte[] buffer);

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="buffer">源字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        object DeserializeObject(byte[] buffer, Type targetType);
    }
}