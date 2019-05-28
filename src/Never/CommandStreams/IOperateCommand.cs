using Never.Commands;
using System;

namespace Never.CommandStreams
{
    /// <summary>
    /// 带操作属性的命令接口
    /// </summary>
    public interface IOperateCommand
    {
        /// <summary>
        /// 命令所在的运行域
        /// </summary>
        string AppDomain { get; }

        /// <summary>
        /// 命令
        /// </summary>
        ICommand Command { get; }

        /// <summary>
        /// 命令类型
        /// </summary>
        string CommandType { get; }

        /// <summary>
        /// 命令类型
        /// </summary>
        string CommandTypeFullName { get; }

        /// <summary>
        /// 操作命令
        /// </summary>
        DateTime CreateDate { get; }

        /// <summary>
        /// 操作者
        /// </summary>
        string Creator { get; }

        /// <summary>
        /// 版本号
        /// </summary>
        int Version { get; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        string AggregateId { get; }

        /// <summary>
        /// 唯一标识的Type
        /// </summary>
        Type AggregateIdType { get; }

        /// <summary>
        /// 当前的HashCode
        /// </summary>
        int HashCode { get; }

        /// <summary>
        /// 当前环境下的自增Id
        /// </summary>
        long Increment { get; }
    }
}