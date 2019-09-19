using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Xml
{
    /// <summary>
    /// 标签类型
    /// </summary>
    public abstract class BaseLabel : ILabel
    {
        /// <summary>
        /// 标签Id
        /// </summary>
        public string TagId { get; set; }

        /// <summary>
        /// sql语句
        /// </summary>
        public string SqlText { get; set; }

        /// <summary>
        /// 标签类型
        /// </summary>
        public abstract LabelType GetLabelType();

        /// <summary>
        /// 格式化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="parameter"></param>
        /// <param name="convert"></param>
        public abstract void Format<T>(SqlTagFormat format, EasySqlParameter<T> parameter, IReadOnlyList<KeyValueTuple<string, object>> convert);

        /// <summary>
        /// 是否找到参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="convert"></param>
        /// <returns></returns>
        public abstract bool ContainParameter<T>(EasySqlParameter<T> parameter, IReadOnlyList<KeyValueTuple<string, object>> convert);

        /// <summary>
        /// 在参数里面查询第一个匹配开始位置（以前一个位置为终点，用于获取参数的前缀去确定是文本参数还是标准参数）的参数
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        protected virtual SqlTagParameterPosition MathPosition(List<SqlTagParameterPosition> parameters, int start)
        {
            if (parameters.Count == 0)
                return null;

            SqlTagParameterPosition found = null;
            for (var i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].PrefixStart == start)
                {
                    found = parameters[i];
                    parameters.Remove(found);
                    return found;
                }
            }

            return null;
        }
    }
}