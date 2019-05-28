namespace Never.Commands
{
    /// <summary>
    /// 串行命令【实现了数据库的行锁】，表示该命令要等待个相同的命令执行完后才执行下一个
    /// </summary>
    public interface ISerialCommand
    {
        /// <summary>
        /// 执行的主键
        /// </summary>
        string Body { get; }
    }
}