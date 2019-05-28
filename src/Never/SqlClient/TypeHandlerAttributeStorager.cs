using System.Collections.Generic;

namespace Never.SqlClient
{
    /// <summary>
    /// 类型处理存储器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TypeHandlerAttributeStorager<T>
    {
        /// <summary>
        /// 存储器
        /// </summary>
        private static SortedList<string, TypeHandlerAttribute> storager = new SortedList<string, TypeHandlerAttribute>(20);

        /// <summary>
        /// 存储
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="memberName"></param>
        public static void Storage(TypeHandlerAttribute attribute, string memberName)
        {
            storager[memberName] = attribute;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static TypeHandlerAttribute Query(string memberName)
        {
            TypeHandlerAttribute attribute = null;
            storager.TryGetValue(memberName, out attribute);
            return attribute;
        }
    }
}