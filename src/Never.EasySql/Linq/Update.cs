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
        internal Context Context { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        internal EasySqlParameter<Parameter> SqlParameter { get; set; }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public Update<Parameter> SetColum(Expression<Func<Parameter, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public Update<Parameter> SetColumFunc(Expression<Func<Parameter, object>> expression, string function)
        {
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public int GetResult()
        {
            if (this.Context.dao.CurrentSession != null)
                return this.Context.dao.Update(this.Context.Build(), this.SqlParameter);

            using (this.Context.dao)
            {
                return this.Context.dao.Update(this.Context.Build(), this.SqlParameter);
            }
        }

        /// <summary>
        /// where
        /// </summary>
        public NWhere<Parameter> Where()
        {
            return new NWhere<Parameter>() { update = this };
        }

        /// <summary>
        /// where
        /// </summary>
        public NWhere<Parameter> Where(Expression<Func<Parameter, bool>> expressionn)
        {
            return new NWhere<Parameter>() { update = this };
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
            internal Update<NParameter> update;

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
            public int GetResult()
            {
                return this.update.GetResult();
            }
        }

        /// <summary>
        /// where 条件
        /// </summary>
        /// <typeparam name="NParameter">查询参数</typeparam>
        public struct NWhere<NParameter, TMember>
        {
            /// <summary>
            /// 
            /// </summary>
            internal Update<NParameter> update;

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
            public int GetResult()
            {
                return this.update.GetResult();
            }
        }
    }
}
