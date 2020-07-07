using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Text
{
    /// <summary>
    /// text label
    /// </summary>
    public sealed class TextLabel : Labels.TextLabel, ILabel
    {
        private readonly Labels.TextLabel lable = null;
        private List<SqlTagParameterPosition> parameterPositions = null;
        private readonly int parameterPositionCount = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lable"></param>
        public TextLabel(Labels.TextLabel lable)
        {
            this.lable = lable;
            this.SqlText = this.lable.SqlText;
            this.parameterPositions = new List<SqlTagParameterPosition>(lable.ParameterPositions);
            this.parameterPositionCount = this.parameterPositions.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="parameter"></param>
        /// <param name="convert"></param>
        public override void Format<T>(SqlTagFormat format, EasySqlParameter<T> parameter, IReadOnlyList<KeyValueTuple<string, object>> convert)
        {
            if (this.SqlText.IsNullOrEmpty())
            {
                return;
            }

            if (this.parameterPositions == null || this.parameterPositionCount == 0)
            {
                format.Write(this.SqlText);
                return;
            }

            int start = 0;
            foreach (var para in this.parameterPositions)
            {
                var firstConvert = convert.FirstOrDefault(o => o.Key.IsEquals(para.Name));
                if (firstConvert == null)
                {
                    throw new Exception(string.Format("当前在sql语句中参数为{0}的值在所提供的参数列表中找不到", para.Name));
                }

                var value = firstConvert.Value;
                if (value is INullableParameter && ((INullableParameter)value).HasValue)
                {
                    value = ((INullableParameter)firstConvert.Value).Value;
                }

                format.WriteOnTextMode(this.SqlText, start, para.PrefixStartIndex - start);
                //非数组
                if (value is System.Collections.IEnumerable == false || value is string)
                {
                    writeObject(value, para);
                }
                else
                {
                    writeArray(value, para);
                }

                start = para.ParameterStopIndex + 1;
            }

            if (this.SqlText.Length >= start)
            {
                format.WriteOnTextMode(this.SqlText, start, this.SqlText.Length - start);
                return;
            }

            throw new Exception(string.Format("{0}字符串遍历出错", ""));

            void writeArray(object value, SqlTagParameterPosition para)
            {
                var item = value as System.Collections.IEnumerable;
                var ator = item.GetEnumerator();
                var appendValue = false;
                var arrayLevel = 0;
                if (format.IfTextParameter(para))
                {
                    while (ator.MoveNext())
                    {
                        if (ator.Current == null || ator.Current == DBNull.Value)
                        {
                            continue;
                        }

                        if (appendValue)
                        {
                            format.WriteOnTextMode(",");
                        }

                        format.WriteOnTextMode(ator.Current.ToString());
                        appendValue = true;
                    }
                }
                else
                {
                    while (ator.MoveNext())
                    {
                        if (ator.Current == null || ator.Current == DBNull.Value)
                        {
                            continue;
                        }

                        var newvalue = (ator.Current == null || ator.Current == DBNull.Value) ? DBNull.Value : ator.Current;
                        var newkey = string.Format("{0}x{1}z", para.Name, arrayLevel);

                        if (appendValue)
                        {
                            format.Write(",");
                        }

                        format.Write(para.ActualPrefix);
                        format.Write(newkey);
                        arrayLevel++;

                        format.AddParameter(newkey, newvalue);
                        appendValue = true;
                    }
                }
            }
            void writeObject(object value, SqlTagParameterPosition para)
            {
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
            }
        }
    }
}
