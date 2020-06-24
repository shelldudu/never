using Never.EasySql.Labels;
using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.MySql
{
    /// <summary>
    /// 更新操作
    /// </summary>
    public sealed class UpdatingContext<Table,Parameter> : Linq.UpdatingContext<Table,Parameter>
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cacheId"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public UpdatingContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(cacheId, dao, tableInfo, sqlParameter)
        {
        }

        /// <summary>
        /// 入口
        /// </summary>
        public override UpdateContext<Table,Parameter> StartSetColumn()
        {
            this.formatColumnAppendCount = this.FormatColumn("a").Length - 1;
            this.tableNamePoint = string.Concat(this.FromTable, ".");
            this.asTableNamePoint = this.AsTable.IsNullOrEmpty() ? string.Empty : string.Concat(this.AsTable, ".");

            var label = this.AsTable.IsNullOrEmpty() ?
                new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("update ", this.FromTable, "\r") }
                : new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("update ", this.FromTable, " as ", AsTable, "\r") };

            this.textLength += label.SqlText.Length;
            this.labels.Add(label);
            this.equalAndPrefix = string.Concat(" = ", this.dao.SqlExecuter.GetParameterPrefix());
            return this;
        }

        /// <summary>
        /// 在update的时候，set字段使用表明还是别名，你可以返回tableNamePoint或者asTableNamePoint
        /// </summary>
        /// <returns></returns>
        protected override string SelectTableNamePointOnSetColunm()
        {
            return base.asTableNamePoint;
        }

        /// <summary>
        /// 对表名格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string FormatTable(string text)
        {
            return string.Concat("`", text, "`");
        }

        /// <summary>
        /// 对字段格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string FormatColumn(string text)
        {
            return string.Concat("`", text, "`");
        }
    }
}
