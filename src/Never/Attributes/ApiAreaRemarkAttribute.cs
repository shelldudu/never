using System;

namespace Never.Attributes
{
    /// <summary>
    /// API区域资源标识
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ApiAreaRemarkAttribute : Attribute
    {
        /// <summary>
        /// 区域名字
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="area"></param>
        public ApiAreaRemarkAttribute(string area)
        {
            this.Area = area;
        }
    }
}