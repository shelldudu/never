using System.Data.Common;

namespace Never.SqlClient
{
    /// <summary>
    /// 模糊的实现
    /// </summary>
    internal class VagueSqlExecuter : SqlExecuter
    {
        #region field

        /// <summary>
        /// 参数前缀
        /// </summary>
        private readonly string parameterPrefix = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="VagueSqlExecuter"/> class.
        /// </summary>
        /// <param name="parameterPrefix"></param>
        /// <param name="factory"></param>
        /// <param name="connectionString"></param>
        public VagueSqlExecuter(string parameterPrefix, DbProviderFactory factory, string connectionString)
            : base(factory, connectionString)
        {
            this.parameterPrefix = parameterPrefix;
        }

        #endregion ctor

        #region prefix

        /// <summary>
        /// 获取Sql参数的前缀
        /// </summary>
        /// <returns></returns>
        public override string GetParameterPrefix()
        {
            return this.parameterPrefix;
        }

        #endregion prefix
    }
}