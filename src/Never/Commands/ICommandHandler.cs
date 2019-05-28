using Never.Attributes;
using System;
using System.Collections.Generic;

namespace Never.Commands
{
    /// <summary>
    /// 命令处理接口
    /// </summary>
    public interface ICommandHandler
    {
    }

    /// <summary>
    /// 命令处理接口
    /// </summary>
    /// <typeparam name="TCommand">命令对象</typeparam>
    public interface ICommandHandler<in TCommand> : ICommandHandler
        where TCommand : ICommand
    {
        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="command">命令对象</param>
        ICommandHandlerResult Execute(ICommandContext context, TCommand command);
    }

    /// <summary>
    /// 命令处理结果
    /// </summary>
    public interface ICommandHandlerResult
    {
        /// <summary>
        /// 状态值
        /// </summary>
        CommandHandlerStatus Status { get; }

        /// <summary>
        /// 处理消息
        /// </summary>
        string Message { get; }

        /// <summary>
        /// 上下文通讯
        /// </summary>
        IDictionary<string, object> Communication { get; }
    }

    /// <summary>
    /// 处理接口数据结果
    /// </summary>
    [Serializable, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [System.Runtime.Serialization.DataContract]
    public sealed class CommandHandlerResult : ICommandHandlerResult
    {
        #region property

        /// <summary>
        /// 状态值
        /// </summary>
        [DefaultValue(Value = "ResultStatus.Fail")]
        [System.Runtime.Serialization.DataMember(Name = "status")]
        [Serialization.Json.DataMember(Name = "status")]
        public CommandHandlerStatus Status { get; set; }

        /// <summary>
        /// 附带信息
        /// </summary>
        [DefaultValue(Value = "string.Empty")]
        [System.Runtime.Serialization.DataMember(Name = "message")]
        [Serialization.Json.DataMember(Name = "message")]
        public string Message { get; set; }

        /// <summary>
        /// 上下文通讯
        /// </summary>
        [System.Runtime.Serialization.IgnoreDataMember]
        [Serialization.Json.IgnoreDataMember]
        public IDictionary<string, object> Communication { get; set; }

        #endregion property

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult{TResult}"/> class.
        /// </summary>
        public CommandHandlerResult()
            : this(CommandHandlerStatus.Success, string.Empty)
        {
            /*保留这个是因为在Wcf中用到契约的话，没这个构造则提示契约不可用*/
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerResult"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="message">The message.</param>
        public CommandHandlerResult(CommandHandlerStatus status, string message)
        {
            this.Status = status;
            this.Message = message;
        }

        #endregion ctor
    }

    /// <summary>
    /// 命令处理结果
    /// </summary>
    public enum CommandHandlerStatus : byte
    {
        /// <summary>
        /// 成功处理
        /// </summary>
        Success = 0,

        /// <summary>
        /// 幂等
        /// </summary>
        Idempotent = 1,

        /// <summary>
        /// 指聚合没有要提交事件
        /// </summary>
        NothingChanged = 2,

        /// <summary>
        /// 资源不足
        /// </summary>
        PoorInventory = 3,

        /// <summary>
        /// 不存在
        /// </summary>
        NotExists = 4,

        /// <summary>
        /// 失败的
        /// </summary>
        Fail = 255,
    }
}