using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Never.Web
{
    /// <summary>
    /// 小数据异步下载器接口
    /// </summary>
    public interface IHttpAsyncDownloader
    {
        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <returns></returns>
        Task<byte[]> Get(string url, IDictionary<string, string> getParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        Task<byte[]> Get(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <returns></returns>
        Task<byte[]> Get(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        Task<byte[]> Get(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType, int timeout);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <returns></returns>
        Task<byte[]> Get(Uri uri);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        Task<byte[]> Get(Uri uri, IDictionary<string, string> headerParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        Task<byte[]> Get(Uri uri, IDictionary<string, string> headerParams, string contentType);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        Task<byte[]> Get(Uri uri, IDictionary<string, string> headerParams, string contentType, int timeout);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        Task<byte[]> Post(string url, IDictionary<string, string> postParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        Task<byte[]> Post(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <returns></returns>
        Task<byte[]> Post(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        Task<byte[]> Post(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        Task<byte[]> Post(Uri uri, IDictionary<string, string> postParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        Task<byte[]> Post(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <returns></returns>
        Task<byte[]> Post(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        Task<byte[]> Post(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        Task<byte[]> Post(string url, string jsonData);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        Task<byte[]> Post(string url, string jsonData, IDictionary<string, string> headerParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        Task<byte[]> Post(string url, string jsonData, IDictionary<string, string> headerParams, int timeout);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        Task<byte[]> Post(Uri uri, string jsonData);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        Task<byte[]> Post(Uri uri, string jsonData, IDictionary<string, string> headerParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        Task<byte[]> Post(Uri uri, string jsonData, IDictionary<string, string> headerParams, int timeout);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <returns></returns>
        Task<string> GetString(string url, IDictionary<string, string> getParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        Task<string> GetString(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <returns></returns>
        Task<string> GetString(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        Task<string> GetString(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType, int timeout);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <returns></returns>
        Task<string> GetString(Uri uri);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        Task<string> GetString(Uri uri, IDictionary<string, string> headerParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        Task<string> GetString(Uri uri, IDictionary<string, string> headerParams, string contentType);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        Task<string> GetString(Uri uri, IDictionary<string, string> headerParams, string contentType, int timeout);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        Task<string> PostString(string url, IDictionary<string, string> postParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        Task<string> PostString(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <returns></returns>
        Task<string> PostString(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        Task<string> PostString(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        Task<string> PostString(Uri uri, IDictionary<string, string> postParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        Task<string> PostString(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <returns></returns>
        Task<string> PostString(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        Task<string> PostString(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        Task<string> PostString(string url, string jsonData);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        Task<string> PostString(string url, string jsonData, IDictionary<string, string> headerParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        Task<string> PostString(string url, string jsonData, IDictionary<string, string> headerParams, int timeout);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        Task<string> PostString(Uri uri, string jsonData);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        Task<string> PostString(Uri uri, string jsonData, IDictionary<string, string> headerParams);

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        Task<string> PostString(Uri uri, string jsonData, IDictionary<string, string> headerParams, int timeout);
    }
}