using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.DataAnnotations
{
    /// <summary>
    /// 验证接口
    /// </summary>
    public interface IValidator<T>
    {
        /// <summary>
        /// 验证某一对象
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<Expression<System.Func<T, object>>, string>> Validate();
    }
}