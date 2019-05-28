using Never.Logging;
using System;

namespace Never.Aop
{
    /// <summary>
    /// 写日志属性
    /// </summary>
    public class LoggerAttribute : Attribute, IRuntimeMode
    {
        /// <summary>
        /// 注册在IoC容器里面的key
        /// </summary>
        public string RegisterKey { get; set; }

        /// <summary>
        /// 运行方式，不同环境应该有不同的运行方式，所有标准应该为：环境提供任何一个值与其相同则工作，比如当前值为空，环境提供不是空值则不工作
        /// </summary>
        public string RuntimeMode { get; set; }

        /// <summary>
        /// 在出错的时候写日志
        /// </summary>
        /// <param name="runtimeModeProvider">环境运行提供者，如果为空则不写，如有需要，则重写该方法则可</param>
        /// <param name="loggerBuilder">日志构建者</param>
        /// <param name="executor">执行者</param>
        /// <param name="exception">异常信息</param>
        /// <param name="context">上下文</param>
        public void OnError(IRuntimeModeProvider runtimeModeProvider, ILoggerBuilder loggerBuilder, object executor, Exception exception, object context)
        {
            var logger = this.GetLogger(runtimeModeProvider, loggerBuilder, executor, exception, context);
            if (logger == null)
                return;

            if (!this.Match(runtimeModeProvider, loggerBuilder, executor, exception, context))
                return;

            this.Write(runtimeModeProvider, logger, executor, exception, context);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="runtimeModeProvider">环境运行提供者，如果为空则不写，如有需要，则重写该方法则可</param>
        /// <param name="logger">日志</param>
        /// <param name="executor">执行者</param>
        /// <param name="exception">异常信息</param>
        /// <param name="context">上下文</param>
        protected virtual void Write(IRuntimeModeProvider runtimeModeProvider, ILogger logger, object executor, Exception exception, object context)
        {
            logger.Error("logger attribute", exception);
        }

        /// <summary>
        /// 返回logger
        /// </summary>
        /// <param name="runtimeModeProvider">环境运行提供者，如果为空则不写，如有需要，则重写该方法则可</param>
        /// <param name="loggerBuilder">日志构建者</param>
        /// <param name="executor">执行者</param>
        /// <param name="exception">异常信息</param>
        /// <param name="context">上下文</param>
        protected virtual ILogger GetLogger(IRuntimeModeProvider runtimeModeProvider, ILoggerBuilder loggerBuilder, object executor, Exception exception, object context)
        {
            if (loggerBuilder == null)
                return null;

            if (executor == null)
                return loggerBuilder.Build(typeof(LoggerAttribute));

            return loggerBuilder.Build(executor.GetType());
        }

        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="runtimeModeProvider">环境运行提供者，如果为空则不写，如有需要，则重写该方法则可</param>
        /// <param name="loggerBuilder">日志构建者</param>
        /// <param name="executor">执行者</param>
        /// <param name="exception">异常信息</param>
        /// <param name="context">上下文</param>
        protected virtual bool Match(IRuntimeModeProvider runtimeModeProvider, ILoggerBuilder loggerBuilder, object executor, Exception exception, object context)
        {
            if (runtimeModeProvider == null)
                return false;

            if (runtimeModeProvider.RuntimeModeArray == null || runtimeModeProvider.RuntimeModeArray.Length <= 0)
                return true;

            foreach (var i in runtimeModeProvider.RuntimeModeArray)
            {
                if (this.RuntimeMode.IsEquals(i.RuntimeMode))
                    return true;
            }

            return false;
        }
    }
}