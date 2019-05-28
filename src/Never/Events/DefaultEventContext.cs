using Never.Commands;
using Never.Security;
using System;
using System.Collections.Generic;

namespace Never.Events
{
    /// <summary>
    /// 默认事件上下文
    /// </summary>
    public class DefaultEventContext : IStartupEventContext, IEventContext
    {
        #region field

        /// <summary>
        /// 命令队列
        /// </summary>
        private readonly Queue<ICommand> queue = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEventContext"/> class.
        /// </summary>
        public DefaultEventContext()
        {
            this.queue = new Queue<ICommand>(10);
            this.WorkTime = DateTime.Now;
            this.Items = new Dictionary<string, object>(20);
        }

        #endregion ctor

        #region ieventContext

        /// <summary>
        /// 上下文集合
        /// </summary>
        public IDictionary<string, object> Items { get; private set; }

        /// <summary>
        /// 当前请求用户
        /// </summary>
        public Security.IUser Consigner { get; protected set; }

        /// <summary>
        /// 作业操作者
        /// </summary>
        public IWorker Worker { get; protected set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime WorkTime { get; set; }

        /// <summary>
        /// 当前执行的对象类型
        /// </summary>
        public Type TargetType { get; internal set; }

        /// <summary>
        /// 运行模式
        /// </summary>
        public virtual Aop.IRuntimeMode[] RuntimeModeArray { get; protected set; }

        /// <summary>
        /// 新加一个命令
        /// </summary>
        /// <param name="command">命令对象</param>
        public virtual void AddCommand(ICommand command)
        {
            this.queue.Enqueue(command);
        }

        /// <summary>
        /// 获取所有的命令
        /// </summary>
        /// <returns></returns>
        public ICommand[] GetAllCommands()
        {
            var temp = this.queue.ToArray();
            this.queue.Clear();
            return temp;
        }

        #endregion ieventContext

        #region init

        /// <summary>
        /// 从命令上下文初始化
        /// </summary>
        /// <param name="context">命令上下文</param>
        public virtual void OnInit(ICommandContext context)
        {
            if (context == null)
                return;

            this.Consigner = context.Consigner;
            this.Worker = context.Worker;

            if (context.Items.Count == 0)
                return;

            foreach (var i in context.Items)
                this.Items[i.Key] = i.Value;

            if (this.Consigner == null && this.Items.ContainsKey("Consigner"))
                this.Consigner = this.Items["Consigner"] as IUser;

            if (this.Worker == null && this.Items.ContainsKey("Worker"))
                this.Worker = this.Items["Worker"] as IWorker;

            if (context.RuntimeModeArray != null)
            {
                if (this.RuntimeModeArray == null)
                    this.RuntimeModeArray = new Aop.IRuntimeMode[context.RuntimeModeArray.Length];

                context.RuntimeModeArray.CopyTo(this.RuntimeModeArray, 0);
            }
        }

        #endregion init
    }
}