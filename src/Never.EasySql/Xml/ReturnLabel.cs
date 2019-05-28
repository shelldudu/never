using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Xml
{
    /// <summary>
    /// 返回标签
    /// </summary>
    /// <seealso cref="Never.EasySql.Xml.BaseLabel" />
    public class ReturnLabel : BaseLabel
    {
        /// <summary>
        /// 文本标签
        /// </summary>
        public TextLabel Line { get; set; }

        /// <summary>
        /// 标签类型
        /// </summary>
        /// <returns></returns>
        public override LabelType GetLabelType() => LabelType.Return;

        /// <summary>
        /// 返回类型
        /// </summary>
        public string Type { get; set; }

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
        public override void Format<T>(SqlTagFormat format, EasySqlParameter<T> parameter, IReadOnlyList<KeyValueTuple<string, object>> convert)
        {
            format.ReturnType = this.Type;
            this.Line.Format(format, parameter, convert);
        }
    }
}