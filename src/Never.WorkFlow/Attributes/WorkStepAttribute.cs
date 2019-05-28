using System;

namespace Never.WorkFlow.Attributes
{
    /// <summary>
    /// 工作单元描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class WorkStepAttribute : Attribute
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Sumarry { get; set; }

        /// <summary>
        /// 工作内容介绍
        /// </summary>
        public string Introduce { get; set; }

        /// <summary>
        /// 唯一编号
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// uniqueId 请使用唯一，否则报错，可以使用随即字符，但也不要过长，避免数据库膨胀
        /// </summary>
        public WorkStepAttribute(string uniqueId)
        {
            this.UniqueId = uniqueId;
        }
    }
}