namespace Never.Messages
{
    /// <summary>
    /// 消息链接对象
    /// </summary>
    public interface IMessageConnection
    {
        #region property

        /// <summary>
        /// 链接字符串
        /// </summary>
        string ConnetctionString { get; }

        #endregion property
    }
}