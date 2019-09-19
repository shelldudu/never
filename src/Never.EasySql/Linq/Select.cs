using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 单个表的查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="T">查询结果对象</typeparam>
    public struct Select<Parameter, T>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal Context Context { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        internal EasySqlParameter<Parameter> SqlParameter { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        public Select<Parameter, T> SelectColum(Expression<Func<Parameter, T, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public NWhere<Parameter, T> Where(Expression<Func<Parameter, T, object>> expression)
        {
            return new NWhere<Parameter, T>();
        }

        /// <summary>
        /// where 条件
        /// </summary>
        /// <typeparam name="NParameter">查询参数</typeparam>
        /// <typeparam name="NT">查询结果对象</typeparam>
        public struct NWhere<NParameter, NT>
        {
            /// <summary>
            /// 返回列表
            /// </summary>
            public NToList<T> ToList(int pageSize, int pageNow)
            {
                return new NToList<T>();
            }

            /// <summary>
            /// 返回单条
            /// </summary>
            public NToSingle<T> ToSingle()
            {
                return new NToSingle<T>();
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public Exists<T> Exists<T1>(Expression<Func<Parameter, T, T1, object>> expression)
            {
                return new Exists<T>();
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="exists">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public Exists<T> Exists(Func<TableInfo, string> exists)
            {
                return new Exists<T>();
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NotExists<T> NotExists<T1>(Expression<Func<Parameter, T, T1, object>> expression)
            {
                return new NotExists<T>();
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public Exists<T> NotExists(Func<TableInfo, string> notexists)
            {
                return new Exists<T>();
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public In<T> In<T1>(Expression<Func<Parameter, T, T1, object>> expression)
            {
                return new In<T>();
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public Exists<T> In(Func<TableInfo, string> notexists)
            {
                return new Exists<T>();
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NotIn<T> NotIn<T1>(Expression<Func<Parameter, T, T1, object>> expression)
            {
                return new NotIn<T>();
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public Exists<T> NotIn(Func<TableInfo, string> notexists)
            {
                return new Exists<T>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT">查询结果对象</typeparam>
        public struct NToList<NT>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public IEnumerable<T> GetResult()
            {
                return System.Linq.Enumerable.Empty<T>();
            }
        }

        /// <summary>
        /// 返回单条
        /// </summary>
        /// <typeparam name="NT">查询结果对象</typeparam>
        public struct NToSingle<NT>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public T GetResult()
            {
                return default(T);
            }
        }

        /// <summary>
        /// 存在
        /// </summary>
        public struct Exists<NT>
        {
            /// <summary>
            /// 返回列表
            /// </summary>
            public NToList<T> ToList(int pageSize, int pageNow)
            {
                return new NToList<T>();
            }

            /// <summary>
            /// 返回单条
            /// </summary>
            public NToSingle<T> ToSingle()
            {
                return new NToSingle<T>();
            }
        }

        /// <summary>
        /// 不存在
        /// </summary>
        public struct NotExists<NT>
        {
            /// <summary>
            /// 返回列表
            /// </summary>
            public NToList<T> ToList(int pageSize, int pageNow)
            {
                return new NToList<T>();
            }

            /// <summary>
            /// 返回单条
            /// </summary>
            public NToSingle<T> ToSingle()
            {
                return new NToSingle<T>();
            }
        }

        /// <summary>
        /// 存在
        /// </summary>
        public struct In<NT>
        {
            /// <summary>
            /// 返回列表
            /// </summary>
            public NToList<T> ToList(int pageSize, int pageNow)
            {
                return new NToList<T>();
            }

            /// <summary>
            /// 返回单条
            /// </summary>
            public NToSingle<T> ToSingle()
            {
                return new NToSingle<T>();
            }
        }

        /// <summary>
        /// 不存在
        /// </summary>
        public struct NotIn<NT>
        {
            /// <summary>
            /// 返回列表
            /// </summary>
            public NToList<T> ToList(int pageSize, int pageNow)
            {
                return new NToList<T>();
            }

            /// <summary>
            /// 返回单条
            /// </summary>
            public NToSingle<T> ToSingle()
            {
                return new NToSingle<T>();
            }
        }


        /// <summary>
        /// orderbyasc
        /// </summary>
        public struct Orderby<NT>
        {
            /// <summary>
            /// 返回列表
            /// </summary>
            public NToList<T> ToList(int pageSize, int pageNow)
            {
                return new NToList<T>();
            }

            /// <summary>
            /// 返回单条
            /// </summary>
            public NToSingle<T> ToSingle()
            {
                return new NToSingle<T>();
            }
        }

        /// <summary>
        /// orderbydesc
        /// </summary>
        public struct Orderbydesc<NT>
        {
            /// <summary>
            /// 返回列表
            /// </summary>
            public NToList<T> ToList(int pageSize, int pageNow)
            {
                return new NToList<T>();
            }

            /// <summary>
            /// 返回单条
            /// </summary>
            public NToSingle<T> ToSingle()
            {
                return new NToSingle<T>();
            }
        }
    }
}
