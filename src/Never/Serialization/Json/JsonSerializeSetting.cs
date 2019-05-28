using System;
using System.Collections.Generic;

namespace Never.Serialization.Json
{
    /// <summary>
    /// setting
    /// </summary>
    public class JsonSerializeSetting : IEqualityComparer<JsonSerializeSetting>, IEquatable<JsonSerializeSetting>
    {
        #region prop

        /// <summary>
        /// 配置名字
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 时间格式化
        /// </summary>
        public DateTimeFormat DateTimeFormat { get; set; }

        /// <summary>
        /// 是否使用false或true去写入流，如果为false,则用1和0替代
        /// </summary>
        public bool WriteNumberOnBoolenType { get; set; }

        /// <summary>
        /// 是否保持guid中横线
        /// </summary>
        public bool WriteHorizontalLineOnGuidType { get; set; }

        /// <summary>
        /// 是否保持Type版本号的信息
        /// </summary>
        public bool WriteVersionOnTypeInfo { get; set; }

        /// <summary>
        /// 使用Uniciode去写入数据
        /// </summary>
        public bool WriteWithUnicodeOnStringType { get; set; }

        /// <summary>
        /// 最大深度，默认为1024
        /// </summary>
        public int MaxDepth { get; set; }

        /// <summary>
        /// 是否使用数值来输出enum的值,开启后枚举的值全部是数值，性能会降级
        /// </summary>
        public bool WriteNumberOnEnumType { get; set; }

        /// <summary>
        /// 是否支持字典的Key能以复合对象存在，很多工具是不支持的
        /// </summary>
        public bool SupportComplexTypeKeyInDictionary { get; set; }

        /// <summary>
        /// 写'null'在对象为空的情况下
        /// </summary>
        public bool WriteNullWhenObjectIsNull { get; set; }

        /// <summary>
        /// 当对象为数值的时候，写上引号，这个是默认的
        /// </summary>
        public bool WriteQuoteWhenObjectIsNumber { get; set; }

        #endregion prop

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializeSetting"/> class.
        /// </summary>
        public JsonSerializeSetting() : this("ser")
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">名字</param>
        public JsonSerializeSetting(string name)
        {
            this.Name = name ?? "ser";
            this.DateTimeFormat = DateTimeFormat.Default;
            this.WriteNumberOnBoolenType = true;
            this.WriteHorizontalLineOnGuidType = true;
            this.WriteVersionOnTypeInfo = true;
            this.WriteWithUnicodeOnStringType = false;
            this.MaxDepth = 1024;
            this.WriteNumberOnEnumType = true;
            this.SupportComplexTypeKeyInDictionary = false;
            this.WriteNullWhenObjectIsNull = true;
            this.WriteQuoteWhenObjectIsNumber = true;
        }

        #endregion ctor

        #region equals

        bool IEqualityComparer<JsonSerializeSetting>.Equals(JsonSerializeSetting x, JsonSerializeSetting y)
        {
            return x.Name == y.Name;
        }

        int IEqualityComparer<JsonSerializeSetting>.GetHashCode(JsonSerializeSetting obj)
        {
            return obj.Name == null ? obj.GetHashCode() : obj.Name.GetHashCode();
        }

        bool IEquatable<JsonSerializeSetting>.Equals(JsonSerializeSetting other)
        {
            return this.Name == other.Name;
        }

        #endregion equals
    }
}