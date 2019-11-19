using Never.DataAnnotations;
using Never.Security;
using Never.Serialization;
using Never.WorkFlow.Messages;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Never.WorkFlow.Coordinations
{
    /// <summary>
    /// 工作流上下文
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class WorkContext : IWorkContext, IWorkStepMessageValidateContext, IWorkContextCleaner, IDisposable
    {
        #region ctor

        /// <summary>
        ///
        /// </summary>
        public WorkContext()
        {
            this.Items = new System.Collections.Concurrent.ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.WorkTime = DateTime.Now;
        }

        #endregion ctor

        #region IWorkContext

        /// <summary>
        /// 当前执行的Id
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime WorkTime { get; set; }

        /// <summary>
        /// 执行者
        /// </summary>
        public IWorker Worker { get; set; }

        /// <summary>
        /// 用户字典
        /// </summary>
        public IDictionary<string, object> Items { get; set; }

        /// <summary>
        /// 任务策略
        /// </summary>
        public ITaskschedStrategy Strategy { get; set; }

        /// <summary>
        /// json序列化内容
        /// </summary>
        public IJsonSerializer JsonSerializer { get; set; }

        /// <summary>
        /// 任务节点
        /// </summary>
        public ITaskschedNode TaskschedNode { get; set; }

        /// <summary>
        /// 详情节点
        /// </summary>
        public IExecutingNode ExecutingNode { get; set; }

        /// <summary>
        /// 创建一个等待的消息
        /// </summary>
        /// <returns></returns>
        public IWorkStepMessage CreateWatiingMessage()
        {
            return new EmptyButWaitingWorkStepMessage() { TaskId = this.TaskId, UserSign = this.TaskschedNode.UserSign, CommandType = this.TaskschedNode.CommandType };
        }

        /// <summary>
        /// 创建一个等待的消息
        /// </summary>
        /// <returns></returns>
        public IWorkStepMessage CreateWatiingMessage(string message, int attachState = 0)
        {
            return new EmptyButWaitingWorkStepMessage() { Message = message, TaskId = this.TaskId, UserSign = this.TaskschedNode.UserSign, CommandType = this.TaskschedNode.CommandType };
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IWorkStepMessage CreateBreakingMessage(string message, int attachState = 0)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message cannt be null");
            }

            return new EmptyAndAbortedWorkStepMessage() { Message = message, TaskId = this.TaskId, UserSign = this.TaskschedNode.UserSign, CommandType = this.TaskschedNode.CommandType };
        }

        /// <summary>
        /// 在消息中匹配某个一消息，因为在一些组合步骤中会返回多个消息，message的消息是
        /// </summary>
        /// <typeparam name="TWorkStreamMessage"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public TWorkStreamMessage Match<TWorkStreamMessage>(IWorkStepMessage message) where TWorkStreamMessage : IWorkStepMessage
        {
            if (message is TWorkStreamMessage)
            {
                return (TWorkStreamMessage)message;
            }

            var messages = message as MultipleWorkStepMessage;
            if (messages == null)
            {
                return default(TWorkStreamMessage);
            }

            foreach (var msg in messages.Messages)
            {
                if (msg is TWorkStreamMessage)
                {
                    return (TWorkStreamMessage)msg;
                }
            }

            return default(TWorkStreamMessage);
        }

        /// <summary>
        /// 在字典上下文中查询某个对象，如果找不到则执行回调，并将其塞进上下文中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="resultCallBack"></param>
        /// <returns></returns>
        public T Find<T>(string key, Func<T> resultCallBack)
        {
            if (string.IsNullOrEmpty(key))
            {
                return resultCallBack == null ? default(T) : resultCallBack();
            }

            if (this.Items.ContainsKey(key))
            {
                var result = this.Items[key];
                if (result == null)
                {
                    if (resultCallBack == null)
                    {
                        return default(T);
                    }

                    result = resultCallBack();
                    if (result != null)
                    {
                        this.Items[key] = result;
                    }

                    return (T)result;
                }
                else
                {
                    if (result is T)
                    {
                        return (T)result;
                    }

                    if (resultCallBack == null)
                    {
                        return default(T);
                    }

                    result = resultCallBack();
                    if (result != null)
                    {
                        this.Items[key] = result;
                    }

                    return (T)result;
                }
            }
            else if (resultCallBack != null)
            {
                var result = (object)resultCallBack.Invoke();
                if (result == null)
                {
                    return default(T);
                }

                this.Items[key] = result;
                return (T)result;
            }

            return default(T);
        }

        #endregion IWorkContext

        #region IStepContext

        /// <summary>
        /// 当前步骤执行Id
        /// </summary>
        public Guid RowId { get; set; }

        #endregion IStepContext

        #region IStepContextCleaner

        /// <summary>
        /// 每一步完成清理
        /// </summary>
        public void StepClear()
        {
            this.paramaterRules = new List<KeyValuePair<LambdaExpression, string>>();
        }

        #endregion IStepContextCleaner

        #region IWorkStepMessageValidateContext

        /// <summary>
        /// 参数验证规则
        /// </summary>
        private List<KeyValuePair<LambdaExpression, string>> paramaterRules = new List<KeyValuePair<LambdaExpression, string>>();

        /// <summary>
        /// 参数验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="express"></param>
        /// <param name="message"></param>
        public void ParamaterRule<T>(Expression<System.Func<T, object>> express, string message) where T : IWorkStepMessage
        {
            this.paramaterRules.Add(new KeyValuePair<LambdaExpression, string>(express, message));
        }

        /// <summary>
        /// 验证结果
        /// </summary>
        public ValidationResult ParamaterResult
        {
            get
            {
                var paramaterResult = new List<ValidationFailure>();
                foreach (var rule in this.paramaterRules)
                {
                    Validator<object>.AddErrors(paramaterResult, rule.Key.Body, rule.Value);
                }

                return new ValidationResult(paramaterResult);
            }
        }

        #endregion IWorkStepMessageValidateContext

        #region IDisposable

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
        }

        #endregion IDisposable
    }
}