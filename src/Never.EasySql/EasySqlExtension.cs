using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 
    /// </summary>
    public static class EasySqlExtension
    {
        /// <summary>
        /// 对idao进一步装饰
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dao"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static EasyDecoratedXmlDao<T> ToEasyXmlDao<T>(this IDao dao, T parameter)
        {
            if (parameter is INullableParameter)
                throw new ArgumentException("parameter类型不能是INullableParameter");

            return new EasyDecoratedXmlDao<T>(dao, new KeyValueEasySqlParameter<T>(parameter));
        }

        /// <summary>
        /// 对idao进一步装饰
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dao"></param>
        /// <param name="table"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static EasyDecoratedXmlDao<T> ToEasyXmlDao<T, V>(this IDao dao, T table, IEnumerable<V> parameter)
        {
            if (table is INullableParameter)
                throw new ArgumentException("table类型不能是INullableParameter");

            if (parameter is INullableParameter)
                throw new ArgumentException("parameter类型不能是INullableParameter");

            return new EasyDecoratedXmlDao<T>(dao, new ArrayEasySqlParameter<T, V>(table, parameter));
        }

        /// <summary>
        /// 对idao进一步装饰
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dao"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static EasyDecoratedTextDao<T> ToEasyTextDao<T>(this IDao dao, T parameter)
        {
            if (parameter is INullableParameter)
                throw new ArgumentException("parameter类型不能是INullableParameter");

            return new EasyDecoratedTextDao<T>(dao, new KeyValueEasySqlParameter<T>(parameter));
        }

        /// <summary>
        /// 对idao进一步装饰
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dao"></param>
        /// <param name="table"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static EasyDecoratedTextDao<T> ToEasyTextDao<T, V>(this IDao dao, T table, IEnumerable<V> parameter)
        {
            if (table is INullableParameter)
                throw new ArgumentException("table类型不能是INullableParameter");

            if (parameter is INullableParameter)
                throw new ArgumentException("parameter类型不能是INullableParameter");

            return new EasyDecoratedTextDao<T>(dao, new ArrayEasySqlParameter<T, V>(table, parameter));
        }

        /// <summary>
        /// 转换为可空类型参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static INullableParameter ToNullableParameter<T>(this T? value) where T : struct
        {
            return new StructNullableParameter<T>(value);
        }

        /// <summary>
        /// 转换为可空类型参数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static INullableParameter ToNullableParameter(this string value)
        {
            return new StringNullableParameter(value);
        }

        /// <summary>
        /// 转换为可空类型参数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static INullableParameter ToNullableParameter(this Guid value)
        {
            return new GuidNullableParameter(value);
        }

        /// <summary>
        /// 转换为可空类型参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static INullableParameter ToNullableParameter<T>(this IEnumerable<T> value) where T : IConvertible
        {
            return new EnumerableNullableParameter<T>(value);
        }

        /// <summary>
        /// 转换为可空类型参数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static INullableParameter ToNullableParameter(this IEnumerable<Guid> value)
        {
            return new EnumerableNullableParameter<Guid>(value);
        }
    }
}