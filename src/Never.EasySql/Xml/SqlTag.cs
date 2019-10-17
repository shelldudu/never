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
    public class SqlTag : Never.EasySql.SqlTag
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

        internal void Build(SqlTagProvider sqlTagProvider, string parameterPrefix, List<SqlTag> includes)
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
                    var sql = sqlTag as SqlTag;
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

        #region 读取

        private TextLabel ReadTextNode(XmlNode node, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
        {
            return this.ReadTextNode(node.InnerText, writer, readerHelper, parameterPrefix);
        }

        internal TextLabel ReadTextNode(string text, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
        {
            var i = -1;
            var length = text.Length;
            var label = new TextLabel() { };
            var cuted = false;
            while (true)
            {
                i++;
                if (i >= length)
                {
                    break;
                }

                switch (text[i])
                {
                    case '\'':
                        {
                            writer.Write('\'');
                            var start = i;
                            var end = text.Length;
                            while (true)
                            {
                                start++;
                                if (text.Length <= start)
                                {
                                    throw new DataFormatException("sql tag {0} can not find ' char on ending;", this.Id);
                                }

                                if (text[start] == '$')
                                {
                                    writer.Write('$');
                                    var start2 = start;
                                    var end2 = text.Length;
                                    while (true)
                                    {
                                        start2++;
                                        if (text.Length <= start2)
                                        {
                                            break;
                                        }

                                        if (text[start2] == '\'')
                                        {
                                            throw new DataFormatException("sql tag {0} can not find $ char;", this.Id);
                                        }

                                        if (text[start2] == '$')
                                        {
                                            writer.Write('$');
                                            end2 = start2 - 1;
                                            cuted = true;
                                            break;
                                        }

                                        writer.Write(text[start2]);
                                        if (this.Pause(readerHelper, text[start2]))
                                        {
                                            end2 = start2 - 1;
                                            break;
                                        }
                                    }

                                    //从引号到$中间多了某些字符
                                    var ss = start + 1;
                                    var se = end2 - start;
                                    var name = text.Sub(ss, se);
                                    var parameter = new SqlTagParameterPosition()
                                    {
                                        Name = name,
                                        SourcePrefix = new string(text[start], 1),
                                        ActualPrefix = parameterPrefix,
                                        StartPosition = start + 1,
                                        PrefixStart = start,
                                        StopPosition = end2,
                                        PositionLength = end2 - start,
                                    };

                                    if (cuted)
                                    {
                                        parameter.StopPosition += 1;
                                        parameter.PositionLength += 1;
                                        cuted = false;
                                    }

                                    parameter.TextParameter = parameter.SourcePrefix == "$";
                                    label.Add(parameter);
                                    start = start2;
                                }
                                else if (text[start] == '@')
                                {
                                    throw new DataFormatException("sql tag {0} reading '@' is error;", this.Id);
                                }
                                else
                                {
                                    writer.Write(text[start]);
                                }
                                if (text[start] == '\'')
                                {
                                    i = start;
                                    break;
                                }
                            }
                        }
                        break;

                    case '$':
                        {
                            writer.Write('$');
                            var start = i;
                            var end = text.Length;
                            while (true)
                            {
                                start++;
                                if (text.Length <= start)
                                {
                                    throw new ArgumentOutOfRangeException($"start with $ must end with $,position is {start}");
                                }

                                if (text[start] == '$')
                                {
                                    writer.Write('$');
                                    end = start - 1;
                                    cuted = true;
                                    break;
                                }

                                writer.Write(text[start]);
                                if (this.Pause(readerHelper, text[start]))
                                {
                                    end = start - 1;
                                    break;
                                }
                            }

                            var name = text.Sub(i + 1, end - i);
                            var parameter = new SqlTagParameterPosition()
                            {
                                Name = name,
                                SourcePrefix = new string(text[i], 1),
                                ActualPrefix = parameterPrefix,
                                StartPosition = i + 1,
                                PrefixStart = i,
                                StopPosition = 1 + end,
                                PositionLength = end - i,
                            };

                            if (cuted)
                            {
                                parameter.StopPosition += 1;
                                parameter.PositionLength += 1;
                                cuted = false;
                            }

                            parameter.TextParameter = parameter.SourcePrefix == "$";
                            label.Add(parameter);
                            i = start;
                        }
                        break;

                    case '@':
                        {
                            writer.Write(parameterPrefix);
                            var start = i;
                            var end = text.Length;
                            while (true)
                            {
                                start++;
                                if (text.Length <= start)
                                {
                                    end = start - 1;
                                    break;
                                }

                                writer.Write(text[start]);
                                if (this.Pause(readerHelper, text[start]))
                                {
                                    end = start - 1;
                                    break;
                                }
                            }

                            var name = text.Sub(i + 1, end - i);
                            var parameter = new SqlTagParameterPosition()
                            {
                                Name = name,
                                SourcePrefix = new string(text[i], 1),
                                ActualPrefix = parameterPrefix,
                                StartPosition = i + 1,
                                PrefixStart = i,
                                StopPosition = (i + 1) + end - i,
                                PositionLength = end - i,
                            };

                            parameter.TextParameter = parameter.SourcePrefix == "$";

                            /*前缀为@@这样的长度*/
                            if (parameterPrefix.Length > 1)
                            {
                                parameter.StopPosition += parameterPrefix.Length - 1;
                                parameter.PositionLength += parameterPrefix.Length - 1;
                            }

                            label.Add(parameter);
                            i = start;
                        }
                        break;

                    default:
                        {
                            writer.Write(text[i]);
                        }
                        break;
                }
            }

            label.SqlText = writer.ToString();
            this.TextLength += label.SqlText.Length;
            writer.Clear();
            return label;
        }

        private TextLabel ReadTextNodeUsingFormatLine(XmlNode node, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
        {
            return this.ReadTextNodeUsingFormatLine(node.InnerText, writer, readerHelper, parameterPrefix);
        }

        internal TextLabel ReadTextNodeUsingFormatLine(string text, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
        {
            var i = -1;
            var length = text.Length;
            var label = new TextLabel() { };
            var hack = 0;
            var cuted = false;
            char last = 'a';
            while (true)
            {
                i++;
                if (i >= length)
                {
                    break;
                }

                if (readerHelper.IsWhiteSpaceChangeLine(text[i]))
                {
                    hack++;
                    continue;
                }

                i--;
                break;
            }

            while (true)
            {
                i++;
                if (i >= length)
                {
                    break;
                }

                switch (text[i])
                {
                    case '\u0020':
                    case '\u0009':
                        {
                            if (readerHelper.IsWhiteSpaceChangeLine(last))
                            {
                                hack++;
                            }
                            else
                            {
                                writer.Write(last = text[i]);
                            }
                        }
                        break;

                    case '\u000A':
                    case '\u000D':
                        {
                            switch (last)
                            {
                                case '\u0020':
                                case '\u0009':
                                    {
                                        writer.Write(last = text[i]);
                                    }
                                    break;

                                case '\u000A':
                                case '\u000D':
                                    {
                                        hack++;
                                    }
                                    break;

                                default:
                                    {
                                        writer.Write(last = text[i]);
                                    }
                                    break;
                            }
                        }
                        break;

                    case '\'':
                        {
                            writer.Write(last = text[i]);
                            var start = i;
                            var end = text.Length;
                            while (true)
                            {
                                start++;
                                if (text.Length <= start)
                                {
                                    throw new DataFormatException("sql tag {0} can not find ' char;", this.Id);
                                }

                                if (text[start] == '$')
                                {
                                    last = '$';
                                    writer.Write('$');
                                    var start2 = start;
                                    var end2 = text.Length;
                                    while (true)
                                    {
                                        start2++;
                                        if (text.Length <= start2)
                                        {
                                            break;
                                        }

                                        if (text[start2] == '\'')
                                        {
                                            throw new DataFormatException("sql tag {0} can not find $ char;", this.Id);
                                        }

                                        if (text[start2] == '$')
                                        {
                                            writer.Write(last = '$');
                                            end2 = start2 - 1;
                                            cuted = true;
                                            break;
                                        }

                                        writer.Write(last = text[start2]);
                                        if (this.Pause(readerHelper, text[start2]))
                                        {
                                            end2 = start2 - 1;
                                            break;
                                        }
                                    }

                                    //从引号到$中间多了某些字符
                                    var ss = start + 1;
                                    var se = end2 - start;
                                    var name = text.Sub(ss, se);
                                    var parameter = new SqlTagParameterPosition()
                                    {
                                        Name = name,
                                        SourcePrefix = new string(text[start], 1),
                                        ActualPrefix = parameterPrefix,
                                        StartPosition = start + 1 - hack,
                                        PrefixStart = start - hack,
                                        StopPosition = end2 - hack,
                                        PositionLength = end2 - start,
                                    };

                                    if (cuted)
                                    {
                                        parameter.StopPosition += 1;
                                        parameter.PositionLength += 1;
                                        cuted = false;
                                    }

                                    parameter.TextParameter = parameter.SourcePrefix == "$";
                                    label.Add(parameter);
                                    start = start2;
                                }
                                else if (text[start] == '@')
                                {
                                    throw new DataFormatException("sql tag {0} reading '@' is error;", this.Id);
                                }
                                else
                                {
                                    writer.Write(last = text[start]);
                                }

                                if (text[start] == '\'')
                                {
                                    i = start;
                                    break;
                                }
                            }
                        }
                        break;

                    case '$':
                        {
                            last = '$';
                            writer.Write('$');
                            var start = i;
                            var end = text.Length;
                            while (true)
                            {
                                start++;
                                if (text.Length <= start)
                                {
                                    throw new ArgumentOutOfRangeException($"start with $ must end with $,position is {start}");
                                }

                                if (text[start] == '$')
                                {
                                    last = '$';
                                    writer.Write('$');
                                    end = start - 1;
                                    cuted = true;
                                    break;
                                }

                                writer.Write(last = text[start]);
                                if (this.Pause(readerHelper, text[start]))
                                {
                                    end = start - 1;
                                    break;
                                }
                            }

                            var name = text.Sub(i + 1, end - i);
                            var parameter = new SqlTagParameterPosition()
                            {
                                Name = name,
                                SourcePrefix = new string(text[i], 1),
                                ActualPrefix = parameterPrefix,
                                StartPosition = (i + 1) - hack,
                                PrefixStart = i - hack,
                                StopPosition = end - hack,
                                PositionLength = (end - i),
                            };

                            if (cuted)
                            {
                                parameter.StopPosition += 1;
                                parameter.PositionLength += 1;
                                cuted = false;
                            }

                            parameter.TextParameter = parameter.SourcePrefix == "$";
                            label.Add(parameter);
                            i = start;
                        }
                        break;

                    case '@':
                        {
                            last = parameterPrefix.Last();
                            writer.Write(parameterPrefix);
                            var start = i;
                            var end = text.Length;
                            while (true)
                            {
                                start++;
                                if (text.Length <= start)
                                {
                                    end = start - 1;
                                    break;
                                }

                                writer.Write(last = text[start]);

                                if (this.Pause(readerHelper, text[start]))
                                {
                                    end = start - 1;
                                    break;
                                }
                            }

                            var name = text.Sub(i + 1, end - i);
                            var parameter = new SqlTagParameterPosition()
                            {
                                Name = name,
                                SourcePrefix = new string(text[i], 1),
                                ActualPrefix = parameterPrefix,
                                StartPosition = (i + 1) - hack,
                                PrefixStart = i - hack,
                                StopPosition = (i + end - i) - hack,
                                PositionLength = (end - i),
                            };

                            parameter.TextParameter = parameter.SourcePrefix == "$";

                            /*前缀为@@这样的长度*/
                            if (parameterPrefix.Length > 1)
                            {
                                parameter.StopPosition += parameterPrefix.Length - 1;
                                parameter.PositionLength += parameterPrefix.Length - 1;
                            }

                            label.Add(parameter);
                            i = start;
                        }
                        break;

                    default:
                        {
                            writer.Write(last = text[i]);
                        }
                        break;
                }
            }

            label.SqlText = writer.ToString();
            this.TextLength += label.SqlText.Length;
            writer.Clear();
            return label;
        }

        private TextLabel ReadReturnNode(XmlNode node, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
        {
            return this.ReadReturnNode(node.InnerText, writer, readerHelper, parameterPrefix);
        }

        internal TextLabel ReadReturnNode(string text, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
        {
            return new TextLabel() { SqlText = text };
        }

        private TextLabel ReadReturnNodeUsingFormatLine(XmlNode node, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
        {
            return this.ReadReturnNodeUsingFormatLine(node.InnerText, writer, readerHelper, parameterPrefix);
        }

        internal TextLabel ReadReturnNodeUsingFormatLine(string text, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
        {
            var i = -1;
            var length = text.Length;
            //0 表示没有引号，1表示已经进入引号
            byte mode = 0;
            var label = new TextLabel() { };
            while (true)
            {
                i++;
                if (i >= length)
                {
                    break;
                }

                if (readerHelper.IsWhiteSpaceChangeLine(text[i]))
                {
                    continue;
                }

                i--;
                break;
            }

            while (true)
            {
                i++;
                if (i >= length)
                {
                    break;
                }

                if (readerHelper.IsWhiteSpaceChangeLine(text[i]))
                {
                    if (mode == 1)
                    {
                        writer.Write(text[i]);
                    }
                    else
                    {
                        var next = this.Next(text, i);
                        if (next.HasValue && readerHelper.IsWhiteSpaceChangeLine(next.Value))
                        {
                            continue;
                        }
                        else
                        {
                            writer.Write(text[i]);
                            continue;
                        }
                    }
                }
                else
                {
                    writer.Write(text[i]);
                }
            }

            label.SqlText = writer.ToString();
            this.TextLength += label.SqlText.Length;
            writer.Clear();
            return label;
        }

        #endregion 读取

        /// <summary>
        /// 参数停顿
        /// </summary>
        /// <param name="readerHelper"></param>
        /// <param name="char"></param>
        /// <returns></returns>
        private bool Pause(SequenceStringReader readerHelper, char @char)
        {
            if (readerHelper.IsWhiteSpaceChangeLine(@char))
            {
                return true;
            }

            switch (@char)
            {
                case ';':
                case ',':
                case ')':
                    {
                        return true;
                    }
            }

            return false;
        }

        private char? Next(string next, int i)
        {
            if (next.Length - 1 <= i)
            {
                return null;
            }

            return next[i + 1];
        }

        #endregion build
    }
}
