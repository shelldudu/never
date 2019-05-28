using System;
using System.Collections.Generic;

namespace Never.IoC.Injections
{
    /// <summary>
    /// 所有的注册规则集合
    /// </summary>
    public interface IRegisterRuleQuery
    {
        /// <summary>
        /// 查询某个规则，会强制匹配Key的一致性，并且会检查兼容性
        /// </summary>
        /// <param name="serviceType">对象Type</param>
        /// <param name="key">注册的Key</param>
        /// <param name="topRule">最顶层的注册规则</param>
        /// <returns></returns>
        RegisterRule FreedomQuery(Type serviceType, string key, RegisterRule topRule);

        /// <summary>
        /// 查询某个规则，会强制匹配Key的一致性，并且不会检查兼容性
        /// </summary>
        /// <param name="serviceType">对象Type</param>
        /// <param name="key">注册的Key</param>
        /// <returns></returns>
        IEnumerable<RegisterRule> StrictQuery(Type serviceType, string key);

        /// <summary>
        /// 查询规则
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="key"></param>
        /// <param name="throw"></param>
        /// <returns></returns>
        RegisterRule QueryRule(Type serviceType, string key, bool @throw);

        /// <summary>
        /// 查询规则
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        RegisterRule[] QueryAllRule(Type serviceType);
    }
}