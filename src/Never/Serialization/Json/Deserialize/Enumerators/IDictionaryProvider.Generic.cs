using System;
using System.Collections.Generic;

namespace Never.Serialization.Json.Deserialize.Enumerators
{
    /// <summary>
    /// 字典类型
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class IDictionaryProvider<TKey, TValue>
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimitiveEnumerableProvider{T}"/> class.
        /// </summary>
        public IDictionaryProvider()
        {
        }

        #endregion ctor

        #region IMethodProvider

        /// <summary>
        /// 写入流中
        /// </summary>
        /// <param name="result"></param>
        /// <param name="name">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="reader">The array.</param>
        public virtual void Parse(IDictionary<TKey, TValue> result, IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
        }

        #endregion IMethodProvider

        #region builder

        /// <summary>
        /// 接受char类型的反序列化
        /// </summary>
        public class StringDeseralizerBuilder<TKeyT> : JsonEmitBuilder<TKey>
        {
            #region field

            /// <summary>
            /// 委托
            /// </summary>
            private static Func<string, TKey> function = null;

            #endregion field

            #region ctor

            /// <summary>
            /// Initializes a new instance of the <see cref="StringDeseralizerBuilder{TKey}"/> class.
            /// </summary>
            public StringDeseralizerBuilder()
            {
            }

            #endregion ctor

            #region static build

            /// <summary>
            /// 进行构建
            /// </summary>
            /// <returns></returns>
            public static Func<string, TKey> Register()
            {
                if (function != null)
                    return function;

                function = new StringDeseralizerBuilder<TKey>().Build();
                return function;
            }

            #endregion static build

            #region SerialierBuilder

            /// <summary>
            /// 进行构建
            /// </summary>
            /// <returns></returns>
            public Func<string, TKey> Build()
            {
                return null;
            }

            #endregion SerialierBuilder
        }

        #endregion builder
    }
}