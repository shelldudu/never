using Never.WorkFlow.Attributes;

namespace Never.WorkFlow.WorkSteps
{
    /// <summary>
    /// 工作的最后一步
    /// </summary>
    [WorkStep("aa1b00b7d77f", Introduce = "开始最后一步表示结束状态", Sumarry = "工作流最后一步")]
    public sealed class EndWorkStep : IWorkStep
    {
        /// <summary>
        /// 执行工作
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="preResult">上一个工作执行的结果</param>
        /// <returns></returns>
        public IWorkStepMessage Execute(IWorkContext context, IWorkStepMessage preResult)
        {
            return preResult;
        }
    }
}