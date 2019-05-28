using System;

namespace Never.IoC
{
    /// <summary>
    /// 自动注入属性
    /// </summary>
    public abstract class BaseAutoInjectingAttribute : Attribute
    {
        /// <summary>
        /// 注入类型
        /// </summary>
        public virtual Type ServiceType { get; }
    }
}