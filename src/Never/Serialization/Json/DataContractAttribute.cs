using System;

namespace Never.Serialization.Json
{
    /// <summary>
    /// 表示是一个json对象
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class DataContractAttribute : Attribute
    {
    }
}