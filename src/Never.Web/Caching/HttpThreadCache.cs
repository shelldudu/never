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
    /// 当前Http请求线程缓存
    /// </summary>
    public sealed class HttpThreadCache : ThreadContextCache, ICaching, IDisposable
    {
        #region

        /// <summary>
        /// 在httprequestItem中的缓存Key
        /// </summary>
        private const string key = "Caching.HttpRequest";

        #endregion

        #region

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpThreadCache"/> class.
        /// </summary>
        public HttpThreadCache()
            : base(GetCurrenDictionary())
        {
        }

        #endregion

        /// <summary>
        /// 获取对象缓存字典
        /// </summary>
        /// <returns></returns>
        private static IDictionary GetCurrenDictionary()
        {
#if !NET461
            return CurrentThreadLocal.Value;
#else
            if (HttpContext.Current == null)
                return CurrentThreadLocal.Value;

            if (HttpContext.Current.Items.Contains(key))
                return System.Web.HttpContext.Current.Items[key] as Hashtable;

            var result = new Hashtable();
            HttpContext.Current.Items[key] = result;

            return result;
#endif
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