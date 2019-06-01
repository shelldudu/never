using Never.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Never.Serialization.Json
{
    /// <summary>
    /// 序列化和反序列化
    /// </summary>
    public abstract class JsonEmitBuilder<T>
    {
        #region field

        /// <summary>
        /// 来源类型
        /// </summary>
        public readonly Type TargetType = null;

        /// <summary>
        /// 要忽略的成员
        /// </summary>
        protected readonly List<string> IgnoredMembers = null;

        /// <summary>
        /// 要忽略的成员的比较器
        /// </summary>
        protected readonly StringComparison StringComparer = StringComparison.Ordinal;

        /// <summary>
        /// 用于检查是否递归引用
        /// </summary>
        protected readonly List<Type> Recursion = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonEmitBuilder{T}"/> class.
        /// </summary>
        protected JsonEmitBuilder()
            : this(new List<string>(2), StringComparison.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonEmitBuilder{T}"/> class.
        /// </summary>
        /// <param name="ignoredMembers">The ignored members.</param>
        /// <param name="stringComparer">The string comparer.</param>
        protected JsonEmitBuilder(List<string> ignoredMembers, StringComparison stringComparer)
        {
            this.Recursion = new List<Type>(30);
            this.IgnoredMembers = ignoredMembers ?? new List<string>(2);
            this.StringComparer = stringComparer;
            this.TargetType = typeof(T);
        }

        #endregion ctor

        #region get members

        /// <summary>
        /// 获取属性和成员
        /// </summary>
        /// <param name="memberType">对象类型</param>
        public virtual MemberInfo[] GetMembers(Type memberType)
        {
            var members = memberType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
            var list = new List<MemberInfo>(members.Length);
            foreach (var member in members)
            {
                if (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field)
                    list.Add(member);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 获取属性和成员,第一个值是基元，第二个值是非基元
        /// </summary>
        /// <param name="memberType">对象类型</param>
        /// <returns>第一个值是基元，第二个值是非基元</returns>
        protected KeyValuePair<MemberInfo[], MemberInfo[]> SortMembers(Type memberType)
        {
            var members = this.GetMembers(memberType);
            if (members == null || members.Length == 0)
                return new KeyValuePair<MemberInfo[], MemberInfo[]>(new MemberInfo[0], new MemberInfo[0]);

            var primitive = new List<MemberInfo>(members.Length);
            var notPrimitive = new List<MemberInfo>(members.Length);

            foreach (var m in members)
            {
                var mType = m.MemberType == MemberTypes.Property ? ((PropertyInfo)m).PropertyType : ((FieldInfo)m).FieldType;
                if (this.IsPrimitiveOrInsideHandleType(mType) || this.IsEnumType(mType) || this.IsNullablePrimitiveOrInsideHandleType(mType) || this.IsNullableEnumType(mType))
                    primitive.Add(m);
                else
                    notPrimitive.Add(m);
            }

            return new KeyValuePair<MemberInfo[], MemberInfo[]>(primitive.ToArray(), notPrimitive.ToArray());
        }

        #endregion get members

        #region ignored

        /// <summary>
        /// 是否属于忽略文件
        /// </summary>
        /// <param name="member">成员</param>
        protected virtual bool IsContainIgnoredMember(MemberInfo member)
        {
            if (this.IgnoredMembers.Count == 0)
                return false;

            if (member == null)
                return false;

            if (string.IsNullOrEmpty(member.Name))
                return false;

            return this.IgnoredMembers.Find(t => { return string.Equals(t, member.Name, this.StringComparer); }) != null;
        }

        #endregion ignored

        #region recursion

        /// <summary>
        /// 检查该对象Type是否在递归作用链中
        /// </summary>
        /// <param name="targetType">目标类型</param>
        protected virtual Type ContainRecursionReference(Type targetType)
        {
            if (targetType == typeof(object))
                return null;

            if (this.IsEnumType(targetType))
                return null;

            if (this.IsPrimitiveOrInsideHandleType(targetType))
                return null;

            if (this.IsNullableEnumType(targetType))
                return null;

            if (this.IsNullableEnumType(targetType))
                return null;

            if (this.Recursion.Count == 0)
            {
                this.Recursion.Add(targetType);
                return null;
            }

            for (var i = 0; i < this.Recursion.Count; i++)
            {
                if (TypeHelper.IsAssignableFrom(targetType, this.Recursion[i]))
                    return this.Recursion[i];

                if (TypeHelper.IsAssignableFrom(this.Recursion[i], targetType))
                    return this.Recursion[i];
            }

            this.Recursion.Add(targetType);
            return null;
        }

        /// <summary>
        /// 删除对象Type是否在递归作用链中
        /// </summary>
        /// <param name="targetType">目标类型</param>
        protected virtual void RemoveRecursionReference(Type targetType)
        {
            this.Recursion.Remove(targetType);
        }

        #endregion recursion

        #region type

        /// <summary>
        /// 是否可以从实例分配
        /// </summary>
        /// <param name="implType">实现类</param>
        /// <param name="baseType">基类</param>
        public virtual bool IsAssignableFrom(Type implType, Type baseType)
        {
            return TypeHelper.IsAssignableFrom(implType, baseType);
        }

        /// <summary>
        /// 是否含有了该类
        /// </summary>
        /// <param name="sourceType">源类型</param>
        /// <param name="targetType">目标类型</param>
        public virtual bool IsContainType(Type sourceType, Type targetType)
        {
            return TypeHelper.IsContainType(sourceType, targetType);
        }

        /// <summary>
        /// 是否复合对象
        /// </summary>
        public virtual bool IsComplexType(MemberInfo memberInfo)
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
        public virtual bool IsValueType(Type type)
        {
            if (type == null)
                return false;

            return type.IsValueType;
        }

        /// <summary>
        /// 是否为复合对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsComplexType(Type type)
        {
            if (type == null)
                return false;

            if (TypeHelper.IsEnumType(type))
                return false;

            if (this.IsNullableEnumType(type))
                return false;

            if (this.IsPrimitiveOrInsideHandleType(type))
                return false;

            if (this.IsNullablePrimitiveOrInsideHandleType(type))
                return false;

            if (TypeHelper.IsEnumerableType(type))
                return false;

            if (type == typeof(object))
                return false;

            return true;
        }

        /// <summary>
        /// 是否为基元类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsPrimitiveType(Type type)
        {
            if (TypeHelper.IsPrimitiveType(type))
                return true;

            return false;
        }

        /// <summary>
        /// 是否为基元类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsNullablePrimitiveType(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType == null)
                return false;

            if (TypeHelper.IsPrimitiveType(nullableType))
                return true;

            return false;
        }

        /// <summary>
        /// 是否为基元或内部处理类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsPrimitiveOrInsideHandleType(Type type)
        {
            if (TypeHelper.IsPrimitiveType(type))
                return true;

            if (type == typeof(string) || type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(Guid) || type == typeof(TimeSpan) || type == typeof(decimal))
                return true;

            return false;
        }

        /// <summary>
        /// 是否为基元或内部处理类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsNullablePrimitiveOrInsideHandleType(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType == null)
                return false;

            if (TypeHelper.IsPrimitiveType(nullableType))
                return true;

            if (nullableType == typeof(string) || nullableType == typeof(DateTime) || nullableType == typeof(DateTimeOffset) || nullableType == typeof(Guid) || nullableType == typeof(TimeSpan) || nullableType == typeof(decimal))
                return true;

            return false;
        }

        /// <summary>
        /// 是否为enumType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsEnumType(Type type)
        {
            return TypeHelper.IsEnumType(type);
        }

        /// <summary>
        /// 是否为enumType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsNullableEnumType(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType == null)
                return false;

            return TypeHelper.IsEnumType(nullableType);
        }

        /// <summary>
        /// 是否为Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsType(Type type)
        {
            return TypeHelper.IsType(type);
        }

        /// <summary>
        /// 是否为数值类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsDigitType(Type type)
        {
            return TypeHelper.IsDigitType(type);
        }

        /// <summary>
        /// 获取泛型数组接口
        /// </summary>
        /// <param name="type"></param>
        public virtual Type[] GetGenericEnumerableInterfaces(Type type)
        {
            return GetInterfaces(type, typeof(IEnumerable<>));
        }

        /// <summary>
        /// 获取包含某个类型的接口
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericType">泛型类型</param>
        public virtual Type[] GetInterfaces(Type type, Type genericType)
        {
            var interfaces = type.GetInterfaces();
            var list = new List<Type>(interfaces.Length);
            foreach (var t in interfaces)
            {
                if (TypeHelper.IsContainType(t, genericType))
                    list.Add(t);
            }

            if (TypeHelper.IsContainType(type, genericType))
                list.Add(type);

            return list.ToArray();
        }

        /// <summary>
        /// 获取数组接口
        /// </summary>
        /// <param name="type"></param>
        public virtual Type[] GetEnumerableInterfaces(Type type)
        {
            return GetInterfaces(type, typeof(IEnumerable));
        }

        /// <summary>
        /// 是否是不用写引号的类型
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public virtual bool IsNoQuotationMarkType(Type type)
        {
            return TypeHelper.IsDigitType(type);
        }

        /// <summary>
        /// 是否为数组
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsEnumerableType(Type type)
        {
            if (type.IsArray)
                return true;

            if (type.IsAssignableFromType(typeof(IEnumerable<>)))
                return true;

            return false;
        }

        #endregion type

        #region load

        /// <summary>
        /// 加载所有属性
        /// </summary>
        /// <returns></returns>
        protected virtual Attribute[] LoadAttributes(MemberInfo member, bool inherit)
        {
            var temp = member.GetCustomAttributes(inherit);
            if (temp.Length == 0)
                return new Attribute[0];

            var attributes = new Attribute[temp.Length];
            for (var i = 0; i < temp.Length; i++)
                attributes[i] = (Attribute)temp[i];

            return attributes;
        }

        /// <summary>
        /// 是否包括了忽略的属性
        /// </summary>
        /// <returns></returns>
        protected virtual bool ContainIgoreAttribute(MemberInfo member, Attribute[] attributes)
        {
            if (attributes == null || attributes.Length == 0)
                return false;

            return ObjectExtension.GetAttribute<IgnoreDataMemberAttribute>(attributes) != null;
        }

        /// <summary>
        /// 加载名字
        /// </summary>
        /// <returns></returns>
        protected virtual string LoadMemberName(MemberInfo member, Attribute[] attributes)
        {
            if (attributes == null || attributes.Length == 0)
                return member.Name;

            var jsonMember = ObjectExtension.GetAttribute<DataMemberAttribute>(attributes);
            if (jsonMember != null)
                return jsonMember.Name;

            var dataMember = ObjectExtension.GetAttribute<System.Runtime.Serialization.DataMemberAttribute>(attributes);
            if (dataMember != null)
                return dataMember.Name;
            return member.Name;
        }

        #endregion load
    }
}