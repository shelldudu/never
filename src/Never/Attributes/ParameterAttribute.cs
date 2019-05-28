using System;

namespace Never.Attributes
{
    /// <summary>
    /// 参数特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = true)]
    public class ParameterAttribute : Attribute
    {
        #region property

        /// <summary>
        /// 参数名
        /// </summary>
        public string Name { get; set; }

        #endregion property

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterAttribute"/> class.
        /// </summary>
        public ParameterAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterAttribute"/> class.
        /// </summary>
        /// <param name="name">参数名字</param>
        public ParameterAttribute(string name)
        {
            this.Name = name;
        }

        #endregion ctor
    }
}