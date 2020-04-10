using Never.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Never.Serialization.Json.Deserialize
{
    /// <summary>
    /// 反序列化构建
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ZzzZzDeserializerBuilder<T> : DeseralizerBuilder<T>, IDeserialierBuilder<T>
    {
        #region field and ctor

        private readonly static ConcurrentDictionary<JsonDeserializeSetting, Func<IDeserializerReader, JsonDeserializeSetting, int, T>> caching = null;

        static ZzzZzDeserializerBuilder()
        {
            caching = new ConcurrentDictionary<JsonDeserializeSetting, Func<IDeserializerReader, JsonDeserializeSetting, int, T>>(new JsonDeserializeSetting());
        }

        #endregion field and ctor

        #region static build

        /// <summary>
        /// 创建委托
        /// </summary>
        /// <returns></returns>
        public static Func<IDeserializerReader, JsonDeserializeSetting, int, T> Register(JsonDeserializeSetting setting)
        {
            Func<IDeserializerReader, JsonDeserializeSetting, int, T> func = null;
            if (caching.TryGetValue(setting, out func))
                return func;

            lock (caching)
            {
                if (caching.TryGetValue(setting, out func))
                    return func;

                var customSerialierBuilder = CustomSerializationProvider.QueryCustomeDeserilizerbuilder<T>();
                if (customSerialierBuilder != null)
                {
                    func = customSerialierBuilder.Build(setting);
                    caching.TryAdd(setting, func);
                    return func;
                }

                func = new ZzzZzDeserializerBuilder<T>().Build(setting);
                caching.TryAdd(setting, func);
                return func;
            }
        }

        #endregion static build

        #region build

        /// <summary>
        /// 进行构建
        /// </summary>
        public Func<IDeserializerReader, JsonDeserializeSetting, int, T> Build(JsonDeserializeSetting setting)
        {
            var emit = EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>>.NewDynamicMethod();
            if (!IsComplexType(this.TargetType) || this.IsType(this.TargetType) || this.IsAssignableFrom(this.TargetType, typeof(Exception)))
            {
                this.BuildNotCepaType(emit, this.TargetType);
                emit.Return();
                return emit.CreateDelegate();
            }

            this.Build(emit);
            emit.Return();
            return emit.CreateDelegate();
        }

        #endregion build
    }
}