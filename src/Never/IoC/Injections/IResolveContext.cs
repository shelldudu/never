using Never.Caching;
using System;

namespace Never.IoC.Injections
{
    /// <summary>
    /// 构建行为在执行过程的上下文
    /// </summary>
    public interface IResolveContext
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="scope"></param>
        T Query<T>(IRegisterRuleDescriptor rule, ILifetimeScope scope);

        /// <summary>
        /// 缓存
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="scope"></param>
        /// <param name="object"></param>
        /// <returns></returns>
        T Cache<T>(IRegisterRuleDescriptor rule, ILifetimeScope scope, T @object);
    }
}