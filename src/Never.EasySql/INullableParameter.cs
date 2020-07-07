using System;
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

    internal class StructNullableParameter<T> : INullableParameter, IGenericeNullableParameter<T> where T : struct
    {
        private readonly T? value = null;

        public StructNullableParameter(T? value)
        {
            this.value = value;
        }

        public bool HasValue => this.value.HasValue;

        public object Value => this.value.HasValue ? (object)this.value.Value : null;

        public T TValue => this.value.HasValue ? this.value.Value : default(T);
    }

    internal class StringNullableParameter : INullableParameter, IGenericeNullableParameter<string>
    {
        private readonly string value = null;

        public StringNullableParameter(string value)
        {
            this.value = value;
        }

        public bool HasValue => this.value.IsNotNullOrEmpty();

        public object Value => this.value.IsNotNullOrEmpty() ? this.value : null;

        public string TValue => this.value.IsNotNullOrEmpty() ? this.value : null;
    }

    internal class GuidNullableParameter : INullableParameter, IGenericeNullableParameter<Guid>
    {
        private readonly Guid value = Guid.Empty;

        public GuidNullableParameter(Guid value)
        {
            this.value = value;
        }

        public bool HasValue => this.value != Guid.Empty;

        public object Value => this.value;

        public Guid TValue => this.value;
    }

    internal class EnumerableNullableParameter<T> : INullableParameter, IGenericeNullableParameter<IEnumerable<T>>
    {
        private readonly IEnumerable<T> value = null;

        public EnumerableNullableParameter(IEnumerable<T> value)
        {
            this.value = value;
        }

        public bool HasValue => this.value != null && this.value.Any();

        public object Value => this.value;

        public IEnumerable<T> TValue => this.value;
    }
}