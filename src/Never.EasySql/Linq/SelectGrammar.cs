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
    public struct SelectGrammar<Parameter, Table>
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
        public SelectGrammar<Parameter, Table> Select(char @all = '*')
        {
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SelectGrammar<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SelectGrammar<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <param name="on"></param>
        public SelectGrammar<Parameter, Table> Join<T2>(Expression<Func<Parameter, Table, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="on"></param>
        public SelectGrammar<Parameter, Table> LeftJoin<T2>(Expression<Func<Parameter, Table, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="on"></param>
        public SelectGrammar<Parameter, Table> RightJoin<T2>(Expression<Func<Parameter, Table, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="on"></param>
        public SelectGrammar<Parameter, Table> InnerJoin<T2>(Expression<Func<Parameter, Table, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public NWhere<Parameter, Table> Where(Expression<Func<Parameter, Table, object>> expression)
        {
            return new NWhere<Parameter, Table>()
            {
                select = this,
            };
        }

        /// <summary>
        /// 返回数组
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNow"></param>
        /// <returns></returns>
        public NToList<Parameter, Table> ToList(int pageNow, int pageSize)
        {
            //this.paged = new PagedSearch(pageNow, pageSize);
            return new NToList<Parameter, Table>()
            {
                crud = this,
            };
        }

        /// <summary>
        /// 返回单条
        /// </summary>
        /// <returns></returns>
        public NToSingle<Parameter, Table> ToSingle()
        {
            return new NToSingle<Parameter, Table>()
            {
                select = this,
            };
        }

        #endregion

        #region where and result

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NParameter">查询参数</typeparam>
        /// <typeparam name="NTable">查询结果对象</typeparam>
        public struct NToList<NParameter, NTable>
        {
            /// <summary>
            /// select
            /// </summary>
            internal SelectGrammar<NParameter, NTable> crud;

            /// <summary>
            /// 返回执行结果
            /// </summary>
            public IEnumerable<NTable> GetResult()
            {
                return null;
                //return this.crud.ToList();
            }
        }

        /// <summary>
        /// 返回单条
        /// </summary>
        /// <typeparam name="NParameter">查询参数</typeparam>
        /// <typeparam name="NT">查询结果对象</typeparam>
        public struct NToSingle<NParameter, NT>
        {
            /// <summary>
            /// select
            /// </summary>
            internal SelectGrammar<NParameter, NT> select;

            /// <summary>
            /// 返回执行结果
            /// </summary>
            public NT GetResult()
            {
                return default(NT);
                // return this.select.dao.QueryForObject<NT, NParameter>(this.select.Build(), select.sqlParameter);
            }
        }

        /// <summary>
        /// where 条件
        /// </summary>
        /// <typeparam name="NParameter">查询参数</typeparam>
        /// <typeparam name="NT">查询结果对象</typeparam>
        public struct NWhere<NParameter, NT>
        {
            /// <summary>
            /// select
            /// </summary>
            internal SelectGrammar<NParameter, NT> select;

            /// <summary>
            /// 返回数组
            /// </summary>
            /// <param name="pageSize"></param>
            /// <param name="pageNow"></param>
            /// <returns></returns>
            public NToList<NParameter, NT> ToList(int pageNow, int pageSize)
            {
                // this.select.paged = new PagedSearch(pageNow, pageSize);
                return new NToList<NParameter, NT>()
                {
                    crud = this.select,
                };
            }

            /// <summary>
            /// 返回单条
            /// </summary>
            /// <returns></returns>
            public NToSingle<NParameter, NT> ToSingle()
            {
                return new NToSingle<NParameter, NT>()
                {
                    select = this.select,
                };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter, NT> AndExists<T1>(Expression<Func<Parameter, Table, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="exists">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NT> AndExists(Func<TableInfo, string> exists)
            {
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter, NT> AndNotExists<T1>(Expression<Func<Parameter, Table, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NT> AndNotExists(Func<TableInfo, string> notexists)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter, NT> AndIn<T1>(Expression<Func<Parameter, Table, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NT> AndIn(Func<TableInfo, string> notexists)
            {
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter, NT> AndNotIn<T1>(Expression<Func<Parameter, Table, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NT> AndNotIn(Func<TableInfo, string> notexists)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter, NT> OrExists<T1>(Expression<Func<Parameter, Table, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="exists">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id Or table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NT> OrExists(Func<TableInfo, string> exists)
            {
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter, NT> OrNotExists<T1>(Expression<Func<Parameter, Table, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id Or table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NT> OrNotExists(Func<TableInfo, string> notexists)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter, NT> OrIn<T1>(Expression<Func<Parameter, Table, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NT> OrIn(Func<TableInfo, string> notexists)
            {
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter, NT> OrNotIn<T1>(Expression<Func<Parameter, Table, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter, NT> OrNotIn(Func<TableInfo, string> notexists)
            {
                return this;
            }

            /// <summary>
            /// 排序
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public NWhere<NParameter, NT> Orderby(Expression<Func<NT, object>> expression)
            {
                var model = expression.Body as ParameterExpression;
                if (model != null)
                {
                    // this.select.orderby.Add(string.Concat(model.Name, " asc"));
                    return this;
                }

                var member = expression.Body as MemberExpression;
                if (member != null)
                {
                    // this.select.orderby.Add(string.Concat(member.Member.Name, " asc"));
                    return this;
                }

                var unary = expression.Body as UnaryExpression;
                if (unary != null)
                {
                    member = unary.Operand as MemberExpression;
                    if (member != null)
                    {
                        // this.select.orderby.Add(string.Concat(member.Member.Name, " asc"));
                        return this;
                    }
                }

                return this;
            }

            /// <summary>
            /// 排序
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public NWhere<NParameter, NT> Orderbydesc(Expression<Func<NT, object>> expression)
            {
                var model = expression.Body as ParameterExpression;
                if (model != null)
                {
                    // this.select.orderby.Add(string.Concat(model.Name, " desc"));
                    return this;
                }

                var member = expression.Body as MemberExpression;
                if (member != null)
                {
                    // this.select.orderby.Add(string.Concat(member.Member.Name, " desc"));
                    return this;
                }

                var unary = expression.Body as UnaryExpression;
                if (unary != null)
                {
                    member = unary.Operand as MemberExpression;
                    if (member != null)
                    {
                        //  this.select.orderby.Add(string.Concat(member.Member.Name, " desc"));
                        return this;
                    }
                }

                return this;
            }
        }

        #endregion
    }
}
