using Never.Exceptions;
using Never.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Labels
{
    /// <summary>
    /// 数组标签
    /// </summary>
    /// <seealso cref="Never.EasySql.Labels.BaseLabel" />
    public class ArrayLabel : BaseLabel
    {
        #region label   

        /// <summary>
        /// 验证参数
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        /// 成功验证参数后会拼接这个字符
        /// </summary>
        public string Then { get; set; }

        /// <summary>
        /// 数组分割符
        /// </summary>
        public string Split { get; set; }
        /// <summary>
        /// 数组开始字母
        /// </summary>
        public string Open { get; set; }
        /// <summary>
        /// 数组结束字母
        /// </summary>
        public string Close { get; set; }

        /// <summary>
        /// 文本标签
        /// </summary>
        public TextLabel Line { get; set; }

        /// <summary>
        /// 标签类型
        /// </summary>
        /// <returns></returns>
        public override LabelType GetLabelType() => LabelType.Array;

        #endregion label

        #region baseLabel

        /// <summary>
        /// 是否找到参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="convert"></param>
        /// <returns></returns>
        public override bool ContainParameter<T>(EasySqlParameter<T> parameter, IReadOnlyList<KeyValueTuple<string, object>> convert)
        {
            if (this.Parameter.IsNullOrEmpty())
                return parameter is ArraySqlParameter<T>;

            var item = convert.FirstOrDefault(o => o.Key.Equals(Parameter));
            if (item != null)
            {
                if (item.Value is INullableParameter)
                {
                    if (((INullableParameter)item.Value).HasValue)
                        return true;

                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 格式化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="parameter"></param>
        /// <param name="convert"></param>
        public override void Format<T>(SqlTagFormat format, EasySqlParameter<T> parameter, IReadOnlyList<KeyValueTuple<string, object>> convert)
        {
            if (this.Parameter.IsNullOrEmpty())
            {
                writeAllParameterIsArray();
                return;
            }
            else
            {
                writeTheParameterIsArray();
                return;
            }

            //全部参数都是数组，表明使用了array标签
            void writeAllParameterIsArray()
            {
                //直接是sql遍历参数
                if (parameter is ISqlParameterEnumerable)
                {
                    if (format.IfContainer)
                    {
                        if (format.IfThenCount > 0 && Then != null)
                            format.Write(Then);
                        else
                            format.IfThenCount++;
                    }
                    else
                    {
                        format.Write(Then);
                    }

                    IEnumerator ator = ((IEnumerable)parameter).GetEnumerator();
                    var arrayLevel = 0;
                    var appendValue = false;
                    while (ator.MoveNext())
                    {
                        if (appendValue)
                            format.Write(Split);

                        format.Write(Open);
                        var para = new KeyValueSqlParameter<T>((T)ator.Current);
                        this.Line.FormatArray(format, this, para, para.Convert(), arrayLevel++);
                        format.Write(Close);
                        appendValue = true;
                    }
                }

                //数组方式
                if (parameter.IsIEnumerable)
                {
                    if (format.IfContainer)
                    {
                        if (format.IfThenCount > 0 && Then != null)
                            format.Write(Then);
                        else
                            format.IfThenCount++;
                    }
                    else
                    {
                        format.Write(Then);
                    }

                    var ator = (((INullableParameter)parameter.Object).Value as System.Collections.IEnumerable).GetEnumerator();
                    var arrayLevel = 0;
                    var appendValue = false;
                    while (ator.MoveNext())
                    {
                        if (appendValue)
                            format.Write(Split);

                        format.Write(Open);
                        var para = new KeyValueSqlParameter<T>((T)ator.Current);
                        this.Line.FormatArray(format, this, para, para.Convert(), arrayLevel++);
                        format.Write(Close);
                        appendValue = true;
                    }
                }

                throw new InvalidException("the sql tag {0} need the array {1} parameter;", format.Id, Parameter);
            }

            //这个参数是一个数组
            void writeTheParameterIsArray()
            {
                if (parameter.Count <= 0)
                    return;

                var item = convert.FirstOrDefault(o => o.Key.Equals(Parameter));
                if (item == null)
                    return;

                var value = item.Value;
                if (value is INullableParameter)
                {
                    var nullValue = value as INullableParameter;
                    if (nullValue.HasValue == false)
                        return;

                    if (nullValue.Value is IEnumerable)
                    {
                        if (format.IfContainer)
                        {
                            if (format.IfThenCount > 0 && Then != null)
                                format.Write(Then);
                            else
                                format.IfThenCount++;
                        }
                        else
                        {
                            format.Write(Then);
                        }

                        format.Write(Open);
                        Line.FormatArray(format, this, parameter, item);
                        format.Write(Close);
                        return;
                    }

                    throw new InvalidException("the sql tag {0} need the array {1} parameter;", format.Id, Parameter);
                }

                if (value is IEnumerable)
                {
                    if (format.IfContainer)
                    {
                        if (format.IfThenCount > 0 && Then != null)
                            format.Write(Then);
                        else
                            format.IfThenCount++;
                    }
                    else
                    {
                        format.Write(Then);
                    }

                    format.Write(Open);
                    this.Line.FormatArray(format, this, parameter, item);
                    format.Write(Close);
                }

                throw new InvalidException("the sql tag {0} need the array {1} parameter;", format.Id, Parameter);
            }
        }

        #endregion baseLabel
    }
}