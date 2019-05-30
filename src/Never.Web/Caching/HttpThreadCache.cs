using Never.Caching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Never.Web.Caching
{
    /// <summary>
    /// 当前Http请求线程缓存
    /// </summary>
    public sealed class HttpThreadCache : ContextCache, ICaching, IDisposable
    {
        #region field and ctor

        /// <summary>
        /// 在httprequestItem中的缓存Key
        /// </summary>
        private const string key = "Caching.HttpThreadCache";

        /// <summary>
        /// 
        /// </summary>
        private static readonly Func<IDictionary> init = null;

        /// <summary>
        /// 
        /// </summary>
        private static AsyncLocal<IDictionary> asyncLocak = null;

        static HttpThreadCache()
        {
            asyncLocak = new AsyncLocal<IDictionary>();
            init = new Func<IDictionary>(() =>
            {
#if NET461
                if (HttpContext.Current == null)
                    goto _do;

                if (HttpContext.Current.Items.Contains(key))
                    return System.Web.HttpContext.Current.Items[key] as Hashtable;

                var result = new Hashtable();
                HttpContext.Current.Items[key] = result;

                return result;
#else
                goto _do;
#endif
            _do:
                {
                    if (asyncLocak.Value == null)
                        asyncLocak.Value = new Hashtable();

                    return asyncLocak.Value;
                }
            });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpThreadCache"/> class.
        /// </summary>
        public HttpThreadCache()
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