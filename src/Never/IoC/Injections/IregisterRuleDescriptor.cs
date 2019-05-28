using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.IoC.Injections
{
    /// <summary>
    /// 规则描述
    /// </summary>
    public interface IRegisterRuleDescriptor
    {
        /// <summary>
        /// 实现者
        /// </summary>
        Type ImplementationType { get; }

        /// <summary>
        /// 目标类型
        /// </summary>
        Type ServiceType { get; }

        /// <summary>
        /// 注入key
        /// </summary>
        string Key { get; }

        /// <summary>
        /// 注册所使用到的参数
        /// </summary>
        KeyValueTuple<string, Type>[] Parameters { get; }

        /// <summary>
        /// 注册所使用到的参数个数
        /// </summary>
        int ParametersCount { get; }

        /// <summary>
        /// 生命周期
        /// </summary>
        ComponentLifeStyle LifeStyle { get; }

        /// <summary>
        /// 返回所描述对象的关键字，通常用在缓存中
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}