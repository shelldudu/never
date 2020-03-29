using Never.EasySql.Xml;
using Never.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// xml内容分析后构造dao
    /// </summary>
    public abstract class XmlContentDaoBuilder : BaseDaoBuilder
    {
        #region init

        /// <summary>
        /// 初始化sqlTag提供者
        /// </summary>
        /// <returns></returns>
        public override ISqlTagProvider InitSqlTagProviderOnStart()
        {
            var provider = new SqlTagProvider();
            var streams = this.GetAllStreams();
            if (streams != null)
            {
                streams = streams.Where(o => o != null).ToArray();
                foreach (var i in streams)
                {
                    this.Load(provider, i, null);
                }
            }

            var include = new List<Never.EasySql.Xml.XmlSqlTag>();
            foreach (var i in provider.GetAll())
            {
                ((Never.EasySql.Xml.XmlSqlTag)i.Value).Build(provider, this.ParameterPrefix, include);
            }

            foreach (var i in include)
            {
                i.Node = null;
            }

            this.OnStarted(streams);
            return provider;
        }

        /// <summary>
        /// 加载所有的tag
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="stream"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        protected void Load(SqlTagProvider provider, Stream stream, string filename = null)
        {
            var doc = new System.Xml.XmlDocument();
            doc.Load(stream);
            var @namespaces = doc["namespace"];
            if (@namespaces == null)
                return;

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

                if (provider.TryGet(nextId, out var tag))
                    throw new DataFormatException("the {0} is duplicated", nextId);

                var sqlTag = new Never.EasySql.Xml.XmlSqlTag();
                sqlTag.Id = nextId;
                sqlTag.NameSpace = id;
                sqlTag.IndentedOnNameSpace = indented;
                if (!LoadCommandName(next, sqlTag))
                {
                    next = next.NextSibling;
                    break;
                }
                sqlTag.Node = next;
                provider.Add(sqlTag);
                next = next.NextSibling;
            }

            //加载命令信息
            bool LoadCommandName(System.Xml.XmlNode node, Never.EasySql.Xml.XmlSqlTag sqlTag)
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
                            var indentedTemp = node.Attributes.GetNamedItem("indented");
                            sqlTag.IndentedOnSqlTag = indentedTemp == null ? true : indentedTemp.Value.AsBool();
                            sqlTag.CommandType = node.Name;
                        }
                        break;

                    case "delete":
                        {
                            var indentedTemp = node.Attributes.GetNamedItem("indented");
                            sqlTag.IndentedOnSqlTag = indentedTemp == null ? true : indentedTemp.Value.AsBool();
                            sqlTag.CommandType = node.Name;
                        }
                        break;

                    case "update":
                        {
                            var indentedTemp = node.Attributes.GetNamedItem("indented");
                            sqlTag.IndentedOnSqlTag = indentedTemp == null ? true : indentedTemp.Value.AsBool();
                            sqlTag.CommandType = node.Name;
                        }
                        break;

                    case "insert":
                        {
                            var indentedTemp = node.Attributes.GetNamedItem("indented");
                            sqlTag.IndentedOnSqlTag = indentedTemp == null ? true : indentedTemp.Value.AsBool();
                            sqlTag.CommandType = node.Name;
                        }
                        break;

                    case "procedure":
                        {
                            var indentedTemp = node.Attributes.GetNamedItem("indented");
                            sqlTag.IndentedOnSqlTag = indentedTemp == null ? true : indentedTemp.Value.AsBool();
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
        }

        /// <summary>
        /// 在启动后
        /// </summary>
        /// <param name="streams"></param>
        protected virtual void OnStarted(IEnumerable<Stream> streams)
        {
            if (streams == null)
            {
                return;
            }

            foreach (var i in streams)
            {
                i.Dispose();
            }
        }

        /// <summary>
        /// 所有的流内容
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<Stream> GetAllStreams()
        {
            return null;
        }

        #endregion

        /// <summary>
        /// 嵌入资源方式的Sql
        /// </summary>
        public abstract class XmlEmbeddedDaoBuilder : XmlContentDaoBuilder
        {
            #region property

            /// <summary>
            /// 获取所有的SqlMap文件
            /// </summary>
            public virtual string[] EmbeddedSqlMaps { get; }

            #endregion property

            #region ctor

            /// <summary>
            ///
            /// </summary>
            protected XmlEmbeddedDaoBuilder() : base()
            {
            }

            #endregion ctor

            #region base

            /// <summary>
            /// 开始工作
            /// </summary>
            public override void Startup()
            {
                base.Startup();
            }

            /// <summary>
            ///
            /// </summary>
            /// <returns></returns>
            protected override IEnumerable<Stream> GetAllStreams()
            {
                var xml = new List<string>(60);
                foreach (var embedded in this.EmbeddedSqlMaps)
                {
                    if (xml.Contains(embedded))
                        continue;

                    xml.Add(embedded);
                }

                var assembly = new List<string>(xml.Count);
                var fileName = new List<string>(xml.Count);
                foreach (var x in xml)
                {
                    var split = x.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (split == null || split.Length < 2)
                        throw new KeyNotExistedException("the embedded file format must demo.a.xml,demo,and this file {0} is wrong!", x);

                    if (fileName.Any(o => o.Equals(split[0], StringComparison.CurrentCultureIgnoreCase)))
                        continue;

                    assembly.Add(split[1].Trim());
                    fileName.Add(split[0].Trim());
                }

                foreach (var ass in this.GetAssemblies())
                {
                    var name = ass.GetName().Name;
                    for (var i = 0; i < fileName.Count; i++)
                    {
                        if (assembly[i].Equals(name, StringComparison.CurrentCulture))
                        {
                            var stream = ass.GetManifestResourceStream(fileName[i]);
                            if (stream == null)
                                throw new KeyNotExistedException("the embedded file {0},{1} not found", fileName[i], assembly[i]);

                            yield return stream;
                        }
                    }
                }
            }

            /// <summary>
            /// 获取程序集
            /// </summary>
            /// <returns></returns>
            protected virtual Assembly[] GetAssemblies()
            {
                return AppDomain.CurrentDomain.GetAssemblies();
            }

            /// <summary>
            /// 获取当前执行的程序集
            /// </summary>
            /// <param name="assembly">程序集</param>
            /// <param name="checking"></param>
            /// <returns></returns>
            protected string[] GetXmlContentFromAssembly(Assembly assembly, System.Func<System.Xml.XmlDocument, string, bool> checking)
            {
                var assemblyName = assembly.GetName();
                var files = assembly.GetManifestResourceNames();
                if (files == null || files.Length <= 0)
                    return new string[0];

                var list = new List<string>(files.Length);
                var doc = new System.Xml.XmlDocument();
                foreach (var file in files)
                {
                    if (doc == null)
                        doc = new System.Xml.XmlDocument();

                    using (var stream = assembly.GetManifestResourceStream(file))
                    {
                        try
                        {
                            /*不是xml文件*/
                            doc.Load(stream);
                        }
                        catch
                        {
                        }

                        if (doc == null && checking != null)
                        {
                            checking(doc, file);
                            continue;
                        }

                        var @namespaces = doc["namespace"];
                        if (@namespaces == null)
                            continue;

                        if (checking != null)
                        {
                            if (checking(doc, file))
                                list.Add(string.Concat(file, ", ", assemblyName.Name));
                        }
                        else
                        {
                            list.Add(string.Concat(file, ", ", assemblyName.Name));
                        }
                    }
                }

                return list.ToArray();
            }

            #endregion base
        }

        /// <summary>
        /// file资源方式的Sql
        /// </summary>
        public abstract class XmlFileDaoBuilder : XmlContentDaoBuilder
        {
            #region property

            /// <summary>
            /// 获取所有的SqlMap文件
            /// </summary>
            public virtual FileInfo[] FileSqlMaps { get; }

            #endregion property

            #region ctor

            /// <summary>
            ///
            /// </summary>
            protected XmlFileDaoBuilder() : base()
            {
            }

            #endregion ctor

            #region base

            /// <summary>
            ///
            /// </summary>
            /// <returns></returns>
            protected override IEnumerable<Stream> GetAllStreams()
            {
                var exists = new List<string>(60);
                foreach (var file in this.FileSqlMaps)
                {
                    if (exists.Contains(file.FullName))
                        continue;

                    if (file.Exists == false)
                        throw new FileNotFoundException(string.Format("{0} not exists", file.FullName));

                    yield return file.OpenRead();
                }
            }

            /// <summary>
            /// 开始工作
            /// </summary>
            public override void Startup()
            {
                base.Startup();
            }

            /// <summary>
            /// 获取当前执行的程序集
            /// </summary>
            /// <param name="path">路径</param>
            /// <param name="checking"></param>
            /// <returns></returns>
            protected FileInfo[] GetXmlContentFromPath(string path, System.Func<System.Xml.XmlDocument, string, bool> checking)
            {
                var files = System.IO.Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
                if (files == null || files.Length <= 0)
                    return new FileInfo[0];

                var list = new List<FileInfo>(files.Length);
                var doc = new System.Xml.XmlDocument();
                foreach (var file in files)
                {
                    if (doc == null)
                        doc = new System.Xml.XmlDocument();

                    if (file.LastIndexOf(".xml", System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        using (var stream = File.Open(file, FileMode.Open))
                        {
                            try
                            {
                                /*不是xml文件*/
                                doc.Load(stream);
                            }
                            catch
                            {
                            }

                            if (doc == null && checking != null)
                            {
                                checking(doc, file);
                                continue;
                            }

                            var @namespaces = doc["namespace"];
                            if (@namespaces == null)
                                continue;

                            if (checking != null)
                            {
                                if (checking(doc, file))
                                    list.Add(new FileInfo(file));
                            }
                            else
                            {
                                list.Add(new FileInfo(file));
                            }

                        }
                    }
                }

                return list.ToArray();
            }

            #endregion base
        }
    }
}
