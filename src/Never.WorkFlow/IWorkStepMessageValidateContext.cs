using System;
using System.Linq.Expressions;

namespace Never.WorkFlow
{
    /// <summary>
    /// 消息验证上下文
    /// </summary>
    public interface IWorkStepMessageValidateContext
    {
        /// <summary>
        /// 参数验证规则
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="express"></param>
        /// <param name="message"></param>
        void ParamaterRule<T>(Expression<System.Func<T, object>> express, string message) where T : IWorkStepMessage;
    }
}