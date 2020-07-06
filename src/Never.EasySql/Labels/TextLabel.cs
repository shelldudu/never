using Never.Exceptions;
using System;
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

            if (this.parameterPositionCount == 1)
            {
                var item = convert.FirstOrDefault(o => o.Key.Equals(this.parameterPositions[0].Name));
                if (item == null)
                    throw new InvalidException("the sql tag {0} need the {1} parameters;", format.Id, this.parameterPositions[0].Name);

                var value = item.Value;
                if (item.Value is IReferceNullableParameter)
                    value = ((IReferceNullableParameter)item.Value).Value;

                if (value == null)
                {
                    if (format.IfTextParameter(this.parameterPositions[0]))
                    {
                        format.WriteOnTextMode(this.SqlText, 0, this.parameterPositions[0].PrefixStartIndex);
                        //format.WriteOnTextMode("\'null\'");
                        //format.WriteOnTextMode("null");
                        format.WriteOnTextMode(this.SqlText, this.parameterPositions[0].ParameterStopIndex + 1, this.SqlText.Length - (this.parameterPositions[0].ParameterStopIndex + 1));
                    }
                    else
                    {
                        format.Write(this.SqlText);
                        format.AddParameter(this.parameterPositions[0].Name, DBNull.Value);
                    }
                }
                else if (value == DBNull.Value)
                {
                    if (format.IfTextParameter(this.parameterPositions[0]))
                    {
                        format.WriteOnTextMode(this.SqlText, 0, this.parameterPositions[0].PrefixStartIndex);
                        //format.WriteOnTextMode("\'null\'");
                        format.WriteOnTextMode("null");
                        format.WriteOnTextMode(this.SqlText, this.parameterPositions[0].ParameterStopIndex + 1, this.SqlText.Length - (this.parameterPositions[0].ParameterStopIndex + 1));
                    }
                    else
                    {
                        format.Write(this.SqlText);
                        format.AddParameter(this.parameterPositions[0].Name, DBNull.Value);
                    }
                }
                else
                {
                    if (format.IfTextParameter(this.parameterPositions[0]))
                    {
                        format.WriteOnTextMode(this.SqlText, 0, this.parameterPositions[0].PrefixStartIndex);
                        //format.WriteOnTextMode('\'');
                        format.WriteOnTextMode(value.ToString());
                        //format.WriteOnTextMode('\'');
                        format.WriteOnTextMode(this.SqlText, this.parameterPositions[0].ParameterStopIndex + 1, this.SqlText.Length - (this.parameterPositions[0].ParameterStopIndex + 1));
                    }
                    else
                    {
                        format.Write(this.SqlText);
                        format.AddParameter(this.parameterPositions[0].Name, value);
                    }
                }

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

                if (format.IfTextParameter(para))
                {
                    var item = convert.FirstOrDefault(o => o.Key.Equals(para.Name));
                    if (item == null)
                        throw new InvalidException("the sql tag {0} need the {1} parameters;", format.Id, para.Name);

                    var value = item.Value;
                    if (item.Value is IReferceNullableParameter)
                        value = ((IReferceNullableParameter)item.Value).Value;

                    if (value == null)
                    {
                        //format.WriteOnTextMode("\'null\'");
                        //format.WriteOnTextMode("null");
                    }
                    if (value == DBNull.Value)
                    {
                        //format.WriteOnTextMode("\'null\'");
                        format.WriteOnTextMode("null");
                    }
                    else
                    {
                        //format.WriteOnTextMode('\'');
                        format.WriteOnTextMode(value.ToString());
                        //format.WriteOnTextMode('\'');
                    }

                    i += para.OccupanLength + 1;
                    if (i < this.SqlText.Length)
                        format.WriteOnTextMode(this.SqlText[i]);
                }
                else
                {
                    var item = convert.FirstOrDefault(o => o.Key.Equals(para.Name));
                    if (item == null)
                        throw new InvalidException("the sql tag {0} need the {1} parameters;", format.Id, para.Name);

                    format.Write(this.SqlText, para.PrefixStartIndex, para.OccupanLength);
                    format.AddParameter(item);
                    i += para.OccupanLength;
                    if (i < this.SqlText.Length)
                        format.Write(this.SqlText[i]);
                }
            }
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

            var copy = this.Copy();
            for (var i = 0; i < this.SqlText.Length; i++)
            {
                var para = this.MathPosition(copy, i);
                if (para == null)
                {
                    format.Write(this.SqlText[i]);
                    continue;
                }

                var value = convert.Value;
                if (convert.Value is IReferceNullableParameter)
                    value = ((IReferceNullableParameter)convert.Value).Value;

                var item = value as System.Collections.IEnumerable;
                var ator = item.GetEnumerator();
                var hadA = false;
                var arrayLevel = 0;
                if (format.IfTextParameter(para))
                {
                    while (ator.MoveNext())
                    {
                        if (ator.Current == null || ator.Current == DBNull.Value)
                            continue;

                        if (hadA)
                            format.WriteOnTextMode(arrayLabel.Split);

                        //format.WriteOnTextMode('\'');
                        format.WriteOnTextMode(ator.Current.ToString());
                        //format.WriteOnTextMode('\'');
                        hadA = true;
                    }

                    i += para.OccupanLength + 1;
                    if (i < this.SqlText.Length)
                        format.WriteOnTextMode(this.SqlText[i]);
                }
                else
                {
                    format.Write(this.SqlText[i]);
                    while (ator.MoveNext())
                    {
                        if (ator.Current == null || ator.Current == DBNull.Value)
                            continue;

                        var newvalue = (ator.Current == null || ator.Current == DBNull.Value) ? DBNull.Value : ator.Current;
                        var newkey = string.Format("{0}x{1}z", para.Name, arrayLevel);

                        if (hadA)
                        {
                            format.Write(arrayLabel.Split);
                            format.Write(para.ActualPrefix);
                        }

                        format.Write(newkey);
                        arrayLevel++;

                        format.AddParameter(newkey, newvalue);
                        hadA = true;
                    }

                    i += para.OccupanLength;
                    if (i < this.SqlText.Length)
                        format.Write(this.SqlText[i]);
                }
            }
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

            var copy = this.Copy();
            var sb = new StringBuilder(this.SqlText);
            for (var i = 0; i < this.SqlText.Length; i++)
            {
                var para = this.MathPosition(copy, i);
                if (para == null)
                {
                    format.Write(this.SqlText[i]);
                    continue;
                }

                if (format.IfTextParameter(para))
                {
                    var item = convert.FirstOrDefault(o => o.Key.Equals(para.Name));
                    if (item == null)
                        throw new InvalidException("the sql tag {0} need the {1} parameters;", format.Id, para.Name);

                    var value = item.Value;
                    if (item.Value is IReferceNullableParameter)
                        value = ((IReferceNullableParameter)item.Value).Value;

                    if (value == null || value == DBNull.Value)
                    {
                        format.WriteOnTextMode("null");
                        i += para.OccupanLength + 1;
                    }
                    else
                    {
                        //format.WriteOnTextMode('\'');
                        format.WriteOnTextMode(value.ToString());
                        //format.WriteOnTextMode('\'');
                        i += para.OccupanLength + 1;
                        if (i < this.SqlText.Length)
                            format.WriteOnTextMode(this.SqlText[i]);
                    }
                }
                else
                {
                    format.Write(this.SqlText[i]);
                    var item = convert.FirstOrDefault(o => o.Key.Equals(para.Name));
                    if (item == null)
                        throw new InvalidException("the sql tag {0} need the {1} parameters;", format.Id, para.Name);

                    var value = item.Value;
                    if (item.Value is IReferceNullableParameter)
                        value = ((IReferceNullableParameter)item.Value).Value;

                    var newvalue = (value == null || value == DBNull.Value) ? DBNull.Value : value;
                    var newkey = string.Format("{0}x{1}z", para.Name, arrayLevel);
                    format.Write(newkey);
                    format.AddParameter(newkey, newvalue);
                    i += para.OccupanLength;
                    if (i < this.SqlText.Length)
                        format.Write(this.SqlText[i]);
                }
            }
        }
    }
}