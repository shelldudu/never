using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never
{
    /// <summary>
    /// 范围对象
    /// </summary>
    /// <typeparam name="TMin">Min类型</typeparam>
    /// <typeparam name="TMax">Max类型</typeparam>
    public class RangeTuple<TMin, TMax>
    {
        /// <summary>
        /// min
        /// </summary>
        public TMin Min { get; private set; }

        /// <summary>
        /// max
        /// </summary>
        public TMax Max { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeTuple{TMin, TMax}"/> class.
        /// </summary>
        protected RangeTuple()
            : this(default(TMin), default(TMax))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeTuple{TMin, TMax}"/> class.
        /// </summary>
        /// <param name="min">The min.</param>
        /// <param name="max">The value.</param>
        public RangeTuple(TMin min, TMax max)
        {
            this.Min = min;
            this.Max = max;
        }
    }
}