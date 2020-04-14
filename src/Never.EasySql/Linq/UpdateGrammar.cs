using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// update操作语法
    /// </summary>
    public struct UpdateGrammar<Parameter>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal UpdateContext<Parameter> Context { get; set; }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        internal UpdateGrammar<Parameter> Entrance()
        {
            this.Context.Entrance();
            return this;
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColumn<TMember>(Expression<Func<Parameter, TMember>> expression)
        {
            this.Context.SetColumn<TMember>(expression);
            return this;
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColumnWithFunc<TMember>(Expression<Func<Parameter, TMember>> expression, string value)
        {
            this.Context.SetColumnWithFunc<TMember>(expression, value);
            return this;
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColumnWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value)
        {
            this.Context.SetColumnWithValue<TMember>(expression, value);
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public int GetResult()
        {
            return Context.GetResult();
        }

        /// <summary>
        /// where
        /// </summary>
        public NWhere<Parameter> Where()
        {
            this.Context.Where();
            return new NWhere<Parameter>() { crud = this };
        }

        /// <summary>
        /// where
        /// </summary>
        public NWhere<Parameter> Where(Expression<Func<Parameter, object>> expression)
        {
            this.Context.Where(expression);
            return new NWhere<Parameter>() { crud = this };
        }

        /// <summary>
        /// where 条件
        /// </summary>
        /// <typeparam name="NParameter">查询参数</typeparam>
        public struct NWhere<NParameter>
        {
            /// <summary>
            /// 
            /// </summary>
            internal UpdateGrammar<NParameter> crud;

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table">另外的表中</typeparam>
            public NWhere<NParameter> AndExists<Table>(Expression<Func<NParameter, Table, bool>> expression)
            {
                this.crud.Context.Exists(AndOrOption.and, expression, null);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table">另外的表中</typeparam>
            /// <param name="where">另外一张表的where语句</param>
            public NWhere<NParameter> AndExists<Table>(Expression<Func<NParameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
            {
                this.crud.Context.Exists(AndOrOption.and, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>        
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> AndExists(string expression)
            {
                this.crud.Context.Exists(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table">另外的表中</typeparam>
            public NWhere<NParameter> AndNotExists<Table>(Expression<Func<NParameter, Table, bool>> expression)
            {
                this.crud.Context.NotExists(AndOrOption.and, expression, null);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table">另外的表中</typeparam>
            /// <param name="where">另外一张表的where语句</param>
            public NWhere<NParameter> AndNotExists<Table>(Expression<Func<NParameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
            {
                this.crud.Context.NotExists(AndOrOption.and, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> AndNotExists(string expression)
            {
                this.crud.Context.NotExists(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table">另外的表中</typeparam>
            public NWhere<NParameter> OrExists<Table>(Expression<Func<NParameter, Table, bool>> expression)
            {
                this.crud.Context.Exists(AndOrOption.or, expression, null);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table">另外的表中</typeparam>
            /// <param name="where">另外一张表的where语句</param>
            public NWhere<NParameter> OrExists<Table>(Expression<Func<NParameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
            {
                this.crud.Context.Exists(AndOrOption.or, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>        
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> OrExists(string expression)
            {
                this.crud.Context.Exists(AndOrOption.or, expression);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table">另外的表中</typeparam>
            public NWhere<NParameter> OrNotExists<Table>(Expression<Func<NParameter, Table, bool>> expression)
            {
                this.crud.Context.NotExists(AndOrOption.or, expression, null);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table">另外的表中</typeparam>
            /// <param name="where">另外一张表的where语句</param>
            public NWhere<NParameter> OrNotExists<Table>(Expression<Func<NParameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
            {
                this.crud.Context.NotExists(AndOrOption.or, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> OrNotExists(string expression)
            {
                this.crud.Context.NotExists(AndOrOption.or, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression"></param>
            /// <typeparam name="Table">另外的表中</typeparam>
            public NWhere<NParameter> AndIn<Table>(Expression<Func<NParameter, Table, bool>> expression)
            {
                this.crud.Context.In(AndOrOption.and, expression, null);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table">另外的表中</typeparam>
            /// <param name="where">where条件</param>
            public NWhere<NParameter> AndIn<Table>(Expression<Func<NParameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
            {
                this.crud.Context.In(AndOrOption.and, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> AndIn(string expression)
            {
                this.crud.Context.In(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table">另外的表中</typeparam>
            public NWhere<NParameter> AndNotIn<Table>(Expression<Func<NParameter, Table, bool>> expression)
            {
                this.crud.Context.NotIn(AndOrOption.and, expression, null);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table">另外的表中</typeparam>
            /// <param name="where">where条件</param>
            public NWhere<NParameter> AndNotIn<Table>(Expression<Func<NParameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
            {
                this.crud.Context.NotIn(AndOrOption.and, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> AndNotIn(string expression)
            {
                this.crud.Context.NotIn(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table">另外的表中</typeparam>
            public NWhere<NParameter> OrIn<Table>(Expression<Func<NParameter, Table, bool>> expression)
            {
                this.crud.Context.In(AndOrOption.or, expression, null);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <param name="where">where</param>
            /// <typeparam name="Table">另外的表中</typeparam>
            public NWhere<NParameter> OrIn<Table>(Expression<Func<NParameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
            {
                this.crud.Context.In(AndOrOption.or, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> OrIn(string expression)
            {
                this.crud.Context.In(AndOrOption.or, expression);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table">另外的表中</typeparam>
            public NWhere<NParameter> OrNotIn<Table>(Expression<Func<NParameter, Table, bool>> expression)
            {
                this.crud.Context.NotIn(AndOrOption.or, expression, null);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <param name="where"></param>
            /// <typeparam name="Table">另外的表中</typeparam>
            public NWhere<NParameter> OrNotIn<Table>(Expression<Func<NParameter, Table, bool>> expression, Expression<Func<Table, bool>> where)
            {
                this.crud.Context.NotIn(AndOrOption.or, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> OrNotIn(string expression)
            {
                this.crud.Context.NotIn(AndOrOption.or, expression);
                return this;
            }

            /// <summary>
            /// 获取结果
            /// </summary>
            public int GetResult()
            {
                return this.crud.GetResult();
            }
        }
    }
}
