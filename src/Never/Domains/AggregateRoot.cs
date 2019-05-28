using Never.DataAnnotations;
using Never.Events;
using Never.Exceptions;
using Never.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Never.Domains
{
    /// <summary>
    /// 聚合根
    /// </summary>
    public abstract class AggregateRoot : IAggregateRoot, IDisposable
    {
        #region property

        /// <summary>
        /// 版本
        /// </summary>
        public virtual int Version { get; protected set; }

        /// <summary>
        /// 是否上下文调用
        /// </summary>
        protected bool IsContextCall { get; private set; }

        /// <summary>
        /// 获取当前修改中递增的版本号，通常每一个事件会加一个版本号
        /// </summary>
        public int IncrementVersion
        {
            get
            {
                return this.queue.Count;
            }
        }

        #endregion property

        #region field

        /// <summary>
        /// 保存事件源列表
        /// </summary>
        private Queue<IEvent> queue = null;

        /// <summary>
        /// 当前事件是否可以提交
        /// </summary>
        private List<bool> commitCondition = null;

        /// <summary>
        /// 存放事件流回溯
        /// </summary>
        private readonly Hashtable eventAction = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        static AggregateRoot()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        protected AggregateRoot()
        {
            this.queue = new Queue<IEvent>(20);
            this.commitCondition = new List<bool>();
            this.eventAction = new Hashtable();
        }

        #endregion ctor

        #region IAggregateRoot成员

        /// <summary>
        /// 获取更改的事件
        /// </summary>
        /// <returns></returns>
        public IEvent[] GetChanges()
        {
            return this.queue.ToArray();
        }

        /// <summary>
        /// 在事件列表中查询类型相同的事件对象
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <returns></returns>
        public IEnumerable<TEvent> FindEvents<TEvent>() where TEvent : IEvent
        {
            return this.FindEvents<TEvent>(this.queue);
        }

        /// <summary>
        /// 在事件列表中查询类型相同的事件对象
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="events">事件列表.</param>
        /// <returns></returns>
        protected IEnumerable<TEvent> FindEvents<TEvent>(IEnumerable<IEvent> events) where TEvent : IEvent
        {
            if (events == null)
            {
                return new TEvent[] { };
            }

            var eType = typeof(TEvent);
            var list = new List<TEvent>(10);
            foreach (var n in events)
            {
                if (n.GetType() == eType)
                {
                    list.Add((TEvent)n);
                }
            }

            return list;
        }

        /// <summary>
        /// 在事件列表中查询类型相同的事件对象
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <returns></returns>
        public TEvent FindEvent<TEvent>() where TEvent : IEvent
        {
            return this.FindEvent<TEvent>(this.queue);
        }

        /// <summary>
        /// 在事件列表中查询类型相同的事件对象
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="events">事件列表.</param>
        /// <returns></returns>
        protected TEvent FindEvent<TEvent>(IEnumerable<IEvent> events) where TEvent : IEvent
        {
            if (events == null)
            {
                return default(TEvent);
            }

            var eType = typeof(TEvent);
            foreach (var n in events)
            {
                if (n.GetType() == eType)
                {
                    return (TEvent)n;
                }
            }

            return default(TEvent);
        }

        /// <summary>
        /// 获取更新事件的条数
        /// </summary>
        /// <returns></returns>
        public int GetChangeCounts()
        {
            return this.queue.Count;
        }

        /// <summary>
        /// 更新版本并清空事件源列表
        /// </summary>
        /// <param name="version">版本</param>
        /// <exception cref="Never.Exceptions.DomainException">版本号错误</exception>
        public IEvent[] Change(int version)
        {
            if ((this.Version + this.queue.Count) != version)
            {
                throw new DomainException(string.Format("版本错误，期望版本为{0},实际版本为{1}", this.Version + this.queue.Count, version));
            }

            this.Version = version;
            var copies = this.GetChanges();
            this.ReleaseChanges();

            return copies;
        }

        /// <summary>
        /// 用历史事件来重新还原聚合根最新状态
        /// </summary>
        /// <param name="history">历史领域事件</param>
        public void ReplyEvent(IEnumerable<IEvent> history)
        {
            foreach (var e in history)
            {
                if (GlobalCompileSetting.DynamicReplaceHashtable)
                {
                    this.ApplyReplyEvent((dynamic)e);
                }
                else
                {
                    var type = e.GetType();
                    var action = this.eventAction[type] as Action<AggregateRoot, IEvent>;
                    if (action == null)
                    {
                        //使用Delegate.CreateDelegate方法
                        //eventAction[type] = action = (Action<AggregateRoot, IEvent>)Delegate.CreateDelegate(typeof(Action<AggregateRoot, IEvent>), typeof(AggregateRoot).GetMethod("ApplyReplyEvent", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(type));

                        /*装一下逼*/
                        var emit = EasyEmitBuilder<Action<IAggregateRoot, IEvent>>.NewDynamicMethod();
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        emit.Call(typeof(AggregateRoot).GetMethod("ApplyReplyEvent", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(type));
                        emit.Return();
                        this.eventAction[type] = action = emit.CreateDelegate();
                    }

                    if (action == null)
                    {
                        continue;
                    }

                    action.Invoke(this, e);
                }

                this.Version++;
            }
        }

        #endregion IAggregateRoot成员

        #region utils

        /// <summary>
        /// 加入事件
        /// </summary>
        /// <param name="e">领域事件</param>
        protected void ApplyEvent<T>(T e) where T : IEvent
        {
            this.IsContextCall = true;
            var handle = this as IHandle<T>;
            if (handle == null)
            {
                throw new FriendlyException("当前聚合根没有实现IHandle<{0}>接口", typeof(T).Name);
            }

            handle.Handle(e);
            this.queue.Enqueue(e);

            this.IsContextCall = false;
            this.AddCommit(true);
        }

        /// <summary>
        /// 加入事件，请不要改动该名字，反射用到的
        /// </summary>
        /// <param name="e">领域事件</param>
        private void ApplyReplyEvent<T>(T e) where T : IEvent
        {
            var handle = this as IHandle<T>;
            if (handle == null)
            {
                return;
            }

            this.IsContextCall = true;
            handle.Handle(e);
            this.IsContextCall = false;
        }

        #endregion utils

        #region IDisposable

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="isdispose">是否释放资源</param>
        protected virtual void Dispose(bool isdispose)
        {
            if (!isdispose)
            {
                return;
            }

            this.ReleaseChanges();
        }

        /// <summary>
        /// 清空事件源列表
        /// </summary>
        protected void ReleaseChanges()
        {
            this.queue.Clear();
            this.commitCondition.Clear();
        }

        #endregion IDisposable

        #region commit

        /// <summary>
        /// 将当前情况加入到是否可以允许修改条件中
        /// </summary>
        /// <param name="can">聚合跟能提交条件</param>
        protected void AddCommit(bool can)
        {
            this.commitCondition.Add(can);
        }

        /// <summary>
        /// 当前情况是否可以允许提交
        /// </summary>
        /// <returns></returns>
        public bool CanCommit()
        {
            if (this.commitCondition.Count == 0)
            {
                return false;
            }

            for (var i = 0; i < this.commitCondition.Count; i++)
            {
                if (!this.commitCondition[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 当前情况是否不可以允许提交
        /// </summary>
        /// <returns></returns>
        public bool CanNotCommit()
        {
            if (this.commitCondition.Count == 0)
            {
                return true;
            }

            for (var i = 0; i < this.commitCondition.Count; i++)
            {
                if (!this.commitCondition[i])
                {
                    return true;
                }
            }

            return false;
        }

        #endregion commit

        #region reject 

        /// <summary>
        /// 拒绝
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="message">错误信息</param>
        /// <param name="option">结果选项</param>
        public ValidationFailure Reject<T>(Expression<Func<T, object>> expression, string message, ValidationOption option) where T : AggregateRoot
        {
            var model = expression as ParameterExpression;
            if (model != null)
            {
                return new ValidationFailure() { ErrorMessage = message, MemberName = model.Name, Option = option };
            }

            var member = expression as MemberExpression;
            if (member != null && member.Member != null)
            {
                return new ValidationFailure() { ErrorMessage = message, MemberName = member.Member.Name, Option = option };
            }

            var unary = expression as UnaryExpression;
            if (unary != null)
            {
                var property = unary.Operand as MemberExpression;
                if (property != null)
                {
                    return new ValidationFailure() { ErrorMessage = message, MemberName = property.Member.Name, Option = option };
                }
            }

            return new ValidationFailure() { ErrorMessage = message, MemberName = expression.ToString(), Option = option };
        }
        #endregion
    }
}