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
    /// 查询操作
    /// </summary>
    public sealed class SelectingContext<Parameter, Table> : Linq.SelectingContext<Parameter, Table>
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cacheId"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public SelectingContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(cacheId, dao, tableInfo, sqlParameter)
        {
        }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> StartEntrance()
        {
            return base.StartEntrance();
        }

        /// <summary>
        /// where
        /// </summary>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> Where()
        {
            return base.Where();
        }

        /// <summary>
        /// wehre
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="andOr"></param>
        /// <returns></returns>
        public override SelectContext<Parameter, Table> Where(Expression<Func<Parameter, Table, bool>> expression, string andOr = null)
        {
            return base.Where(expression, andOr);
        }

        /// <summary>
        /// 查询结果
        /// </summary>
        public override Table GetResult()
        {
            return base.GetResult();
        }

        /// <summary>
        /// 查询结果
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public override IEnumerable<Table> GetResults(int startIndex, int endIndex)
        {
            this.LoadOrderBy(true);
            var change = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = "\r",
            };

            this.labels.Add(change);
            this.textLength += change.SqlText.Length;

            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat("limit ", this.dao.SqlExecuter.GetParameterPrefix(), "StartIndex", ", ", this.dao.SqlExecuter.GetParameterPrefix(), "EndIndex"),
            };

            label.Add(new SqlTagParameterPosition()
            {
                ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                Name = "StartIndex",
                OccupanLength = this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex".Length,
                PrefixStartIndex = "limit ".Length,
                ParameterStartIndex = "limit ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length,
                ParameterStopIndex = "limit ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex".Length - 1,
                TextParameter = false,
            });
            label.Add(new SqlTagParameterPosition()
            {
                ActualPrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                SourcePrefix = this.dao.SqlExecuter.GetParameterPrefix(),
                Name = "EndIndex",
                OccupanLength = this.dao.SqlExecuter.GetParameterPrefix().Length + "EndIndex".Length,
                PrefixStartIndex = "limit ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex".Length + ", ".Length,
                ParameterStartIndex = "limit ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex".Length + ", ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length,
                ParameterStopIndex = "limit ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "StartIndex".Length + ", ".Length + this.dao.SqlExecuter.GetParameterPrefix().Length + "EndIndex".Length - 1,
                TextParameter = false,
            });

            this.labels.Add(label);
            this.textLength += label.SqlText.Length;

            return base.GetResults(startIndex, endIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnWhereInit()
        {
            base.OnWhereInit();
        }

        /// <summary>
        /// 对表格或字段格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string FormatTableAndColumn(string text)
        {
            return string.Concat("`", text, "`");
        }
    }
}
