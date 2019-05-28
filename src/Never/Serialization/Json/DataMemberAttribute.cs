using System;

namespace Never.Serialization.Json
{
    /// <summary>
    /// json属性或字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class DataMemberAttribute : Attribute
    {
        /// <summary>
        /// 成员名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMemberAttribute"/> class.
        /// </summary>
        public DataMemberAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMemberAttribute"/> class with the specified name.
        /// </summary>
        /// <param name="propertyName">属性或字段名字</param>
        public DataMemberAttribute(string propertyName)
        {
            Name = propertyName;
        }
    }
}