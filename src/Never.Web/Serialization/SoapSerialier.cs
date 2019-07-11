#if !NET461
#else

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Never.Web.Serialization
{
    /// <summary>
    /// soap 对象序列化
    /// </summary>
    public struct SoapSerialier
    {
        #region 序列化

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="graph">源数据</param>
        /// <returns></returns>
        public byte[] SerializeObject(object graph)
        {
            if (graph == null)
            {
                return null;
            }

            var format = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
            using (var st = new MemoryStream())
            {
                format.Serialize(st, graph);
                return st.ToArray();
            }
        }

        #endregion 序列化

        #region 反序列化

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="bytes">源数据</param>
        /// <returns></returns>
        public T Deserialize<T>(byte[] bytes)
        {
            if (bytes == null)
            {
                return default(T);
            }

            var format = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
            using (var st = new MemoryStream(bytes))
            {
                return (T)format.Deserialize(st);
            }
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="bytes">源数据</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public object Deserialize(byte[] bytes, Type targetType)
        {
            if (bytes == null)
            {
                return null;
            }

            var format = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
            using (var st = new MemoryStream(bytes))
            {
                return format.Deserialize(st);
            }
        }

        #endregion 反序列化
    }
}

#endif