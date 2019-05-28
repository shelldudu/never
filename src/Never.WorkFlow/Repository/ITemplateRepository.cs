using Never.Serialization;

namespace Never.WorkFlow.Repository
{
    /// <summary>
    /// 模板仓库
    /// </summary>
    public interface ITemplateRepository
    {
        /// <summary>
        /// 批量创建模板
        /// </summary>
        /// <param name="jsonSerializer"></param>
        /// <param name="addTemplates"></param>
        /// <param name="changeTemplates"></param>
        void SaveAndChange(IJsonSerializer jsonSerializer, Template[] addTemplates, Template[] changeTemplates);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="templateName">模板</param>
        /// <returns></returns>
        int Remove(string templateName);

        /// <summary>
        /// 查询所有模板
        /// </summary>
        /// <param name="jsonSerializer"></param>
        /// <returns></returns>
        Template[] GetAll(IJsonSerializer jsonSerializer);

        /// <summary>
        /// 查询某一模板
        /// </summary>
        /// <param name="jsonSerializer"></param>
        /// <param name="templateName"></param>
        /// <returns></returns>
        Template Get(IJsonSerializer jsonSerializer, string templateName);
    }
}