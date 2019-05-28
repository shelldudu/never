using Never.Exceptions;
using Never.Utils;
using Never.WorkFlow.Exceptions;
using Never.WorkFlow.Repository;
using System;
using System.Collections.Generic;

namespace Never.WorkFlow.Coordinations
{
    /// <summary>
    /// 任务引擎，所有的任务都是从这里开始工作的
    /// </summary>
    public class TaskschedQueue : ITaskschedQueue
    {
        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="workflowengine"></param>
        /// <param name="excutingRepository"></param>
        /// <param name="templateRepository"></param>
        public TaskschedQueue(IWorkFlowEngine workflowengine, IExecutingRepository excutingRepository, ITemplateRepository templateRepository)
        {
            this.workflowengine = workflowengine;
            this.excutingRepository = excutingRepository;
            this.templateRepository = templateRepository;
        }

        #endregion ctor

        #region field

        private readonly IExecutingRepository excutingRepository = null;
        private readonly ITemplateRepository templateRepository = null;
        private readonly IWorkFlowEngine workflowengine = null;

        #endregion field

        #region push

        /// <summary>
        /// 查询一个任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public TaskschedNode Get(Guid taskId)
        {
            return this.excutingRepository.Get(taskId);
        }

        /// <summary>
        /// 查询用户标识的所有任务
        /// </summary>
        /// <param name="statusArray">状态</param>
        /// <returns></returns>
        public TaskschedNode[] GetAll(TaskschedStatus[] statusArray)
        {
            return this.excutingRepository.GetAll(statusArray);
        }

        /// <summary>
        /// 查询用户标识的所有任务
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="statusArray">状态</param>
        /// <returns></returns>
        public TaskschedNode[] GetAll(TaskschedStatus[] statusArray, string commandType)
        {
            return this.excutingRepository.GetAll(statusArray, commandType);
        }

        /// <summary>
        /// 查询用户标识的所有任务
        /// </summary>
        /// <param name="userSign"></param>
        /// <param name="statusArray"></param>
        /// <param name="startTime">开始时间</param>
        /// <returns></returns>
        public TaskschedNode[] GetAll(string userSign, TaskschedStatus[] statusArray, DateTime? startTime = null)
        {
            return this.excutingRepository.GetAll(userSign, statusArray, startTime);
        }

        /// <summary>
        /// 查询用户标识的所有任务
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="userSign">用户标识</param>
        /// <param name="statusArray">状态</param>
        /// <param name="startTime">开始时间</param>
        /// <returns></returns>
        public TaskschedNode[] GetAll(string userSign, string commandType, TaskschedStatus[] statusArray, DateTime? startTime = null)
        {
            return this.excutingRepository.GetAll(userSign, commandType, statusArray, startTime);
        }

        /// <summary>
        /// 将当前激发的消息压入队列中，开始进行按模板的步骤进行工作流，并返回当前模板组织的节点信息
        /// </summary>
        /// <param name="userSign">用户标识，长度不能超过80个字符</param>
        /// <param name="templateName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public TaskschedNode Push<TWorkStepMessage>(string templateName, string userSign, TWorkStepMessage message) where TWorkStepMessage : IWorkStepMessage
        {
            return this.Push(templateName, userSign, null, message);
        }

        /// <summary>
        /// 将当前激发的消息压入队列中，开始进行按模板的步骤进行工作流，并返回当前模板组织的节点信息
        /// </summary>
        /// <param name="commandType">命令类型，长度不能超过20个字符</param>
        /// <param name="userSign">用户标识，长度不能超过80个字符</param>
        /// <param name="templateName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public TaskschedNode Push<TWorkStepMessage>(string templateName, string userSign, string commandType, TWorkStepMessage message) where TWorkStepMessage : IWorkStepMessage
        {
            var template = this.templateRepository.Get(this.workflowengine.JsonSerializer, templateName);
            if (template == null)
            {
                throw new TemplateNotFoundException("没有找到该{0}模板内容", new[] { templateName });
            }

            if (commandType.IsNullOrWhiteSpace())
            {
                throw new WorkFlowKeyTooLongException("commandType is null");
            }

            if (commandType.Length > 20)
            {
                throw new WorkFlowKeyTooLongException("commandType长度超过了20个字符");
            }

            if (userSign.IsNotNullOrEmpty() && userSign.Length > 80)
            {
                throw new WorkFlowKeyTooLongException("userSign长度超过了80个字符");
            }

            var task = new TaskschedNode()
            {
                TaskId = NewId.GenerateGuid(),
                ProcessPercent = 0m,
                StartTime = DateTime.Now.AddSeconds(10),
                NextStep = 1,
                Status = TaskschedStatus.Working,
                UserSign = userSign,
                UserSignState = this.GetASCII(userSign),
                CommandType = commandType,
                StepCount = template.StepCount - 2,
            };

            task.PushMessage = this.SerializeObject(task, message);
            var nodes = new List<ExecutingNode>(task.StepCount - 2);
            foreach (var step in template.Steps)
            {
                if (step.Order == 0)
                {
                    continue;
                }

                if (step.Order == template.StepCount - 1)
                {
                    continue;
                }

                var node = new ExecutingNode()
                {
                    TaskId = task.TaskId,
                    StepCount = task.StepCount - 2,
                    RowId = NewId.GenerateGuid(),
                    OrderNo = step.Order,
                    StepType = step.ToStepType(),
                    StepCoordinationMode = step.Mode,
                    StartTime = DateTime.Now,
                    FailTimes = 0,
                    ExecutingMessage = string.Empty,
                    ResultMessage = string.Empty,
                };

                nodes.Add(node);
            }

            this.excutingRepository.Save(task, nodes.ToArray());
            return task;
        }

        /// <summary>
        /// 获取routeKey的ascuill总值
        /// </summary>
        /// <param name="routeKey">路由Key</param>
        /// <returns></returns>
        private int GetASCII(string routeKey)
        {
            if (string.IsNullOrEmpty(routeKey))
            {
                return 0;
            }

            var index = 0;
            foreach (var c in routeKey.ToCharArray())
            {
                index += c;
            };

            return index;
        }

        private string SerializeObject<TWorkStepMessage>(TaskschedNode task, TWorkStepMessage message) where TWorkStepMessage : IWorkStepMessage
        {
            SetTaskId<TWorkStepMessage>.Register().Invoke(message, task.TaskId);
            var msg = new Never.Messages.MessagePacket()
            {
                Body = this.workflowengine.JsonSerializer.SerializeObject(message),
                ContentType = Never.Messages.MessagePacket.GetContentType(message),
            };

            return this.workflowengine.JsonSerializer.SerializeObject(msg);
        }

        /// <summary>
        /// set
        /// </summary>
        /// <typeparam name="TWorkStepMessage"></typeparam>
        private class SetTaskId<TWorkStepMessage> where TWorkStepMessage : IWorkStepMessage
        {
            private static Action<TWorkStepMessage, Guid> action = null;

            public static Action<TWorkStepMessage, Guid> Register()
            {
                if (action != null)
                {
                    return action;
                }

                var il = Never.Reflection.EasyEmitBuilder<Action<TWorkStepMessage, Guid>>.NewDynamicMethod();
                var method = typeof(TWorkStepMessage).GetMethod("set_TaskId");
                if (method != null)
                {
                    il.LoadArgument(0);
                    il.LoadArgument(1);
                    il.Call(method);
                }

                il.Return();
                return action = il.CreateDelegate();
            }
        }

        #endregion push
    }
}