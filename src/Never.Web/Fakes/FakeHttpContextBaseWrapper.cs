#if !NET461
# else

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Profile;
using System.Web.SessionState;

namespace Never.Web.Fakes
{
    /// <summary>
    ///
    /// </summary>
    public class FakeHttpContextBaseWrapper : HttpContextBase
    {
        #region field

        private readonly HttpContextBase httpContextBase = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeHttpContextBaseWrapper"/> class.
        /// </summary>
        public FakeHttpContextBaseWrapper()
        {
            if (HttpContext.Current == null)
                httpContextBase = new FakeHttpContextBaseWrapper();
            else
                httpContextBase = new System.Web.HttpContextWrapper(HttpContext.Current);
        }

        #endregion ctor

        #region member

        /// <summary>
        /// Gets all errors.
        /// </summary>
        /// <value>
        /// All errors.
        /// </value>
        public override Exception[] AllErrors
        {
            get
            {
                return this.httpContextBase.AllErrors;
            }
        }

        /// <summary>
        /// Gets the application.
        /// </summary>
        /// <value>
        /// The application.
        /// </value>
        public override HttpApplicationStateBase Application
        {
            get
            {
                return this.httpContextBase.Application;
            }
        }

        /// <summary>
        /// Gets or sets the application instance.
        /// </summary>
        /// <value>
        /// The application instance.
        /// </value>
        public override HttpApplication ApplicationInstance
        {
            get
            {
                return this.httpContextBase.ApplicationInstance;
            }

            set
            {
                this.httpContextBase.ApplicationInstance = value;
            }
        }

