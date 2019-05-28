using Never.Commands;

namespace Never.Events
{
    /// <summary>
    /// 启动上下文
    /// </summary>
    public interface IStartupEventContext : IEventContext
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        void OnInit(ICommandContext context);
    }
}