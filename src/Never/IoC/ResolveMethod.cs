using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.IoC
{
    /// <summary>
    /// resove method
    /// </summary>
    public enum ResolveMethod : byte
    {
        /// <summary>
        /// default
        /// </summary>
        Resolve = 0,

        /// <summary>
        /// try resolve
        /// </summary>
        ResolveTriable = 1,

        /// <summary>
        /// optional
        /// </summary>
        ResolveOptional = 2,
    }
}
