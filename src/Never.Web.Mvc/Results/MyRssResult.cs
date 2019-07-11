#if !NET461
#else

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;

namespace Never.Web.Mvc.Results
{
    /// <summary>
    /// RssResult
    /// </summary>
    public class MyRssResult : ActionResult, IHttpActionResult
    {
        #region property

        /// <summary>
        /// 序列化对象
        /// </summary>
        public SyndicationFeed Feed { get; set; }

        #endregion property

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="feed"></param>
        public MyRssResult(SyndicationFeed feed)
        {
            this.Feed = feed;
        }

        #endregion ctor

        #region 重新渲染

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/rss+xml";
            var rssFormatter = new Rss20FeedFormatter(Feed);
            using (var writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                rssFormatter.WriteTo(writer);
            }
        }

        #endregion 重新渲染
    }
}

#endif