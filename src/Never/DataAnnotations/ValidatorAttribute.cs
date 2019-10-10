using System;

namespace Never.DataAnnotations
{
    /// <summary>
    /// 数组验证模型特性<see cref="ValidatorAttribute.ValidatorType"/>类型通常是实现了<see cref="IValidator"/>接口
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public class ValidatorAttribute : Attribute
    {
        #region prop

        /// <summary>
        /// 验证器类型
        /// </summary>
        public Type ValidatorType { get; set; }

        #endregion prop

        #region ctor

        /// <summary>
        ///
        /// </summary>
        public ValidatorAttribute()
            : this(null)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="validatorType"></param>
        public ValidatorAttribute(Type validatorType)
        {
            this.ValidatorType = validatorType;
        }

        #endregion ctor
    }
}