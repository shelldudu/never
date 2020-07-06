using Never.EasySql.Labels;
using Never.EasySql.Xml;
using Never.Exceptions;
using Never.Serialization.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Never.EasySql
{
    /// <summary>
    /// sqltag
    /// </summary>
    public class SqlTag
    {
        #region prop

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// 所有节点
        /// </summary>
        public IEnumerable<ILabel> Labels { get; internal set; }

        /// <summary>
        /// 所有节点总长度
        /// </summary>
        public int TextLength { get; internal set; }

        #endregion

        #region format

        /// <summary>
        /// 格式化内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public virtual SqlTagFormat Format<T>(EasySqlParameter<T> parameter)
        {
            var format = new SqlTagFormat(this.TextLength, parameter.Count) { Id = this.Id };
            if (this.Labels.Any())
            {
                var convert = parameter.Convert();
                foreach (var line in this.Labels)
                {
                    line.Format(format, parameter, convert);
                }
            }

            return format;
        }

        /// <summary>
        /// 格式化内容，返回文本内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public virtual SqlTagFormat FormatForText<T>(EasySqlParameter<T> parameter)
        {
            var format = new SqlTagFormat.TextSqlTagFormat(this.TextLength, parameter.Count) { Id = this.Id };
            if (this.Labels.Any())
            {
                var convert = parameter.Convert();
                foreach (var line in this.Labels)
                {
                    line.Format(format, parameter, convert);
                }
            }

            return format;
        }

        #endregion format

        #region 读取

        /// <summary>
        /// 读取textLabel
        /// </summary>
        /// <param name="text"></param>
        /// <param name="writer"></param>
        /// <param name="readerHelper"></param>
        /// <param name="parameterPrefix"></param>
        /// <returns></returns>
        public TextLabel ReadTextNode(string text, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
        {
            var i = -1;
            var length = text.Length;
            var label = new TextLabel() { };
            bool escape = false;
            while (true)
            {
                i++;
                if (i >= length)
                {
                    if (escape == true)
                    {
                        throw new DataFormatException("sql tag {0} can not find ' char on ending;", this.Id);
                    }
                    break;
                }

                switch (text[i])
                {
                    case '\'':
                        {
                            writer.Write('\'');
                            if (escape == false)
                            {
                                escape = true;
                            }
                            else
                            {
                                escape = false;
                            }
                        }
                        break;

                    case '$':
                        {
                            writer.Write(parameterPrefix);
                            var start = i;
                            int end;
                            while (true)
                            {
                                start++;
                                if (text.Length <= start)
                                {
                                    throw new ArgumentOutOfRangeException($"start with $ must end with $,position is {start}");
                                }

                                if (text[start] == '$')
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
                                ParameterStartIndex = i + parameterPrefix.Length,
                                PrefixStartIndex = i,
                                ParameterStopIndex = end,
                                OccupanLength = end - i,
                                TextParameter = true,
                            };

                            label.Add(parameter);
                            i = start;
                        }
                        break;

                    case '@':
                        {
                            var start = i;
                            if (text.Length > i + 1 && text[i + 1] == '@')
                            {
                                while (true)
                                {
                                    start++;
                                    if (text.Length <= start)
                                    {
                                        label.SqlText = writer.ToString();
                                        this.TextLength += label.SqlText.Length;
                                        writer.Clear();
                                        return label;
                                    }

                                    writer.Write(text[start]);
                                    if (this.Pause(readerHelper, text[start]))
                                    {
                                        label.SqlText = writer.ToString();
                                        this.TextLength += label.SqlText.Length;
                                        writer.Clear();
                                        return label;
                                    }
                                }
                            };

                            writer.Write(parameterPrefix);
                            int end;
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
                                ParameterStartIndex = i + parameterPrefix.Length,
                                PrefixStartIndex = i,
                                ParameterStopIndex = end,
                                OccupanLength = end - i,
                                TextParameter = false,
                            };

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

        /// <summary>
        /// 读取textLabel
        /// </summary>
        /// <param name="text"></param>
        /// <param name="writer"></param>
        /// <param name="readerHelper"></param>
        /// <param name="parameterPrefix"></param>
        /// <returns></returns>
        public TextLabel ReadTextNodeUsingFormatLine(string text, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
        {
            var i = -1;
            var length = text.Length;
            var label = new TextLabel() { };
            var hack = 0;
            bool escape = false;
            char last = 'a';
            while (true)
            {
                i++;
                if (i >= length)
                {
                    if (escape == true)
                    {
                        throw new DataFormatException("sql tag {0} can not find ' char on ending;", this.Id);
                    }

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
                            writer.Write('\'');
                            if (escape == false)
                            {
                                escape = true;
                            }
                            else
                            {
                                escape = false;
                            }
                        }
                        break;

                    case '$':
                        {
                            last = '$';
                            writer.Write(parameterPrefix);
                            var start = i;
                            int end;
                            while (true)
                            {
                                start++;
                                if (text.Length <= start)
                                {
                                    throw new ArgumentOutOfRangeException($"start with $ must end with $,position is {start}");
                                }

                                if (text[start] == '$')
                                {
                                    end = start - 1;
                                    last = text[end];
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
                                ParameterStartIndex = (i + parameterPrefix.Length) - hack,
                                PrefixStartIndex = i - hack,
                                ParameterStopIndex = end - hack,
                                OccupanLength = end - i,
                                TextParameter = true,
                            };

                            label.Add(parameter);
                            i = start;
                        }
                        break;

                    case '@':
                        {
                            var start = i;
                            if (text.Length > i + 1 && text[i + 1] == '@')
                            {
                                while (true)
                                {
                                    start++;
                                    if (text.Length <= start)
                                    {
                                        label.SqlText = writer.ToString();
                                        this.TextLength += label.SqlText.Length;
                                        writer.Clear();
                                        return label;
                                    }

                                    writer.Write(text[start]);
                                    if (this.Pause(readerHelper, text[start]))
                                    {
                                        label.SqlText = writer.ToString();
                                        this.TextLength += label.SqlText.Length;
                                        writer.Clear();
                                        return label;
                                    }
                                }
                            };

                            last = parameterPrefix.Last();
                            writer.Write(parameterPrefix);
                            int end;
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
                                ParameterStartIndex = (i + parameterPrefix.Length) - hack,
                                PrefixStartIndex = i - hack,
                                ParameterStopIndex = end - hack,
                                OccupanLength = end - i,
                                TextParameter = false,
                            };

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

        /// <summary>
        /// 读取return标签
        /// </summary>
        /// <param name="text"></param>
        /// <param name="writer"></param>
        /// <param name="readerHelper"></param>
        /// <param name="parameterPrefix"></param>
        /// <returns></returns>
        public TextLabel ReadReturnNode(string text, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
        {
            return new TextLabel() { SqlText = text };
        }

        /// <summary>
        /// 读取return标签
        /// </summary>
        /// <param name="text"></param>
        /// <param name="writer"></param>
        /// <param name="readerHelper"></param>
        /// <param name="parameterPrefix"></param>
        /// <returns></returns>
        public TextLabel ReadReturnNodeUsingFormatLine(string text, ThunderWriter writer, SequenceStringReader readerHelper, string parameterPrefix)
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

        /// <summary>
        /// 参数停顿
        /// </summary>
        /// <param name="readerHelper"></param>
        /// <param name="char"></param>
        /// <returns></returns>
        protected bool Pause(SequenceStringReader readerHelper, char @char)
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

        /// <summary>
        /// 下一个字符是什么
        /// </summary>
        /// <param name="next"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public char? Next(string next, int i)
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