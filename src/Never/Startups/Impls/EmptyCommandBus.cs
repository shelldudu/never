using Never.Commands;
using System.Threading.Tasks;

namespace Never.Startups.Impls
{
    /// <summary>
    /// 没有任何功能的命令总线
    /// </summary>
    public sealed class EmptyCommandBus : ICommandBus
    {
        #region field

        /// <summary>
        /// 空对象
        /// </summary>
        public static EmptyCommandBus Only
        {
            get
            {
                if (Singleton<EmptyCommandBus>.Instance == null)
                    Singleton<EmptyCommandBus>.Instance = new EmptyCommandBus();

                return Singleton<EmptyCommandBus>.Instance;
            }
        }

        #endregion field

        #region ctor

        /// <summary>
        /// Prevents a default instance of the <see cref="EmptyCommandBus"/> class from being created.
        /// </summary>
        private EmptyCommandBus()
        {
        }

        #endregion ctor

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        ICommandHandlerResult ICommandBus.Send<TCommand>(TCommand c)
        {
            return new CommandHandlerResult(CommandHandlerStatus.Success, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="c"></param>
        /// <param name="communication"></param>
        /// <returns></returns>
        ICommandHandlerResult ICommandBus.Send<TCommand>(TCommand c, HandlerCommunication communication)
        {
            return new CommandHandlerResult(CommandHandlerStatus.Success, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<HandlerCommunication> ICommandBus.SendAsync<TCommand>(TCommand c)
        {
            return Task.FromResult(new HandlerCommunication());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="c"></param>
        /// <param name="communication"></param>
        /// <returns></returns>
        Task<HandlerCommunication> ICommandBus.SendAsync<TCommand>(TCommand c, HandlerCommunication communication)
        {
            return Task.FromResult(new HandlerCommunication());
        }
    }
}
