using System;

namespace Never.Startups
{
    /// <summary>
    /// 对象类型加工处理器
    /// </summary>
    public interface ITypeProcessor
    {
        /// <summary>
        /// 处理类型
        /// </summary>
        /// <param name="application">程序宿主环境配置</param>
        /// <param name="type">对象类型</param>
        void Processing(IApplicationStartup application, Type type);
    }
}