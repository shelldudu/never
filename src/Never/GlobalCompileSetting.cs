using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never
{
    /// <summary>
    /// 优化配置
    /// </summary>
    public static class GlobalCompileSetting
    {
        /// <summary>
        /// 使用dynamic类型来替换一些hashtable的行为
        /// </summary>
        public static bool DynamicReplaceHashtable { get; set; }
    }
}