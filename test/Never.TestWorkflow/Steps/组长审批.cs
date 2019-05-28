using Never.WorkFlow.Attributes;
using Never.WorkFlow.Test.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.WorkFlow.Test.Steps
{
    [WorkStep("a739010ffe40", Introduce = "组长审批", Sumarry = "组长审批")]
    public class 组长审批 : IWorkStep
    {
        public IWorkStepMessage Execute(IWorkContext context, IWorkStepMessage preResult)
        {
            return new 组长审批意见结果() { TaskId = context.TaskId };
        }
    }
}