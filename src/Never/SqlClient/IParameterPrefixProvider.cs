namespace Never.SqlClient
{
    /// <summary>
    /// 参数前缀
    /// </summary>
    public interface IParameterPrefixProvider
    {
        /// <summary>
        /// 获取参数的前缀，比如@ ; ?等
        /// </summary>
        /// <returns></returns>
        string GetParameterPrefix();
    }
}