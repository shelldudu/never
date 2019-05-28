using Never.Commands;
using Never.Events;
using Never.Threading;
using System;
using System.Collections.Generic;

namespace Never.EventStreams
{
    /// <summary>
    /// 多个事件流存储接口
    /// </summary>
    public sealed class MultipleEventStorager : IEventStorager, IDisposable
    {
        #region field

        /// <summary>
        ///  事件流接口集合
        /// </summary>
        private IEnumerable<IEventStorager> storager = null;

        /// <summary>
        ///  创建事件流接口集合
        /// </summary>
        private readonly Func<IEnumerable<IEventStorager>> callback = null;

        /// <summary>
        /// 锁
        /// </summary>
        private readonly IRigidLocker locker = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleEventStorager"/> class.
        /// </summary>
        /// <param name="storager">The storager.</param>
        public MultipleEventStorager(IEnumerable<IEventStorager> storager)
        {
            this.storager = storager;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleEventStorager"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public MultipleEventStorager(Func<IEnumerable<IEventStorager>> callback)
        {
            this.callback = callback;
            this.locker = new MonitorLocker();
        }

        #endregion ctor

        /// <summary>
        /// 查询接口
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IEventStorager> GetStorager()
        {
            if (this.storager != null)
                return this.storager;

            if (this.callback == null)
                return new IEventStorager[0];

            return this.locker.EnterLock(() =>
            {
                return this.storager = this.callback.Invoke();
            });
        }

        /// <summary>
        /// 批量保存领域事件
        /// </summary>
        /// <param name="commandContext">上下文</param>
        /// <param name="events">事件列表</param>
        public void Save(ICommandContext commandContext, IEnumerable<KeyValuePair<Type, IEvent[]>> events)
        {
            foreach (var storager in this.GetStorager())
            {
                try
                {
                    storager.Save(commandContext, events);
                }
                catch
                {
                }
                finally
                {
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            this.locker.Dispose();
        }

    }
}