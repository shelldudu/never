using System;
using System.Collections.Generic;

namespace Never.Serialization.Json
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public static class EnumValues<TEnum> where TEnum : struct, IConvertible
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly static IDictionary<string, TEnum> nameValues = null;

        /// <summary>
        ///
        /// </summary>
        private readonly static IDictionary<long, KeyValuePair<string, TEnum>> valueValues = null;

        /// <summary>
        ///
        /// </summary>
        private readonly static IDictionary<ulong, KeyValuePair<string, TEnum>> uvalueValues = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        static EnumValues()
        {
            var enumType = typeof(TEnum);
            var ns = Enum.GetNames(enumType);
            var vs = Enum.GetValues(enumType);

            nameValues = new Dictionary<string, TEnum>(vs.Length);
            valueValues = new Dictionary<long, KeyValuePair<string, TEnum>>(vs.Length);
            uvalueValues = new Dictionary<ulong, KeyValuePair<string, TEnum>>(vs.Length);

            if (Enum.GetUnderlyingType(enumType) == typeof(ulong))
            {
                for (var i = 0; i < ns.Length; i++)
                {
                    var v = (TEnum)vs.GetValue(i);
                    nameValues[ns[i]] = v;
                    uvalueValues[v.ToUInt64(System.Globalization.NumberFormatInfo.InvariantInfo)] = new KeyValuePair<string, TEnum>(ns[i], v);
                }
            }
            else
            {
                for (var i = 0; i < ns.Length; i++)
                {
                    var v = (TEnum)vs.GetValue(i);
                    nameValues[ns[i]] = v;
                    valueValues[v.ToInt64(System.Globalization.NumberFormatInfo.InvariantInfo)] = new KeyValuePair<string, TEnum>(ns[i], v);
                }
            }
        }

        #endregion ctor

        #region

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string value, out TEnum result)
        {
            return nameValues.TryGetValue(value, out result);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(long value, out KeyValuePair<string, TEnum> result)
        {
            return valueValues.TryGetValue(value, out result);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(ulong value, out KeyValuePair<string, TEnum> result)
        {
            return uvalueValues.TryGetValue(value, out result);
        }

        #endregion

        #region todict

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, TEnum> ToStringDictionary()
        {
            var dict = new Dictionary<string, TEnum>(nameValues.Count);
            foreach (var i in nameValues)
            {
                dict[i.Key] = i.Value;
            }
            return dict;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static IDictionary<long, KeyValuePair<string, TEnum>> ToLongDictionary()
        {
            var dict = new Dictionary<long, KeyValuePair<string, TEnum>>(nameValues.Count);
            foreach (var i in valueValues)
            {
                dict[i.Key] = i.Value;
            }
            return dict;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static IDictionary<ulong, KeyValuePair<string, TEnum>> ToULongDictionary()
        {
            var dict = new Dictionary<ulong, KeyValuePair<string, TEnum>>(nameValues.Count);
            foreach (var i in uvalueValues)
            {
                dict[i.Key] = i.Value;
            }
            return dict;
        }

        #endregion
    }
}