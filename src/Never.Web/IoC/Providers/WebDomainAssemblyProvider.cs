using Never.IoC.Providers;
using System;
using System.IO;
using System.Reflection;
using System.Web;

namespace Never.Web.IoC.Providers
{
    /// <summary>
    /// Web应用域对象提供者
    /// </summary>
    public class WebDomainAssemblyProvider : AppDomainAssemblyProvider, Never.IoC.IAssemblyProvider
    {
        #region filed

        /// <summary>
        /// 不加载到上下文的程序集
        /// </summary>
        private readonly Func<string, bool> notloadAssemblyName = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDomainAssemblyProvider"/> class.
        /// </summary>
        /// <param name="notloadAssemblyName">不加载到上下文的程序集</param>
        public WebDomainAssemblyProvider(Func<string, bool> notloadAssemblyName)
        {
            this.notloadAssemblyName = notloadAssemblyName;
        }

        #endregion

        #region AppDomainTypeFinder成员

        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <returns></returns>
        protected override AssemblyName GetAssemblyName(string file)
        {
            if (this.notloadAssemblyName != null)
            {
                if (this.notloadAssemblyName(file))
                    return base.GetAssemblyName(file);

                return null;
            }

            return base.GetAssemblyName(file);
        }

        /// <summary>
        /// 获取程序集
        /// </summary>
        /// <returns></returns>
        public override Assembly[] GetAssemblies()
        {
            return base.GetAssemblies();
        }

        /// <summary>
        /// 获取其他程序集
        /// </summary>
        /// <returns></returns>
        protected override Assembly[] LoadOtherAssemblies()
        {
            if (this.notloadAssemblyName != null)
                return GetAssemblies(new DirectoryInfo(this.CurrentBinPath()), SearchOption.AllDirectories, this);

            return GetAssemblies(new DirectoryInfo(this.CurrentBinPath()));
        }

        /// <summary>
        /// 返回Bin目录
        /// </summary>
        /// <returns></returns>
        private string CurrentBinPath()
        {
#if !NET461
            return AppDomain.CurrentDomain.BaseDirectory;
#else
            return System.Web.Hosting.HostingEnvironment.IsHosted ? HttpRuntime.BinDirectory : AppDomain.CurrentDomain.BaseDirectory;
#endif
        }
        #endregion AppDomainTypeFinder成员

        #region utils

        /// <summary>
        /// 默认类型提供者
        /// </summary>
        public new static WebDomainAssemblyProvider Default { get; } = new WebDomainAssemblyProvider(null);

        #endregion default
    }
}