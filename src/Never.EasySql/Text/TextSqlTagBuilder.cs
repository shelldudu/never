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
    sealed class TextSqlTagBuilder
    {
        /// <summary>
        /// 缓存起来的东西
        /// </summary>
        private static readonly SqlTagProvider provider = new SqlTagProvider();

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
                if (provider.TryGet(cacheId, out var tag))
                    return tag;

                tag = Build(sql, dao);
                tag.Id = cacheId;
                provider.Add(tag);
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
        static TextSqlTag Build(string sql, IDao dao)
        {
            var sqltag = new TextSqlTag()
            {
                Id = NewId.GenerateString(NewId.StringLength.L24),
                Labels = new List<ILabel>(1),
                TextLength = sql.Length,
            };

            var label = sqltag.ReadTextNode(sql, new Serialization.Json.ThunderWriter(sql.Length), new Serialization.Json.SequenceStringReader("1"), dao.SqlExecuter.GetParameterPrefix());
            ((List<ILabel>)sqltag.Labels).Add(new TextLabel(label));
            return sqltag;
        }
    }
}
