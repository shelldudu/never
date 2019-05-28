namespace Never.Commands
{
    /// <summary>
    /// 【并发】的时候放弃执行的命令
    /// </summary>
    public interface IAbortedSerialCommand : ISerialCommand
    {
    }
}