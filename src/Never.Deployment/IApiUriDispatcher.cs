using Never.Serialization;
using System;
using System.Collections.Generic;

namespace Never.Deployment
{
    /// <summary>
    /// ApiUri转发
    /// </summary>
    public interface IApiUriDispatcher
    {
        /// <summary>
        /// 获取routeKey的ascuill总值
        /// </summary>
        /// <param name="routeKey">路由Key</param>
        /// <returns></returns>
        int SumASCII(string routeKey);

        /// <summary>
        /// 获取routeKey的ascuill总值
        /// </summary>
        /// <param name="routeKey">路由Key</param>
        /// <returns></returns>
        int SumASCII(long routeKey);

        /// <summary>
        /// 获取routeKey的ascuill总值
        /// </summary>
        /// <param name="uniqueId">路由Key</param>
        /// <returns></returns>
        int SumASCII(Guid uniqueId);

        /// <summary>
        /// 获取routeKey的ascuill总值
        /// </summary>
        /// <typeparam name="TKey">路由类型</typeparam>
        /// <param name="routeKey">路由Key</param>
        /// <returns></returns>
        int SumASCII<TKey>(TKey routeKey);

        /// <summary>
        /// 获取Url，如果该组的资源在脱机状态下，则将该次索引指向下一组资源中
        /// </summary>
        /// <param name="routePrimaryKey">api路由主键</param>
        /// <returns></returns>
        string GetCurrentUrlHost(string routePrimaryKey);

        /// <summary>
        /// 获取Url，如果该组的资源在脱机状态下，则将该次索引指向下一组资源中
        /// </summary>
        /// <param name="routePrimaryKey">api路由主键</param>
        /// <returns></returns>
        string GetCurrentUrlHost(IRoutePrimaryKeySelect routePrimaryKey);

        /// <summary>
        /// 合成url
        /// </summary>
        /// <param name="routePrimaryKey"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        UrlConcat ConcatApiUrl(string routePrimaryKey, string route);

        /// <summary>
        /// 合成url
        /// </summary>
        /// <param name="request"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        UrlConcat ConcatApiUrl(IRoutePrimaryKeySelect request, string route);

        /// <summary>
        /// 以Post方式获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="request">参数请求</param>
        /// <param name="jsonData">json请求内容</param>
        /// <param name="route">路由标识</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        T Post<T>(IJsonSerializer jsonSerializer, IRoutePrimaryKeySelect request, string route, string jsonData, IDictionary<string, string> headerParams, int timeout = 30000);

        /// <summary>
        /// 以Post方式发送请求
        /// </summary>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="request">参数请求</param>
        /// <param name="jsonData">json请求内容</param>
        /// <param name="route">路由标识</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        void Post(IJsonSerializer jsonSerializer, IRoutePrimaryKeySelect request, string route, string jsonData, IDictionary<string, string> headerParams, int timeout = 30000);

        /// <summary>
        /// 以Post方式获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="jsonData">json请求内容</param>
        /// <param name="url">请求url</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        T Post<T>(IJsonSerializer jsonSerializer, UrlConcat url, string jsonData, IDictionary<string, string> headerParams, int timeout = 30000);

        /// <summary>
        /// 以Post方式发送请求
        /// </summary>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="jsonData">json请求内容</param>
        /// <param name="url">请求url</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        void Post(IJsonSerializer jsonSerializer, UrlConcat url, string jsonData, IDictionary<string, string> headerParams, int timeout = 30000);

        /// <summary>
        /// 以Get方式获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="request">参数请求</param>
        /// <param name="requestPara">请求参数集合.</param>
        /// <param name="route">路由标识</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        T Get<T>(IJsonSerializer jsonSerializer, IRoutePrimaryKeySelect request, string route, IDictionary<string, string> requestPara, IDictionary<string, string> headerParams, int timeout = 30000);

        /// <summary>
        /// 以Get方式发送请求
        /// </summary>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="request">参数请求</param>
        /// <param name="requestPara">请求参数集合.</param>
        /// <param name="route">路由标识</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        void Get(IJsonSerializer jsonSerializer, IRoutePrimaryKeySelect request, string route, IDictionary<string, string> requestPara, IDictionary<string, string> headerParams, int timeout = 30000);

        /// <summary>
        /// 以Get方式获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="requestPara">请求参数集合.</param>
        /// <param name="url">请求url</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        T Get<T>(IJsonSerializer jsonSerializer, UrlConcat url, IDictionary<string, string> requestPara, IDictionary<string, string> headerParams, int timeout = 30000);

        /// <summary>
        /// 以Get方式发送请求
        /// </summary>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="requestPara">请求参数集合.</param>
        /// <param name="url">请求url</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        void Get(IJsonSerializer jsonSerializer, UrlConcat url, IDictionary<string, string> requestPara, IDictionary<string, string> headerParams, int timeout = 30000);
    }
}
