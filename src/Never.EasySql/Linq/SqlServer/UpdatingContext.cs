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
    public sealed class UpdatingContext<Parameter> : Linq.UpdatingContext<Parameter>
    {
        /// <summary>
        /// update join
        /// </summary>
        private List<JoinInfo> updateJoin;

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
        public override UpdateContext<Parameter> StartSetColumn()
        {
            this.formatColumnAppendCount = this.FormatColumn("a").Length - 1;
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
        public override UpdateContext<Parameter> Where()
        {
            if (this.updateJoin.IsNotNullOrEmpty())
            {
                var label = new TextLabel()
                {
                    SqlText = this.LoadUpdateJoin(this.FromTable, this.AsTable, updateJoin).ToString(),
                    TagId = NewId.GenerateNumber(),
                };

                this.labels.Add(label);
                this.textLength += label.SqlText.Length;
            }

            return base.Where();
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public override UpdateContext<Parameter> Where(Expression<Func<Parameter, object>> expression)
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
                var label = new TextLabel()
                {
                    SqlText = this.LoadUpdateJoin(this.FromTable, this.AsTable, updateJoin).ToString(),
                    TagId = NewId.GenerateNumber(),
                };

                this.labels.Add(label);
                this.textLength += label.SqlText.Length;
            }

            return base.Where(expression);
        }

        /// <summary>
        /// 在update的时候，set字段使用表明还是别名，你可以返回tableNamePoint或者asTableNamePoint
        /// </summary>
        /// <returns></returns>
        protected override string SelectTableNamePointOnSetolunm()
        {
            return base.asTableNamePoint;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joins"></param>
        /// <returns></returns>
        public override UpdateContext<Parameter> JoinOnUpdate(List<JoinInfo> joins)
        {
            this.updateJoin = joins;
            return this;
        }

        /// <summary>
        /// 对表名格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string FormatTable(string text)
        {
            return string.Concat("[", text, "]");
        }

        /// <summary>
        /// 对字段格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string FormatColumn(string text)
        {
            return string.Concat("[", text, "]");
        }
    }
}
