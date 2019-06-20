using System;

namespace Never.IoC
{
    /// <summary>
    /// 自动注入分组
    /// </summary>
    public class AutoInjectingGroupInfo
    {
        /// <summary>
        /// 注入属性
        /// </summary>
        public AutoInjectingAttribute Attribute { get; set; }

        /// <summary>
        /// 具体对象类型（即放着attribute属性的类型）
        /// </summary>
        public Type ImplementationType { get; set; }

        /// <summary>
        /// 注入Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 注入生命周期
        /// </summary>
        public ComponentLifeStyle LifeStyle { get; set; }
    }
}