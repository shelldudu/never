using Never.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Commands.Recovery
{
    /// <summary>
    /// 恢复事件类型
    /// </summary>
    public class RecoveryEventModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public Guid UniqueId { get; set; }

        /// <summary>
        /// 事件处理者
        /// </summary>
        public Type EventHandlerType { get; set; }

        /// <summary>
        /// 事件
        /// </summary>
        public IEvent @Event { get; set; }
    }
}