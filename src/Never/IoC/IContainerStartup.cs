using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.IoC
{
    /// <summary>
    /// 容器初始化过程
    /// </summary>
    public interface IContainerStartup
    {
        /// <summary>
        /// 初始化
        /// </summary>
        event EventHandler<IContainerStartupEventArgs> OnIniting;

        /// <summary>
        /// 初始化
        /// </summary>
        void Init();

        /// <summary>
        /// 启动中
        /// </summary>
        event EventHandler<IContainerStartupEventArgs> OnStarting;

        /// <summary>
        /// 启动中
        /// </summary>
        void Startup();
    }
}