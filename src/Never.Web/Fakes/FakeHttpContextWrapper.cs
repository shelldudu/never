#if !NET461
#else

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace Never.Web.Fakes
{
    /// <summary>
    /// 伪造httpcontext请求
    /// </summary>
    public class FakeHttpContextWrapper : System.Web.HttpContextBase
    {
        #region field

        /// <summary>
        /// The principal
        /// </summary>
        private IPrincipal principal = null;

        /// <summary>
        /// The item
        /// </summary>
        private readonly IDictionary item = null;

        /// <summary>
        /// The cookies
        /// </summary>
        private readonly HttpCookieCollection cookies;

        /// <summary>
        /// The form parameters
        /// </summary>
        private readonly NameValueCollection formParams;

        /// <summary>
        /// The query string parameters
        /// </summary>
        private readonly NameValueCollection queryStringParams;

        /// <summary>
        /// The headers
        /// </summary>
        private readonly NameValueCollection headers;

        /// <summary>
        /// The server variables
        /// </summary>
        private readonly NameValueCollection serverVariables;

        /// <summary>
        /// The URL
        /// </summary>
        private readonly Uri url;

        /// <summary>
        /// The URL referrer
        /// </summary>
        private readonly Uri urlReferrer;

        /// <summary>
        /// The relative URL
        /// </summary>
        private readonly string relativeUrl = null;

        /// <summary>
        /// The method
        /// </summary>
        private readonly string method = null;

        /// <summary>
        /// The root
        /// </summary>
        private static readonly FakeHttpContextWrapper root = new FakeHttpContextWrapper("~/");

        /// <summary>
        /// The request
        /// </summary>
        private readonly HttpRequestBase request = null;

        /// <summary>
        /// The response
        /// </summary>
        private readonly HttpResponseBase response = null;

        /// <summary>
        /// The session
        /// </summary>
        private readonly HttpSessionStateBase session = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Roots this instance.
        /// </summary>
        /// <returns></returns>
        public static FakeHttpContextWrapper Root
        {
            get
            {
                return root;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeHttpContextWrapper"/> class.
        /// </summary>
        /// <param name="relativeUrl">The relative URL.</param>
        public FakeHttpContextWrapper(string relativeUrl)
            : this(relativeUrl, null, null, null, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeHttpContextWrapper"/> class.
        /// </summary>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <param name="principal">The principal.</param>
        /// <param name="cookies">The cookies.</param>
        /// <param name="formParams">The form parameters.</param>
        /// <param name="queryStringParams">The query string parameters.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="serverVariables">The server variables.</param>
        public FakeHttpContextWrapper(string relativeUrl,
            IPrincipal principal,
            HttpCookieCollection cookies,
            NameValueCollection formParams,
            NameValueCollection queryStringParams,
            NameValueCollection headers,
            NameValueCollection serverVariables)
        {
            this.item = new Dictionary<object, object>();
            this.relativeUrl = relativeUrl;
            this.method = "HttpGet";
            this.cookies = cookies ?? new HttpCookieCollection();
            this.formParams = formParams ?? new NameValueCollection();
            this.queryStringParams = queryStringParams ?? new NameValueCollection();
            this.headers = headers ?? new NameValueCollection();
            this.serverVariables = serverVariables ?? new NameValueCollection();
            this.principal = principal ?? new Security.UserPrincipal(new Security.UserIdentity(null));
            this.url = new Uri(relativeUrl);
            this.urlReferrer = null;
            this.request = new FakeHttpRequestWrapper(this.relativeUrl, this.method, this.cookies, this.formParams, this.queryStringParams, this.headers, this.serverVariables, this.url, this.urlReferrer);
            this.response = new FakeHttpResponseWrapper(this.cookies);
            this.session = new FakeHttpSessionStateWrapper();
        }

        #endregion ctor

        #region members

        /// <summary>
        /// Gets the relative URL.
        /// </summary>
        public string RelativeUrl
        {
            get
            {
                return this.relativeUrl;
            }
        }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>
        /// The request.
        /// </value>
        public override HttpRequestBase Request
        {
            get
            {
                return request;
            }
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public override HttpResponseBase Response
        {
            get
            {
                return response;
            }
        }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public override IPrincipal User
        {
            get
            {
                return principal;
            }

            set
            {
                principal = value;
            }
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public override HttpSessionStateBase Session
        {
            get
            {
                return this.session;
            }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public override IDictionary Items
        {
            get
            {
                return item;
            }
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public override object GetService(Type serviceType)
        {
            object @object = null;
            Never.IoC.ContainerContext.Current.ServiceLocator.TryResolve(serviceType, ref @object);
            return @object;
        }

        #endregion members
    }
}

#endif