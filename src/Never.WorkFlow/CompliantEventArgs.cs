using System;
using System.Collections.Generic;

namespace Never.WorkFlow
{
    /// <summary>
    /// 兼容性测试事件
    /// </summary>
    public sealed class CompliantEventArgs : EventArgs
    {
        #region ctor

        /// <summary>
        ///
        /// </summary>
        public CompliantEventArgs()
        {
            this.collections = new Dictionary<Type, KeyValuePair<IWorkStepMessage, Func<Type, IWorkStepMessage, bool>>>(10);
        }

        #endregion ctor

        #region field

        /// <summary>
        /// 测试列表
        /// </summary>
        internal readonly Dictionary<Type, KeyValuePair<IWorkStepMessage, Func<Type, IWorkStepMessage, bool>>> collections = null;

        #endregion field

        #region register

        /// <summary>
        /// 注册测试兼容性事件
        /// </summary>
        /// <typeparam name="TWorkStep">工作流步骤</typeparam>
        /// <param name="produceMessage">当前步骤最后产生的消息</param>
        /// <param name="testWithPreWorkStep">与上一步骤产生的结果去测试是否兼容</param>
        public void Register<TWorkStep>(IWorkStepMessage produceMessage, Func<Type, IWorkStepMessage, bool> testWithPreWorkStep) where TWorkStep : IWorkStep
        {
            this.collections[typeof(TWorkStep)] = new KeyValuePair<IWorkStepMessage, Func<Type, IWorkStepMessage, bool>>(produceMessage, testWithPreWorkStep);
        }

        /// <summary>
        /// 注册测试兼容性事件
        /// </summary>
        /// <param name="workStepType"></param>
        /// <param name="message"></param>
        /// <param name="test"></param>
        public void Register(Type workStepType, IWorkStepMessage message, Func<Type, IWorkStepMessage, bool> test)
        {
            this.collections[workStepType] = new KeyValuePair<IWorkStepMessage, Func<Type, IWorkStepMessage, bool>>(message, test);
        }

        #endregion register
    }
}