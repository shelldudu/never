using Never.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Never.IO
{
    /// <summary>
    /// 文件监控抽象类
    /// </summary>
    public class FileWatcher : FileSystemWatcher, IWorkService
    {
        #region field and prop and ctor

        /// <summary>
        /// 文件
        /// </summary>
        public readonly FileInfo File;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWatcher"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        public FileWatcher(FileInfo file) : base(System.IO.Path.GetDirectoryName(file.FullName), file.Name)
        {
            this.File = file;
        }

        #endregion field and prop and ctor

        #region IWorkService

        /// <summary>
        /// 开户启动任务
        /// </summary>
        public virtual void Startup()
        {
        }

        /// <summary>
        /// 关闭任务
        /// </summary>
        public virtual void Shutdown()
        {
        }

        #endregion IWorkService
    }
}