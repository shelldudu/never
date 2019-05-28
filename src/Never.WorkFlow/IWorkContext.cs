using Never.Serialization;
using System;
using System.Collections.Generic;

namespace Never.WorkFlow
{
    /// <summary>
    /// 工作上下文
    /// </summary>
    public interface IWorkContext : Never.IWorkContext
    {
        /// <summary>
        /// 当前执行的Id
        /// </summary>
        Guid TaskId { get; }

        /// <summary>
        /// 在消息中匹配某个一消息，因为在一些组合步骤中会返回多个消息，message的消息是
        /// </summary>
        /// <typeparam name="TWorkStreamMessage"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        TWorkStreamMessage Match<TWorkStreamMessage>(IWorkStepMessage message) where TWorkStreamMessage : IWorkStepMessage;

        /// <summary>
        /// 在字典上下文中查询某个对象，如果找不到则执行回调，并将其塞进上下文中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="resultCallBack"></param>
        /// <returns></returns>
        T Find<T>(string key, Func<T> resultCallBack);

        /// <summary>
        /// 创建一个等待的消息
        /// </summary>
        /// <returns></returns>
        IWorkStepMessage CreateWatiingMessage();

        /// <summary>
        /// 创建一个等待的消息
        /// </summary>
        /// <returns></returns>
        IWorkStepMessage CreateWatiingMessage(string message, int attachState = 0);

        /// <summary>
        /// 创建一个中断的消息
        /// </summary>
        /// <returns></returns>
        IWorkStepMessage CreateBreakingMessage(string message, int attachState = 0);

        /// <summary>
        /// 任务策略
        /// </summary>
        ITaskschedStrategy Strategy { get; }

        /// <summary>
        /// json序列化内容
        /// </summary>
        IJsonSerializer JsonSerializer { get; }

        /// <summary>
        /// 用户字典
        /// </summary>
        IDictionary<string, object> Items { get; }

        /// <summary>
        /// 任务节点
        /// </summary>
        ITaskschedNode TaskschedNode { get; }

        /// <summary>
        /// 详情节点
        /// </summary>
        IExecutingNode ExecutingNode { get; }
    }
}