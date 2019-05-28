using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.IoC.Injections
{
    /// <summary>
    /// 不再注册
    /// </summary>
    public struct UnableRegisterRule
    {
        /// <summary>
        /// 是否不可行了
        /// </summary>
        public bool Unabled { get; }

        /// <summary>
        /// 构造对象
        /// </summary>
        /// <param name="unabled"></param>
        public UnableRegisterRule(bool unabled)
        {
            this.Unabled = unabled;
        }
    }
}