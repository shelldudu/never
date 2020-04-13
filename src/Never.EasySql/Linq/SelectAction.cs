using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="Table">查询结果对象</typeparam>
    public struct SelectAction<Parameter, Table>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal SelectContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public SelectAction<Parameter, Table> As(string table)
        {
            this.Context.AsTable(table);
            return this;
        }

        /// <summary>
        /// 从哪一张表更新
        /// </summary>
        public SelectAction<Parameter, Table> From(string table)
        {
            this.Context.From(table);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SelectGrammar<Parameter, Table> Select(char @all = '*')
        {
            return new SelectGrammar<Parameter, Table>().Select();
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public SelectGrammar<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            return new SelectGrammar<Parameter, Table>().Select(expression);
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public SelectGrammar<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            return new SelectGrammar<Parameter, Table>().Select(expression, @as);
        }
    }
}
