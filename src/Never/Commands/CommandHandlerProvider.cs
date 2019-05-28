using Never.IoC;

namespace Never.Commands
{
    /// <summary>
    /// 命令提供者
    /// </summary>
    public class CommandHandlerProvider
    {
        #region field

        /// <summary>
        /// 服务定位器
        /// </summary>
        private readonly ILifetimeScope scope = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerProvider"/> class.
        /// </summary>
        public CommandHandlerProvider(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        #endregion ctor

        #region prop

        /// <summary>
        ///
        /// </summary>
        public ILifetimeScope Scope
        {
            get
            {
                return this.scope;
            }
        }

        #endregion prop

        #region ICommandProvider成员

        /// <summary>
        /// 查找所有监听了T对象的命令监听者
        /// </summary>
        /// <typeparam name="TCommand">命令类型</typeparam>
        /// <returns></returns>
        public ICommandHandler<TCommand>[] FindCommandHandler<TCommand>() where TCommand : ICommand
        {
            return this.scope.ResolveAll<ICommandHandler<TCommand>>();
        }

        /// <summary>
        /// 获取命令上下文提供者
        /// </summary>
        /// <returns></returns>
        public ICommandContext FindCommandContext()
        {
            return this.scope.ResolveOptional<ICommandContext>() ?? DefaultCommandContext.New();
        }

        #endregion ICommandProvider成员
    }
}