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
    public sealed class SelectingContext<Table,Parameter> : Linq.SelectingContext<Table,Parameter>
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
        public override SelectContext<Table,Parameter> StartEntrance()
        {
            return base.StartEntrance();
        }

        /// <summary>
        /// where
        /// </summary>
        /// <returns></returns>
        public override SelectContext<Table,Parameter> Where()
        {
            return base.Where();
        }

        /// <summary>
        /// wehre
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override SelectContext<Table,Parameter> Where(Expression<Func<Table,Parameter, bool>> expression)
        {
            return base.Where(expression);
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
            var label = new TextLabel()
            {
                TagId = NewId.GenerateNumber(),
                SqlText = string.Concat("limit ", this.dao.SqlExecuter.GetParameterPrefix(), "PageNow", ", ", this.dao.SqlExecuter.GetParameterPrefix(), "PageSize"),
            };
            this.labels.Add(label);

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
