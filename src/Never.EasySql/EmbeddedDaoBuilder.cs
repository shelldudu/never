using Never.Exceptions;
using Never.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Never.EasySql
{
    /// <summary>
    /// 嵌入资源方式的Sql
    /// </summary>
    public abstract class EmbeddedDaoBuilder : BaseDaoBuilder
    {
        #region property

        /// <summary>
        /// 获取所有的SqlMap文件
        /// </summary>
        public abstract string[] EmbeddedSqlMaps { get; }

        #endregion property

        #region ctor

        /// <summary>
        ///
        /// </summary>
        protected EmbeddedDaoBuilder() : base()
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
        protected virtual string[] GetXmlContentFromAssembly(Assembly assembly, System.Func<System.Xml.XmlDocument, string, bool> checking)
        {
            var assemblyName = assembly.GetName();
            var files = assembly.GetManifestResourceNames();
            if (files == null || files.Length <= 0)
                return new string[0];

            var list = new List<string>(files.Length);
            var doc = new System.Xml.XmlDocument();
            foreach (var file in files)
            {
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
}