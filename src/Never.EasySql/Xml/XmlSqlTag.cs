using Never.EasySql.Labels;
using Never.Exceptions;
using Never.Serialization.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Never.EasySql.Xml
{
    /// <summary>
    /// sqltag
    /// </summary>
    public class XmlSqlTag : Never.EasySql.SqlTag
    {
        #region prop
        /// <summary>
        /// 命令空间
        /// </summary>
        public string NameSpace { get; internal set; }

        /// <summary>
        /// 命令空间是否使用缩进信息
        /// </summary>
        public bool IndentedOnNameSpace { get; internal set; }

        /// <summary>
        /// 自身是否使用缩进信息
        /// </summary>
        public bool IndentedOnSqlTag { get; internal set; }

        /// <summary>
        /// 命令
        /// </summary>
        public string CommandType { get; internal set; }

        /// <summary>
        /// 节点
        /// </summary>
        public XmlNode Node { get; internal set; }

        /// <summary>
        /// 是否格式化
        /// </summary>
        public bool FormatLine
        {
            get
            {
                if (!this.IndentedOnSqlTag)
                {
                    return false;
                }

                return this.IndentedOnNameSpace;
            }
        }

        #endregion label

        #region build

        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="sqlTagProvider"></param>
        /// <param name="parameterPrefix"></param>
        /// <param name="includes"></param>
        public void Build(SqlTagProvider sqlTagProvider, string parameterPrefix, List<XmlSqlTag> includes)
        {
            if (this.Node == null)
            {
                return;
            }

            if (this.CommandType == "sql")
            {
                includes.Add(this);
                return;
            }

            if (!this.Node.HasChildNodes || this.Node.ChildNodes.Count <= 0)
            {
                return;
            }

            var readerHelper = new SequenceStringReader("1");
            if (this.Node.ChildNodes.Count == 1 && this.Node.FirstChild.NodeType == XmlNodeType.Text)
            {
                this.Labels = new List<ILabel>()
                {
                    this.FormatLine?
                    this.ReadTextNodeUsingFormatLine(this.Node.FirstChild,new ThunderWriter(this.Node.FirstChild.InnerXml.Length),readerHelper,parameterPrefix)
                    :
                    this.ReadTextNode(this.Node.FirstChild,new ThunderWriter(this.Node.FirstChild.InnerXml.Length),readerHelper,parameterPrefix)
                };
                return;
            }

            while (true)
            {
                var replace = new Dictionary<XmlNode, List<XmlNode>>();
                this.QueryReplaceNode(sqlTagProvider, this.Node, replace);
                if (replace.Count == 0)
                {
                    break;
                }

                foreach (var r in replace)
                {
                    if (r.Value == null || r.Value.Count == 0)
                    {
                        r.Key.ParentNode.RemoveChild(r.Key);
                    }

                    var next = r.Key;
                    foreach (var s in r.Value)
                    {
                        next = r.Key.ParentNode.InsertAfter(s, next);
                    }
                }

                foreach (var r in replace)
                {
                    if (r.Value == null || r.Value.Count == 0)
                    {
                        continue;
                    }

                    r.Key.ParentNode.RemoveChild(r.Key);
                }
            }

            this.Labels = new List<ILabel>(this.Node.ChildNodes.Count);
            foreach (XmlNode node in this.Node.ChildNodes)
            {
                this.Build(node, (List<ILabel>)this.Labels, readerHelper, parameterPrefix);
            }

            this.Node = null;
        }

        private void QueryReplaceNode(SqlTagProvider sqlTagProvider, XmlNode parentNode, Dictionary<XmlNode, List<XmlNode>> replace)
        {
            var childnodes = parentNode.ChildNodes;
            foreach (XmlNode node in childnodes)
            {
                if (node.NodeType == XmlNodeType.Text)
                {
                    continue;
                }

                if (node.LocalName.IsEquals("include") || node.Name.IsEquals("include"))
                {
                    var id = node.Attributes.GetNamedItem("refid");
                    if (id == null || id.Value.IsNullOrEmpty())
                    {
                        throw new KeyNotExistedException("Id", string.Format("can not build the {0} tag,because the include has not attribue name id", this.Id));
                    }

                    sqlTagProvider.TryGet(id.Value, out var sqlTag);
                    var sql = sqlTag as XmlSqlTag;
                    if (sql != null && sql.Node != null)
                    {
                        if (sql.CommandType.IsNotEquals("sql", StringComparison.CurrentCultureIgnoreCase))
                        {
                            throw new DataFormatException("tag just allow to include sql tag");
                        }

                        foreach (XmlNode j in sql.Node)
                        {
                            if (j.LocalName.IsEquals("include") || j.Name.IsEquals("include"))
                            {
                                throw new DataFormatException("sql tag can not include other resource");
                            }
                        }
                    }

                    if (sql == null)
                    {
                        throw new KeyNotExistedException("Id", string.Format("can not build the {0} tag,because the include has not attribue name id", this.Id));
                    }

                    if (sql.Node == null || !sql.Node.HasChildNodes)
                    {
                        continue;
                    }

                    replace[node] = new List<XmlNode>(sql.Node.ChildNodes.Count);
                    foreach (XmlNode subNode in sql.Node.ChildNodes)
                    {
                        replace[node].Add(subNode.CloneNode(true));
                    }
                }

                if (node.HasChildNodes)
                {
                    this.QueryReplaceNode(sqlTagProvider, node, replace);
                }
            }
        }

        private void Build(XmlNode node, List<ILabel> lines, SequenceStringReader readerHelper, string parameterPrefix)
        {
            if (node.NodeType == XmlNodeType.Text)
            {
                var label = this.FormatLine ? this.ReadTextNodeUsingFormatLine(node, new ThunderWriter(node.InnerXml.Length), readerHelper, parameterPrefix) : this.ReadTextNode(node, new ThunderWriter(node.InnerXml.Length), readerHelper, parameterPrefix);
                lines.Add(label);
                return;
            }

            if (node.NodeType == XmlNodeType.Element)
            {
                var parameter = node.Attributes.GetNamedItem("parameter");
                var then = node.Attributes.GetNamedItem("then");

                switch (node.Name)
                {
                    case "ifarray":
                        {
                            var open = node.Attributes.GetNamedItem("open");
                            var close = node.Attributes.GetNamedItem("close");

                            var split = node.Attributes.GetNamedItem("split");
                            var label = new ArrayLabel()
                            {
                                Open = open == null ? null : open.Value,
                                Close = close == null ? null : close.Value,
                                Split = split == null ? null : split.Value,
                                Parameter = parameter == null ? null : parameter.Value,
                                Then = then == null ? null : (this.FormatLine ? string.Concat(then.Value.Trim(), ' ') : then.Value.Trim()),
                                Line = this.FormatLine ? this.ReadTextNodeUsingFormatLine(node, new ThunderWriter(node.InnerText.Length), readerHelper, parameterPrefix) : this.ReadTextNode(node, new ThunderWriter(node.InnerText.Length), readerHelper, parameterPrefix),
                            };

                            lines.Add(label);
                        }
                        break;

                    case "if":
                        {
                            var end = node.Attributes.GetNamedItem("end");
                            var split = node.Attributes.GetNamedItem("split");
                            var label = new IfLabel()
                            {
                                Labels = new List<ILabel>(node.ChildNodes.Count),
                                Then = then == null ? null : (this.FormatLine ? string.Concat(then.Value.Trim(), ' ') : then.Value.Trim()),
                                End = end == null ? null : end.Value.Trim(),
                                Split = split == null ? null : split.Value,
                            };

                            if (node.HasChildNodes)
                            {
                                foreach (XmlNode s in node.ChildNodes)
                                {
                                    this.Build(s, label.Labels, readerHelper, parameterPrefix);
                                }

                                if (then != null)
                                {
                                    this.TextLength += then.Value.Length;
                                }
                            }

                            lines.Add(label);
                        }
                        break;

                    case "ifnotnull":
                        {
                            var label = new NotNullLabel()
                            {
                                Then = then == null ? null : (this.FormatLine ? string.Concat(then.Value.Trim(), ' ') : then.Value.Trim()),
                                Parameter = parameter == null ? null : parameter.Value,
                                Labels = node.HasChildNodes ? new List<ILabel>(node.ChildNodes.Count) : new List<ILabel>(0),
                            };

                            if (node.HasChildNodes)
                            {
                                foreach (XmlNode s in node.ChildNodes)
                                {
                                    this.Build(s, label.Labels, readerHelper, parameterPrefix);
                                }

                                if (then != null)
                                {
                                    this.TextLength += then.Value.Length;
                                }
                            }

                            lines.Add(label);
                        }
                        break;

                    case "ifnotempty":
                        {
                            var label = new NotEmptyLabel()
                            {
                                Then = then == null ? null : (this.FormatLine ? string.Concat(then.Value.Trim(), ' ') : then.Value.Trim()),
                                Parameter = parameter == null ? null : parameter.Value,
                                Labels = node.HasChildNodes ? new List<ILabel>(node.ChildNodes.Count) : new List<ILabel>(0),
                            };

                            if (node.HasChildNodes)
                            {
                                foreach (XmlNode s in node.ChildNodes)
                                {
                                    this.Build(s, label.Labels, readerHelper, parameterPrefix);
                                }

                                if (then != null)
                                {
                                    this.TextLength += then.Value.Length;
                                }
                            }

                            lines.Add(label);
                        }
                        break;

                    case "ifnull":
                        {
                            var label = new NullLabel()
                            {
                                Then = then == null ? null : (this.FormatLine ? string.Concat(then.Value.Trim(), ' ') : then.Value.Trim()),
                                Parameter = parameter == null ? null : parameter.Value,
                                Labels = node.HasChildNodes ? new List<ILabel>(node.ChildNodes.Count) : new List<ILabel>(0),
                            };

                            if (node.HasChildNodes)
                            {
                                foreach (XmlNode s in node.ChildNodes)
                                {
                                    this.Build(s, label.Labels, readerHelper, parameterPrefix);
                                }

                                if (then != null)
                                {
                                    this.TextLength += then.Value.Length;
                                }
                            }

                            lines.Add(label);
                        }
                        break;

                    case "ifempty":
                        {
                            var label = new EmptyLabel()
                            {
                                Then = then == null ? null : (this.FormatLine ? string.Concat(then.Value.Trim(), ' ') : then.Value.Trim()),
                                Parameter = parameter == null ? null : parameter.Value,
                                Labels = node.HasChildNodes ? new List<ILabel>(node.ChildNodes.Count) : new List<ILabel>(0),
                            };

                            if (node.HasChildNodes)
                            {
                                foreach (XmlNode s in node.ChildNodes)
                                {
                                    this.Build(s, label.Labels, readerHelper, parameterPrefix);
                                }

                                if (then != null)
                                {
                                    this.TextLength += then.Value.Length;
                                }
                            }

                            lines.Add(label);
                        }
                        break;

                    case "ifcontain":
                        {
                            var label = new ContainLabel()
                            {
                                Then = then == null ? null : (this.FormatLine ? string.Concat(then.Value.Trim(), ' ') : then.Value.Trim()),
                                Parameter = parameter == null ? null : parameter.Value,
                                Labels = node.HasChildNodes ? new List<ILabel>(node.ChildNodes.Count) : new List<ILabel>(0),
                            };

                            if (node.HasChildNodes)
                            {
                                foreach (XmlNode s in node.ChildNodes)
                                {
                                    this.Build(s, label.Labels, readerHelper, parameterPrefix);
                                }

                                if (then != null)
                                {
                                    this.TextLength += then.Value.Length;
                                }
                            }

                            lines.Add(label);
                        }
                        break;
                    case "ifnotexists":
                        {
                            var label = new NotExistsLabel()
                            {
                                Then = then == null ? null : (this.FormatLine ? string.Concat(then.Value.Trim(), ' ') : then.Value.Trim()),
                                Parameter = parameter == null ? null : parameter.Value,
                                Labels = node.HasChildNodes ? new List<ILabel>(node.ChildNodes.Count) : new List<ILabel>(0),
                            };

                            if (node.HasChildNodes)
                            {
                                foreach (XmlNode s in node.ChildNodes)
                                {
                                    this.Build(s, label.Labels, readerHelper, parameterPrefix);
                                }

                                if (then != null)
                                {
                                    this.TextLength += then.Value.Length;
                                }
                            }

                            lines.Add(label);
                        }
                        break;

                    case "return":
                        {
                            var type = node.Attributes.GetNamedItem("type");
                            var label = new ReturnLabel()
                            {
                                Type = type == null ? null : type.Value,
                                Line = this.FormatLine ?
                                this.ReadReturnNodeUsingFormatLine(node, new ThunderWriter(node.InnerXml.Length), readerHelper, parameterPrefix)
                                :
                                this.ReadReturnNode(node, new ThunderWriter(node.InnerXml.Length), readerHelper, parameterPrefix),
                            };

                            if (label.Line.SqlText != null)
                            {
                                this.TextLength += label.Line.SqlText.Length;
                            }

                            lines.Add(label);
                        }
                        break;
                }
            }
        }


        /// <summary>
        /// 读取textLabel
        /// </summary>
        /// <param name="node"></param>
        /// <param name="writer"></param>
        /// <param name="readerHelper"></param>
        /// <param name="parameterPrefix"></param>
        /// <returns></returns>
        public TextLabel ReadTextNode(XmlNode node, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
        {
            return this.ReadTextNode(node.InnerText, writer, readerHelper, parameterPrefix, true);
        }

        /// <summary>
        /// 读取textLabel
        /// </summary>
        /// <param name="node"></param>
        /// <param name="writer"></param>
        /// <param name="readerHelper"></param>
        /// <param name="parameterPrefix"></param>
        /// <returns></returns>
        public TextLabel ReadTextNodeUsingFormatLine(XmlNode node, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
        {
            return this.ReadTextNodeUsingFormatLine(node.InnerText, writer, readerHelper, parameterPrefix, true);
        }

        /// <summary>
        /// 读取return标签
        /// </summary>
        /// <param name="node"></param>
        /// <param name="writer"></param>
        /// <param name="readerHelper"></param>
        /// <param name="parameterPrefix"></param>
        /// <returns></returns>
        public TextLabel ReadReturnNode(XmlNode node, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
        {
            return this.ReadReturnNode(node.InnerText, writer, readerHelper, parameterPrefix);
        }
        /// <summary>
        /// 读取return标签
        /// </summary>
        /// <param name="node"></param>
        /// <param name="writer"></param>
        /// <param name="readerHelper"></param>
        /// <param name="parameterPrefix"></param>
        /// <returns></returns>
        public TextLabel ReadReturnNodeUsingFormatLine(XmlNode node, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
        {
            return this.ReadReturnNodeUsingFormatLine(node.InnerText, writer, readerHelper, parameterPrefix);
        }


        #endregion
    }

}
