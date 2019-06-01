using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Serialization.Json
{
    /// <summary>
    /// 内部处理type
    /// </summary>
    public enum InsideHandleType
    {
        /// <summary>
        /// string
        /// </summary>
        @string = 0,

        /// <summary>
        /// datetime
        /// </summary>
        @datetime = 1,

        /// <summary>
        /// datetimeoffset
        /// </summary>
        @datetimeoffset = 2,

        /// <summary>
        /// guid
        /// </summary>
        @guid =3,

        /// <summary>
        /// timespan
        /// </summary>
        @timespan = 4,

        /// <summary>
        /// decimal
        /// </summary>
        @decimal = 5
    }
}
