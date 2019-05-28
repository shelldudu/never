using Never.Serialization.Json;
using Never.Serialization.Json.Deserialize;
using Never.Serialization.Json.Serialize;
using System;
using System.Collections;
using System.IO;

namespace Never.Serialization
{
    /// <summary>
    /// Easy json，配置<see cref="Never.Serialization.Json.DefaultSetting"/>默认一些设定
    /// </summary>
    public sealed class EasyJsonSerializer : IJsonSerializer
    {
        #region IJsonSerializer

        T IJsonSerializer.Deserialize<T>(string json)
        {
            return Deserialize<T>(json);
        }

        object IJsonSerializer.DeserializeObject(string json, Type targetType)
        {
            return DeserializeObject(json, targetType);
        }

        string IJsonSerializer.Serialize<T>(T @object)
        {
            return Serialize(@object);
        }

        string IJsonSerializer.SerializeObject(object @object)
        {
            return SerializeObject(@object);
        }

        #endregion IJsonSerializer

        #region deserializer

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="json">源字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public static object DeserializeObject(string json, Type targetType)
        {
            return Deserialize(json, targetType, DefaultSetting.DefaultDeserializeSetting);
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="json">源字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="setting">配置</param>
        /// <returns></returns>
        public static object Deserialize(string json, Type targetType, JsonDeserializeSetting setting)
        {
            /*object类*/
            if (targetType == typeof(object))
            {
                throw new ArgumentNullException("当前不支持object反序列化");
            }

            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            if (json.Equals("null"))
            {
                return null;
            }

            if (targetType == null)
                return null;

            var @delegate = QueryDeserializeBuilder(targetType, setting);
            var reader = ThunderReader.Load(json);
            {
                return @delegate(reader, setting, 0);
            }
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="json">源字符串</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            return Deserialize<T>(json, DefaultSetting.DefaultDeserializeSetting);
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="json">源字符串</param>
        /// <param name="setting">配置</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json, JsonDeserializeSetting setting)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }

            if (json.Equals("null"))
            {
                return default(T);
            }

            var reader = ThunderReader.Load(json);
            {
                var build = ZzzZzDeserializerBuilder<T>.Register(setting);
                return build.Invoke(reader, setting, 0);
            }
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="reader">读取器</param>
        /// <returns></returns>
        public static T Deserialize<T>(IDeserializerReader reader)
        {
            return Deserialize<T>(reader, DefaultSetting.DefaultDeserializeSetting);
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="reader">读取器</param>
        /// <param name="setting">配置</param>
        /// <returns></returns>
        public static T Deserialize<T>(IDeserializerReader reader, JsonDeserializeSetting setting)
        {
            reader.Reset();
            var build = ZzzZzDeserializerBuilder<T>.Register(setting);
            return build.Invoke(reader, setting, 0);
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="reader">读取器</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public static object DeserializeObject(IDeserializerReader reader, Type targetType)
        {
            return Deserialize(reader, targetType, DefaultSetting.DefaultDeserializeSetting);
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="reader">读取器</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="setting">配置</param>
        /// <returns></returns>
        public static object Deserialize(IDeserializerReader reader, Type targetType, JsonDeserializeSetting setting)
        {
            if (targetType == null)
                return null;

            var @delegate = QueryDeserializeBuilder(targetType, setting);
            reader.Reset();

            return @delegate(reader, setting, 0);
        }

        /// <summary>
        /// 执行构建序列化,反射用到的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="setting"></param>
        /// <param name="arrayLevel"></param>
        /// <returns></returns>
        private static object DeserializeBuilder<T>(IDeserializerReader reader, JsonDeserializeSetting setting, int arrayLevel)
        {
            return ZzzZzDeserializerBuilder<T>.Register(setting).Invoke(reader, setting, arrayLevel);
        }

        /// <summary>
        /// 查询执行者
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="setting"></param>
        /// <returns>返回可执行</returns>
        private static Func<IDeserializerReader, JsonDeserializeSetting, int, object> QueryDeserializeBuilder(Type sourceType, JsonDeserializeSetting setting)
        {
            var @delegate = DefaultSetting.DeserializeBuilderTable[sourceType] as Func<IDeserializerReader, JsonDeserializeSetting, int, object>;
            if (@delegate == null)
            {
                if (sourceType == typeof(object))
                {
                    DefaultSetting.DeserializeBuilderTable[sourceType] = @delegate = (x, y, l) => { return null; };
                }
                else
                {
                    DefaultSetting.DeserializeBuilderTable[sourceType] = @delegate = (Func<IDeserializerReader, JsonDeserializeSetting, int, object>)Delegate.CreateDelegate(typeof(Func<IDeserializerReader, JsonDeserializeSetting, int, object>), typeof(EasyJsonSerializer).GetMethod("DeserializeBuilder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).MakeGenericMethod(sourceType));
                }
            }

            return @delegate;
        }

        #endregion deserializer

        #region serializer

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="object">源对象</param>
        /// <returns></returns>
        public static string SerializeObject(object @object)
        {
            return SerializeObject(@object, DefaultSetting.DefaultSerializeSetting);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="object">源对象</param>
        /// <param name="setting">配置</param>
        /// <returns></returns>
        public static string SerializeObject(object @object, JsonSerializeSetting setting)
        {
            var writer = new ThunderWriter();
            {
                Serialize(@object, setting, writer);
                return writer.ToString();
            }
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object">源对象</param>
        /// <returns></returns>
        public static string Serialize<T>(T @object)
        {
            return Serialize<T>(@object, DefaultSetting.DefaultSerializeSetting);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object">源对象</param>
        /// <param name="setting">配置</param>
        /// <returns></returns>
        public static string Serialize<T>(T @object, JsonSerializeSetting setting)
        {
            var writer = new ThunderWriter();
            {
                Serialize(@object, setting, writer);
                return writer.ToString();
            }
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object">源对象</param>
        /// <param name="writer">写入流</param>
        public static void Serialize<T>(T @object, TextWriter writer)
        {
            Serialize(@object, writer, DefaultSetting.DefaultSerializeSetting);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object">源对象</param>
        /// <param name="setting">配置</param>
        /// <param name="writer">写入流</param>
        public static void Serialize<T>(T @object, TextWriter writer, JsonSerializeSetting setting)
        {
            Serialize(@object, setting, new TextWriterDecorate(writer));
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="object">源对象</param>
        /// <param name="writer">写入流</param>
        /// <returns></returns>
        public static void SerializeObject(object @object, TextWriter writer)
        {
            SerializeObject(@object, writer, DefaultSetting.DefaultSerializeSetting);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="object">源对象</param>
        /// <param name="setting">配置</param>
        /// <param name="writer">写入流</param>
        /// <returns></returns>
        public static void SerializeObject(object @object, TextWriter writer, JsonSerializeSetting setting)
        {
            Serialize(@object, setting, new TextWriterDecorate(writer));
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object">源对象</param>
        /// <param name="setting">配置</param>
        /// <param name="writer">写入流</param>
        /// <returns></returns>
        private static void Serialize<T>(T @object, JsonSerializeSetting setting, ISerializerWriter writer)
        {
            /*object*/
            if (typeof(T) == typeof(object))
            {
                Serialize((object)@object, setting, writer);
                return;
            }

            var build = ZzzZzSerialierBuilder<T>.Register(setting);
            build(writer, setting, @object, 0);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="object">源对象</param>
        /// <param name="setting">配置</param>
        /// <param name="writer">写入流</param>
        /// <returns></returns>
        private static void Serialize(object @object, JsonSerializeSetting setting, ISerializerWriter writer)
        {
            if (@object == null)
            {
                writer.Write("null");
                return;
            }

            var @delegate = SerialierBuilderHelper.QueryBuilderInvoker(@object.GetType());
            @delegate(writer, setting, @object, 0);
        }

        #endregion serializer
    }
}