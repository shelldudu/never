using System;

namespace Never
{
    /// <summary>
    /// 工作触发器
    /// </summary>
    public interface IWorkTigger
    {
        /// <summary>
        /// 触发间隔
        /// </summary>
        TimeSpan Timer { get; }
    }
}