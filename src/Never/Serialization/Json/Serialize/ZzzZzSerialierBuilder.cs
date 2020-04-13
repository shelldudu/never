using Never.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Never.Serialization.Json.Serialize
{
    /// <summary>
    /// json serializer builder
    /// </summary>
    public class ZzzZzSerialierBuilder<T> : SerialierBuilder<T>, ISerialierBuilder<T>
    {
        #region field  and ctor

        private readonly static ConcurrentDictionary<JsonSerializeSetting, Action<ISerializerWriter, JsonSerializeSetting, T, byte>> caching = null;

        static ZzzZzSerialierBuilder()
        {
            caching = new ConcurrentDictionary<JsonSerializeSetting, Action<ISerializerWriter, JsonSerializeSetting, T, byte>>(new JsonSerializeSetting());
        }

        #endregion field  and ctor

        #region static build

        /// <summary>
        /// 创建委托
        /// </summary>
        /// <returns></returns>
        public static Action<ISerializerWriter, JsonSerializeSetting, T, byte> Register(JsonSerializeSetting setting)
        {
            Action<ISerializerWriter, JsonSerializeSetting, T, byte> action = null;
            if (caching.TryGetValue(setting, out action))
                return action;

            lock (caching)
            {
                if (caching.TryGetValue(setting, out action))
                    return action;

                var customSerialierBuilder = CustomSerializationProvider.QueryCustomeSerilizerbuilder<T>();
                if (customSerialierBuilder != null)
                {
                    action = customSerialierBuilder.Build(setting);
                    caching.TryAdd(setting, action);
                    return action;
                }

                action = new ZzzZzSerialierBuilder<T>().Build(setting);
                caching.TryAdd(setting, action);
                return action;
            }
        }

        #endregion static build

        #region build

        /// <summary>
        /// 进行构建
        /// </summary>
        public Action<ISerializerWriter, JsonSerializeSetting, T, byte> Build(JsonSerializeSetting setting)
        {
            var emit = EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>>.NewDynamicMethod();
            if (!IsComplexType(this.TargetType) || this.IsType(this.TargetType) || this.IsAssignableFrom(this.TargetType, typeof(Exception)) || this.IsAssignableFrom(this.TargetType, typeof(JsonObject)))
            {
                this.BuildNotCepaType(emit, setting, this.TargetType);
                emit.Return();
                return emit.CreateDelegate();
            }

            //this.WriteObjectFrontSigil(emit);
            this.Build(emit, setting);
            // this.WriteObjectLastSigil(emit);

            emit.Return();
            return emit.CreateDelegate();
        }

        #endregion build
    }
}