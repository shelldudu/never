using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Never.Mappers
{
    /// <summary>
    /// 映射助手
    /// </summary>
    internal static class MapperBuilderHelper
    {
        #region field

        /// <summary>
        /// 常见的转换方法
        /// </summary>
        private readonly static IDictionary<Type, IDictionary<Type, MethodInfo>> definedMethodDict = null;

        /// <summary>
        /// 常见的类型
        /// </summary>
        private readonly static IList<Type> definedTypeDict = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes the <see cref="MapperBuilderHelper"/> class.
        /// </summary>
        static MapperBuilderHelper()
        {
            definedTypeDict = new List<Type>();
            definedTypeDict.Add(typeof(short));
            definedTypeDict.Add(typeof(int));
            definedTypeDict.Add(typeof(long));
            definedTypeDict.Add(typeof(string));
            definedTypeDict.Add(typeof(float));
            definedTypeDict.Add(typeof(double));
            definedTypeDict.Add(typeof(DateTime));
            definedTypeDict.Add(typeof(char));
            definedTypeDict.Add(typeof(bool));
            definedTypeDict.Add(typeof(byte));
            definedTypeDict.Add(typeof(decimal));
            definedTypeDict.Add(typeof(Guid));
            definedTypeDict.Add(typeof(ushort));
            definedTypeDict.Add(typeof(uint));
            definedTypeDict.Add(typeof(ulong));

            definedMethodDict = new Dictionary<Type, IDictionary<Type, MethodInfo>>(12);

            /*所有的转换方法*/
            var methods = typeof(Convert).GetMethods(BindingFlags.Public | BindingFlags.Static);
            /*int*/
            var a = new Dictionary<Type, MethodInfo>();
            definedMethodDict[typeof(int)] = new Dictionary<Type, MethodInfo>(12);
            definedMethodDict[typeof(long)] = new Dictionary<Type, MethodInfo>(12);
            definedMethodDict[typeof(sbyte)] = new Dictionary<Type, MethodInfo>(12);
            definedMethodDict[typeof(float)] = new Dictionary<Type, MethodInfo>(12);
            definedMethodDict[typeof(string)] = new Dictionary<Type, MethodInfo>(12);
            definedMethodDict[typeof(ushort)] = new Dictionary<Type, MethodInfo>(12);
            definedMethodDict[typeof(bool)] = new Dictionary<Type, MethodInfo>(12);
            definedMethodDict[typeof(byte)] = new Dictionary<Type, MethodInfo>(12);
            definedMethodDict[typeof(char)] = new Dictionary<Type, MethodInfo>(12);
            definedMethodDict[typeof(DateTime)] = new Dictionary<Type, MethodInfo>(12);
            definedMethodDict[typeof(decimal)] = new Dictionary<Type, MethodInfo>(12);
            definedMethodDict[typeof(double)] = new Dictionary<Type, MethodInfo>(12);
            definedMethodDict[typeof(short)] = new Dictionary<Type, MethodInfo>(12);
            definedMethodDict[typeof(uint)] = new Dictionary<Type, MethodInfo>(12);
            definedMethodDict[typeof(ulong)] = new Dictionary<Type, MethodInfo>(12);

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                if (parameters == null || parameters.Length != 1)
                    continue;

                switch (method.Name)
                {
                    case "ToInt32":
                        {
                            definedMethodDict[typeof(int)].Add(parameters[0].ParameterType, method);
                        }
                        break;

                    case "ToInt64":
                        {
                            definedMethodDict[typeof(long)].Add(parameters[0].ParameterType, method);
                        }
                        break;

                    case "ToSByte":
                        {
                            definedMethodDict[typeof(sbyte)].Add(parameters[0].ParameterType, method);
                        }
                        break;

                    case "ToSingle":
                        {
                            definedMethodDict[typeof(float)].Add(parameters[0].ParameterType, method);
                        }
                        break;

                    case "ToString":
                        {
                            definedMethodDict[typeof(string)].Add(parameters[0].ParameterType, method);
                        }
                        break;

                    case "ToUInt16":
                        {
                            definedMethodDict[typeof(ushort)].Add(parameters[0].ParameterType, method);
                        }
                        break;

                    case "ToByte":
                        {
                            definedMethodDict[typeof(bool)].Add(parameters[0].ParameterType, method);
                        }
                        break;

                    case "ToChar":
                        {
                            definedMethodDict[typeof(char)].Add(parameters[0].ParameterType, method);
                        }
                        break;

                    case "ToDateTime":
                        {
                            definedMethodDict[typeof(DateTime)].Add(parameters[0].ParameterType, method);
                        }
                        break;

                    case "ToDecimal":
                        {
                            definedMethodDict[typeof(decimal)].Add(parameters[0].ParameterType, method);
                        }
                        break;

                    case "ToDouble":
                        {
                            definedMethodDict[typeof(double)].Add(parameters[0].ParameterType, method);
                        }
                        break;

                    case "ToInt16":
                        {
                            definedMethodDict[typeof(short)].Add(parameters[0].ParameterType, method);
                        }
                        break;

                    case "ToUInt32":
                        {
                            definedMethodDict[typeof(uint)].Add(parameters[0].ParameterType, method);
                        }
                        break;

                    case "ToUInt64":
                        {
                            definedMethodDict[typeof(ulong)].Add(parameters[0].ParameterType, method);
                        }
                        break;
                }
            }

            definedMethodDict[typeof(string)].Add(typeof(Guid), typeof(MapperBuilderHelper).GetMethod("GuidToString"));
        }

        #endregion ctor

        #region containType

        /// <summary>
        /// 是否包含类型
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static bool ContainType(Type targetType)
        {
            return definedTypeDict.Contains(targetType);
        }

        #endregion containType

        #region getmethod

        public static string GuidToString(Guid guid)
        {
            return guid.ToString();
        }

        public static MethodInfo GetConvertMethod(Type toType, Type fromType)
        {
            return definedMethodDict[toType][fromType];
        }

        public static bool TryGetConvertMethod(Type toType, Type fromType, out MethodInfo method)
        {
            method = null;
            if (definedMethodDict.ContainsKey(toType) == false)
                return false;

            var current = definedMethodDict[toType];
            if (current.ContainsKey(fromType) == false)
                return false;

            method = current[fromType];
            return true;
        }

        #endregion getmethod

        #region string to other

        /// <summary>
        /// 枚举ToString，反射用到的，不能删
        /// </summary>
        private static string _GenericToString<T>(T @object, MapperContext context) where T : IConvertible
        {
            return @object.ToString();
        }

        #endregion string to other

        #region enumerable

        /// <summary>
        /// 确定条数，反射用到的，不能删
        /// </summary>
        public static int MakeSureEnumerableCount<F>(IEnumerable<F> source, MapperContext context)
        {
            return source == null ? 0 : source.Count();
        }

        /// <summary>
        /// 反射用到的，不能删
        /// </summary>
        public static void LoadIntoArray<T, F>(T[] target, IEnumerable<F> source, MapperContext context)
        {
            if (source.IsNullOrEmpty() || target.IsNullOrEmpty())
                return;

            for (int i = 0; i < source.Count(); i++)
            {
                if (i < target.Length)
                    target[i] = EasyMapper.Map<F, T>(source.ElementAt(i), null, context);
            }
        }

        /// <summary>
        /// 反射用到的，不能删
        /// </summary>
        public static void LoadIntoEnumerable<T, F>(IEnumerable<T> target, IEnumerable<F> source, MapperContext context)
        {
            if (source.IsNullOrEmpty())
                return;

            foreach (var s in source)
                target = target.Union(new[] { EasyMapper.Map<F, T>(s, null, context) });
        }

        /// <summary>
        /// 反射用到的，不能删
        /// </summary>
        public static void LoadIntoList<T, F>(IList<T> target, IEnumerable<F> source, MapperContext context)
        {
            if (source.IsNullOrEmpty())
                return;

            foreach (var s in source)
            {
                target.Add(EasyMapper.Map<F, T>(s, null, context));
            }

            return;
        }

        /// <summary>
        /// 反射用到的，不能删
        /// </summary>
        public static void LoadIntoCollection<T, F>(ICollection<T> target, IEnumerable<F> source, MapperContext context)
        {
            if (source.IsNullOrEmpty())
                return;

            foreach (var s in source)
            {
                target.Add(EasyMapper.Map<F, T>(s, null, context));
            }

            return;
        }

        /// <summary>
        /// 反射用到的，不能删
        /// </summary>
        public static void KeyValuePairLoadIntoDictionary<TKey, TValue, FKey, FValue>(IDictionary<TKey, TValue> target, IEnumerable<KeyValuePair<FKey, FValue>> source, MapperContext context)
        {
            if (source.IsNullOrEmpty())
                return;

            foreach (var s in source)
            {
                var key = EasyMapper.Map<FKey, TKey>(s.Key, null, context);
                var value = EasyMapper.Map<FValue, TValue>(s.Value, null, context);
                target[key] = value;
            }

            return;
        }

        /// <summary>
        /// 反射用到的，不能删
        /// </summary>
        public static void KeyValuePairLoadIntoCollection<TKey, TValue, FKey, FValue>(ICollection<KeyValuePair<TKey, TValue>> target, IEnumerable<KeyValuePair<FKey, FValue>> source, MapperContext context)
        {
            if (source.IsNullOrEmpty())
                return;

            foreach (var s in source)
            {
                var key = EasyMapper.Map<FKey, TKey>(s.Key, null, context);
                var value = EasyMapper.Map<FValue, TValue>(s.Value, null, context);
                target.Add(new KeyValuePair<TKey, TValue>(key, value));
            }

            return;
        }

        /// <summary>
        /// 反射用到的，不能删
        /// </summary>
        public static T ConvertToValueFromNullableValue<T, F>(F? value, MapperContext context) where T : struct, IConvertible where F : struct, IConvertible
        {
            if (value.HasValue == false)
                return default(T);

            return EasyMapper.Map<F, T>(value.Value, null, context);
        }

        /// <summary>
        /// 反射用到的，不能删
        /// </summary>
        public static T? ConvertToNullableValueFromNullableValue<T, F>(F? value, MapperContext context) where T : struct, IConvertible where F : struct, IConvertible
        {
            if (value.HasValue == false)
                return default(T);

            return EasyMapper.Map<F, T>(value.Value);
        }

        /// <summary>
        /// 反射用到的，不能删
        /// </summary>
        public static T? ConvertToNullableValueFromValue<T, F>(F value, MapperContext context) where T : struct, IConvertible where F : struct, IConvertible
        {
            return new T?(EasyMapper.Map<F, T>(value, null, context));
        }

        #endregion enumerable
    }
}