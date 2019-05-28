using Never.Serialization.Json.Deserialize.Enumerators;
using Never.Serialization.Json.MethodProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Never.Serialization.Json.Deserialize
{
    internal class DeseralizerBuilderHelper
    {
        #region field

        /// <summary>
        /// 常见的转换方法
        /// </summary>
        private readonly static IDictionary<Type, MethodInfo> definedWriteMethodDict = null;

        /// <summary>
        /// 常见的类型
        /// </summary>
        private readonly static IList<Type> primitiveTypeAndNullablePritiveTypeDict = null;

        /// <summary>
        /// 对数组常见的转换方法
        /// </summary>
        private readonly static IDictionary<Type, MethodInfo> primitiveArrayDefinedWriteMethodDict = null;

        #endregion field

        #region ctor

        static DeseralizerBuilderHelper()
        {
            definedWriteMethodDict = new Dictionary<Type, MethodInfo>(15);
            var methodProvideEngrafting = typeof(ParseMethodProviderEngrafting);
            definedWriteMethodDict[typeof(bool)] = methodProvideEngrafting.GetMethod("BoolParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(byte)] = methodProvideEngrafting.GetMethod("ByteParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(char)] = methodProvideEngrafting.GetMethod("CharParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(DateTime)] = methodProvideEngrafting.GetMethod("DateTimeParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(decimal)] = methodProvideEngrafting.GetMethod("DecimalParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(double)] = methodProvideEngrafting.GetMethod("DoubleParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(float)] = methodProvideEngrafting.GetMethod("FloatParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(Guid)] = methodProvideEngrafting.GetMethod("GuidParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(short)] = methodProvideEngrafting.GetMethod("Int16Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) }); ;
            definedWriteMethodDict[typeof(int)] = methodProvideEngrafting.GetMethod("Int32Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(long)] = methodProvideEngrafting.GetMethod("Int64Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(string)] = methodProvideEngrafting.GetMethod("StringParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(ushort)] = methodProvideEngrafting.GetMethod("UInt16Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(uint)] = methodProvideEngrafting.GetMethod("UInt32Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(ulong)] = methodProvideEngrafting.GetMethod("UInt64Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(Type)] = methodProvideEngrafting.GetMethod("TypeParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(TimeSpan)] = methodProvideEngrafting.GetMethod("TimeSpanParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });

            var nullableMethodProvideEngrafting = typeof(NullableParseMethodProviderEngrafting);
            definedWriteMethodDict[typeof(bool?)] = nullableMethodProvideEngrafting.GetMethod("BoolParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(byte?)] = nullableMethodProvideEngrafting.GetMethod("ByteParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(char?)] = nullableMethodProvideEngrafting.GetMethod("CharParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(DateTime?)] = nullableMethodProvideEngrafting.GetMethod("DateTimeParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(decimal?)] = nullableMethodProvideEngrafting.GetMethod("DecimalParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(double?)] = nullableMethodProvideEngrafting.GetMethod("DoubleParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(float?)] = nullableMethodProvideEngrafting.GetMethod("FloatParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(Guid?)] = nullableMethodProvideEngrafting.GetMethod("GuidParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(short?)] = nullableMethodProvideEngrafting.GetMethod("Int16Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) }); ;
            definedWriteMethodDict[typeof(int?)] = nullableMethodProvideEngrafting.GetMethod("Int32Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(long?)] = nullableMethodProvideEngrafting.GetMethod("Int64Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(ushort?)] = nullableMethodProvideEngrafting.GetMethod("UInt16Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(uint?)] = nullableMethodProvideEngrafting.GetMethod("UInt32Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(ulong?)] = nullableMethodProvideEngrafting.GetMethod("UInt64Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(TimeSpan?)] = nullableMethodProvideEngrafting.GetMethod("TimeSpanParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string) });

            primitiveTypeAndNullablePritiveTypeDict = new List<Type>(30);

            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(short));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(int));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(long));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(string));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(float));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(double));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(DateTime));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(char));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(bool));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(byte));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(decimal));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(Guid));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(ushort));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(uint));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(ulong));

            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(short?));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(int?));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(long?));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(float?));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(double?));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(DateTime?));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(char?));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(bool?));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(byte?));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(decimal?));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(Guid?));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(ushort?));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(uint?));
            primitiveTypeAndNullablePritiveTypeDict.Add(typeof(ulong?));

            primitiveArrayDefinedWriteMethodDict = new Dictionary<Type, MethodInfo>(30);
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<bool>)] = methodProvideEngrafting.GetMethod("ArrayBoolParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<byte>)] = methodProvideEngrafting.GetMethod("ArrayByteParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<char>)] = methodProvideEngrafting.GetMethod("ArrayCharParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<DateTime>)] = methodProvideEngrafting.GetMethod("ArrayDateTimeParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<decimal>)] = methodProvideEngrafting.GetMethod("ArrayDecimalParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<double>)] = methodProvideEngrafting.GetMethod("ArrayDoubleParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<float>)] = methodProvideEngrafting.GetMethod("ArrayFloatParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<Guid>)] = methodProvideEngrafting.GetMethod("ArrayGuidParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<short>)] = methodProvideEngrafting.GetMethod("ArrayInt16Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<int>)] = methodProvideEngrafting.GetMethod("ArrayInt32Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<long>)] = methodProvideEngrafting.GetMethod("ArrayInt64Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<string>)] = methodProvideEngrafting.GetMethod("ArrayStringParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<ushort>)] = methodProvideEngrafting.GetMethod("ArrayUInt16Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<uint>)] = methodProvideEngrafting.GetMethod("ArrayUInt32Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<ulong>)] = methodProvideEngrafting.GetMethod("ArrayUInt64Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });

            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<bool?>)] = nullableMethodProvideEngrafting.GetMethod("ArrayBoolParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<byte?>)] = nullableMethodProvideEngrafting.GetMethod("ArrayByteParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<char?>)] = nullableMethodProvideEngrafting.GetMethod("ArrayCharParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<DateTime?>)] = nullableMethodProvideEngrafting.GetMethod("ArrayDateTimeParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<decimal?>)] = nullableMethodProvideEngrafting.GetMethod("ArrayDecimalParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<double?>)] = nullableMethodProvideEngrafting.GetMethod("ArrayDoubleParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<float?>)] = nullableMethodProvideEngrafting.GetMethod("ArrayFloatParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<Guid?>)] = nullableMethodProvideEngrafting.GetMethod("ArrayGuidParse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<short?>)] = nullableMethodProvideEngrafting.GetMethod("ArrayInt16Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<int?>)] = nullableMethodProvideEngrafting.GetMethod("ArrayInt32Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<long?>)] = nullableMethodProvideEngrafting.GetMethod("ArrayInt64Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<ushort?>)] = nullableMethodProvideEngrafting.GetMethod("ArrayUInt16Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<uint?>)] = nullableMethodProvideEngrafting.GetMethod("ArrayUInt32Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<ulong?>)] = nullableMethodProvideEngrafting.GetMethod("ArrayUInt64Parse", new[] { typeof(IDeserializerReader), typeof(JsonDeserializeSetting), typeof(string), typeof(int) });
        }

        #endregion ctor

        #region getmethod

        /// <summary>
        /// 获取写入流内容方法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MethodInfo GetParseMethod(Type type)
        {
            MethodInfo method = null;
            definedWriteMethodDict.TryGetValue(type, out method);
            return method;
        }

        /// <summary>
        /// 获取基元类型写入流内容方法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MethodInfo GetPrimitiveArrayParseMethod(Type type)
        {
            MethodInfo method = null;
            primitiveArrayDefinedWriteMethodDict.TryGetValue(type, out method);
            return method;
        }

        #endregion getmethod

        #region custom deser

        /// <summary>
        /// 执行builder,反射用到的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="name"></param>
        /// <param name="arrayLevel">层次</param>
        internal static T CallBuilderInvoke<T>(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            var @delegate = ZzzZzDeserializerBuilder<T>.Register(setting);
            return name == null ? @delegate.Invoke(reader, setting, arrayLevel) : @delegate.Invoke(reader.Parse(reader.Read(name)), setting, arrayLevel);
        }

        #endregion custom deser

        #region containType

        /// <summary>
        /// 是否包含类型
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static bool ContainPrimitiveDefinedType(Type targetType)
        {
            return primitiveTypeAndNullablePritiveTypeDict.Contains(targetType);
        }

        #endregion containType

        #region enum

        /// <summary>
        /// 获取enum执行方法
        /// </summary>
        /// <returns>MethodInfo.</returns>
        public static MethodInfo GetEnumParseMethod(Type sourceType)
        {
            MethodInfo method = null;
            if (definedWriteMethodDict.TryGetValue(sourceType, out method))
                return method;

            var methods = typeof(ParseMethodProviderEngrafting).GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var m in methods)
            {
                if (m.Name == "EnumParse" && m.IsGenericMethod)
                {
                    method = m.MakeGenericMethod(sourceType);
                    definedWriteMethodDict[sourceType] = method;
                    return method;
                }
            }

            return method;
        }

        /// <summary>
        /// 获取enum执行方法
        /// </summary>
        /// <returns>MethodInfo.</returns>
        public static MethodInfo GetNullableEnumParseMethod(Type sourceType, Type nullableSourceType)
        {
            MethodInfo method = null;
            if (definedWriteMethodDict.TryGetValue(sourceType, out method))
                return method;

            var methods = typeof(NullableParseMethodProviderEngrafting).GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var m in methods)
            {
                if (m.Name == "NullableEnumParse" && m.IsGenericMethod)
                {
                    method = m.MakeGenericMethod(nullableSourceType);
                    definedWriteMethodDict[sourceType] = method;
                    return method;
                }
            }

            return method;
        }

        /// <summary>
        /// 获取enum执行方法
        /// </summary>
        /// <returns>MethodInfo.</returns>
        public static MethodInfo GetEnumArrayParseMethod(Type sourceType, Type arraySourceType)
        {
            MethodInfo method = null;
            if (primitiveArrayDefinedWriteMethodDict.TryGetValue(arraySourceType, out method))
                return method;

            var methods = typeof(ParseMethodProviderEngrafting).GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var m in methods)
            {
                if (m.Name == "ArrayEnumParse" && m.IsGenericMethod)
                {
                    method = m.MakeGenericMethod(sourceType);
                    primitiveArrayDefinedWriteMethodDict[arraySourceType] = method;
                    return method;
                }
            }

            return method;
        }

        /// <summary>
        /// 获取enum执行方法
        /// </summary>
        /// <returns>MethodInfo.</returns>
        public static MethodInfo GetNullableEnumArrayParseMethod(Type sourceType, Type arraySourceType, Type nullableSourceType)
        {
            MethodInfo method = null;
            if (primitiveArrayDefinedWriteMethodDict.TryGetValue(arraySourceType, out method))
                return method;

            var methods = typeof(NullableParseMethodProviderEngrafting).GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var m in methods)
            {
                if (m.Name == "ArrayEnumParse" && m.IsGenericMethod)
                {
                    method = m.MakeGenericMethod(nullableSourceType);
                    primitiveArrayDefinedWriteMethodDict[arraySourceType] = method;
                    return method;
                }
            }

            return method;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long LongParseFromStringOnDeseralizing(string str)
        {
            long result = 0L;
            long.TryParse(str, out result);
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ulong UlongParseFromStringOnDeseralizing(string str)
        {
            ulong result = 0L;
            ulong.TryParse(str, out result);
            return result;
        }

        /// <summary>
        /// Strings the equality.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static bool StringEqualityOnDeseralizing(string a, string b)
        {
            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="long"></param>
        /// <returns></returns>
        public static T LongParseToEnumOnDeseralizing<T>(long @long) where T : struct
        {
            var value = default(T);
            Enum.TryParse(@long.ToString(), out value);
            return value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ulong"></param>
        /// <returns></returns>
        public static T UlongParseToEnumOnDeseralizing<T>(ulong @ulong) where T : struct
        {
            var value = default(T);
            Enum.TryParse(@ulong.ToString(), out value);
            return value;
        }

        #endregion enum

        #region exception

        /// <summary>
        /// 获取exception执行方法
        /// </summary>
        /// <returns>MethodInfo.</returns>
        public static MethodInfo GetExceptionParseMethod(Type sourceType)
        {
            MethodInfo method = null;
            if (definedWriteMethodDict.TryGetValue(sourceType, out method))
                return method;

            var methods = typeof(ParseMethodProviderEngrafting).GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var m in methods)
            {
                if (m.Name == "ExceptionParse" && m.IsGenericMethod)
                {
                    method = m.MakeGenericMethod(sourceType);
                    definedWriteMethodDict[sourceType] = method;
                    return method;
                }
            }

            return method;
        }

        #endregion exception

        #region complex

        /// <summary>
        /// 执行builder
        /// </summary>
        internal static T[] CallObjectArrayInvoke<T>(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return ObjectEnumerableProvider<T>.Default.Parse(reader, setting, name, arrayLevel);
        }

        internal static T CallObjectInvoke<T>(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            var node = reader.Read(name);
            if (node == null)
                return default(T);

            var nodeValue = node.GetValue();
            if (nodeValue != null && StringMethodProvider.Default.IsNullValue(nodeValue))
                return default(T);

            return ZzzZzDeserializerBuilder<T>.Register(setting).Invoke(reader.Parse(node), setting, arrayLevel);
        }

        internal static Nullable<T> CallNullableObjectInvoke<T>(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel) where T : struct
        {
            if (reader.Count == 0)
                return null;

            var node = reader.Read(name);
            if (node == null)
                return null;

            var nodeValue = node.GetValue();
            if (node != null && StringMethodProvider.Default.IsNullValue(nodeValue))
                return null;

            if (node != null && node.NodeType == ContentNodeType.Object)
                return ZzzZzDeserializerBuilder<T>.Register(setting).Invoke(reader.Parse(node), setting, arrayLevel);

            return ZzzZzDeserializerBuilder<T>.Register(setting).Invoke(reader, setting, arrayLevel);
        }

        #endregion complex

        #region copy

        /// <summary>
        /// 复制到List实例中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        internal static void CopyToICollection<T>(ICollection<T> target, T[] source)
        {
            for (var i = 0; i < source.Length; i++)
                target.Add(source[i]);
        }

        #endregion copy

        #region dictionary

        public static void LoadIntoDictionary(IDictionary target, IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            IDictionaryProvider.Default.Load(target, reader, setting, name, arrayLevel);
        }

        public static void LoadIntoGenericDictionary<T, V>(IDictionary<T, V> target, IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            IDictionaryProvider.Default.Load<T, V>(target, reader, setting, name, arrayLevel);
        }

        public static void LoadIntoStringKeyGenericDictionary<V>(IDictionary<string, V> target, IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            IDictionaryProvider.Default.LoadStringKey<V>(target, reader, setting, name, arrayLevel);
        }

        public static void LoadIntoStringKeyStringvalueDictionary(IDictionary<string, string> target, IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            IDictionaryProvider.Default.LoadStringKeyStringvalue(target, reader, setting, name, arrayLevel);
        }

        #endregion dictionary
    }
}