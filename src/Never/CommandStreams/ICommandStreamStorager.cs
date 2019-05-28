namespace Never.CommandStreams
{
    /// <summary>
    /// 领域命令存储接口
    /// </summary>
    public interface ICommandStreamStorager
    {
        /// <summary>
        /// 保存领域命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <returns></returns>
        void Save(IOperateCommand command);
    }
}