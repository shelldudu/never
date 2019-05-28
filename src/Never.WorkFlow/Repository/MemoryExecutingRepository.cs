using Never.Serialization;
using Never.WorkFlow.Coordinations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Never.WorkFlow.Repository
{
    /// <summary>
    /// 内存模式的执行任务仓库
    /// </summary>
    public class MemoryExecutingRepository : IExecutingRepository, ITemplateRepository
    {
        #region field

        private readonly Dictionary<Guid, TaskschedNode> task = null;
        private readonly Dictionary<Guid, ExecutingNode> executing = null;
        private readonly Dictionary<string, Template> template = null;
        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        public MemoryExecutingRepository()
        {
            this.task = new Dictionary<Guid, TaskschedNode>(100);
            this.executing = new Dictionary<Guid, ExecutingNode>(500);
            this.template = new Dictionary<string, Template>(20);
        }

        #endregion ctor

        #region executing

        /// <summary>
        ///
        /// </summary>
        /// <param name="task"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public int Abort(TaskschedNode task, ExecutingNode[] nodes)
        {
            return 1;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="statusArray"></param>
        /// <returns></returns>
        public TaskschedNode[] GetAll(TaskschedStatus[] statusArray)
        {
            return this.task.Values.Where(o => statusArray.Contains(o.Status)).ToArray();
        }

        /// <summary>
        /// 查询所有的任务
        /// </summary>
        /// <param name="statusArray"></param>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        public TaskschedNode[] GetAll(TaskschedStatus[] statusArray, string commandType)
        {
            return this.task.Values.Where(o => statusArray.Contains(o.Status) && o.CommandType.IsEquals(commandType)).ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="withAllMessage">是否需要所有消息字段</param>
        /// <returns></returns>
        public ExecutingNode[] GetAll(Guid taskId, bool withAllMessage = false)
        {
            return this.executing.Values.Where(o => o.TaskId == taskId).ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="usersign"></param>
        /// <param name="statusArray"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public TaskschedNode[] GetAll(string usersign, TaskschedStatus[] statusArray, DateTime? startTime = null)
        {
            if (statusArray != null && statusArray.Length > 0)
            {
                return this.task.Values.Where(o => statusArray.Contains(o.Status) && o.UserSign.IsEquals(usersign) && o.StartTime > startTime).ToArray();
            }

            return this.task.Values.Where(o => o.UserSign.IsEquals(usersign) && o.StartTime > startTime).ToArray();
        }

        /// <summary>
        /// 查询某标识下的所有的任务
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="usersign"></param>
        /// <param name="statusArray"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public TaskschedNode[] GetAll(string usersign, string commandType, TaskschedStatus[] statusArray, DateTime? startTime = null)
        {
            if (statusArray != null && statusArray.Length > 0)
            {
                return this.task.Values.Where(o => statusArray.Contains(o.Status) && o.UserSign.IsEquals(usersign) && o.CommandType.IsEquals(commandType) && o.StartTime > startTime).ToArray();
            }

            return this.task.Values.Where(o => o.UserSign.IsEquals(usersign) && o.CommandType.IsEquals(commandType) && o.StartTime > startTime).ToArray();
        }

        /// <summary>
        /// 查询用户标识的所有任务
        /// </summary>
        /// <param name="statusArray">状态</param>
        /// <param name="paged">分页查询</param>
        /// <returns></returns>
        public virtual TaskschedNode[] GetList(PagedSearch paged, TaskschedStatus[] statusArray)
        {
            return this.GetAll(statusArray);
        }

        /// <summary>
        /// 查询用户标识的所有任务
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="statusArray">状态</param>
        /// <param name="paged">分页查询</param>
        /// <returns></returns>
        public virtual TaskschedNode[] GetList(PagedSearch paged, TaskschedStatus[] statusArray, string commandType)
        {
            return this.GetAll(statusArray, commandType);
        }

        /// <summary>
        /// 查询用户标识的所有任务
        /// </summary>
        /// <param name="userSign">用户标识</param>
        /// <param name="statusArray">状态</param>
        /// <param name="paged">分页查询</param>
        /// <returns></returns>
        public virtual TaskschedNode[] GetList(PagedSearch paged, string userSign, TaskschedStatus[] statusArray)
        {
            return this.GetAll(userSign, statusArray);
        }

        /// <summary>
        /// 查询用户标识的所有任务
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="userSign">用户标识</param>
        /// <param name="statusArray">状态</param>
        /// <param name="paged">分页查询</param>
        /// <returns></returns>
        public virtual TaskschedNode[] GetList(PagedSearch paged, string userSign, string commandType, TaskschedStatus[] statusArray)
        {
            return this.GetAll(userSign, commandType, statusArray);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="task"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public int AllDone(TaskschedNode task, ExecutingNode node)
        {
            return 1;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="task"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public int Save(TaskschedNode task, ExecutingNode[] nodes)
        {
            this.task[task.TaskId] = task;
            foreach (var node in nodes)
            {
                this.executing[node.RowId] = node;
            }

            return 1;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="task"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public int Done(TaskschedNode task, ExecutingNode node)
        {
            return 1;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="task"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public int Exception(TaskschedNode task, ExecutingNode node)
        {
            return 1;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public int Finish(TaskschedNode task)
        {
            return 1;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="task"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public int NextWork(TaskschedNode task, ExecutingNode node)
        {
            return 1;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public TaskschedNode Get(Guid taskId)
        {
            return this.task.ContainsKey(taskId) ? this.task[taskId] : null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="rowId"></param>
        /// <param name="withAllMessage">是否需要所有消息字段</param>
        /// <returns></returns>
        public ExecutingNode Query(Guid taskId, Guid rowId, bool withAllMessage = false)
        {
            return this.executing.Values.Where(o => o.TaskId == taskId && o.RowId == rowId).FirstOrDefault();
        }

        #endregion executing

        #region template

        /// <summary>
        ///
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public int Change(Template template)
        {
            if (!this.template.ContainsKey(template.Name))
            {
                return 0;
            }

            this.template[template.Name] = template;
            return 1;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public int Create(Template template)
        {
            this.template[template.Name] = template;
            return 1;
        }

        /// <summary>
        /// 批量创建模板
        /// </summary>
        /// <param name="jsonSerializer"></param>
        /// <param name="addTemplates"></param>
        /// <param name="changeTemplates"></param>
        public void SaveAndChange(IJsonSerializer jsonSerializer, Template[] addTemplates, Template[] changeTemplates)
        {
            foreach (var template in addTemplates)
            {
                this.Create(template);
            }

            foreach (var template in changeTemplates)
            {
                this.Change(template);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="jsonSerializer"></param>
        /// <returns></returns>
        public Template[] GetAll(IJsonSerializer jsonSerializer)
        {
            return this.template.Values.ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="jsonSerializer"></param>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public Template Get(IJsonSerializer jsonSerializer, string templateName)
        {
            if (!this.template.ContainsKey(templateName))
            {
                return null;
            }

            return this.template[templateName];
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public int Remove(string templateName)
        {
            if (!this.template.ContainsKey(templateName))
            {
                return 0;
            }

            this.template.Remove(templateName);
            return 1;
        }

        #endregion template
    }
}