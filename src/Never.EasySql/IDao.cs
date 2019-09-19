using Never.EasySql.Xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 数据操作接口
    /// </summary>
    public interface IDao : IDisposable
    {
        /// <summary>
        /// 数据源一些配置
        /// </summary>
        IDataSource DataSource { get; }

        /// <summary>
        /// 开启事务后一些会话
        /// </summary>
        ISession CurrentSession { get; }

        /// <summary>
        /// Sql执行者
        /// </summary>
        IEasySqlExecuter SqlExecuter { get; }

        /// <summary>
        /// sqlTag提供者
        /// </summary>
        ISqlTagProvider SqlTagProvider { get; }

        /// <summary>
        /// 开启新事务
        /// </summary>
        ISession BeginTransaction();

        /// <summary>
        /// 开启新事务
        /// </summary>
        /// <param name="level"></param>
        ISession BeginTransaction(IsolationLevel level);

        /// <summary>
        /// 提交
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="closeConnection">关闭连接</param>
        void CommitTransaction(bool closeConnection);

        /// <summary>
        /// 回滚
        /// </summary>
        void RollBackTransaction();

        /// <summary>
        /// 回滚
        /// </summary>
        /// <param name="closeConnection">关闭连接</param>
        void RollBackTransaction(bool closeConnection);

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <param name="sqlTag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        int Update<Parameter>(SqlTag sqlTag, EasySqlParameter<Parameter> parameter);

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <param name="sqlTag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        int Delete<Parameter>(SqlTag sqlTag, EasySqlParameter<Parameter> parameter);

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <param name="sqlTag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        object Insert<Parameter>(SqlTag sqlTag, EasySqlParameter<Parameter> parameter);

        /// <summary>
        /// 查询某一行
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <typeparam name="Parameter"></typeparam>
        /// <param name="sqlTag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Result QueryForObject<Result, Parameter>(SqlTag sqlTag, EasySqlParameter<Parameter> parameter);

        /// <summary>
        /// 查询数组
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <typeparam name="Parameter"></typeparam>
        /// <param name="sqlTag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        IEnumerable<Result> QueryForEnumerable<Result, Parameter>(SqlTag sqlTag, EasySqlParameter<Parameter> parameter);

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <param name="sqlTag"></param>
        /// <param name="parameter"></param>
        /// <param name="callmode">请使用枚举的位运算如<see cref="CallMode.CommandStoredProcedure"/> | <see cref="CallMode.ExecuteScalar"/> </param>
        /// <returns></returns>
        object Call<Parameter>(SqlTag sqlTag, EasySqlParameter<Parameter> parameter, CallMode callmode);

        /// <summary>
        /// 获取格式化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlTag"></param>
        /// <param name="parameter"></param>
        /// <param name="formatText"></param>
        /// <returns></returns>
        SqlTagFormat GetSqlTagFormat<T>(SqlTag sqlTag, EasySqlParameter<T> parameter, bool formatText = false);
    }
}