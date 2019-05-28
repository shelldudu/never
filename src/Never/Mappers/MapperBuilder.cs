using Never.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Never.Mappers
{
    /// <summary>
    /// 自动映射创建
    /// </summary>
    public class MapperBuilder
    {
        #region members

        /// <summary>
        /// 获取成员
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        protected List<MemberInfo> GetMembers(Type targetType)
        {
            var members = targetType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
            if (members == null || members.Length == 0)
                return new List<MemberInfo>(0);

            var list = new List<MemberInfo>(members.Length);
            foreach (var to in members)
            {
                if (to.MemberType == MemberTypes.Property || to.MemberType == MemberTypes.Field)
                    list.Add(to);
            }

            return list;
        }

        /// <summary>
        /// 是否为复合对象
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        protected virtual bool IsComplexType(MemberInfo memberInfo)
        {
            if (memberInfo == null)
                return false;

            var type = memberInfo as Type;
            if (type != null)
                return IsComplexType(type);

            var prop = memberInfo as PropertyInfo;
            if (prop != null)
                return IsComplexType(prop.PropertyType);

            var field = memberInfo as FieldInfo;
            if (field != null)
                return IsComplexType(field.FieldType);

            return false;
        }

        /// <summary>
        /// 是否为复合对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual bool IsComplexType(Type type)
        {
            if (type == null)
                return false;

            if (TypeHelper.IsPrimitiveType(type))
                return false;

            if (TypeHelper.IsEnumType(type))
                return false;

            if (type == typeof(string) || type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(Guid) || type == typeof(TimeSpan) || type == typeof(decimal))
                return false;

            if (TypeHelper.IsEnumerableType(type))
                return false;

            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType == null)
                return true;

            if (TypeHelper.IsPrimitiveType(nullableType))
                return false;

            if (TypeHelper.IsEnumType(nullableType))
                return false;

            if (nullableType == typeof(string) || nullableType == typeof(DateTime) || nullableType == typeof(DateTimeOffset) || nullableType == typeof(Guid) || nullableType == typeof(TimeSpan) || nullableType == typeof(decimal))
                return false;

            return true;
        }

        /// <summary>
        /// 类型是否可以互相转换
        /// </summary>
        /// <param name="destinationType"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        protected virtual bool CanConvert(Type destinationType, Type sourceType)
        {
            var destinationConverter = GetConverter(destinationType);
            if (destinationConverter != null && destinationConverter.CanConvertFrom(sourceType))
                return true;

            var sourceConverter = GetConverter(sourceType);
            if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                return true;

            return false;
        }

        /// <summary>
        /// 获取类型转换器
        /// </summary>
        /// <param name="seqType">源类型</param>
        /// <returns></returns>
        protected virtual TypeConverter GetConverter(Type seqType)
        {
            return TypeDescriptor.GetConverter(seqType);
        }

        #endregion members
    }
}