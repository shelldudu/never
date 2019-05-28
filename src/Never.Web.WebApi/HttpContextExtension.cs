

using Never;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Never.Web.WebApi
{
    /// <summary>
    /// Htpp请求相关帮助
    /// </summary>
    public static class HttpContextExtension
    {
#if !NET461

        /// <summary>
        /// 获取Htpp请求中的IP地址
        /// </summary>
        /// <param name="context">Http请求</param>
        /// <returns></returns>
        public static string GetContextIP(this Microsoft.AspNetCore.Http.HttpContext context)
        {
            return GetContextIP(context.Request) ?? context.Connection.RemoteIpAddress.ToString();
        }

        /// <summary>
        /// 获取Htpp请求中的IP地址
        /// </summary>
        /// <param name="httpRequest">Http请求</param>
        /// <returns></returns>
        public static string GetContextIP(this Microsoft.AspNetCore.Http.HttpRequest httpRequest)
        {
            if (httpRequest == null || httpRequest.Headers.IsNullOrEmpty())
                return string.Empty;

            string stream = httpRequest.Headers.ContainsKey("HTTP_X_FORWARDED_FOR") ? httpRequest.Headers["HTTP_X_FORWARDED_FOR"].FirstOrDefault() : null;
            if (string.IsNullOrEmpty(stream))
                stream = httpRequest.Headers.ContainsKey("REMOTE_ADDR") ? httpRequest.Headers["REMOTE_ADDR"].FirstOrDefault() : null;

            if (string.IsNullOrEmpty(stream))
                return string.Empty;

            var ip = stream.Split(new[] { ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries).ElementAt(0);
            return ObjectExtension.IsIP(ip) ? ip : string.Empty;
        }

#else

        /// <summary>
        /// 获取Htpp请求中的IP地址
        /// </summary>
        /// <param name="context">Http请求</param>
        /// <returns></returns>
        public static string GetContextIP(this System.Web.HttpContext context)
        {
            return GetContextIP(context.Request);
        }

        /// <summary>
        /// 获取Htpp请求中的IP地址
        /// </summary>
        /// <param name="context">Http请求</param>
        /// <returns></returns>
        public static string GetContextIP(this System.Web.HttpContextBase context)
        {
            return GetContextIP(context.Request);
        }

        /// <summary>
        /// 获取Htpp请求中的IP地址
        /// </summary>
        /// <param name="httpRequest">Http请求</param>
        /// <returns></returns>
        public static string GetContextIP(this System.Web.HttpRequest httpRequest)
        {
            if (httpRequest == null || httpRequest.ServerVariables == null)
                return string.Empty;

            string stream = httpRequest.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(stream))
                stream = httpRequest.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(stream))
                stream = httpRequest.UserHostAddress;

            if (string.IsNullOrEmpty(stream))
                return string.Empty;

            var ip = stream.Split(new[] { ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries).ElementAt(0);
            return ObjectExtension.IsIP(ip) ? ip : string.Empty;
        }

        /// <summary>
        /// 获取Htpp请求中的IP地址
        /// </summary>
        /// <param name="httpRequest">Http请求</param>
        /// <returns></returns>
        public static string GetContextIP(this System.Web.HttpRequestBase httpRequest)
        {
            if (httpRequest == null || httpRequest.ServerVariables == null)
                return string.Empty;

            string stream = httpRequest.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(stream))
                stream = httpRequest.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(stream))
                stream = httpRequest.UserHostAddress;

            if (string.IsNullOrEmpty(stream))
                return string.Empty;

            var ip = stream.Split(new[] { ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries).ElementAt(0);
            return ObjectExtension.IsIP(ip) ? ip : string.Empty;
        }

#endif
    }
}
