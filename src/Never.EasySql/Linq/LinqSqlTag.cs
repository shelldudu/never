using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// linq tag
    /// </summary>
    public sealed class LinqSqlTag : SqlTag
    {
        private readonly Dictionary<string, object> templateParameter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheId"></param>
        public LinqSqlTag(string cacheId)
        {
            this.Id = cacheId ?? NewId.GenerateGuid().ToString();
            this.Labels = new List<ILabel>();
        }

        private LinqSqlTag(Dictionary<string, object> templateParameter)
        {
            this.templateParameter = templateParameter;
        }

        /// <summary>
        /// 克隆新的规则
        /// </summary>
        /// <param name="templateParameter"></param>
        /// <returns></returns>
        public LinqSqlTag Clone(Dictionary<string, object> templateParameter)
        {
            if (templateParameter.Any())
            {
                return new LinqSqlTag(templateParameter)
                {
                    Id = this.Id,
                    Labels = this.Labels,
                    TextLength = this.TextLength,
                };
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override SqlTagFormat Format<T>(EasySqlParameter<T> parameter)
        {
            var format = new SqlTagFormat(this.TextLength, parameter.Count) { Id = this.Id };
            if (this.Labels.Any())
            {
                if (this.templateParameter != null && this.templateParameter.Any())
                {
                    foreach (var a in this.templateParameter)
                    {
                        parameter.ReplaceParameter(a.Key, a.Value);
                    }
                }

                var convert = parameter.Convert();
                foreach (var line in this.Labels)
                {
                    line.Format(format, parameter, convert);
                }
            }

            return format;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override SqlTagFormat FormatForText<T>(EasySqlParameter<T> parameter)
        {
            var format = new SqlTagFormat.TextSqlTagFormat(this.TextLength, parameter.Count) { Id = this.Id };
            if (this.Labels.Any())
            {
                if (this.templateParameter != null && this.templateParameter.Any())
                {
                    foreach (var a in this.templateParameter)
                    {
                        parameter.ReplaceParameter(a.Key, a.Value);
                    }
                }

                var convert = parameter.Convert();
                foreach (var line in this.Labels)
                {
                    line.Format(format, parameter, convert);
                }
            }

            return format;
        }
    }
}
