using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Never.Web
{
    /// <summary>
    /// HttpClient异步下载器
    /// </summary>
    public struct HttpClientDownloader
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientDownloader"/> class.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        public HttpClientDownloader(Encoding encoding)
        {
            this.encoding = encoding ?? Encoding.UTF8;
        }

        #endregion ctor

        #region encoding

        /// <summary>
        /// 数据编码
        /// </summary>
        private readonly Encoding encoding;

        /// <summary>
        /// 数据编码
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return this.encoding ?? Encoding.UTF8;
            }
        }

        #endregion encoding

        #region byte[]

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <returns></returns>
        public async Task<byte[]> GetAsync(string url)
        {
            return await this.GetAsync(url, null, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <returns></returns>
        public async Task<byte[]> GetAsync(string url, IDictionary<string, string> getParams)
        {
            return await this.GetAsync(url, getParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> GetAsync(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams)
        {
            return await this.GetAsync(url, getParams, headerParams, "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <returns></returns>
        public async Task<byte[]> GetAsync(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.GetAsync(url, getParams, headerParams, contentType, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<byte[]> GetAsync(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            string paramString = string.Empty;
            if (getParams != null && getParams.Count > 0)
            {
                foreach (var param in getParams)
                {
                    paramString = string.Concat(paramString, string.Format("{0}={1}&", param.Key, param.Value));
                }

                paramString = paramString.Trim('&');
            }

            return await this.GetAsync(new Uri(string.Concat(url, url.IndexOf("?") >= 0 ? "&" : "?", paramString)), headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <returns></returns>
        public async Task<byte[]> GetAsync(Uri uri)
        {
            return await this.GetAsync(uri, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> GetAsync(Uri uri, IDictionary<string, string> headerParams)
        {
            return await this.GetAsync(uri, headerParams, "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> GetAsync(Uri uri, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.GetAsync(uri, headerParams, contentType, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<byte[]> GetAsync(Uri uri, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            using (var client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip }))
            {
                if (headerParams != null && headerParams.Count > 0)
                {
                    foreach (var header in headerParams)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                if (response.Headers != null && response.Headers.Count() > 0 && headerParams != null)
                {
                    foreach (var h in response.Headers)
                    {
                        headerParams[h.Key] = string.Join(",", h.Value);
                    }
                }

                return await response.Content.ReadAsByteArrayAsync();
            }
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <returns></returns>
        public async Task<byte[]> PostAsync(string url)
        {
            return await this.PostAsync(url, null, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        public async Task<byte[]> PostAsync(string url, IDictionary<string, string> postParams)
        {
            return await this.PostAsync(url, postParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> PostAsync(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams)
        {
            return await this.PostAsync(url, postParams, headerParams, "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> PostAsync(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.PostAsync(url, postParams, headerParams, contentType, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<byte[]> PostAsync(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return await this.PostAsync(new Uri(url), postParams, headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<byte[]> PostAsync(string url, Stream postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return await this.PostAsync(new Uri(url), postParams, headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <returns></returns>
        public async Task<byte[]> PostAsync(Uri uri)
        {
            return await this.PostAsync(uri, null, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        public async Task<byte[]> PostAsync(Uri uri, IDictionary<string, string> postParams)
        {
            return await this.PostAsync(uri, postParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> PostAsync(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams)
        {
            return await this.PostAsync(uri, postParams, headerParams, "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> PostAsync(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.PostAsync(uri, postParams, headerParams, contentType, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<byte[]> PostAsync(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            using (var client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip }))
            {
                if (headerParams != null && headerParams.Count > 0)
                {
                    Array.ForEach(headerParams.ToArray(), p =>
                    {
                        client.DefaultRequestHeaders.Add(p.Key, p.Value);
                    });
                }

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                HttpContent content = new FormUrlEncodedContent(postParams);
                content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                if (timeout > 0)
                {
                    client.Timeout = TimeSpan.FromMilliseconds(timeout);
                }

                var response = await client.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();
                if (response.Headers != null && response.Headers.Count() > 0 && headerParams != null)
                {
                    foreach (var h in response.Headers)
                    {
                        headerParams[h.Key] = string.Join(",", h.Value);
                    }
                }

                return await response.Content.ReadAsByteArrayAsync();
            }
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<byte[]> PostAsync(Uri uri, Stream postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            using (var client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip }))
            {
                if (headerParams != null && headerParams.Count > 0)
                {
                    Array.ForEach(headerParams.ToArray(), p =>
                    {
                        client.DefaultRequestHeaders.Add(p.Key, p.Value);
                    });
                }

                using (var stream = new MemoryStream())
                {
                    if (postParams != null)
                    {
                        postParams.CopyTo(stream);
                        stream.Position = 0;
                        stream.Flush();
                    }

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                    HttpContent content = new StreamContent(stream);
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    if (timeout > 0)
                    {
                        client.Timeout = TimeSpan.FromMilliseconds(timeout);
                    }

                    var response = await client.PostAsync(uri, content);
                    response.EnsureSuccessStatusCode();
                    if (response.Headers != null && response.Headers.Count() > 0 && headerParams != null)
                    {
                        foreach (var h in response.Headers)
                        {
                            headerParams[h.Key] = string.Join(",", h.Value);
                        }
                    }

                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
        }

        #endregion byte[]

        #region string

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string url, IDictionary<string, string> getParams)
        {
            return await this.GetStringAsync(url, getParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams)
        {
            return await this.GetStringAsync(url, getParams, headerParams, "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.GetStringAsync(url, getParams, headerParams, contentType, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return this.Encoding.GetString(await this.GetAsync(url, getParams, headerParams, contentType, timeout));
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(Uri uri)
        {
            return await this.GetStringAsync(uri, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(Uri uri, IDictionary<string, string> headerParams)
        {
            return await this.GetStringAsync(uri, headerParams, "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(Uri uri, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.GetStringAsync(uri, headerParams, contentType, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(Uri uri, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return this.Encoding.GetString(await this.GetAsync(uri, headerParams, contentType, timeout));
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <returns></returns>
        public async Task<string> PostStringAsync(string url)
        {
            return await this.PostStringAsync(url, null, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        public async Task<string> PostStringAsync(string url, IDictionary<string, string> postParams)
        {
            return await this.PostStringAsync(url, postParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<string> PostStringAsync(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams)
        {
            return await this.PostStringAsync(url, postParams, headerParams, "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        public async Task<string> PostStringAsync(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.PostStringAsync(url, postParams, headerParams, contentType, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<string> PostStringAsync(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return await this.PostStringAsync(new Uri(url), postParams, headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <returns></returns>
        public async Task<string> PostStringAsync(Uri uri)
        {
            return await this.PostStringAsync(uri, null, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        public async Task<string> PostStringAsync(Uri uri, IDictionary<string, string> postParams)
        {
            return await this.PostStringAsync(uri, postParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<string> PostStringAsync(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams)
        {
            return await this.PostStringAsync(uri, postParams, headerParams, "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        public async Task<string> PostStringAsync(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.PostStringAsync(uri, postParams, headerParams, contentType, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<string> PostStringAsync(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return this.Encoding.GetString(await this.PostAsync(uri, postParams, headerParams, contentType, timeout));
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <returns></returns>
        public async Task<string> PostStringAsync(Uri uri, Stream postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.PostStringAsync(uri, postParams, headerParams, contentType, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<string> PostStringAsync(string uri, Stream postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return await this.PostStringAsync(new Uri(uri), postParams, headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<string> PostStringAsync(Uri uri, Stream postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return this.Encoding.GetString(await this.PostAsync(uri, postParams, headerParams, contentType, timeout));
        }

        #endregion string
    }
}