using Never.Web.WebApi.Encryptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Never.Web.WebApi.MessageHandlers
{
    /// <summary>
    /// 延时加载内容流
    /// </summary>
    public class LazyStreamContent : HttpContent
    {
        #region field

        /// <summary>
        /// The stream content
        /// </summary>
        private DelegatingStreamContent streamContent = null;

        /// <summary>
        /// The request
        /// </summary>
        private readonly HttpRequestMessage request = null;

        /// <summary>
        /// The model
        /// </summary>
        private readonly IApiContentEncryptor encryption = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyStreamContent"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="encryption">The model.</param>
        public LazyStreamContent(HttpRequestMessage request, IApiContentEncryptor encryption)
        {
            this.request = request;
            this.encryption = encryption;
        }

        #endregion ctor

        #region load

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        public HttpContent Load()
        {
            var buffer = this.request.Content.ReadAsByteArrayAsync().Result;
            if (buffer == null || buffer.Length == 0)
                return request.Content;

            //this.streamContent = new DelegatingStreamContent(new MemoryStream(Encoding.UTF8.GetBytes(this.encryption.Decrypt(this.request.Content.ReadAsStringAsync().Result))));

            this.streamContent = new DelegatingStreamContent(this.encryption.Decrypt(buffer, this.request.Content.Headers.ContentEncoding));

            /*copy*/
            this.Headers.ContentType = request.Content.Headers.ContentType;
            this.Headers.ContentRange = request.Content.Headers.ContentRange;
            this.Headers.Expires = request.Content.Headers.Expires;
            this.Headers.LastModified = request.Content.Headers.LastModified;
            this.Headers.ContentMD5 = request.Content.Headers.ContentMD5;
            this.Headers.ContentLocation = request.Content.Headers.ContentLocation;
            this.Headers.ContentLength = request.Content.Headers.ContentLength;
            this.Headers.ContentDisposition = request.Content.Headers.ContentDisposition;

            if (request.Content.Headers.Allow.Any())
                Array.ForEach(request.Content.Headers.Allow.ToArray(), o => { if (!this.Headers.Allow.Contains(o)) this.Headers.Allow.Add(o); });

            if (request.Content.Headers.ContentEncoding.Any())
                Array.ForEach(request.Content.Headers.ContentEncoding.ToArray(), o => { if (!this.Headers.ContentEncoding.Contains(o)) this.Headers.ContentEncoding.Add(o); });

            if (request.Content.Headers.ContentLanguage.Any())
                Array.ForEach(request.Content.Headers.ContentLanguage.ToArray(), o => { if (!this.Headers.ContentLanguage.Contains(o)) this.Headers.ContentLanguage.Add(o); });

            return this;
        }

        /// <summary>
        /// Loads the specified original.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <returns></returns>
        public HttpContent Load(HttpContent original)
        {
            var result = this.request.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(result))
                return request.Content;

            /*copy*/
            //this.Headers.ContentType = original.Headers.ContentType;
            //this.Headers.ContentRange = original.Headers.ContentRange;
            //this.Headers.Expires = original.Headers.Expires;
            //this.Headers.LastModified = original.Headers.LastModified;
            //this.Headers.ContentMD5 = original.Headers.ContentMD5;
            //this.Headers.ContentLocation = original.Headers.ContentLocation;
            //this.Headers.ContentLength = original.Headers.ContentLength;
            //this.Headers.ContentDisposition = original.Headers.ContentDisposition;

            //if (original.Headers.Allow.Any())
            //    Array.ForEach(original.Headers.Allow.ToArray(), o => { if (!this.Headers.Allow.Contains(o)) this.Headers.Allow.Add(o); });

            //if (original.Headers.ContentEncoding.Any())
            //    Array.ForEach(original.Headers.ContentEncoding.ToArray(), o => { if (!this.Headers.ContentEncoding.Contains(o)) this.Headers.ContentEncoding.Add(o); });

            //if (original.Headers.ContentLanguage.Any())
            //    Array.ForEach(original.Headers.ContentLanguage.ToArray(), o => { if (!this.Headers.ContentLanguage.Contains(o)) this.Headers.ContentLanguage.Add(o); });

            this.streamContent = new DelegatingStreamContent(this.encryption.Encrypt(result, this.request.Content.Headers.ContentEncoding));
            //this.streamContent = new DelegatingStreamContent(new MemoryStream(Encoding.UTF8.GetBytes(this.encryption.Encrypt(result))));
            return this;
        }

        #endregion load

        #region HttpContent

        /// <summary>
        /// Serializes to stream asynchronous.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return this.streamContent.WriteToStreamAsync(stream, context);
        }

        /// <summary>
        /// Tries the length of the compute.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        protected override bool TryComputeLength(out long length)
        {
            return this.streamContent.TryCalculateLength(out length);
        }

        #endregion HttpContent
    }
}