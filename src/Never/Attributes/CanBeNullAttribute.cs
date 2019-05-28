using System;

namespace Never.Attributes
{
    /// <summary>
    /// 指示对象可以为空
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class CanBeNullAttribute : ParameterAttribute
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="CanBeNullAttribute"/> class.
        /// </summary>
        public CanBeNullAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanBeNullAttribute"/> class.
        /// </summary>
        /// <param name="name">参数名字</param>
        public CanBeNullAttribute(string name)
            : base(name)
        {
            this.Name = name;
        }

        #endregion ctor
    }
}