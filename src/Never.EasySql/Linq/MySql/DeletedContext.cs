using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.MySql
{
    public sealed class DeletedContext<Parameter> : Linq.DeleteContext<Parameter>
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly LinqSqlTag sqlTag;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlTag"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public DeletedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        protected override string Format(string text)
        {
            throw new NotImplementedException();
        }
    }
}
