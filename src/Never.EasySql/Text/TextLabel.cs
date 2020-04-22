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
            if (this.lable.SqlText.IsNullOrEmpty())
            {
                return;
            }

            if (this.parameterPositions == null || this.parameterPositionCount == 0)
            {
                format.Write(this.lable.SqlText);
                return;
            }

            for (var i = 0; i < this.lable.SqlText.Length; i++)
            {
                var para = this.MathPosition(this.parameterPositions, i);
                if (para == null)
                {
                    format.Write(this.lable.SqlText[i]);
                    continue;
                }

                var firstConvert = convert.FirstOrDefault(o => o.Key.IsEquals(para.Name));
                if (firstConvert == null)
                {
                    throw new Exception(string.Format("当前在sql语句中参数为{0}的值在所提供的参数列表中找不到", para.Name));
                }

                var value = firstConvert.Value;
                if (value is INullableParameter)
                {
                    value = ((IReferceNullableParameter)firstConvert.Value).Value;
                }

                var isArray = value is System.Collections.IEnumerable;
                if (!isArray || value is string)
                {
                    if (format.IfTextParameter(para))
                    {
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
                        format.WriteOnTextMode(this.lable.SqlText[i]);
                    }
                    else
                    {
                        var item = convert.FirstOrDefault(o => o.Key.Equals(para.Name));
                        if (item == null)
                        {
                            throw new Exception(string.Format("当前在sql语句中参数为{0}的值在所提供的参数列表中找不到", para.Name));
                        }

                        format.Write(this.lable.SqlText, para.PrefixStartIndex, para.OccupanLength + 1);
                        format.AddParameter(item);
                        i += para.OccupanLength + 1;
                        format.Write(this.lable.SqlText[i]);
                    }
                }
                else
                {
                    var item = value as System.Collections.IEnumerable;
                    var ator = item.GetEnumerator();
                    var hadA = false;
                    var arrayLevel = 0;
                    if (format.IfTextParameter(para))
                    {
                        while (ator.MoveNext())
                        {
                            if (ator.Current == null || ator.Current == DBNull.Value)
                            {
                                continue;
                            }

                            if (hadA)
                            {
                                format.WriteOnTextMode(",");
                            }

                            //format.WriteOnTextMode('\'');
                            format.WriteOnTextMode(ator.Current.ToString());
                            //format.WriteOnTextMode('\'');
                            hadA = true;
                        }

                        i += para.OccupanLength + 1;
                        format.WriteOnTextMode(this.lable.SqlText[i]);
                    }
                    else
                    {
                        format.Write(this.lable.SqlText[i]);
                        while (ator.MoveNext())
                        {
                            if (ator.Current == null || ator.Current == DBNull.Value)
                            {
                                continue;
                            }

                            var newvalue = (ator.Current == null || ator.Current == DBNull.Value) ? DBNull.Value : ator.Current;
                            var newkey = string.Format("{0}x{1}z", para.Name, arrayLevel);

                            if (hadA)
                            {
                                format.Write(",");
                                format.Write(para.ActualPrefix);
                            }

                            format.Write(newkey);
                            arrayLevel++;

                            format.AddParameter(newkey, newvalue);
                            hadA = true;
                        }

                        i += para.OccupanLength + 1;
                        format.Write(this.lable.SqlText[i]);
                    }
                }
            }
        }
    }
}
