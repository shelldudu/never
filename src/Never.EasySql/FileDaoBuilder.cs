using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    ///
    /// </summary>
    public abstract class FileDaoBuilder : BaseDaoBuilder
    {
        #region property

        /// <summary>
        /// 获取所有的SqlMap文件
        /// </summary>
        public abstract FileInfo[] FileSqlMaps { get; }

        #endregion property

        #region ctor

        /// <summary>
        ///
        /// </summary>
        protected FileDaoBuilder() : base()
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
        protected virtual FileInfo[] GetXmlContentFromPath(string path, System.Func<System.Xml.XmlDocument, string, bool> checking)
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