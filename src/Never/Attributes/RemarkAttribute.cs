using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never.Attributes
{
    /// <summary>
    /// 一个标识属性
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class RemarkAttribute : Attribute
    {
        #region field

        /// <summary>
        /// 该对象别称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 该值是否可见
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="RemarkAttribute"/> class.
        /// </summary>
        public RemarkAttribute()
        {
            this.Name = string.Empty;
            this.Group = string.Empty;
            this.Value = string.Empty;
            this.Visible = true;
        }

        #endregion ctor

        /// <summary>
        /// 获取Type类型特性
        /// </summary>
        public static RemarkAttribute GetRemarkAttribute(MemberInfo memberInfo, bool inherit = false)
        {
            var attribute = memberInfo.GetAttribute<RemarkAttribute>(inherit);
            return attribute ?? new RemarkAttribute();
        }

        /// <summary>
        /// 通过枚举实例对象获取特性
        /// </summary>
        public static RemarkAttribute GetRemarkAttribute(Enum @enum, bool inherit = false)
        {
            var type = @enum.GetType();
            var field = type.GetField(@enum.ToString());
            if (field == null)
                return new RemarkAttribute();

            return GetRemarkAttribute(field, inherit);
        }

        /// <summary>
        /// 通过枚举实例对象获取字典: Key为枚举基础类型，Value为<see cref="RemarkAttribute"/>特性
        /// </summary>
        public static IDictionary<TValue, RemarkAttribute> GetRemarkAttributes<TValue>(Enum @enum, bool inherit = false) where TValue : struct, IConvertible
        {
            return GetRemarkAttributes<TValue>(@enum.GetType(), inherit);
        }

        /// <summary>
        /// 通过枚举实例对象获取字典: Key为枚举基础类型，Value为<see cref="RemarkAttribute"/>特性
        /// </summary>
        public static IDictionary<TValue, RemarkAttribute> GetRemarkAttributes<TValue>(Type enumType, bool inherit = false) where TValue : struct, IConvertible
        {
            if (!enumType.IsEnum)
                throw new ArgumentException("enumType不是枚举类型", "enumType");

            var members = enumType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            if (members == null || members.Length == 0)
                return new Dictionary<TValue, RemarkAttribute>();

            var dict = new Dictionary<TValue, RemarkAttribute>();
            foreach (var member in members)
            {
                if (member == null)
                    continue;

                var attribute = member.GetAttribute<RemarkAttribute>(inherit);
                if (attribute == null)
                    continue;

                var key = default(TValue);
                key = (TValue)member.GetValue(enumType);
                dict[key] = attribute;
            }

            return dict;
        }
    }
}