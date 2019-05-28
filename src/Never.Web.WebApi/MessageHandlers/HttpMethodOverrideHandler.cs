using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Never.Web.WebApi.MessageHandlers
{
    /// <summary>
    /// 将HttpMethod方法更新为用Headers中定义了【X-HttpMethod-Override】标头的HttpMethod
    /// </summary>
    public class HttpMethodOverrideHandler : DelegatingHandler
    {
        #region field

        /// <summary>
        /// The key in header to remark
        /// </summary>
        protected readonly string keyInHeaderToRemark;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMethodOverrideHandler"/> class.
        /// </summary>
        /// <param name="keyInHeaderToRemark">The key in header to remark.</param>
        public HttpMethodOverrideHandler(string keyInHeaderToRemark)
        {
            this.keyInHeaderToRemark = keyInHeaderToRemark;
        }

        #endregion ctor

        /// <summary>
        /// Sends the asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (keyInHeaderToRemark.IsNullOrEmpty())
                return base.SendAsync(request, cancellationToken);

            IEnumerable<string> methodOverrideHeader;
            if (request.Headers.TryGetValues(keyInHeaderToRemark, out methodOverrideHeader) && methodOverrideHeader.Any())
                request.Method = new HttpMethod(methodOverrideHeader.First());

            return base.SendAsync(request, cancellationToken);
        }
    }
}