using Never.IoC;
using Never.Logging;
using Never.Messages;
using Never.WorkFlow.Messages;
using Never.WorkFlow.Repository;
using Never.WorkFlow.WorkSteps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.WorkFlow.Coordinations
{
    /// <summary>
    /// 任务引擎
    /// </summary>
    public sealed class TaskschedEngine : ITaskschedEngine
    {
        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="executingRepository"></param>
        public TaskschedEngine(IWorkFlowEngine engine, IExecutingRepository executingRepository)
        {
            this.engine = engine;
            this.executingRepository = executingRepository;
        }

        #endregion ctor

        #region field

        /// <summary>
        /// 执行仓库
        /// </summary>
        private readonly IExecutingRepository executingRepository = null;

        /// <summary>
        /// 引擎
        /// </summary>
        private readonly IWorkFlowEngine engine = null;

        #endregion field

        #region prop

        /// <summary>
        /// 消息发送者
        /// </summary>
        public IMessagePublisher MessagePublisher { get; set; }

        /// <summary>
        /// 并发处理
        /// </summary>
        public ISerialTaskStrategy SerialTaskStrategy { get; set; }

        #endregion prop

        #region execute

        /// <summary>
        /// 执行该队列下所有任务
        /// </summary>
        /// <param name="taskId"></param>
        public void Execute(Guid taskId)
        {
            var task = this.executingRepository.Get(taskId);
            this.Execute(task);
        }

        /// <summary>
        /// 执行该队列下所有任务
        /// </summary>
        /// <param name="task"></param>
        public void Execute(TaskschedNode task)
        {
            if (this.SerialTaskStrategy == null)
            {
                this.Executing(task);
            }
            else
            {
                this.SerialTaskStrategy.LockAndInvoke(task, () => this.Executing(task));
            }
        }

        /// <summary>
        /// 执行该队列下所有任务
        /// </summary>
        private void Executing(TaskschedNode task)
        {
            if (task.FinishTime.HasValue || task.Status != TaskschedStatus.Working)
            {
                return;
            }

            /*还没开始工作*/
            if (task.NextTime.HasValue && task.NextTime.Value > DateTime.Now)
            {
                return;
            }

            if (task.PushMessage.IsNullOrEmpty())
            {
                task.FinishTime = task.FinishTime.HasValue ? task.FinishTime.Value : DateTime.Now;
                task.Status = TaskschedStatus.Finish;
                this.executingRepository.Finish(task);
                return;
            }

            var nodes = this.executingRepository.GetAll(task.TaskId);
            if (nodes == null || nodes.Count() == 0)
            {
                task.FinishTime = task.FinishTime.HasValue ? task.FinishTime.Value : DateTime.Now;
                task.Status = TaskschedStatus.Finish;
                this.executingRepository.Finish(task);
                return;
            }

            /*准备好工作了*/
            var context = new WorkContext()
            {
                TaskId = task.TaskId,
                TaskschedNode = task,
                JsonSerializer = this.engine.JsonSerializer,
                Strategy = this.engine.Strategy,
            };

            var list = new List<ExecutingNode>(nodes.Length + 2);
            list.Add(new ExecutingNode
            {
                ResultMessage = task.PushMessage,
                StepCount = nodes.Length + 2,
                StartTime = DateTime.Now,
                StepCoordinationMode = CoordinationMode.Any,
                WaitTimes = 0,
                FailTimes = 0,
                FinishTime = DateTime.Now,
                OrderNo = 0,
                RowId = Guid.Empty,
                StepType = "aa1b00b7cf4e",
                TaskId = task.TaskId,
                ExecutingMessage = string.Empty,
            });

            list.AddRange(nodes.OrderBy(ta => ta.OrderNo));
            list.Add(new ExecutingNode
            {
                ResultMessage = task.PushMessage,
                StepCount = nodes.Length + 2,
                StartTime = DateTime.Now,
                StepCoordinationMode = CoordinationMode.Any,
                WaitTimes = 0,
                FailTimes = 0,
                FinishTime = DateTime.Now,
                OrderNo = nodes.Length + 1,
                RowId = Guid.Empty,
                StepType = "aa1b00b7d77f",
                TaskId = task.TaskId,
                ExecutingMessage = string.Empty,
            });

            nodes = list.ToArray();

            foreach (var node in nodes)
            {
                context.ExecutingNode = null;
                /*第一步*/
                if (node.OrderNo == 0)
                {
                    continue;
                }

                if (node.FinishTime.HasValue)
                {
                    continue;
                }

                /*最后一步，应该要当完成*/
                if (node.OrderNo == task.StepCount + 1 || task.FinishTime.HasValue)
                {
                    task.FinishTime = task.FinishTime.HasValue ? task.FinishTime.Value : DateTime.Now;
                    task.Status = TaskschedStatus.Finish;
                    this.executingRepository.Finish(task);
                    return;
                }

                var worksteps = new List<KeyValueTuple<IWorkStep, string>>(node.StepCount);
                var preMsg = default(IWorkStepMessage);
                try
                {
                    if (!this.IsReady(this.engine.Strategy, task, nodes, node, worksteps, out preMsg))
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    node.ExecutingMessage = ex.GetInnerException().GetFullMessage();
                    node.FailTimes++;
                    var timeSpan = this.engine.Strategy.NextTime(task, node, ex);
                    if (timeSpan != TimeSpan.Zero)
                    {
                        task.NextTime = task.NextTime.HasValue ? task.NextTime.Value.Add(timeSpan) : DateTime.Now.Add(timeSpan);
                        this.executingRepository.Exception(task, node);
                        return;
                    }
                }

                context.RowId = node.RowId;
                context.ExecutingNode = node;

                if (preMsg is IWorkStepMessageValidator)
                {
                    try
                    {
                        ((IWorkStepMessageValidator)preMsg).Validate(context);
                        if (!context.ParamaterResult.IsValid)
                        {
                            var sb = new StringBuilder(200);
                            foreach (var error in context.ParamaterResult.Errors)
                            {
                                sb.AppendFormat("the {0} step has error: paramater key is : {1} ,validate result is {2};", node.OrderNo, error.MemberName, error.ErrorMessage);
                            }

                            var finishNodes = new List<ExecutingNode>(nodes.Count());
                            foreach (var t in nodes.Where(t => t.OrderNo >= node.OrderNo))
                            {
                                if (t.OrderNo == 0 || t.OrderNo == nodes.Length - 1)
                                {
                                    continue;
                                }

                                t.ExecutingMessage = sb.ToString();
                                finishNodes.Add(t);
                            }

                            this.executingRepository.Abort(task, finishNodes.ToArray());
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        /*这里的导演应该是本引擎执行的异常了，因工作步骤的异常都会记录到每次的任务中*/
                        this.ResolveLoggerBuilder().Build(typeof(TaskschedEngine)).Error("验证消息参数出错", ex);
                    }
                }

                try
                {
                    switch (node.StepCoordinationMode)
                    {
                        default:
                        case CoordinationMode.Any:
                            {
                                if (this.ExecuteAny(this.engine.Strategy, task, nodes, node, worksteps, context, preMsg))
                                {
                                    continue;
                                }

                                return;
                            }

                        case CoordinationMode.Meanwhile:
                            {
                                if (this.ExecuteMeanwhile(this.engine.Strategy, task, nodes, node, worksteps, context, preMsg))
                                {
                                    continue;
                                }

                                return;
                            }
                    }
                }
                catch (Exception ex)
                {
                    /*这里的导演应该是本引擎执行的异常了，因工作步骤的异常都会记录到每次的任务中*/
                    this.ResolveLoggerBuilder().Build(typeof(TaskschedEngine)).Error("执行本地储存更新失败", ex);
                }
            }
        }

        /// <summary>
        /// 检查工作
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="task"></param>
        /// <param name="nodes"></param>
        /// <param name="node"></param>
        /// <param name="worksteps"></param>
        /// <param name="preMsg"></param>
        /// <returns></returns>
        private bool IsReady(ITaskschedStrategy strategy, TaskschedNode task, ExecutingNode[] nodes, ExecutingNode node, IList<KeyValueTuple<IWorkStep, string>> worksteps, out IWorkStepMessage preMsg)
        {
            preMsg = null;
            /*获取前一个节点信息*/
            var preNode = nodes.FirstOrDefault(o => o.OrderNo == node.OrderNo - 1);
            var finishNodes = new List<ExecutingNode>(nodes.Count());

            if (preNode == null)
            {
                foreach (var t in nodes.Where(t => t.OrderNo >= node.OrderNo))
                {
                    if (t.OrderNo == 0 || t.OrderNo == nodes.Length - 1)
                    {
                        continue;
                    }

                    t.ExecutingMessage = string.Format("当前步骤{0}找不到上一步{1}信息，执行中断", node.OrderNo, node.OrderNo - 1);
                    finishNodes.Add(t);
                }

                this.executingRepository.Abort(task, finishNodes.ToArray());
                return false;
            }

            var prePacket = this.engine.JsonSerializer.Deserialize<Never.Messages.MessagePacket>(preNode.ResultMessage);
            if (prePacket == null)
            {
                foreach (var t in nodes.Where(t => t.OrderNo >= node.OrderNo))
                {
                    if (t.OrderNo == 0 || t.OrderNo == nodes.Length - 1)
                    {
                        continue;
                    }

                    t.ExecutingMessage = string.Format("当前步骤{0}消息类型不是Never.Messages.MessagePacket，执行中断", node.OrderNo);
                    finishNodes.Add(t);
                }

                this.executingRepository.Abort(task, finishNodes.ToArray());
                return false;
            }

            var preMsgType = Type.GetType(prePacket.ContentType);
            if (preMsgType == null)
            {
                foreach (var t in nodes.Where(t => t.OrderNo >= node.OrderNo))
                {
                    if (t.OrderNo == 0 || t.OrderNo == nodes.Length - 1)
                    {
                        continue;
                    }

                    t.ExecutingMessage = string.Format("当前步骤{0}消息类型{1}不能在当前环境中找到类型，执行中断", node.OrderNo, prePacket.ContentType);
                    finishNodes.Add(t);
                }

                this.executingRepository.Abort(task, finishNodes.ToArray());
                return false;
            }

            preMsg = this.engine.JsonSerializer.DeserializeObject(prePacket.Body, preMsgType) as IWorkStepMessage;
            if (preMsgType == typeof(MeanwhileWorkStepMessage))
            {
                var multiplemsg = new MultipleWorkStepMessage();
                multiplemsg.TaskId = ((MeanwhileWorkStepMessage)preMsg).TaskId;
                foreach (var msg in ((MeanwhileWorkStepMessage)preMsg).Messages)
                {
                    var pck = this.engine.JsonSerializer.Deserialize<Never.Messages.MessagePacket>(msg);
                    var pmsg = this.engine.JsonSerializer.DeserializeObject(pck.Body, Type.GetType(pck.ContentType)) as IWorkStepMessage;
                    multiplemsg.Append(pmsg);
                }
            }

            if (preMsg == null)
            {
                foreach (var t in nodes.Where(t => t.OrderNo >= node.OrderNo))
                {
                    if (t.OrderNo == 0 || t.OrderNo == nodes.Length - 1)
                    {
                        continue;
                    }

                    t.ExecutingMessage = string.Format("当前步骤{0}消息类型{1}不是指定的IWorkStepMessage消息，执行中断", node.OrderNo, prePacket.ContentType);
                    finishNodes.Add(t);
                }

                this.executingRepository.Abort(task, finishNodes.ToArray());
                return false;
            }

            var stepTypes = node.StepType.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            string nullStepType = string.Empty;

            /*fix guid*/
            foreach (var step in stepTypes)
            {
                var stepType = TemplateWorkStep.Find(step);
                if (stepType != null)
                {
                    if (ContainerContext.Current == null)
                    {
                        var workstep = Activator.CreateInstance(stepType) as IWorkStep;
                        worksteps.Add(new KeyValueTuple<IWorkStep, string>(workstep, step));
                    }
                    else
                    {
                        var workstep = ContainerContext.Current.ServiceLocator.Resolve(stepType) as IWorkStep;
                        worksteps.Add(new KeyValueTuple<IWorkStep, string>(workstep, step));
                    }
                }
                else
                {
                    nullStepType = step;
                    worksteps.Add(new KeyValueTuple<IWorkStep, string>(null, step));
                }
            }

            if (worksteps.All(o => o.Key == null))
            {
                foreach (var t in nodes.Where(t => t.OrderNo >= node.OrderNo))
                {
                    if (t.OrderNo == 0 || t.OrderNo == nodes.Length - 1)
                    {
                        continue;
                    }

                    t.ExecutingMessage = string.Format("当前步骤{0}所有工作实例不能在当前环境中实例化，执行中断", node.OrderNo);
                    finishNodes.Add(t);
                }

                this.executingRepository.Abort(task, finishNodes.ToArray());
                return false;
            }

            if (node.StepCoordinationMode == CoordinationMode.Meanwhile && nullStepType.IsNotNullOrEmpty())
            {
                foreach (var t in nodes.Where(t => t.OrderNo >= node.OrderNo))
                {
                    if (t.OrderNo == 0 || t.OrderNo == nodes.Length - 1)
                    {
                        continue;
                    }

                    t.ExecutingMessage = string.Format("当前步骤{0}工作实例{1}不能在当前环境中找到类型，执行中断", node.OrderNo, nullStepType);
                    finishNodes.Add(t);
                }

                this.executingRepository.Abort(task, finishNodes.ToArray());
                return false;
            }

            return true;
        }

        /// <summary>
        /// 开始工作
        /// </summary>
        private bool ExecuteAny(ITaskschedStrategy strategy, TaskschedNode task, ExecutingNode[] nodes, ExecutingNode node, IList<KeyValueTuple<IWorkStep, string>> worksteps, WorkContext context, IWorkStepMessage preMsg)
        {
            var timeSpan = TimeSpan.Zero;
            Exception lastException = null;
            IWorkStepMessage msg = null;
            var finishNodes = new List<ExecutingNode>(nodes.Count());

            foreach (var workstep in worksteps)
            {
                try
                {
                    msg = workstep.Key.Execute(context, preMsg);
                    if (msg != null)
                    {
                        task.AttachState = msg.AttachState;
                    }
                    lastException = null;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    context.StepClear();
                    continue;
                }

                if (msg == null)
                {
                    context.StepClear();
                    continue;
                }

                if (msg is EmptyAndAbortedWorkStepMessage)
                {
                    foreach (var t in nodes.Where(t => t.OrderNo >= node.OrderNo))
                    {
                        if (t.OrderNo == 0 || t.OrderNo == nodes.Length - 1)
                        {
                            continue;
                        }

                        t.ExecutingMessage = string.Format("当前步骤{0}执行了中断行为，消息结果为：{1}", node.OrderNo, ((EmptyAndAbortedWorkStepMessage)msg).Message);
                        finishNodes.Add(t);
                    }

                    this.executingRepository.Abort(task, finishNodes.ToArray());
                    if (this.MessagePublisher != null)
                    {
                        try
                        {
                            this.MessagePublisher.PublishMessageAsync(msg);
                        }
                        catch
                        {
                        }
                    }
                    return false;
                }

                if (msg is EmptyButWaitingWorkStepMessage)
                {
                    var span = strategy.NextWorkTimeOnMessageIsWaiting(task, node);
                    node.ExecutingMessage = ((EmptyButWaitingWorkStepMessage)msg).Message;
                    task.NextTime = task.NextTime.HasValue ? task.NextTime.Value.Add(span) : DateTime.Now.Add(span);
                    this.executingRepository.NextWork(task, node);
                    if (this.MessagePublisher != null)
                    {
                        try
                        {
                            this.MessagePublisher.PublishMessageAsync(msg);
                        }
                        catch
                        {
                        }
                    }

                    return false;
                }

                if (msg.TaskId == Guid.Empty)
                {
                    this.SetMessageTaskId(msg, task);
                }

                var packet = new Never.Messages.MessagePacket()
                {
                    ContentType = Never.Messages.MessagePacket.GetContentType(msg),
                    Body = this.engine.JsonSerializer.SerializeObject(msg),
                };

                context.StepClear();

                node.ResultMessage = this.engine.JsonSerializer.Serialize(packet);
                node.FinishTime = DateTime.Now;
                task.ProcessPercent = task.StepCount <= 0 ? 100m : ((100 * (task.NextStep) / ((decimal)task.StepCount)).FormatR(2));

                if (task.NextStep == task.StepCount)
                {
                    task.FinishTime = DateTime.Now;
                    this.executingRepository.AllDone(task, node);
                }
                else
                {
                    task.NextStep = task.NextStep + 1;
                    this.executingRepository.Done(task, node);
                }

                if (this.MessagePublisher != null)
                {
                    try
                    {
                        this.MessagePublisher.PublishMessageAsync(msg);
                    }
                    catch
                    {
                    }
                }

                return true;
            }

            if (lastException != null)
            {
                if (this.MessagePublisher != null)
                {
                    try
                    {
                        this.MessagePublisher.PublishMessageAsync(new ExecutingErrorWorkStepMessage() { TaskId = task.TaskId, CommandType = task.CommandType, UserSign = task.UserSign, Exception = lastException, PushMessage = task.PushMessage });
                    }
                    finally
                    {
                    }
                }

                node.ExecutingMessage = lastException.GetInnerException().GetFullMessage();
                node.FailTimes++;
                timeSpan = strategy.NextTime(task, node, lastException);
            }

            if (timeSpan != TimeSpan.Zero)
            {
                task.NextTime = task.NextTime.HasValue ? task.NextTime.Value.Add(timeSpan) : DateTime.Now.Add(timeSpan);
                this.executingRepository.Exception(task, node);
                return false;
            }

            timeSpan = strategy.NextWorkTimeOnMessageIsNull(task, node, node.WaitTimes + 1);
            if (timeSpan != TimeSpan.Zero)
            {
                node.WaitTimes++;
                task.NextTime = task.NextTime.HasValue ? task.NextTime.Value.Add(timeSpan) : DateTime.Now.Add(timeSpan);
                this.executingRepository.NextWork(task, node);
                return false;
            }

            foreach (var t in nodes.Where(t => t.OrderNo >= node.OrderNo))
            {
                if (t.OrderNo == 0 || t.OrderNo == nodes.Length - 1)
                {
                    continue;
                }

                t.ExecutingMessage = string.Format("当前步骤{0}协同方式为任何,但所有工作实例返回空消息，执行中断", node.OrderNo);
                finishNodes.Add(t);
            }

            this.executingRepository.Abort(task, finishNodes.ToArray());
            return false;
        }

        /// <summary>
        /// 开始工作
        /// </summary>
        private bool ExecuteMeanwhile(ITaskschedStrategy strategy, TaskschedNode task, ExecutingNode[] nodes, ExecutingNode node, IList<KeyValueTuple<IWorkStep, string>> worksteps, WorkContext context, IWorkStepMessage preMsg)
        {
            var finishNodes = new List<ExecutingNode>(nodes.Count());
            var messages = new MeanwhileWorkStepMessage();
            var msgs = new List<IWorkStepMessage>(nodes.Count());
            messages.TaskId = task.TaskId;

            IWorkStepMessage msg = null;
            var timeSpan = TimeSpan.Zero;
            Exception lastException = null;

            foreach (var workstep in worksteps)
            {
                try
                {
                    msg = workstep.Key.Execute(context, preMsg);
                    if (msg != null)
                    {
                        task.AttachState = msg.AttachState;
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    node.ExecutingMessage = ex.GetInnerException().GetFullMessage();
                    node.FailTimes++;
                    timeSpan = strategy.NextTime(task, node, ex);
                }
                if (msg is EmptyAndAbortedWorkStepMessage)
                {
                    foreach (var t in nodes.Where(t => t.OrderNo >= node.OrderNo))
                    {
                        if (t.OrderNo == 0 || t.OrderNo == nodes.Length - 1)
                        {
                            continue;
                        }

                        t.ExecutingMessage = string.Format("当前步骤{0}执行了中断行为，消息结果为：{1}", node.OrderNo, ((EmptyAndAbortedWorkStepMessage)msg).Message);
                        finishNodes.Add(t);
                    }

                    this.executingRepository.Abort(task, finishNodes.ToArray());
                    if (this.MessagePublisher != null)
                    {
                        try
                        {
                            this.MessagePublisher.PublishMessageAsync(msg);
                        }
                        catch
                        {
                        }
                    }
                    return false;
                }
                if (msg is EmptyButWaitingWorkStepMessage)
                {
                    var span = strategy.NextWorkTimeOnMessageIsWaiting(task, node);
                    node.ExecutingMessage = ((EmptyButWaitingWorkStepMessage)msg).Message;
                    task.NextTime = task.NextTime.HasValue ? task.NextTime.Value.Add(span) : DateTime.Now.Add(span);
                    this.executingRepository.NextWork(task, node);
                    if (this.MessagePublisher != null)
                    {
                        try
                        {
                            this.MessagePublisher.PublishMessageAsync(msg);
                        }
                        catch
                        {
                        }
                    }
                    return false;
                }

                if (lastException != null && this.MessagePublisher != null)
                {
                    try
                    {
                        this.MessagePublisher.PublishMessageAsync(new ExecutingErrorWorkStepMessage() { TaskId = task.TaskId, CommandType = task.CommandType, UserSign = task.UserSign, Exception = lastException, PushMessage = task.PushMessage });
                    }
                    finally
                    {
                    }
                }

                if (timeSpan != TimeSpan.Zero)
                {
                    task.NextTime = task.NextTime.HasValue ? task.NextTime.Value.Add(timeSpan) : DateTime.Now.Add(timeSpan);
                    this.executingRepository.Exception(task, node);
                    return false;
                }

                if (msg == null)
                {
                    timeSpan = strategy.NextWorkTimeOnMessageIsNull(task, node, node.WaitTimes + 1);
                    if (timeSpan != TimeSpan.Zero)
                    {
                        node.WaitTimes++;
                        task.NextTime = task.NextTime.HasValue ? task.NextTime.Value.Add(timeSpan) : DateTime.Now.Add(timeSpan);
                        this.executingRepository.NextWork(task, node);
                        return false;
                    }

                    foreach (var t in nodes.Where(t => t.OrderNo >= node.OrderNo))
                    {
                        if (t.OrderNo == 0 || t.OrderNo == nodes.Length - 1)
                        {
                            continue;
                        }

                        t.ExecutingMessage = string.Format("当前步骤{0}协同方式为全部,但工作实例{1}返回空消息，执行中断", node.OrderNo, workstep.Value);
                        finishNodes.Add(t);
                    }

                    this.executingRepository.Abort(task, finishNodes.ToArray());
                    return false;
                }

                if (msg.TaskId == Guid.Empty)
                {
                    this.SetMessageTaskId(msg, task);
                }

                msgs.Add(msg);

                messages.Messages.Add(this.engine.JsonSerializer.Serialize(new Never.Messages.MessagePacket()
                {
                    ContentType = Never.Messages.MessagePacket.GetContentType(msg),
                    Body = this.engine.JsonSerializer.Serialize(msg)
                }));
            }

            var mulitePacket = new Never.Messages.MessagePacket()
            {
                ContentType = Never.Messages.MessagePacket.GetContentType(messages),
                Body = this.engine.JsonSerializer.Serialize(messages),
            };

            context.StepClear();
            node.ResultMessage = this.engine.JsonSerializer.Serialize(mulitePacket);
            node.FinishTime = DateTime.Now;
            task.ProcessPercent = task.StepCount <= 0 ? 100m : ((100 * (task.NextStep) / ((decimal)task.StepCount)).FormatR(2));

            if (task.NextStep == task.StepCount)
            {
                task.FinishTime = DateTime.Now;
                this.executingRepository.AllDone(task, node);
            }
            else
            {
                task.NextStep = task.NextStep + 1;
                this.executingRepository.Done(task, node);
            }

            if (this.MessagePublisher != null)
            {
                try
                {
                    foreach (var m in msgs)
                    {
                        this.MessagePublisher.PublishMessageAsync(m);
                    }
                }
                catch
                {
                }
            }

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TWorkStepMessage"></typeparam>
        /// <param name="message"></param>
        /// <param name="task"></param>
        private void SetMessageTaskId<TWorkStepMessage>(TWorkStepMessage message, TaskschedNode task) where TWorkStepMessage : IWorkStepMessage
        {
            SetTaskId.SetId(message, task);
        }

        #endregion execute

        #region logger

        /// <summary>
        /// 当前系统是否已经注册了配置功能
        /// </summary>
        private static bool? registerLoggerBuilder = null;

        /// <summary>
        /// 查找投资服务对象
        /// </summary>
        /// <returns></returns>
        private ILoggerBuilder ResolveLoggerBuilder()
        {
            if (ContainerContext.Current == null)
            {
                return LoggerBuilder.Empty;
            }

            ILoggerBuilder @object = null;
            if (!registerLoggerBuilder.HasValue)
            {
                registerLoggerBuilder = ContainerContext.Current.ServiceLocator.TryResolve(ref @object);
                return @object;
            };

            if (registerLoggerBuilder.Value)
            {
                return ContainerContext.Current.ServiceLocator.Resolve<ILoggerBuilder>();
            }

            return null;
        }

        #endregion logger

        #region set id

        private class SetTaskId
        {
            private static Hashtable table = new Hashtable();

            public static void SetId(IWorkStepMessage message, TaskschedNode task)
            {
                var sourceType = message.GetType();
                var @delegate = table[sourceType] as Action<IWorkStepMessage, TaskschedNode>;
                if (@delegate != null)
                {
                    @delegate.Invoke(message, task);
                    return;
                }

                table[sourceType] = @delegate = (Action<IWorkStepMessage, TaskschedNode>)Delegate.CreateDelegate(typeof(Action<IWorkStepMessage, TaskschedNode>), typeof(SetTaskId).GetMethod("MySetId").MakeGenericMethod(sourceType));
                @delegate.Invoke(message, task);
                return;
            }

            public static void MySetId<TWorkStepMessage>(IWorkStepMessage message, TaskschedNode task)
            {
                SetTaskId<TWorkStepMessage>.Register().Invoke((TWorkStepMessage)message, task.TaskId);
            }
        }

        /// <summary>
        /// set
        /// </summary>
        /// <typeparam name="TWorkStepMessage"></typeparam>
        private class SetTaskId<TWorkStepMessage>
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

        #endregion set id
    }
}