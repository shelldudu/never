namespace Never
{
    /// <summary>
    /// 工作服务
    /// </summary>
    public interface IWorkService
    {
        /// <summary>
        /// 工作开始了
        /// </summary>
        void Startup();

        /// <summary>
        /// 工作停止了
        /// </summary>
        void Shutdown();
    }
}