using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Never.IoC.Providers
{
    /// <summary>
    /// 默认类型提供者
    /// </summary>
    public class DefaultAssemblyProvider : IAssemblyProvider
    {
        #region AppDomain

        /// <summary>
        /// 获取当前 System.Threading.Thread 的当前应用程序域。
        /// </summary>
        public virtual AppDomain CurrentDomain
        {
            get
            {
                return AppDomain.CurrentDomain;
            }
        }

        /// <summary>
        /// 默认类型提供者
        /// </summary>
        public static DefaultAssemblyProvider Default { get; } = new DefaultAssemblyProvider();

        #endregion AppDomain

        #region staic

        /// <summary>
        /// 获取所有程序集对象
        /// </summary>
        /// <param name="assemblyPath">文件路径</param>
        /// <param name="option">搜索条件</param>
        /// <param name="provider">类型提供者</param>
        /// <returns></returns>
        public static Assembly[] GetAssemblies(DirectoryInfo assemblyPath, SearchOption option = SearchOption.AllDirectories, DefaultAssemblyProvider provider = null)
        {
            var files = Directory.GetFiles(assemblyPath.FullName, "*.dll", option);
            if (files == null || files.Length <= 0)
                return null;

            var list = new List<Assembly>(files.Length);
            foreach (var file in files)
            {
                if (string.IsNullOrEmpty(file))
                    continue;
                try
                {
                    if (provider != null)
                    {
                        var assemblyName = provider.GetAssemblyName(file);
                        if (assemblyName != null)
                        {
                            var assembly = Assembly.Load(assemblyName);
                            if (assembly != null)
                                list.Add(assembly);
                        }
                    }
                    else
                    {
                        var assembly = Assembly.Load(AssemblyName.GetAssemblyName(file));
                        if (assembly != null)
                            list.Add(assembly);
                    }
                }
                catch (BadImageFormatException ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 获取所有描述程序集
        /// </summary>
        /// <returns></returns>
        /// <param name="assemblyPath">文件路径</param>
        /// <param name="option">搜索条件</param>
        /// <param name="provider">类型提供者</param>
        public static AssemblyName[] GetAssemblyNames(DirectoryInfo assemblyPath, SearchOption option = SearchOption.AllDirectories, DefaultAssemblyProvider provider = null)
        {
            var files = Directory.GetFiles(assemblyPath.FullName, "*.dll", option);
            if (files == null || files.Length <= 0)
                return null;

            var list = new List<AssemblyName>(files.Length);
            foreach (var file in files)
            {
                if (string.IsNullOrEmpty(file))
                    continue;

                try
                {
                    if (provider != null)
                    {
                        var assembly = provider.GetAssemblyName(file);
                        if (assembly != null)
                            list.Add(assembly);
                    }
                    else
                    {
                        var assembly = AssemblyName.GetAssemblyName(file);
                        if (assembly != null)
                            list.Add(assembly);
                    }
                }
                catch (BadImageFormatException ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }

            return list.ToArray();
        }

        #endregion staic

        #region IAssemblyProvider 成员

        /// <summary>
        /// 获取所有程序集对象
        /// </summary>
        /// <returns></returns>
        public virtual Assembly[] GetAssemblies()
        {
            return this.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <returns></returns>
        protected virtual AssemblyName GetAssemblyName(string file)
        {
            return AssemblyName.GetAssemblyName(file);
        }

        #endregion IAssemblyProvider 成员
    }
}