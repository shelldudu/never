using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Never.DataAnnotations
{
    /// <summary>
    /// 验证接口
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// 验证某一对象
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        ValidationResult Validate(object target);
    }
}