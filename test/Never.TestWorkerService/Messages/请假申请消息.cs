using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.WorkFlow.Test.Messages
{
    public class 请假申请消息 : IWorkStepMessage, IWorkStepMessageValidator
    {
        #region prop

        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public string Reason { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }

        public Guid TaskId { get; set; }
        public int AttachState { get; set; }
        #endregion prop

        public bool Validate()
        {
            if (this.UserName.IsNotNullOrEmpty())
            {
                return false;
            }

            if (this.Reason.IsNotNullOrEmpty())
            {
                return false;
            }

            if (this.BeginTime > this.EndTime)
            {
                return false;
            }

            if (this.EndTime < this.CreateDate)
            {
                return false;
            }

            return true;
        }

        public void Validate(IWorkStepMessageValidateContext context)
        {
            if (this.UserName.IsNullOrEmpty())
            {
                context.ParamaterRule<请假申请消息>(model => model.UserName, "申请请假用户名不能为空");
            }

            if (this.Reason.IsNullOrEmpty())
            {
                context.ParamaterRule<请假申请消息>(model => model.Reason, "申请请假理由不能为空");
            }

            if (this.BeginTime > this.EndTime)
            {
                context.ParamaterRule<请假申请消息>(model => model.Reason, "申请请假开始时间不能小于结束不能为空");
            }

            if (this.EndTime < this.CreateDate)
            {
                context.ParamaterRule<请假申请消息>(model => model.Reason, "申请请假结束时间不能小时申请时间");
            }
        }
    }
}