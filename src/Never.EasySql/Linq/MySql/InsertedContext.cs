using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.MySql
{
    public sealed class InsertedContext<Parameter> : Linq.InsertContext<Parameter>
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
        public InsertedContext(LinqSqlTag sqlTag, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.sqlTag = sqlTag;
        }

        public override Linq.InsertContext<Parameter> Colum(Expression<Func<Parameter, object>> expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.InsertContext<Parameter> ColumWithFunc(Expression<Func<Parameter, object>> expression, string function)
        {
            throw new NotImplementedException();
        }

        public override Linq.InsertContext<Parameter> ColumWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value)
        {
            throw new NotImplementedException();
        }

        public override Linq.InsertContext<Parameter> Entrance(char flag)
        {
            throw new NotImplementedException();
        }

        public override Result GetResult<Result>()
        {
            throw new NotImplementedException();
        }

        public override void GetResult()
        {
            throw new NotImplementedException();
        }

        public override Linq.InsertContext<Parameter> InsertLastInsertId()
        {
            throw new NotImplementedException();
        }

        public override void Into(string table)
        {
            throw new NotImplementedException();
        }

        protected override string Format(string text)
        {
            throw new NotImplementedException();
        }
    }
}
