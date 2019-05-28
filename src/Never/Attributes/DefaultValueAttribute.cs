using System;

namespace Never.Attributes
{
    /// <summary>
    /// 默认值
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class DefaultValueAttribute : ParameterAttribute
    {
        #region property

        /// <summary>
        /// 默认值
        /// </summary>
        public object Value { get; set; }

        #endregion property

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValueAttribute"/> class.
        /// </summary>
        public DefaultValueAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValueAttribute"/> class.
        /// </summary>
        /// <param name="name">参数名字</param>
        /// <param name="value">默认值</param>
        public DefaultValueAttribute(string name, object value)
            : base(name)
        {
            this.Name = name;
            this.Value = value;
        }

        #endregion ctor
    }
}