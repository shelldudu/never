namespace Never.Messages
{
    /// <summary>
    /// 消息链接对象
    /// </summary>
    public class DefaultMessageConnection : Never.Messages.IMessageConnection
    {
        #region property

        /// <summary>
        /// 链接字符串
        /// </summary>
        public virtual string ConnetctionString { get; set; }

        #endregion property
    }
}