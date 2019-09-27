using Never.WorkFlow.Attributes;
using Never.WorkFlow.Test.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.WorkFlow.Test.Steps
{
    [WorkStep("a7390110b733", Introduce = "用户申请请假", Sumarry = "这是用户申请请假的")]
    public class 请假申请 : IWorkStep
    {
        public IWorkStepMessage Execute(IWorkContext context, IWorkStepMessage preResult)
        {
            return preResult;
        }
    }
}