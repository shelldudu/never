using Never.Commands;

namespace Never.CommandStreams
{
    /// <summary>
    /// 命令分析接口
    /// </summary>
    public interface ICommandStreamAnalyser
    {
        /// <summary>
        /// 分析命令
        /// </summary>
        /// <typeparam name="TCommand">命令类型</typeparam>
        /// <param name="context">命令上下文</param>
        /// <param name="command">命令</param>
        /// <returns></returns>
        IOperateCommand Analyse<TCommand>(ICommandContext context, TCommand command) where TCommand : ICommand;
    }
}