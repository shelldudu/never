using Never.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Commands.Recovery
{
    /// <summary>
    /// 操作者
    /// </summary>
    public struct RecoveryWorker : Security.IWorker
    {
        #region prop

        /// <summary>
        /// 操作者Id
        /// </summary>
        public long WorkerId
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// 操作者名字
        /// </summary>
        public string WorkerName
        {
            get
            {
                return "recovery";
            }
        }

        /// <summary>
        /// Id
        /// </summary>
        public int AdditionId
        {
            get; set;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid AdditionGuid
        {
            get; set;
        }

        #endregion prop
    }
}