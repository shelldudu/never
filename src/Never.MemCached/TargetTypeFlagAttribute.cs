using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Memcached
{
    /// <summary>
    /// 类型
    /// </summary>
    public class TargetTypeFlagAttribute : Attribute
    {
        public Type Type { get; set; }
    }
}
