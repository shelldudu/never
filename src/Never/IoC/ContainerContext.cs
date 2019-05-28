using System;
using System.Runtime.CompilerServices;

namespace Never.IoC
{
    /// <summary>
    /// IoC容器引擎环境
    /// </summary>
    public static class ContainerContext
    {
        #region prop

        /// <summary>
        /// 当前容器
        /// </summary>
        public static IContainer Current
        {
            get
            {
                return Singleton<IContainer>.Instance;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                Singleton<IContainer>.Instance = value;
            }
        }

        #endregion prop
    }
}