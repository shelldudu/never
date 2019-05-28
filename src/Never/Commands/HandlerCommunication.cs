using Never.Events;
using System;
using System.Collections.Generic;

namespace Never.Commands
{
    /// <summary>
    /// 上下文
    /// </summary>
    [Serializable]
    public sealed class HandlerCommunication : Dictionary<string, object>, IDictionary<string, object>
    {
        #region ctor

        /// <summary>
        /// 初始化 HandlerCommunication 类的新实例，该实例为空，具有默认的初始容量并为键类型使用默认的相等比较器。
        /// </summary>
        public HandlerCommunication()
        {
            this.RuntimeModeArray = new List<Never.Aop.IRuntimeMode>(0);
        }

        /// <summary>
        /// 初始化 HandlerCommunication 类的新实例，该实例为空，具有指定的初始容量并为键类型使用默认的相等比较器。
        /// </summary>
        /// <param name="capacity"></param>
        public HandlerCommunication(int capacity) : base(capacity)
        {
            this.RuntimeModeArray = new List<Never.Aop.IRuntimeMode>(0);
        }

        /// <summary>
        /// 初始化 HandlerCommunication 类的新实例，该实例为空，具有默认的初始容量并使用指定的 System.Collections.Generic.IEqualityComparer string。
        /// </summary>
        /// <param name="comparer">比较键时要使用的 System.Collections.Generic.IEqualityComparer`1 实现，或者为 null，以便为键类型使用默认的</param>
        public HandlerCommunication(IEqualityComparer<string> comparer) : base(comparer)
        {
            this.RuntimeModeArray = new List<Never.Aop.IRuntimeMode>(0);
        }

        /// <summary>
        /// 初始化 HandlerCommunication 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary`2   复制的元素并为键类型使用默认的相等比较器。
        /// </summary>
        /// <param name="dictionary"></param>
        public HandlerCommunication(IDictionary<string, object> dictionary) : base(dictionary)
        {
            this.RuntimeModeArray = new List<Never.Aop.IRuntimeMode>(0);
        }

        /// <summary>
        /// 初始化 HandlerCommunication 类的新实例，该实例为空，具有指定的初始容量并使用指定的 System.Collections.Generic.IEqualityComparer`string`。
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="comparer"></param>
        public HandlerCommunication(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer)
        {
            this.RuntimeModeArray = new List<Never.Aop.IRuntimeMode>(0);
        }

        /// <summary>
        /// 初始化 HandlerCommunication 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary`2
        /// 中复制的元素并使用指定的 System.Collections.Generic.IEqualityComparer`1。
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="comparer"></param>
        public HandlerCommunication(IDictionary<string, object> dictionary, IEqualityComparer<string> comparer) : base(dictionary, comparer)
        {
            this.RuntimeModeArray = new List<Never.Aop.IRuntimeMode>(0);
        }

        /// <summary>
        /// 使用事件上下文初始化对象
        /// </summary>
        /// <param name="eventContext"></param>
        public HandlerCommunication(IEventContext eventContext) : base((eventContext == null || eventContext.Items == null) ? 0 : eventContext.Items.Count)
        {
            this.Consigner = eventContext.Consigner;
            this.Worker = eventContext.Worker;
            this.RuntimeModeArray = new List<Aop.IRuntimeMode>(eventContext.RuntimeModeArray ?? new Aop.IRuntimeMode[0]);
            if (eventContext.Items != null)
            {
                foreach (var i in eventContext.Items)
                {
                    this[i.Key] = i;
                }
            }
        }

        /// <summary>
        /// 使用命令上下文初始化对象
        /// </summary>
        /// <param name="commandContext"></param>
        public HandlerCommunication(ICommandContext commandContext) : base((commandContext == null || commandContext.Items == null) ? 0 : commandContext.Items.Count)
        {
            this.Consigner = commandContext.Consigner;
            this.Worker = commandContext.Worker;
            this.RuntimeModeArray = new List<Aop.IRuntimeMode>(commandContext.RuntimeModeArray ?? new Aop.IRuntimeMode[0]);
            if (commandContext.Items != null)
            {
                foreach (var i in commandContext.Items)
                {
                    this[i.Key] = i;
                }
            }
        }

        #endregion ctor

        #region worker

        /// <summary>
        /// 用户委托者
        /// </summary>
        public Never.Security.IUser Consigner { get; set; }

        /// <summary>
        /// 作业操作者
        /// </summary>
        public Never.Security.IWorker Worker { get; set; }

        /// <summary>
        /// 运行模式
        /// </summary>
        public List<Never.Aop.IRuntimeMode> RuntimeModeArray { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        public ICommandHandlerResult HandlerResult { get; set; }
        #endregion worker
    }
}