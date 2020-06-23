using Never.EasySql.Labels;
using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 插入语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    public class InsertingContext<Parameter, Table> : InsertContext<Parameter, Table>
    {
        private readonly string cacheId;
        private int textLength;
        private int setTimes;
        private string tableName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheId"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public InsertingContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.cacheId = cacheId;
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

        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="table"></param>
        public override Linq.InsertContext<Parameter, Table> Into(string table)
        {
            this.tableName = table;
            return this;
        }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        public override Linq.InsertContext<Parameter, Table> Entrance(char flag)
        {
            this.tableName = this.tableName.IsNullOrEmpty() ? this.FindTableName<Parameter>(tableInfo) : this.tableName;
            var label = new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("insert into ", this.FormatTable(this.tableName)) };
            this.textLength += label.SqlText.Length;
            this.labels.Add(label);

            return this;
        }

        public override Result GetResult<Result>()
        {
            throw new NotImplementedException();
        }

        public override void GetResult()
        {
            throw new NotImplementedException();
        }

        public override Linq.InsertContext<Parameter, Table> InsertLastInsertId()
        {
            throw new NotImplementedException();
        }

        public override Linq.InsertContext<Parameter, Table> Colum(Expression<Func<Table, object>> expression)
        {
            throw new NotImplementedException();
        }

        public override Linq.InsertContext<Parameter, Table> ColumWithFunc(Expression<Func<Table, object>> expression, string function)
        {
            throw new NotImplementedException();
        }

        public override Linq.InsertContext<Parameter, Table> ColumWithValue<TMember>(Expression<Func<Table, TMember>> expression, TMember value)
        {
            throw new NotImplementedException();
        }
    }
}
