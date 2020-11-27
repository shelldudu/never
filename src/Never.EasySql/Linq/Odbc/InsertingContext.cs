using Never.EasySql.Labels;
using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.Odbc
{
    /// <summary>
    /// 插入操作
    /// </summary>
    public sealed class InsertingContext<Parameter, Table> : Linq.InsertingContext<Parameter, Table>
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cacheId"></param>
        /// <param name="dao"></param>
        /// <param name="tableInfo"></param>
        /// <param name="sqlParameter"></param>
        public InsertingContext(string cacheId, IDao dao, TableInfo tableInfo, EasySqlParameter<Parameter> sqlParameter) : base(cacheId, dao, tableInfo, sqlParameter)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override InsertContext<Parameter, Table> InsertLastInsertId<ReturnType>()
        {
            this.LoadSqlOnGetResulting();
            this.labels.Add(new ReturnLabel()
            {
                TagId = NewId.GenerateNumber(),
                Line = new TextLabel()
                {
                    TagId = NewId.GenerateNumber(),
                    SqlText = this.UseBulk ? ";select @@Identity;" : "select @@Identity;",
                },
                Type = typeof(ReturnType).Name.ToLower(),
            });

            return this;
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
