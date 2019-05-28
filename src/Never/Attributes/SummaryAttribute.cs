using System;

namespace Never.Attributes
{
    /// <summary>
    /// 摘要说明特性
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class SummaryAttribute : Attribute
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Descn { get; set; }
    }
}