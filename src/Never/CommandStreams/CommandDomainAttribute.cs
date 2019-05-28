using System;

namespace Never.CommandStreams
{
    /// <summary>
    /// 命令所处在的运行域
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandDomainAttribute : Attribute
    {
        /// <summary>
        /// 运行域
        /// </summary>
        public string Domain { get; set; }
    }
}