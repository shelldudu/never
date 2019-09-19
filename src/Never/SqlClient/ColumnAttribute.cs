using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.SqlClient
{
    /// <summary>
    /// 字段特性
    /// </summary>
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 字段别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 字段的特性
        /// </summary>
        public ColumnOptional Optional { get; set; }

        /// <summary>
        /// 字段的特性
        /// </summary>
        [Flags]
        public enum ColumnOptional
        {
            /// <summary>
            /// none
            /// </summary>
            None = 0,

            /// <summary>
            /// primary
            /// </summary>
            Primary = 1,

            /// <summary>
            /// +1
            /// </summary>
            AutoIncrement = 2
        }
    }
}
