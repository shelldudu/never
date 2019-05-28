using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Deployment
{
    /// <summary>
    /// 连接生成
    /// </summary>
    public struct UrlConcat
    {
        /// <summary>
        /// 当前路径
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 当前路由
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// 连接选择
        /// </summary>
        public UrlConcatOption Option { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (this.Option)
            {
                default:
                case UrlConcatOption.Concat:
                    return string.Concat(this.Host, this.Route);
                case UrlConcatOption.Format:
                    return string.Format(this.Host, this.Route);
            }
        }
    }

    /// <summary>
    /// 连接选择
    /// </summary>
    public enum UrlConcatOption
    {
        /// <summary>
        /// concat
        /// </summary>
        Concat = 0,

        /// <summary>
        /// format
        /// </summary>
        Format = 1,
    }
}
