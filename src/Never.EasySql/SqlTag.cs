using Never.EasySql.Xml;
using Never.Exceptions;
using Never.Serialization.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Never.EasySql
{
    /// <summary>
    /// sqltag
    /// </summary>
    public class SqlTag
    {
        #region prop

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// 所有节点
        /// </summary>
        public IEnumerable<ILabel> Labels { get; internal set; }

        /// <summary>
        /// 所有节点总长度
        /// </summary>
        public int TextLength { get; internal set; }

        #endregion

        #region format

        /// <summary>
        /// 格式化内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public SqlTagFormat Format<T>(EasySqlParameter<T> parameter)
        {
            var format = new SqlTagFormat(this.TextLength, parameter.Count) { Id = this.Id };
            if (this.Labels.Any())
            {
                var convert = parameter.Convert();
                foreach (var line in this.Labels)
                {
                    line.Format(format, parameter, convert);
                }
            }

            return format;
        }

        /// <summary>
        /// 格式化内容，返回文本内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public SqlTagFormat FormatForText<T>(EasySqlParameter<T> parameter)
        {
            var format = new SqlTagFormat.TextSqlTagFormat(this.TextLength, parameter.Count) { Id = this.Id };
            if (this.Labels.Any())
            {
                var convert = parameter.Convert();
                foreach (var line in this.Labels)
                {
                    line.Format(format, parameter, convert);
                }
            }

            return format;
        }

        #endregion format
    }
}