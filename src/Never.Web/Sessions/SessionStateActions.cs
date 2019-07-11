namespace Never.Web.Sessions
{
    /// <summary>
    /// 标识数据存储区中的会话项的是否为需要初始化的会话。
    /// </summary>
    public enum SessionStateActions
    {
        /// <summary>
        /// 任何初始化操作不需要由调用的代码执行。
        /// </summary>
        None = 0,

        /// <summary>
        /// 数据存储区中的会话项是为需要初始化会话。
        /// </summary>
        InitializeItem = 1
    }
}