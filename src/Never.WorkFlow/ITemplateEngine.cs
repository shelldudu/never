using System;

namespace Never.WorkFlow
{
    /// <summary>
    /// 模板引擎接口
    /// </summary>
    public interface ITemplateEngine
    {
        /// <summary>
        /// 登记模板
        /// </summary>
        /// <param name="template">模板</param>
        void Register(Template template);

        /// <summary>
        /// 查询登记模板
        /// </summary>
        /// <param name="templateName">模板名字</param>
        /// <returns></returns>
        Template Select(string templateName);

        /// <summary>
        /// 兼容性事件测试
        /// </summary>
        event EventHandler<CompliantEventArgs> TestCompliant;

        /// <summary>
        /// 准备好了
        /// </summary>
        /// <returns></returns>
        void Startup();
    }
}