using System;

namespace Never.IoC.Injections
{
    /// <summary>
    /// 规则更新
    /// </summary>
    public interface IRegisterRuleChangeable
    {
        /// <summary>
        /// 更新容器规则
        /// </summary>
        /// <param name="rule"></param>
        void Update(RegisterRuleCollector rule);

        /// <summary>
        /// 创建规则，如果已经构建，则该规则不会被加入到规则集合中
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        RegisterRule CreateRule(Type serviceType);
    }
}