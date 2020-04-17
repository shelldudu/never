using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 更新操作
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    public struct Update<Parameter>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal UpdateContext<Parameter> Context { get; set; }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public Update<Parameter> As(string table)
        {
            this.Context.AsTable(table);
            return this;
        }

        /// <summary>
        /// 从哪一张表更新
        /// </summary>
        public Update<Parameter> From(string table)
        {
            this.Context.From(table);
            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="expression"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public Update<Parameter> Join<Table>(string tableAsName, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> and)
        {

            return this;
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="expression"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public Update<Parameter> InnerJoin<Table>(string tableAsName, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> and)
        {
            return this;
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="expression"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public Update<Parameter> LeftJoin<Table>(string tableAsName, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> and)
        {
            return this;
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="expression"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public Update<Parameter> RightJoin<Table>(string tableAsName, Expression<Func<Parameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
        {
            return this;
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColum<TMember>(Expression<Func<Parameter, TMember>> expression)
        {
            return new UpdateGrammar<Parameter>() { Context = this.Context }.Entrance().SetColumn(expression);
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColumWithFunc<TMember>(Expression<Func<Parameter, TMember>> expression, string value)
        {
            return new UpdateGrammar<Parameter>() { Context = this.Context }.Entrance().SetColumnWithFunc(expression, value);
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColumWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value)
        {
            return new UpdateGrammar<Parameter>() { Context = this.Context }.Entrance().SetColumnWithValue(expression, value);
        }
    }
}
