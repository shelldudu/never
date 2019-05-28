using Never.Domains;
using Never.Logging;
using Never.Security;
using System;
using System.Collections.Generic;

namespace Never.Commands
{
    /// <summary>
    /// 命令上下文
    /// </summary>
    public interface ICommandContext : Never.Aop.IRuntimeModeProvider, IWorkContext
    {
        /// <summary>
        /// 上下文集合
        /// </summary>
        IDictionary<string, object> Items { get; }

        /// <summary>
        /// 当前委托人
        /// </summary>
        IUser Consigner { get; }

        /// <summary>
        /// 验证信息
        /// </summary>
        /// <param name="root">聚合根</param>
        /// <param name="command">命令</param>
        void Validate(IAggregateRoot root, ICommand command);

        /// <summary>
        /// 获取聚合根，仅支持【事件流的内存模式】
        /// </summary>
        /// <typeparam name="TAggregateRoot">聚合根</typeparam>
        /// <typeparam name="TAggregateRootKey">聚合根标识Id，目前只有Guid,long,string,int四种</typeparam>
        /// <param name="key">内存模式下的key</param>
        /// <returns></returns>
        TAggregateRoot GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(TAggregateRootKey key) where TAggregateRoot : class, IAggregateRoot;

        /// <summary>
        /// 获取聚合根，仅支持【仓库模式】
        /// </summary>
        /// <typeparam name="TAggregateRoot">聚合根</typeparam>
        /// <typeparam name="TAggregateRootKey">聚合根标识Id，目前只有Guid,long,string,int四种</typeparam>
        /// <param name="key">内存模式下的key</param>
        /// <param name="getAggregate">如果找不到，则从仓库里面初始化</param>
        /// <returns></returns>
        TAggregateRoot GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(TAggregateRootKey key, Func<TAggregateRoot> getAggregate) where TAggregateRoot : class, IAggregateRoot;

        /// <summary>
        /// 获取聚合根，仅支持【仓库模式】
        /// </summary>
        /// <typeparam name="TAggregateRoot">聚合根</typeparam>
        /// <typeparam name="TAggregateRootKey">聚合根标识Id，目前只有Guid,long,string,int四种</typeparam>
        /// <param name="key">内存模式下的key</param>
        /// <param name="getAggregate">如果找不到，则从仓库里面初始化</param>
        /// <returns></returns>
        TAggregateRoot GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(TAggregateRootKey key, Func<TAggregateRootKey, TAggregateRoot> getAggregate) where TAggregateRoot : class, IAggregateRoot;

        /// <summary>
        /// 获取更新过的聚合根
        /// </summary>
        /// <returns></returns>
        IAggregateRoot[] GetChangeAggregateRoot();

        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        ICommandHandlerResult CreateResult(CommandHandlerStatus status);

        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        ICommandHandlerResult CreateResult(CommandHandlerStatus status, string message);
    }
}