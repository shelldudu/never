using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Xml
{
    /// <summary>
    /// 参数位置
    /// </summary>
    public class ParameterPosition : IEquatable<ParameterPosition>
    {
        /// <summary>
        /// 参数名字 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 原来前辍的位置
        /// </summary>
        public string SourcePrefix { get; set; }

        /// <summary>
        /// 实际前辍的位置
        /// </summary>
        public string ActualPrefix { get; set; }

        /// <summary>
        /// 是否text参数
        /// </summary>
        public bool TextParameter { get; set; }

        /// <summary>
        /// 前辍的开始位置
        /// </summary>
        public int PrefixStart { get; set; }

        /// <summary>
        /// 参数的开始位置
        /// </summary>
        public int StartPosition { get; set; }
        /// <summary>
        /// 参数的结束位置
        /// </summary>
        public int StopPosition { get; set; }

        /// <summary>
        /// 参数的长度
        /// </summary>
        public int PositionLength { get; set; }

        /// <summary>
        /// 指示当前对象是否等于同一类型的另一个对象。
        /// </summary>
        /// <param name="other">一个与此对象进行比较的对象。</param>
        /// <returns>
        /// 如果当前对象等于 <paramref name="other" /> 参数，则为 <see langword="true" />；否则为 <see langword="false" />。
        /// </returns>
        public bool Equals(ParameterPosition other)
        {
            return this.Name == other.Name;
        }
    }
}