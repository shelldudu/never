using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration.ConfigCenter
{
    /// <summary>
    /// 关键字查找者
    /// </summary>
    public interface ICustomKeyValueFinder
    {
        /// <summary>
        /// 查询关键字
        /// </summary>
        /// <param name="mode">eat还是cat</param>
        /// <param name="key">@user参数</param>
        string Find(string mode, string key);
    }
}
