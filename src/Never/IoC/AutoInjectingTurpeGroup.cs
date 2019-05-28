using System;

namespace Never.IoC
{
    /// <summary>
    /// 自动注入分组
    /// </summary>
    public class AutoInjectingTurpeGroup
    {
        /// <summary>
        /// 注入属性
        /// </summary>
        public BaseAutoInjectingAttribute Attribute { get; set; }

        /// <summary>
        /// 具体对象类型（即放着attribute属性的类型）
        /// </summary>
        public Type ImplementationType { get; set; }
    }
}