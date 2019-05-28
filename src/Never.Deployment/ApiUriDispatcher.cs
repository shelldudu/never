using Never.Logging;
using Never.Net;
using Never.Serialization;
using Never.Utils;
using Never.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Deployment
{
    /// <summary>
    /// 路由Url配置
    /// </summary>
    /// <typeparam name="TApiRouteProvider">The type of the pi route provider.</typeparam>
    public class ApiUriDispatcher<TApiRouteProvider> : IApiUriDispatcher, IApiRouteLogTracker where TApiRouteProvider : IApiRouteProvider
    {
        #region field and ctor

        /// <summary>
        /// 上一次写日志时间
        /// </summary>
        private DateTime LastLoggerTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiUriDispatcher{ApiRouteProvider}"/> class.
        /// </summary>
        public ApiUriDispatcher(TApiRouteProvider apiRouteProvider, IA10HealthReport a10HealthReport)
        {
            this.ApiRouteProvider = apiRouteProvider;
            this.A10HealthReport = a10HealthReport;
            this.LastLoggerTime = DateTime.Now;
        }

        #endregion ctor

        /// <summary>
        /// 路由提供者
        /// </summary>
        public TApiRouteProvider ApiRouteProvider { get; }

        /// <summary>
        /// A10活动报告
        /// </summary>
        public IA10HealthReport A10HealthReport { get; }

        /// <summary>
        /// 日志信息
        /// </summary>
        public Func<ILoggerBuilder> LoggerBuilder { get; set; }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="source"></param>
        /// <param name="match"></param>
        public virtual Task Write(IEnumerable<IApiUriHealthElement> source, IEnumerable<IApiUriHealthElement> match)
        {
            if (this.LoggerBuilder == null)
                return Task.CompletedTask;

            if (source.IsNullOrEmpty() && match.IsNullOrEmpty())
                return Task.CompletedTask;

            if (source.Count() == match.Count())
                return Task.CompletedTask;

            if (this.LastLoggerTime >= DateTime.Now)
                return Task.CompletedTask;

            this.LastLoggerTime = DateTime.Now.AddMinutes(1);
            return Task.Run(() =>
            {
                var logger = this.LoggerBuilder().Build(typeof(ApiUriDispatcher<>));
                if (logger == null)
                    return;

                var sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendFormat("apiroute '{0}' check the servers are stop:", this.ApiRouteProvider.Group);
                foreach (var s in source)
                {
                    if (match.Any(t => t.Id == s.Id))
                        continue;

                    sb.AppendLine();
                    sb.AppendFormat("a10url:{0},text:{1}", s.A10Url, s.ReportText);
                }

                logger.Info(sb.ToString());
                return;
            });
        }

        #region utils

        /// <summary>
        /// 获取routeKey的ascuill总值
        /// </summary>
        /// <param name="routeKey">路由Key</param>
        /// <returns></returns>
        public int SumASCII(string routeKey)
        {
            var index = 0;
            Array.ForEach((routeKey ?? string.Empty).ToArray(), route =>
            {
                index += route;
            });

            return index;
        }

        /// <summary>
        /// 获取routeKey的ascuill总值
        /// </summary>
        /// <param name="routeKey">路由Key</param>
        /// <returns></returns>
        public int SumASCII(long routeKey)
        {
            var index = 0;
            Array.ForEach(Math.Abs(routeKey).ToString().ToArray(), route =>
            {
                index += route;
            });

            return index;
        }

        /// <summary>
        /// 获取routeKey的ascuill总值
        /// </summary>
        /// <param name="uniqueId">路由Key</param>
        /// <returns></returns>
        public int SumASCII(Guid uniqueId)
        {
            var index = 0;
            Array.ForEach(uniqueId.ToString().ToArray(), route =>
            {
                index += route;
            });

            return index;
        }

        /// <summary>
        /// 获取routeKey的ascuill总值
        /// </summary>
        /// <param name="routeKey">路由Key</param>
        /// <returns></returns>
        public int SumASCII<TKey>(TKey routeKey)
        {
            return this.SumASCII(routeKey.ToString());
        }

        /// <summary>
        /// 获取当前活动的ApiUrl
        /// </summary>
        /// <param name="selectKey">api路由主键</param>
        /// <returns></returns>
        [Never.Attributes.Summary(Descn = "根据路由主健返回当前活动中的api，如果api都没有活动，则返回第一个Api")]
        public string GetCurrentUrlHost(string selectKey)
        {
            return this.GetCurrentApiUrlHost(new StringRoutePrimaryKeySelect() { PrimaryKey = selectKey }, this.ApiRouteProvider.A10ContentMatch);
        }

        /// <summary>
        /// 获取Url，如果该组的资源在脱机状态下，则将该次索引指向下一组资源中
        /// </summary>
        /// <param name="routePrimaryKey">api路由主键</param>
        /// <returns></returns>
        public string GetCurrentUrlHost(IRoutePrimaryKeySelect routePrimaryKey)
        {
            return this.GetCurrentApiUrlHost(routePrimaryKey, this.ApiRouteProvider.A10ContentMatch);
        }

        /// <summary>
        /// 获取Url，如果该组的资源在脱机状态下，则将该次索引指向下一组资源中
        /// </summary>
        /// <param name="routePrimaryKey">api路由主键</param>
        /// <param name="matchA10Content">匹配A10请求回来的内容</param>
        /// <returns></returns>
        private string GetCurrentApiUrlHost(IRoutePrimaryKeySelect routePrimaryKey, System.Func<string, bool> matchA10Content)
        {
            var temp = this.A10HealthReport.GetReport(this.ApiRouteProvider.Group);
            if (temp.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var matchs = this.Select(temp, matchA10Content);
            if (matchs.IsNullOrEmpty())
                return null;

            var index = this.SumASCII(routePrimaryKey == null ? Randomizer.Next(matchs.Count()).ToString() : routePrimaryKey.PrimaryKey) % matchs.Count();
            return matchs[index].ApiUri;
        }

        /// <summary>
        /// 匹配url，默认至少返回一条记录，即使所有工作不正常
        /// </summary>
        /// <param name="source">url健康信息</param>
        /// <param name="matchA10Content">对source参数每一个元素最后报告的内容进行匹配，如果是相同，则返回可进行请求的url结果中</param>
        /// <returns></returns>
        protected virtual IApiUriHealthElement[] Select(IEnumerable<IApiUriHealthElement> source, System.Func<string, bool> matchA10Content)
        {
            var matchs = (from n in source where matchA10Content.Invoke(n.ReportText) orderby n.Id select n).ToArray();
            if (matchs.IsNullOrEmpty())
            {
                this.Write(source, Enumerable.Empty<IApiUriHealthElement>());

                /*表示可能都不能正常工作，则取temp的第一个*/
                return new[] { source.OrderBy(o => o.Id).FirstOrDefault() };
            }

            //写日志
            this.Write(source, matchs);

            return matchs;
        }

        /// <summary>
        /// 合成url
        /// </summary>
        /// <param name="routePrimaryKey"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public virtual UrlConcat ConcatApiUrl(string routePrimaryKey, string route)
        {
            return new UrlConcat() { Host = this.GetCurrentUrlHost(routePrimaryKey), Route = route, Option = UrlConcatOption.Concat };
        }

        /// <summary>
        /// 合成Url
        /// </summary>
        /// <param name="request"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public virtual UrlConcat ConcatApiUrl(IRoutePrimaryKeySelect request, string route)
        {
            return new UrlConcat() { Host = this.GetCurrentUrlHost(request), Route = route, Option = UrlConcatOption.Concat };
        }

        /// <summary>
        /// 以Post方式获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="request">参数请求</param>
        /// <param name="jsonDate">json请求内容</param>
        /// <param name="route">路由标识</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        public T Post<T>(IJsonSerializer jsonSerializer, IRoutePrimaryKeySelect request, string route, string jsonDate, IDictionary<string, string> headerParams, int timeout = 30000)
        {
            return this.Post<T>(jsonSerializer, this.ConcatApiUrl(request, route), jsonDate, headerParams, timeout);
        }

        /// <summary>
        /// 以Post方式发送请求
        /// </summary>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="request">参数请求</param>
        /// <param name="jsonDate">json请求内容</param>
        /// <param name="route">路由标识</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        public void Post(IJsonSerializer jsonSerializer, IRoutePrimaryKeySelect request, string route, string jsonDate, IDictionary<string, string> headerParams, int timeout = 30000)
        {
            this.Post(jsonSerializer, this.ConcatApiUrl(request, route), jsonDate, headerParams);
        }

        /// <summary>
        /// 以Post方式获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="url">参数请求</param>
        /// <param name="jsonDate">json请求内容</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        public virtual T Post<T>(IJsonSerializer jsonSerializer, UrlConcat url, string jsonDate, IDictionary<string, string> headerParams, int timeout = 30000)
        {
            var txt = new WebRequestDownloader().PostString(url.ToString(), jsonDate, headerParams, timeout);
            if (txt.IsNullOrEmpty())
                return default(T);

            return jsonSerializer.Deserialize<T>(txt);
        }

        /// <summary>
        /// 以Post方式获取数据
        /// </summary>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="url">参数请求</param>
        /// <param name="jsonDate">json请求内容</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        public virtual void Post(IJsonSerializer jsonSerializer, UrlConcat url, string jsonDate, IDictionary<string, string> headerParams, int timeout = 30000)
        {
            new WebRequestDownloader().PostString(url.ToString(), jsonDate, headerParams, timeout);
        }

        /// <summary>
        /// 以Get方式获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="request">参数请求</param>
        /// <param name="requestPara">请求参数集合.</param>
        /// <param name="route">路由标识</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        public T Get<T>(IJsonSerializer jsonSerializer, IRoutePrimaryKeySelect request, string route, IDictionary<string, string> requestPara, IDictionary<string, string> headerParams, int timeout = 30000)
        {
            return this.Get<T>(jsonSerializer, this.ConcatApiUrl(request, route), requestPara, headerParams, timeout);
        }

        /// <summary>
        /// 以Get方式发送请求
        /// </summary>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="request">参数请求</param>
        /// <param name="requestPara">请求参数集合.</param>
        /// <param name="route">路由标识</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        public void Get(IJsonSerializer jsonSerializer, IRoutePrimaryKeySelect request, string route, IDictionary<string, string> requestPara, IDictionary<string, string> headerParams, int timeout = 30000)
        {
            this.Get(jsonSerializer, this.ConcatApiUrl(request, route), requestPara, headerParams, timeout);
        }

        /// <summary>
        /// 以Get方式获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="url">请求url</param>
        /// <param name="requestPara">请求参数集合.</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        public virtual T Get<T>(IJsonSerializer jsonSerializer, UrlConcat url, IDictionary<string, string> requestPara, IDictionary<string, string> headerParams, int timeout = 30000)
        {
            var txt = new WebRequestDownloader().GetString(url.ToString(), requestPara, headerParams, ContentType.Json, timeout);
            if (txt.IsNullOrEmpty())
                return default(T);

            return jsonSerializer.Deserialize<T>(txt);
        }

        /// <summary>
        /// 以Get方式获取数据
        /// </summary>
        /// <param name="jsonSerializer">json序列化</param>
        /// <param name="url">请求url</param>
        /// <param name="requestPara">请求参数集合.</param>
        /// <param name="headerParams">放在header参数.</param>
        /// <param name="timeout">过期时间，以毫秒为单位</param>
        /// <returns></returns>
        public virtual void Get(IJsonSerializer jsonSerializer, UrlConcat url, IDictionary<string, string> requestPara, IDictionary<string, string> headerParams, int timeout = 30000)
        {
            new WebRequestDownloader().GetString(url.ToString(), requestPara, headerParams, ContentType.Json, timeout);
        }

        #endregion utils
    }
}