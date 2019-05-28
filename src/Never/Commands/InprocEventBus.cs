using Never.Events;
using Never.Exceptions;
using Never.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Never.Commands
{
    /// <summary>
    /// 进程内发布事件
    /// </summary>
    internal class InprocEventBus : EventBus, IEventBus
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="InprocEventBus"/> class.
        /// </summary>
        /// <param name="commandBus"></param>
        /// <param name="serviceLocator"></param>
        public InprocEventBus(ICommandBus commandBus,
            IServiceLocator serviceLocator) : base(commandBus, serviceLocator)
        {
        }

        #endregion ctor

        #region push

        public override void Push(ICommandContext context, IEnumerable<IEvent[]> events)
        {
            if (events == null)
                return;

            foreach (var splits in events)
            {
                if (splits.IsNullOrEmpty())
                    continue;

                var task = Task.Factory.StartNew(() => { });
                foreach (var split in splits)
                    task.ContinueWith((t) => this.PublishEvent(split, context));
            }
        }

        #endregion push
    }
}