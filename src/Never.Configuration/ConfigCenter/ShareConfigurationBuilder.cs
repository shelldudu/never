using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Never.Configuration.ConfigCenter
{
    /// <summary>
    /// 配置文件，当前只读取*.config与*.json配置文件
    /// </summary>
    public class ShareConfigurationBuilder : IDisposable
    {
        #region action

        /// <summary>
        /// 共享文件
        /// </summary>
        public FileInfo File { get; }

        /// <summary>
        /// 编码
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// json的配置文件
        /// </summary>
        public ShareFileInfo JsonShareFile { get; private set; }

        /// <summary>
        /// xml的json的配置文件
        /// </summary>
        public ShareFileInfo XmlShareFile { get; private set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public ConfigFileType FileType { get; private set; }

        /// <summary>
        /// 在构建之中
        /// </summary>
        public event EventHandler<ShareFileEventArgs> OnBuilding
        {
            add
            {
                this.events.Add(value);
            }
            remove
            {
                this.events.Remove(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private List<EventHandler<ShareFileEventArgs>> events = null;

        /// <summary>
        /// 构建了
        /// </summary>
        private bool builded = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFileInfo"></param>
        public ShareConfigurationBuilder(ConfigFileInfo configFileInfo)
        {
            this.File = configFileInfo.File;
            this.Encoding = configFileInfo.Encoding;
            this.events = new List<EventHandler<ShareFileEventArgs>>();
        }

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        public ShareConfigurationBuilder Build()
        {
            if (builded)
                return this;

            return this.Rebuild();
        }

        /// <summary>
        /// 重新构建
        /// </summary>
        /// <returns></returns>
        public ShareConfigurationBuilder Rebuild()
        {
            this.builded = this.builded || true;

            switch (this.File.Extension.ToLower())
            {
                case ".json":
                    {
                        var content = System.IO.File.ReadAllText(this.File.FullName, this.Encoding);
                        if (content.IsNullOrWhiteSpace())
                            return this;

                        //替（1）/**/，（2）//
                        content = Regex.Replace(content, "/\\*(?<name>[\\w\\W]+?)\\*/", (m) =>
                        {
                            return string.Empty;
                        });

                        content = Regex.Replace(content, @"[^:]//(?<name>(.*?))[\r\n]", (m) =>
                        {
                            var firstindex = m.Value.IndexOf("//");
                            if (firstindex == 0)
                                return string.Empty;

                            var value = m.Value.Sub(0, firstindex).Trim();
                            return value;
                        });

                        this.JsonShareFile = this.HandleJsonFile(this.File, content);
                        this.FileType = ConfigFileType.Json;
                    }
                    break;
                case ".xml":
                    {
                        var content = System.IO.File.ReadAllText(this.File.FullName, this.Encoding);
                        if (content.IsNullOrWhiteSpace())
                            return this;

                        //替（1）<!--**-->
                        content = Regex.Replace(content, "<!--(?<name>[\\w\\W]+?)-->", (m) =>
                        {
                            return string.Empty;
                        });

                        this.XmlShareFile = this.HandleXmlFile(this.File, content);
                        this.FileType = ConfigFileType.Xml;
                    }
                    break;
            }

            return this;
        }

        /// <summary>
        /// 读取共享文件的所有内容，只读取key-value的类型
        /// </summary>
        /// <param name="file"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        private ShareFileInfo HandleXmlFile(FileInfo file, string xml)
        {
            var reader = new System.Xml.XmlDocument();
            reader.LoadXml(xml);
            if (reader.ChildNodes == null || reader.ChildNodes.Count <= 1)
                return null;

            var config = reader.ChildNodes[1];
            if (config == null || config.HasChildNodes == false)
            {
                return null;
            }

            var list = new List<KeyValueTuple<string, string>>(config.ChildNodes.Count);
            var templateNode = config.ChildNodes[0];
            if (templateNode.Name.IsNotEquals("template"))
            {
                throw new Exception(string.Format("share file {0} first children node must be template node;", file.FullName));
            }

            var tvalue = templateNode.Attributes.GetNamedItem("value");
            if (tvalue == null || tvalue.Value.IsNullOrWhiteSpace())
            {
                throw new Exception(string.Format("share file {0} template node must be has value attribute;", file.FullName));
            }

            if (tvalue.Value.IsNotEquals("true", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(string.Format("share file {0} template value must been true,so is not a template file;", file.FullName));
            }

            var tsharename = templateNode.Attributes.GetNamedItem("sharename");
            if (tsharename == null || tsharename.Value.IsNullOrWhiteSpace())
            {
                throw new Exception(string.Format("share file {0} template node must be has sharename attribute;", file.FullName));
            }

            list.Add(new KeyValueTuple<string, string>("template", tvalue.Value));
            list.Add(new KeyValueTuple<string, string>("sharename", tsharename.Value));

            foreach (XmlNode node in config.ChildNodes)
            {
                if (node == templateNode)
                    continue;

                var key = node.Attributes.GetNamedItem("key");
                var value = node.Attributes.GetNamedItem("value");

                if (node.HasChildNodes)
                {
                    var keyname = string.Empty;
                    var name = node.Attributes.GetNamedItem("name");
                    if (name != null && name.Value != null)
                    {
                        keyname = name.Value;
                    }
                    else
                    {
                        var id = node.Attributes.GetNamedItem("id");
                        if (id != null && id.Value != null)
                        {
                            keyname = id.Value;
                        }
                        else
                        {
                            if (key != null)
                                keyname = key.Value;
                            else if (value != null)
                                keyname = value.Value;
                        }
                    }

                    throw new Exception(string.Format("share file {0} must be key-vlue type,the key {1} content has childnodes;", file.FullName, keyname, node.NodeType));
                }

                if (key != null && key.Value.IsNotNullOrWhiteSpace() && value != null)
                {
                    list.Add(new KeyValueTuple<string, string>(key.Value, value.Value));
                }
            }

            var shareName = list.FirstOrDefault(ta => ta.Key == "sharename");
            if (shareName != null)
            {
                list.Remove(shareName);
            }

            var template = list.FirstOrDefault(ta => ta.Key == "template");
            if (template != null)
            {
                list.Remove(template);
            }

            var istemplate = template?.Value.IsEquals("true");
            return new ShareFileInfo(shareName?.Value, istemplate.HasValue ? istemplate.Value : false, list.Select(ta => new ShareFileInfo.ShareNodeInfo(ta.Key, ta.Value)).ToList());
        }

        /// <summary>
        /// 读取共享文件的所有内容，只读取key-value的类型
        /// </summary>
        /// <param name="file"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        private ShareFileInfo HandleJsonFile(FileInfo file, string json)
        {
            var reader = Never.Serialization.Json.ThunderReader.Load(json);
            var list = new List<KeyValueTuple<string, string>>(reader.Count);
            foreach (var node in reader)
            {
                switch (node.NodeType)
                {
                    case Serialization.Json.Deserialize.ContentNodeType.String:
                        {
                            list.Add(new KeyValueTuple<string, string>(node.Key, node.ToString()));
                        }
                        break;
                    case Serialization.Json.Deserialize.ContentNodeType.Object:
                        {
                            list.Add(new KeyValueTuple<string, string>(node.Key, node.ToString()));
                        }
                        break;
                    case Serialization.Json.Deserialize.ContentNodeType.Array:
                        {
                            list.Add(new KeyValueTuple<string, string>(node.Key, node.ToString()));
                        }
                        break;
                }
            }

            var shareName = list.FirstOrDefault(ta => ta.Key == "sharename");
            if (shareName != null)
            {
                list.Remove(shareName);
                if (shareName.Value.IsNullOrWhiteSpace())
                    throw new Exception(string.Format("share file {0} key {1} must has value;", file.FullName, shareName.Key));
            }
            else
            {
                throw new Exception(string.Format("share file {0} template node must be has sharename attribute;", file.FullName));
            }

            var template = list.FirstOrDefault(ta => ta.Key == "template");
            if (template != null)
            {
                list.Remove(template);
                if (template.Value.IsNullOrWhiteSpace())
                    throw new Exception(string.Format("share file {0} key {1} must has value;", file.FullName, template.Key));

                if (template.Value.IsNotEquals("true", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception(string.Format("share file {0} template value must been true,so is not a template file;", file.FullName));
                }
            }
            else
            {
                throw new Exception(string.Format("share file {0} template node must be has sharename attribute;", file.FullName));
            }

            var istemplate = template?.Value.IsEquals("true");
            return new ShareFileInfo(shareName?.Value, istemplate.HasValue ? istemplate.Value : false, list.Select(ta => new ShareFileInfo.ShareNodeInfo(ta.Key, ta.Value)).ToList());
        }

        /// <summary>
        /// 是否相同
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ShareConfigurationBuilder other)
        {
            return this.File == other.File;
        }

        /// <summary>
        /// 在构建之中
        /// </summary>
        /// <returns></returns>
        /// <param name="old"></param>
        public ShareConfigurationBuilder Build(ShareConfigurationBuilder old = null)
        {
            if (old != null)
            {
                foreach (var @event in old.events)
                {
                    this.OnBuilding += @event;
                }

                old.Dispose();
            }

            var e = new ShareFileEventArgs(this.JsonShareFile, this.XmlShareFile);
            this.events.ForEach(ta => ta.Invoke(this, e));
            return this;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            this.events.Clear();
        }
        #endregion action
    }
}
