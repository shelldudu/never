using Never.Utils;
using System;
using System.Collections.Concurrent;
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
        /// 缓存起来的东西
        /// </summary>
        private static readonly ConcurrentDictionary<string, SqlTag> cached = new ConcurrentDictionary<string, SqlTag>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cacheId">缓存key</param>
        /// <param name="dao"></param>
        /// <returns></returns>
        public static SqlTag Build(string sql, string cacheId, IDao dao)
        {
            if (cacheId.IsNotNullOrWhiteSpace())
            {
                if (cached.TryGetValue(cacheId, out var tag))
                    return tag;

                tag = Build(sql, dao);
                cached.TryAdd(cacheId, tag);
                return tag;
            }

            return Build(sql, dao);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dao"></param>
        /// <returns></returns>
        static SqlTag Build(string sql, IDao dao)
        {
            var sqltag = new Xml.SqlTag()
            {
                CommandType = null,
                Id = NewId.GenerateString(NewId.StringLength.L24),
                IndentedOnNameSpace = false,
                IndentedOnSqlTag = false,
                NameSpace = null,
                Labels = new List<ILabel>(1),
            };

            var label = sqltag.ReadTextNode(sql, new Serialization.Json.ThunderWriter(sql.Length), new Serialization.Json.SequenceStringReader("1"), dao.SqlExecuter.GetParameterPrefix());
            ((List<ILabel>)sqltag.Labels).Add(new TextLabel(label));
            return sqltag;
        }
    }
}
