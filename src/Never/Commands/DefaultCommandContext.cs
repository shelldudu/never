using Never.Domains;
using Never.IoC;
using Never.Logging;
using Never.Security;
using System;
using System.Collections.Generic;

namespace Never.Commands
{
    /// <summary>
    /// 默认命令上下文
    /// 如需使用事件流模式，请使用<see cref="MemoryCommandContext"/>
    /// 如需使用混合模式，请使用<see cref="MixtureCommandContext"/>
    /// </summary>
    public class DefaultCommandContext : ICommandContextInitable, ICommandContext
    {
        #region field

        /// <summary>
        /// 存放在当前请求模式的聚合根
        /// </summary>
        protected readonly Queue<IAggregateRoot> LifetimeScopeQueue = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCommandContext"/> class.
        /// </summary>
        public DefaultCommandContext()
        {
            this.Items = new Dictionary<string, object>();
            this.WorkTime = DateTime.Now;
            this.LifetimeScopeQueue = new Queue<IAggregateRoot>(1);
        }

        #endregion ctor

        #region new

        /// <summary>
        /// 重新生成一个上下文
        /// </summary>
        /// <returns></returns>
        public static DefaultCommandContext New()
        {
            return new DefaultCommandContext();
        }

        #endregion new

        #region ICommandContext

        /// <summary>
        /// 上下文集合
        /// </summary>
        public IDictionary<string, object> Items { get; private set; }

        /// <summary>
        /// 当前请求用户
        /// </summary>
        public Security.IUser Consigner { get; set; }

        /// <summary>
        /// 作业操作者
        /// </summary>
        public IWorker Worker { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime WorkTime { get; set; }

        /// <summary>
        /// 运行模式
        /// </summary>
        public virtual Aop.IRuntimeMode[] RuntimeModeArray { get; set; }

        /// <summary>
        /// 初始化命令
        /// </summary>
        /// <param name="communication">上下文通讯</param>
        /// <param name="command">命令对象</param>
        public virtual void OnInit(HandlerCommunication communication, ICommand command)
        {
            if (communication != null)
            {
                foreach (var item in communication)
                    this.Items[item.Key] = item.Value;
            }

            var principal = System.Threading.Thread.CurrentPrincipal as UserPrincipal;
            if (principal != null)
            {
                if (this.Consigner == null)
                    this.Consigner = principal.CurrentUser;

                if (this.Worker == null)
                    this.Worker = principal.CurrentUser as IWorker;
            }

            if (communication.Consigner != null)
                this.Consigner = communication.Consigner;

            if (communication.Worker != null)
                this.Worker = communication.Worker;

            if (communication.RuntimeModeArray != null)
            {
                if (this.RuntimeModeArray == null)
                    this.RuntimeModeArray = new Aop.IRuntimeMode[communication.RuntimeModeArray.Count];

                communication.RuntimeModeArray.CopyTo(this.RuntimeModeArray, 0);
            }
        }

        /// <summary>
        /// 验证信息
        /// </summary>
        /// <param name="root">聚合根</param>
        /// <param name="command">命令</param>
        public virtual void Validate(IAggregateRoot root, ICommand command)
        {
        }

        /// <summary>
        /// 获取聚合根，仅支持【事件流的内存模式】
        /// </summary>
        /// <typeparam name="TAggregateRootKey">聚合根标识Id，目前只有Guid,long,string,int四种</typeparam>
        /// <typeparam name="TAggregateRoot">聚合根</typeparam>
        /// <param name="key">内存模式下的key</param>
        /// <returns></returns>
        public virtual TAggregateRoot GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(TAggregateRootKey key) where TAggregateRoot : class, IAggregateRoot
        {
            return GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(key, default(Func<TAggregateRoot>));
        }

        /// <summary>
        /// 获取聚合根，仅支持【仓库模式】
        /// </summary>
        /// <typeparam name="TAggregateRootKey">聚合根标识Id，目前只有Guid,long,string,int四种</typeparam>
        /// <typeparam name="TAggregateRoot">聚合根</typeparam>
        /// <param name="key">内存模式下的key</param>
        /// <param name="getAggregate">如果找不到，则从仓库里面初始化</param>
        /// <returns></returns>
        public virtual TAggregateRoot GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(TAggregateRootKey key, Func<TAggregateRoot> getAggregate) where TAggregateRoot : class, IAggregateRoot
        {
            if (getAggregate == null)
                return default(TAggregateRoot);

            var root = getAggregate.Invoke();
            if (root == null)
                return default(TAggregateRoot);

            if (this.LifetimeScopeQueue.Contains(root))
                return root;

            this.LifetimeScopeQueue.Enqueue(root);
            return root;
        }

        /// <summary>
        /// 获取聚合根，仅支持【仓库模式】
        /// </summary>
        /// <typeparam name="TAggregateRootKey">聚合根标识Id，目前只有Guid,long,string,int四种</typeparam>
        /// <typeparam name="TAggregateRoot">聚合根</typeparam>
        /// <param name="key">内存模式下的key</param>
        /// <param name="getAggregate">如果找不到，则从仓库里面初始化</param>
        /// <returns></returns>
        public virtual TAggregateRoot GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(TAggregateRootKey key, Func<TAggregateRootKey, TAggregateRoot> getAggregate) where TAggregateRoot : class, IAggregateRoot
        {
            if (getAggregate == null)
                return default(TAggregateRoot);

            var root = getAggregate.Invoke(key);
            if (root == null)
                return default(TAggregateRoot);

            if (this.LifetimeScopeQueue.Contains(root))
                return root;

            this.LifetimeScopeQueue.Enqueue(root);
            return root;
        }

        /// <summary>
        /// 获取更新过的聚合根
        /// </summary>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public virtual IAggregateRoot[] GetChangeAggregateRoot()
        {
            var temp = this.LifetimeScopeQueue.ToArray();
            this.LifetimeScopeQueue.Clear();
            return temp;
        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public ICommandHandlerResult CreateResult(CommandHandlerStatus status)
        {
            return this.CreateResult(status, string.Empty);
        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public ICommandHandlerResult CreateResult(CommandHandlerStatus status, string message)
        {
            return new CommandHandlerResult(status, message) { Communication = this.Items };
        }

        #endregion ICommandContext
    }
}