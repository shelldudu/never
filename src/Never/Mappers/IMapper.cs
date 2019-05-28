using System;

namespace Never.Mappers
{
    /// <summary>
    /// 自动映射
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <returns></returns>
        To Map<From, To>(From from);

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <param name="callBack">当完成映射后的回调</param>
        /// <returns></returns>
        To Map<From, To>(From from, Action<From, To> callBack);

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <param name="target">目标对象，请实例化该对象</param>
        /// <returns></returns>
        To Map<From, To>(From from, To target);

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <typeparam name="To">目标对象</typeparam>
        /// <typeparam name="From">来源对象</typeparam>
        /// <param name="from">来源对象</param>
        /// <param name="target">目标对象，请实例化该对象</param>
        /// <param name="callBack">当完成映射后的回调</param>
        /// <returns></returns>
        To Map<From, To>(From from, To target, Action<From, To> callBack);
    }
}