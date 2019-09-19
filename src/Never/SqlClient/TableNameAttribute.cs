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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        /// 表名别名
        /// </summary>
        public string Alias { get; set; }
    }
}
