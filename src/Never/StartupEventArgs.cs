using System;

namespace Never
{
    /// <summary>
    /// 启动事件
    /// </summary>
    public class StartupEventArgs : EventArgs
    {
        /// <summary>
        /// 资源
        /// </summary>
        public object Collector { get; }

        /// <summary>
        /// 启动信息
        /// </summary>
        public ApplicationStartup Startup { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="startup"></param>
        public StartupEventArgs(ApplicationStartup startup) : this(startup, null)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="collector"></param>
        public StartupEventArgs(ApplicationStartup startup, object collector)
        {
            this.Startup = startup;
            this.Collector = collector;
        }
    }
}