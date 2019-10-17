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
    public abstract class Select<Parameter, T>
    {
        #region field and ctor

        /// <summary>
        /// 从缓存到得的信息
        /// </summary>
        private readonly SqlTag sqlTagFromCached = null;

        /// <summary>
        /// dao
        /// </summary>
        protected readonly IDao dao;

        /// <summary>
        /// 缓存cached
        /// </summary>
        protected readonly string cacheId;

        /// <summary>
        /// label的集合
        /// </summary>
        protected readonly ICollection<BuildingLableInfo> labels;

        /// <summary>
        /// 参数
        /// </summary>
        protected readonly EasySqlParameter<Parameter> sqlParameter;

        /// <summary>
        /// 表的信息
        /// </summary>
        protected readonly TableInfo tableInfo;

        /// <summary>
        /// as新表名
        /// </summary>
        protected string @as;

        /// <summary>
        /// 表名
        /// </summary>
        protected string tableName;

        /// <summary>
        /// 列名
        /// </summary>
        protected List<string> columns;

        /// <summary>
        /// 排序
        /// </summary>
        protected List<string> orderby;

        /// <summary>
        /// 分页
        /// </summary>
        protected PagedSearch paged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sqlParameter"></param>
        /// <param name="cacheId"></param>
        protected Select(IDao dao, EasySqlParameter<Parameter> sqlParameter, string cacheId)
        {
            this.dao = dao; this.sqlParameter = sqlParameter; this.cacheId = cacheId;
            if (this.dao.SqlTagProvider.TryGet(this.cacheId, out sqlTagFromCached) == false)
            {
                this.labels = new List<BuildingLableInfo>();
                this.columns = new List<string>();
                this.orderby = new List<string>();
            }
        }

        #endregion

        #region build

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        protected virtual SqlTag Build()
        {
            return this.sqlTagFromCached;
        }

        /// <summary>
        /// 检查table的信息
        /// </summary>
        protected void CheckTableInfo()
        {
            if (this.tableInfo.TableName != null)
                return;
        }

        #endregion

        #region linq

        /// <summary>
        /// 字段名
        /// </summary>
        public Select<Parameter, T> SelectColum(Expression<Func<Parameter, T, object>> expression)
        {
            var model = expression.Body as ParameterExpression;
            if (model != null)
            {
                this.columns.Add(model.Name);
                return this;
            }

            var member = expression.Body as MemberExpression;
            if (member != null)
            {
                this.columns.Add(member.Member.Name);
                return this;
            }

            var unary = expression.Body as UnaryExpression;
            if (unary != null)
            {
                member = unary.Operand as MemberExpression;
                if (member != null)
                {
                    this.columns.Add(member.Member.Name);
                    return this;
                }
            }

            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public Select<Parameter, T> SelectColum(Expression<Func<Parameter, T, object>> expression, string asMemberName)
        {
            var model = expression.Body as ParameterExpression;
            if (model != null)
            {
                this.columns.Add(string.Concat(model.Name, " as ", asMemberName));
                return this;
            }

            var member = expression.Body as MemberExpression;
            if (member != null)
            {
                this.columns.Add(member.Member.Name);
                return this;
            }

            var unary = expression.Body as UnaryExpression;
            if (unary != null)
            {
                member = unary.Operand as MemberExpression;
                if (member != null)
                {
                    this.columns.Add(member.Member.Name);
                    return this;
                }
            }

            return this;
        }

        /// <summary>
        /// 新的表名，可以不用<typeparamref name="T"/>里的表名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Select<Parameter, T> From(string tableName)
        {
            this.tableName = tableName;
            return this;
        }

        /// <summary>
        /// as新的表名 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Select<Parameter, T> As(string tableName)
        {
            this.@as = tableName;
            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <param name="on"></param>
        public virtual Select<Parameter, T> Join<T2>(Expression<Func<Parameter, T, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="on"></param>
        public virtual Select<Parameter, T> LeftJoin<T2>(Expression<Func<Parameter, T, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="on"></param>
        public virtual Select<Parameter, T> RightJoin<T2>(Expression<Func<Parameter, T, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="on"></param>
        public virtual Select<Parameter, T> InnerJoin<T2>(Expression<Func<Parameter, T, T2, object>> on)
        {
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public virtual NWhere<Parameter, T> Where(Expression<Func<Parameter, T, object>> expression)
        {
            return new NWhere<Parameter, T>()
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
        public NToList<Parameter, T> ToList(int pageNow, int pageSize)
        {
            this.paged = new PagedSearch(pageNow, pageSize);
            return new NToList<Parameter, T>()
            {
                select = this,
            };
        }

        /// <summary>
        /// 返回单条
        /// </summary>
        /// <returns></returns>
        public NToSingle<Parameter, T> ToSingle()
        {
            return new NToSingle<Parameter, T>()
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
        /// <typeparam name="NT">查询结果对象</typeparam>
        public struct NToList<NParameter, NT>
        {
            /// <summary>
            /// select
            /// </summary>
            internal Select<NParameter, NT> select;

            /// <summary>
            /// 返回执行结果
            /// </summary>
            public IEnumerable<NT> GetResult()
            {
                return this.select.dao.QueryForEnumerable<NT, NParameter>(this.select.Build(), select.sqlParameter);
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
            internal Select<NParameter, NT> select;

            /// <summary>
            /// 返回执行结果
            /// </summary>
            public NT GetResult()
            {
                return this.select.dao.QueryForObject<NT, NParameter>(this.select.Build(), select.sqlParameter);
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
            internal Select<NParameter, NT> select;

            /// <summary>
            /// 返回数组
            /// </summary>
            /// <param name="pageSize"></param>
            /// <param name="pageNow"></param>
            /// <returns></returns>
            public NToList<NParameter, NT> ToList(int pageNow, int pageSize)
            {
                this.select.paged = new PagedSearch(pageNow, pageSize);
                return new NToList<NParameter, NT>()
                {
                    select = this.select,
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
            public NWhere<NParameter, NT> AndExists<T1>(Expression<Func<Parameter, T, T1, object>> expression)
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
            public NWhere<NParameter, NT> AndNotExists<T1>(Expression<Func<Parameter, T, T1, object>> expression)
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
            public NWhere<NParameter, NT> AndIn<T1>(Expression<Func<Parameter, T, T1, object>> expression)
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
            public NWhere<NParameter, NT> AndNotIn<T1>(Expression<Func<Parameter, T, T1, object>> expression)
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
            public NWhere<NParameter, NT> OrExists<T1>(Expression<Func<Parameter, T, T1, object>> expression)
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
            public NWhere<NParameter, NT> OrNotExists<T1>(Expression<Func<Parameter, T, T1, object>> expression)
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
            public NWhere<NParameter, NT> OrIn<T1>(Expression<Func<Parameter, T, T1, object>> expression)
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
            public NWhere<NParameter, NT> OrNotIn<T1>(Expression<Func<Parameter, T, T1, object>> expression)
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
                    this.select.orderby.Add(string.Concat(model.Name, " asc"));
                    return this;
                }

                var member = expression.Body as MemberExpression;
                if (member != null)
                {
                    this.select.orderby.Add(string.Concat(member.Member.Name, " asc"));
                    return this;
                }

                var unary = expression.Body as UnaryExpression;
                if (unary != null)
                {
                    member = unary.Operand as MemberExpression;
                    if (member != null)
                    {
                        this.select.orderby.Add(string.Concat(member.Member.Name, " asc"));
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
                    this.select.orderby.Add(string.Concat(model.Name, " desc"));
                    return this;
                }

                var member = expression.Body as MemberExpression;
                if (member != null)
                {
                    this.select.orderby.Add(string.Concat(member.Member.Name, " desc"));
                    return this;
                }

                var unary = expression.Body as UnaryExpression;
                if (unary != null)
                {
                    member = unary.Operand as MemberExpression;
                    if (member != null)
                    {
                        this.select.orderby.Add(string.Concat(member.Member.Name, " desc"));
                        return this;
                    }
                }

                return this;
            }
        }

        #endregion
    }
}
