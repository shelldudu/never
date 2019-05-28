using System;
using System.Collections.Generic;

namespace Never.Logging
{
    /// <summary>
    /// 日志输出
    /// </summary>
    /// <remarks>使用装饰者设计（代理）模式，其本身不会实现</remarks>
    public sealed class AllotLogger : ILogger
    {
        #region field

        /// <summary>
        /// 日志输出对象集合
        /// </summary>
        private readonly ICollection<ILogger> loggerList = null;

        #endregion field

        #region

        /// <summary>
        /// Initializes a new instance of the <see cref="AllotLogger"/> class.
        /// </summary>
        /// <param name="loggerList">日志输出对象集合</param>
        public AllotLogger(ICollection<ILogger> loggerList)
        {
            this.loggerList = loggerList;
        }

        #endregion

        #region ILogger成员

        /// <summary>
        /// 以debug等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        public void Debug(string message)
        {
            if (this.loggerList == null)
                return;

            foreach (var logger in this.loggerList)
            {
                if (logger == null)
                    continue;
                logger.Debug(message);
            }
        }

        /// <summary>
        /// 以debug等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="exception">异常信息</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        public void Debug(string message, Exception exception)
        {
            if (this.loggerList == null)
                return;

            foreach (var logger in this.loggerList)
            {
                if (logger == null)
                    continue;
                logger.Debug(message, exception);
            }
        }

        /// <summary>
        /// 以Error等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        public void Error(string message)
        {
            if (this.loggerList == null)
                return;

            foreach (var logger in this.loggerList)
            {
                if (logger == null)
                    continue;
                logger.Error(message);
            }
        }

        /// <summary>
        /// 以Error等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="exception">异常信息</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        public void Error(string message, Exception exception)
        {
            if (this.loggerList == null)
                return;

            foreach (var logger in this.loggerList)
            {
                if (logger == null)
                    continue;
                logger.Error(message, exception);
            }
        }

        /// <summary>
        /// 以Fatal等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        public void Fatal(string message)
        {
            if (this.loggerList == null)
                return;

            foreach (var logger in this.loggerList)
            {
                if (logger == null)
                    continue;
                logger.Fatal(message);
            }
        }

        /// <summary>
        /// 以Fatal等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="exception">异常信息</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        public void Fatal(string message, Exception exception)
        {
            if (this.loggerList == null)
                return;

            foreach (var logger in this.loggerList)
            {
                if (logger == null)
                    continue;
                logger.Fatal(message, exception);
            }
        }

        /// <summary>
        /// 以Info等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        public void Info(string message)
        {
            if (this.loggerList == null)
                return;

            foreach (var logger in this.loggerList)
            {
                if (logger == null)
                    continue;
                logger.Info(message);
            }
        }

        /// <summary>
        /// 以Info等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="exception">异常信息</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        public void Info(string message, Exception exception)
        {
            if (this.loggerList == null)
                return;

            foreach (var logger in this.loggerList)
            {
                if (logger == null)
                    continue;
                logger.Info(message, exception);
            }
        }

        /// <summary>
        /// 以Warn等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        public void Warn(string message)
        {
            if (this.loggerList == null)
                return;

            foreach (var logger in this.loggerList)
            {
                if (logger == null)
                    continue;
                logger.Warn(message);
            }
        }

        /// <summary>
        /// 以Warn等级输出日志内容(debug &lt; info &lt; warn &lt; error &lt; fatal)
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="exception">异常信息</param>
        /// <remarks>
        /// 日志等级为debug &lt; info &lt; warn &lt; error &lt; fatal
        /// </remarks>
        public void Warn(string message, Exception exception)
        {
            if (this.loggerList == null)
                return;

            foreach (var logger in this.loggerList)
            {
                if (logger == null)
                    continue;
                logger.Warn(message, exception);
            }
        }

        #endregion

        #region Empty

        /// <summary>
        /// 空对象
        /// </summary>
        private static AllotLogger empty = new AllotLogger(null);

        /// <summary>
        /// 空对象
        /// </summary>
        public static AllotLogger Empty
        {
            get
            {
                return empty;
            }
        }

        #endregion
    }
}