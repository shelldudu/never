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
    /// <typeparam name="TAggregateRootId">聚合根类型.</typeparam>
    public abstract class AggregateRoot<TAggregateRootId> : AggregateRoot, IAggregateRoot, IAggregateRoot<TAggregateRootId>
    {
        #region property

        /// <summary>
        /// 聚合根标识
        /// </summary>
        public virtual TAggregateRootId AggregateId { get; protected set; }

        #endregion property

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="aggregateId">唯一标识</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        protected AggregateRoot(TAggregateRootId aggregateId)
        {
            this.AggregateId = aggregateId;
        }

        #endregion ctor

        #region reject

        /// <summary>
        /// 拒绝
        /// </summary>
        /// <param name="target"></param>
        /// <param name="expression">表达式</param>
        /// <param name="message">错误信息</param>
        protected ValidationFailure RejectBreak<T>(T target, Expression<Func<T, object>> expression, string message) where T : AggregateRoot
        {
            return this.Reject<T>(expression, message, ValidationOption.Break);
        }

        /// <summary>
        /// 拒绝
        /// </summary>
        /// <param name="target"></param>
        /// <param name="expression">表达式</param>
        /// <param name="message">错误信息</param>
        protected ValidationFailure RejectContinue<T>(T target, Expression<Func<T, object>> expression, string message) where T : AggregateRoot
        {
            return this.Reject<T>(expression, message, ValidationOption.Continue);
        }

        #endregion
    }
}