using System;

namespace Never.Attributes
{
    /// <summary>
    /// 参数长度限定
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class ParameterLengthAttribute : ParameterAttribute
    {
        #region property

        /// <summary>
        /// 最大长度
        /// </summary>
        public int Length { get; set; }

        #endregion property

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterLengthAttribute"/> class.
        /// </summary>
        public ParameterLengthAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterLengthAttribute"/> class.
        /// </summary>
        /// <param name="name">参数名字</param>
        /// <param name="length">长度</param>
        public ParameterLengthAttribute(string name, int length)
            : base(name)
        {
            this.Name = name;
            this.Length = length;
        }

        #endregion ctor
    }
}