using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 删除操作
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    public class DeletingContext<Parameter> : DeleteContext<Parameter>
    {
        private readonly string cacheId;
        private int textLength;
        private int setTimes;
        private string tableName;
        private string asName;
        /// <summary>
        /// 
        /// </summary> 
        /// <param name="cacheId"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public DeletingContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.cacheId = cacheId;
        }

        public override Linq.DeleteContext<Parameter> AsTable(string table)
        {
            throw new NotImplementedException();
        }

        public override Linq.DeleteContext<Parameter> Entrance()
        {
            throw new NotImplementedException();
        }

        public override Linq.DeleteContext<Parameter> Exists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.DeleteContext<Parameter> Exists(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.DeleteContext<Parameter> From(string table)
        {
            throw new NotImplementedException();
        }

        public override int GetResult()
        {
            var sqlTag = new LinqSqlTag(this.cacheId)
            {
                Labels = this.labels.AsEnumerable(),
                TextLength = this.textLength,
            };

            LinqSqlTagProvider.Set(sqlTag);
            return this.Execute(sqlTag.Clone(this.templateParameter), this.dao, this.sqlParameter);
        }

        public override Linq.DeleteContext<Parameter> In<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.DeleteContext<Parameter> In(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.DeleteContext<Parameter> NotExists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.DeleteContext<Parameter> NotExists(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.DeleteContext<Parameter> NotIn<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.DeleteContext<Parameter> NotIn(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.DeleteContext<Parameter> Where()
        {
            throw new NotImplementedException();
        }

        public override Linq.DeleteContext<Parameter> Where(Expression<Func<Parameter, object>> expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 对字段格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string Format(string text)
        {
            return string.Concat("`", text, "`");
        }
    }
}
