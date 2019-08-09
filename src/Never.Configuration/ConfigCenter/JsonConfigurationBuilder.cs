using Never.Serialization.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Never.Configuration.ConfigCenter
{
    /// <summary>
    /// json配置文件读取
    /// </summary>
    public class JsonConfigurationBuilder : IConfigurationBuilder, IShareFileReference
    {
        #region field and ctor

        private readonly List<ShareFileInfo> usingShareInfo = null;
        private IEnumerable<ShareConfigurationBuilder> share = null;
        private readonly ICustomKeyValueFinder keyValueFinder = null;
        private bool builded = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="share"></param>
        /// <param name="fileInfo"></param>
        /// <param name="keyValueFinder"></param>
        public JsonConfigurationBuilder(IEnumerable<ShareConfigurationBuilder> share, ConfigFileInfo fileInfo, ICustomKeyValueFinder keyValueFinder)
        {
            this.share = share;
            this.File = fileInfo;
            this.Name = this.File.File.Name.Replace(this.File.File.Extension, "").Trim(new[] { ' ', '.' });
            this.AllName = this.File.File.Name;
            this.usingShareInfo = new List<ShareFileInfo>();
            this.keyValueFinder = keyValueFinder;
        }

        #endregion

        #region prop and refence

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 全名称
        /// </summary>
        public string AllName { get; }

        /// <summary>
        /// 文件内容
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public ConfigFileType FileType { get { return ConfigFileType.Json; } }

        /// <summary>
        /// 文件
        /// </summary>
        public ConfigFileInfo File { get; }

        /// <summary>
        /// 文件与共享文件的引用
        /// </summary>
        public IConfigurationBuilder Builder { get { return this; } }

        /// <summary>
        /// 文件与共享文件的引用
        /// </summary>
        public IEnumerable<ShareFileInfo> Reference
        {
            get
            {
                if (this.usingShareInfo.IsNullOrEmpty())
                    return null;

                this.Build();

                return this.usingShareInfo;
            }
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        public bool Match(bool useFileAllNameAsAppUniqueId, string name)
        {
            return useFileAllNameAsAppUniqueId ? this.AllName.IsEquals(name) : this.Name.IsEquals(name);
        }

        #endregion

        #region start

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public JsonConfigurationBuilder Build()
        {
            if (this.builded)
                return this;

            this.builded = true;
            this.Content = this.Build(this.File);
            return this;
        }

        /// <summary>
        /// 重新构建
        /// </summary>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public void Rebuild(IEnumerable<ShareConfigurationBuilder> shares)
        {
            this.share = shares;
            this.Content = this.Build(this.File);
        }

        /// <summary>
        /// 构建信息
        /// </summary>
        protected string Build(ConfigFileInfo fileInfo)
        {
            var content = System.IO.File.ReadAllText(fileInfo.File.FullName, fileInfo.Encoding);
            if (content.IsNullOrWhiteSpace())
                return content;

            //replace find://eat@user
            content = Regex.Replace(content, "\"find://(eat)@(.*?)\"", m =>
            {
                var split = m.Value.Trim(new[] { '\'', '\"' }).Split('@');
                if (split == null || split.Length <= 1)
                    return m.Value;

                var value = this.Find("eat", split[1]);
                return value.IsNotNullOrEmpty() ? value : m.Value;
            });

            //replace find://cat@user
            content = Regex.Replace(content, "\"find://(cat)@(.*?)\"", m =>
            {
                var split = m.Value.Trim(new[] { '\'', '\"' }).Split('@');
                if (split == null || split.Length <= 1)
                    return m.Value;

                var value = this.Find("cat", split[1]);

                return value.IsNotNullOrEmpty() ? string.Concat(m.Value[0], value, m.Value[m.Value.Length - 1]) : m.Value;
            });

            //replace link://eat@user
            content = Regex.Replace(content, "\"link://eat@(.*?)\"", m =>
            {
                var split = m.Value.Trim(new[] { '\'', '\"' }).Split('@');
                if (split == null || split.Length <= 1)
                    return m.Value;

                var keys = new List<string>(split.Length);
                for (var k = 2; k < split.Length; k++)
                {
                    keys.Add(split[k]);
                }
                var value = this.Link(split[1], keys);
                return value.IsNotNullOrEmpty() ? value : m.Value;
            });

            //replace link://cat@user
            content = Regex.Replace(content, "\"link://cat@(.*?)\"", m =>
            {
                var split = m.Value.Trim(new[] { '\'', '\"' }).Split('@');
                if (split == null || split.Length <= 1)
                    return m.Value;

                var keys = new List<string>(split.Length);
                for (var k = 2; k < split.Length; k++)
                {
                    keys.Add(split[k]);
                }
                var value = this.Link(split[1], keys);
                return value.IsNotNullOrEmpty() ? string.Concat(m.Value[0], value, m.Value[m.Value.Length - 1]) : m.Value;
            });

            return content;
        }

        /// <summary>
        /// 查询关键字
        /// </summary>
        /// <param name="mode">eat还是cat</param>
        /// <param name="key">@user参数</param>
        /// <returns></returns>
        protected string Find(string mode, string key)
        {
            if (this.keyValueFinder == null)
                throw new Exception(string.Format("the keyValueFinder is null;", share, this.File.File.Name));

            return this.keyValueFinder.Find(mode, key);
        }

        /// <summary>
        /// 外连关键字
        /// </summary>
        /// <param name="share">共享文件内容</param>
        /// <param name="keys">关键字</param>
        /// <returns></returns>
        protected string Link(string share, List<string> keys)
        {
            var jsonShareItem = this.share.FirstOrDefault(ta => ta.JsonShareFile != null && ta.JsonShareFile.Name == share);
            if (jsonShareItem != null)
            {
                var sharelist = jsonShareItem.JsonShareFile;
                if (sharelist.Node.IsNullOrEmpty())
                    throw new Exception(string.Format("the share {0} file has no node;", share, this.File.File.Name));

                var key = string.Join(":", keys);
                var first = sharelist.Node.FirstOrDefault(ta => ta.Key == key);
                if (first == null)
                    throw new Exception(string.Format("can not find the key {1} in the share {0} file", share, key));

                if (!this.usingShareInfo.Contains(sharelist))
                    this.usingShareInfo.Add(sharelist);

                return first.Value;
            }

            var xmlShareItem = this.share.FirstOrDefault(ta => ta.XmlShareFile != null && ta.XmlShareFile.Name == share);
            if (xmlShareItem != null)
            {
                var sharelist = xmlShareItem.XmlShareFile;
                if (sharelist.Node.IsNullOrEmpty())
                    throw new Exception(string.Format("the share {0} file has no node;", share, this.File.File.Name));

                var key = string.Join(":", keys);
                var first = sharelist.Node.FirstOrDefault(ta => ta.Key == key);
                if (first == null)
                    throw new Exception(string.Format("can not find the key {1} in the share {0} file", share, key));

                if (!this.usingShareInfo.Contains(sharelist))
                    this.usingShareInfo.Add(sharelist);

                return first.Value;
            }

            throw new Exception(string.Format("can not find the share {0} file while load the {1} files", share, this.File.File.Name));
        }

        #endregion
    }
}
