using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// insert 语法
    /// </summary>
    public struct SingleInsertGrammar<Table, Parameter>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal InsertContext<Table, Parameter> Context { get; set; }

        /// <summary>
        /// 插入的所有字段名
        /// </summary>
        public SingleInsertGrammar<Table, Parameter> InsertAll()
        {
            this.Context.InsertAll();
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public SingleInsertGrammar<Table, Parameter> Colum(Expression<Func<Table, object>> keyValue)
        {
            this.Context.Colum(keyValue);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public SingleInsertGrammar<Table, Parameter> Colum(Expression<Func<Table, object>> key, Expression<Func<Parameter, object>> value)
        {
            this.Context.Colum(key, value);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public SingleInsertGrammar<Table, Parameter> ColumWithFunc(Expression<Func<Table, object>> key, string value)
        {
            this.Context.ColumWithFunc(key, value);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public SingleInsertGrammar<Table, Parameter> ColumWithValue<TMember>(Expression<Func<Table, TMember>> key, TMember value)
        {
            this.Context.ColumWithValue(key, value);
            return this;
        }

        /// <summary>
        /// 返回最后插入语句
        /// </summary>
        public SingleInsertGrammar<Table, Parameter> LastInsertId()
        {
            this.Context.InsertLastInsertId();
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public void GetResult()
        {
            this.Context.GetResult();
            return;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public Result GetResult<Result>()
        {
            return this.Context.GetResult<Result>();
        }
    }

    /// <summary>
    /// insert 语法
    /// </summary>
    public struct BulkInsertGrammar<Table, Parameter>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal InsertContext<Table, Parameter> Context { get; set; }

        /// <summary>
        /// 插入的所有字段名
        /// </summary>
        public BulkInsertGrammar<Table, Parameter> InsertAll()
        {
            this.Context.InsertAll();
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public BulkInsertGrammar<Table, Parameter> Colum(Expression<Func<Table, object>> keyValue)
        {
            this.Context.Colum(keyValue);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public BulkInsertGrammar<Table, Parameter> Colum<UnitParameter>(Expression<Func<Table, object>> key, Expression<Func<UnitParameter, object>> value)
        {
            string columnName = this.Context.FindColumnName(key, Linq.Context.FindTableInfo<Table>(), out _);
            string parameterName = this.Context.FindColumnName(value, Linq.Context.FindTableInfo<UnitParameter>(), out _);
            this.Context.InsertColumn(columnName, parameterName, false);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public BulkInsertGrammar<Table, Parameter> ColumWithFunc(Expression<Func<Table, object>> key, string value)
        {
            this.Context.ColumWithFunc(key, value);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public BulkInsertGrammar<Table, Parameter> ColumWithValue<TMember>(Expression<Func<Table, TMember>> key, TMember value)
        {
            this.Context.ColumWithValue(key, value);
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public void GetResult()
        {
            this.Context.GetResult();
            return;
        }
    }
}
