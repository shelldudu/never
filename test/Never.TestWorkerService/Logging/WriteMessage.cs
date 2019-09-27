using Never.Messages;
using Never.WorkFlow.Test.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.WorkFlow.Test.Logging
{
    public class WriteMessage : IMessageSubscriber<请假申请消息>, IMessageSubscriber<请假成功短信通知>, IMessageSubscriber<人事审批请假意见结果>, IMessageSubscriber<主管审批意见结果>, IMessageSubscriber<组长审批意见结果>
    {
        public void Execute(IMessageContext context, 请假申请消息 e)
        {
            Console.WriteLine(Never.Serialization.SerializeEnvironment.JsonSerializer.Serialize(e));
        }

        public void Execute(IMessageContext context, 请假成功短信通知 e)
        {
            Console.WriteLine(Never.Serialization.SerializeEnvironment.JsonSerializer.Serialize(e));
        }

        public void Execute(IMessageContext context, 人事审批请假意见结果 e)
        {
            Console.WriteLine(Never.Serialization.SerializeEnvironment.JsonSerializer.Serialize(e));
        }

        public void Execute(IMessageContext context, 主管审批意见结果 e)
        {
            Console.WriteLine(Never.Serialization.SerializeEnvironment.JsonSerializer.Serialize(e));
        }

        public void Execute(IMessageContext context, 组长审批意见结果 e)
        {
            Console.WriteLine(Never.Serialization.SerializeEnvironment.JsonSerializer.Serialize(e));
        }
    }
}