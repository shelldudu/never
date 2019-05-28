#if !NET461
#else

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Never.Web.Fakes
{
    /// <summary>
    ///
    /// </summary>
    public class FakeHttpRequestWrapper : HttpRequestBase
    {
        #region field

        private readonly HttpCookieCollection cookies;
        private readonly NameValueCollection formParams;
        private readonly NameValueCollection queryStringParams;
        private readonly NameValueCollection headers;
        private readonly NameValueCollection serverVariables;
        private readonly Uri url;
        private readonly Uri urlReferrer;
        private readonly string relativeUrl = null;
        private readonly string method = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeHttpRequestWrapper"/> class.
        /// </summary>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="cookies">The cookies.</param>
        /// <param name="formParams">The form parameters.</param>
        /// <param name="queryStringParams">The query string parameters.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="serverVariables">The server variables.</param>
        /// <param name="url">The URL.</param>
        /// <param name="urlReferrer">The URL referrer.</param>
        public FakeHttpRequestWrapper(string relativeUrl,
            string method,
            HttpCookieCollection cookies,
            NameValueCollection formParams,
            NameValueCollection queryStringParams,
            NameValueCollection headers,
            NameValueCollection serverVariables,
            Uri url,
            Uri urlReferrer)
        {
            this.relativeUrl = relativeUrl;
            this.method = method;
            this.cookies = cookies ?? new HttpCookieCollection();
            this.formParams = formParams ?? new NameValueCollection();
            this.queryStringParams = queryStringParams ?? new NameValueCollection();
            this.headers = headers ?? new NameValueCollection();
            this.serverVariables = serverVariables ?? new NameValueCollection();
            this.url = url;
            this.urlReferrer = urlReferrer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeHttpRequestWrapper"/> class.
        /// </summary>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <param name="url">The URL.</param>
        /// <param name="urlReferrer">The URL referrer.</param>
        public FakeHttpRequestWrapper(string relativeUrl, Uri url, Uri urlReferrer)
            : this(relativeUrl, "HttpGet", new HttpCookieCollection(), new NameValueCollection(), new NameValueCollection(), new NameValueCollection(), new NameValueCollection(), url, urlReferrer)
        {
        }

        #endregion ctor

        #region

        /// <summary>
        /// Gets the HTTP method.
        /// </summary>
        /// <value>
        /// The HTTP method.
        /// </value>
        public override string HttpMethod
        {
            get
            {
                return this.method;
            }
        }

        /// <summary>
        /// Gets the server variables.
        /// </summary>
        /// <value>
        /// The server variables.
        /// </value>
        public override NameValueCollection ServerVariables
        {
            get
            {
                return this.serverVariables;
            }
        }

        /// <summary>
        /// Gets the form.
        /// </summary>
        /// <value>
        /// The form.
        /// </value>
        public override NameValueCollection Form
        {
            get
            {
                return this.formParams;
            }
        }

        /// <summary>
        /// Gets the query string.
        /// </summary>
        /// <value>
        /// The query string.
        /// </value>
        public override NameValueCollection QueryString
        {
            get
            {
                return this.queryStringParams;
            }
        }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        public override NameValueCollection Headers
        {
            get
            {
                return this.headers;
            }
        }

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
                return this.cookies;
            }
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public override Uri Url
        {
            get
            {
                return this.url;
            }
        }

        /// <summary>
        /// Gets the URL referrer.
        /// </summary>
        /// <value>
        /// The URL referrer.
        /// </value>
        public override Uri UrlReferrer
        {
            get
            {
                return this.urlReferrer;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is secure connection.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is secure connection; otherwise, <c>false</c>.
        /// </value>
        public override bool IsSecureConnection
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is authenticated.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is authenticated; otherwise, <c>false</c>.
        /// </value>
        public override bool IsAuthenticated
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the raw URL.
        /// </summary>
        /// <value>
        /// The raw URL.
        /// </value>
        public override string RawUrl
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the user host address.
        /// </summary>
        /// <value>
        /// The user host address.
        /// </value>
        public override string UserHostAddress
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion
    }
}

#endif