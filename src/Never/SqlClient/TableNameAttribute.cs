using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.SqlClient
{
    /// <summary>
    /// 表名特性
    /// </summary>
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        /// 表名别名
        /// </summary>
        public string Alias { get; set; }
    }
}
