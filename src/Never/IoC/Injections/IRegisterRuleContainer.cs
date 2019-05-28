using System;
using System.Collections.Generic;

namespace Never.IoC.Injections
{
    /// <summary>
    /// 规则容器
    /// </summary>
    public interface IRegisterRuleContainer : IRegisterRuleQuery, IRegisterRuleChangeable
    {
        /// <summary>
        /// 规则库
        /// </summary>
        IEnumerable<RegisterRule> Rules { get; }

        /// <summary>
        /// 构建信息
        /// </summary>
        /// <param name="rules">规则信息</param>
        /// <param name="scope">范围参数</param>
        /// <param name="context">构建行为在执行过程的上下文</param>
        /// <returns></returns>
        object[] ResolveAll(RegisterRule[] rules, ILifetimeScope scope, IResolveContext context);

        /// <summary>
        /// 构建信息
        /// </summary>
        /// <param name="rule">规则信息</param>
        /// <param name="scope">范围参数</param>
        /// <param name="context">构建行为在执行过程的上下文</param>
        /// <returns></returns>
        object ResolveRule(RegisterRule rule, ILifetimeScope scope, IResolveContext context);

        /// <summary>
        /// 构建信息
        /// </summary>
        /// <param name="rule">规则信息</param>
        /// <param name="scope">范围参数</param>
        /// <param name="context">构建行为在执行过程的上下文</param>
        /// <returns></returns>
        object OptionalResolveRule(RegisterRule rule, ILifetimeScope scope, IResolveContext context);
    }
}