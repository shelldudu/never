using System;
using System.Collections.Generic;

namespace Never.WorkFlow.Exceptions
{
    /// <summary>
    /// 模板内容没有找到
    /// </summary>
    [Serializable]
    public class TemplateNotFoundException : KeyNotFoundException
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="messageFormat"></param>
        /// <param name="message"></param>
        public TemplateNotFoundException(string messageFormat, string[] message)
            : base(string.Format(messageFormat, message))
        {
        }
    }
}