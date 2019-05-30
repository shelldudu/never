#if NET461

using Never.Caching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Never.Web.Caching
{
    /// <summary>
    /// 当前Http请求缓存
    /// </summary>
    public sealed class HttpRequestCache : ContextCache, ICaching, IDisposable
    {
        #region field and ctor

        /// <summary>
        /// 在httprequestItem中的缓存Key
        /// </summary>
        private const string key = "Caching.HttpRequestCache";

        /// <summary>
        /// 
        /// </summary>
        private static readonly Func<IDictionary> init = null;

        static HttpRequestCache()
        {
            init = new Func<IDictionary>(() =>
            {
                if (HttpContext.Current.Items.Contains(key))
                    return System.Web.HttpContext.Current.Items[key] as Hashtable;

                var result = new Hashtable();
                HttpContext.Current.Items[key] = result;

                return result;
            });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContextCache"/> class.
        /// </summary>
        public HttpRequestCache()
            : base(init())
        {
        }

        #endregion


        #region IDisposable成员

        /// <summary>
        /// 释放内部资源
        /// </summary>
        /// <param name="isDispose">是否释放</param>
        protected override void Dispose(bool isDispose)
        {
            base.Dispose(isDispose);
        }

        #endregion IDisposable成员
    }
}

#endif