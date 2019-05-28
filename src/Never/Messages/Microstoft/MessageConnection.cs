#if NET461

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Messaging;
using System.Text;

namespace Never.Messages.Microstoft
{
    /// <summary>
    /// MSMQ消息链接对象
    /// </summary>
    public class MessageConnection : DefaultMessageConnection, Never.Messages.IMessageConnection
    {
        #region const

        /// <summary>
        /// 格式为[OS:{1}\Private$\{2}]
        /// </summary>
        public const string MSMQ_OS_Route = @"FormatName:DIRECT=OS:{0}\Private$\{1}";

        /// <summary>
        /// 格式为[TCP:{1}\Private$\{2}]
        /// </summary>
        public const string MSMQ_TCP_Route = @"FormatName:DIRECT=TCP:{0}\Private$\{1}";

        #endregion const
    }
}

#endif