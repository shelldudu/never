using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.MySql
{
    public sealed class DeleteContext<Parameter> : Linq.DeleteContext<Parameter>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public DeleteContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
        }

        public override void AsTable(string table)
        {
            throw new NotImplementedException();
        }

        public override Linq.DeleteContext<Parameter> Entrance()
        {
            throw new NotImplementedException();
        }

        public override Linq.UpdateContext<Parameter> Exists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.UpdateContext<Parameter> Exists(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override void From(string table)
        {
            throw new NotImplementedException();
        }

        public override int GetResult()
        {
            throw new NotImplementedException();
        }

        public override Linq.UpdateContext<Parameter> In<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.UpdateContext<Parameter> In(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.UpdateContext<Parameter> NotExists<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.UpdateContext<Parameter> NotExists(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.UpdateContext<Parameter> NotIn<Table>(AndOrOption option, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            throw new NotImplementedException();
        }

        public override Linq.UpdateContext<Parameter> NotIn(AndOrOption option, string expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.UpdateContext<Parameter> Where()
        {
            throw new NotImplementedException();
        }

        public override Linq.UpdateContext<Parameter> Where(Expression<Func<Parameter, object>> expression)
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
