using Never.Commands;

namespace Never.CommandStreams
{
    /// <summary>
    /// 空的命令流保存对象
    /// </summary>
    public sealed class EmptyCommandStreamStorager : ICommandStorager, ICommandStreamStorager
    {
        #region ctor

        /// <summary>
        /// PrCommands a default instance of the <see cref="EmptyCommandStreamStorager"/> class from being created.
        /// </summary>
        public EmptyCommandStreamStorager()
        {
        }

        #endregion ctor

        #region ICommandStreamTypeStorager

        /// <summary>
        /// 批量保存领域命令
        /// </summary>
        /// <param name="command">命令列表</param>
        /// <returns></returns>
        public void Save(IOperateCommand command)
        {
        }

        /// <summary>
        /// 保存领域命令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">命令</param>
        /// <param name="commandContext">命令上下文</param>
        public void Save<T>(ICommandContext commandContext, T command) where T : ICommand
        {
         
        }

        #endregion ICommandStreamTypeStorager

        #region only

        /// <summary>
        /// Gets the only.
        /// </summary>
        /// <value>
        /// The only.
        /// </value>
        public static EmptyCommandStreamStorager Empty
        {
            get
            {
                if (Singleton<EmptyCommandStreamStorager>.Instance == null)
                    Singleton<EmptyCommandStreamStorager>.Instance = new EmptyCommandStreamStorager();

                return Singleton<EmptyCommandStreamStorager>.Instance;
            }
        }

        #endregion only
    }
}