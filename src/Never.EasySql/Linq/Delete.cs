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
    public struct Delete<Parameter>
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
        /// 获取结果
        /// </summary>
        public int GetResult()
        {
            if (this.Context.dao.CurrentSession != null)
                return this.Context.dao.Delete(this.Context.Build(), this.SqlParameter);

            using (this.Context.dao)
            {
                return this.Context.dao.Delete(this.Context.Build(), this.SqlParameter);
            }
        }

        /// <summary>
        /// where
        /// </summary>
        public NWhere<Parameter> Where()
        {
            return new NWhere<Parameter>() { Context = this.Context, delete = this };
        }

        /// <summary>
        /// where
        /// </summary>
        public NWhere<Parameter> Where<TMember>(Expression<Func<Parameter, object>> expression, string @operator, TMember value)
        {
            return new NWhere<Parameter>() { Context = this.Context, delete = this };
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
            internal Delete<NParameter> delete;

            /// <summary>
            /// 上下文
            /// </summary>
            internal Context Context { get; set; }

            /// <summary>
            /// and
            /// </summary>
            public NWhere<NParameter> And<TMember>(Expression<Func<Parameter, object>> expression, string @operator, TMember value)
            {
                return this;
            }

            /// <summary>
            /// or
            /// </summary>
            public NWhere<NParameter> Or<TMember>(Expression<Func<Parameter, object>> expression, string @operator, TMember value)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter> AndExists<T1>(Expression<Func<Parameter, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="exists">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> AndExists(Func<TableInfo, string> exists)
            {
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter> AndNotExists<T1>(Expression<Func<Parameter, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> AndNotExists(Func<TableInfo, string> notexists)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter> AndIn<T1>(Expression<Func<Parameter, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> AndIn(Func<TableInfo, string> notexists)
            {
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter> AndNotIn<T1>(Expression<Func<Parameter, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> AndNotIn(Func<TableInfo, string> notexists)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter> OrExists<T1>(Expression<Func<Parameter, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="exists">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> OrExists(Func<TableInfo, string> exists)
            {
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter> OrNotExists<T1>(Expression<Func<Parameter, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> OrNotExists(Func<TableInfo, string> notexists)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter> OrIn<T1>(Expression<Func<Parameter, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> OrIn(Func<TableInfo, string> notexists)
            {
                return this;
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NWhere<NParameter> OrNotIn<T1>(Expression<Func<Parameter, T1, object>> expression)
            {
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="notexists">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public NWhere<NParameter> OrNotIn(Func<TableInfo, string> notexists)
            {
                return this;
            }

            /// <summary>
            /// 获取结果
            /// </summary>
            public int GetResult()
            {
                return this.delete.GetResult();
            }
        }
    }
}
