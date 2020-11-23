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
    public struct Update<Parameter, Table>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal UpdateContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public Update<Parameter, Table> As(string table)
        {
            this.Context.As(table);
            return this;
        }

        /// <summary>
        /// 从哪一张表更新
        /// </summary>
        public Update<Parameter, Table> From(string table)
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
        public UpdateJoinGrammar<Parameter, Table, Table1> Join<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateJoinGrammar<Parameter, Table, Table1>(@as, JoinOption.Join) { update = new UpdateGrammar<Parameter, Table>() { Context = this.Context } };
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table, Table1> InnerJoin<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateJoinGrammar<Parameter, Table, Table1>(@as, JoinOption.InnerJoin) { update = new UpdateGrammar<Parameter, Table>() { Context = this.Context } };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table, Table1> LeftJoin<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateJoinGrammar<Parameter, Table, Table1>(@as, JoinOption.LeftJoin) { update = new UpdateGrammar<Parameter, Table>() { Context = this.Context } };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table, Table1> RightJoin<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateJoinGrammar<Parameter, Table, Table1>(@as, JoinOption.RightJoin) { update = new UpdateGrammar<Parameter, Table>() { Context = this.Context } };
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter, Table> SetColumn<TMember>(Expression<Func<Table, TMember>> keyValue)
        {
            return new UpdateGrammar<Parameter, Table>() { Context = this.Context }.StartSetColumn().SetColumn(keyValue);
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter, Table> SetColumn<TMember>(Expression<Func<Table, TMember>> key, Expression<Func<Parameter, TMember>> value)
        {
            return new UpdateGrammar<Parameter, Table>() { Context = this.Context }.StartSetColumn().SetColumn(key, value);
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter, Table> SetColumnWithFunc<TMember>(Expression<Func<Table, TMember>> key, string value)
        {
            return new UpdateGrammar<Parameter, Table>() { Context = this.Context }.StartSetColumn().SetColumnWithFunc(key, value);
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter, Table> SetColumnWithValue<TMember>(Expression<Func<Table, TMember>> key, TMember value)
        {
            return new UpdateGrammar<Parameter, Table>() { Context = this.Context }.StartSetColumn().SetColumnWithValue(key, value);
        }
    }
}
