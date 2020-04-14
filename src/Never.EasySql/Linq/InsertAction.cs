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
    public struct InsertAction<Parameter>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal _InsertContext<Parameter> Context { get; set; }

        /// <summary>
        /// 从哪一张表插入
        /// </summary>
        public InsertAction<Parameter> From(string table)
        {
            this.Context.Into(table);
            return this;
        }

        /// <summary>
        /// 单条插入
        /// </summary>
        /// <returns></returns>
        public SingleInsertGrammar<Parameter> ToSingle()
        {
            this.Context.Entrance('s');
            return new SingleInsertGrammar<Parameter>() { Context = this.Context };
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <returns></returns>
        public BulkInsertGrammar<Parameter> ToBulk()
        {
            this.Context.Entrance('b');
            return new BulkInsertGrammar<Parameter>() { Context = this.Context };
        }
    }
}
