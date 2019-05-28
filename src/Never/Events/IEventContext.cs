using Never.Commands;
using Never.Security;
using System;
using System.Collections.Generic;

namespace Never.Events
{
    /// <summary>
    /// 事件上下文
    /// </summary>
    public interface IEventContext : Never.Aop.IRuntimeModeProvider, IWorkContext
    {
        /// <summary>
        /// 上下文集合
        /// </summary>
        IDictionary<string, object> Items { get; }

        /// <summary>
        /// 当前请求用户
        /// </summary>
        IUser Consigner { get; }

        /// <summary>
        /// 获取所有的命令
        /// </summary>
        /// <returns></returns>
        ICommand[] GetAllCommands();

        /// <summary>
        /// 新加一个命令
        /// </summary>
        /// <param name="command">命令对象</param>
        void AddCommand(ICommand command);

        /// <summary>
        /// 当前执行的对象类型
        /// </summary>
        Type TargetType { get; }
    }
}