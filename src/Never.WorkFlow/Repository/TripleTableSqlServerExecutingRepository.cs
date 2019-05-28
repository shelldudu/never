using Never.WorkFlow.Coordinations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.WorkFlow.Repository
{
    /// <summary>
    /// sql server 模式的执行任务仓库
    /// </summary>
    public abstract class TripleTableSqlServerExecutingRepository : TripleTableSqlExecutingRepository, IExecutingRepository, ITemplateRepository
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string SelectIdentity => "select SCOPE_IDENTITY();";

        #region search

        /// <summary>
        /// 搜索条件
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public PagedData<TaskschedNode> GetPage(TaskschedNodeSearch search)
        {
            var where = new StringBuilder(200);
            var table = new Hashtable();

            if (!string.IsNullOrEmpty(search.CommandType))
            {
                where.Append("CommandType  = @CommandType ");
                table["CommandType"] = search.CommandType;
            }

            if (!string.IsNullOrEmpty(search.UserSign))
            {
                if (table.Count > 0)
                {
                    where.Append("and UserSign  = @UserSign ");
                }
                else
                {
                    where.Append("UserSign  = @UserSign ");
                }

                table["UserSign"] = search.UserSign;
            }

            if (search.StatusArray != null && search.StatusArray.Length > 0)
            {
                if (table.Count > 0)
                {
                    where.Append("and Status  in (@Status) ");
                }
                else
                {
                    where.Append("Status in (@Status) ");
                }

                table["Status"] = search.StatusArray.Select(o => (byte)o).ToArray();
            }

            if (table.Count > 0)
            {
                where.Append("and StartTime >= @StartTime ");
            }
            else
            {
                where.Append("StartTime >= @StartTime ");
            }

            table["StartTime"] = DateTime.Now;

            if (where.Length > 0)
            {
                where.Insert(0, "where ");
            }

            table["StartIndex"] = search.StartIndex;
            table["EndIndex"] = search.EndIndex;

            using (var sql = this.Open())
            {
                var totalCount = sql.QueryForObject<int>(string.Format("select count(0) from {0}task {1}", this.TablePrefixName, where.ToString()), table);
                if (totalCount <= 0)
                {
                    return new PagedData<TaskschedNode>(search.PageNow, search.PageSize, 0, new TaskschedNode[0]);
                }

                var records = sql.QueryForEnumerable<TaskschedNode>(string.Format("select * from (select r.*,ROW_NUMBER() OVER (order by id asc) as rank from {0}task as r {1}) as temp where temp.rank >= @StartIndex and temp.rank < @EndIndex;", this.TablePrefixName, where.ToString()), table);
                return new PagedData<TaskschedNode>(search.PageNow, search.PageSize, totalCount, records);
            }
        }

        /// <summary>
        /// 搜索条件
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public TaskschedNode[] GetList(TaskschedNodeSearch search)
        {
            var where = new StringBuilder(200);
            var table = new Hashtable();

            if (!string.IsNullOrEmpty(search.CommandType))
            {
                where.Append("CommandType  = @CommandType ");
                table["CommandType"] = search.CommandType;
            }

            if (!string.IsNullOrEmpty(search.UserSign))
            {
                if (table.Count > 0)
                {
                    where.Append("and UserSign  = @UserSign ");
                }
                else
                {
                    where.Append("UserSign  = @UserSign ");
                }

                table["UserSign"] = search.UserSign;
            }

            if (search.StatusArray != null && search.StatusArray.Length > 0)
            {
                if (table.Count > 0)
                {
                    where.Append("and Status  in (@Status) ");
                }
                else
                {
                    where.Append("Status in (@Status) ");
                }

                table["Status"] = search.StatusArray.Select(o => (byte)o).ToArray();
            }

            if (table.Count > 0)
            {
                where.Append("and StartTime >= @StartTime ");
            }
            else
            {
                where.Append("StartTime >= @StartTime ");
            }

            table["StartTime"] = DateTime.Now;

            if (where.Length > 0)
            {
                where.Insert(0, "where ");
            }

            table["StartIndex"] = search.StartIndex;
            table["EndIndex"] = search.EndIndex;

            using (var sql = this.Open())
            {
                return sql.QueryForEnumerable<TaskschedNode>(string.Format("select * from (select r.*,ROW_NUMBER() OVER (order by id asc) as rank from {0}task as r {1}) as temp where temp.rank >= @StartIndex and temp.rank < @EndIndex;", this.TablePrefixName, where.ToString()), table).ToArray();
            }
        }

        /// <summary>
        /// 搜索条件
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public TaskschedNode[] GetAll(TaskschedNodeSearch search)
        {
            var where = new StringBuilder(200);
            var table = new Hashtable();

            if (!string.IsNullOrEmpty(search.CommandType))
            {
                where.Append("CommandType  = @CommandType ");
                table["CommandType"] = search.CommandType;
            }

            if (!string.IsNullOrEmpty(search.UserSign))
            {
                if (table.Count > 0)
                {
                    where.Append("and UserSign  = @UserSign ");
                }
                else
                {
                    where.Append("UserSign  = @UserSign ");
                }

                table["UserSign"] = search.UserSign;
            }

            if (search.StatusArray != null && search.StatusArray.Length > 0)
            {
                if (table.Count > 0)
                {
                    where.Append("and Status  in (@Status) ");
                }
                else
                {
                    where.Append("Status in (@Status) ");
                }

                table["Status"] = search.StatusArray.Select(o => (byte)o).ToArray();
            }

            if (table.Count > 0)
            {
                where.Append("and StartTime >= @StartTime ");
            }
            else
            {
                where.Append("StartTime >= @StartTime ");
            }

            table["StartTime"] = DateTime.Now;

            if (where.Length > 0)
            {
                where.Insert(0, "where ");
            }

            using (var sql = this.Open())
            {
                return sql.QueryForEnumerable<TaskschedNode>(string.Format("select r.* from {0}task as r {1};", this.TablePrefixName, where.ToString()), table).ToArray();
            }
        }

        #endregion search
    }
}