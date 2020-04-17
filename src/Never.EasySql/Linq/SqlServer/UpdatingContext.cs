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
            return new TextLabel() { TagId = NewId.GenerateNumber(), SqlText = string.Concat("update ", this.tableName, "\r", "set") };
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public override UpdateContext<Parameter> Where()
        {
            if (this.updateJoin.Any())
            {
                var label = new TextLabel()
                {
                    SqlText = string.Concat(" from ", this.tableName),
                    TagId = NewId.GenerateNumber(),
                };

                if (this.asTableName.IsNotNullOrEmpty()) 
                {
                
                }

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
            if (this.updateJoin.Any())
            {
                var label = new TextLabel()
                {
                    SqlText = "where 1 = 1 \r",
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
        protected override string SelectTableNameOnSetolunm()
        {
            return base.tableNamePoint;
        }

        /// <summary>
        /// 对字段格式化
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override string Format(string text)
        {
            return string.Concat("[", text, "]");
        }
    }
}
