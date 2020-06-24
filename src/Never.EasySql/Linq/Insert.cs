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
    public struct Insert<Table,Parameter>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal InsertContext<Table,Parameter> Context { get; set; }

        /// <summary>
        /// 从哪一张表插入
        /// </summary>
        public Insert<Table,Parameter> Into(string table)
        {
            this.Context.Into(table);
            return this;
        }

        /// <summary>
        /// 单条插入
        /// </summary>
        /// <returns></returns>
        public UnitInsertGrammar<Parameter> UseUnit()
        {
            this.Context.Entrance('s');
            return new UnitInsertGrammar<Parameter>() { Context = this.Context };
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <returns></returns>
        public BulkInsertGrammar<Parameter> UseBulk()
        {
            this.Context.Entrance('b');
            return new BulkInsertGrammar<Parameter>() { Context = this.Context };
        }
    }
}
