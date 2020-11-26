using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 可空类型
    /// </summary>
    public interface INullableParameter
    {
        /// <summary>
        /// 是否有值,如果是Guid类型的，则跟Guid.Empty比较，如果是string类型的，则与string.Empty比较，如果是可空类型的int?，则看是否有值 ，如果是数组的，则看数组是否可有长度
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// 值
        /// </summary>
        object Value { get; }
    }

    /// <summary>
    /// 可空类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericeNullableParameter<T> : INullableParameter
    {
        /// <summary>
        /// 值
        /// </summary>
        T TValue { get; }
    }

    /// <summary>
    /// 值对象的可空类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StructNullableParameter<T> : INullableParameter, IGenericeNullableParameter<T> where T : struct
    {
        private readonly T? value = null;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value"></param>
        public StructNullableParameter(T? value)
        {
            this.value = value;
        }

        /// <summary>
        /// 是否有值
        /// </summary>
        public bool HasValue => this.value.HasValue;

        /// <summary>
        /// 值
        /// </summary>
        public object Value => this.value.HasValue ? (object)this.value.Value : null;

        /// <summary>
        /// 值
        /// </summary>
        public T TValue => this.value.HasValue ? this.value.Value : default(T);

    }

    /// <summary>
    /// 字符串对象的可空类型
    /// </summary>
    public class StringNullableParameter : INullableParameter, IGenericeNullableParameter<string>
    {
        private readonly string value = null;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value"></param>
        public StringNullableParameter(string value)
        {
            this.value = value;
        }
        /// <summary>
        /// 是否有值
        /// </summary>
        public bool HasValue => this.value.IsNotNullOrEmpty();
        /// <summary>
        /// 值
        /// </summary>
        public object Value => this.value.IsNotNullOrEmpty() ? this.value : null;
        /// <summary>
        /// 值
        /// </summary>
        public string TValue => this.value.IsNotNullOrEmpty() ? this.value : null;
    }

    /// <summary>
    /// Guid对象的可空类型
    /// </summary>
    public class GuidNullableParameter : INullableParameter, IGenericeNullableParameter<Guid>
    {
        private readonly Guid value = Guid.Empty;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value"></param>
        public GuidNullableParameter(Guid value)
        {
            this.value = value;
        }
        /// <summary>
        /// 是否有值
        /// </summary>
        public bool HasValue => this.value != Guid.Empty;
        /// <summary>
        /// 值
        /// </summary>
        public object Value => this.value;
        /// <summary>
        /// 值
        /// </summary>
        public Guid TValue => this.value;
    }

    /// <summary>
    /// 可遍历对象的可空类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumerableNullableParameter<T> : INullableParameter, IGenericeNullableParameter<IEnumerable<T>>, IEnumerable<T>
    {
        private readonly IEnumerable<T> value = null;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value"></param>
        public EnumerableNullableParameter(IEnumerable<T> value)
        {
            this.value = value;
        }
        /// <summary>
        /// 是否有值
        /// </summary>
        public bool HasValue => this.value != null && this.value.Any();
        /// <summary>
        /// 值
        /// </summary>
        public object Value => this.value;
        /// <summary>
        /// 值
        /// </summary>
        public IEnumerable<T> TValue => this.value;

        /// <summary>
        /// get enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (this.HasValue)
                return this.value.GetEnumerator();

            return Enumerable.Empty<T>().GetEnumerator();
        }

        /// <summary>
        /// get enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (this.HasValue)
                return this.value.GetEnumerator();

            return Enumerable.Empty<T>().GetEnumerator();
        }
    }
}