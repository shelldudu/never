using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Serialization.Json
{
    /// <summary>
    /// 默认配置
    /// </summary>
    public class DefaultSetting
    {
        #region singleton

        /// <summary>
        /// 序列化配置
        /// </summary>
        public static JsonSerializeSetting DefaultSerializeSetting { get; set; }

        /// <summary>
        /// 反序列化配置
        /// </summary>
        public static JsonDeserializeSetting DefaultDeserializeSetting { get; set; }

        /// <summary>
        /// 使用object去序列化时使用的字典
        /// </summary>
        internal static Hashtable DeserializeBuilderTable = null;

        #endregion singleton

        #region ctor

        /// <summary>
        /// Initializes static members of the <see cref="DefaultSetting"/> class.
        /// </summary>
        static DefaultSetting()
        {
            DefaultSerializeSetting = new JsonSerializeSetting("_d_");
            DefaultDeserializeSetting = new JsonDeserializeSetting("_d_");
            DeserializeBuilderTable = new Hashtable(30);
        }

        #endregion ctor
    }
}
