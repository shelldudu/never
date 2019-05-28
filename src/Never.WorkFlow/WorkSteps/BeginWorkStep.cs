using Never.WorkFlow.Attributes;

namespace Never.WorkFlow.WorkSteps
{
    /// <summary>
    /// 工作的第一步
    /// </summary>
    [WorkStep("aa1b00b7cf4e", Introduce = "开始工作流的第一步，通常是测试环境", Sumarry = "工作流第一步")]
    public sealed class BeginWorkStep : IWorkStep
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