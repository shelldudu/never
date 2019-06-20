using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never.IoC
{
    /// <summary>
    /// 主动注入环境提供者
    /// </summary>
    public interface IAutoInjectingEnvironmentProvider
    {
        /// <summary>
        /// 要验证的自动注入属性
        /// </summary>
        /// <returns></returns>
        Type GetAutoInjectingAttributeType();

        /// <summary>
        /// 注入类型
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="eventArgs"></param>
        void Call(AutoInjectingGroupInfo[] groups, IContainerStartupEventArgs eventArgs);
    }
}