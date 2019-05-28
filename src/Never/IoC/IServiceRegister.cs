using System;

namespace Never.IoC
{
    /// <summary>
    /// 服务注册器
    /// </summary>
    public interface IServiceRegister
    {
        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <typeparam name="TImplementation">继承TSservice对象的具体对象</typeparam>
        /// <typeparam name="TService">服务类型</typeparam>
        void RegisterType<TImplementation, TService>();

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <typeparam name="TImplementation">继承TSservice对象的具体对象</typeparam>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="lifeStyle">生命周期</param>
        void RegisterType<TImplementation, TService>(string key, ComponentLifeStyle lifeStyle);

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <param name="implementationType">继承TSservice对象的具体对象</param>
        /// <param name="serviceType">服务类型</param>
        void RegisterType(Type implementationType, Type serviceType);

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <param name="implementationType">继承TSservice对象的具体对象</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="key">key</param>
        /// <param name="lifeStyle">生命周期</param>
        void RegisterType(Type implementationType, Type serviceType, string key, ComponentLifeStyle lifeStyle);

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">服务对象</param>
        void RegisterInstance<TService>(TService instance);

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">服务对象</param>
        /// <param name="key">key</param>
        void RegisterInstance<TService>(TService instance, string key);

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <param name="instance">服务对象</param>
        /// <param name="serviceType">服务类型</param>
        void RegisterInstance(object instance, Type serviceType);

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <param name="instance">服务对象</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="key">key</param>
        void RegisterInstance(object instance, Type serviceType, string key);

        /// <summary>
        /// 注册对象
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        void RegisterObject(Type serviceType);

        /// <summary>
        /// 注册对象
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns></returns>
        void RegisterObject<TService>();

        /// <summary>
        /// 是否注册了组件
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns></returns>
        bool Contain<TService>();

        /// <summary>
        /// 是否注册了组件
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        bool Contain(Type serviceType);
    }
}