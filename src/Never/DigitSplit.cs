using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never
{
    /// <summary>
    /// 数字分割
    /// </summary>
    public struct DigitSplit<T> where T : IConvertible
    {
        /// <summary>
        /// 左边的值
        /// </summary>
        public T Left { get; set; }

        /// <summary>
        /// 左边的长度
        /// </summary>
        public int LeftLength { get; set; }

        /// <summary>
        /// 右边的值
        /// </summary>
        public T Right { get; set; }

        /// <summary>
        /// 右边的长度
        /// </summary>
        public int RightLength { get; set; }

        /// <summary>
        /// 分隔符
        /// </summary>
        public char Split => '.';

        /// <summary>
        /// 分割符的长度，为0表示为整数，为1表示带小数点
        /// </summary>
        public int SplitLength { get; set; }

        /// <summary>
        /// 总的长度，含小数点的长度
        /// </summary>
        public int TotalLength { get; set; }
    }
}
