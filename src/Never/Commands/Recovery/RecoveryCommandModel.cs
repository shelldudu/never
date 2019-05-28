using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Commands.Recovery
{
    /// <summary>
    /// 恢复命令类型
    /// </summary>
    public class RecoveryCommandModel
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
        /// 命令
        /// </summary>
        public ICommand Command { get; set; }
    }
}