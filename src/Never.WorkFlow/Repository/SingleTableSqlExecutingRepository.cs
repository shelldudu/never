using Never;
using Never.Mappers;
using Never.SqlClient;
using Never.WorkFlow.Coordinations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.WorkFlow.Repository
{
    /// <summary>
    /// 单一表实现的仓库【不保存异常节点信息】
    /// </summary>
    public abstract class SingleTableSqlExecutingRepository : SqlExecutingRepository, IExecutingRepository, ITemplateRepository
    {
        #region executing

        private class MyExecutingNode : ExecutingNode
        {
            public string CoordinationMode { get; set; }
        }

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
            const string sqlText = "select TaskId,UserSign,UserSignState,AttachState,CommandType,Status,StartTime,NextTime,FinishTime,StepCount,NextStep,ProcessPercent,PushMessage,CreateDate,EditDate,Version from {0}task where Status in (@Status) and CommandType = @CommandType and StartTime >= @StartTime;";
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
            var target = default(MyExecutingNode);
            if (withAllMessage)
            {
                using (var sql = this.Open())
                {
                    target = sql.QueryForObject<MyExecutingNode>(string.Format("select TaskId,StepCount,StartTime,FinishTime,FailTimes,WaitTimes,StepType,CoordinationMode,ExecutingMessage,ResultMessage,CreateDate,EditDate,Version from {0}task where taskId = @TaskId;", this.TablePrefixName), new { @TaskId = taskId });
                }
            }
            else
            {
                using (var sql = this.Open())
                {
                    target = sql.QueryForObject<MyExecutingNode>(string.Format("select TaskId,StepCount,StartTime,FinishTime,FailTimes,WaitTimes,StepType,CoordinationMode,ResultMessage,CreateDate,EditDate,Version from {0}task where taskId = @TaskId;", this.TablePrefixName), new { @TaskId = taskId });
                }
            }

            if (target == null)
            {
                return new ExecutingNode[0];
            }

            if (target.StepCount <= 0 || target.StepType.IsNullOrWhiteSpace())
            {
                return new ExecutingNode[0];
            }

            var list = new List<ExecutingNode>(target.StepCount);
            var steps = target.StepType.Split(',');
            var modes = target.CoordinationMode.Split(',');
            for (int i = 0; i < target.StepCount && i < steps.Length && i < modes.Length; i++)
            {
                var node = EasyMapper.Map(target, new ExecutingNode());
                node.StepCoordinationMode = modes[i].AsEnum<CoordinationMode>();
                node.StepType = steps[i];
                node.OrderNo = (i + 1);
                list.Add(node);
            }

            return list.ToArray();
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
            const string taskInsertText = "insert into {0}task(TaskId,UserSign,UserSignState,AttachState,CommandType,Status,StartTime,NextTime,FinishTime,StepCount,NextStep,ProcessPercent,PushMessage,CreateDate,EditDate,Version,FailTimes,WaitTimes,StepType,CoordinationMode,ExecutingMessage,ResultMessage) values(@TaskId,@UserSign,@UserSignState,0,@CommandType,@Status,@StartTime,@NextTime,@FinishTime,@StepCount,@NextStep,@ProcessPercent,@PushMessage,@CreateDate,@EditDate,@Version,@FailTimes,@WaitTimes,@StepType,@CoordinationMode,@ExecutingMessage,@ResultMessage);";
            using (var sql = this.Open())
            {
                var para = new
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
                    @CreateDate = DateTime.Now,
                    @EditDate = DateTime.Now,
                    @Version = 1,
                    @FailTimes = 0,
                    @WaitTimes = 0,
                    @StepType = string.Join(",", nodes.Select(ta => ta.StepType)),
                    @CoordinationMode = string.Join(",", nodes.Select(ta => (int)ta.StepCoordinationMode)),
                    @ExecutingMessage = DBNull.Value,
                    ResultMessage = DBNull.Value,
                };

                var rowId = sql.Insert(string.Concat(string.Format(taskInsertText, this.TablePrefixName), this.SelectIdentity), para);
                return (int)Convert.ChangeType(rowId, TypeCode.Int32);
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
            using (var sql = this.Open())
            {
                var para = new
                {
                    @TaskId = task.TaskId,
                    @EditDate = DateTime.Now
                };

                var rowId = sql.Update(string.Format(taskUpdateText, this.TablePrefixName), para);
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
            const string taskUpdateText = "update {0}task set NextTime = @NextTime, EditDate = @EditDate,AttachState = @AttachState,ExecutingMessage = @ExecutingMessage, Version = Version + 1 where TaskId = @TaskId and Status = 0;";
            using (var sql = this.Open())
            {
                var para = new
                {
                    @NextTime = task.NextTime,
                    @TaskId = task.TaskId,
                    @EditDate = DateTime.Now,
                    @ExecutingMessage = node.ExecutingMessage,
                    @AttachState = task.AttachState
                };

                var rowId = sql.Update(string.Format(taskUpdateText, this.TablePrefixName), para);
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
            const string taskUpdateText = "update {0}task set NextTime = @NextTime, EditDate = @EditDate, AttachState = @AttachState, ExecutingMessage = @ExecutingMessage, Version = Version + 1 where TaskId = @TaskId and Status = 0;";
            using (var sql = this.Open())
            {
                var para = new
                {
                    @NextTime = task.NextTime,
                    @TaskId = task.TaskId,
                    @EditDate = DateTime.Now,
                    @ExecutingMessage = node.ExecutingMessage,
                    @AttachState = task.AttachState
                };

                var rowId = sql.Update(string.Format(taskUpdateText, this.TablePrefixName), para);
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
            const string taskUpdateText = "update {0}task set  NextStep = @NextStep,EditDate = @EditDate,ProcessPercent = @ProcessPercent, AttachState = @AttachState, ResultMessage = @ResultMessage, Version = Version + 1,NextTime = null where TaskId = @TaskId and Status = 0;";
            using (var sql = this.Open())
            {
                var para = new
                {
                    @NextStep = task.NextStep,
                    @TaskId = task.TaskId,
                    @EditDate = DateTime.Now,
                    @ProcessPercent = task.ProcessPercent,
                    @AttachState = task.AttachState,
                    @ResultMessage = node.ResultMessage,
                };

                var rowId = sql.Update(string.Format(taskUpdateText, this.TablePrefixName), para);
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
            const string taskUpdateText = "update {0}task set  Status = 2,NextStep = @NextStep,FinishTime = @FinishTime,EditDate = @EditDate,ProcessPercent = @ProcessPercent, AttachState = @AttachState,FailTimes = 0, WaitTimes = 0, StepType = null, CoordinationMode = null, ExecutingMessage = null,ResultMessage = @ResultMessage, Version = Version + 1, NextTime = null where TaskId = @TaskId and Status = 0;";
            using (var sql = this.Open())
            {
                var para = new
                {
                    @FinishTime = task.FinishTime,
                    @NextStep = task.NextStep,
                    @TaskId = task.TaskId,
                    @EditDate = DateTime.Now,
                    @ProcessPercent = task.ProcessPercent,
                    @ResultMessage = node.ResultMessage,
                    AttachState = task.AttachState
                };

                var rowId = sql.Update(string.Format(taskUpdateText, this.TablePrefixName), para);
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
            const string sqlText = "update {0}task set  Status = 2,NextStep = @NextStep,FinishTime = @FinishTime, AttachState = @AttachState, FailTimes = 0, WaitTimes = 0, ExecutingMessage = null,ResultMessage = null, EditDate = @EditDate,Version = Version + 1,ProcessPercent = 100  where TaskId = @TaskId and Status = 0;";
            using (var sql = this.Open())
            {
                return sql.Update(string.Format(sqlText, this.TablePrefixName),
                    new
                    {
                        @FinishTime = (task.FinishTime.HasValue ? task.FinishTime.Value : DateTime.Now),
                        @EditDate = DateTime.Now,
                        @NextStep = task.StepCount,
                        @AttachState = task.AttachState,
                        @TaskId = task.TaskId,
                    });
            }
        }

        #endregion executing
    }
}
