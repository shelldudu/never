using Never.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Labels
{
    /// <summary>
    /// 文本标签
    /// </summary>
    /// <seealso cref="Never.EasySql.Labels.BaseLabel" />
    public class TextLabel : BaseLabel
    {
        /// <summary>
        /// 标签类型
        /// </summary>
        /// <returns></returns>
        public override LabelType GetLabelType() => LabelType.Text;

        /// <summary>
        /// sql参数
        /// </summary>
        private List<SqlTagParameterPosition> parameterPositions = null;

        /// <summary>
        /// sql参数个数
        /// </summary>
        private int parameterPositionCount = 0;

        /// <summary>
        /// sql参数
        /// </summary>
        public IEnumerable<SqlTagParameterPosition> ParameterPositions
        {
            get
            {
                return this.parameterPositions == null ? Enumerable.Empty<SqlTagParameterPosition>() : this.parameterPositions.AsEnumerable();
            }
        }

        /// <summary>
        /// sql参数个数
        /// </summary>
        public int ParameterPositionCount
        {
            get
            {
                return this.parameterPositionCount;
            }
        }

        /// <summary>
        /// 新加参数
        /// </summary>
        /// <param name="parameter"></param>
        public void Add(SqlTagParameterPosition parameter)
        {
            if (this.parameterPositions == null)
            {
                this.parameterPositions = new List<SqlTagParameterPosition>();
                this.parameterPositions.Add(parameter);
                this.parameterPositionCount++;
                return;
            }

            this.parameterPositions.Add(parameter);
            this.parameterPositionCount++;
        }

        /// <summary>
        /// 新加参数
        /// </summary>
        /// <param name="parameters"></param>
        public void AddRange(IEnumerable<SqlTagParameterPosition> parameters)
        {
            if (this.parameterPositions == null)
            {
                this.parameterPositions = new List<SqlTagParameterPosition>();
                this.parameterPositions.AddRange(parameters);
                this.parameterPositionCount += parameters.Count();
                return;
            }

            this.parameterPositions.AddRange(parameters);
            this.parameterPositionCount += parameters.Count();
        }
        /// <summary>
        /// 复制参数
        /// </summary>
        /// <returns></returns>
        protected List<SqlTagParameterPosition> Copy()
        {
            return this.parameterPositions.ToList();
        }

        /// <summary>
        /// 是否找到参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="convert"></param>
        /// <returns></returns>

        public override bool ContainParameter<T>(EasySqlParameter<T> parameter, IReadOnlyList<KeyValueTuple<string, object>> convert)
        {
            return true;
        }

        /// <summary>
        /// 格式化sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="parameter"></param>
        /// <param name="convert"></param>
        public override void Format<T>(SqlTagFormat format, EasySqlParameter<T> parameter, IReadOnlyList<KeyValueTuple<string, object>> convert)
        {
            if (this.SqlText.IsNullOrEmpty())
                return;

            if (this.parameterPositions == null || this.parameterPositionCount == 0)
            {
                format.Write(this.SqlText);
                return;
            }

            int start = 0;
            foreach (var para in this.parameterPositions)
            {
                var item = convert.FirstOrDefault(o => o.Key.Equals(para.Name));
                if (item == null)
                    throw new InvalidException("the sql tag {0} need the {1} parameters;", format.Id, para.Name);

                var value = item.Value;
                if (item.Value is INullableParameter && ((INullableParameter)item.Value).HasValue)
                    value = ((INullableParameter)item.Value).Value;

                format.WriteOnTextMode(this.SqlText, start, para.PrefixStartIndex - start);
                if (value == null || value == DBNull.Value)
                {
                    if (format.IfTextParameter(para))
                    {
                        format.WriteOnTextMode("null");
                    }
                    else
                    {
                        format.WriteOnTextMode(this.SqlText, para.PrefixStartIndex, para.ParameterStopIndex - para.PrefixStartIndex + 1);
                        format.AddParameter(para.Name, DBNull.Value);
                    }
                }
                else
                {
                    if (format.IfTextParameter(para))
                    {
                        format.WriteOnTextMode(value.ToString());
                    }
                    else
                    {
                        format.WriteOnTextMode(this.SqlText, para.PrefixStartIndex, para.ParameterStopIndex - para.PrefixStartIndex + 1);
                        format.AddParameter(para.Name, value);
                    }
                }

                start = para.ParameterStopIndex + 1;
            }

            if (this.SqlText.Length >= start)
            {
                format.WriteOnTextMode(this.SqlText, start, this.SqlText.Length - start);
                return;
            }

            throw new Exception(string.Format("{0}字符串遍历出错", ""));
        }

        /// <summary>
        /// 格式化sql语句，是数组使用的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="parameter"></param>
        /// <param name="convert"></param>
        /// <param name="arrayLabel"></param>
        public void FormatArray<T>(SqlTagFormat format, ArrayLabel arrayLabel, EasySqlParameter<T> parameter, KeyValueTuple<string, object> convert)
        {
            if (this.SqlText.IsNullOrEmpty())
                return;

            if (this.parameterPositions == null || this.parameterPositionCount == 0)
            {
                format.Write(this.SqlText);
                return;
            }

            var ator = ((IEnumerable)convert.Value).GetEnumerator();
            int start = 0;
            foreach (var para in this.parameterPositions)
            {
                format.WriteOnTextMode(this.SqlText, start, para.PrefixStartIndex - start);
                var appendValue = false;
                var arrayLevel = 0;
                if (format.IfTextParameter(para))
                {
                    while (ator.MoveNext())
                    {
                        if (ator.Current == null || ator.Current == DBNull.Value)
                            continue;

                        if (appendValue)
                            format.WriteOnTextMode(arrayLabel.Split);

                        format.WriteOnTextMode(ator.Current.ToString());
                        appendValue = true;
                    }

                    start = para.ParameterStopIndex + 1;
                    continue;
                }

                while (ator.MoveNext())
                {
                    if (ator.Current == null || ator.Current == DBNull.Value)
                        continue;

                    var newvalue = (ator.Current == null || ator.Current == DBNull.Value) ? DBNull.Value : ator.Current;
                    var newkey = string.Format("{0}x{1}z", para.Name, arrayLevel);

                    if (appendValue)
                    {
                        format.Write(arrayLabel.Split);
                    }

                    format.Write(para.ActualPrefix);
                    format.Write(newkey);
                    arrayLevel++;

                    format.AddParameter(newkey, newvalue);
                    appendValue = true;
                }

                start = para.ParameterStopIndex + 1;
                continue;
            }

            if (this.SqlText.Length >= start)
            {
                format.WriteOnTextMode(this.SqlText, start, this.SqlText.Length - start);
                return;
            }

            throw new Exception(string.Format("{0}字符串遍历出错", ""));
        }

        /// <summary>
        /// 格式化sql语句，是数组使用的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="parameter"></param>
        /// <param name="convert"></param>
        /// <param name="arrayLevel"></param>
        /// <param name="arrayLabel"></param>
        public void FormatArray<T>(SqlTagFormat format, ArrayLabel arrayLabel, EasySqlParameter<T> parameter, IReadOnlyList<KeyValueTuple<string, object>> convert, int arrayLevel)
        {
            if (this.SqlText.IsNullOrEmpty())
                return;

            if (this.parameterPositions == null || this.parameterPositionCount == 0)
            {
                format.Write(this.SqlText);
                return;
            }

            if (this.parameterPositionCount == 1)
            {
                writeParameter(this.SqlText, this.parameterPositions[0], 0);
                return;
            }

            var copy = this.Copy();
            for (var i = 0; i < this.SqlText.Length; i++)
            {
                var para = this.MathPosition(copy, i);
                if (para == null)
                {
                    format.Write(this.SqlText[i]);
                    continue;
                }

                writeParameter(this.SqlText, para, i);
                i += para.OccupanLength - 1;
            }

            //写参数
            void writeParameter(string text, SqlTagParameterPosition parameterPosition, int index)
            {
                if (format.IfTextParameter(parameterPosition))
                {
                    var item = convert.FirstOrDefault(o => o.Key.Equals(parameterPosition.Name));
                    if (item == null)
                        throw new InvalidException("the sql tag {0} need the {1} parameters;", format.Id, parameterPosition.Name);

                    var value = item.Value;
                    if (item.Value is INullableParameter)
                        value = ((INullableParameter)item.Value).Value;

                    if (value == null || value == DBNull.Value)
                    {
                        format.WriteOnTextMode("null");
                    }
                    else
                    {
                        format.WriteOnTextMode(value.ToString());
                    }
                }
                else
                {
                    format.Write(text[index]);
                    var item = convert.FirstOrDefault(o => o.Key.Equals(parameterPosition.Name));
                    if (item == null)
                        throw new InvalidException("the sql tag {0} need the {1} parameters;", format.Id, parameterPosition.Name);

                    var value = item.Value;
                    if (item.Value is INullableParameter)
                        value = ((INullableParameter)item.Value).Value;

                    var newvalue = (value == null || value == DBNull.Value) ? DBNull.Value : value;
                    var newkey = string.Format("{0}x{1}z", parameterPosition.Name, arrayLevel);
                    format.Write(newkey);
                    format.AddParameter(newkey, newvalue);
                }
            }
        }
    }
}