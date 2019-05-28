using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.DataAnnotations
{
    /// <summary>
    /// 验证选项
    /// </summary>
    public interface IValidationOption
    {
        /// <summary>
        /// 结果选项
        /// </summary>
        ValidationOption Option { get; }
    }
}
