using System;

namespace Never.Logging
{
    /// <summary>
    /// 日志输出
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 以debug等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        void Debug(string message);

        /// <summary>
        /// 以debug等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="exception">异常信息</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        void Debug(string message, Exception exception);

        /// <summary>
        /// 以Error等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        void Error(string message);

        /// <summary>
        /// 以Error等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="exception">异常信息</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        void Error(string message, Exception exception);

        /// <summary>
        /// 以Fatal等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        void Fatal(string message);

        /// <summary>
        /// 以Fatal等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="exception">异常信息</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        void Fatal(string message, Exception exception);

        /// <summary>
        /// 以Info等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        void Info(string message);

        /// <summary>
        /// 以Info等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="exception">异常信息</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        void Info(string message, Exception exception);

        /// <summary>
        /// 以Warn等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        void Warn(string message);

        /// <summary>
        /// 以Warn等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="exception">异常信息</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        void Warn(string message, Exception exception);
    }
}