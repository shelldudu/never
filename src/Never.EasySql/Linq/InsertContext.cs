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
    public abstract class InsertContext<Parameter, Table> : Context
    {
        /// <summary>
        /// dao
        /// </summary>
        internal readonly IDao dao;

        /// <summary>
        /// tableInfo
        /// </summary>
        internal readonly TableInfo tableInfo;

        /// <summary>
        /// sqlparameter
        /// </summary>
        internal readonly EasySqlParameter<Parameter> sqlParameter;

        /// <summary>
        /// labels
        /// </summary>
        protected readonly List<ILabel> labels;

        /// <summary>
        /// 临时参数
        /// </summary>
        protected readonly Dictionary<string, object> templateParameter;

        /// <summary>
        /// 是否使用批量推入
        /// </summary>
        public bool UseBulk
        {
            get; protected set;
        }

        /// <summary>
        /// 从哪个表
        /// </summary>
        public string InsertTable
        {
            get; protected set;
        }

        /// <summary>
        /// 插入记录次数
        /// </summary>
        protected int insertTimes;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        protected InsertContext(IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(tableInfo)
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
        public virtual InsertContext<Parameter, Table> Into(string table)
        {
            this.InsertTable = this.FormatTableAndColumn(table);
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
        /// <returns></returns>
        public abstract InsertContext<Parameter, Table> StartEntrance();

        /// <summary>
        /// 设为单条
        /// </summary>
        /// <returns></returns>
        public InsertContext<Parameter, Table> SetSingle()
        {
            this.UseBulk = false;
            return this;
        }

        /// <summary>
        /// 设为多条
        /// </summary>
        /// <returns></returns>
        public InsertContext<Parameter, Table> SetBulk()
        {
            this.UseBulk = true;
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public abstract Result GetResult<Result>();

        /// <summary>
        /// 获取结果
        /// </summary>
        public abstract void GetResult();

        /// <summary>
        /// 获取sql语句
        /// </summary>
        /// <returns></returns>
        public abstract SqlTagFormat GetSqlTagFormat(bool formatText = false);

        /// <summary>
        /// 最后自增字符串
        /// </summary>
        /// <returns></returns>
        public abstract InsertContext<Parameter, Table> InsertLastInsertId<ReturnType>();

        /// <summary>
        /// 插入字段名
        /// </summary>
        public abstract InsertContext<Parameter, Table> InsertColumn(string columnName, string parameterName, bool textParameter, bool function);

        /// <summary>
        /// 插入所有字段
        /// </summary>
        /// <returns></returns>
        public virtual InsertContext<Parameter, Table> InsertAll()
        {
            if (this.insertTimes > 0)
                return this;

            foreach (var i in this.tableInfo.Columns)
            {
                if (i.Column == null)
                {
                    var name = i.Member.Name;
                    this.InsertColumn(name, name, false, false);
                }
                else
                {
                    if ((i.Column.Optional & SqlClient.ColumnAttribute.ColumnOptional.AutoIncrement) == SqlClient.ColumnAttribute.ColumnOptional.AutoIncrement)
                        continue;

                    var name = i.Column.Name.IsNullOrEmpty() ? i.Member.Name : i.Column.Name;
                    this.InsertColumn(name, name, false, false);
                }
            }

            return this;
        }

        /// <summary>
        /// 插入的字段
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public virtual InsertContext<Parameter, Table> Colum(Expression<Func<Table, object>> keyValue)
        {
            string columnName = this.FindColumnName(keyValue, this.tableInfo, out var memberInfo, out var columnInfo);
            if (columnInfo.Column != null && ((columnInfo.Column.Optional & SqlClient.ColumnAttribute.ColumnOptional.AutoIncrement) == SqlClient.ColumnAttribute.ColumnOptional.AutoIncrement))
                return this;

            return this.InsertColumn(columnName, memberInfo.Name, false, false);
        }

        /// <summary>
        /// 插入的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual InsertContext<Parameter, Table> Colum(Expression<Func<Table, object>> key, Expression<Func<Parameter, object>> value)
        {
            string columnName = this.FindColumnName(key, this.tableInfo, out _);
            string parameterName = this.FindColumnName(value, FindTableInfo(typeof(Parameter)), out _);
            return this.InsertColumn(columnName, parameterName, false, false);
        }

        /// <summary>
        /// 插入的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual InsertContext<Parameter, Table> ColumWithFunc(Expression<Func<Table, object>> key, string value)
        {
            string columnName = this.FindColumnName(key, this.tableInfo, out _);
            this.templateParameter[columnName] = value;
            return this.InsertColumn(columnName, columnName, true, true);
        }

        /// <summary>
        /// 插入的字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual InsertContext<Parameter, Table> ColumWithValue<TMember>(Expression<Func<Table, TMember>> key, TMember value)
        {
            string columnName = this.FindColumnName(key, this.tableInfo, out _);
            this.templateParameter[columnName] = value;
            return this.InsertColumn(columnName, columnName, true, false);
        }
    }
}
