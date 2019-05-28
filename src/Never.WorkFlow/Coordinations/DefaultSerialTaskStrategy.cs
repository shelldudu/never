using System;

namespace Never.WorkFlow.Coordinations
{
    /// <summary>
    /// 并发任务执行策略，当前使用锁定taskId策略
    /// </summary>
    public class DefaultSerialTaskStrategy : ISerialTaskStrategy
    {
        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="callback">回调</param>
        /// <returns></returns>
        public void LockAndInvoke(ITaskschedNode task, Action callback)
        {
            if (task == null)
            {
                if (callback != null)
                    callback.Invoke();

                return;
            }

            lock (task.TaskId.ToString())
            {
                if (callback != null)
                    callback.Invoke();

                return;
            }
        }
    }
}