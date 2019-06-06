using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Memcached
{
    /// <summary>
    /// 目标类型
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public enum TargetTypeFlag : byte
    {
        /// <summary>
        /// char
        /// </summary>
        [TargetTypeFlag(Type = typeof(char))]
        @char = 0,

        /// <summary>
        /// bool
        /// </summary>
        [TargetTypeFlag(Type = typeof(bool))]
        @bool = 1,

        /// <summary>
        /// byte
        /// </summary>
        [TargetTypeFlag(Type = typeof(byte))]
        @byte = 2,

        /// <summary>
        /// short
        /// </summary>
        [TargetTypeFlag(Type = typeof(short))]
        @short = 3,

        /// <summary>
        /// ushort
        /// </summary>
        [TargetTypeFlag(Type = typeof(ushort))]
        @ushort = 4,

        /// <summary>
        /// int
        /// </summary>
        [TargetTypeFlag(Type = typeof(int))]
        @int = 5,

        /// <summary>
        /// uint
        /// </summary>
        [TargetTypeFlag(Type = typeof(uint))]
        @uint = 6,

        /// <summary>
        /// long
        /// </summary>
        [TargetTypeFlag(Type = typeof(long))]
        @long = 7,

        /// <summary>
        /// ulong
        /// </summary>
        [TargetTypeFlag(Type = typeof(ulong))]
        @ulong = 8,

        /// <summary>
        /// float
        /// </summary>
        [TargetTypeFlag(Type = typeof(float))]
        @float = 9,

        /// <summary>
        /// double
        /// </summary>
        [TargetTypeFlag(Type = typeof(double))]
        @double = 10,

        /// <summary>
        /// decimal
        /// </summary>
        [TargetTypeFlag(Type = typeof(decimal))]
        @decimal = 11,

        /// <summary>
        /// timespan
        /// </summary>
        [TargetTypeFlag(Type = typeof(TimeSpan))]
        @timespan = 12,

        /// <summary>
        /// datetime
        /// </summary>
        [TargetTypeFlag(Type = typeof(DateTime))]
        @datetime = 13
    }
}
