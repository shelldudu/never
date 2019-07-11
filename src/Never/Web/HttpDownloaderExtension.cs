using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Never.Web
{
    /// <summary>
    /// 下载扩展
    /// </summary>
    public static class HttpDownloaderExtension
    {
        #region IHttpDownloader

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <returns></returns>
        public static byte[] JGet(this HttpRequestDownloader downloader, string url)
        {
            return JGet(downloader, url, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <returns></returns>
        public static byte[] JGet(this HttpRequestDownloader downloader, string url, IDictionary<string, string> getParams)
        {
            return JGet(downloader, url, getParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static byte[] JGet(this HttpRequestDownloader downloader, string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams)
        {
            return JGet(downloader, url, getParams, headerParams, "application/json", -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static byte[] JGet(this HttpRequestDownloader downloader, string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return downloader.Get(url, getParams, headerParams, contentType, timeout, out var httpStatus);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <returns></returns>
        public static byte[] JGet(this HttpRequestDownloader downloader, Uri uri)
        {
            return JGet(downloader, uri, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static byte[] JGet(this HttpRequestDownloader downloader, Uri uri, IDictionary<string, string> headerParams)
        {
            return JGet(downloader, uri, headerParams, "application/json", -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static byte[] JGet(this HttpRequestDownloader downloader, Uri uri, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return downloader.Get(uri, headerParams, contentType, timeout, out var httpStatus);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <returns></returns>
        public static string JGetString(this HttpRequestDownloader downloader, string url)
        {
            return JGetString(downloader, url, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <returns></returns>
        public static string JGetString(this HttpRequestDownloader downloader, string url, IDictionary<string, string> getParams)
        {
            return JGetString(downloader, url, getParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static string JGetString(this HttpRequestDownloader downloader, string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams)
        {
            return JGetString(downloader, url, getParams, headerParams, "application/json", -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static string JGetString(this HttpRequestDownloader downloader, string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return downloader.GetString(url, getParams, headerParams, contentType, timeout, out var httpStatus);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <returns></returns>
        public static string JGetString(this HttpRequestDownloader downloader, Uri uri)
        {
            return JGetString(downloader, uri, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static string JGetString(this HttpRequestDownloader downloader, Uri uri, IDictionary<string, string> headerParams)
        {
            return JGetString(downloader, uri, headerParams, "application/json", -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static string JGetString(this HttpRequestDownloader downloader, Uri uri, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return downloader.GetString(uri, headerParams, contentType, timeout, out var httpStatus);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static byte[] JPost(this HttpRequestDownloader downloader, string url, string jsonData)
        {
            return JPost(downloader, url, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static byte[] JPost(this HttpRequestDownloader downloader, string url, string jsonData, IDictionary<string, string> headerParams)
        {
            return JPost(downloader, url, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static byte[] JPost(this HttpRequestDownloader downloader, string url, string jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return JPost(downloader, new Uri(url), jsonData, headerParams, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static byte[] JPost(this HttpRequestDownloader downloader, string url, Stream jsonData)
        {
            return JPost(downloader, url, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static byte[] JPost(this HttpRequestDownloader downloader, string url, Stream jsonData, IDictionary<string, string> headerParams)
        {
            return JPost(downloader, url, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static byte[] JPost(this HttpRequestDownloader downloader, string url, Stream jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return JPost(downloader, new Uri(url), jsonData, headerParams, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static byte[] JPost(this HttpRequestDownloader downloader, Uri uri, string jsonData)
        {
            return JPost(downloader, uri, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static byte[] JPost(this HttpRequestDownloader downloader, Uri uri, string jsonData, IDictionary<string, string> headerParams)
        {
            return JPost(downloader, uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static byte[] JPost(this HttpRequestDownloader downloader, Uri uri, string jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return JPost(downloader, uri, jsonData, headerParams, string.Empty, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static byte[] JPost(this HttpRequestDownloader downloader, Uri uri, string jsonData, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return JPost(downloader, uri, new MemoryStream(downloader.Encoding.GetBytes(jsonData ?? "{}")), headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static byte[] JPost(this HttpRequestDownloader downloader, Uri uri, Stream jsonData)
        {
            return JPost(downloader, uri, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static byte[] JPost(this HttpRequestDownloader downloader, Uri uri, Stream jsonData, IDictionary<string, string> headerParams)
        {
            return JPost(downloader, uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static byte[] JPost(this HttpRequestDownloader downloader, Uri uri, Stream jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return JPost(downloader, uri, jsonData, headerParams, string.Empty, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static byte[] JPost(this HttpRequestDownloader downloader, Uri uri, Stream jsonData, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return downloader.Post(uri, jsonData == null ? new MemoryStream(downloader.Encoding.GetBytes("{}")) : jsonData, headerParams, string.IsNullOrWhiteSpace(contentType) ? "application/json" : contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static string JPostString(this HttpRequestDownloader downloader, string url, string jsonData)
        {
            return JPostString(downloader, url, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static string JPostString(this HttpRequestDownloader downloader, string url, string jsonData, IDictionary<string, string> headerParams)
        {
            return JPostString(downloader, url, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static string JPostString(this HttpRequestDownloader downloader, string url, string jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return JPostString(downloader, new Uri(url), jsonData, headerParams, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static string JPostString(this HttpRequestDownloader downloader, Uri uri, string jsonData)
        {
            return JPostString(downloader, uri, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static string JPostString(this HttpRequestDownloader downloader, Uri uri, string jsonData, IDictionary<string, string> headerParams)
        {
            return JPostString(downloader, uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static string JPostString(this HttpRequestDownloader downloader, Uri uri, string jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return JPostString(downloader, uri, jsonData, headerParams, string.Empty, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static string JPostString(this HttpRequestDownloader downloader, Uri uri, string jsonData, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return JPostString(downloader, uri, new MemoryStream(downloader.Encoding.GetBytes(jsonData ?? "{}")), headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static string JPostString(this HttpRequestDownloader downloader, string url, Stream jsonData)
        {
            return JPostString(downloader, url, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static string JPostString(this HttpRequestDownloader downloader, string url, Stream jsonData, IDictionary<string, string> headerParams)
        {
            return JPostString(downloader, url, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static string JPostString(this HttpRequestDownloader downloader, string url, Stream jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return JPostString(downloader, url, jsonData, headerParams, string.Empty, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static string JPostString(this HttpRequestDownloader downloader, string url, Stream jsonData, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return JPostString(downloader, new Uri(url), jsonData, headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static string JPostString(this HttpRequestDownloader downloader, Uri uri, Stream jsonData)
        {
            return JPostString(downloader, uri, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static string JPostString(this HttpRequestDownloader downloader, Uri uri, Stream jsonData, IDictionary<string, string> headerParams)
        {
            return JPostString(downloader, uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static string JPostString(this HttpRequestDownloader downloader, Uri uri, Stream jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return JPostString(downloader, uri, jsonData, headerParams, string.Empty, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static string JPostString(this HttpRequestDownloader downloader, Uri uri, Stream jsonData, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return downloader.PostString(uri, jsonData == null ? new MemoryStream(downloader.Encoding.GetBytes("{}")) : jsonData, headerParams, string.IsNullOrWhiteSpace(contentType) ? "application/json" : contentType, timeout);
        }

        #endregion IHttpDownloader

        #region IHttpAsyncDownloader

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <returns></returns>
        public static async Task<byte[]> JGetAsync(this HttpClientDownloader downloader, string url)
        {
            return await JGetAsync(downloader, url, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <returns></returns>
        public static async Task<byte[]> JGetAsync(this HttpClientDownloader downloader, string url, IDictionary<string, string> getParams)
        {
            return await JGetAsync(downloader, url, getParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static async Task<byte[]> JGetAsync(this HttpClientDownloader downloader, string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams)
        {
            return await JGetAsync(downloader, url, getParams, headerParams, "application/json", -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static async Task<byte[]> JGetAsync(this HttpClientDownloader downloader, string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return await downloader.GetAsync(url, getParams, headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <returns></returns>
        public static async Task<byte[]> JGetAsync(this HttpClientDownloader downloader, Uri uri)
        {
            return await JGetAsync(downloader, uri, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static async Task<byte[]> JGetAsync(this HttpClientDownloader downloader, Uri uri, IDictionary<string, string> headerParams)
        {
            return await JGetAsync(downloader, uri, headerParams, "application/json", -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static async Task<byte[]> JGetAsync(this HttpClientDownloader downloader, Uri uri, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return await downloader.JGetAsync(uri, headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <returns></returns>
        public static async Task<string> JGetStringAsync(this HttpClientDownloader downloader, string url)
        {
            return await JGetStringAsync(downloader, url, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <returns></returns>
        public static async Task<string> JGetStringAsync(this HttpClientDownloader downloader, string url, IDictionary<string, string> getParams)
        {
            return await JGetStringAsync(downloader, url, getParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static async Task<string> JGetStringAsync(this HttpClientDownloader downloader, string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams)
        {
            return await JGetStringAsync(downloader, url, getParams, headerParams, "application/json", -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static async Task<string> JGetStringAsync(this HttpClientDownloader downloader, string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return await downloader.JGetStringAsync(url, getParams, headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <returns></returns>
        public static async Task<string> JGetStringAsync(this HttpClientDownloader downloader, Uri uri)
        {
            return await JGetStringAsync(downloader, uri, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static async Task<string> JGetStringAsync(this HttpClientDownloader downloader, Uri uri, IDictionary<string, string> headerParams)
        {
            return await JGetStringAsync(downloader, uri, headerParams, "application/json", -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static async Task<string> JGetStringAsync(this HttpClientDownloader downloader, Uri uri, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return await downloader.JGetStringAsync(uri, headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static Task<byte[]> JPost(this HttpClientDownloader downloader, string url, string jsonData)
        {
            return JPost(downloader, url, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static Task<byte[]> JPost(this HttpClientDownloader downloader, string url, string jsonData, IDictionary<string, string> headerParams)
        {
            return JPost(downloader, url, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static Task<byte[]> JPost(this HttpClientDownloader downloader, string url, string jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return JPost(downloader, new Uri(url), jsonData, headerParams, string.Empty, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static Task<byte[]> JPost(this HttpClientDownloader downloader, Uri uri, string jsonData)
        {
            return JPost(downloader, uri, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static Task<byte[]> JPost(this HttpClientDownloader downloader, Uri uri, string jsonData, IDictionary<string, string> headerParams)
        {
            return JPost(downloader, uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static Task<byte[]> JPost(this HttpClientDownloader downloader, Uri uri, string jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return JPost(downloader, uri, jsonData, headerParams, string.Empty, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static Task<byte[]> JPost(this HttpClientDownloader downloader, Uri uri, string jsonData, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return JPost(downloader, uri, new MemoryStream(downloader.Encoding.GetBytes(jsonData ?? "{}")), headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static Task<byte[]> JPost(this HttpClientDownloader downloader, string url, Stream jsonData)
        {
            return JPost(downloader, url, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static Task<byte[]> JPost(this HttpClientDownloader downloader, string url, Stream jsonData, IDictionary<string, string> headerParams)
        {
            return JPost(downloader, url, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static Task<byte[]> JPost(this HttpClientDownloader downloader, string url, Stream jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return JPost(downloader, new Uri(url), jsonData, headerParams, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static Task<byte[]> JPost(this HttpClientDownloader downloader, Uri uri, Stream jsonData)
        {
            return JPost(downloader, uri, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static Task<byte[]> JPost(this HttpClientDownloader downloader, Uri uri, Stream jsonData, IDictionary<string, string> headerParams)
        {
            return JPost(downloader, uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static Task<byte[]> JPost(this HttpClientDownloader downloader, Uri uri, Stream jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return JPost(downloader, uri, jsonData, headerParams, string.Empty, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static Task<byte[]> JPost(this HttpClientDownloader downloader, Uri uri, Stream jsonData, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return downloader.PostAsync(uri, jsonData == null ? new MemoryStream(downloader.Encoding.GetBytes("{}")) : jsonData, headerParams, string.IsNullOrWhiteSpace(contentType) ? "application/json" : contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static async Task<string> JPostStringAsync(this HttpClientDownloader downloader, string url, string jsonData)
        {
            return await JPostStringAsync(downloader, url, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static async Task<string> JPostStringAsync(this HttpClientDownloader downloader, string url, string jsonData, IDictionary<string, string> headerParams)
        {
            return await JPostStringAsync(downloader, url, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static async Task<string> JPostStringAsync(this HttpClientDownloader downloader, string url, string jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return await JPostStringAsync(downloader, new Uri(url), jsonData, headerParams, string.Empty, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static async Task<string> JPostStringAsync(this HttpClientDownloader downloader, Uri uri, string jsonData)
        {
            return await JPostStringAsync(downloader, uri, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static async Task<string> JPostStringAsync(this HttpClientDownloader downloader, Uri uri, string jsonData, IDictionary<string, string> headerParams)
        {
            return await JPostStringAsync(downloader, uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static async Task<string> JPostStringAsync(this HttpClientDownloader downloader, Uri uri, string jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return await JPostStringAsync(downloader, uri, jsonData, headerParams, string.Empty, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static async Task<string> JPostStringAsync(this HttpClientDownloader downloader, Uri uri, string jsonData, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return await JPostStringAsync(downloader, uri, new MemoryStream(downloader.Encoding.GetBytes(jsonData ?? "{}")), headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static async Task<string> JPostStringAsync(this HttpClientDownloader downloader, string url, Stream jsonData)
        {
            return await JPostStringAsync(downloader, url, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static async Task<string> JPostStringAsync(this HttpClientDownloader downloader, string url, Stream jsonData, IDictionary<string, string> headerParams)
        {
            return await JPostStringAsync(downloader, url, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static async Task<string> JPostStringAsync(this HttpClientDownloader downloader, string url, Stream jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return await JPostStringAsync(downloader, url, jsonData, headerParams, string.Empty, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static async Task<string> JPostStringAsync(this HttpClientDownloader downloader, string url, Stream jsonData, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return await JPostStringAsync(downloader, new Uri(url), jsonData, headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public static async Task<string> JPostStringAsync(this HttpClientDownloader downloader, Uri uri, Stream jsonData)
        {
            return await JPostStringAsync(downloader, uri, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public static async Task<string> JPostStringAsync(this HttpClientDownloader downloader, Uri uri, Stream jsonData, IDictionary<string, string> headerParams)
        {
            return await JPostStringAsync(downloader, uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static async Task<string> JPostStringAsync(this HttpClientDownloader downloader, Uri uri, Stream jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return await JPostStringAsync(downloader, uri, jsonData, headerParams, string.Empty, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="downloader">下載器</param>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public static async Task<string> JPostStringAsync(this HttpClientDownloader downloader, Uri uri, Stream jsonData, IDictionary<string, string> headerParams, string contentType = "application/json", int timeout = -1)
        {
            return await downloader.PostStringAsync(uri, jsonData == null ? new MemoryStream(downloader.Encoding.GetBytes("{}")) : jsonData, headerParams, string.IsNullOrWhiteSpace(contentType) ? "application/json" : contentType, timeout);
        }

        #endregion IHttpAsyncDownloader
    }
}