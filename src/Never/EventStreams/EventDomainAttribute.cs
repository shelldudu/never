using System;

namespace Never.EventStreams
{
    /// <summary>
    /// 事件所处在的运行域
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventDomainAttribute : Attribute
    {
        /// <summary>
        /// 运行域
        /// </summary>
        public string Domain { get; set; }
    }
}