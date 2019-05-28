using Never.Attributes;
using System;

namespace Never.CommandStreams
{
    /// <summary>
    /// 忽略保存命令的行为
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class IgnoreStoreCommandAttribute : IgnoreAnalyseAttribute
    {
    }
}