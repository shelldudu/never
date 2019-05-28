using Never.Commands;
using System;

namespace Never.CommandStreams
{
    /// <summary>
    /// 带操作属性的命令
    /// </summary>
    public class DefaultOperateCommand : IOperateCommand
    {
        #region prop

        /// <summary>
        /// 命令所在的运行域
        /// </summary>
        public string AppDomain { get; set; }

        /// <summary>
        /// Command
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// 操作命令
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 操作者
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        public string CommandType { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        public string CommandTypeFullName { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string AggregateId { get; set; }

        /// <summary>
        /// 唯一标识的Type
        /// </summary>
        public Type AggregateIdType { get; set; }

        /// <summary>
        /// 当前的HashCode
        /// </summary>
        public int HashCode { get; set; }

        /// <summary>
        /// 当前环境的自增Id
        /// </summary>
        public long Increment { get; set; }

        #endregion prop

        #region incerement

        /// <summary>
        /// 自增数值
        /// </summary>
        private static long increment = 0;

        /// <summary>
        /// 获取自增数值
        /// </summary>
        public static long NextIncrement
        {
            get
            {
                return System.Threading.Interlocked.Increment(ref increment);
            }
        }

        #endregion incerement
    }
}