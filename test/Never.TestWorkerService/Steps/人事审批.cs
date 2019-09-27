using Never.WorkFlow.Attributes;
using Never.WorkFlow.Test.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.WorkFlow.Test.Steps
{
    [WorkStep("a739010fd35f", Introduce = "人事审批", Sumarry = "人事审批")]
    public class 人事审批 : IWorkStep
    {
        public IWorkStepMessage Execute(IWorkContext context, IWorkStepMessage preResult)
        {
            return new 人事审批请假意见结果() { TaskId = context.TaskId };
        }
    }
}