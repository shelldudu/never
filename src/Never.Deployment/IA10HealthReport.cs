using System;
using System.Collections.Generic;

namespace Never.Deployment
{
    /// <summary>
    /// A10接口报告
    /// </summary>
    public interface IA10HealthReport
    {
        /// <summary>
        /// 开始工作
        /// </summary>
        /// <param name="intervalSecond">每个A10检查的间隔，以秒为单位</param>
        /// <param name="providers"></param>
        /// <param name="notRefresh">是否不刷新URL（即默认表示可用），ture表示停掉了这个timer</param>
        IA10HealthReport Startup(int intervalSecond, IEnumerable<IApiRouteProvider> providers, bool notRefresh = false);

        /// <summary>
        /// 替换提供者
        /// </summary>
        /// <param name="intervalSecond">每个A10检查的间隔，以秒为单位</param>
        /// <param name="providers"></param>
        /// <param name="notRefresh">是否不刷新URL（即默认表示可用），ture表示停掉了这个timer</param>
        IA10HealthReport Replace(int intervalSecond, IEnumerable<IApiRouteProvider> providers, bool notRefresh = false);

        /// <summary>
        /// 获取当前组Group的健康信息
        /// </summary>
        /// <param name="group">组信息</param>
        /// <returns></returns>
        IEnumerable<IApiUriHealthElement> GetReport(string @group);
    }
}