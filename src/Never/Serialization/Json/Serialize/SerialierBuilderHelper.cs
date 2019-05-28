using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Never.Serialization.Json.Serialize
{
    /// <summary>
    /// 助手
    /// </summary>
    public class SerialierBuilderHelper
    {
        #region field

        /// <summary>
        /// 常见的转换方法
        /// </summary>
        private readonly static IDictionary<Type, MethodInfo> definedWriteMethodDict = null;

        /// <summary>
        ///
        /// </summary>
        private readonly static IDictionary<Type, Action<ISerializerWriter, JsonSerializeSetting, object>> builderDelegateDict = null;

        /// <summary>
        /// 常见的类型
        /// </summary>
        private readonly static IList<Type> primitiveTypeAndNullablePritiveTypeDict = null;

        /// <summary>
        /// 对数组常见的转换方法
        /// </summary>
        private readonly static IDictionary<Type, MethodInfo> primitiveArrayDefinedWriteMethodDict = null;

        /// <summary>
        /// 使用object去序列化时使用的字典
        /// </summary>
        private readonly static Hashtable builderTable = null;

        /// <summary>
        ///使用object去序列化时使用的字方法
        /// </summary>
        private readonly static MethodInfo builderInvoker = null;

        /// <summary>
        ///使用exception去序列化时使用的字方法
        /// </summary>
        private readonly static MethodInfo exBuilderInvoker = null;

        /// <summary>
        ///使用enum去序列化时使用的字方法
        /// </summary>
        private readonly static MethodInfo enumBuilderInvoker = null;

        #endregion field

        #region ctor

        static SerialierBuilderHelper()
        {
            definedWriteMethodDict = new Dictionary<Type, MethodInfo>(15);
            builderDelegateDict = new Dictionary<Type, Action<ISerializerWriter, JsonSerializeSetting, object>>(200);
            builderTable = new Hashtable(500);
            var builderInvokers = typeof(SerialierBuilderHelper).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
            foreach (var invoker in builderInvokers)
            {
                if (invoker.Name == "BuilderInvoke" && invoker.IsGenericMethod && invoker.GetParameters().Length == 4)
                {
                    builderInvoker = invoker;
                    continue;
                }
                if (invoker.Name == "ExceptionBuilderInvoke" && invoker.IsGenericMethod && invoker.GetParameters().Length == 4)
                {
                    exBuilderInvoker = invoker;
                    continue;
                }
                if (invoker.Name == "EnumBuilderInvoke" && invoker.IsGenericMethod && invoker.GetParameters().Length == 4)
                {
                    enumBuilderInvoker = invoker;
                    continue;
                }
            }

            var methodProvideEngrafting = typeof(ParseMethodProviderEngrafting);
            definedWriteMethodDict[typeof(bool)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(bool) });
            definedWriteMethodDict[typeof(byte)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(byte) });
            definedWriteMethodDict[typeof(char)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(char) });
            definedWriteMethodDict[typeof(DateTime)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(DateTime) });
            definedWriteMethodDict[typeof(decimal)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(decimal) });
            definedWriteMethodDict[typeof(double)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(double) });
            definedWriteMethodDict[typeof(float)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(float) });
            definedWriteMethodDict[typeof(Guid)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(Guid) });
            definedWriteMethodDict[typeof(short)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(short) }); ;
            definedWriteMethodDict[typeof(int)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(int) });
            definedWriteMethodDict[typeof(long)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(long) });
            definedWriteMethodDict[typeof(string)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(string) });
            definedWriteMethodDict[typeof(ushort)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(ushort) });
            definedWriteMethodDict[typeof(uint)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(uint) });
            definedWriteMethodDict[typeof(ulong)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(ulong) });
            definedWriteMethodDict[typeof(Type)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(Type) });
            definedWriteMethodDict[typeof(TimeSpan)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(TimeSpan) });

            var nullableMethodProvideEngrafting = typeof(NullableParseMethodProviderEngrafting);
            definedWriteMethodDict[typeof(bool?)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(bool?) });
            definedWriteMethodDict[typeof(byte?)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(bool?) });
            definedWriteMethodDict[typeof(char?)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(bool?) });
            definedWriteMethodDict[typeof(DateTime?)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(DateTime?) });
            definedWriteMethodDict[typeof(decimal?)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(decimal?) });
            definedWriteMethodDict[typeof(double?)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(double?) });
            definedWriteMethodDict[typeof(float?)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(float?) });
            definedWriteMethodDict[typeof(Guid?)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(Guid?) });
            definedWriteMethodDict[typeof(short?)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(short?) }); ;
            definedWriteMethodDict[typeof(int?)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(int?) });
            definedWriteMethodDict[typeof(long?)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(long?) });
            definedWriteMethodDict[typeof(ushort?)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(ushort?) });
            definedWriteMethodDict[typeof(uint?)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(uint?) });
            definedWriteMethodDict[typeof(ulong?)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(ulong?) });
            definedWriteMethodDict[typeof(TimeSpan?)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(TimeSpan?) });

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
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<bool>)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<bool>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<byte>)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<byte>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<char>)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<char>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<DateTime>)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<DateTime>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<decimal>)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<decimal>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<double>)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<double>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<float>)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<float>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<Guid>)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<Guid>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<short>)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<short>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<int>)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<int>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<long>)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<long>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<string>)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<string>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<ushort>)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<ushort>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<uint>)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<uint>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<ulong>)] = methodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<ulong>) });

            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<bool?>)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<bool?>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<byte?>)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<byte?>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<char?>)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<char?>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<DateTime?>)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<DateTime?>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<decimal?>)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<decimal?>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<double?>)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<double?>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<float?>)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<float?>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<Guid?>)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<Guid?>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<short?>)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<short?>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<int?>)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<int?>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<long?>)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<long?>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<ushort?>)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<ushort?>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<uint?>)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<uint?>) });
            primitiveArrayDefinedWriteMethodDict[typeof(IEnumerable<ulong?>)] = nullableMethodProvideEngrafting.GetMethod("Write", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<ulong?>) });
        }

        #endregion ctor

        #region getmethod

        /// <summary>
        /// 获取写入流内容方法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MethodInfo GetWriteMethod(Type type)
        {
            MethodInfo method = null;
            definedWriteMethodDict.TryGetValue(type, out method);
            return method;
        }

        /// <summary>
        /// 获取string类型的写入流内容方法
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetStringWriteMethod()
        {
            return definedWriteMethodDict[typeof(string)];
        }

        /// <summary>
        /// 获取时间类型的写入流内容方法
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetDateTimeWriteMethod()
        {
            return definedWriteMethodDict[typeof(DateTime)];
        }

        /// <summary>
        /// 获取时间类型的写入流内容方法
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetNullableDateTimeWriteMethod()
        {
            return definedWriteMethodDict[typeof(DateTime?)];
        }

        /// <summary>
        /// 获取基元类型写入流内容方法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MethodInfo GetPrimitiveArrayWriteMethod(Type type)
        {
            MethodInfo method = null;
            primitiveArrayDefinedWriteMethodDict.TryGetValue(type, out method);
            return method;
        }

        #endregion getmethod

        #region invoke build

        /// <summary>
        /// 执行构建序列化,反射用到的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer"></param>
        /// <param name="setting"></param>
        /// <param name="level"></param>
        /// <param name="object"></param>
        /// <returns></returns>
        private static void BuilderInvoke<T>(ISerializerWriter writer, JsonSerializeSetting setting, object @object, byte level)
        {
            ZzzZzSerialierBuilder<T>.Register(setting).Invoke(writer, setting, (T)@object, level);
        }

        /// <summary>
        /// 查询执行者
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <returns>返回可执行</returns>
        public static Action<ISerializerWriter, JsonSerializeSetting, object, byte> QueryBuilderInvoker(Type sourceType)
        {
            if (sourceType == typeof(object))
                return (x, y, z, l) => x.Write("{}");

            var @delegate = builderTable[sourceType] as Action<ISerializerWriter, JsonSerializeSetting, object, byte>;
            if (@delegate == null)
                builderTable[sourceType] = @delegate = (Action<ISerializerWriter, JsonSerializeSetting, object, byte>)Delegate.CreateDelegate(typeof(Action<ISerializerWriter, JsonSerializeSetting, object, byte>), builderInvoker.MakeGenericMethod(sourceType));

            return @delegate;
        }

        /// <summary>
        /// 执行builder,反射用到的
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="level"></param>
        /// <param name="source">The source.</param>
        internal static void CallObjectBuilderInvoke(ISerializerWriter writer, JsonSerializeSetting setting, object source, byte level)
        {
            if (source == null)
            {
                writer.Write("null");
                return;
            }

            QueryBuilderInvoker(source.GetType()).Invoke(writer, setting, source, level++);
        }

        /// <summary>
        /// 执行builder,反射用到的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="level"></param>
        /// <param name="source">The source.</param>
        internal static void CallBuilderInvoke<T>(ISerializerWriter writer, JsonSerializeSetting setting, T source, byte level)
        {
            var @delegate = ZzzZzSerialierBuilder<T>.Register(setting);
            @delegate(writer, setting, source, level++);
        }

        /// <summary>
        /// 执行builder,反射用到的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="level"></param>
        /// <param name="source">The source.</param>
        internal static void CallCustomeBuilderInvoke<T>(ISerializerWriter writer, JsonSerializeSetting setting, T source, byte level)
        {
            var @delegate = CustomSerializationProvider.QueryCustomeSerilizerbuilder<T>().Build(setting);
            @delegate(writer, setting, source, level++);
        }

        /// <summary>
        /// 执行builder,反射用到的
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="level"></param>
        /// <param name="source">The source.</param>
        internal static void CallNullablePrimitiveBuilderInvoke<T>(ISerializerWriter writer, JsonSerializeSetting setting, Nullable<T> source, byte level) where T : struct
        {
            if (!source.HasValue)
            {
                writer.Write("null");
                return;
            }

            ZzzZzSerialierBuilder<T>.Register(setting).Invoke(writer, setting, source.Value, level);
        }

        /// <summary>
        /// 比较深度
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="source"></param>
        /// <param name="writer"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        internal static bool CallSerializeMaxDepthCompare<T>(ISerializerWriter writer, JsonSerializeSetting setting, T source, byte level)
        {
            if (setting.MaxDepth == 0)
                return level <= 100;

            return setting.MaxDepth > level;
        }

        #endregion invoke build

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
                if (m.Name == "EnumWrite" && m.IsGenericMethod)
                {
                    method = m.MakeGenericMethod(sourceType);
                    break;
                }
            }
            definedWriteMethodDict[sourceType] = method;
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
                if (m.Name == "EnumWrite" && m.IsGenericMethod)
                {
                    method = m.MakeGenericMethod(nullableSourceType);
                    break;
                }
            }

            definedWriteMethodDict[sourceType] = method;
            return method;
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
                if (m.Name == "ExceptionWrite" && m.IsGenericMethod)
                {
                    method = m.MakeGenericMethod(sourceType);
                    break;
                }
            }

            definedWriteMethodDict[sourceType] = method;
            return method;
        }

        #endregion exception
    }
}