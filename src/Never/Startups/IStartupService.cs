namespace Never.Startups
{
    /// <summary>
    /// 启动服务
    /// </summary>
    public interface IStartupService
    {
        /// <summary>
        /// 在程序宿主环境开始启动时刻，要处理的逻辑
        /// </summary>
        /// <param name="context">启动上下文</param>
        void OnStarting(StartupContext context);

        /// <summary>
        /// 排序，通常IoC的规则会排在前十，所以其他对象请在IoC后面
        /// </summary>
        int Order { get; }
    }
}