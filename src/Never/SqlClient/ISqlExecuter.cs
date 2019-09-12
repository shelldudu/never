using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Never.SqlClient
{
    /// <summary>
    /// 据库操作接口
    /// </summary>
    public interface ISqlExecuter : IDisposable, IParameterPrefixProvider
    {
        #region connection

        /// <summary>
        /// 连接字符串
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// 数据库工厂
        /// </summary>
        DbProviderFactory ProviderFactory { get; }

        #endregion connection

        #region reader
        /// <summary>
        /// 获取DbDataReader,高速度，推荐使用，默认为Sql查询
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        IDataReader CreateReader(string sql, object @parameter);

        /// <summary>
        /// 获取DbDataReader
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        IDataReader CreateReader(string sql, CommandType commandType, object @parameter);

        /// <summary>
        /// 读取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        IEnumerable<T> QueryForEnumerable<T>(string sql, object @parameter);

        /// <summary>
        /// 读取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        IEnumerable<T> QueryForEnumerable<T>(string sql, CommandType commandType, object @parameter);

        /// <summary>
        /// 读取单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        T QueryForObject<T>(string sql, object @parameter);

        /// <summary>
        /// 读取单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        T QueryForObject<T>(string sql, CommandType commandType, object @parameter);

        #endregion reader

        #region excute

        /// <summary>
        /// 执行insert into 语句
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        object Insert(string sql, object @parameter);

        /// <summary>
        /// 执行insert into 语句
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        object Insert(string sql, CommandType commandType, object @parameter);

        /// <summary>
        /// 执行 update 语句
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        int Update(string sql, object @parameter);

        /// <summary>
        /// 执行 update 语句
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        int Update(string sql, CommandType commandType, object @parameter);

        /// <summary>
        /// 执行 delete 语句
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        int Delete(string sql, object @parameter);

        /// <summary>
        /// 执行 delete 语句
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        int Delete(string sql, CommandType commandType, object @parameter);

        /// <summary>
        /// 返回执行影响行数，默认Sql查询语句
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        int ExecuteNonQuery(string sql, object @parameter);

        /// <summary>
        /// 返回执行影响行数
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        int ExecuteNonQuery(string sql, CommandType commandType, object @parameter);

        /// <summary>
        /// 返回执行第一行第一列的值，默认Sql查询语句
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        object ExecuteScalar(string sql, object @parameter);

        /// <summary>
        /// 返回执行第一行第一列的值
        /// </summary>
        /// <param name="sql">查询字符串</param>
        /// <param name="commandType">查询命令的解释模式</param>
        /// <param name="parameter">查询参数</param>
        /// <returns></returns>
        object ExecuteScalar(string sql, CommandType commandType, object @parameter);

        #endregion excute
    }
}