using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Never.DataAnnotations
{
    /// <summary>
    /// 验证接口
    /// </summary>
    public interface IAmValidator
    {
        /// <summary>
        /// 验证某一对象
        /// </summary>
        /// <returns></returns>
        ValidationResult Validate();
    }
}