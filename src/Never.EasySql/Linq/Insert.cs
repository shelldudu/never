using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 插入语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    public struct Insert<Parameter>
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
        /// 更新的字段名
        /// </summary>
        public Insert<Parameter> ValueColum(Expression<Func<Parameter, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public Insert<Parameter> ValueColumFunc(Expression<Func<Parameter, object>> expression, string function)
        {
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public Result GetResult<Result>()
        {
            return default(Result);
        }

        /// <summary>
        /// where
        /// </summary>
        public NWhere<Parameter> Where()
        {
            return new NWhere<Parameter>();
        }

        /// <summary>
        /// 返回最后插入语句
        /// </summary>
        public NLastInsertId<Parameter> LastInsertId()
        {
            return new NLastInsertId<Parameter>();
        }

        /// <summary>
        /// where 条件
        /// </summary>
        /// <typeparam name="NParameter">查询参数</typeparam>
        public struct NWhere<NParameter>
        {
            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter> Exists<T1>(Expression<Func<Parameter, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="exists">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> Exists(Func<TableInfo, string> exists)
            {
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter> NotExists<T1>(Expression<Func<Parameter, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> NotExists(Func<TableInfo, string> notexists)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter> In<T1>(Expression<Func<Parameter, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> In(Func<TableInfo, string> notexists)
            {
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter> NotIn<T1>(Expression<Func<Parameter, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> NotIn(Func<TableInfo, string> notexists)
            {
                return this;
            }

            /// <summary>
            /// 获取结果
            /// </summary>
            public Result GetResult<Result>()
            {
                return default(Result);
            }

        }

        /// <summary>
        /// 返回最后插入语句
        /// </summary>
        /// <typeparam name="NParameter"></typeparam>
        public struct NLastInsertId<NParameter>
        {
            /// where
            /// </summary>
            public NWhere<NParameter> Where()
            {
                return new NWhere<NParameter>();
            }

            /// <summary>
            /// 获取结果
            /// </summary>
            public Result GetResult<Result>()
            {
                return default(Result);
            }
        }
    }
}
