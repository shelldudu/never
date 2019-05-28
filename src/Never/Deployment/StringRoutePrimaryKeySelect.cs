namespace Never.Deployment
{
    /// <summary>
    /// 字符串路由主键
    /// </summary>
    public struct StringRoutePrimaryKeySelect : IRoutePrimaryKeySelect
    {
        #region prop

        /// <summary>
        /// 关键Key
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        /// 路由主键
        /// </summary>
        string IRoutePrimaryKeySelect.PrimaryKey
        {
            get
            {
                return this.PrimaryKey;
            }
        }

        #endregion prop
    }
}