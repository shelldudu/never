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
