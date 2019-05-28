namespace Never.Commands
{
    /// <summary>
    /// 默认上下文
    /// </summary>
    public interface ICommandContextInitable : ICommandContext
    {
        /// <summary>
        /// 初始化命令
        /// </summary>
        /// <param name="communication">上下文通讯</param>
        /// <param name="command">命令对象</param>
        void OnInit(HandlerCommunication communication, ICommand command);
    }
}