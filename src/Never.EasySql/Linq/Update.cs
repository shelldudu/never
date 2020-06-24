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
    /// <typeparam name="Table"></typeparam>
    public struct Update<Table,Parameter>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal UpdateContext<Table,Parameter> Context { get; set; }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public Update<Table,Parameter> As(string table)
        {
            this.Context.As(table);
            return this;
        }

        /// <summary>
        /// 从哪一张表更新
        /// </summary>
        public Update<Table,Parameter> From(string table)
        {
            this.Context.From(table);
            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Table,Parameter, Table1> Join<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateJoinGrammar<Table,Parameter, Table1>(@as, JoinOption.Join) { update = new UpdateGrammar<Table,Parameter>() { Context = this.Context } };
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Table,Parameter, Table1> InnerJoin<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateJoinGrammar<Table,Parameter, Table1>(@as, JoinOption.InnerJoin) { update = new UpdateGrammar<Table,Parameter>() { Context = this.Context } };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Table,Parameter, Table1> LeftJoin<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateJoinGrammar<Table,Parameter, Table1>(@as, JoinOption.LeftJoin) { update = new UpdateGrammar<Table,Parameter>() { Context = this.Context } };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Table,Parameter, Table1> RightJoin<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateJoinGrammar<Table,Parameter, Table1>(@as, JoinOption.RightJoin) { update = new UpdateGrammar<Table,Parameter>() { Context = this.Context } };
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Table,Parameter> SetColumn<TMember>(Expression<Func<Table, TMember>> expression)
        {
            return new UpdateGrammar<Table,Parameter>() { Context = this.Context }.StartSetColumn().SetColumn(expression);
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Table,Parameter> SetColumWithFunc<TMember>(Expression<Func<Table, TMember>> expression, string value)
        {
            return new UpdateGrammar<Table,Parameter>() { Context = this.Context }.StartSetColumn().SetColumnWithFunc(expression, value);
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Table,Parameter> SetColumWithValue<TMember>(Expression<Func<Table, TMember>> expression, TMember value)
        {
            return new UpdateGrammar<Table,Parameter>() { Context = this.Context }.StartSetColumn().SetColumnWithValue(expression, value);
        }
    }
}
