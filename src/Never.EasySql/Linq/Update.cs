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
            this.Context.As(table);
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
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table> Join<Table>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateJoinGrammar<Parameter, Table>(@as, JoinOption.Join) { update = new UpdateGrammar<Parameter>() { Context = this.Context } };
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table> InnerJoin<Table>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateJoinGrammar<Parameter, Table>(@as, JoinOption.InnerJoin) { update = new UpdateGrammar<Parameter>() { Context = this.Context } };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table> LeftJoin<Table>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateJoinGrammar<Parameter, Table>(@as, JoinOption.LeftJoin) { update = new UpdateGrammar<Parameter>() { Context = this.Context } };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table> RightJoin<Table>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateJoinGrammar<Parameter, Table>(@as, JoinOption.RightJoin) { update = new UpdateGrammar<Parameter>() { Context = this.Context } };
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColumn<TMember>(Expression<Func<Parameter, TMember>> expression)
        {
            return new UpdateGrammar<Parameter>() { Context = this.Context }.StartSetColumn().SetColumn(expression);
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColumWithFunc<TMember>(Expression<Func<Parameter, TMember>> expression, string value)
        {
            return new UpdateGrammar<Parameter>() { Context = this.Context }.StartSetColumn().SetColumnWithFunc(expression, value);
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColumWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value)
        {
            return new UpdateGrammar<Parameter>() { Context = this.Context }.StartSetColumn().SetColumnWithValue(expression, value);
        }
    }
}
