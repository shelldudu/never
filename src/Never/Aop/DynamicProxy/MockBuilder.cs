using Never.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Never.Aop.DynamicProxy
{
    /// <summary>
    /// 构建T对象的代理
    /// </summary>
    public class MockBuilder
    {
        #region class

        /// <summary>
        /// 下一个类型名字
        /// </summary>
        protected class NextTypeName
        {
            /// <summary>
            /// 类的名字
            /// </summary>
            public string TypeName { get; set; }

            /// <summary>
            /// 获取命名空间
            /// </summary>
            public string NameSplace { get; set; }
        }

        #endregion class

        #region newTypename

        /// <summary>
        /// 获取新建的类名
        /// </summary>
        /// <returns></returns>
        protected virtual NextTypeName GetNextTypeName(Type type)
        {
            if (type.IsPublic)
                return new NextTypeName() { TypeName = type.Name };

            return new NextTypeName() { TypeName = type.Name, NameSplace = type.Namespace };
        }

        #endregion newTypename

        #region supportType

        /// <summary>
        /// 支持的类型
        /// </summary>
        /// <param name="type">The type.</param>
        public virtual void SupportType(Type type)
        {
            /*不可见，则是private或者protected或者internal的*/
            if (!type.IsVisible)
                throw new ArgumentException(string.Format("mock need the visible type,and {0} is not visible", type.FullName));

            /*支持公开的接口*/
            if (type.IsInterface)
                return;

            /*支持非密封类*/
            if (type.IsClass && !type.IsSealed)
                return;

            throw new ArgumentException(string.Format("mock need the public and not sealed class type or public interface , but {0} is not passed", type.FullName));
        }

        #endregion supportType

        #region get members

        /// <summary>
        /// 获取接口的属性和成员
        /// </summary>
        /// <param name="memberType">对象类型</param>
        public virtual MemberInfo[] GetInterfaceMembers(Type memberType)
        {
            if (!memberType.IsInterface)
                return new MemberInfo[0];

            var members = memberType.GetMembers();
            var interfaces = memberType.GetInterfaces();
            if (interfaces == null || interfaces.Length == 0)
                return members;

            var list = new List<MemberInfo>(members);
            foreach (var @interface in interfaces)
                list.AddRange(GetInterfaceMembers(@interface));

            return list.ToArray();
        }

        /// <summary>
        /// 获取属性和成员
        /// </summary>
        /// <param name="memberType">对象类型</param>
        public virtual MemberInfo[] GetMembers(Type memberType)
        {
            if (memberType.IsInterface)
                return new MemberInfo[0];

            return memberType.GetMembers();
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

        /// <summary>
        /// 获取所有属性成员
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        protected virtual PropertyInfo[] SortProperties(MemberInfo[] members)
        {
            var list = new List<PropertyInfo>(members.Length);
            foreach (var member in members)
            {
                if (member.MemberType == MemberTypes.Property)
                    list.Add((PropertyInfo)member);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 获取所有方法
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        protected virtual MethodInfo[] SortMethods(MemberInfo[] members)
        {
            var list = new List<MethodInfo>(members.Length);
            foreach (var member in members)
            {
                if (member.MemberType == MemberTypes.Method)
                    list.Add((MethodInfo)member);
            }

            return list.ToArray();
        }

        #endregion get members

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
        /// 是否为基元类型
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
        /// 是否为基元类型
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

        #endregion type

        #region get properyName

        /// <summary>
        /// 获取属性的名字
        /// </summary>
        /// <param name="method">方法名字</param>
        /// <returns></returns>
        public virtual string GetPropertyName(MethodInfo method)
        {
            return method.Name.Substring(4);
        }

        #endregion get properyName
    }
}