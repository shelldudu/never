using System;
using System.Collections.Generic;

namespace Never.Deployment
{
    /// <summary>
    /// 路由提供者
    /// </summary>
    public interface IApiRouteProvider
    {
        /// <summary>
        /// 当前组
        /// </summary>
        string Group { get; }

        /// <summary>
        /// url,a10元素
        /// </summary>
        IEnumerable<ApiUrlA10Element> ApiUrlA10Elements { get; }

        /// <summary>
        /// a10内容匹配
        /// </summary>
        Func<string, bool> A10ContentMatch { get; }
    }
}