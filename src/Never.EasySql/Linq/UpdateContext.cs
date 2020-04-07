﻿using System;
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
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected int Execute(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter)
        {
            return dao.Update(sqlTag, sqlParameter);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="sqlTag"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        protected int Execute(LinqSqlTag sqlTag, IDao dao, EasySqlParameter<Parameter> sqlParameter, System.Data.IsolationLevel isolationLevel)
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
        /// as新表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public abstract void AsTable(string table);

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public abstract UpdateContext<Parameter> SetColum<TMember>(Expression<Func<Parameter, TMember>> expression);

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public abstract UpdateContext<Parameter> SetColumWithFunc<TMember>(Expression<Func<Parameter, TMember>> expression, string value);

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public abstract UpdateContext<Parameter> SetColumWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, object value);

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
        public abstract UpdateContext<Parameter> Exists<T1>(AndOrOption option, Expression<Func<Parameter, T1, bool>> expression);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract UpdateContext<Parameter> NotExists<T1>(AndOrOption option, Expression<Func<Parameter, T1, bool>> expression);

        /// <summary>
        /// 存在
        /// </summary>
        public abstract UpdateContext<Parameter> In<T1>(AndOrOption option, Expression<Func<Parameter, T1, bool>> expression);

        /// <summary>
        /// 不存在
        /// </summary>
        public abstract UpdateContext<Parameter> NotIn<T1>(AndOrOption option, Expression<Func<Parameter, T1, bool>> expression);

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