using System;

namespace Never.Aop.DomainFilters
{
    /// <summary>
    /// 事件处理优先级，里面的Order[排序，请设置为大于0，如果小于0，则当为1处理]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class EventHandlerPriorityAttribute : Attribute
    {
        /// <summary>
        /// 排序，请设置为大于0，如果小于0，则当为1处理
        /// </summary>
        public int Order { get; set; }
    }
}