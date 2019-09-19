using Never.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 表的信息
    /// </summary>
    public struct TableInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        public TableNameAttribute TableName { get; set; }

        /// <summary>
        /// 表名，因可以使用As方法将表名重新命名
        /// </summary>
        public string TableNameAlias { get; set; }

        /// <summary>
        /// 字段信息
        /// </summary>
        public IEnumerable<ColumnInfo> Columns { get; set; }

        /// <summary>
        /// 字段信息
        /// </summary>
        public struct ColumnInfo
        {
            /// <summary>
            /// 字段特性
            /// </summary>
            public ColumnAttribute Column { get; set; }

            /// <summary>
            /// typehandler特性
            /// </summary>
            public TypeHandlerAttribute TypeHandler { get; set; }

            /// <summary>
            /// 成员
            /// </summary>
            public MemberInfo Member { get; set; }
        }
    }
}
