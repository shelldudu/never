using System;

namespace Never.Logging
{
    /// <summary>
    /// 日志输出管理器
    /// </summary>
    public interface ILoggerBuilder
    {
        /// <summary>
        /// 构建实例
        /// </summary>
        /// <param name="loggerName">日志名字</param>
        /// <returns></returns>
        ILogger Build(string loggerName);

        /// <summary>
        /// 构建实例
        /// </summary>
        /// <param name="loggerType">日志类型</param>
        /// <returns></returns>
        ILogger Build(Type loggerType);
    }
}