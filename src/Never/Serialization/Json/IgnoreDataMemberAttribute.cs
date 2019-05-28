using System;

namespace Never.Serialization.Json
{
    /// <summary>
    /// 表示不进行序列化的
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class IgnoreDataMemberAttribute : Attribute
    {
    }
}