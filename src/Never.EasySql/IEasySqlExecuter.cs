using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 据库操作接口
    /// </summary>
    /// <seealso cref="Never.SqlClient.ISqlExecuter" />
    /// <seealso cref="Never.SqlClient.ITransactionExecuter" />
    /// <seealso cref="Never.SqlClient.IParameterPrefixProvider" />
    /// <seealso cref="Never.EasySql.IEasySqlTransactionExecuter" />
    public interface IEasySqlExecuter : Never.SqlClient.ISqlExecuter, Never.SqlClient.ITransactionExecuter, Never.SqlClient.IParameterPrefixProvider, IEasySqlTransactionExecuter
    {
        /// <summary>
        /// 读取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">执行字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        IEnumerable<T> QueryForEnumerable<T>(string sql, CommandType commandType, KeyValuePair<string, object>[] @parameter);

        /// <summary>
        /// 读取单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">执行字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        T QueryForObject<T>(string sql, CommandType commandType, KeyValuePair<string, object>[] @parameter);

        /// <summary>
        /// 执行insert into 语句
        /// </summary>
        /// <param name="sql">执行字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        object Insert(string sql, CommandType commandType, KeyValuePair<string, object>[] @parameter);

        /// <summary>
        /// 执行 update 语句
        /// </summary>
        /// <param name="sql">执行字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        int Update(string sql, CommandType commandType, KeyValuePair<string, object>[] @parameter);

        /// <summary>
        /// 执行 delete 语句
        /// </summary>
        /// <param name="sql">执行字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        int Delete(string sql, CommandType commandType, KeyValuePair<string, object>[] @parameter);

        /// <summary>
        /// 返回执行影响行数
        /// </summary>
        /// <param name="sql">执行字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        int ExecuteNonQuery(string sql, CommandType commandType, KeyValuePair<string, object>[] @parameter);

        /// <summary>
        /// 执行 ExecuteScalar 语句
        /// </summary>
        /// <param name="sql">执行字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        object ExecuteScalar(string sql, CommandType commandType, KeyValuePair<string, object>[] @parameter);
    }
}