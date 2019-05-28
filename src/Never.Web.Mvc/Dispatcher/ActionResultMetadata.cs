using System;
using System.Reflection;

namespace Never.Web.Mvc.Dispatcher
{
    /// <summary>
    /// actionresult资源
    /// </summary>
    public struct ActionResultMetadata
    {
        /// <summary>
        ///  当前控制器名字
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// 当前控制器类型
        /// </summary>
        public Type ControllerType { get; set; }

        /// <summary>
        /// 当前Action方法
        /// </summary>
        public MethodInfo ActionMethod { get; set; }

        /// <summary>
        /// 区域名，如果支持的话
        /// </summary>
        public string AreaName { get; set; }
    }
}