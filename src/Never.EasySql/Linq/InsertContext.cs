using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 插入上下文
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    public abstract class InsertContext<Table, Parameter> : Context
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
        /// 从哪个表
        /// </summary>
        public string InsertTable
        {
            get; protected set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        protected InsertContext(IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter)
        {
            this.dao = dao; this.tableInfo = tableInfo; this.sqlParameter = sqlParameter;
            this.labels = new List<ILabel>(10);
            this.templateParameter = new Dictionary<string, object>(10);
        }

        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public virtual InsertContext<Table, Parameter> Into(string table)
        {
            this.InsertTable = this.FormatTable(table);
            return this;
        }


        /// <summary>
        /// 检查名称是否合格
        /// </summary>
        /// <param name="tableName"></param>
        public virtual void CheckTableNameIsExists(string tableName)
        {
            if (this.InsertTable.IsEquals(tableName))
                throw new Exception(string.Format("the table name {0} is equal alias Name {1}", this.InsertTable, tableName));
        }

        /// <summary>
        /// 入口
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public abstract InsertContext<Table, Parameter> StartInsertColumn(char flag);

        /// <summary>
        /// 获取结果
        /// </summary>
        public abstract Result GetResult<Result>();

        /// <summary>
        /// 获取结果
        /// </summary>
        public abstract void GetResult();

        /// <summary>
        /// 最后自增字符串
        /// </summary>
        /// <returns></returns>
        public abstract InsertContext<Table, Parameter> InsertLastInsertId();

        /// <summary>
        /// 插入字段名
        /// </summary>
        public abstract InsertContext<Table, Parameter> InsertColumn(string columnName, string parameterName, bool textParameter);

        /// <summary>
        /// 插入所有字段
        /// </summary>
        /// <returns></returns>
        public virtual InsertContext<Table, Parameter> InsertAll()
        {
            foreach (var i in this.tableInfo.Columns)
            {
                if (i.Column.Optional == SqlClient.ColumnAttribute.ColumnOptional.AutoIncrement)
                    continue;

                var name = i.Column.Alias.IsNullOrEmpty() ? i.Member.Name : i.Column.Alias;
                this.InsertColumn(name, name, false);
            }

            return this;
        }

        /// <summary>
        /// 插入的字段
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public virtual InsertContext<Table, Parameter> Colum(Expression<Func<Table, object>> keyValue)
        {
            string columnName = this.FindColumnName(keyValue, this.tableInfo, out _);
            return this.InsertColumn(columnName, columnName, false);
        }

        /// <summary>
        /// 插入的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual InsertContext<Table, Parameter> Colum(Expression<Func<Table, object>> key, Expression<Func<Parameter, object>> value)
        {
            string columnName = this.FindColumnName(key, this.tableInfo, out _);
            string parameterName = this.FindColumnName(value, this.tableInfo, out _);
            return this.InsertColumn(columnName, parameterName, false);
        }

        /// <summary>
        /// 插入的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual InsertContext<Table, Parameter> ColumWithFunc(Expression<Func<Table, object>> key, string value)
        {
            string columnName = this.FindColumnName(key, this.tableInfo, out _);
            this.templateParameter[columnName] = value;
            return this.InsertColumn(columnName, columnName, true);
        }

        /// <summary>
        /// 插入的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual InsertContext<Table, Parameter> ColumWithValue<TMember>(Expression<Func<Table, TMember>> key, TMember value)
        {
            string columnName = this.FindColumnName(key, this.tableInfo, out _);
            this.templateParameter[columnName] = value;
            return this.InsertColumn(columnName, columnName, false);
        }
    }
}
