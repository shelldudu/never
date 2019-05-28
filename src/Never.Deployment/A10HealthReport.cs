using Never.IoC;
using Never.Logging;
using Never.Web;
using Never.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Never.Deployment
{
    /// <summary>
    /// 检查A10信息
    /// </summary>
    internal class A10HealthReport : IA10HealthReport
    {
        #region group

        private List<ApiUriHealthElement> collection = null;
        private List<IApiRouteProvider> providers = null;
        private readonly System.Threading.Timer timer = null;
        private bool isWorking = false;
        #endregion group

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="A10HealthReport"/> class.
        /// </summary>
        public A10HealthReport()
        {
            this.collection = new List<ApiUriHealthElement>();
            this.providers = new List<IApiRouteProvider>();
            this.timer = new System.Threading.Timer(Change, this, TimeSpan.FromMilliseconds(-1), TimeSpan.FromSeconds(60));
        }

        #endregion ctor

        #region repleate

        protected void Change(object state)
        {
            System.Threading.Interlocked.Exchange(ref this.collection, this.Load(false));
            if (this.collection.Count == 0)
                return;

            for (var i = 0; i < this.collection.Count; i++)
            {
                var item = this.collection.ElementAt(i);
                string txt = string.Empty;
                try
                {
                    if (item.A10Url.IsNullOrEmpty())
                    {
                        //表示当前不可用
                        item.ReportText = string.Empty;
                        item.ReportTime = DateTime.Now;
                    }
                    else
                    {
                        txt = new WebRequestDownloader().GetString(item.A10Url, null, null, ContentType.Default, 0);
                        item.ReportText = txt;
                        item.ReportTime = DateTime.Now;
                    }
                }
                catch
                {
                    //表示当前不可用
                    item.ReportText = string.Empty;
                    item.ReportTime = DateTime.Now;
                }
            }

            /*认为没有完成，2分钟后继续*/
            return;
        }

        #endregion repleate

        #region get group

        /// <summary>
        /// 开始工作
        /// </summary>
        /// <param name="intervalSecond">每个A10检查的间隔，以秒为单位</param>
        /// <param name="providers"></param>
        /// <param name="notRefresh">是否不刷新URL去检查内容（即默认表示可用），ture表示停掉了这个timer</param>
        public IA10HealthReport Startup(int intervalSecond, IEnumerable<IApiRouteProvider> providers, bool notRefresh = false)
        {
            if (this.isWorking)
                return this;

            if (providers == null || !providers.Any())
                return this;

            this.providers.Clear();
            this.providers.AddRange(providers);
            System.Threading.Interlocked.Exchange(ref this.collection, this.Load(notRefresh));
            if (notRefresh)
            {
                this.timer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromSeconds(intervalSecond));
            }
            else
            {
                this.timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(intervalSecond));
            }

            return this;
        }

        /// <summary>
        /// 替换提供者
        /// </summary>
        /// <param name="providers"></param>
        /// <param name="intervalSecond">每个A10检查的间隔，以秒为单位</param>
        /// <param name="notRefresh">是否不刷新URL（即默认表示可用），ture表示停掉了这个timer</param>
        public IA10HealthReport Replace(int intervalSecond, IEnumerable<IApiRouteProvider> providers, bool notRefresh = false)
        {
            if (!this.isWorking)
                return this;

            this.providers.Clear();
            if (providers == null || !providers.Any())
                return this;

            this.providers.AddRange(providers);
            System.Threading.Interlocked.Exchange(ref this.collection, this.Load(notRefresh));
            if (notRefresh)
            {
                this.timer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromSeconds(intervalSecond));
            }
            else
            {
                this.timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(intervalSecond));
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notRefresh">是否不刷新URL（即默认表示可用），ture表示停掉了这个timer</param>
        private List<ApiUriHealthElement> Load(bool notRefresh)
        {
            if (this.providers == null || !this.providers.Any())
                return new List<ApiUriHealthElement>();

            var collection = new List<ApiUriHealthElement>(this.providers.Count * 2);
            for (var i = 0; i < this.providers.Count; i++)
            {
                var provider = this.providers[i];
                if (provider == null)
                    continue;

                var elements = provider.ApiUrlA10Elements.ToArray();
                for (var j = 0; j < elements.Length; j++)
                {
                    var e = elements[j];
                    collection.Add(new ApiUriHealthElement()
                    {
                        Group = provider.Group,
                        ApiUri = e.ApiUrl,
                        A10Url = e.A10Url,
                        ReportTime = DateTime.Now,
                        ReportText = notRefresh ? "work" : null,
                    });
                }
            }

            return collection;
        }

        /// <summary>
        /// 获取最新报告
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns></returns>
        public IEnumerable<IApiUriHealthElement> GetReport(string @group)
        {
            var array = (from n in this.collection where n.Group == @group select n).ToArray();
            return array;
        }

        #endregion get group
    }
}