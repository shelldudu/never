using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 更新操作上下文
    /// </summary>
    public abstract class UpdateContext<Parameter> : Context
    {
        /// <summary>
        /// dao
        /// </summary>
        protected readonly IDao dao;

        /// <summary>
        /// tableInfo
        /// </summary>
        protected readonly TableInfo tableInfo;

        /// <summary>
        /// sqlparameter
        /// </summary>
        protected readonly EasySqlParameter<Parameter> sqlParameter;

        /// <summary>
        /// labels
        /// </summary>
        protected readonly List<ILabel> labels;

        /// <summary>
        /// 临时参数
        /// </summary>
        protected readonly Dictionary<string, object> templateParameter;


        /// <summary>
        /// update的jion东西
        /// </summary>
        protected List<UpdateJoin> updateJoin;

        #region 

        /// <summary>
        /// update的jion东西
        /// </summary>
        protected struct UpdateJoin
        {
            /// <summary>
            /// join的类型
            /// </summary>
            public JoinOption JoinOption;

            /// <summary>
            /// join后第二张表的别名
            /// </summary>
            public string TableAsName;

            /// <summary>
            /// join的on
            /// </summary>
            public Expression On;

            /// <summary>
            /// join的and
            /// </summary>
            public Expression And;

            /// <summary>
            /// 参数type
            /// </summary>
            public Type[] Types;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        protected UpdateContext(IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter)
        {
            this.dao = dao; this.tableInfo = tableInfo; this.sqlParameter = sqlParameter;
            this.labels = new List<ILabel>(10);
            this.templateParameter = new Dictionary<string, object>(10);
        }

        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected int Update(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
        {
            return dao.Update(sqlTag, sqlParameter);
        }

        /// <summary>
        /// 执行更新（事务）
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected int Update(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
        {
            dao.BeginTransaction(isolationLevel);
            try
            {
                var row = dao.Update(sqlTag, sqlParameter);
                dao.CommitTransaction();
                return row;
            }
            catch
            {
                dao.RollBackTransaction();
                return -1;
            }
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public abstract int GetResult();

        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public abstract UpdateContext<Parameter> From(string table);

        /// <summary>
        /// join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> Join<Table1>(string tableAsName, Expression<Func<Parameter, Table1, bool>> on, Expression<Func<Table1, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.Join,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1) }
            });

            return this;
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> InnerJoin<Table1>(string tableAsName, Expression<Func<Parameter, Table1, bool>> on, Expression<Func<Table1, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.InnerJoin,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1) }
            });

            return this;
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> LeftJoin<Table1>(string tableAsName, Expression<Func<Parameter, Table1, bool>> on, Expression<Func<Table1, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.LeftJoin,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1) }
            });

            return this;
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> RightJoin<Table1>(string tableAsName, Expression<Func<Parameter, Table1, bool>> on, Expression<Func<Table1, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.RightJoin,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1) }
            });

            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> Join<Table1, Table2>(string tableAsName, Expression<Func<Parameter, Table1, Table2, bool>> on, Expression<Func<Table1, Table2, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.Join,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2) }
            });

            return this;
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> InnerJoin<Table1, Table2>(string tableAsName, Expression<Func<Parameter, Table1, Table2, bool>> on, Expression<Func<Table1, Table2, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.InnerJoin,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2) }
            });

            return this;
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> LeftJoin<Table1, Table2>(string tableAsName, Expression<Func<Parameter, Table1, Table2, bool>> on, Expression<Func<Table1, Table2, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.LeftJoin,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2) }
            });

            return this;
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> RightJoin<Table1, Table2>(string tableAsName, Expression<Func<Parameter, Table1, Table2, bool>> on, Expression<Func<Table1, Table2, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.RightJoin,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2) }
            });

            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> Join<Table1, Table2, Table3>(string tableAsName, Expression<Func<Parameter, Table1, Table2, Table3, bool>> on, Expression<Func<Table1, Table2, Table3, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.Join,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3) }
            });

            return this;
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> InnerJoin<Table1, Table2, Table3>(string tableAsName, Expression<Func<Parameter, Table1, Table2, Table3, bool>> on, Expression<Func<Table1, Table2, Table3, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.InnerJoin,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3) }
            });

            return this;
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> LeftJoin<Table1, Table2, Table3>(string tableAsName, Expression<Func<Parameter, Table1, Table2, Table3, bool>> on, Expression<Func<Table1, Table2, Table3, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.LeftJoin,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3) }
            });

            return this;
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> RightJoin<Table1, Table2, Table3>(string tableAsName, Expression<Func<Parameter, Table1, Table2, Table3, bool>> on, Expression<Func<Table1, Table2, Table3, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.RightJoin,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3) }
            });

            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> Join<Table1, Table2, Table3, Table4>(string tableAsName, Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> on, Expression<Func<Table1, Table2, Table3, Table4, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.Join,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3), typeof(Table4) }
            });

            return this;
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> InnerJoin<Table1, Table2, Table3, Table4>(string tableAsName, Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> on, Expression<Func<Table1, Table2, Table3, Table4, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.InnerJoin,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3), typeof(Table4) }
            });

            return this;
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> LeftJoin<Table1, Table2, Table3, Table4>(string tableAsName, Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> on, Expression<Func<Table1, Table2, Table3, Table4, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.LeftJoin,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3), typeof(Table4) }
            });

            return this;
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="tableAsName"></param>
        /// <param name="on"></param>
        /// <param name="and"></param>
        /// <returns></returns>
        public virtual UpdateContext<Parameter> RightJoin<Table1, Table2, Table3, Table4>(string tableAsName, Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> on, Expression<Func<Table1, Table2, Table3, Table4, bool>> and)
        {
            if (this.updateJoin == null)
                this.updateJoin = new List<UpdateJoin>(1);

            this.updateJoin.Add(new UpdateJoin()
            {
                TableAsName = tableAsName,
                And = and,
                JoinOption = JoinOption.RightJoin,
                On = on,
                Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3), typeof(Table4) }
            });

            return this;
        }

        /// <summary>
        /// as新表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public abstract UpdateContext<Parameter> AsTable(string table);

        /// <summary>
        /// 入口
        /// </summary>
        public abstract UpdateContext<Parameter> Entrance();

        /// <summary>
        /// 在update的时候，set字段使用表明还是别名，你可以返回tableNamePoint或者asTableNamePoint
        /// </summary>
        /// <returns></returns>
        protected abstract string SelectTableNamePointOnSetolunm();

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public abstract UpdateContext<Parameter> SetColumn<TMember>(Expression<Func<Parameter, TMember>> expression);

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public abstract UpdateContext<Parameter> SetColumnWithFunc<TMember>(Expression<Func<Parameter, TMember>> expression, string value);

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public abstract UpdateContext<Parameter> SetColumnWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value);

        /// <summary>
        /// where
        /// </summary>
        public abstract UpdateContext<Parameter> Where();

        /// <summary>
        /// where
        /// </summary>
        public abstract UpdateContext<Parameter> Where(Expression<Func<Parameter, object>> expression);

        /// <summary>
        /// 存在
        /// </summary>
        public abstract UpdateContext<Parameter> Exists<Table1>(AndOrOption option, Expression<Func<Parameter, Table1, bool>> where, Expression<Func<Table1, bool>> and);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract UpdateContext<Parameter> NotExists<Table1>(AndOrOption option, Expression<Func<Parameter, Table1, bool>> where, Expression<Func<Table1, bool>> and);

        /// <summary>
        /// 存在
        /// </summary>
        public abstract UpdateContext<Parameter> Exists<Table1,Table2>(AndOrOption option, Expression<Func<Parameter, Table1, Table2, bool>> where, Expression<Func<Table1, Table2, bool>> and);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract UpdateContext<Parameter> NotExists<Table1, Table2>(AndOrOption option, Expression<Func<Parameter, Table1, Table2, bool>> where, Expression<Func<Table1, Table2, bool>> and);

        /// <summary>
        /// 存在
        /// </summary>
        public abstract UpdateContext<Parameter> Exists<Table1, Table2, Table3>(AndOrOption option, Expression<Func<Parameter, Table1, Table2, Table3, bool>> where, Expression<Func<Table1, Table2, Table3, bool>> and);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract UpdateContext<Parameter> NotExists<Table1, Table2, Table3>(AndOrOption option, Expression<Func<Parameter, Table1, Table2, Table3, bool>> where, Expression<Func<Table1, Table2, Table3, bool>> and);


        /// <summary>
        /// 存在
        /// </summary>
        public abstract UpdateContext<Parameter> Exists<Table1, Table2, Table3, Table4>(AndOrOption option, Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> where, Expression<Func<Table1, Table2, Table3, Table4, bool>> and);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract UpdateContext<Parameter> NotExists<Table1, Table2, Table3, Table4>(AndOrOption option, Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> where, Expression<Func<Table1, Table2, Table3, Table4, bool>> and);

        /// <summary>
        /// 存在
        /// </summary>
        public abstract UpdateContext<Parameter> In<Table1>(AndOrOption option, Expression<Func<Parameter, Table1, bool>> expression, Expression<Func<Table1, bool>> where);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract UpdateContext<Parameter> NotIn<Table1>(AndOrOption option, Expression<Func<Parameter, Table1, bool>> expression, Expression<Func<Table1, bool>> where);

        /// <summary>
        /// 存在
        /// </summary>
        public abstract UpdateContext<Parameter> Exists(AndOrOption option, string expression);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract UpdateContext<Parameter> NotExists(AndOrOption option, string expression);

        /// <summary>
        /// 存在
        /// </summary>
        public abstract UpdateContext<Parameter> In(AndOrOption option, string expression);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract UpdateContext<Parameter> NotIn(AndOrOption option, string expression);
    }
}
