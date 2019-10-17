using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public class BuildingLableInfo
    {
        /// <summary>
        /// 标签
        /// </summary>
        public ILabel Label { get; set; }

        /// <summary>
        /// 是否已经构建了
        /// </summary>
        public bool Builded { get; set; }
    }
}
