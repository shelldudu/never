﻿using System;
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
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <returns></returns>
        private static string _GenericToString<T>(T @object) where T : IConvertible
        {
            return @object.ToString();
        }

        #endregion string to other

        #region enumerable

        public static IEnumerable<T> ConvertToEnumerable<T, F>(IEnumerable<F> source)
        {
            if (source.IsNullOrEmpty())
                yield return default(T);

            foreach (var s in source)
                yield return EasyMapper.Map<F, T>(s);
        }

        public static IList<T> LoadIntoList<T, F>(IList<T> target, IEnumerable<F> source)
        {
            if (source.IsNullOrEmpty())
                return target;

            var list = new List<T>(source.Count());
            foreach (var s in source)
            {
                list.Add(EasyMapper.Map<F, T>(s));
            }

            return list;
        }

        public static ICollection<T> LoadIntoCollection<T, F>(ICollection<T> target, IEnumerable<F> source)
        {
            if (source.IsNullOrEmpty())
                return target;

            var list = new List<T>(source.Count());
            foreach (var s in source)
            {
                list.Add(EasyMapper.Map<F, T>(s));
            }

            return list;
        }

        public static IDictionary<TKey, TValue> ConvertToDictionary<TKey, TValue, FKey, FValue>(IEnumerable<KeyValuePair<FKey, FValue>> source)
        {
            if (source.IsNullOrEmpty())
                return new Dictionary<TKey, TValue>();

            var dictionary = new Dictionary<TKey, TValue>(source.Count());
            foreach (var s in source)
            {
                var key = EasyMapper.Map<FKey, TKey>(s.Key);
                var value = EasyMapper.Map<FValue, TValue>(s.Value);
                dictionary[key] = value;
            }

            return dictionary;
        }

        public static IDictionary<TKey, TValue> LoadIntoDictionary<TKey, TValue, FKey, FValue>(IDictionary<TKey, TValue> target, IEnumerable<KeyValuePair<FKey, FValue>> source)
        {
            if (source.IsNullOrEmpty())
                return target;

            var dictionary = new Dictionary<TKey, TValue>(source.Count());
            foreach (var s in source)
            {
                var key = EasyMapper.Map<FKey, TKey>(s.Key);
                var value = EasyMapper.Map<FValue, TValue>(s.Value);
                dictionary[key] = value;
            }

            return target = dictionary;
        }

        public static ICollection<KeyValuePair<TKey, TValue>> ConvertToCollection<TKey, TValue, FKey, FValue>(IEnumerable<KeyValuePair<FKey, FValue>> source)
        {
            if (source.IsNullOrEmpty())
                return new Dictionary<TKey, TValue>();

            var dictionary = new Dictionary<TKey, TValue>(source.Count());
            foreach (var s in source)
            {
                var key = EasyMapper.Map<FKey, TKey>(s.Key);
                var value = EasyMapper.Map<FValue, TValue>(s.Value);
                dictionary[key] = value;
            }

            return dictionary;
        }

        public static ICollection<KeyValuePair<TKey, TValue>> LoadIntoCollection<TKey, TValue, FKey, FValue>(ICollection<KeyValuePair<TKey, TValue>> target, IEnumerable<KeyValuePair<FKey, FValue>> source)
        {
            if (source.IsNullOrEmpty())
                return target;

            var dictionary = new Dictionary<TKey, TValue>(source.Count());
            foreach (var s in source)
            {
                var key = EasyMapper.Map<FKey, TKey>(s.Key);
                var value = EasyMapper.Map<FValue, TValue>(s.Value);
                dictionary[key] = value;
            }

            return target = dictionary;
        }

        #endregion enumerable

        #region nullable

        public static T ConvertToValueFromNullableValue<T, F>(F? value) where T : struct, IConvertible where F : struct, IConvertible
        {
            if (value.HasValue == false)
                return default(T);

            return EasyMapper.Map<F, T>(value.Value);
        }

        public static T? ConvertToNullableValueFromNullableValue<T, F>(F? value) where T : struct, IConvertible where F : struct, IConvertible
        {
            if (value.HasValue == false)
                return default(T);

            return EasyMapper.Map<F, T>(value.Value);
        }

        public static T? ConvertToNullableValueFromValue<T, F>(F value) where T : struct, IConvertible where F : struct, IConvertible
        {
            return new T?(EasyMapper.Map<F, T>(value));
        }

        #endregion nullable
    }
}