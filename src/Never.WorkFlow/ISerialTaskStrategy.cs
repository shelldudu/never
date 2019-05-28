using System;

namespace Never.WorkFlow
{
    /// <summary>
    /// 并发任务执行策略
    /// </summary>
    public interface ISerialTaskStrategy
    {
        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="callback">回调</param>
        /// <returns></returns>
        void LockAndInvoke(ITaskschedNode task, Action callback);
    }
}