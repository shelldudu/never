using Never.Commands;

namespace Never.CommandStreams
{
    /// <summary>
    /// 领域命令存储接口
    /// </summary>
    public interface ICommandStorager
    {
        /// <summary>
        /// 保存领域命令
        /// </summary>
        /// <typeparam name="T">命令类型</typeparam>
        /// <param name="command">命令</param>
        /// <param name="commandContext">命令上下文</param>
        void Save<T>(ICommandContext commandContext, T command) where T : ICommand;
    }
}
