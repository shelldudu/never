using Never.WorkFlow.Coordinations;
using System;

namespace Never.WorkFlow
{
    /// <summary>
    /// 工作开始引擎
    /// </summary>
    public interface ITaskschedQueue
    {
        /// <summary>
        /// 查询一个任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        TaskschedNode Get(Guid taskId);

        /// <summary>
        /// 查询用户标识的所有任务
        /// </summary>
        /// <param name="statusArray">状态</param>
        /// <returns></returns>
        TaskschedNode[] GetAll(TaskschedStatus[] statusArray);

        /// <summary>
        /// 查询用户标识的所有任务
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="statusArray">状态</param>
        /// <returns></returns>
        TaskschedNode[] GetAll(TaskschedStatus[] statusArray, string commandType);

        /// <summary>
        /// 查询用户标识的所有任务
        /// </summary>
        /// <param name="userSign">用户标识</param>
        /// <param name="statusArray">状态</param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        TaskschedNode[] GetAll(string userSign, TaskschedStatus[] statusArray, DateTime? startTime = null);

        /// <summary>
        /// 查询用户标识的所有任务
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="userSign">用户标识</param>
        /// <param name="statusArray">状态</param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        TaskschedNode[] GetAll(string userSign, string commandType, TaskschedStatus[] statusArray, DateTime? startTime = null);

        /// <summary>
        /// 将当前激发的消息压入队列中，开始进行按模板的步骤进行工作流，并返回当前模板组织的节点信息
        /// </summary>
        /// <param name="userSign">用户标识，长度不能超过80个字符</param>
        /// <param name="templateName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        TaskschedNode Push<TWorkStepMessage>(string templateName, string userSign, TWorkStepMessage message) where TWorkStepMessage : IWorkStepMessage;

        /// <summary>
        /// 将当前激发的消息压入队列中，开始进行按模板的步骤进行工作流，并返回当前模板组织的节点信息
        /// </summary>
        /// <param name="commandType">命令类型，长度不能超过20个字符</param>
        /// <param name="userSign">用户标识，长度不能超过80个字符</param>
        /// <param name="templateName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        TaskschedNode Push<TWorkStepMessage>(string templateName, string userSign, string commandType, TWorkStepMessage message) where TWorkStepMessage : IWorkStepMessage;
    }
}