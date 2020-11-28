using Never.Exceptions;
using Never.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        #region static
        private static readonly Hashtable arrayFormat = null;
        private static readonly MethodInfo arrayMethod = null;
        static ArrayLabel()
        {
            arrayFormat = new Hashtable(50);
            foreach (var method in typeof(ArrayLabel).GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (method.IsGenericMethod && method.Name.IsEquals("FormatArray"))
                {
                    arrayMethod = method;
                    break;
                }
            }
        }
        #endregion

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
                    return;
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

                    if (parameter is INullableParameter)
                    {
                        var ator = (((INullableParameter)parameter).Value as System.Collections.IEnumerable).GetEnumerator();
                        var arrayLevel = 0;
                        var appendValue = false;
                        while (ator.MoveNext())
                        {
                            if (appendValue)
                                format.Write(Split);

                            format.Write(Open);
                            this.FormatArray(this.Line, format, ator.Current, arrayLevel++);
                            format.Write(Close);
                            appendValue = true;
                        }
                        return;
                    }
                    else if (parameter is KeyValueSqlParameter<T>)
                    {
                        var ator = (((KeyValueSqlParameter<T>)parameter).Object as System.Collections.IEnumerable).GetEnumerator();
                        var arrayLevel = 0;
                        var appendValue = false;
                        while (ator.MoveNext())
                        {
                            if (appendValue)
                                format.Write(Split);

                            format.Write(Open);
                            this.FormatArray(this.Line, format, ator.Current, arrayLevel++);
                            format.Write(Close);
                            appendValue = true;
                        }
                        return;
                    }
                    else
                    {
                        var ator = (parameter as System.Collections.IEnumerable).GetEnumerator();
                        var arrayLevel = 0;
                        var appendValue = false;
                        while (ator.MoveNext())
                        {
                            if (appendValue)
                                format.Write(Split);

                            format.Write(Open);
                            this.FormatArray(this.Line, format, ator.Current, arrayLevel++);
                            format.Write(Close);
                            appendValue = true;
                        }
                        return;
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
                    return;
                }

                throw new InvalidException("the sql tag {0} need the array {1} parameter;", format.Id, Parameter);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textLabel"></param>
        /// <param name="format"></param>
        /// <param name="value"></param>
        /// <param name="arrayLevel"></param>
        public void FormatArray(TextLabel textLabel, SqlTagFormat format, object value, int arrayLevel)
        {
            if (value == null)
                return;

            var type = value.GetType();
            if (arrayFormat[type] == null)
            {
                var emitBuilder = EasyEmitBuilder<Action<TextLabel, SqlTagFormat, ArrayLabel, object, int>>.NewDynamicMethod();
                emitBuilder.LoadArgument(0);
                emitBuilder.LoadArgument(1);
                emitBuilder.LoadArgument(2);
                emitBuilder.LoadArgument(3);
                emitBuilder.LoadArgument(4);
                emitBuilder.Call(arrayMethod.MakeGenericMethod(type));
                emitBuilder.Return();
                arrayFormat[type] = emitBuilder.CreateDelegate();
            }

            Action<TextLabel, SqlTagFormat, ArrayLabel, object, int> action = arrayFormat[type] as Action<TextLabel, SqlTagFormat, ArrayLabel, object, int>;
            action.Invoke(textLabel, format, this, value, arrayLevel);
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="textLabel"></param>
        /// <param name="format"></param>
        /// <param name="arrayLabel"></param>
        /// <param name="value"></param>
        /// <param name="arrayLevel"></param>
        public static void FormatArray<T>(TextLabel textLabel, SqlTagFormat format, ArrayLabel arrayLabel, object value, int arrayLevel)
        {
            var para = new KeyValueSqlParameter<T>((T)value);
            textLabel.FormatArray(format, arrayLabel, para, para.Convert(), arrayLevel);
        }
        #endregion baseLabel
    }
}