using System;
using System.Collections.Generic;

namespace Never.Serialization.Json
{
    /// <summary>
    /// setting
    /// </summary>
    public class JsonDeserializeSetting : IEqualityComparer<JsonDeserializeSetting>, IEquatable<JsonDeserializeSetting>
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

        #endregion prop

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDeserializeSetting"/> class.
        /// </summary>
        public JsonDeserializeSetting() : this("der")
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">名字</param>
        public JsonDeserializeSetting(string name)
        {
            this.Name = name ?? "desr";
            this.DateTimeFormat = DateTimeFormat.Default;
        }

        #endregion ctor

        #region equals

        bool IEqualityComparer<JsonDeserializeSetting>.Equals(JsonDeserializeSetting x, JsonDeserializeSetting y)
        {
            return x.Name == y.Name;
        }

        int IEqualityComparer<JsonDeserializeSetting>.GetHashCode(JsonDeserializeSetting obj)
        {
            return obj.Name == null ? obj.GetHashCode() : obj.Name.GetHashCode();
        }

        bool IEquatable<JsonDeserializeSetting>.Equals(JsonDeserializeSetting other)
        {
            return this.Name == other.Name;
        }

        #endregion equals
    }
}