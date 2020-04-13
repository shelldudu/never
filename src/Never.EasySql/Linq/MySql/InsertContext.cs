using Never.EasySql.Labels;
using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.MySql
{
    /// <summary>
    /// 插入语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    public sealed class InsertContext<Parameter> : Linq.InsertContext<Parameter>
    {
        private readonly string cacheId;
        private int textLength;
        private int setTimes;
        private string tableName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheId"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public InsertContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(dao, tableInfo, sqlParameter)
        {
            this.cacheId = cacheId;
        }

        /// <summary>
        /// 对字段格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string Format(string text)
        {
            return string.Concat("`", text, "`");
        }

        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="table"></param>
        public override void Into(string table)
        {
            this.tableName = table;
        }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        public override Linq.InsertContext<Parameter> Entrance()
        {
            this.tableName = this.tableName.IsNullOrEmpty() ? this.FindTableName<Parameter>(tableInfo) : this.tableName;
            var label = new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("insert into ", this.Format(this.tableName)) };
            this.textLength += label.SqlText.Length;
            this.labels.Add(label);

            return this;
        }
    }
}
