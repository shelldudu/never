namespace Never.Deployment
{
    /// <summary>
    /// 路由主键接口
    /// </summary>
    public interface IRoutePrimaryKeySelect
    {
        /// <summary>
        /// 路由主键
        /// </summary>
        string PrimaryKey { get; }
    }
}