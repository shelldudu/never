using Never.Attributes;
using Never.EasySql.Linq;
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

            return new EasyDecoratedXmlDao<T>(dao, new KeyValueSqlParameter<T>(parameter));
        }

        /// <summary>
        /// 对idao进一步装饰
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dao"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static EasyDecoratedXmlDao<T> ToEasyXmlDao<T>(this IDao dao, T[] parameter)
        {
            if (parameter is INullableParameter)
                throw new ArgumentException("parameter类型不能是INullableParameter");

            return new EasyDecoratedXmlDao<T>(dao, new ArraySqlParameter<T>(parameter));
        }

        /// <summary>
        /// 对idao进一步装饰
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dao"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static EasyDecoratedXmlDao<T> ToEasyXmlDao<T>(this IDao dao, IEnumerable<T> parameter)
        {
            if (parameter is INullableParameter)
                throw new ArgumentException("parameter类型不能是INullableParameter");

            return new EasyDecoratedXmlDao<T>(dao, new ArraySqlParameter<T>(parameter));
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

            return new EasyDecoratedTextDao<T>(dao, new KeyValueSqlParameter<T>(parameter));
        }

        /// <summary>
        /// 对idao进一步装饰
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dao"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static EasyDecoratedTextDao<T> ToEasyTextDao<T>(this IDao dao, T[] parameter)
        {
            if (parameter is INullableParameter)
                throw new ArgumentException("parameter类型不能是INullableParameter");

            return new EasyDecoratedTextDao<T>(dao, new ArraySqlParameter<T>(parameter));
        }

        /// <summary>
        /// 对idao进一步装饰
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dao"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static EasyDecoratedTextDao<T> ToEasyTextDao<T>(this IDao dao, IEnumerable<T> parameter)
        {
            if (parameter is INullableParameter)
                throw new ArgumentException("parameter类型不能是INullableParameter");

            return new EasyDecoratedTextDao<T>(dao, new ArraySqlParameter<T>(parameter));
        }

        /// <summary>
        /// 对idao进一步装饰
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static EasyDecoratedLinqDao<T> ToEasyLinqDao<T>(this IDao dao, T parameter)
        {
            return new EasyDecoratedLinqDao<T>(dao, new KeyValueSqlParameter<T>(parameter));
        }

        /// <summary>
        /// 对idao进一步装饰
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dao"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static EasyDecoratedLinqDao<T> ToEasyLinqDao<T>(this IDao dao, T[] parameter)
        {
            if (parameter is INullableParameter)
                throw new ArgumentException("parameter类型不能是INullableParameter");

            return new EasyDecoratedLinqDao<T>(dao, new ArraySqlParameter<T>(parameter));
        }

        /// <summary>
        /// 对idao进一步装饰
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dao"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static EasyDecoratedLinqDao<T> ToEasyLinqDao<T>(this IDao dao, IEnumerable<T> parameter)
        {
            if (parameter is INullableParameter)
                throw new ArgumentException("parameter类型不能是INullableParameter");

            return new EasyDecoratedLinqDao<T>(dao, new ArraySqlParameter<T>(parameter));
        }

        /// <summary>
        /// 转换为可空类型参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StructNullableParameter<T> ToNullableParameter<T>(this T? value) where T : struct
        {
            return new StructNullableParameter<T>(value);
        }

        /// <summary>
        /// 转换为可空类型参数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringNullableParameter ToNullableParameter(this string value)
        {
            return new StringNullableParameter(value);
        }

        /// <summary>
        /// 转换为可空类型参数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static GuidNullableParameter ToNullableParameter(this Guid value)
        {
            return new GuidNullableParameter(value);
        }

        /// <summary>
        /// 转换为可空类型参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static EnumerableNullableParameter<T> ToNullableParameter<T>(this IEnumerable<T> value) where T : IConvertible
        {
            return new EnumerableNullableParameter<T>(value);
        }

        /// <summary>
        /// 转换为可空类型参数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static EnumerableNullableParameter<Guid> ToNullableParameter(this IEnumerable<Guid> value)
        {
            return new EnumerableNullableParameter<Guid>(value);
        }

        /// <summary>
        /// 转换为可空类型参数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static EnumerableNullableParameter<string> ToNullableParameter(this IEnumerable<string> value)
        {
            return new EnumerableNullableParameter<string>(value);
        }

        /// <summary>
        /// In
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [Summary(Descn = "linq extension method")]
        public static bool In<T>(this T target, EnumerableNullableParameter<T> value) where T : struct, IConvertible
        {
            return false;
        }

        /// <summary>
        /// In
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [Summary(Descn = "linq extension method")]
        public static bool In(this string target, EnumerableNullableParameter<string> value)
        {
            return false;
        }

        /// <summary>
        /// In
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [Summary(Descn = "linq extension method")]
        public static bool In(this Guid target, EnumerableNullableParameter<Guid> value)
        {
            return false;
        }

        /// <summary>
        /// like
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [Summary(Descn = "linq extension method")]
        public static bool Like(this string value, string target)
        {
            return false;
        }

        /// <summary>
        /// like
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [Summary(Descn = "linq extension method")]
        public static bool LeftLike(this string value, string target)
        {
            return false;
        }

        /// <summary>
        /// like
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [Summary(Descn = "linq extension method")]
        public static bool RightLike(this string value, string target)
        {
            return false;
        }
    }
}