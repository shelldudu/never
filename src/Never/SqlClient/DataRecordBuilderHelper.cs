using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Never.SqlClient
{
    /// <summary>
    /// 助手
    /// </summary>
    internal static class DataRecordBuilderHelper
    {
        #region field

        /// <summary>
        /// 已经定义好的常用类型
        /// </summary>
        internal readonly static IDictionary<Type, MethodInfo> DefinedTypeDict = null;

        #endregion field

        #region method

        /// <summary>
        /// 获取Type的值
        /// </summary>
        internal static MethodInfo _mGetFieldType = typeof(IDataRecord).GetMethod("GetFieldType", new[] { typeof(int) });

        /// <summary>
        /// 获取object的值
        /// </summary>
        internal static MethodInfo _mGetValue = typeof(IDataRecord).GetMethod("GetValue", new[] { typeof(int) });

        /// <summary>
        /// 获取行的名字
        /// </summary>
        internal static MethodInfo _mGetName = typeof(IDataRecord).GetMethod("GetName", new[] { typeof(int) });

        /// <summary>
        /// 根据名字获取索引值
        /// </summary>
        internal static MethodInfo _mGetOrdinal = typeof(IDataRecord).GetMethod("GetOrdinal", new[] { typeof(string) });

        /// <summary>
        /// 获取是否为空
        /// </summary>
        internal static MethodInfo _mIsDBNull = typeof(IDataRecord).GetMethod("IsDBNull", new[] { typeof(int) });

        #endregion method

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlExecuter"/> class.
        /// </summary>
        static DataRecordBuilderHelper()
        {
            DefinedTypeDict = new Dictionary<Type, MethodInfo>(12);
            DefinedTypeDict[typeof(short)] = typeof(IDataRecord).GetMethod("GetInt16", new[] { typeof(int) });
            DefinedTypeDict[typeof(int)] = typeof(IDataRecord).GetMethod("GetInt32", new[] { typeof(int) });
            DefinedTypeDict[typeof(long)] = typeof(IDataRecord).GetMethod("GetInt64", new[] { typeof(int) });
            DefinedTypeDict[typeof(string)] = typeof(IDataRecord).GetMethod("GetString", new[] { typeof(int) });
            DefinedTypeDict[typeof(float)] = typeof(IDataRecord).GetMethod("GetFloat", new[] { typeof(int) });
            DefinedTypeDict[typeof(double)] = typeof(IDataRecord).GetMethod("GetDouble", new[] { typeof(int) });
            DefinedTypeDict[typeof(DateTime)] = typeof(IDataRecord).GetMethod("GetDateTime", new[] { typeof(int) });
            DefinedTypeDict[typeof(char)] = typeof(IDataRecord).GetMethod("GetChar", new[] { typeof(int) });
            DefinedTypeDict[typeof(bool)] = typeof(IDataRecord).GetMethod("GetBoolean", new[] { typeof(int) });
            DefinedTypeDict[typeof(byte)] = typeof(IDataRecord).GetMethod("GetByte", new[] { typeof(int) });
            DefinedTypeDict[typeof(decimal)] = typeof(IDataRecord).GetMethod("GetDecimal", new[] { typeof(int) });
            DefinedTypeDict[typeof(Guid)] = typeof(IDataRecord).GetMethod("GetGuid", new[] { typeof(int) });
        }

        #endregion ctor

        #region emun

        /// <summary>
        /// 转换枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static T _EnumParse<T, V>(V value) where T : struct, IConvertible
        {
            var type = typeof(T);
            if (!type.IsEnum)
                return default(T);

            return (T)Enum.Parse(typeof(T), value.ToString());
        }

        #endregion emun

        #region typehandler

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Instance"></typeparam>
        /// <param name="dataRecord"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static T ReadingValueFromDataRecordTypeHandler<T, Instance>(System.Data.IDataRecord dataRecord, string memberName)
        {
            var attribute = TypeHandlerAttributeStorager<Instance>.Query(memberName);
            if (attribute == null)
                return default(T);

            return attribute.OnDataReading<T>(dataRecord, memberName);
        }

        #endregion typehandler
    }
}