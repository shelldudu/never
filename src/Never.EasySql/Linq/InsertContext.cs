using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 插入上下文
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    public abstract class InsertContext<Parameter> : Context
    {
        /// <summary>
        /// dao
        /// </summary>
        protected readonly IDao dao;

        /// <summary>
        /// tableInfo
        /// </summary>
        protected readonly TableInfo tableInfo;

        /// <summary>
        /// sqlparameter
        /// </summary>
        protected readonly EasySqlParameter<Parameter> sqlParameter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        protected InsertContext(IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter)
        {
            this.dao = dao; this.tableInfo = tableInfo; this.sqlParameter = sqlParameter;
        }
    }
}
