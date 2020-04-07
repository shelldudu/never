using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Labels
{
    /// <summary>
    /// 标签类型
    /// </summary>
    public enum LabelType
    {
        /// <summary>
        /// The not null
        /// </summary>
        NotNull = 1,

        /// <summary>
        /// The null
        /// </summary>
        Null = 2,

        /// <summary>
        /// The contain
        /// </summary>
        Contain = 3,

        /// <summary>
        /// If
        /// </summary>
        If = 4,

        /// <summary>
        /// The text
        /// </summary>
        Text = 5,

        /// <summary>
        /// The not empty
        /// </summary>
        NotEmpty = 6,

        /// <summary>
        /// The empty
        /// </summary>
        Empty = 7,

        /// <summary>
        /// The array
        /// </summary>
        Array = 10,

        /// <summary>
        /// The return
        /// </summary>
        Return = 11,

        /// <summary>
        /// The not exists
        /// </summary>
        NotExists = 12,
    }
}