using System;

namespace Never.Attributes
{
    /// <summary>
    /// 指示字符串不可以为空
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class NotNullOrEmptyAttribute : ParameterAttribute
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullOrEmptyAttribute"/> class.
        /// </summary>
        public NotNullOrEmptyAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullOrEmptyAttribute"/> class.
        /// </summary>
        /// <param name="name">参数名字</param>
        public NotNullOrEmptyAttribute(string name)
            : base(name)
        {
            this.Name = name;
        }

        #endregion ctor
    }
}