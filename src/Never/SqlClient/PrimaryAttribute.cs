using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.SqlClient
{
    /// <summary>
    /// 主键特性
    /// </summary>
    public class PrimaryAttribute : Attribute
    {
        /// <summary>
        /// 字段别名
        /// </summary>
        public string AliasName { get; set; }
    }
}
