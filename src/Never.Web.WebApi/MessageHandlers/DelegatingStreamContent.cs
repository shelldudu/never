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
    /// 对内容流进行授权阅读
    /// </summary>
    public class DelegatingStreamContent : StreamContent
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatingStreamContent"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public DelegatingStreamContent(Stream stream)
            : base(stream)
        {
        }

        #endregion ctor

        #region streamContent

        /// <summary>
        /// Writes to stream asynchronous.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task WriteToStreamAsync(Stream stream, TransportContext context)
        {
            return this.SerializeToStreamAsync(stream, context);
        }

        /// <summary>
        /// Tries the length of the calculate.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public bool TryCalculateLength(out long length)
        {
            return this.TryComputeLength(out length);
        }

        /// <summary>
        /// Gets the content read stream asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task<Stream> GetContentReadStreamAsync()
        {
            return this.CreateContentReadStreamAsync();
        }

        #endregion streamContent
    }
}