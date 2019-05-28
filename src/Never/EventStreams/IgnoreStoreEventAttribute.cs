using Never.Attributes;
using System;

namespace Never.EventStreams
{
    /// <summary>
    /// 忽略保存事件的行为
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class IgnoreStoreEventAttribute : IgnoreAnalyseAttribute
    {
    }
}