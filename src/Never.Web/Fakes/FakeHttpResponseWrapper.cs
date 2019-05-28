#if !NET461
#else

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Never.Web.Fakes
{
    /// <summary>
    ///
    /// </summary>
    public sealed class FakeHttpResponseWrapper : HttpResponseBase
    {
        #region field

        private readonly HttpCookieCollection cookies = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeHttpResponseWrapper"/> class.
        /// </summary>
        public FakeHttpResponseWrapper()
            : this(new HttpCookieCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeHttpResponseWrapper"/> class.
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        public FakeHttpResponseWrapper(HttpCookieCollection cookies)
        {
            this.cookies = cookies ?? new HttpCookieCollection();
        }

        #endregion ctor

        /// <summary>
        /// Gets the cookies.
        /// </summary>
        /// <value>
        /// The cookies.
        /// </value>
        public override HttpCookieCollection Cookies
        {
            get
            {
                return this.Cookies;
            }
        }
    }
}

#endif