using System;

namespace Never.Logging
{
    /// <summary>
    /// 日志输出管理器
    /// </summary>
    public class LoggerBuilder : ILoggerBuilder
    {
        #region field

        /// <summary>
        /// 日志输出仓库
        /// </summary>
        private ILogger logger = null;

        #endregion field

        #region

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerBuilder"/> class.
        /// </summary>
        public LoggerBuilder()
            : this(AllotLogger.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerBuilder"/> class.
        /// </summary>
        /// <param name="logger">日志输出对象.</param>
        protected LoggerBuilder(ILogger logger)
        {
            this.logger = logger;
        }

        #endregion

        #region ILogRepository成员

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="loggerName">日志名字</param>
        /// <returns></returns>
        public virtual ILogger Build(string loggerName)
        {
            return this.logger;
        }

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="loggerType">日志类型</param>
        /// <returns></returns>
        public virtual ILogger Build(Type loggerType)
        {
            return this.logger;
        }

        #endregion

        #region empty

        /// <summary>
        /// 单例
        /// </summary>
        private static LoggerBuilder empty = new LoggerBuilder();

        /// <summary>
        /// 单例
        /// </summary>
        public static LoggerBuilder Empty
        {
            get
            {
                return empty;
            }
        }

        #endregion
    }
}