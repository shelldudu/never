using System;
using System.Collections.Generic;

namespace Never.Serialization.Json
{
    /// <summary>
    /// 用户自定义序列化一些行为注册
    /// </summary>
    public static class CustomSerializationProvider
    {
        #region dict

        /// <summary>
        /// 时间格式字典
        /// </summary>
        private static readonly IDictionary<DateTimeFormat, object> datetimeConvertMethod = null;

        /// <summary>
        /// 自定义类型序列化字典
        /// </summary>
        private static readonly IDictionary<Type, object> serailizerMethod = null;

        /// <summary>
        /// 自定义类型反序列化字典
        /// </summary>
        private static readonly IDictionary<Type, object> deserailizerMethod = null;

        #endregion dict

        #region ctor

        /// <summary>
        /// Initializes static members of the <see cref="CustomSerializationProvider"/> class.
        /// </summary>
        static CustomSerializationProvider()
        {
            datetimeConvertMethod = new Dictionary<DateTimeFormat, object>(10);
            serailizerMethod = new Dictionary<Type, object>(10);
            deserailizerMethod = new Dictionary<Type, object>(10);
        }

        #endregion ctor

        #region datetime

        /// <summary>
        /// 注册时间类型的转换方法提供者
        /// </summary>
        /// <param name="format">时间格式化</param>
        /// <param name="methodProvider">转换方法提供者</param>
        public static void RegisterCustomDatetimeFormat(DateTimeFormat format, IConvertMethodProvider<DateTime?> methodProvider)
        {
            if (methodProvider == null)
                throw new ArgumentNullException("methodProvider");

            datetimeConvertMethod[format] = methodProvider;
        }

        /// <summary>
        /// 注册时间类型的转换方法提供者
        /// </summary>
        /// <param name="format">时间格式化</param>
        /// <param name="methodProvider">转换方法提供者</param>
        public static void RegisterCustomDatetimeFormat(DateTimeFormat format, IConvertMethodProvider<DateTime> methodProvider)
        {
            if (methodProvider == null)
                throw new ArgumentNullException("methodProvider");

            datetimeConvertMethod[format] = methodProvider;
        }

        /// <summary>
        /// 查询时间类型的转换方法提供者
        /// </summary>
        /// <param name="format">时间格式化</param>
        /// <returns>转换方法提供者</returns>
        public static object QueryCustomDatetimeFormat(DateTimeFormat format)
        {
            object target = null;
            datetimeConvertMethod.TryGetValue(format, out target);
            return target;
        }

        #endregion datetime

        #region serializer

        /// <summary>
        /// 注册某些类型自定义的序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serilizerbuilder">序列化</param>
        public static void RegisteCustomeSerilizerbuilder<T>(ISerialierBuilder<T> serilizerbuilder)
        {
            if (serilizerbuilder == null)
                throw new ArgumentNullException("serializer");

            serailizerMethod[typeof(T)] = serilizerbuilder;
        }

        /// <summary>
        /// 查询某些类型自定义的序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static ISerialierBuilder<T> QueryCustomeSerilizerbuilder<T>()
        {
            object serilizerbuilder = null;
            serailizerMethod.TryGetValue(typeof(T), out serilizerbuilder);
            return serilizerbuilder as ISerialierBuilder<T>;
        }

        /// <summary>
        /// 是否包含了某个自定义类型的序列化
        /// </summary>
        /// <param name="type">自定义类型</param>
        public static bool ContainCustomeSerilizerbuilder(Type type)
        {
            return serailizerMethod.ContainsKey(type);
        }

        #endregion serializer

        #region deserializer

        /// <summary>
        /// 注册某些类型自定义的反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deserilizerbuilder">序列化</param>
        public static void RegisteCustomeDeserilizerbuilder<T>(IDeserialierBuilder<T> deserilizerbuilder)
        {
            if (deserilizerbuilder == null)
                throw new ArgumentNullException("deserializer");

            deserailizerMethod[typeof(T)] = deserilizerbuilder;
        }

        /// <summary>
        /// 注册某些类型自定义的反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static IDeserialierBuilder<T> QueryCustomeDeserilizerbuilder<T>()
        {
            object desserilizerbuilder = null;
            deserailizerMethod.TryGetValue(typeof(T), out desserilizerbuilder);
            return desserilizerbuilder as IDeserialierBuilder<T>;
        }

        /// <summary>
        /// 是否包含了某个自定义类型的反序列化
        /// </summary>
        /// <param name="type">自定义类型</param>
        public static bool ContainCustomeDeserilizerbuilder(Type type)
        {
            return deserailizerMethod.ContainsKey(type);
        }

        #endregion deserializer
    }
}