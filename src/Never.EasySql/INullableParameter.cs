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
    }

    /// <summary>
    /// 可空类型
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface IReferceNullableParameter : INullableParameter
    {
        /// <summary>
        /// 值
        /// </summary>
        object Value { get; }
    }

    /// <summary>
    /// 可空类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface IGenericeNullableParameter<T> : INullableParameter
    {
        /// <summary>
        /// 值
        /// </summary>
        T Value { get; }
    }

    internal class StructNullableParameter<T> : INullableParameter, IReferceNullableParameter, IGenericeNullableParameter<T> where T : struct
    {
        private readonly T? value = null;

        public StructNullableParameter(T? value)
        {
            this.value = value;
        }

        bool INullableParameter.HasValue => this.value.HasValue;

        object IReferceNullableParameter.Value => this.value.HasValue ? (object)this.value.Value : null;

        T IGenericeNullableParameter<T>.Value => this.value.HasValue ? this.value.Value : default(T);
    }

    internal class StringNullableParameter : INullableParameter, IReferceNullableParameter, IGenericeNullableParameter<string>
    {
        private readonly string value = null;

        public StringNullableParameter(string value)
        {
            this.value = value;
        }

        bool INullableParameter.HasValue => this.value.IsNotNullOrEmpty();

        object IReferceNullableParameter.Value => this.value.IsNotNullOrEmpty() ? this.value : null;

        string IGenericeNullableParameter<string>.Value => this.value.IsNotNullOrEmpty() ? this.value : null;
    }

    internal class GuidNullableParameter : INullableParameter, IReferceNullableParameter, IGenericeNullableParameter<Guid>
    {
        private readonly Guid value;

        public GuidNullableParameter(Guid value)
        {
            this.value = value;
        }

        bool INullableParameter.HasValue => this.value != Guid.Empty;

        object IReferceNullableParameter.Value => this.value != Guid.Empty ? this.value : Guid.Empty;

        Guid IGenericeNullableParameter<Guid>.Value => this.value != Guid.Empty ? this.value : Guid.Empty;
    }

    internal class EnumerableNullableParameter<T> : INullableParameter, IReferceNullableParameter, IGenericeNullableParameter<IEnumerable<T>>
    {
        private readonly IEnumerable<T> value = null;

        public EnumerableNullableParameter(IEnumerable<T> value)
        {
            this.value = value;
        }

        bool INullableParameter.HasValue => this.value != null && this.value.Any();

        object IReferceNullableParameter.Value => this.value;

        IEnumerable<T> IGenericeNullableParameter<IEnumerable<T>>.Value => this.value;
    }
}