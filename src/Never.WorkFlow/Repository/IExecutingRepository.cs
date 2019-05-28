using Never.WorkFlow.Coordinations;
using System;

namespace Never.WorkFlow.Repository
{
    /// <summary>
    /// 执行任务仓库
    /// </summary>
    public interface IExecutingRepository
    {
        /// <summary>
        /// 创建任务节点
        /// </summary>
        int Save(TaskschedNode node, ExecutingNode[] nodes);

        /// <summary>
        /// 获取某个任务节点信息
        /// </summary>
        TaskschedNode Get(Guid taskId);

        /// <summary>
        /// 完成任务节点
        /// </summary>
        int Finish(TaskschedNode task);

        /// <summary>
        /// 完成任务节点
        /// </summary>
        int Abort(TaskschedNode task, ExecutingNode[] nodes);

        /// <summary>
        /// 完成任务节点
        /// </summary>
        int Done(TaskschedNode task, ExecutingNode node);

        /// <summary>
        /// 成功
        /// </summary>
        int AllDone(TaskschedNode task, ExecutingNode node);

        /// <summary>
        /// 失败
        /// </summary>
        int Exception(TaskschedNode task, ExecutingNode node);

        /// <summary>
        /// 标识下一次工作，消息为空的情况下
        /// </summary>
        int NextWork(TaskschedNode task, ExecutingNode node);

        /// <summary>
        /// 查询所有的任务
        /// </summary>
        /// <param name="statusArray"></param>
        /// <returns></returns>
        TaskschedNode[] GetAll(TaskschedStatus[] statusArray);

        /// <summary>
        /// 查询所有的任务
        /// </summary>
        TaskschedNode[] GetAll(TaskschedStatus[] statusArray, string commandType);

        /// <summary>
        /// 查询某标识下的所有的任务
        /// </summary>
        TaskschedNode[] GetAll(string usersign, TaskschedStatus[] statusArray, DateTime? startTime = null);

        /// <summary>
        /// 查询某标识下的所有的任务
        /// </summary>
        TaskschedNode[] GetAll(string usersign, string commandType, TaskschedStatus[] statusArray, DateTime? startTime = null);

        /// <summary>
        /// 查询某一的任务所有的某步骤运行情况
        /// </summary>
        ExecutingNode[] GetAll(Guid taskId, bool withAllMessage = false);
    }
}