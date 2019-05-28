using System;
using System.Collections.Generic;
using System.IO;
using Never;
using Never.Web;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Never.Net
{
    /// <summary>
    /// HttpClient异步下载器
    /// </summary>
    public struct HttpClientDownloader : IHttpAsyncDownloader
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

        #endregion

        #region byte[]

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <returns></returns>
        public async Task<byte[]> Get(string url, IDictionary<string, string> getParams)
        {
            return await this.Get(url, getParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> Get(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams)
        {
            return await this.Get(url, getParams, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <returns></returns>
        public async Task<byte[]> Get(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.Get(url, getParams, headerParams, contentType, -1);
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
        public async Task<byte[]> Get(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType, int timeout)
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

            return await this.Get(new Uri(string.Concat(url, url.IndexOf("?") >= 0 ? "&" : "?", paramString)), headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <returns></returns>
        public async Task<byte[]> Get(Uri uri)
        {
            return await this.Get(uri, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> Get(Uri uri, IDictionary<string, string> headerParams)
        {
            return await this.Get(uri, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> Get(Uri uri, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.Get(uri, headerParams, contentType, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<byte[]> Get(Uri uri, IDictionary<string, string> headerParams, string contentType, int timeout)
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
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        public async Task<byte[]> Post(string url, IDictionary<string, string> postParams)
        {
            return await this.Post(url, postParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> Post(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams)
        {
            return await this.Post(url, postParams, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> Post(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.Post(url, postParams, headerParams, contentType, -1);
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
        public async Task<byte[]> Post(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return await this.Post(new Uri(url), postParams, headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        public async Task<byte[]> Post(Uri uri, IDictionary<string, string> postParams)
        {
            return await this.Post(uri, postParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> Post(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams)
        {
            return await this.Post(uri, postParams, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> Post(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.Post(uri, postParams, headerParams, contentType, -1);
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
        public async Task<byte[]> Post(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
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
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public async Task<byte[]> Post(string url, string jsonData)
        {
            return await this.Post(url, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> Post(string url, string jsonData, IDictionary<string, string> headerParams)
        {
            return await this.Post(url, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<byte[]> Post(string url, string jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return await this.Post(new Uri(url), jsonData, headerParams, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public async Task<byte[]> Post(Uri uri, string jsonData)
        {
            return await this.Post(uri, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> Post(Uri uri, string jsonData, IDictionary<string, string> headerParams)
        {
            return await this.Post(uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<byte[]> Post(Uri uri, string jsonData, IDictionary<string, string> headerParams, int timeout)
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

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType.Json));
                HttpContent content = new StringContent(jsonData);
                content.Headers.ContentType = new MediaTypeHeaderValue(ContentType.Json);
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
        /// <returns></returns>
        public async Task<byte[]> Post(Uri uri, Stream postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.Post(uri, postParams, headerParams, contentType, -1);
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
        public async Task<byte[]> Post(Uri uri, Stream postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
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

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<byte[]> Post(Uri uri, Stream jsonData, IDictionary<string, string> headerParams)
        {
            return await this.Post(uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<byte[]> Post(Uri uri, Stream jsonData, IDictionary<string, string> headerParams, int timeout)
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
                    if (jsonData != null)
                    {
                        jsonData.CopyTo(stream);
                        stream.Position = 0;
                        stream.Flush();
                    }
                    else
                    {
                        stream.Write(new byte[2] { 123, 125 }, 0, 2);
                        stream.Position = 0;
                        stream.Flush();
                    }

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType.Json));
                    HttpContent content = new StreamContent(stream);
                    content.Headers.ContentType = new MediaTypeHeaderValue(ContentType.Json);
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
        public async Task<string> GetString(string url, IDictionary<string, string> getParams)
        {
            return await this.GetString(url, getParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<string> GetString(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams)
        {
            return await this.GetString(url, getParams, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <returns></returns>
        public async Task<string> GetString(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.GetString(url, getParams, headerParams, contentType, -1);
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
        public async Task<string> GetString(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return this.Encoding.GetString(await this.Get(url, getParams, headerParams, contentType, timeout));
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <returns></returns>
        public async Task<string> GetString(Uri uri)
        {
            return await this.GetString(uri, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<string> GetString(Uri uri, IDictionary<string, string> headerParams)
        {
            return await this.GetString(uri, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        public async Task<string> GetString(Uri uri, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.GetString(uri, headerParams, contentType, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<string> GetString(Uri uri, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return this.Encoding.GetString(await this.Get(uri, headerParams, contentType, timeout));
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        public async Task<string> PostString(string url, IDictionary<string, string> postParams)
        {
            return await this.PostString(url, postParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<string> PostString(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams)
        {
            return await this.PostString(url, postParams, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        public async Task<string> PostString(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.PostString(url, postParams, headerParams, contentType, -1);
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
        public async Task<string> PostString(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return await this.PostString(new Uri(url), postParams, headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        public async Task<string> PostString(Uri uri, IDictionary<string, string> postParams)
        {
            return await this.PostString(uri, postParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<string> PostString(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams)
        {
            return await this.PostString(uri, postParams, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        public async Task<string> PostString(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.PostString(uri, postParams, headerParams, contentType, -1);
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
        public async Task<string> PostString(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return this.Encoding.GetString(await this.Post(uri, postParams, headerParams, contentType, timeout));
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public async Task<string> PostString(string url, string jsonData)
        {
            return await this.PostString(url, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<string> PostString(string url, string jsonData, IDictionary<string, string> headerParams)
        {
            return await this.PostString(url, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<string> PostString(string url, string jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return await this.PostString(new Uri(url), jsonData, headerParams, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public async Task<string> PostString(Uri uri, string jsonData)
        {
            return await this.PostString(uri, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<string> PostString(Uri uri, string jsonData, IDictionary<string, string> headerParams)
        {
            return await this.PostString(uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<string> PostString(Uri uri, string jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return this.Encoding.GetString(await this.Post(uri, jsonData, headerParams, timeout));
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <returns></returns>
        public async Task<string> PostString(Uri uri, Stream postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return await this.PostString(uri, postParams, headerParams, contentType, -1);
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
        public async Task<string> PostString(Uri uri, Stream postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return this.Encoding.GetString(await this.Post(uri, postParams, headerParams, timeout));
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public async Task<string> PostString(Uri uri, Stream jsonData, IDictionary<string, string> headerParams)
        {
            return await this.PostString(uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public async Task<string> PostString(Uri uri, Stream jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return this.Encoding.GetString(await this.Post(uri, jsonData, headerParams, timeout));
        }

        #endregion string
    }
}