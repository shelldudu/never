using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.MySql
{
    public sealed class SelectedContext<Parameter, Table> : Linq.SelectContext<Parameter, Table>
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
        public SelectedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override void AsTable(string table)
        {
            throw new NotImplementedException();
        }

        public override void From(string table)
        {
            throw new NotImplementedException();
        }

        protected override string Format(string text)
        {
            throw new NotImplementedException();
        }
    }
}
