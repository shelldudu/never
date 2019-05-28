using Never;
using Never.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Never.Net
{
    /// <summary>
    /// 网络资源的简单下载
    /// </summary>
    public struct WebRequestDownloader : IHttpDownloader
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequestDownloader"/> class.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        public WebRequestDownloader(Encoding encoding)
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
        /// <returns></returns>
        public byte[] Get(string url)
        {
            return this.Get(url, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <returns></returns>
        public byte[] Get(string url, IDictionary<string, string> getParams)
        {
            return this.Get(url, getParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public byte[] Get(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams)
        {
            return this.Get(url, getParams, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <returns></returns>
        public byte[] Get(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType)
        {
            return this.Get(url, getParams, headerParams, contentType, -1);
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
        public byte[] Get(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType, int timeout)
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

            return this.Get(new Uri(string.Concat(url, url.IndexOf("?") >= 0 ? "&" : "?", paramString)), headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <returns></returns>
        public byte[] Get(Uri uri)
        {
            return this.Get(uri, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public byte[] Get(Uri uri, IDictionary<string, string> headerParams)
        {
            return this.Get(uri, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        public byte[] Get(Uri uri, IDictionary<string, string> headerParams, string contentType)
        {
            return this.Get(uri, headerParams, contentType, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public byte[] Get(Uri uri, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = contentType;
            if (timeout > 0)
            {
                request.Timeout = timeout;
            }

            if (headerParams != null && headerParams.Count > 0)
            {
                foreach (var header in headerParams)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            using (var response = request.GetResponse() as HttpWebResponse)
            using (var st = new MemoryStream())
            {
                if (response.Headers != null && response.Headers.AllKeys != null && headerParams != null)
                {
                    foreach (var h in response.Headers.AllKeys)
                    {
                        headerParams[h] = response.Headers.Get(h);
                    }
                }

                response.GetResponseStream().CopyTo(st);
                st.Position = 0;
                return st.ToArray();
            }
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        public byte[] Post(string url, IDictionary<string, string> postParams)
        {
            return this.Post(url, postParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public byte[] Post(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams)
        {
            return this.Post(url, postParams, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <returns></returns>
        public byte[] Post(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return this.Post(url, postParams, headerParams, contentType, -1);
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
        public byte[] Post(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return this.Post(new Uri(url), postParams, headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        public byte[] Post(Uri uri, IDictionary<string, string> postParams)
        {
            return this.Post(uri, postParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public byte[] Post(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams)
        {
            return this.Post(uri, postParams, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <returns></returns>
        public byte[] Post(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return this.Post(uri, postParams, headerParams, contentType, -1);
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
        public byte[] Post(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            string paramString = string.Empty;
            if (postParams != null && postParams.Count > 0)
            {
                if (contentType.Equals(ContentType.Json, StringComparison.OrdinalIgnoreCase))
                {
                    paramString = "{";
                    foreach (var param in postParams)
                    {
                        paramString = string.Concat(paramString, string.Format("\"{0}\":\"{1}\",", param.Key, param.Value));
                    }

                    paramString = paramString.Trim(',') + "}";

                    return this.Post(uri, paramString, headerParams, timeout);
                }

                foreach (var param in postParams)
                {
                    paramString = string.Concat(paramString, string.Format("{0}={1}&", param.Key, param.Value));
                }

                paramString = paramString.Trim('&');
            }

            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = contentType;

            if (timeout > 0)
            {
                request.Timeout = timeout;
            }

            if (headerParams != null)
            {
                foreach (var header in headerParams)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var data = this.Encoding.GetBytes(paramString);
            request.ContentLength = data.Length;
            using (Stream write = request.GetRequestStream())
            {
                write.Write(data, 0, data.Length);
                write.Flush();
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (var st = new MemoryStream())
            {
                if (response.Headers != null && response.Headers.AllKeys != null && headerParams != null)
                {
                    foreach (var h in response.Headers.AllKeys)
                    {
                        headerParams[h] = response.Headers.Get(h);
                    }
                }

                response.GetResponseStream().CopyTo(st);
                st.Position = 0;
                return st.ToArray();
            }
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public byte[] Post(string url, string jsonData)
        {
            return this.Post(url, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public byte[] Post(string url, string jsonData, IDictionary<string, string> headerParams)
        {
            return this.Post(url, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public byte[] Post(string url, string jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return this.Post(new Uri(url), jsonData, headerParams, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public byte[] Post(Uri uri, string jsonData)
        {
            return this.Post(uri, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public byte[] Post(Uri uri, string jsonData, IDictionary<string, string> headerParams)
        {
            return this.Post(uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public byte[] Post(Uri uri, string jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            string paramString = string.Empty;
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = ContentType.Json;

            if (timeout > 0)
            {
                request.Timeout = timeout;
            }

            if (headerParams != null)
            {
                foreach (var header in headerParams)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (!string.IsNullOrEmpty(jsonData))
            {
                var data = this.Encoding.GetBytes(jsonData);
                request.ContentLength = data.Length;
                using (Stream write = request.GetRequestStream())
                {
                    write.Write(data, 0, data.Length);
                    write.Flush();
                }
            }

            using (var response = request.GetResponse())
            using (var st = new MemoryStream())
            {
                if (response.Headers != null && response.Headers.AllKeys != null && headerParams != null)
                {
                    foreach (var h in response.Headers.AllKeys)
                    {
                        headerParams[h] = response.Headers.Get(h);
                    }
                }

                response.GetResponseStream().CopyTo(st);
                st.Position = 0;
                return st.ToArray();
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
        public byte[] Post(Uri uri, Stream postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return this.Post(uri, postParams, headerParams, contentType, -1);
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
        public byte[] Post(Uri uri, Stream postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            string paramString = string.Empty;
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = contentType;

            if (timeout > 0)
            {
                request.Timeout = timeout;
            }

            if (headerParams != null)
            {
                foreach (var header in headerParams)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (postParams != null)
            {
                if (postParams.CanSeek)
                {
                    using (var post = request.GetRequestStream())
                    {
                        postParams.Position = 0;
                        postParams.CopyTo(post);
                        post.Flush();
                    }
                }
                else
                {
                    using (var post = request.GetRequestStream())
                    using (var reader = new StreamReader(postParams))
                    using (var writer = new StreamWriter(post))
                    {
                        var text = reader.ReadToEnd();
                        writer.Write(text);
                        writer.Flush();
                    }
                }
            }
            else
            {
                using (Stream write = request.GetRequestStream())
                {
                    write.Write(new byte[0], 0, 0);
                    write.Flush();
                }
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (var st = new MemoryStream())
            {
                if (response.Headers != null && response.Headers.AllKeys != null && headerParams != null)
                {
                    foreach (var h in response.Headers.AllKeys)
                    {
                        headerParams[h] = response.Headers.Get(h);
                    }
                }

                response.GetResponseStream().CopyTo(st);
                st.Position = 0;
                return st.ToArray();
            }
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public byte[] Post(Uri uri, Stream jsonData, IDictionary<string, string> headerParams)
        {
            return this.Post(uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public byte[] Post(Uri uri, Stream jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            string paramString = string.Empty;
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = ContentType.Json;

            if (timeout > 0)
            {
                request.Timeout = timeout;
            }

            if (headerParams != null)
            {
                foreach (var header in headerParams)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (jsonData != null)
            {
                if (jsonData.CanSeek)
                {
                    using (var post = request.GetRequestStream())
                    {
                        jsonData.Position = 0;
                        jsonData.CopyTo(post);
                        post.Flush();
                    }
                }
                else
                {
                    using (var post = request.GetRequestStream())
                    using (var reader = new StreamReader(jsonData))
                    using (var writer = new StreamWriter(post))
                    {
                        var text = reader.ReadToEnd();
                        writer.Write(text);
                        writer.Flush();
                    }
                }
            }
            else
            {
                using (Stream write = request.GetRequestStream())
                {
                    write.Write(new byte[2] { 123, 125 }, 0, 2);
                    write.Flush();
                }
            }

            using (var response = request.GetResponse() as HttpWebResponse)
            using (var st = new MemoryStream())
            {
                if (response.Headers != null && response.Headers.AllKeys != null && headerParams != null)
                {
                    foreach (var h in response.Headers.AllKeys)
                    {
                        headerParams[h] = response.Headers.Get(h);
                    }
                }

                response.GetResponseStream().CopyTo(st);
                st.Position = 0;
                return st.ToArray();
            }
        }

        #endregion byte[]

        #region string

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <returns></returns>
        public string GetString(string url)
        {
            return this.GetString(url, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <returns></returns>
        public string GetString(string url, IDictionary<string, string> getParams)
        {
            return this.GetString(url, getParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public string GetString(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams)
        {
            return this.GetString(url, getParams, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="getParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">内容类型</param>
        /// <returns></returns>
        public string GetString(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType)
        {
            return this.GetString(url, getParams, headerParams, contentType, -1);
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
        public string GetString(string url, IDictionary<string, string> getParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return this.Encoding.GetString(this.Get(url, getParams, headerParams, contentType, timeout));
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <returns></returns>
        public string GetString(Uri uri)
        {
            return this.GetString(uri, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public string GetString(Uri uri, IDictionary<string, string> headerParams)
        {
            return this.GetString(uri, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <returns></returns>
        public string GetString(Uri uri, IDictionary<string, string> headerParams, string contentType)
        {
            return this.GetString(uri, headerParams, contentType, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public string GetString(Uri uri, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return this.Encoding.GetString(this.Get(uri, headerParams, contentType, timeout));
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        public string PostString(string url, IDictionary<string, string> postParams)
        {
            return this.PostString(url, postParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public string PostString(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams)
        {
            return this.PostString(url, postParams, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <returns></returns>
        public string PostString(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return this.PostString(url, postParams, headerParams, contentType, -1);
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
        public string PostString(string url, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return this.PostString(new Uri(url), postParams, headerParams, contentType, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <returns></returns>
        public string PostString(Uri uri, IDictionary<string, string> postParams)
        {
            return this.PostString(uri, postParams, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public string PostString(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams)
        {
            return this.PostString(uri, postParams, headerParams, ContentType.Default);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <returns></returns>
        public string PostString(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return this.PostString(uri, postParams, headerParams, contentType, -1);
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
        public string PostString(Uri uri, IDictionary<string, string> postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return this.Encoding.GetString(this.Post(uri, postParams, headerParams, contentType, timeout));
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public string PostString(string url, string jsonData)
        {
            return this.PostString(url, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public string PostString(string url, string jsonData, IDictionary<string, string> headerParams)
        {
            return this.PostString(url, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="url">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public string PostString(string url, string jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return this.PostString(new Uri(url), jsonData, headerParams, timeout);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <returns></returns>
        public string PostString(Uri uri, string jsonData)
        {
            return this.PostString(uri, jsonData, null);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public string PostString(Uri uri, string jsonData, IDictionary<string, string> headerParams)
        {
            return this.PostString(uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public string PostString(Uri uri, string jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return this.Encoding.GetString(this.Post(uri, jsonData, headerParams, timeout));
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="postParams">请求参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="contentType"> 标头的值</param>
        /// <returns></returns>
        public string PostString(Uri uri, Stream postParams, IDictionary<string, string> headerParams, string contentType)
        {
            return this.PostString(uri, postParams, headerParams, contentType, -1);
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
        public string PostString(Uri uri, Stream postParams, IDictionary<string, string> headerParams, string contentType, int timeout)
        {
            return this.Encoding.GetString(this.Post(uri, postParams, headerParams, contentType, timeout));
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <returns></returns>
        public string PostString(Uri uri, Stream jsonData, IDictionary<string, string> headerParams)
        {
            return this.PostString(uri, jsonData, headerParams, -1);
        }

        /// <summary>
        /// 从Url地址中下载数据
        /// </summary>
        /// <param name="uri">Url请求地址</param>
        /// <param name="jsonData">请求json参数</param>
        /// <param name="headerParams">标头的值</param>
        /// <param name="timeout">请求时间，以毫秒为单位，为0的则表示使用默认,默认值是 100,000 毫秒（100 秒）</param>
        /// <returns></returns>
        public string PostString(Uri uri, Stream jsonData, IDictionary<string, string> headerParams, int timeout)
        {
            return this.Encoding.GetString(this.Post(uri, jsonData, headerParams, timeout));
        }

        #endregion string
    }
}