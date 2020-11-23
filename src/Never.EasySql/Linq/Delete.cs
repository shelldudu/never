using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 删除操作
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    public struct Delete<Parameter, Table>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal DeleteContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 删除的表名
        /// </summary>
        public Delete<Parameter, Table> As(string table)
        {
            this.Context.As(table);
            return this;
        }

        /// <summary>
        /// 从哪一张表删除
        /// </summary>
        public Delete<Parameter, Table> From(string table)
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
        public DeleteJoinGrammar<Parameter, Table, Table1> Join<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new DeleteJoinGrammar<Parameter, Table, Table1>(@as, JoinOption.Join) { delete = new DeleteGrammar<Parameter, Table>() { Context = this.Context } };
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteJoinGrammar<Parameter, Table, Table1> InnerJoin<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new DeleteJoinGrammar<Parameter, Table, Table1>(@as, JoinOption.InnerJoin) { delete = new DeleteGrammar<Parameter, Table>() { Context = this.Context } };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteJoinGrammar<Parameter, Table, Table1> LeftJoin<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new DeleteJoinGrammar<Parameter, Table, Table1>(@as, JoinOption.LeftJoin) { delete = new DeleteGrammar<Parameter, Table>() { Context = this.Context } };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteJoinGrammar<Parameter, Table, Table1> RightJoin<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new DeleteJoinGrammar<Parameter, Table, Table1>(@as, JoinOption.RightJoin) { delete = new DeleteGrammar<Parameter, Table>() { Context = this.Context } };
        }

        /// <summary>
        /// where
        /// </summary>
        public DeleteWhereGrammar<Parameter, Table> Where()
        {
            return new DeleteGrammar<Parameter, Table>() { Context = this.Context }.StartDeleteRecord().Where();
        }

        /// <summary>
        /// where
        /// </summary>
        public DeleteWhereGrammar<Parameter, Table> Where(Expression<Func<Parameter, Table, bool>> expression)
        {
            return new DeleteGrammar<Parameter, Table>() { Context = this.Context }.StartDeleteRecord().Where(expression);
        }
    }
}
