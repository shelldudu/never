using Never.IoC.Injections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.IoC
{
    /// <summary>
    /// 生命周期存储器
    /// </summary>
    public interface IComponentLifeStyleStorager
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rule"></param>
        /// <returns></returns>
        T Query<T>(IRegisterRuleDescriptor rule);

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rule"></param>
        /// <param name="object"></param>
        /// <returns></returns>
        T Cache<T>(IRegisterRuleDescriptor rule, T @object);
    }
}