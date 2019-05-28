using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Never.IoC.Providers
{
    /// <summary>
    /// 应用域对象提供者
    /// </summary>
    public class AppDomainAssemblyProvider : Never.IoC.Providers.DefaultAssemblyProvider, Never.IoC.IAssemblyProvider
    {
        #region field
        /// <summary>
        /// 不加载到上下文的程序集
        /// </summary>
        private readonly Func<string, bool> notloadAssemblyName = null;
        #endregion

        #region ctor

        /// <summary>
        /// Initializes static members of the <see cref="AppDomainAssemblyProvider"/> class.
        /// </summary>
        static AppDomainAssemblyProvider()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainAssemblyProvider"/> class.
        /// </summary>
        public AppDomainAssemblyProvider() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainAssemblyProvider"/> class.
        /// <param name="notloadAssemblyName">不加载到上下文的程序集</param>
        /// </summary>
        public AppDomainAssemblyProvider(Func<string, bool> notloadAssemblyName)
        {
            this.notloadAssemblyName = notloadAssemblyName;
        }

        #endregion ctor

        #region field

        /// <summary>
        /// 是否已经加载了其他程序集
        /// </summary>
        private bool IsLoadOtherAssemblies = false;

        #endregion field

        #region IAssemblyProvider成员

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
            if (this.IsLoadOtherAssemblies)
                return base.GetAssemblies();

            var old = base.GetAssemblies();
            if (IsLoadOtherAssemblies)
                return base.GetAssemblies();

            var assemblies = LoadOtherAssemblies();
            if (assemblies == null || assemblies.Length == 0)
                return base.GetAssemblies();

            Assembly[] success = LoadAssemblyIntoAppDomain(assemblies);
            IsLoadOtherAssemblies = true;

            if (success == null)
                return base.GetAssemblies();

            var list = new List<Assembly>(success.Length + 100);
            list.AddRange(success);
            list.AddRange(base.GetAssemblies());

            return list.ToArray();
        }

        /// <summary>
        /// 加载程序集到当前运行域中
        /// </summary>
        /// <param name="assemblies">程序集.</param>
        /// <returns></returns>
        private Assembly[] LoadAssemblyIntoAppDomain(Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
                return new Assembly[] { };

            var result = new List<Assembly>(assemblies.Length);
            /*已加载的程序集*/
            var names = new List<Assembly>(100);
            names.AddRange(CurrentDomain.GetAssemblies());
            bool isNull = names == null || names.Count == 0;

            foreach (var assembly in assemblies)
            {
                try
                {
                    if (isNull || names.Find(o => o.FullName.Contains(assembly.FullName)) == null)
                        result.Add(CurrentDomain.Load(new AssemblyName(assembly.FullName)));
                }
                catch (BadImageFormatException ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }

            return result.ToArray();
        }

        #endregion IAssemblyProvider成员

        #region virtual

        /// <summary>
        /// 获取其他程序集
        /// </summary>
        /// <returns></returns>
        protected virtual Assembly[] LoadOtherAssemblies()
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
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        #endregion virtual

        #region utils

        /// <summary>
        /// 默认类型提供者
        /// </summary>
        public new static AppDomainAssemblyProvider Default { get; } = new AppDomainAssemblyProvider(null);

        #endregion default
    }
}