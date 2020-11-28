using Never.EasySql.Labels;
using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.SqlServer
{
    /// <summary>
    /// 更新操作
    /// </summary>
    public sealed class UpdatingContext<Parameter, Table> : Linq.UpdatingContext<Parameter, Table>
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
        public override UpdateContext<Parameter, Table> StartEntrance()
        {
            this.formatColumnAppendCount = this.FormatTableAndColumn("a").Length - 1;
            this.tableNamePoint = string.Concat(this.FromTable, ".");
            this.asTableNamePoint = this.AsTable.IsNullOrEmpty() ? string.Empty : string.Concat(this.AsTable, ".");

            var label = new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("update ", this.FromTable, "\r") };
            this.textLength += label.SqlText.Length;
            this.labels.Add(label);
            this.equalAndPrefix = string.Concat(" = ", this.dao.SqlExecuter.GetParameterPrefix());
            return this;
        }


        /// <summary>
        /// where 条件
        /// </summary>
        public override UpdateContext<Parameter, Table> Where()
        {
            if (this.updateJoin.IsNotNullOrEmpty())
            {
                var label = this.LoadJoinLabel(this.FromTable, this.AsTable, updateJoin, this.dao);

                this.labels.Add(label);
                this.textLength += label.SqlText.Length;
            }

            return base.Where();
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public override UpdateContext<Parameter, Table> Where(Expression<Func<Parameter, Table, bool>> expression, string andOr = null)
        {
            if (this.AsTable.IsNotNullOrEmpty())
            {
                var label = new TextLabel()
                {
                    SqlText = string.Concat("from ", this.FromTable, " as ", this.AsTable, this.updateJoin.IsNotNullOrEmpty() ? " " : "\r"),
                    TagId = NewId.GenerateNumber(),
                };

                this.labels.Add(label);
                this.textLength += label.SqlText.Length;
            }

            if (this.updateJoin.IsNotNullOrEmpty())
            {
                var label = this.LoadJoinLabel(this.FromTable, this.AsTable, updateJoin, this.dao);

                this.labels.Add(label);
                this.textLength += label.SqlText.Length;
            }

            return base.Where(expression);
        }

        /// <summary>
        /// set的时候选择什么表名（用别名还是真表名）
        /// </summary>
        /// <returns></returns>
        protected override string SelectTableNamePointOnSetColunm()
        {
            return this.tableNamePoint;
        }

        /// <summary>
        /// 对表格或字段格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string FormatTableAndColumn(string text)
        {
            return string.Concat("[", text, "]");
        }
    }
}
