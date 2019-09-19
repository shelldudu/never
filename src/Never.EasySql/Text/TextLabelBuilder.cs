using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Text
{
    /// <summary>
    /// text builder
    /// </summary>
    public sealed class TextLabelBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dao"></param>
        /// <returns></returns>
        public static SqlTag Build(string sql, IDao dao)
        {
            var sqltag = new SqlTag()
            {
                CommandType = null,
                Id = NewId.GenerateString(NewId.StringLength.L24),
                IndentedOnNameSpace = false,
                IndentedOnSqlTag = false,
                NameSpace = null,
                Labels = new List<ILabel>(1),
            };

            var label = sqltag.ReadTextNode(sql, new Serialization.Json.ThunderWriter(sql.Length), new Serialization.Json.SequenceStringReader("1"), dao.SqlExecuter.GetParameterPrefix());
            sqltag.Labels.Add(new TextLabel(label));
            return sqltag;
        }
    }
}
