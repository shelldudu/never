using System;

namespace Never.Attributes
{
    /// <summary>
    /// 指示对象不可以为空
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class NotNullAttribute : ParameterAttribute
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullAttribute"/> class.
        /// </summary>
        public NotNullAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullAttribute"/> class.
        /// </summary>
        /// <param name="name">参数名字</param>
        public NotNullAttribute(string name)
            : base(name)
        {
            this.Name = name;
        }

        #endregion ctor
    }
}