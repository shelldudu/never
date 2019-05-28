using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never
{
    /// <summary>
    /// 选择接口
    /// </summary>
    /// <typeparam name="TOptin"></typeparam>
    public interface IValuableOption<out TOptin>
    {
        /// <summary>
        ///  TOptin对象实例
        /// </summary>
        TOptin Value { get; }
    }
}