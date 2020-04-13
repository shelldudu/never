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
    public struct SingleInsertGrammar<Parameter>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal InsertContext<Parameter> Context { get; set; }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public SingleInsertGrammar<Parameter> Colum(Expression<Func<Parameter, object>> expression)
        {
            this.Context.Colum(expression);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public SingleInsertGrammar<Parameter> ColumWithFunc(Expression<Func<Parameter, object>> expression, string function)
        {
            this.Context.ColumWithFunc(expression, function);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public SingleInsertGrammar<Parameter> ColumWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value)
        {
            this.Context.ColumWithValue(expression, value);
            return this;
        }

        /// <summary>
        /// 返回最后插入语句
        /// </summary>
        public SingleInsertGrammar<Parameter> LastInsertId()
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
    public struct EnumerableInsertGrammar<Parameter>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal InsertContext<Parameter> Context { get; set; }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public EnumerableInsertGrammar<Parameter> Colum(Expression<Func<Parameter, object>> expression)
        {
            this.Context.Colum(expression);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public EnumerableInsertGrammar<Parameter> ColumWithFunc(Expression<Func<Parameter, object>> expression, string function)
        {
            this.Context.ColumWithFunc(expression, function);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public EnumerableInsertGrammar<Parameter> ColumWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value)
        {
            this.Context.ColumWithValue(expression, value);
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
