using Never.WorkFlow.Attributes;
using Never.WorkFlow.Test.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.WorkFlow.Test.Steps
{
    [WorkStep("a73901108f30", Introduce = "人事主管审批审批", Sumarry = "主管审批")]
    public class 主管审批 : IWorkStep
    {
        public IWorkStepMessage Execute(IWorkContext context, IWorkStepMessage preResult)
        {
            // return context.CreateWatiingMessage();
            return new 主管审批意见结果() { TaskId = context.TaskId };
        }
    }
}