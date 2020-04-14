using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 单条查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="Table">查询结果对象</typeparam>
    public struct SingleSelectGrammar<Parameter, Table>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal SelectContext<Parameter, Table> Context { get; set; }

        #region linq

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table> Select(char @all = '*')
        {
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <param name="on"></param>
        public SingleSelectGrammar<Parameter, Table> Join<T2>(Expression<Func<Parameter, Table, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="on"></param>
        public SingleSelectGrammar<Parameter, Table> LeftJoin<T2>(Expression<Func<Parameter, Table, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="on"></param>
        public SingleSelectGrammar<Parameter, Table> RightJoin<T2>(Expression<Func<Parameter, Table, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="on"></param>
        public SingleSelectGrammar<Parameter, Table> InnerJoin<T2>(Expression<Func<Parameter, Table, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public NWhere<Parameter, Table> Where()
        {
            return new NWhere<Parameter, Table>()
            {
                crud = this,
            };
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public NWhere<Parameter, Table> Where(Expression<Func<Parameter, Table, object>> expression)
        {
            return new NWhere<Parameter, Table>()
            {
                crud = this,
            };
        }

        /// <summary>
        /// 返回执行结果
        /// </summary>
        public Table GetResult()
        {
            return default(Table);
        }

        #endregion

        #region where

        /// <summary>
        /// where 条件
        /// </summary>
        /// <typeparam name="NParameter">查询参数</typeparam>
        /// <typeparam name="NTable">查询结果对象</typeparam>
        public struct NWhere<NParameter, NTable>
        {
            /// <summary>
            /// select
            /// </summary>
            internal SingleSelectGrammar<NParameter, NTable> crud;

            /// <summary>
            /// 返回执行结果
            /// </summary>
            public NTable GetResult()
            {
                return this.crud.GetResult();
            }


            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter,NTable> AndExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.Exists(AndOrOption.and, expression, null);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            /// <param name="where">另外一张表的where语句</param>
            public NWhere<NParameter,NTable> AndExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.Exists(AndOrOption.and, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>        
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter,NTable> AndExists(string expression)
            {
                this.crud.Context.Exists(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter,NTable> AndNotExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.NotExists(AndOrOption.and, expression, null);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            /// <param name="where">另外一张表的where语句</param>
            public NWhere<NParameter,NTable> AndNotExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.NotExists(AndOrOption.and, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter,NTable> AndNotExists(string expression)
            {
                this.crud.Context.NotExists(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter,NTable> OrExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.Exists(AndOrOption.or, expression, null);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            /// <param name="where">另外一张表的where语句</param>
            public NWhere<NParameter,NTable> OrExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.Exists(AndOrOption.or, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>        
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter,NTable> OrExists(string expression)
            {
                this.crud.Context.Exists(AndOrOption.or, expression);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter,NTable> OrNotExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.NotExists(AndOrOption.or, expression, null);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            /// <param name="where">另外一张表的where语句</param>
            public NWhere<NParameter,NTable> OrNotExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.NotExists(AndOrOption.or, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter,NTable> OrNotExists(string expression)
            {
                this.crud.Context.NotExists(AndOrOption.or, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression"></param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter,NTable> AndIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.In(AndOrOption.and, expression, null);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            /// <param name="where">where条件</param>
            public NWhere<NParameter,NTable> AndIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.In(AndOrOption.and, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter,NTable> AndIn(string expression)
            {
                this.crud.Context.In(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter,NTable> AndNotIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.NotIn(AndOrOption.and, expression, null);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            /// <param name="where">where条件</param>
            public NWhere<NParameter,NTable> AndNotIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.NotIn(AndOrOption.and, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter,NTable> AndNotIn(string expression)
            {
                this.crud.Context.NotIn(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter,NTable> OrIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.In(AndOrOption.or, expression, null);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <param name="where">where</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter,NTable> OrIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.In(AndOrOption.or, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter,NTable> OrIn(string expression)
            {
                this.crud.Context.In(AndOrOption.or, expression);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter,NTable> OrNotIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.NotIn(AndOrOption.or, expression, null);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <param name="where"></param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter,NTable> OrNotIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.NotIn(AndOrOption.or, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter,NTable> OrNotIn(string expression)
            {
                this.crud.Context.NotIn(AndOrOption.or, expression);
                return this;
            }
        }

        #endregion
    }

    /// <summary>
    /// 多条查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="Table">查询结果对象</typeparam>
    public struct EnumerableSelectGrammar<Parameter, Table>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal SelectContext<Parameter, Table> Context { get; set; }

        #region linq

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table> Select(char @all = '*')
        {
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <param name="on"></param>
        public EnumerableSelectGrammar<Parameter, Table> Join<T2>(Expression<Func<Parameter, Table, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="on"></param>
        public EnumerableSelectGrammar<Parameter, Table> LeftJoin<T2>(Expression<Func<Parameter, Table, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="on"></param>
        public EnumerableSelectGrammar<Parameter, Table> RightJoin<T2>(Expression<Func<Parameter, Table, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="on"></param>
        public EnumerableSelectGrammar<Parameter, Table> InnerJoin<T2>(Expression<Func<Parameter, Table, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public NWhere<Parameter, Table> Where()
        {
            return new NWhere<Parameter, Table>()
            {
                crud = this,
            };
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public NWhere<Parameter, Table> Where(Expression<Func<Parameter, Table, object>> expression)
        {
            return new NWhere<Parameter, Table>()
            {
                crud = this,
            };
        }

        /// <summary>
        /// 返回执行结果
        /// </summary>
        public IEnumerable<Table> GetResult()
        {
            return default(IEnumerable<Table>);
        }

        #endregion

        #region where

        /// <summary>
        /// where 条件
        /// </summary>
        /// <typeparam name="NParameter">查询参数</typeparam>
        /// <typeparam name="NTable">查询结果对象</typeparam>
        public struct NWhere<NParameter, NTable>
        {
            /// <summary>
            /// select
            /// </summary>
            internal EnumerableSelectGrammar<NParameter, NTable> crud;

            /// <summary>
            /// 返回执行结果
            /// </summary>
            public IEnumerable<NTable> GetResult()
            {
                return this.crud.GetResult();
            }


            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter, NTable> AndExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.Exists(AndOrOption.and, expression, null);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            /// <param name="where">另外一张表的where语句</param>
            public NWhere<NParameter, NTable> AndExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.Exists(AndOrOption.and, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>        
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NTable> AndExists(string expression)
            {
                this.crud.Context.Exists(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter, NTable> AndNotExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.NotExists(AndOrOption.and, expression, null);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            /// <param name="where">另外一张表的where语句</param>
            public NWhere<NParameter, NTable> AndNotExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.NotExists(AndOrOption.and, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NTable> AndNotExists(string expression)
            {
                this.crud.Context.NotExists(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter, NTable> OrExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.Exists(AndOrOption.or, expression, null);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            /// <param name="where">另外一张表的where语句</param>
            public NWhere<NParameter, NTable> OrExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.Exists(AndOrOption.or, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>        
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NTable> OrExists(string expression)
            {
                this.crud.Context.Exists(AndOrOption.or, expression);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter, NTable> OrNotExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.NotExists(AndOrOption.or, expression, null);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            /// <param name="where">另外一张表的where语句</param>
            public NWhere<NParameter, NTable> OrNotExists<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.NotExists(AndOrOption.or, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NTable> OrNotExists(string expression)
            {
                this.crud.Context.NotExists(AndOrOption.or, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression"></param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter, NTable> AndIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.In(AndOrOption.and, expression, null);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            /// <param name="where">where条件</param>
            public NWhere<NParameter, NTable> AndIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.In(AndOrOption.and, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NTable> AndIn(string expression)
            {
                this.crud.Context.In(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter, NTable> AndNotIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.NotIn(AndOrOption.and, expression, null);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            /// <param name="where">where条件</param>
            public NWhere<NParameter, NTable> AndNotIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.NotIn(AndOrOption.and, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NTable> AndNotIn(string expression)
            {
                this.crud.Context.NotIn(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter, NTable> OrIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.In(AndOrOption.or, expression, null);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <param name="where">where</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter, NTable> OrIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.In(AndOrOption.or, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NTable> OrIn(string expression)
            {
                this.crud.Context.In(AndOrOption.or, expression);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter, NTable> OrNotIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression)
            {
                this.crud.Context.NotIn(AndOrOption.or, expression, null);
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <param name="expression">expression</param>
            /// <param name="where"></param>
            /// <typeparam name="Table2">另外的表中</typeparam>
            public NWhere<NParameter, NTable> OrNotIn<Table2>(Expression<Func<NParameter, Table2, bool>> expression, Expression<Func<Table2, bool>> where)
            {
                this.crud.Context.NotIn(AndOrOption.or, expression, where);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NTable> OrNotIn(string expression)
            {
                this.crud.Context.NotIn(AndOrOption.or, expression);
                return this;
            }
        }

        #endregion
    }
}
