using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Configuration.ConfigCenter
{
    /// <summary>
    /// 共享文件信息
    /// </summary>
    public sealed class ShareFileInfo
    {
        /// <summary>
        /// 共享名字，全局要唯一
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 是否模板
        /// </summary>
        public bool Template { get; }

        /// <summary>
        /// 节点信息
        /// </summary>
        public IEnumerable<ShareNodeInfo> Node { get; }

        /// <summary>
        /// 
        /// </summary>
        public ShareFileInfo(string sharename, bool template, IEnumerable<ShareNodeInfo> node)
        {
            this.Name = sharename; this.Template = template; this.Node = node;
        }

        #region nested

        /// <summary>
        /// 节点信息
        /// </summary>
        public sealed class ShareNodeInfo
        {
            /// <summary>
            /// key
            /// </summary>
            public string Key { get; }

            /// <summary>
            /// 是否使用替换
            /// </summary>
            public bool NeedToChange { get; private set; }

            /// <summary>
            /// 修改前辍
            /// </summary>
            public string ChangedPrefix { get; private set; }

            /// <summary>
            /// value
            /// </summary>
            public string Value { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public ShareNodeInfo(string key, string value)
            {
                this.Key = key;
                if (value.StartsWith("find://eat"))
                {
                    this.Value = value.Sub("find://eat".Length).TrimStart('@');
                    this.NeedToChange = true;
                    this.ChangedPrefix = "find://eat";
                }
                else if (value.StartsWith("find://cat"))
                {
                    this.Value = value.Sub("find://cat".Length).TrimStart('@');
                    this.NeedToChange = true;
                    this.ChangedPrefix = "find://cat";
                }
                else
                {
                    this.Value = value;
                    this.NeedToChange = false;
                    this.ChangedPrefix = string.Empty;
                }
            }

            /// <summary>
            /// 替换
            /// </summary>
            /// <param name="value"></param>
            public void Replace(string value)
            {
                this.Value = value;
            }

            /// <summary>
            /// 返回值
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return this.Value;
            }
        }

        #endregion
    }
}
