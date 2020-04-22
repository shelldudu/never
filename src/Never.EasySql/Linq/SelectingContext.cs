using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// select语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    public class SelectingContext<Parameter, Table> : SelectContext<Parameter, Table>
    {
        private readonly string cacheId;
        private int textLength;
        private int setTimes;
        private string tableName;
        private string asName;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public SelectingContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.cacheId = cacheId;
        }

        public override Linq.SelectContext<Parameter, Table> AsTable(string table)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> Entrance()
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> Exists<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> Exists(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> From(string table)
        {
            throw new NotImplementedException();
        }

        public override Table GetResult()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Table> GetResults()
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> In<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> In(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> NotExists<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> NotExists(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> NotIn<Table2>(AndOrOption option, Expression<Func<Parameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> NotIn(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> Where()
        {
            throw new NotImplementedException();
        }

        public override Linq.SelectContext<Parameter, Table> Where(Expression<Func<Parameter, object>> expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 对表名格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string FormatTable(string text)
        {
            return text;
        }

        /// <summary>
        /// 对字段格式化
        /// </summary>
        protected override string FormatColumn(string text)
        {
            return text;
        }
    }
}
