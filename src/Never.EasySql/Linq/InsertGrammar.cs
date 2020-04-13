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
    public struct InsertGrammar<Parameter>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal InsertContext<Parameter> Context { get; set; }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public InsertGrammar<Parameter> Colum(Expression<Func<Parameter, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public InsertGrammar<Parameter> ColumWithFunc(Expression<Func<Parameter, object>> expression, string function)
        {
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public InsertGrammar<Parameter> ColumWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value)
        {
            return this;
        }

        /// <summary>
        /// 返回最后插入语句
        /// </summary>
        public InsertGrammar<Parameter> LastInsertId()
        {
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public void GetResult()
        {
            return;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public Result GetResult<Result>()
        {
            return default(Result);
        }
    }

    /// <summary>
    /// insert 语法
    /// </summary>
    public struct InsertBulkGrammar<Parameter>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal InsertContext<Parameter> Context { get; set; }

        /// <summary>
        /// 获取结果
        /// </summary>
        public void GetResult()
        {
            return;
        }
    }
}
