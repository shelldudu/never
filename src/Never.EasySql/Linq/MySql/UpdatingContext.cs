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
    public sealed class UpdatingContext<Parameter> : Linq.UpdatingContext<Parameter>
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
        /// 获取入口的标签
        /// </summary>
        /// <returns></returns>
        protected override TextLabel GetFirstLabelOnEntrance()
        {
            if (this.asTableName.IsNullOrEmpty())
                return new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("update ", this.tableName, "\r", "set") };

            return new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("update ", this.tableName, " as ", asTableName, "\r", "set") };
        }

        /// <summary>
        /// 在update的时候，set字段使用表明还是别名，你可以返回tableNamePoint或者asTableNamePoint
        /// </summary>
        /// <returns></returns>
        protected override string SelectTableNameOnSetolunm()
        {
            return base.asTableNamePoint;
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
    }
}
