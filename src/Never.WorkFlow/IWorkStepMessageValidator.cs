namespace Never.WorkFlow
{
    /// <summary>
    /// 消息验证
    /// </summary>
    public interface IWorkStepMessageValidator
    {
        /// <summary>
        /// 验证消息
        /// </summary>
        /// <param name="context">消息验证上下文</param>
        void Validate(IWorkStepMessageValidateContext context);
    }
}