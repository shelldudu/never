using System;
using System.Collections;
using System.Reflection;

namespace Never.Reflection
{
    /// <summary>
    /// 类型助手，因为可能有针对不同的环境下编译，所以还是放在这里好了
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// 获取上一层的类型
        /// </summary>
        /// <param name="type">目标对象</param>
        /// <returns></returns>
        public static Type GetBaseType(Type type)
        {
            var t = type;
            while (true)
            {
                if (t.BaseType == null)
                    return t;

                t = t.BaseType;
            }
        }

        /// <summary>
        /// 查询指定某个接口
        /// </summary>
        /// <param name="type"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static Type GetInterface(Type type, Type targetType)
        {
            if (type == targetType)
                return type;

            var @interfaces = type.FindInterfaces((objType, objCriteria) => true, null);
            foreach (var @interface in @interfaces)
            {
                if (@interface == targetType)
                    return @interface;

                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == targetType)
                    return @interface;
            }

            return null;
        }

        /// <summary>
        /// 查询指定某个接口
        /// </summary>
        public static Type GetInterface<TargetType>(Type type)
        {
            return GetInterface(type, typeof(TargetType));
        }

        /// <summary>
        /// 当前是否值对象
        /// </summary>
        /// <param name="type">目标对象</param>
        /// <returns></returns>
        public static bool IsValueType(Type type)
        {
            return type.IsValueType;
        }

        /// <summary>
        /// 当前是否包含泛型参数
        /// </summary>
        /// <param name="type">目标对象</param>
        /// <returns></returns>
        public static bool ContainsGenericParameters(Type type)
        {
            return type.ContainsGenericParameters;
        }

        /// <summary>
        /// 当前是否泛型
        /// </summary>
        /// <param name="type">目标对象</param>
        /// <returns></returns>
        public static bool IsGenericType(Type type)
        {
            return type.IsGenericType;
        }

        /// <summary>
        /// 当前是否枚举
        /// </summary>
        /// <param name="type">目标对象</param>
        /// <returns></returns>
        public static bool IsEnumType(Type type)
        {
            return type.IsEnum;
        }

        /// <summary>
        /// 当前是Type
        /// </summary>
        /// <param name="type">目标对象</param>
        /// <returns></returns>
        public static bool IsType(Type type)
        {
            return type == typeof(Type);
        }

        /// <summary>
        /// 当前是否基元类型
        /// </summary>
        /// <param name="type">目标对象</param>
        /// <returns></returns>
        public static bool IsPrimitiveType(Type type)
        {
            return type.IsPrimitive;
        }

        /// <summary>
        /// 当前是否接口
        /// </summary>
        /// <param name="type">目标对象</param>
        /// <returns></returns>
        public static bool IsInterface(Type type)
        {
            return type.IsInterface;
        }

        /// <summary>
        /// 是否为数组
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEnumerableType(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// 是否为字符串类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsStringType(Type type)
        {
            return type == typeof(string);
        }

        /// <summary>
        /// 是否为字符串类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCharType(Type type)
        {
            return type == typeof(char);
        }

        /// <summary>
        /// 是否可以从实例分配
        /// </summary>
        /// <param name="implType">实现类</param>
        /// <param name="baseType">基类</param>
        public static bool IsAssignableFrom(Type implType, Type baseType)
        {
            return baseType.IsAssignableFrom(implType);
        }

        /// <summary>
        /// 是否为整形数值类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIntegerNumberType(Type type)
        {
            return
                type == typeof(byte) ||
                type == typeof(sbyte) ||
                type == typeof(short) ||
                type == typeof(ushort) ||
                type == typeof(int) ||
                type == typeof(uint) ||
                type == typeof(long) ||
                type == typeof(ulong);
        }

        /// <summary>
        /// 是否为数值类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDigitType(Type type)
        {
            return
                type == typeof(byte) ||
                type == typeof(sbyte) ||
                type == typeof(short) ||
                type == typeof(ushort) ||
                type == typeof(int) ||
                type == typeof(uint) ||
                type == typeof(long) ||
                type == typeof(ulong) ||
                type == typeof(float) ||
                type == typeof(double) ||
                type == typeof(decimal);
        }

        /// <summary>
        /// 是否为浮点数值类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsFloatingPointNumberType(Type type)
        {
            return type == typeof(float) || type == typeof(double) || type == typeof(decimal);
        }

        /// <summary>
        /// 是否包含目标类型的
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static bool IsContainType(Type sourceType, Type targetType)
        {
            if (sourceType == targetType)
                return true;

            if (sourceType.IsGenericType && sourceType.GetGenericTypeDefinition() == targetType)
                return true;

            var interfaces = sourceType.GetInterfaces();
            if (interfaces == null || interfaces.Length <= 0)
                return false;

            for (var i = 0; i < interfaces.Length; i++)
            {
                var inter = interfaces[i];
                if (inter.IsGenericType)
                {
                    if (inter.GetGenericTypeDefinition() == targetType)
                        return true;
                }

                if (inter == targetType)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 获取某一个方法
        /// </summary>
        /// <param name="type">目标对象</param>
        /// <param name="name"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public static MethodInfo GetMethod(Type type, string name, Type[] parameterTypes)
        {
            return type.GetMethod(name, parameterTypes);
        }

        /// <summary>
        /// 获取当前类型所处的模块
        /// </summary>
        /// <param name="type">目标对象</param>
        /// <returns></returns>
        public static Module GetModule(Type type)
        {
            return type.Module;
        }

        /// <summary>
        /// 是否指针
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPointer(Type type)
        {
            return type.IsPointer;
        }
    }
}