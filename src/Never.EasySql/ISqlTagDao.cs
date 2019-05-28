using Never.EasySql.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// SqlTag操作接口
    /// </summary>
    public interface ISqlTagDao
    {
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <param name="tag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        int Update<Parameter>(SqlTag tag, EasySqlParameter<Parameter> parameter);

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <param name="tag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        int Delete<Parameter>(SqlTag tag, EasySqlParameter<Parameter> parameter);

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <param name="tag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        object Insert<Parameter>(SqlTag tag, EasySqlParameter<Parameter> parameter);

        /// <summary>
        /// 查询某一行
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <typeparam name="Parameter"></typeparam>
        /// <param name="tag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Result QueryForObject<Result, Parameter>(SqlTag tag, EasySqlParameter<Parameter> parameter);

        /// <summary>
        /// 查询数组
        /// </summary>
        /// <typeparam name="Result"></typeparam>
        /// <typeparam name="Parameter"></typeparam>
        /// <param name="tag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        IEnumerable<Result> QueryForEnumerable<Result, Parameter>(SqlTag tag, EasySqlParameter<Parameter> parameter);

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <typeparam name="Parameter"></typeparam>
        /// <param name="tag"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        object Call<Parameter>(SqlTag tag, EasySqlParameter<Parameter> parameter);
    }
}
