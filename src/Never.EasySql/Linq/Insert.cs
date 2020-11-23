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
    public struct Insert<Parameter, Table>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal InsertContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 从哪一张表插入
        /// </summary>
        public Insert<Parameter, Table> Into(string table)
        {
            this.Context.Into(table);
            return this;
        }

        /// <summary>
        /// 单条插入
        /// </summary>
        /// <returns></returns>
        public SingleInsertGrammar<Parameter, Table> UseSingle()
        {
            return new SingleInsertGrammar<Parameter, Table>() { Context = this.Context }.StartInsertRecord();
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <returns></returns>
        public BulkInsertGrammar<Parameter, Table> UseBulk()
        {
            return new BulkInsertGrammar<Parameter, Table>() { Context = this.Context }.StartInsertRecord();
        }
    }
}
