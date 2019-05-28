using Never;
using Never.Exceptions;
using Never.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Never.Serialization
{
    /// <summary>
    /// 序列化环境
    /// </summary>
    public static class SerializeEnvironment
    {
        #region prop

        /// <summary>
        /// 当前json序列化接口
        /// </summary>
        public static IJsonSerializer JsonSerializer
        {
            get
            {
                return Singleton<IJsonSerializer>.Instance ?? new Never.Serialization.EasyJsonSerializer();
            }
        }

        /// <summary>
        /// 当前二进制序列化接口
        /// </summary>
        public static IBinarySerializer BinarySerializer
        {
            get
            {
                return Singleton<IBinarySerializer>.Instance ?? new Never.Serialization.BinarySerializer();
            }
        }

        /// <summary>
        /// 当前xml序列化接口
        /// </summary>
        public static IXmlSerializer XmlSerializer
        {
            get
            {
                return Singleton<IXmlSerializer>.Instance ?? new Never.Serialization.XmlSerializer();
            }
        }

        #endregion prop

        #region ctor

        static SerializeEnvironment()
        {
        }

        #endregion ctor

        #region replace

        /// <summary>
        /// 更新json序列化接口
        /// </summary>
        /// <param name="jsonSerializer">json序列化接口</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IJsonSerializer SetSerializer(IJsonSerializer jsonSerializer)
        {
            if (jsonSerializer == null)
            {
                throw new ParameterNullException("jsonSerializer", "jsonSerializer接口不能为空");
            }

            Singleton<IJsonSerializer>.Instance = jsonSerializer;
            return jsonSerializer;
        }

        /// <summary>
        /// 更新二进制序列化接口
        /// </summary>
        /// <param name="binarySerializer">二进制序列化接口</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IBinarySerializer SetSerializer(IBinarySerializer binarySerializer)
        {
            if (binarySerializer == null)
            {
                throw new ParameterNullException("binarySerializer", "binarySerializer接口不能为空");
            }

            Singleton<IBinarySerializer>.Instance = binarySerializer;
            return binarySerializer;
        }

        /// <summary>
        /// 更新二进制序列化接口
        /// </summary>
        /// <param name="xmlSerializer">xml序列化接口</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IXmlSerializer SetSerializer(IXmlSerializer xmlSerializer)
        {
            if (xmlSerializer == null)
            {
                throw new ParameterNullException("xmlSerializer", "xmlSerializer接口不能为空");
            }

            Singleton<IXmlSerializer>.Instance = xmlSerializer;
            return xmlSerializer;
        }

        #endregion replace
    }
}