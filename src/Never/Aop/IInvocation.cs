using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never.Aop
{
    /// <summary>
    /// 调用信息
    /// </summary>
    public interface IInvocation
    {
        /// <summary>
        /// 代理的对象
        /// </summary>
        object Proxy { get; }

        /// <summary>
        /// 代理的对象类型
        /// </summary>
        Type ProxyType { get; }

        /// <summary>
        /// 代理的对象所实现的接口
        /// </summary>
        Type[] Interfaces { get; }

        /// <summary>
        /// 代理的参数
        /// </summary>
        KeyValuePair<string, object>[] Arguments { get; }

        /// <summary>
        /// 泛型方法参数类型
        /// </summary>
        Type[] GenericArguments { get; }

        /// <summary>
        /// 代理的方法
        /// </summary>
        MethodInfo Method { get; }

        /// <summary>
        /// 该方法的特性，不会返回空对象
        /// </summary>
        Attribute[] MethodAttributes { get; }

        /// <summary>
        /// 该代理的特性，不会返回空对象
        /// </summary>
        Attribute[] ProxyAttributes { get; }

        /// <summary>
        /// 上下文
        /// </summary>
        IDictionary<string, object> Items { get; }
    }
}