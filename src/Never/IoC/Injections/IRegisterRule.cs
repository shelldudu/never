using Never.Aop;
using System;

namespace Never.IoC.Injections
{
    /// <summary>
    /// 注册规则
    /// </summary>
    public interface IRegisterRule : ICloneable
    {
        /// <summary>
        /// 构造函数参数
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="key">注册key</param>
        /// <returns></returns>
        IRegisterRule WithParameter<TService>(string key);

        /// <summary>
        /// 注册为代理服务
        /// </summary>
        /// <param name="proxyType">代理服务，该类型要自己实现被代理的信息，比如承认对象，实现相同接口等</param>
        /// <returns></returns>
        IObviousProxyRegisterRule AsProxy(Type proxyType);

        /// <summary>
        /// 注册为代理服务
        /// </summary>
        /// <typeparam name="TProxyType">代理服务，该类型要自己实现被代理的信息，比如承认对象，实现相同接口等</typeparam>
        /// <returns></returns>
        IObviousProxyRegisterRule AsProxy<TProxyType>();

        /// <summary>
        /// 注册为代理服务，该方法会自动生成一个类，实现了接口或承继了被代理
        /// </summary>
        /// <returns></returns>
        IShadowProxyRegisterRule AsProxy();

        /// <summary>
        /// 注册为代理服务，该方法会自动生成一个类，实现了接口或承继了被代理
        /// </summary>
        /// <param name="setting">配置</param>
        /// <returns></returns>
        IShadowProxyRegisterRule AsProxy(InterceptCompileSetting setting);

        /// <summary>
        /// 返回当前注册的增量缓存字符串key
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}