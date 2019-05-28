namespace Never.WorkFlow
{
    /// <summary>
    /// 每一步清理一下环境
    /// </summary>
    public interface IWorkContextCleaner
    {
        /// <summary>
        /// 每一步完成清理
        /// </summary>
        void StepClear();
    }
}