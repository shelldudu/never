using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Never.SqlClient;
using static Never.EasySql.Linq.TableInfo;
using System.Reflection;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class TableInfoProvider
    {
        /// <summary>
        /// 获取表的信息
        /// </summary>
        static readonly ConcurrentDictionary<Type, TableInfo> tableInfoDictionary = new ConcurrentDictionary<Type, TableInfo>();

        /// <summary>
        /// 获取表的信息
        /// </summary>
        static readonly ConcurrentDictionary<TypeStringKey, TableInfo> customTableInfoDictionary = new ConcurrentDictionary<TypeStringKey, TableInfo>(new TypeStringKeyComparer());

        private struct TypeStringKey : IEquatable<TypeStringKey>
        {
            public Type Type { get; set; }

            public string Cached { get; set; }

            public bool Equals(TypeStringKey other)
            {
                return this.Type == other.Type && this.Cached == other.Cached;
            }
        }

        private struct TypeStringKeyComparer : IEqualityComparer<TypeStringKey>
        {
            public bool Equals(TypeStringKey x, TypeStringKey y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(TypeStringKey obj)
            {
                return obj.GetHashCode();
            }
        }

        /// <summary>
        /// 更新表的信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TableInfo AnalyzeTableInfo(Type type)
        {
            var table = type.GetAttribute<TableNameAttribute>();
            var columns = new List<ColumnInfo>();
            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                if (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field)
                {
                    var column = new ColumnInfo()
                    {
                        Member = member,
                        Column = member.GetAttribute<ColumnAttribute>(),
                        TypeHandler = member.GetAttribute<TypeHandlerAttribute>()
                    };

                    columns.Add(column);
                }
            }

            return new TableInfo()
            {
                TableName = table,
                Columns = columns
            };
        }

        #region type

        /// <summary>
        /// 获取表的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public static bool TryGetTableInfo<T>(out TableInfo tableInfo)
        {
            return TryGetTableInfo(typeof(T), out tableInfo);
        }

        /// <summary>
        /// 获取表的信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public static bool TryGetTableInfo(Type type, out TableInfo tableInfo)
        {
            if (tableInfoDictionary.TryGetValue(type, out tableInfo))
                return true;

            return false;
        }

        /// <summary>
        /// 新加表的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TryAddTableInfo<T>()
        {
            return TryAddTableInfo(typeof(T));
        }

        /// <summary>
        /// 新加表的信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool TryAddTableInfo(Type type)
        {
            return TryUpdateTableInfo(type);
        }

        /// <summary>
        /// 新加表的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public static bool TryAddTableInfo<T>(TableInfo tableInfo)
        {
            return TryAddTableInfo(typeof(T), tableInfo);
        }

        /// <summary>
        /// 新加表的信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public static bool TryAddTableInfo(Type type, TableInfo tableInfo)
        {
            return tableInfoDictionary.TryAdd(type, tableInfo);
        }

        /// <summary>
        /// 更新表的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TryUpdateTableInfo<T>()
        {
            return TryUpdateTableInfo(typeof(T));
        }

        /// <summary>
        /// 更新表的信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool TryUpdateTableInfo(Type type)
        {
            var table = AnalyzeTableInfo(type);
            return TryUpdateTableInfo(type, table);
        }

        /// <summary>
        /// 更新表的信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public static bool TryUpdateTableInfo(Type type, out TableInfo tableInfo)
        {
            tableInfo = AnalyzeTableInfo(type);
            return TryUpdateTableInfo(type, tableInfo);
        }
        /// <summary>
        /// 更新表的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public static bool TryUpdateTableInfo<T>(TableInfo tableInfo)
        {
            return TryUpdateTableInfo(typeof(T), tableInfo);
        }

        /// <summary>
        /// 更新表的信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public static bool TryUpdateTableInfo(Type type, TableInfo tableInfo)
        {
            tableInfoDictionary.TryRemove(type, out _);
            tableInfoDictionary[type] = tableInfo;
            return true;
        }

        #endregion

        #region type and string

        /// <summary>
        /// 获取表的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cachedId">缓存key</param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public static bool TryGetTableInfo<T>(string cachedId, out TableInfo tableInfo)
        {
            return TryGetTableInfo(cachedId, typeof(T), out tableInfo);
        }

        /// <summary>
        /// 获取表的信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cachedId">缓存key</param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public static bool TryGetTableInfo(string cachedId, Type type, out TableInfo tableInfo)
        {
            if (customTableInfoDictionary.TryGetValue(new TypeStringKey() { Type = type, Cached = cachedId }, out tableInfo))
                return true;

            return false;
        }

        /// <summary>
        /// 新加表的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cachedId">缓存key</param>
        /// <returns></returns>
        public static bool TryAddTableInfo<T>(string cachedId)
        {
            return TryAddTableInfo(typeof(T));
        }

        /// <summary>
        /// 新加表的信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cachedId">缓存key</param>
        /// <returns></returns>
        public static bool TryAddTableInfo(string cachedId, Type type)
        {
            return TryUpdateTableInfo(cachedId, type);
        }

        /// <summary>
        /// 新加表的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableInfo"></param>
        /// <param name="cachedId">缓存key</param>
        /// <returns></returns>
        public static bool TryAddTableInfo<T>(string cachedId, TableInfo tableInfo)
        {
            return TryAddTableInfo(cachedId, typeof(T), tableInfo);
        }

        /// <summary>
        /// 新加表的信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cachedId">缓存key</param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public static bool TryAddTableInfo(string cachedId, Type type, TableInfo tableInfo)
        {
            return customTableInfoDictionary.TryAdd(new TypeStringKey() { Type = type, Cached = cachedId }, tableInfo);
        }

        /// <summary>
        /// 更新表的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cachedId">缓存key</param>
        /// <returns></returns>
        public static bool TryUpdateTableInfo<T>(string cachedId)
        {
            return TryUpdateTableInfo(cachedId, typeof(T));
        }

        /// <summary>
        /// 更新表的信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cachedId">缓存key</param>
        /// <returns></returns>
        public static bool TryUpdateTableInfo(string cachedId, Type type)
        {
            var table = AnalyzeTableInfo(type);
            return TryUpdateTableInfo(cachedId, type, table);
        }

        /// <summary>
        /// 更新表的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableInfo"></param>
        /// <param name="cachedId">缓存key</param>
        /// <returns></returns>
        public static bool TryUpdateTableInfo<T>(string cachedId, TableInfo tableInfo)
        {
            return TryUpdateTableInfo(cachedId, typeof(T), tableInfo);
        }

        /// <summary>
        /// 更新表的信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cachedId">缓存key</param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        public static bool TryUpdateTableInfo(string cachedId, Type type, TableInfo tableInfo)
        {
            customTableInfoDictionary.TryRemove(new TypeStringKey() { Type = type, Cached = cachedId }, out _);
            customTableInfoDictionary[new TypeStringKey() { Type = type, Cached = cachedId }] = tableInfo;
            return true;
        }
        #endregion
    }
}
