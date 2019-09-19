using Never.Exceptions;
using Never.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// sqltag提供者
    /// </summary>
    /// <seealso cref="Never.EasySql.ISqlTagProvider" />
    public class SqlTagProvider : ISqlTagProvider
    {
        #region field

        /// <summary>
        /// 所有入tag
        /// </summary>
        private readonly ConcurrentDictionary<string, SqlTag> sortedSet = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        public SqlTagProvider()
        {
            this.sortedSet = new ConcurrentDictionary<string, SqlTag>(StringComparer.CurrentCulture);
        }

        #endregion ctor

        #region ISqlTagProvider

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, SqlTag>> GetAll()
        {
            return this.sortedSet;
        }

        /// <summary>
        /// 获取某一个
        /// </summary>
        /// <param name="sqlId"></param>
        /// <returns></returns>
        public SqlTag Get(string sqlId)
        {
            SqlTag sqlTag = null;
            TryGet(sqlId, out sqlTag);
            return sqlTag;
        }

        /// <summary>
        /// 获取某一个
        /// </summary>
        /// <param name="sqlId"></param>
        /// <param name="sqlTag"></param>
        /// <returns></returns>
        public bool TryGet(string sqlId, out SqlTag sqlTag)
        {
            if (!this.sortedSet.TryGetValue(sqlId, out sqlTag))
                return false;

            return true;
        }

        #endregion ISqlTagProvider

        #region load

        /// <summary>
        /// 加载所有的tag
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public SqlTagProvider Load(Stream stream, string filename = null)
        {
            var doc = new System.Xml.XmlDocument();
            doc.Load(stream);
            var @namespaces = doc["namespace"];
            if (@namespaces == null)
                return this;

            var idele = namespaces.Attributes.GetNamedItem("id");
            var indentedele = namespaces.Attributes.GetNamedItem("indented");
            var id = idele == null ? "" : idele.Value;
            var indented = indentedele == null ? true : indentedele.Value.AsBool();

            var next = @namespaces.FirstChild;
            while (next != null)
            {
                if (next.NodeType == System.Xml.XmlNodeType.Comment)
                {
                    next = next.NextSibling;
                    continue;
                }

                var name = next.Name;
                var nextIdele = next.Attributes.GetNamedItem("id");
                if (nextIdele == null)
                    throw new Never.Exceptions.KeyNotExistedException("can not find the id atrribute in this {0} file", filename);

                var nextId = nextIdele.Value;
                if (nextId.IsNullOrEmpty())
                    throw new Never.Exceptions.DataFormatException("can not find the id atrribute in this {0} file", filename);

                if (this.sortedSet.ContainsKey(nextId))
                    throw new DataFormatException("the {0} is duplicated", nextId);

                var sqlTag = new SqlTag();
                sqlTag.Id = nextId;
                sqlTag.NameSpace = id;
                sqlTag.IndentedOnNameSpace = indented;
                if (!LoadCommandName(next, sqlTag))
                {
                    next = next.NextSibling;
                    break;
                }
                sqlTag.Node = next;
                this.sortedSet[nextId] = sqlTag;
                next = next.NextSibling;
            }

            return this;
        }

        /// <summary>
        /// 读取命令名字
        /// </summary>
        /// <param name="node"></param>
        /// <param name="sqlTag"></param>
        /// <returns></returns>
        internal bool LoadCommandName(System.Xml.XmlNode node, SqlTag sqlTag)
        {
            switch (node.Name)
            {
                case "sql":
                    {
                        sqlTag.CommandType = node.Name;
                    }
                    break;

                case "select":
                    {
                        var indented = node.Attributes.GetNamedItem("indented");
                        sqlTag.IndentedOnSqlTag = indented == null ? true : indented.Value.AsBool();
                        sqlTag.CommandType = node.Name;
                    }
                    break;

                case "delete":
                    {
                        var indented = node.Attributes.GetNamedItem("indented");
                        sqlTag.IndentedOnSqlTag = indented == null ? true : indented.Value.AsBool();
                        sqlTag.CommandType = node.Name;
                    }
                    break;

                case "update":
                    {
                        var indented = node.Attributes.GetNamedItem("indented");
                        sqlTag.IndentedOnSqlTag = indented == null ? true : indented.Value.AsBool();
                        sqlTag.CommandType = node.Name;
                    }
                    break;

                case "insert":
                    {
                        var indented = node.Attributes.GetNamedItem("indented");
                        sqlTag.IndentedOnSqlTag = indented == null ? true : indented.Value.AsBool();
                        sqlTag.CommandType = node.Name;
                    }
                    break;

                case "procedure":
                    {
                        var indented = node.Attributes.GetNamedItem("indented");
                        sqlTag.IndentedOnSqlTag = indented == null ? true : indented.Value.AsBool();
                        sqlTag.CommandType = node.Name;
                    }
                    break;

                default:
                    {
                        return false;
                    }
            }

            return true;
        }

        /// <summary>
        /// 开始构建
        /// </summary>
        internal void Build(string stringPrefix)
        {
            var include = new List<SqlTag>();
            foreach (var i in this.sortedSet)
            {
                i.Value.Build(this, stringPrefix, include);
            }

            foreach (var i in include)
            {
                i.Node = null;
            }
        }

        /// <summary>
        /// 获取某一个
        /// </summary>
        /// <param name="sqlTag"></param>
        /// <returns></returns>
        internal void Add(SqlTag sqlTag)
        {
            SqlTag temp = null;
            if (this.TryGet(sqlTag.Id, out temp))
                return;

            this.sortedSet[sqlTag.Id] = sqlTag;
            return;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sqlTag"></param>
        internal void Remove(SqlTag sqlTag)
        {
            SqlTag id = null;
            this.sortedSet.TryRemove(sqlTag.Id, out id);
        }

        #endregion load
    }
}