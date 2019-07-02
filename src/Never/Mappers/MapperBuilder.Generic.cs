using Never.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Never.Mappers
{
    /// <summary>
    /// 自动映射创建
    /// </summary>
    public partial class MapperBuilder<From, To> : MapperBuilder
    {
        #region field and ctor

        private static ConcurrentDictionary<MapperSetting, Func<From, To>> funcCaching = null;

        private static ConcurrentDictionary<MapperSetting, Action<From, To>> actionCaching = null;

        static MapperBuilder()
        {
            funcCaching = new ConcurrentDictionary<MapperSetting, Func<From, To>>(new MapperSetting());
            actionCaching = new ConcurrentDictionary<MapperSetting, Action<From, To>>(new MapperSetting());
        }

        #endregion field and ctor

        #region find

        /// <summary>
        /// 查询IEnumerable KeyValuePair Type
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        private Type FindIEnumerableKeyValuePairGenericType(Type sourceType)
        {
            if (sourceType.IsAssignableFromType(typeof(IEnumerable<>)) == false)
                return null;

            if (sourceType.IsGenericType && sourceType.IsGenericTypeDefinition == false)
            {
                var parameters = sourceType.GetGenericArguments();
                foreach (var parameter in parameters)
                {
                    if (parameter.IsAssignableFromType(typeof(KeyValuePair<,>)))
                    {
                        return typeof(IEnumerable<>).MakeGenericType(parameter);
                    }
                }
            }

            var @interfaces = sourceType.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                var type = this.FindIEnumerableKeyValuePairGenericType(@interface);
                if (type != null)
                    return type;
            }

            return null;
        }

        /// <summary>
        /// 查询指定某个接口，可能返回泛型接口
        /// </summary>
        /// <param name="type"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private Type FindInterfaceOrGenericInterface(Type type, Type targetType)
        {
            if (type == targetType)
                return type;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == targetType)
                return type;

            var @interfaces = type.GetInterfaces();
            foreach (var @interface in @interfaces)
            {
                if (@interface == targetType)
                    return @interface;

                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == targetType)
                    return @interface;
            }

            return null;
        }

        #endregion find
    }
}