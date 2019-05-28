using Never;
using Never.Serialization;
using Never.WorkFlow.Coordinations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.WorkFlow.Repository
{
    /// <summary>
    /// sql 模式的执行任务仓库
    /// </summary>
    public abstract class TripleTableSqlExecutingRepository : SqlExecutingRepository, IExecutingRepository, ITemplateRepository
    {
        #region field

        private readonly Dictionary<string, Template> dict = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        public TripleTableSqlExecutingRepository() : this(10)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="capacity"></param>
        public TripleTableSqlExecutingRepository(int capacity)
        {
            this.dict = new Dictionary<string, Template>(capacity);
        }

        #endregion ctor

        #region executing

        /// <summary>
        /// 查询某状态的节点
        /// </summary>
        /// <param name="statusArray"></param>
        /// <returns></returns>
        public virtual TaskschedNode[] GetAll(TaskschedStatus[] statusArray)
        {
            const string sqlText = "select TaskId,UserSign,UserSignState,AttachState,CommandType,Status,StartTime,NextTime,FinishTime,StepCount,NextStep,ProcessPercent,PushMessage,CreateDate,EditDate,Version from {0}task where Status in (@Status) and StartTime >= @StartTime;";
            using (var sql = this.Open())
            {
                return sql.QueryForEnumerable<TaskschedNode>(string.Format(sqlText, this.TablePrefixName), new { @Status = statusArray.Select(o => (byte)o).ToArray(), StartTime = DateTime.Now }).ToArray();
            }
        }

        /// <summary>
        /// 查询所有的任务
        /// </summary>
        /// <param name="statusArray"></param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        public virtual TaskschedNode[] GetAll(TaskschedStatus[] statusArray, string commandType)
        {
            const string sqlText = "select TaskId,UserSign,AttachState,CommandType,Status,StartTime,NextTime,FinishTime,StepCount,NextStep,ProcessPercent,PushMessage,CreateDate,EditDate,Version from {0}task where Status in (@Status) and CommandType = @CommandType and StartTime >= @StartTime;";
            using (var sql = this.Open())
            {
                return sql.QueryForEnumerable<TaskschedNode>(string.Format(sqlText, this.TablePrefixName), new { @Status = statusArray.Select(o => (byte)o).ToArray(), @CommandType = commandType, StartTime = DateTime.Now }).ToArray();
            }
        }

        /// <summary>
        /// 查询某标识下的所有的任务
        /// </summary>
        /// <returns></returns>
        public virtual TaskschedNode[] GetAll(string usersign, TaskschedStatus[] statusArray, DateTime? startTime = null)
        {
            const string sqlText = "select TaskId,UserSign,UserSignState,AttachState,CommandType,Status,StartTime,NextTime,FinishTime,StepCount,NextStep,ProcessPercent,PushMessage,CreateDate,EditDate,Version from {0}task where UserSign = @UserSign and Status in (@Status) and StartTime >= @StartTime;";
            const string sqlText2 = "select TaskId,UserSign,UserSignState,AttachState,CommandType,Status,StartTime,NextTime,FinishTime,StepCount,NextStep,ProcessPercent,PushMessage,CreateDate,EditDate,Version from {0}task where UserSign = @UserSign and Status in (@Status);";
            const string sqlText3 = "select TaskId,UserSign,UserSignState,AttachState,CommandType,Status,StartTime,NextTime,FinishTime,StepCount,NextStep,ProcessPercent,PushMessage,CreateDate,EditDate,Version from {0}task where UserSign = @UserSign and StartTime >= @StartTime;";
            const string sqlText4 = "select TaskId,UserSign,UserSignState,AttachState,CommandType,Status,StartTime,NextTime,FinishTime,StepCount,NextStep,ProcessPercent,PushMessage,CreateDate,EditDate,Version from {0}task where UserSign = @UserSign;";
            using (var sql = this.Open())
            {
                if (statusArray != null && statusArray.Length > 0)
                {
                    if (startTime.HasValue)
                    {
                        return sql.QueryForEnumerable<TaskschedNode>(string.Format(sqlText, this.TablePrefixName), new { @Status = statusArray.Select(o => (byte)o).ToArray(), @UserSign = usersign, StartTime = startTime.Value }).ToArray();
                    }

                    return sql.QueryForEnumerable<TaskschedNode>(string.Format(sqlText2, this.TablePrefixName), new { @Status = statusArray.Select(o => (byte)o).ToArray(), @UserSign = usersign }).ToArray();
                }

                if (startTime.HasValue)
                {
                    return sql.QueryForEnumerable<TaskschedNode>(string.Format(sqlText3, this.TablePrefixName), new { @UserSign = usersign, StartTime = startTime.Value }).ToArray();
                }

                return sql.QueryForEnumerable<TaskschedNode>(string.Format(sqlText4, this.TablePrefixName), new { @UserSign = usersign }).ToArray();
            }
        }

        /// <summary>
        /// 查询某标识下的所有的任务
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="usersign"></param>
        /// <param name="statusArray"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public virtual TaskschedNode[] GetAll(string usersign, string commandType, TaskschedStatus[] statusArray, DateTime? startTime = null)
        {
            const string sqlText = "select TaskId,UserSign,UserSignState,AttachState,CommandType,Status,StartTime,NextTime,FinishTime,StepCount,NextStep,ProcessPercent,PushMessage,CreateDate,EditDate,Version from {0}task where UserSign = @UserSign and Status in (@Status) and CommandType = @CommandType and StartTime >= @StartTime;";
            const string sqlText2 = "select TaskId,UserSign,UserSignState,AttachState,CommandType,Status,StartTime,NextTime,FinishTime,StepCount,NextStep,ProcessPercent,PushMessage,CreateDate,EditDate,Version from {0}task where UserSign = @UserSign and Status in (@Status) and CommandType = @CommandType;";
            const string sqlText3 = "select TaskId,UserSign,UserSignState,AttachState,CommandType,Status,StartTime,NextTime,FinishTime,StepCount,NextStep,ProcessPercent,PushMessage,CreateDate,EditDate,Version from {0}task where UserSign = @UserSign and CommandType = @CommandType and StartTime >= @StartTime;";
            const string sqlText4 = "select TaskId,UserSign,UserSignState,AttachState,CommandType,Status,StartTime,NextTime,FinishTime,StepCount,NextStep,ProcessPercent,PushMessage,CreateDate,EditDate,Version from {0}task where UserSign = @UserSign and CommandType = @CommandType;";

            using (var sql = this.Open())
            {
                if (statusArray != null && statusArray.Length > 0)
                {
                    if (startTime.HasValue)
                    {
                        return sql.QueryForEnumerable<TaskschedNode>(string.Format(sqlText, this.TablePrefixName), new { @Status = statusArray.Select(o => (byte)o).ToArray(), @UserSign = usersign, @CommandType = commandType, StartTime = startTime.Value }).ToArray();
                    }

                    return sql.QueryForEnumerable<TaskschedNode>(string.Format(sqlText2, this.TablePrefixName), new { @Status = statusArray.Select(o => (byte)o).ToArray(), @UserSign = usersign, @CommandType = commandType }).ToArray();
                }

                if (startTime.HasValue)
                {
                    return sql.QueryForEnumerable<TaskschedNode>(string.Format(sqlText3, this.TablePrefixName), new { @UserSign = usersign, @CommandType = commandType, StartTime = startTime.Value }).ToArray();
                }

                return sql.QueryForEnumerable<TaskschedNode>(string.Format(sqlText4, this.TablePrefixName), new { @UserSign = usersign, @CommandType = commandType }).ToArray();
            }
        }

        /// <summary>
        /// 查询该任务所有的详情节点
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="withAllMessage">是否需要所有消息字段</param>
        /// <returns></returns>
        public virtual ExecutingNode[] GetAll(Guid taskId, bool withAllMessage = false)
        {
            if (withAllMessage)
            {
                using (var sql = this.Open())
                {
                    return sql.QueryForEnumerable<ExecutingNode>(string.Format("select RowId,TaskId,OrderNo,StepCount,StartTime,FinishTime,FailTimes,WaitTimes,StepType,StepCoordinationMode,ExecutingMessage,ResultMessage,CreateDate,EditDate,Version from {0}task_node where taskId = @TaskId;", this.TablePrefixName), new { @TaskId = taskId }).ToArray();
                }
            }

            using (var sql = this.Open())
            {
                return sql.QueryForEnumerable<ExecutingNode>(string.Format("select RowId,TaskId,OrderNo,StepCount,StartTime,FinishTime,FailTimes,WaitTimes,StepType,StepCoordinationMode,ResultMessage,CreateDate,EditDate,Version from {0}task_node where taskId = @TaskId;", this.TablePrefixName), new { @TaskId = taskId }).ToArray();
            }
        }

        /// <summary>
        /// 查询某任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public virtual TaskschedNode Get(Guid taskId)
        {
            const string sqlText = "select TaskId,UserSign,UserSignState,AttachState,CommandType,Status,StartTime,NextTime,FinishTime,StepCount,NextStep,ProcessPercent,PushMessage,CreateDate,EditDate,Version from {0}task where taskId = @TaskId;";
            using (var sql = this.Open())
            {
                return sql.QueryForObject<TaskschedNode>(string.Format(sqlText, this.TablePrefixName), new { @TaskId = taskId });
            }
        }

        /// <summary>
        /// 创建一个任务
        /// </summary>
        /// <param name="task"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public virtual int Save(TaskschedNode task, ExecutingNode[] nodes)
        {
            const string taskInsertText = "insert into {0}task(TaskId,UserSign,UserSignState,AttachState,CommandType,Status,StartTime,NextTime,FinishTime,StepCount,NextStep,ProcessPercent,PushMessage,CreateDate,EditDate,Version) values(@TaskId,@UserSign,@UserSignState,0,@CommandType,@Status,@StartTime,@NextTime,@FinishTime,@StepCount,@NextStep,@ProcessPercent,@PushMessage,@CreateDate,@EditDate,@Version);";
            const string nodeInsertText = "insert into {0}task_node(RowId,TaskId,OrderNo,StepCount,StartTime,FinishTime,FailTimes,WaitTimes,StepType,StepCoordinationMode,ResultMessage,Removed,CreateDate,EditDate,Version) values(@RowId,@TaskId,@OrderNo,@StepCount,@StartTime,@FinishTime,@FailTimes,@WaitTimes,@StepType,@StepCoordinationMode,@ResultMessage,@Removed,@CreateDate,@EditDate,@Version);";
            using (var sql = this.Open())
            {
                var rowId = 0;
                var transaction = sql as SqlClient.ITransactionExecuter;
                if (transaction != null)
                {
                    transaction.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                    try
                    {
                        rowId = (int)Convert.ChangeType(sql.Insert(string.Concat(string.Format(taskInsertText, this.TablePrefixName), this.SelectIdentity),
                                            new
                                            {
                                                @TaskId = task.TaskId,
                                                @UserSign = task.UserSign,
                                                @UserSignState = task.UserSignState,
                                                @CommandType = task.CommandType,
                                                @Status = (int)task.Status,
                                                @StartTime = task.StartTime,
                                                @NextTime = task.NextTime,
                                                @FinishTime = task.FinishTime,
                                                @StepCount = task.StepCount,
                                                @NextStep = task.NextStep,
                                                @ProcessPercent = task.ProcessPercent,
                                                @PushMessage = task.PushMessage,
                                                @CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                                @EditDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                                @Version = 1,
                                            }), TypeCode.Int32);

                        foreach (var node in nodes)
                        {
                            sql.Insert(string.Format(nodeInsertText, this.TablePrefixName),
                                new
                                {
                                    @RowId = node.RowId,
                                    @TaskId = node.TaskId,
                                    @OrderNo = node.OrderNo,
                                    @StepCount = task.StepCount,
                                    @StartTime = node.StartTime,
                                    @FinishTime = node.FinishTime,
                                    @FailTimes = node.FailTimes,
                                    @WaitTimes = node.WaitTimes,
                                    @StepType = node.StepType,
                                    @StepCoordinationMode = node.StepCoordinationMode,
                                    @Removed = false,
                                    @ResultMessage = string.IsNullOrEmpty(node.ResultMessage) ? null : node.ResultMessage,
                                    @CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    @EditDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    @Version = 1,
                                });
                        }

                        transaction.CommitTransaction();
                        return rowId;
                    }
                    catch
                    {
                        transaction.RollBackTransaction();
                        throw;
                    }
                }

                var tasksb = new StringBuilder(200);
                var writer = new Serialization.Json.ThunderWriter(task.PushMessage == null ? 0 : task.PushMessage.Length);

                tasksb.AppendFormat("insert into {0}task(TaskId,UserSign,UserSignState,AttachState,CommandType,Status,StartTime,NextTime,FinishTime,StepCount,NextStep,ProcessPercent,PushMessage,CreateDate,EditDate,Version) values('{1}','{2}','{3}',0,'{4}',0,'{5}',null,null,'{6}',1,0,'{7}','{8}','{9}',1);",
                    this.TablePrefixName,
                    task.TaskId.ToString(),
                    task.UserSign,
                    task.UserSignState,
                    task.CommandType,
                    task.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    task.StepCount,
                    this.Transferred(writer, task.PushMessage),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                foreach (var node in nodes)
                {
                    if (node.FinishTime.HasValue)
                    {
                        tasksb.AppendFormat("insert into {0}task_node(RowId,TaskId,OrderNo,StepCount,StartTime,FinishTime,FailTimes,WaitTimes,StepType,StepCoordinationMode,ResultMessage,Removed,CreateDate,EditDate,Version)values('{1}','{2}','{3}','{4}','{5}','{6}',0,0,'{7}','{8}','{9}',0,'{10}','{11}',1);",
                         this.TablePrefixName,
                         node.RowId.ToString(),
                         node.TaskId.ToString(),
                         node.OrderNo,
                         task.StepCount,
                         DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                         node.FinishTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                         node.StepType,
                         (byte)node.StepCoordinationMode,
                         this.Transferred(writer, node.ResultMessage),
                         DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                         DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        tasksb.AppendFormat("insert into {0}task_node(RowId,TaskId,OrderNo,StepCount,StartTime,FailTimes,WaitTimes,StepType,StepCoordinationMode,ResultMessage,Removed,CreateDate,EditDate,Version)values('{1}','{2}','{3}','{4}','{5}',0,0,'{6}','{7}','{8}',0,'{9}','{10}',1);",
                            this.TablePrefixName,
                            node.RowId.ToString(),
                            node.TaskId.ToString(),
                            node.OrderNo,
                            task.StepCount,
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            node.StepType,
                            (byte)node.StepCoordinationMode,
                            this.Transferred(writer, node.ResultMessage),
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }

                tasksb.Append(this.SelectIdentity);
                rowId = (int)Convert.ChangeType(sql.Insert(tasksb.ToString(), null), TypeCode.Int32);
                return rowId;
            }
        }

        /// <summary>
        /// 中断该节点下的所有任务
        /// </summary>
        /// <param name="task"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public virtual int Abort(TaskschedNode task, ExecutingNode[] nodes)
        {
            const string taskUpdateText = "update {0}task set Status = 3,EditDate = @EditDate ,AttachState = @AttachState, Version = Version + 1 ,NextTime = null where TaskId = @TaskId and Status = 0;";
            const string nodeUpdateText = "update {0}task_node set ExecutingMessage = @ExecutingMessage, EditDate = @EditDate,Version = Version + 1 where RowId = @RowId and TaskId = @TaskId and  FinishTime is null;";
            using (var sql = this.Open())
            {
                var rowId = 0;
                var transaction = sql as SqlClient.ITransactionExecuter;
                if (transaction != null)
                {
                    transaction.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                    try
                    {
                        rowId = sql.Update(string.Format(taskUpdateText, this.TablePrefixName), new { @TaskId = task.TaskId, @EditDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
                        if (rowId > 0)
                        {
                            foreach (var node in nodes)
                            {
                                sql.Update(string.Format(nodeUpdateText, this.TablePrefixName), new { @ExecutingMessage = node.ExecutingMessage, @RowId = node.RowId, @TaskId = node.TaskId, @EditDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), @AttachState = task.AttachState });
                            }
                        }
                        transaction.CommitTransaction();
                    }
                    catch
                    {
                        transaction.RollBackTransaction();
                        throw;
                    }

                    return rowId;
                }

                var tasksb = new StringBuilder(100);
                tasksb.AppendFormat("update {0}task set  Status = 3, EditDate = '{1}',AttachState = '{2}' Version = Version + 1 where TaskId = '{3}' and Status = 0;",
                    this.TablePrefixName,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    task.AttachState.ToString(),
                    task.TaskId.ToString());

                var nodesb = new StringBuilder(500);
                foreach (var node in nodes)
                {
                    nodesb.AppendFormat("update {0}task_node set ExecutingMessage = '{1}', EditDate = '{2}',Version = Version + 1 where RowId = '{3}' and TaskId = '{4}' and FinishTime is null;",
                            this.TablePrefixName,
                            node.ExecutingMessage,
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            node.RowId.ToString(),
                            node.TaskId.ToString());
                }

                rowId = sql.Update(tasksb.ToString(), null);
                if (rowId > 0)
                {
                    rowId = sql.Update(nodesb.ToString(), null);
                }

                return rowId;
            }
        }

        /// <summary>
        /// 在详情节点出现错误的时候，可能会更新节点任务的下一次开始时间
        /// </summary>
        /// <param name="task"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual int Exception(TaskschedNode task, ExecutingNode node)
        {
            const string taskUpdateText = "update {0}task set NextTime = @NextTime, EditDate = @EditDate,AttachState = @AttachState, Version = Version + 1 where TaskId = @TaskId and Status = 0;";
            const string nodeUpdateText = "update {0}task_node set FailTimes = FailTimes + 1,ExecutingMessage = @ExecutingMessage, EditDate = @EditDate,Version = Version + 1 where RowId = @RowId and TaskId = @TaskId and FinishTime is null;";
            const string errorInsertText = "insert into {0}task_node_error(RowId,CreateDate,StepType,FailException)values(@RowId,@CreateDate,@StepType,@FailException)";
            using (var sql = this.Open())
            {
                var rowId = 0;
                var transaction = sql as SqlClient.ITransactionExecuter;
                if (transaction != null)
                {
                    transaction.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                    try
                    {
                        rowId = sql.Update(string.Format(taskUpdateText, this.TablePrefixName), new { @NextTime = task.NextTime, @TaskId = task.TaskId, @EditDate = DateTime.Now, AttachState = task.AttachState });
                        if (rowId > 0)
                        {
                            rowId = sql.Update(string.Format(nodeUpdateText, this.TablePrefixName), new { @ExecutingMessage = node.ExecutingMessage, @RowId = node.RowId, @TaskId = node.TaskId, @EditDate = DateTime.Now, });
                            sql.Insert(string.Format(errorInsertText, this.TablePrefixName), new { @RowId = node.RowId, @CreateDate = DateTime.Now, @StepType = node.StepType, @FailException = node.ExecutingMessage });
                        }

                        transaction.CommitTransaction();
                    }
                    catch
                    {
                        transaction.RollBackTransaction();
                        throw;
                    }

                    return rowId;
                }

                var tasksb = new StringBuilder(100);
                tasksb.AppendFormat("update {0}task set NextTime = '{1}', EditDate = '{2}',AttachState = '{3}' Version = Version + 1 where TaskId = '{4}' and Status = 0;",
                    this.TablePrefixName,
                    (task.NextTime.HasValue ? task.NextTime.Value : DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    task.AttachState.ToString(),
                    task.TaskId.ToString());

                var nodesb = new StringBuilder(100);
                nodesb.AppendFormat("update {0}task_node set FailTimes = FailTimes + 1,ExecutingMessage = '{1}', EditDate = '{2}',Version = Version + 1 where RowId = '{3}' and TaskId = '{4}' and FinishTime is null;",
                    this.TablePrefixName,
                    this.Transferred(node.ExecutingMessage),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    node.RowId.ToString(),
                    node.TaskId.ToString());

                var errorsb = new StringBuilder(100);
                errorsb.AppendFormat("insert into {0}task_node_error(RowId,CreateDate,StepType,FailException)values('{1}','{2}','{3}','{4}');",
                    this.TablePrefixName,
                    node.RowId.ToString(),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    node.StepType,
                    this.Transferred(node.ExecutingMessage));

                rowId = sql.Update(tasksb.ToString(), null);
                if (rowId > 0)
                {
                    rowId = sql.Update(nodesb.ToString(), null);
                    sql.Insert(errorsb.ToString(), null);
                }

                return rowId;
            }
        }

        /// <summary>
        /// 在详情节点出现空消息的时候，可能会更新节点任务的下一次开始时间和详情节点的空消息次数
        /// </summary>
        /// <param name="task"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual int NextWork(TaskschedNode task, ExecutingNode node)
        {
            const string taskUpdateText = "update {0}task set NextTime = @NextTime, EditDate = @EditDate, AttachState = @AttachState, Version = Version + 1 where TaskId = @TaskId and Status = 0;";
            const string nodeUpdateText = "update {0}task_node set WaitTimes = WaitTimes + 1, ExecutingMessage = @ExecutingMessage, EditDate = @EditDate,Version = Version + 1 where RowId = @RowId and TaskId = @TaskId and FinishTime is null;";
            using (var sql = this.Open())
            {
                var rowId = 0;
                var transaction = sql as SqlClient.ITransactionExecuter;
                if (transaction != null)
                {
                    transaction.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                    try
                    {
                        rowId = sql.Update(string.Format(taskUpdateText, this.TablePrefixName), new { @NextTime = task.NextTime, @TaskId = task.TaskId, @EditDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), AttachState = task.AttachState, });
                        if (rowId > 0)
                        {
                            sql.Update(string.Format(nodeUpdateText, this.TablePrefixName), new { @RowId = node.RowId, @ExecutingMessage = node.ExecutingMessage, @TaskId = node.TaskId, @EditDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), });

                        }

                        transaction.CommitTransaction();
                    }
                    catch
                    {
                        transaction.RollBackTransaction();
                        throw;
                    }

                    return rowId;
                }

                var tasksb = new StringBuilder(100);
                tasksb.AppendFormat("update {0}task set NextTime = '{1}',EditDate = '{2}', AttachState = '{3}' Version = Version + 1 where TaskId = '{4}' and Status = 0;",
                    this.TablePrefixName,
                    (task.NextTime.HasValue ? task.NextTime.Value : DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    task.AttachState.ToString(),
                    task.TaskId.ToString());

                var nodesb = new StringBuilder(100);
                nodesb.AppendFormat("update {0}task_node set WaitTimes = WaitTimes + 1, ExecutingMessage = '{1}', EditDate = '{2}',Version = Version + 1 where RowId = '{3}' and TaskId = '{4}' and FinishTime is null;",
                    this.TablePrefixName,
                    node.ExecutingMessage,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    node.RowId.ToString(),
                    node.TaskId.ToString());

                rowId = sql.Update(tasksb.ToString(), null);
                if (rowId > 0)
                {
                    rowId = sql.Update(nodesb.ToString(), null);
                }

                return rowId;
            }
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="task"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual int Done(TaskschedNode task, ExecutingNode node)
        {
            const string taskUpdateText = "update {0}task set  NextStep = @NextStep,EditDate = @EditDate,ProcessPercent = @ProcessPercent, AttachState = @AttachState, Version = Version + 1,NextTime = null where TaskId = @TaskId and Status = 0;";
            const string nodeUpdateText = "update {0}task_node set FinishTime = @FinishTime, ResultMessage = @ResultMessage,EditDate = @EditDate,Version = Version + 1 where RowId = @RowId and TaskId = @TaskId and FinishTime is null;";
            using (var sql = this.Open())
            {
                var rowId = 0;
                var transaction = sql as SqlClient.SqlExecuter;
                if (transaction != null)
                {
                    transaction.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                    try
                    {
                        rowId = sql.Update(string.Format(taskUpdateText, this.TablePrefixName), new { @NextStep = task.NextStep, @TaskId = task.TaskId, @EditDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), @ProcessPercent = task.ProcessPercent, AttachState = task.AttachState, });
                        if (rowId > 0)
                        {
                            sql.Update(string.Format(nodeUpdateText, this.TablePrefixName), new { @FinishTime = node.FinishTime, @ResultMessage = node.ResultMessage, @RowId = node.RowId, @TaskId = node.TaskId, @EditDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), });
                        }

                        transaction.CommitTransaction();
                    }
                    catch
                    {
                        transaction.RollBackTransaction();
                        throw;
                    }

                    return rowId;
                }

                var tasksb = new StringBuilder(100);
                tasksb.AppendFormat("update {0}task set NextStep = '{1}' EditDate = '{2}', Version = Version + 1,ProcessPercent = {3},AttachState ='{4}' NextTime = null where TaskId = '{5}' and Status = 0;",
                    this.TablePrefixName,
                    task.NextStep,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    task.ProcessPercent,
                    task.AttachState.ToString(),
                    task.TaskId.ToString());

                var nodesb = new StringBuilder(100);
                nodesb.AppendFormat("update {0}task_node set FinishTime = '{1}', ResultMessage = '{2}', EditDate = '{3}',Version = Version + 1 where RowId = '{4}' and TaskId = '{5}' and FinishTime is null;",
                    this.TablePrefixName,
                    (node.FinishTime.HasValue ? node.FinishTime.Value : DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"),
                    this.Transferred(node.ResultMessage),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    node.RowId.ToString(),
                    node.TaskId.ToString());

                rowId = sql.Update(tasksb.ToString(), null);
                if (rowId > 0)
                {
                    rowId = sql.Update(nodesb.ToString(), null);
                }

                return rowId;
            }
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="task"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual int AllDone(TaskschedNode task, ExecutingNode node)
        {
            const string taskUpdateText = "update {0}task set  Status = 2,NextStep = @NextStep,FinishTime = @FinishTime,EditDate = @EditDate,ProcessPercent = @ProcessPercent, AttachState = @AttachState, Version = Version + 1, NextTime = null where TaskId = @TaskId and Status = 0;";
            const string nodeUpdateText = "update {0}task_node set FinishTime = @FinishTime, ResultMessage = @ResultMessage,EditDate = @EditDate,Version = Version + 1 where RowId = @RowId and TaskId = @TaskId and FinishTime is null;";
            const string allNodeUpdateText = "update {0}task_node set Removed = 1 where TaskId = @TaskId;";

            using (var sql = this.Open())
            {
                var rowId = 0;
                var transaction = sql as SqlClient.SqlExecuter;
                if (transaction != null)
                {
                    transaction.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                    try
                    {
                        rowId = sql.Update(string.Format(taskUpdateText, this.TablePrefixName), new { @FinishTime = task.FinishTime, @NextStep = task.NextStep, @TaskId = task.TaskId, @EditDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), @ProcessPercent = task.ProcessPercent, AttachState = task.AttachState });
                        if (rowId > 0)
                        {
                            sql.Update(string.Format(nodeUpdateText, this.TablePrefixName), new { @FinishTime = node.FinishTime, @ResultMessage = node.ResultMessage, @RowId = node.RowId, @TaskId = node.TaskId, @EditDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), });
                            sql.Update(string.Format(allNodeUpdateText, this.TablePrefixName), new { @TaskId = node.TaskId });
                        }

                        transaction.CommitTransaction();
                    }
                    catch
                    {
                        transaction.RollBackTransaction();
                        throw;
                    }

                    return rowId;
                }

                var tasksb = new StringBuilder(100);
                tasksb.AppendFormat("update {0}task set Status = 3,NextStep = '{1}',FinishTime = '{2}', EditDate = '{3}', Version = Version + 1,ProcessPercent = '{4}',AttachState = '{5}' ,NextTime = null where TaskId = '{6}' and Status = 0;",
                    this.TablePrefixName,
                    task.NextStep,
                    (task.FinishTime.HasValue ? task.FinishTime.Value : DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    task.ProcessPercent,
                    task.AttachState,
                    task.TaskId.ToString());

                var nodesb = new StringBuilder(100);
                nodesb.AppendFormat("update {0}task_node set FinishTime = '{1}', ResultMessage = '{2}', EditDate = '{3}',Version = Version + 1 where RowId = '{4}' and TaskId = '{5}' and FinishTime is null;",
                    this.TablePrefixName,
                    (node.FinishTime.HasValue ? node.FinishTime.Value : DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"),
                    this.Transferred(node.ResultMessage),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    node.RowId.ToString(),
                    node.TaskId.ToString());

                rowId = sql.Update(tasksb.ToString(), null);
                if (rowId > 0)
                {
                    rowId = sql.Update(nodesb.ToString(), null);
                    sql.Update(string.Format("update {0}task_node set Removed = 1 where TaskId = '{1}'; ", this.TablePrefixName, node.TaskId.ToString()), null);
                }

                return rowId;
            }
        }

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public virtual int Finish(TaskschedNode task)
        {
            const string sqlText = "update {0}task set  Status = 2,NextStep = @NextStep,FinishTime = @FinishTime, AttachState = @AttachState, EditDate = @EditDate,Version = Version + 1,ProcessPercent = 100  where TaskId = @TaskId and Status = 0;";
            using (var sql = this.Open())
            {
                return sql.Update(string.Format(sqlText, this.TablePrefixName),
                    new
                    {
                        @FinishTime = (task.FinishTime.HasValue ? task.FinishTime.Value : DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"),
                        @EditDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        @NextStep = task.StepCount,
                        @AttachState = task.AttachState,
                        @TaskId = task.TaskId,
                    });
            }
        }

        #endregion executing
    }
}