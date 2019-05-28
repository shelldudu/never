using Never.Commands;
using Never.Threading;
using System;
using System.Collections.Generic;

namespace Never.CommandStreams
{
    /// <summary>
    /// 多个命令流存储接口
    /// </summary>
    public sealed class MultipleCommandStorager : ICommandStorager, IDisposable
    {
        #region field

        /// <summary>
        ///  命令流接口集合
        /// </summary>
        private IEnumerable<ICommandStorager> storager = null;

        /// <summary>
        ///  创建命令流接口集合
        /// </summary>
        private readonly Func<IEnumerable<ICommandStorager>> callback = null;

        /// <summary>
        /// 锁
        /// </summary>
        private readonly IRigidLocker locker = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleCommandStorager"/> class.
        /// </summary>
        /// <param name="storager">The storager.</param>
        public MultipleCommandStorager(IEnumerable<ICommandStorager> storager)
        {
            this.storager = storager;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ICommandStorager"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public MultipleCommandStorager(Func<IEnumerable<ICommandStorager>> callback)
        {
            this.callback = callback;
            this.locker = new MonitorLocker();
        }

        #endregion ctor

        /// <summary>
        /// 查询接口
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICommandStorager> GetStorager()
        {
            if (this.storager != null)
                return this.storager;

            if (this.callback == null)
                return new ICommandStorager[0];

            return this.locker.EnterLock(() =>
            {
                return this.storager = this.callback.Invoke();
            });
        }

        /// <summary>
        /// 保存领域命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="commandContext">命令上下文</param>
        public void Save<T>(ICommandContext commandContext, T command) where T : ICommand
        {
            foreach (var storager in this.GetStorager())
            {
                try
                {
                    storager.Save(commandContext, command);
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