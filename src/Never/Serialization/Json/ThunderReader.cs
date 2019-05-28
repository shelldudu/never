using Never.Serialization.Json.Deserialize;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Never.Serialization.Json
{
    /// <summary>
    /// 快速装载数据
    /// </summary>
    public class ThunderReader : IDeserializerReader, IEnumerable<JsonContentNode>
    {
        #region field and ctor

        /// <summary>
        /// 下一节点的读取
        /// </summary>
        private int move = -1;

        /// <summary>
        /// 原始json 串
        /// </summary>
        private readonly string original;

        /// <summary>
        /// json节点
        /// </summary>
        private readonly IList<JsonContentNode> nodes = null;

        /// <summary>
        /// 所有字符串
        /// </summary>
        private readonly char[] chars = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThunderReader"/> class.
        /// </summary>
        /// <param name="json">The json.</param>
        public ThunderReader(string json)
        {
            if (json == null)
                throw new ArgumentNullException("json", "json parameter is null");

            this.original = json;
            this.chars = json.ToCharArray();
            this.nodes = this.AnalyzeNode();
            if (this.nodes == null)
                this.nodes = new List<JsonContentNode>(0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThunderReader"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        protected ThunderReader(IObjectContentNode node)
        {
            this.original = node.Original;
            this.chars = node.Segment.Array;
            if (node.NodeType == ContentNodeType.String)
            {
                this.ContainerSignal = ContainerSignal.Empty;
                this.nodes = new List<JsonContentNode>() { (JsonContentNode)node };
            }
            else
            {
                if (node.NodeType == ContentNodeType.Object)
                    this.ContainerSignal = ContainerSignal.Object;
                else
                    this.ContainerSignal = ContainerSignal.Array;

                this.nodes = (IList<JsonContentNode>)node.Node;
            }
        }

        #endregion ctor

        #region ienumerable

        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>
        /// 用于循环访问集合的枚举数。
        /// </returns>
        public IEnumerator<JsonContentNode> GetEnumerator()
        {
            if (this.nodes == null)
                return (new List<JsonContentNode>()).GetEnumerator();

            return this.nodes.GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>
        /// 可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator" /> 对象。
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (this.nodes == null)
                return (new JsonContentNode[0]).GetEnumerator();

            return this.nodes.GetEnumerator();
        }

        #endregion ienumerable

        #region new and load

        /// <summary>
        /// json节点解析器
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static ThunderReader Load(string json)
        {
            return new ThunderReader(json);
        }

        /// <summary>
        /// 重新组织节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static ThunderReader Load(IObjectContentNode node)
        {
            return new ThunderReader(node);
        }

        /// <summary>
        /// 对jsontext进行节点解析
        /// </summary>
        /// <returns></returns>
        protected IList<JsonContentNode> AnalyzeNode()
        {
            if (this.original == null)
                return new JsonContentNode[0];

            if (this.original.Length < 3)
            {
                switch (this.original.Length)
                {
                    case 0:
                        {
                            this.ContainerSignal = ContainerSignal.Empty;
                            return new JsonContentNode[0];
                        };
                    case 1:
                        {
                            this.ContainerSignal = ContainerSignal.Empty;
                            var node = new JsonContentNode(this.chars, 0, 1)
                            {
                                Key = "",
                                Original = this.original,
                                Level = 0,
                                Node = null,
                                NodeType = ContentNodeType.String,
                            };

                            return new JsonContentNode[1]
                            {
                                node
                            };
                        };
                    case 2:
                        {
                            switch (this.original)
                            {
                                case "{}":
                                    {
                                        this.ContainerSignal = ContainerSignal.Object;
                                        return new JsonContentNode[1]
                                        {
                                            new JsonContentNode(this.chars, 0, 2)
                                            {
                                                Key ="",
                                                Original = this.original,
                                                Level = 0,
                                                Node = null,
                                                NodeType = ContentNodeType.Object,
                                            }
                                        };
                                    }

                                case "[]":
                                    {
                                        this.ContainerSignal = ContainerSignal.Array;
                                        return new JsonContentNode[1]
                                        {
                                            new JsonContentNode(this.chars, 0, 2)
                                            {
                                                Key ="",
                                                Original = this.original,
                                                Level = 0,
                                                Node = null,
                                                NodeType = ContentNodeType.Array,
                                            }
                                        };
                                    }
                                case "\"\"":
                                    {
                                        this.ContainerSignal = ContainerSignal.Empty;
                                        return new JsonContentNode[1]
                                        {
                                            new JsonContentNode(this.chars, 0, 2)
                                            {
                                                Key = "",
                                                Original = this.original,
                                                Level = 0,
                                                Node = null,
                                                NodeType = ContentNodeType.String,
                                            }
                                        };
                                    }
                                case "\'\'":
                                    {
                                        this.ContainerSignal = ContainerSignal.Empty;
                                        return new JsonContentNode[1]
                                        {
                                            new JsonContentNode(this.chars, 0, 2)
                                            {
                                                Key = "",
                                                Original = this.original,
                                                Level = 0,
                                                Node = null,
                                                NodeType = ContentNodeType.String,
                                            }
                                        };
                                    }
                                case "  ":
                                    {
                                        this.ContainerSignal = ContainerSignal.Empty;
                                        return new JsonContentNode[1]
                                        {
                                            new JsonContentNode(this.chars, 0, 2)
                                            {
                                                Key = "",
                                                Original = this.original,
                                                Level = 0,
                                                Node = null,
                                                NodeType = ContentNodeType.String,
                                            }
                                        };
                                    }
                            }
                            break;
                        };
                }
            }

            var reader = new SequenceStringReader(this.original);
            reader.Move();
            reader.SkipSpaceEnterResult();
            switch (reader.Current)
            {
                case '{':
                    {
                        var list = new List<JsonContentNode>();
                        this.ParseObject(list, reader, 0, ContentNodeType.Object, 0);
                        this.ContainerSignal = ContainerSignal.Object;
                        return list;
                    }

                case '[':
                    {
                        var list = new List<JsonContentNode>();
                        this.ParseArray(list, reader, 0, ContentNodeType.Array, 0);
                        this.ContainerSignal = ContainerSignal.Array;
                        return list;
                    }
                case '\'':
                    {
                        var node = new JsonContentNode()
                        {
                            Level = 0,
                            Key = "",
                            ValueQuote = ValueQuoteSignal.Single,
                            NodeType = ContentNodeType.String,
                            Original = this.original,
                        };

                        this.ReadAllContent(reader, node, reader.Head + 1);
                        this.ContainerSignal = ContainerSignal.Empty;
                        return new JsonContentNode[1] { node };
                    }

                case '\"':
                    {
                        var node = new JsonContentNode()
                        {
                            Level = 0,
                            Key = "",
                            ValueQuote = ValueQuoteSignal.Double,
                            NodeType = ContentNodeType.String,
                            Original = this.original,
                        };

                        this.ReadAllContent(reader, node, reader.Head + 1);
                        this.ContainerSignal = ContainerSignal.Empty;
                        return new JsonContentNode[1] { node };
                    }

                default:
                    {
                        var node = new JsonContentNode()
                        {
                            Level = 0,
                            Key = "",
                            ValueQuote = ValueQuoteSignal.No,
                            NodeType = ContentNodeType.String,
                            Original = this.original,
                        };
                        this.ReadAllContent(reader, node, reader.Head);
                        this.ContainerSignal = ContainerSignal.Empty;
                        return new JsonContentNode[1] { node };
                    }
            }
        }

        #endregion new and load

        #region IDeserializerReader

        /// <summary>
        /// 连续读取字符
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public virtual string Read(int start, int end)
        {
            return this.original.Substring(start, end - start);
        }

        /// <summary>
        /// 读取一个节点值
        /// </summary>
        /// <param name="key">节点key</param>
        /// <returns></returns>
        public virtual IObjectContentNode Read(string key)
        {
            return this.Read(key, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 读取一个节点值
        /// </summary>
        /// <param name="key">节点key</param>
        /// <param name="comparison">排序规则</param>
        /// <returns></returns>
        public virtual IObjectContentNode Read(string key, StringComparison comparison)
        {
            if (this.nodes == null)
                return null;

            if (string.IsNullOrEmpty(key))
            {
                //序列化0,2,3单一对象的时候，由于分析得到的结果是一个数组Node，所以返回第一项就可以了
                if (this.ContainerSignal == ContainerSignal.Empty && this.nodes.Count == 1 && this.nodes[0].NodeType == ContentNodeType.String)
                {
                    return this.nodes[0];
                }

                return null;
            }

            foreach (var node in this.nodes)
            {
                if (key.Equals(node.Key, comparison))
                    return node;
            }

            return null;
        }

        /// <summary>
        /// 当前的条数
        /// </summary>
        public virtual int Count
        {
            get
            {
                return this.nodes.Count;
            }
        }

        /// <summary>
        /// 当前内容的容器结构类型
        /// </summary>
        public ContainerSignal ContainerSignal { get; protected set; }

        /// <summary>
        /// 重新组织节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDeserializerReader Parse(IObjectContentNode node)
        {
            return new ThunderReader(node);
        }

        /// <summary>
        /// 读取下一个节点
        /// </summary>
        /// <returns></returns>
        public virtual IObjectContentNode MoveNext()
        {
            if (this.nodes.Count == 0)
                return null;

            if (this.nodes.Count <= this.move + 1)
                return null;

            this.move++;
            return this.nodes[this.move];
        }

        /// <summary>
        /// 重置读取器
        /// </summary>
        public void Reset()
        {
            this.move = -1;
        }

        #endregion IDeserializerReader

        #region parse

        /// <summary>
        /// 把当前json文本转为 object json节点，如果是递归使用的，返回值应该是最后递归一层所遇到结束标签对应的类型
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="reader"></param>
        /// <param name="level"></param>
        /// <param name="containerType"></param>
        /// <param name="arrayLevel"></param>
        protected virtual void ParseObject(IList<JsonContentNode> nodes, SequenceStringReader reader, int level, ContentNodeType containerType, int arrayLevel)
        {
            /*先把数据按标准排序{'a':'eg'}
            （1）[一定要在最前面，如果遇到回车，空格，先吃了这些字符
            （2）读取到最后，一定要判断]在最后的位置
            */

            /*删除回车，空格字符*/
            reader.SkipSpaceEnterResult();
            if (reader.Current != '{')
                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

            var start = reader.Head;
            /*清空空格*/
            var rollback = reader.SkipSpaceEnterResult();

            /*非法字符与结束字符*/
            switch (reader.Current)
            {
                case '}':
                    {
                        if (containerType != ContentNodeType.Object)
                            throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                        nodes.Add(new JsonContentNode(this.chars, start, start)
                        {
                            Key = "",
                            Escaping = false,
                            ValueQuote = ValueQuoteSignal.No,
                            Original = reader.Original,
                            Level = level,
                            Node = null,
                            NodeType = ContentNodeType.Object,
                        });

                        while (reader.Read())
                        {
                            if (!reader.IsWhiteSpaceChangeLine())
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                        }

                        return;
                    }
                case '[':
                    {
                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                    }
                case ']':
                    {
                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                    }
                case '\\':
                    {
                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                    }
                case ',':
                    {
                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                    }
                case ':':
                    {
                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                    }
            }

            /*清空开始标签*/
            reader.ClearStart();

            /*清空转义标签*/
            reader.ClearEscaping();

            /*是否在读取value*/
            bool readingValue = false;
            /*当前节点*/
            JsonContentNode node = null;
            if (rollback)
            {
                /*回滚到上一个字符*/
                reader.Rollback(1);
                reader.RollbackStep = 0;
            }
            byte empty = 0;
            while (reader.Read())
            {
                if (reader.IsWhiteSpaceChangeLine())
                {
                    /*遇上这个字符，要看清空空格，回车后得到的第一个字符，然后回滚到while那里读取*/
                    reader.SkipSpaceEnterResult();
                    reader.ClearToStart(reader.Head);
                    if (reader.Ending)
                    {
                        if (empty == 0)
                            return;

                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                    }
                }

                empty = 1;
                if (!readingValue)
                {
                    readingValue = true;
                    reader.Rollback(1);
                    while (reader.Read())
                    {
                        node = new JsonContentNode() { Original = this.original, };
                        this.ReadKey(reader, node, containerType);
                        if (reader.Current == '}' && node.Key.IsNullOrEmpty())
                            return;

                        /*对value进行标识，清空特定字符*/
                        if (reader.SkipSpaceEnterResult())
                        {
                            /*回滚到上一个字符*/
                            reader.Rollback(1);
                            reader.RollbackStep = 0;
                        }
                        break;
                    }
                }
                else
                {
                    readingValue = false;
                    reader.Rollback(1);
                    while (reader.Read())
                    {
                        if (reader.Current == '\'' || reader.Current == '\"')
                        {
                            this.ReadValue(reader, node, ContentNodeType.Object);
                            nodes.Add(node);
                            /*清空开始标签*/
                            reader.ClearStart();
                            /*清空转义标签*/
                            reader.ClearEscaping();
                            if (reader.Ending)
                            {
                                if (reader.Current != '}')
                                    throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                                return;
                            }

                            if (reader.Current == ',')
                                break;

                            if (reader.Current == '}')
                                return;

                            /*读取完后，要清理空格，如果还有下一个字符的话，那应该只能是,了*/
                            reader.SkipSpaceEnterResult();
                            if (reader.Ending)
                            {
                                if (reader.Current != '}')
                                    throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                                return;
                            }

                            if (reader.Current == '}')
                                return;

                            if (reader.Current != ',')
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            break;
                        }
                        else if (reader.Current == '[')
                        {
                            node.Node = new List<JsonContentNode>();
                            node.Original = reader.Original;
                            node.NodeType = ContentNodeType.Array;
                            var instart = reader.Head + 1;
                            this.ParseArray((List<JsonContentNode>)node.Node, reader, level + 1, ContentNodeType.Array, 0);
                            node.Segment = new ArraySegment<char>(this.chars, instart, reader.Head - instart);
                            nodes.Add(node);
                            /*清空开始标签*/
                            reader.ClearStart();
                            /*清空转义标签*/
                            reader.ClearEscaping();

                            /*当前节点必定是]*/
                            reader.Move().SkipSpaceEnterResult();
                            if (reader.Ending)
                            {
                                if (reader.Current != '}')
                                    throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                                return;
                            }

                            if (reader.Current == ',')
                                break;

                            if (reader.Current == '}')
                                return;

                            /*读取完后，要清理空格，如果还有下一个字符的话，那应该只能是,了*/
                            reader.SkipSpaceEnterResult();
                            if (reader.Ending)
                            {
                                if (reader.Current != '}')
                                    throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                                return;
                            }

                            if (reader.Current == '}')
                                return;

                            if (reader.Current != ',')
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            break;
                        }
                        else if (reader.Current == '{')
                        {
                            node.Node = new List<JsonContentNode>();
                            node.Original = reader.Original;
                            node.NodeType = ContentNodeType.Object;
                            var instart = reader.Head + 1;
                            this.ParseObject((List<JsonContentNode>)node.Node, reader, level + 1, ContentNodeType.Object, arrayLevel);
                            node.Segment = new ArraySegment<char>(this.chars, instart, reader.Head - instart);
                            nodes.Add(node);
                            /*清空开始标签*/
                            reader.ClearStart();
                            /*清空转义标签*/
                            reader.ClearEscaping();
                            /*当前节点必定是}*/
                            reader.Move().SkipSpaceEnterResult();
                            if (reader.Ending)
                            {
                                if (reader.Current != '}')
                                    throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                                return;
                            }

                            if (reader.Current == ',')
                                break;

                            if (reader.Current == '}')
                                return;

                            /*读取完后，要清理空格，如果还有下一个字符的话，那应该只能是,了*/
                            reader.SkipSpaceEnterResult();
                            if (reader.Ending)
                            {
                                if (reader.Current != '}')
                                    throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                                return;
                            }

                            if (reader.Current == '}')
                                return;

                            if (reader.Current != ',')
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            break;
                        }
                        else
                        {
                            this.ReadValue(reader, node, ContentNodeType.Object);
                            nodes.Add(node);
                            /*清空开始标签*/
                            reader.ClearStart();
                            /*清空转义标签*/
                            reader.ClearEscaping();

                            if (reader.Ending)
                            {
                                if (reader.Current != '}')
                                    throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                                return;
                            }

                            if (reader.Current == ',')
                                break;

                            if (reader.Current == '}')
                                return;

                            /*读取完后，要清理空格，如果还有下一个字符的话，那应该只能是,了*/
                            reader.SkipSpaceEnterResult();
                            if (reader.Ending)
                            {
                                if (reader.Current != '}')
                                    throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                                return;
                            }

                            if (reader.Current == '}')
                                return;

                            if (reader.Current != ',')
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 把当前json文本转为 array json节点，如果是递归使用的，返回值应该是最后递归一层所遇到结束标签对应的类型
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="reader"></param>
        /// <param name="level"></param>
        /// <param name="containerType"></param>
        /// <param name="arrayLevel"></param>
        protected virtual void ParseArray(IList<JsonContentNode> nodes, SequenceStringReader reader, int level, ContentNodeType containerType, int arrayLevel)
        {
            /*先把数据按标准排序['a','c',']']
            （1）[一定要在最前面，如果遇到回车，空格，先吃了这些字符
            （2）读取到最后，一定要判断]在最后的位置
            */

            /*删除回车，空格字符*/
            reader.SkipSpaceEnterResult();
            if (reader.Current != '[')
                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

            var start = reader.Head;
            /*清空空格*/
            var rollback = reader.SkipSpaceEnterResult();

            /*非法字符与结束字符*/
            switch (reader.Current)
            {
                case ']':
                    {
                        nodes.Add(new JsonContentNode(this.chars, reader.Head, reader.Head)
                        {
                            Key = "",
                            Escaping = false,
                            ValueQuote = ValueQuoteSignal.No,
                            Original = reader.Original,
                            Level = level,
                            Node = null,
                            NodeType = ContentNodeType.Array,
                        });

                        return;
                    }
                case '\\':
                    {
                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                    }
                case ',':
                    {
                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                    }
                case ':':
                    {
                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                    }
            }

            /*清空开始标签*/
            reader.ClearStart();
            /*清空转义标签*/
            reader.ClearEscaping();
            if (rollback)
            {
                /*回滚到上一个字符*/
                reader.Rollback(1);
                reader.RollbackStep = 0;
            }

            byte empty = 0;
            /*开始循环读取*/
            while (reader.Read())
            {
                if (reader.IsWhiteSpaceChangeLine())
                {
                    /*遇上这个字符，要看清空空格，回车后得到的第一个字符，然后回滚到while那里读取*/
                    reader.SkipSpaceEnterResult();
                    reader.ClearToStart(reader.Head);
                    if (reader.Ending)
                    {
                        if (empty == 0)
                            return;

                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                    }
                }

                empty = 1;
                switch (reader.Current)
                {
                    case '\"':
                    case '\'':
                        {
                            var node = new JsonContentNode() { Original = this.original, Key = "" };
                            this.ReadValue(reader, node, ContentNodeType.Array);
                            nodes.Add(node);
                            /*清空开始标签*/
                            reader.ClearStart();
                            /*清空转义标签*/
                            reader.ClearEscaping();
                            if (reader.Ending)
                            {
                                if (reader.Current != ']')
                                    throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                                return;
                            }

                            if (reader.Current == ']')
                                return;

                            /*读取完后，要清理空格，如果还有下一个字符的话，那应该只能是,了*/
                            reader.SkipSpaceEnterResult();

                            if (reader.Current != ',')
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                        }
                        break;

                    default:
                        {
                            var node = new JsonContentNode() { Original = this.original, Key = "" };
                            this.ReadValue(reader, node, ContentNodeType.Array);
                            nodes.Add(node);
                            node = null;
                            /*清空开始标签*/
                            reader.ClearStart();
                            /*清空转义标签*/
                            reader.ClearEscaping();

                            if (reader.Ending)
                            {
                                if (reader.Current == ']')
                                    return;

                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                            }

                            if (reader.Current == ']')
                                return;

                            /*读取完后，要清理空格，如果还有下一个字符的话，那应该只能是,了*/
                            reader.SkipSpaceEnterResult();
                            if (reader.Current != ',')
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                        }
                        break;

                    case '[':
                        {
                            /*数组里面开了数组*/
                            JsonContentNode arrayNode = null;
                            arrayNode = new JsonContentNode()
                            {
                                NodeType = ContentNodeType.Array,
                                Key = string.Empty,
                                Level = level,
                                Original = reader.Original,
                                Node = new List<JsonContentNode>(),
                                ArrayLevel = arrayLevel + 1,
                            };

                            var instart = reader.Head + 1;
                            this.ParseArray((List<JsonContentNode>)arrayNode.Node, reader, level + 1, ContentNodeType.Array, arrayLevel + 1);
                            arrayNode.Segment = new ArraySegment<char>(this.chars, instart, reader.Head - instart);
                            nodes.Add(arrayNode);
                            /*清空开始标签*/
                            reader.ClearStart();
                            /*清空转义标签*/
                            reader.ClearEscaping();
                            if (reader.Ending)
                            {
                                if (reader.Current == ']')
                                    return;

                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                            }

                            /* [ [] ]
                             * [ []       , [], [] ]
                             * [ {} ]
                             * [ {}, {}]
                             */

                            if (reader.Current != ']')
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            /*读取完后，要清理空格，如果还有下一个字符的话，那应该只能是,了*/
                            reader.Move().SkipSpaceEnterResult();
                            if (reader.Current == ']')
                                return;
                            //switch (containerType)
                            //{
                            //    case ContentNodeType.Object:
                            //        {
                            //            if (reader.Current == '}')
                            //                return;
                            //        }
                            //        break;
                            //    case ContentNodeType.Array:
                            //        {
                            //            if (reader.Current == ']')
                            //                return;
                            //        }
                            //        break;
                            //}

                            if (reader.Current != ',')
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                        }
                        break;

                    case '{':
                        {
                            /*数组里面开了对象*/
                            JsonContentNode arrayNode = null;
                            arrayNode = new JsonContentNode()
                            {
                                NodeType = ContentNodeType.Object,
                                Key = string.Empty,
                                Level = level,
                                Original = reader.Original,
                                Node = new List<JsonContentNode>(),
                            };

                            var instart = reader.Head + 1;
                            this.ParseObject((List<JsonContentNode>)arrayNode.Node, reader, level + 1, ContentNodeType.Object, arrayLevel);
                            arrayNode.Segment = new ArraySegment<char>(this.chars, instart, reader.Head - instart);
                            nodes.Add(arrayNode);
                            /*清空开始标签*/
                            reader.ClearStart();
                            /*清空转义标签*/
                            reader.ClearEscaping();
                            if (reader.Ending)
                            {
                                if (reader.Current == '}')
                                    return;

                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                            }

                            /* { a:b }
                             * { a:[],b:[] }
                             * { a:{},b:{} }
                             */
                            /*由于对象必定以}结束*/
                            if (reader.Current != '}')
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            /*读取完后，要清理空格，如果还有下一个字符的话，那应该只能是,了*/
                            reader.Move().SkipSpaceEnterResult();
                            if (reader.Current == ']')
                                return;

                            //switch (containerType)
                            //{
                            //    case ContentNodeType.Object:
                            //        {
                            //            if (reader.Current == '}')
                            //                return;
                            //        }
                            //        break;
                            //    case ContentNodeType.Array:
                            //        {
                            //            if (reader.Current == ']')
                            //                return;
                            //        }
                            //        break;
                            //}

                            if (reader.Current != ',')
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 读取key，只有key-value的时候才存在，结束符必定为 :
        /// </summary>
        protected virtual void ReadKey(SequenceStringReader reader, JsonContentNode node, ContentNodeType style)
        {
            /*先忽略回车，空格，因为key一定会带引号，如果不带，则遇到空格也一起忽略*/
            reader.SkipSpaceEnterResult();

            /*标识开始*/
            reader.Start(reader.Head);

            /*先确定key的引号类型*/
            switch (reader.Current)
            {
                case '\'':
                    {
                        node.ValueQuote = ValueQuoteSignal.Single;
                        reader.ClearToStart(reader.Head + 1);
                    }
                    break;

                case '\"':
                    {
                        node.ValueQuote = ValueQuoteSignal.Double;
                        reader.ClearToStart(reader.Head + 1);
                    }
                    break;

                case '}':
                    {
                        if (style == ContentNodeType.Object)
                            return;
                    }
                    break;

                default:
                    {
                        node.ValueQuote = ValueQuoteSignal.No;
                    }
                    break;
            }

            /*循环读取*/
            while (reader.Read())
            {
                switch (reader.Current)
                {
                    case '\r':
                    case '\n':
                    case '\t':
                    case ' ':
                        {
                            if (node.ValueQuote != ValueQuoteSignal.No)
                                continue;

                            /*遇上了空格，读取key值*/
                            node.Key = this.Substring(reader.Original, reader.Started, reader.Head);
                            reader.SkipSpaceEnterResult();
                            if (reader.Ending)
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            if (reader.Current != ':')
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            /*清空标记*/
                            reader.ClearStart();
                            return;
                        }

                    case ':':
                        {
                            /*只有没有引号的才会读取到这个值*/
                            if (node.ValueQuote != ValueQuoteSignal.No)
                                continue;

                            node.Key = this.Substring(reader.Original, reader.Started, reader.Head);
                            /*清空标记*/
                            reader.ClearStart();
                            return;
                        }

                    case '\'':
                        {
                            node.Key = this.Substring(reader.Original, reader.Started, reader.Head);
                            /*移动到下一个节点，去掉了'\\'*/
                            reader.Move();
                            reader.SkipSpaceEnterResult();
                            if (reader.Ending)
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            if (reader.Current != ':')
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            /*清空标记*/
                            reader.ClearStart();
                            return;
                        }

                    case '\"':
                        {
                            node.Key = this.Substring(reader.Original, reader.Started, reader.Head);
                            /*移动到下一个节点，去掉了'\\'*/
                            reader.Move();
                            reader.SkipSpaceEnterResult();
                            if (reader.Ending)
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            if (reader.Current != ':')
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            /*清空标记*/
                            reader.ClearStart();
                            return;
                        }

                    case '\\':
                        {
                            /*如果一个key遇上转义符，这里应该报错才行*/
                            /*读取到转义符号，往下读一个字段*/
                            if (reader.Read())
                            {
                                switch (reader.Current)
                                {
                                    case 'b':
                                        {
                                        }
                                        break;

                                    case 't':
                                        {
                                        }
                                        break;

                                    case 'n':
                                        {
                                        }
                                        break;

                                    case '"':
                                        {
                                        }
                                        break;

                                    case 'f':
                                        {
                                        }
                                        break;

                                    case 'r':
                                        {
                                        }
                                        break;

                                    case '\\':
                                        {
                                        }
                                        break;

                                    case '\'':
                                        {
                                        }
                                        break;

                                    case '/':
                                        {
                                        }
                                        break;

                                    case 'u':
                                        {
                                        }
                                        break;

                                    default:
                                        {
                                            throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                                        }
                                }
                            }
                        }
                        break;
                }
            }

            throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
        }

        /// <summary>
        /// 读取vallue，返回结果会有 ']'，'}'，'Ending'，','
        /// </summary>
        protected virtual void ReadValue(SequenceStringReader reader, JsonContentNode node, ContentNodeType style)
        {
            if (reader.Current == '\'' || reader.Current == '\"')
            {
                /*标识当前索引*/
                reader.Start(reader.Head);
            }
            else
            {
                /*先忽略回车，空格，因为key一定会带引号，如果不带，则遇到空格也一起忽略*/
                reader.SkipSpaceEnterResult();

                /*标识开始*/
                reader.Start(reader.Head);
            }

            switch (reader.Current)
            {
                default:
                    {
                        node.ValueQuote = ValueQuoteSignal.No;
                        reader.Start(reader.Head);
                    }
                    break;

                case ']':
                    {
                        if (style == ContentNodeType.Array)
                        {
                            node.Segment = this.ToArray(reader.Started, reader.Head);
                            node.Original = reader.Original;
                            node.NodeType = ContentNodeType.String;
                            node.Escaping = reader.Escaping;
                            return;
                        }

                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                    }
                case '}':
                    {
                        if (style == ContentNodeType.Object)
                        {
                            node.Segment = this.ToArray(reader.Started, reader.Head);
                            node.Original = reader.Original;
                            node.NodeType = ContentNodeType.String;
                            node.Escaping = reader.Escaping;
                            return;
                        }

                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                    }
                case ',':
                case ';':
                case '\\':
                    {
                        /*没有引号的，进来就是\,,,;*/
                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                    }
                case '\'':
                    {
                        node.ValueQuote = ValueQuoteSignal.Single;
                        /*重新定位开始字符位置*/
                        reader.ClearToStart(reader.Head + 1);
                    }
                    break;

                case '\"':
                    {
                        node.ValueQuote = ValueQuoteSignal.Double;
                        /*重新定位开始字符位置*/
                        reader.ClearToStart(reader.Head + 1);
                    }
                    break;
            }

            int skipCount = 0;
            while (reader.Read())
            {
                switch (reader.Current)
                {
                    case '\u0020':
                    case '\u0009':
                    case '\u000A':
                    case '\u000D':
                        {
                            /*带引号的节点，如果在没有遇上引号而先遇上了这些]}结束符号，则认为是节点内容的*/
                            if (node.ValueQuote != ValueQuoteSignal.No)
                                continue;

                            skipCount = reader.SkipSpaceEnterCount();
                            if (skipCount > 1 && reader.IsWhiteSpaceChangeLine())
                                skipCount--;

                            //if (skipCount > 1)
                            //    skipCount--;
                            //else
                            //{
                            //    if (reader.Ending && skipCount > 0)
                            //    {
                            //        if (reader.IsWhiteSpaceChangeLine())
                            //        {
                            //        }
                            //        else
                            //        {
                            //            skipCount--;
                            //        }
                            //    }
                            //}

                            switch (style)
                            {
                                case ContentNodeType.String:
                                    {
                                        if (reader.Ending)
                                        {
                                            node.Segment = this.ToArray(reader.Started, reader.Head - skipCount);
                                            node.Original = reader.Original;
                                            node.NodeType = ContentNodeType.String;
                                            node.Escaping = reader.Escaping;
                                            return;
                                        }

                                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                                    }
                                case ContentNodeType.Array:
                                    {
                                        if (reader.Current == ']')
                                        {
                                            node.Segment = this.ToArray(reader.Started, reader.Head - skipCount);
                                            node.Original = reader.Original;
                                            node.NodeType = ContentNodeType.String;
                                            node.Escaping = reader.Escaping;
                                            return;
                                        }
                                        else if (reader.Current == ',')
                                        {
                                            if (node.ValueQuote == ValueQuoteSignal.No)
                                            {
                                                /*没有引号的，读取到这个值，应该是分割符号了*/
                                                node.Segment = this.ToArray(reader.Started, reader.Head - skipCount);
                                                node.Original = reader.Original;
                                                node.NodeType = ContentNodeType.String;
                                                node.Escaping = reader.Escaping;
                                                return;
                                            }
                                        }
                                    }
                                    break;

                                case ContentNodeType.Object:
                                    {
                                        if (reader.Current == '}')
                                        {
                                            node.Segment = this.ToArray(reader.Started, reader.Head - skipCount);
                                            node.Original = reader.Original;
                                            node.NodeType = ContentNodeType.String;
                                            node.Escaping = reader.Escaping;
                                            return;
                                        }
                                        else if (reader.Current == ',')
                                        {
                                            if (node.ValueQuote == ValueQuoteSignal.No)
                                            {
                                                /*没有引号的，读取到这个值，应该是分割符号了*/
                                                node.Segment = this.ToArray(reader.Started, reader.Head - skipCount);
                                                node.Original = reader.Original;
                                                node.NodeType = ContentNodeType.String;
                                                node.Escaping = reader.Escaping;
                                                return;
                                            }
                                        }
                                    }
                                    break;
                            }
                            throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                        }

                    case '\\':
                        {
                            reader.Escaped();
                            /*读取到转义符号，要在这里开始*/
                            if (reader.Read())
                            {
                                switch (reader.Current)
                                {
                                    case 'b':
                                        {
                                        }
                                        break;

                                    case 't':
                                        {
                                        }
                                        break;

                                    case 'n':
                                        {
                                        }
                                        break;

                                    case '"':
                                        {
                                        }
                                        break;

                                    case 'f':
                                        {
                                        }
                                        break;

                                    case 'r':
                                        {
                                        }
                                        break;

                                    case '\\':
                                        {
                                        }
                                        break;

                                    case '\'':
                                        {
                                        }
                                        break;

                                    case '/':
                                        {
                                        }
                                        break;

                                    case 'u':
                                        {
                                        }
                                        break;

                                    default:
                                        {
                                            throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
                                        }
                                }
                            }
                        }
                        break;

                    case ']':
                        {
                            /*带引号的节点，如果在没有遇上引号而先遇上了这些]}结束符号，则认为是节点内容的*/
                            if (node.ValueQuote != ValueQuoteSignal.No)
                                continue;

                            /*能遇到这个字符，如果没有引号装饰，该串表示有问题,因为引号装饰的不可能会达到这里，只能跳到引号那里*/
                            if (style != ContentNodeType.Array)
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            node.Segment = this.ToArray(reader.Started, reader.Head);
                            node.Original = reader.Original;
                            node.NodeType = ContentNodeType.String;
                            node.Escaping = reader.Escaping;
                            return;
                        }

                    case '}':
                        {
                            /*带引号的节点，如果在没有遇上引号而先遇上了这些]}结束符号，则认为是节点内容的*/
                            if (node.ValueQuote != ValueQuoteSignal.No)
                                continue;

                            /*能遇到这个字符，如果没有引号装饰，该串表示有问题,因为引号装饰的不可能会达到这里，只能跳到引号那里*/
                            if (style != ContentNodeType.Object)
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            node.Segment = this.ToArray(reader.Started, reader.Head);
                            node.Original = reader.Original;
                            node.NodeType = ContentNodeType.String;
                            node.Escaping = reader.Escaping;
                            return;
                        }

                    case '\'':
                        {
                            if (node.ValueQuote == ValueQuoteSignal.Double)
                                continue;

                            if (node.ValueQuote != ValueQuoteSignal.Single)
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            node.Segment = this.ToArray(reader.Started, reader.Head);
                            node.Original = reader.Original;
                            node.NodeType = ContentNodeType.String;
                            node.Escaping = reader.Escaping;

                            /*再次读取下一个字符，要找到结束符*/
                            reader.Move().SkipSpaceEnterResult();
                            return;
                        }

                    case '\"':
                        {
                            if (node.ValueQuote != ValueQuoteSignal.Double)
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            node.Segment = this.ToArray(reader.Started, reader.Head);
                            node.Original = reader.Original;
                            node.NodeType = ContentNodeType.String;
                            node.Escaping = reader.Escaping;

                            /*再次读取下一个字符，要找到结束符*/
                            reader.Move().SkipSpaceEnterResult();
                            return;
                        }

                    case ',':
                        {
                            if (node.ValueQuote != ValueQuoteSignal.No)
                                continue;

                            /*没有引号的，读取到这个值，应该是分割符号了*/
                            node.Segment = this.ToArray(reader.Started, reader.Head);
                            node.Original = reader.Original;
                            node.NodeType = ContentNodeType.String;
                            node.Escaping = reader.Escaping;
                            return;
                        }

                    default:
                        {
                            if (!reader.Ending)
                                continue;

                            if (style != ContentNodeType.String)
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            if (node.ValueQuote != ValueQuoteSignal.No)
                                throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));

                            node.Segment = this.ToArray(reader.Started, reader.Head + 1);
                            node.Original = reader.Original;
                            node.NodeType = ContentNodeType.String;
                            node.Escaping = reader.Escaping;
                            return;
                        }
                }
            }

            throw new ArgumentException(string.Format("在读取{0}处字符失败了", reader.Head.ToString()));
        }

        /// <summary>
        /// 读取vallue，直接读取所有值
        /// </summary>
        protected virtual void ReadAllContent(SequenceStringReader reader, JsonContentNode node, int head)
        {
            var @char = new char[reader.Length - head];
            var move = 0;
            switch (node.ValueQuote)
            {
                case ValueQuoteSignal.No:
                    {
                        for (var i = reader.Length - 1; i >= head; i--)
                        {
                            move = i;
                            if (reader.IsWhiteSpaceChangeLine(reader.Original[i]))
                                continue;

                            move++;
                            node.Segment = new ArraySegment<char>(this.chars, head, move - head);
                            return;
                        }

                        node.Segment = new ArraySegment<char>(this.chars);
                        return;
                    }
                case ValueQuoteSignal.Double:
                    {
                        for (var i = head; i <= reader.Length - 1; i++)
                        {
                            if (reader.Original[i] == '\\' && reader.NextMatch('\"'))
                            {
                                i++;
                                continue;
                            }

                            if (reader.Original[i] == '\"')
                            {
                                move = i;
                                if (i == reader.Length - 1)
                                {
                                    node.Segment = new ArraySegment<char>(this.chars, head, move - head);
                                    return;
                                }
                                else
                                {
                                    i++;
                                    while (true)
                                    {
                                        if (i == reader.Length - 1)
                                            break;

                                        if (reader.IsWhiteSpaceChangeLine(reader.Original[i]))
                                            continue;

                                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", i.ToString()));
                                    }
                                }

                                node.Segment = new ArraySegment<char>(this.chars, head, move - head);
                                return;
                            }
                        }

                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", (reader.Length - 1).ToString()));
                    }
                case ValueQuoteSignal.Single:
                    {
                        for (var i = head; i <= reader.Length - 1; i++)
                        {
                            if (reader.Original[i] == '\\' && reader.NextMatch('\"'))
                            {
                                i++;
                                continue;
                            }

                            if (reader.Original[i] == '\'')
                            {
                                move = i;
                                if (i == reader.Length - 1)
                                {
                                    node.Segment = new ArraySegment<char>(this.chars, head, move - head);
                                    return;
                                }
                                else
                                {
                                    i++;
                                    while (true)
                                    {
                                        if (i == reader.Length - 1)
                                            break;

                                        if (reader.IsWhiteSpaceChangeLine(reader.Original[i]))
                                            continue;
                                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", i.ToString()));
                                    }
                                }

                                node.Segment = new ArraySegment<char>(this.chars, head, move - head);
                                return;
                            }
                        }

                        throw new ArgumentException(string.Format("在读取{0}处字符失败了", (reader.Length - 1).ToString()));
                    }
            }

            throw new ArgumentException(string.Format("在读取{0}处字符失败了", (reader.Length - 1).ToString()));
        }

        #endregion parse

        #region utils

        ///// <summary>
        ///// 将字符串创建一个char数组
        ///// </summary>
        ///// <param name="json">字符串内容</param>
        ///// <param name="start">开始索引</param>
        ///// <param name="end">结束索引</param>
        ///// <returns></returns>
        //public char[] ToArray(string json, int start, int end)
        //{
        //    var ch = new char[end - start];
        //    for (var i = start; i < end && i < json.Length; i++)
        //    {
        //        ch[i - start] = json[i];
        //    }
        //    return ch;
        //}

        /// <summary>
        /// 将字符串创建一个char数组
        /// </summary>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <returns></returns>
        public ArraySegment<char> ToArray(int start, int end)
        {
            var offset = 0;
            for (var i = start; i < end && i < this.chars.Length; i++)
            {
                offset++;
            }

            return new ArraySegment<char>(this.chars, start, offset);
        }

        /// <summary>
        /// 将字符串创建一个char数组
        /// </summary>
        /// <param name="json">字符串内容</param>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <returns></returns>
        public string Substring(string json, int start, int end)
        {
            var offset = 0;
            for (var i = start; i < end && i < this.chars.Length; i++)
            {
                offset++;
            }

            return json.Substring(start, offset);
        }

        #endregion utils

        #region tostring

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.original;
        }

        #endregion
    }
}