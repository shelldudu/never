using System;

namespace Never.Domains
{
    /// <summary>
    /// 事务仓库
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="level">事务级别</param>
        /// <param name="callback">回调</param>
        void UsingTran(System.Data.IsolationLevel level, Action callback);
    }
}