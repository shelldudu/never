using Never.Exceptions;
using Never.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Xml
{
    /// <summary>
    /// 数组标签
    /// </summary>
    /// <seealso cref="Never.EasySql.Xml.BaseLabel" />
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
            if (Parameter.IsNullOrEmpty())
                return true;

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
            if (Parameter.IsNullOrEmpty())
            {
                if (parameter.IsIEnumerable || parameter is ISqlParameterEnumerable)
                {
                    if (parameter.Object is INullableParameter)
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

                        var ator = (((IReferceNullableParameter)parameter.Object).Value as System.Collections.IEnumerable).GetEnumerator();
                        var i = 0;
                        var hadA = false;
                        while (ator.MoveNext())
                        {
                            if (hadA)
                                format.Write(Split);

                            format.Write(Open);
                            Build(ator.Current).Invoke(format, this, Line, ator.Current, i++);
                            format.Write(Close);
                            hadA = true;
                        }
                    }
                    else
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

                        IEnumerator ator = null;
                        if (parameter is ISqlParameterEnumerable)
                            ator = ((IEnumerable)parameter).GetEnumerator();
                        else
                            ator = (parameter.Object as System.Collections.IEnumerable).GetEnumerator();

                        var i = 0;
                        var hadA = false;
                        while (ator.MoveNext())
                        {
                            if (hadA)
                                format.Write(Split);

                            format.Write(Open);
                            Build(ator.Current).Invoke(format, this, Line, ator.Current, i++);
                            format.Write(Close);
                            hadA = true;
                        }
                    }
                }
                else
                {
                    throw new InvalidException("the sql tag {0} need the array {1} parameter;", format.Id, Parameter);
                }
            }

            if (parameter.Count <= 0)
                return;

            var item = convert.FirstOrDefault(o => o.Key.Equals(Parameter));
            if (item == null)
                return;

            var value = item.Value;
            if (item.Value is INullableParameter)
            {
                if (((INullableParameter)item.Value).HasValue)
                {
                    if (((IReferceNullableParameter)item.Value).Value is System.Collections.IEnumerable)
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
                        Line.FormatArray<T>(format, this, parameter, item);
                        format.Write(Close);
                    }
                    else
                    {
                        throw new InvalidException("the sql tag {0} need the array {1} parameter;", format.Id, Parameter);
                    }
                }
                else
                {
                    return;
                }
            }
            else if (value is System.Collections.IEnumerable)
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
                Line.FormatArray<T>(format, this, parameter, item);
                format.Write(Close);
            }
            else
            {
                throw new InvalidException("the sql tag {0} need the array {1} parameter;", format.Id, Parameter);
            }
        }

        #endregion baseLabel

        #region build

        private interface IBuild
        {
            Action<SqlTagFormat, ArrayLabel, TextLabel, object, int> Build();
        }

        private class ArrayLabelBuilder<T> : IBuild
        {
            static ArrayLabelBuilder()
            {
                Empty = new Action<SqlTagFormat, ArrayLabel, TextLabel, object, int>((x, y, z, o, s) => { });
            }

            public static Action<SqlTagFormat, ArrayLabel, TextLabel, object, int> Action { get; private set; }
            public static Action<SqlTagFormat, ArrayLabel, TextLabel, object, int> Empty { get; private set; }

            public Action<SqlTagFormat, ArrayLabel, TextLabel, object, int> Build()
            {
                if (Action != null)
                    return Action;

                var emit = EasyEmitBuilder<Action<SqlTagFormat, ArrayLabel, TextLabel, object, int>>.NewDynamicMethod();
                var method = typeof(ArrayLabel).GetMethod("LoadUsingDelegated").MakeGenericMethod(typeof(T));
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadArgument(2);
                emit.LoadArgument(3);
                emit.LoadArgument(4);
                emit.Call(method);
                emit.Return();
                Action = emit.CreateDelegate();
                return Action;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static Action<SqlTagFormat, ArrayLabel, TextLabel, object, int> Build(object parameter)
        {
            if (parameter.GetType() == typeof(object))
                return ArrayLabelBuilder<object>.Empty;

            var @class = typeof(ArrayLabelBuilder<>).MakeGenericType(parameter.GetType());
            return ((IBuild)Activator.CreateInstance(@class)).Build();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="arrayLabel"></param>
        /// <param name="label"></param>
        /// <param name="parameter"></param>
        /// <param name="arrayLevel"></param>
        public static void LoadUsingDelegated<T>(SqlTagFormat format, ArrayLabel arrayLabel, TextLabel label, object parameter, int arrayLevel)
        {
            var para = new KeyValueEasySqlParameter<T>((T)parameter);
            label.FormatArray(format, arrayLabel, para, para.Convert(), arrayLevel);
        }

        #endregion build
    }
}