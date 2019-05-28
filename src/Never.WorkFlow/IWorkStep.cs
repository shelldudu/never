namespace Never.WorkFlow
{
    /// <summary>
    /// 每一个工作步骤
    /// </summary>
    public interface IWorkStep
    {
        /// <summary>
        /// 执行工作
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="preResult">上一个工作执行的结果</param>
        /// <returns></returns>
        IWorkStepMessage Execute(IWorkContext context, IWorkStepMessage preResult);
    }
}