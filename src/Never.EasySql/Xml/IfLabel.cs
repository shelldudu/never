using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Xml
{
    /// <summary>
    /// if标签
    /// </summary>
    /// <seealso cref="Never.EasySql.Xml.BaseLabel" />
    public class IfLabel : BaseLabel
    {
        /// <summary>
        /// 成功验证参数后会拼接这个字符
        /// </summary>
        public string Then { get; set; }

        /// <summary>
        /// 结束时候拼接的字符
        /// </summary>
        public string End { get; set; }

        /// <summary>
        /// 分割，里面子标签每成功一次都是加这个字符
        /// </summary>
        public string Split { get; set; }

        /// <summary>
        /// 标签类型
        /// </summary>
        /// <returns></returns>
        public override LabelType GetLabelType() => LabelType.If;

        /// <summary>
        /// 子标签
        /// </summary>
        public List<ILabel> Labels { get; set; }

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
        /// 格式化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="parameter"></param>
        /// <param name="convert"></param>
        /// <exception cref="Never.Exceptions.InvalidException">if tag contain other {0} tag</exception>
        public override void Format<T>(SqlTagFormat format, EasySqlParameter<T> parameter, IReadOnlyList<KeyValueTuple<string, object>> convert)
        {
            var writeThen = false;
            format.IfContainer = true;
            format.IfThenCount = 0;
            var splitCount = 0;
            foreach (BaseLabel lable in this.Labels)
            {
                switch (lable.GetLabelType())
                {
                    case LabelType.Empty:
                    case LabelType.NotEmpty:
                    case LabelType.Null:
                    case LabelType.NotNull:
                    case LabelType.Contain:
                    case LabelType.NotExists:
                    case LabelType.Array:
                        {
                            if (lable.ContainParameter(parameter, convert))
                            {
                                if (!writeThen)
                                {
                                    writeThen = true;
                                    format.Write(this.Then);
                                }

                                if (this.Split.IsNotNullOrEmpty())
                                {
                                    if (splitCount > 0)
                                    {
                                        format.Write(this.Split);
                                    }
                                    else
                                    {
                                        splitCount++;
                                    }
                                }

                                lable.Format(format, parameter, convert);
                            }
                        }
                        break;

                    case LabelType.Text:
                        {
                            if (this.Split.IsNotNullOrEmpty())
                            {
                                if (splitCount > 0)
                                {
                                    format.Write(this.Split);
                                }
                                else
                                {
                                    splitCount++;
                                }
                            }
                            lable.Format(format, parameter, convert);
                        }
                        break;

                    default:
                        {
                            throw new Never.Exceptions.InvalidException("if tag contain other {0} tag", lable.GetLabelType().ToString());
                        }
                }
            }

            if (writeThen && this.End.IsNotNullOrEmpty())
            {
                format.Write(this.End);
            }

            format.IfContainer = false;
            format.IfThenCount = 0;
        }
    }
}