        /// <summary>
        /// Adds the error.
        /// </summary>
        /// <param name="errorInfo">The error information.</param>
        public override void AddError(Exception errorInfo)
        {
            this.httpContextBase.AddError(errorInfo);
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <value>
        /// The cache.
        /// </value>
        public override Cache Cache
        {
            get
            {
                return this.httpContextBase.Cache;
            }
        }

        /// <summary>
        /// Clears the error.
        /// </summary>
        public override void ClearError()
        {
            this.httpContextBase.ClearError();
        }

        /// <summary>
        /// Gets the current handler.
        /// </summary>
        /// <value>
        /// The current handler.
        /// </value>
        public override IHttpHandler CurrentHandler
        {
            get
            {
                return this.httpContextBase.CurrentHandler;
            }
        }

        /// <summary>
        /// Gets the current notification.
        /// </summary>
        /// <value>
        /// The current notification.
        /// </value>
        public override RequestNotification CurrentNotification
        {
            get
            {
                return this.httpContextBase.CurrentNotification;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.httpContextBase.Equals(obj);
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public override Exception Error
        {
            get
            {
                return this.httpContextBase.Error;
            }
        }

        /// <summary>
        /// Gets the global resource object.
        /// </summary>
        /// <param name="classKey">The class key.</param>
        /// <param name="resourceKey">The resource key.</param>
        /// <returns></returns>
        public override object GetGlobalResourceObject(string classKey, string resourceKey)
        {
            return this.httpContextBase.GetGlobalResourceObject(classKey, resourceKey);
        }

        /// <summary>
        /// Gets the global resource object.
        /// </summary>
        /// <param name="classKey">The class key.</param>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public override object GetGlobalResourceObject(string classKey, string resourceKey, CultureInfo culture)
        {
            return this.httpContextBase.GetGlobalResourceObject(classKey, resourceKey, culture);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return this.httpContextBase.GetHashCode();
        }

        /// <summary>
        /// Gets the local resource object.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="resourceKey">The resource key.</param>
        /// <returns></returns>
        public override object GetLocalResourceObject(string virtualPath, string resourceKey)
        {
            return this.httpContextBase.GetLocalResourceObject(virtualPath, resourceKey);
        }

        /// <summary>
        /// Gets the local resource object.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public override object GetLocalResourceObject(string virtualPath, string resourceKey, CultureInfo culture)
        {
            return this.httpContextBase.GetLocalResourceObject(virtualPath, resourceKey, culture);
        }

        /// <summary>
        /// Gets the section.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns></returns>
        public override object GetSection(string sectionName)
        {
            return this.httpContextBase.GetSection(sectionName);
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public override object GetService(Type serviceType)
        {
            return this.httpContextBase.GetService(serviceType);
        }

        /// <summary>
        /// Gets or sets the handler.
        /// </summary>
        /// <value>
        /// The handler.
        /// </value>
        public override IHttpHandler Handler
        {
            get
            {
                return this.httpContextBase.Handler;
            }

            set
            {
                this.httpContextBase.Handler = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is custom error enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is custom error enabled; otherwise, <c>false</c>.
        /// </value>
        public override bool IsCustomErrorEnabled
        {
            get
            {
                return this.httpContextBase.IsCustomErrorEnabled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is debugging enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is debugging enabled; otherwise, <c>false</c>.
        /// </value>
        public override bool IsDebuggingEnabled
        {
            get
            {
                return this.httpContextBase.IsDebuggingEnabled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is post notification.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is post notification; otherwise, <c>false</c>.
        /// </value>
        public override bool IsPostNotification
        {
            get
            {
                return this.httpContextBase.IsPostNotification;
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
                return this.httpContextBase.Items;
            }
        }

        /// <summary>
        /// Gets the previous handler.
        /// </summary>
        /// <value>
        /// The previous handler.
        /// </value>
        public override IHttpHandler PreviousHandler
        {
            get
            {
                return this.httpContextBase.PreviousHandler;
            }
        }

        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <value>
        /// The profile.
        /// </value>
        public override ProfileBase Profile
        {
            get
            {
                return this.httpContextBase.Profile;
            }
        }

        /// <summary>
        /// Remaps the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public override void RemapHandler(IHttpHandler handler)
        {
            this.httpContextBase.RemapHandler(handler);
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
                return this.httpContextBase.Request;
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
                return this.httpContextBase.Response;
            }
        }

        /// <summary>
        /// Rewrites the path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="pathInfo">The path information.</param>
        /// <param name="queryString">The query string.</param>
        public override void RewritePath(string filePath, string pathInfo, string queryString)
        {
            this.httpContextBase.RewritePath(filePath, pathInfo, queryString);
        }

        /// <summary>
        /// Rewrites the path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="pathInfo">The path information.</param>
        /// <param name="queryString">The query string.</param>
        /// <param name="setClientFilePath">if set to <c>true</c> [set client file path].</param>
        public override void RewritePath(string filePath, string pathInfo, string queryString, bool setClientFilePath)
        {
            this.httpContextBase.RewritePath(filePath, pathInfo, queryString, setClientFilePath);
        }

        /// <summary>
        /// Rewrites the path.
        /// </summary>
        /// <param name="path">The path.</param>
        public override void RewritePath(string path)
        {
            this.httpContextBase.RewritePath(path);
        }

        /// <summary>
        /// Rewrites the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="rebaseClientPath">if set to <c>true</c> [rebase client path].</param>
        public override void RewritePath(string path, bool rebaseClientPath)
        {
            this.httpContextBase.RewritePath(path, rebaseClientPath);
        }

        /// <summary>
        /// Gets the server.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        public override HttpServerUtilityBase Server
        {
            get
            {
                return this.httpContextBase.Server;
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
                return this.httpContextBase.Session;
            }
        }

        /// <summary>
        /// Sets the session state behavior.
        /// </summary>
        /// <param name="sessionStateBehavior">The session state behavior.</param>
        public override void SetSessionStateBehavior(SessionStateBehavior sessionStateBehavior)
        {
            this.httpContextBase.SetSessionStateBehavior(sessionStateBehavior);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [skip authorization].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [skip authorization]; otherwise, <c>false</c>.
        /// </value>
        public override bool SkipAuthorization
        {
            get
            {
                return this.httpContextBase.SkipAuthorization;
            }

            set
            {
                this.httpContextBase.SkipAuthorization = value;
            }
        }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public override DateTime Timestamp
        {
            get
            {
                return this.httpContextBase.Timestamp;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.httpContextBase.ToString();
        }

        /// <summary>
        /// Gets the trace.
        /// </summary>
        /// <value>
        /// The trace.
        /// </value>
        public override TraceContext Trace
        {
            get
            {
                return this.httpContextBase.Trace;
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
                return this.httpContextBase.User;
            }

            set
            {
                this.httpContextBase.User = value;
            }
        }

        #endregion member
    }
}

#endif