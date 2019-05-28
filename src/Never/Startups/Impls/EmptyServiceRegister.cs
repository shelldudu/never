using Never.IoC;
using System;

namespace Never.Startups.Impls
{
    /// <summary>
    /// 服务注册者
    /// </summary>
    public sealed class EmptyServiceRegister : Never.IoC.IServiceRegister
    {
        #region field

        /// <summary>
        /// 空对象
        /// </summary>
        public static EmptyServiceRegister Only
        {
            get
            {
                if (Singleton<EmptyServiceRegister>.Instance == null)
                    Singleton<EmptyServiceRegister>.Instance = new EmptyServiceRegister();

                return Singleton<EmptyServiceRegister>.Instance;
            }
        }

        #endregion field

        #region ctor

        /// <summary>
        /// Prevents a default instance of the <see cref="EmptyServiceRegister"/> class from being created.
        /// </summary>
        private EmptyServiceRegister()
        {
        }

        #endregion ctor

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <typeparam name="TImplementation">继承TSservice对象的具体对象</typeparam>
        /// <typeparam name="TService">服务类型</typeparam>
        void IServiceRegister.RegisterType<TImplementation, TService>()
        {
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <typeparam name="TImplementation">继承TSservice对象的具体对象</typeparam>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="lifeStyle">生命周期</param>
        void IoC.IServiceRegister.RegisterType<TImplementation, TService>(string key, IoC.ComponentLifeStyle lifeStyle)
        {
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <param name="implementationType">继承TSservice对象的具体对象</param>
        /// <param name="serviceType">服务类型</param>
        void IServiceRegister.RegisterType(Type implementationType, Type serviceType)
        {
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <param name="implementationType">继承TSservice对象的具体对象</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="key">key</param>
        /// <param name="lifeStyle">生命周期</param>
        void IoC.IServiceRegister.RegisterType(Type implementationType, Type serviceType, string key, IoC.ComponentLifeStyle lifeStyle)
        {
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">服务对象</param>
        void IServiceRegister.RegisterInstance<TService>(TService instance)
        {
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">服务对象</param>
        /// <param name="key">key</param>
        void IoC.IServiceRegister.RegisterInstance<TService>(TService instance, string key)
        {
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <param name="instance">服务对象</param>
        /// <param name="serviceType">服务类型</param>
        void IServiceRegister.RegisterInstance(object instance, Type serviceType)
        {
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <param name="instance">服务对象</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="key">key</param>
        void IoC.IServiceRegister.RegisterInstance(object instance, Type serviceType, string key)
        {
        }

        void IServiceRegister.RegisterObject(Type serviceType)
        {
        }

        void IServiceRegister.RegisterObject<TService>()
        {
        }

        /// <summary>
        /// 是否注册了组件
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns></returns>
        bool IoC.IServiceRegister.Contain<TService>()
        {
            return false;
        }

        /// <summary>
        /// 是否注册了组件
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        bool IoC.IServiceRegister.Contain(Type serviceType)
        {
            return false;
        }
    }
}