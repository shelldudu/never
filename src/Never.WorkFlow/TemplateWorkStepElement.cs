using Never.WorkFlow.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Never.WorkFlow
{
    /// <summary>
    /// 步骤执行的元素
    /// </summary>
    public class TemplateWorkStepElement
    {
        #region prop

        /// <summary>
        /// 节点顺序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 工作步骤，使用Attribute的UniqueId
        /// </summary>
        public string[] Steps { get; set; }

        /// <summary>
        /// 协同方式
        /// </summary>
        public CoordinationMode Mode { get; set; }

        #endregion prop

        #region type

        /// <summary>
        /// 步骤类型
        /// </summary>
        /// <returns></returns>
        public string ToStepType()
        {
            var sb = new StringBuilder();
            foreach (var step in this.Steps)
            {
                sb.Append(step);
                sb.Append("|");
            }

            return sb.ToString().TrimEnd('|');
        }

        #endregion type
    }
}