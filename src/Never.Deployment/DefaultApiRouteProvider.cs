using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Deployment
{
    /// <summary>
    /// 路由提供者，使用后会注入<seealso cref="Never.Deployment.ApiUriDispatcher{TApiRouteProvider}"/>实例
    /// </summary>
    /// <seealso cref="Never.Deployment.IApiRouteProvider" />
    public abstract class DefaultApiRouteProvider : IApiRouteProvider
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultApiRouteProvider"/> class.
        /// </summary>
        protected DefaultApiRouteProvider() : this((x) => x != null && x.IndexOf("work") > 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultApiRouteProvider"/> class.
        /// </summary>
        /// <param name="A10ContentMatch">The a10 content match.</param>
        protected DefaultApiRouteProvider(Func<string, bool> A10ContentMatch)
        {
            this.A10ContentMatch = A10ContentMatch ?? ((x) => x != null && x.IndexOf("work") > 0);
            this.Group = NewId.GenerateGuid().ToString();
        }

        #endregion

        #region apirouteProvider

        /// <summary>
        /// a10内容匹配
        /// </summary>
        public Func<string, bool> A10ContentMatch { get; }

        /// <summary>
        /// 唯一键，表示当前组的唯一资源
        /// </summary>
        public virtual string Group { get; }

        /// <summary>
        /// 获取A10资源信息
        /// </summary>
        public abstract IEnumerable<ApiUrlA10Element> ApiUrlA10Elements { get; }

        #endregion apirouteProvider
    }
}
