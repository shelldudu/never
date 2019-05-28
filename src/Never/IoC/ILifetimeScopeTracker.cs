using Never.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.IoC
{
    /// <summary>
    /// 组件生命范围定义跟踪者
    /// </summary>
    public interface ILifetimeScopeTracker
    {
        /// <summary>
        /// 开始一个范围
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        ILifetimeScope StartScope(ILifetimeScope parent);

        /// <summary>
        /// 清空所有范围
        /// </summary>
        void CleanScope();
    }
}