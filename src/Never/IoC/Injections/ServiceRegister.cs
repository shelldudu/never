using Never.Exceptions;
using System;

namespace Never.IoC.Injections
{
    /// <summary>
    /// 注册组件接口实现对象
    /// </summary>
    public sealed class ServiceRegister : IServiceRegister
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly IRegisterRuleContainer register = null;

        /// <summary>
        /// 检查状态
        /// </summary>
        private IValuableOption<UnableRegisterRule> option = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRegister"/> class.
        /// </summary>
        /// <param name="register"></param>
        /// <param name="option"></param>
        public ServiceRegister(IRegisterRuleContainer register, IValuableOption<UnableRegisterRule> option)
        {
            this.register = register;
            this.option = option;
        }

        #endregion ctor

        #region IServiceRegister成员

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <typeparam name="TImplementation">继承TSservice对象的具体对象</typeparam>
        /// <typeparam name="TService">服务类型</typeparam>
        public void RegisterType<TImplementation, TService>()
        {
            if (this.option.Value.Unabled)
                throw new InvalidException("the builder is builded,can not update rules");

            this.RegisterType<TImplementation, TService>(string.Empty, ComponentLifeStyle.Transient);
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <typeparam name="TImplementation">继承TSservice对象的具体对象</typeparam>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="lifeStyle">生命周期</param>
        public void RegisterType<TImplementation, TService>(string key, ComponentLifeStyle lifeStyle)
        {
            if (this.option.Value.Unabled)
                throw new InvalidException("the builder is builded,can not update rules");

            var rule = new RegisterRuleCollector(1);
            rule.RegisterType<TImplementation, TService>(key, lifeStyle);
            register.Update(rule);
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <param name="implementationType">继承TSservice对象的具体对象</param>
        /// <param name="serviceType">服务类型</param>
        public void RegisterType(Type implementationType, Type serviceType)
        {
            if (this.option.Value.Unabled)
                throw new InvalidException("the builder is builded,can not update rules");

            this.RegisterType(implementationType, serviceType, string.Empty, ComponentLifeStyle.Transient);
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <param name="implementationType">继承TSservice对象的具体对象</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="key">key</param>
        /// <param name="lifeStyle">生命周期</param>
        public void RegisterType(Type implementationType, Type serviceType, string key, ComponentLifeStyle lifeStyle)
        {
            if (this.option.Value.Unabled)
                throw new InvalidException("the builder is builded,can not update rules");

            var rule = new RegisterRuleCollector(1);
            rule.RegisterType(implementationType, serviceType, key, lifeStyle);
            register.Update(rule);
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">服务对象</param>
        public void RegisterInstance<TService>(TService instance)
        {
            if (this.option.Value.Unabled)
                throw new InvalidException("the builder is builded,can not update rules");

            this.RegisterInstance(instance, typeof(TService), string.Empty);
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">服务对象</param>
        /// <param name="key">key</param>
        public void RegisterInstance<TService>(TService instance, string key)
        {
            if (this.option.Value.Unabled)
                throw new InvalidException("the builder is builded,can not update rules");

            var rule = new RegisterRuleCollector(1);
            rule.RegisterInstance<TService>(instance, key);
            register.Update(rule);
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <param name="instance">服务对象</param>
        /// <param name="serviceType">服务类型</param>
        public void RegisterInstance(object instance, Type serviceType)
        {
            if (this.option.Value.Unabled)
                throw new InvalidException("the builder is builded,can not update rules");

            this.RegisterInstance(instance, serviceType, string.Empty);
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <param name="instance">服务对象</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="key">key</param>
        public void RegisterInstance(object instance, Type serviceType, string key)
        {
            if (this.option.Value.Unabled)
                throw new InvalidException("the builder is builded,can not update rules");

            var rule = new RegisterRuleCollector(1);
            rule.RegisterInstance(instance, serviceType, key);
            register.Update(rule);
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="mission">回调生成</param>
        /// <param name="key">key</param>
        /// <param name="lifeStyle">生命周期</param>
        /// <returns></returns>
        public void RegisterCallBack<TService>(string key, ComponentLifeStyle lifeStyle, Func<TService> mission)
        {
            if (this.option.Value.Unabled)
                throw new InvalidException("the builder is builded,can not update rules");

            var rule = new RegisterRuleCollector(1);
            rule.RegisterCallBack(key, lifeStyle, mission);
            register.Update(rule);
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="mission">回调生成</param>
        /// <param name="key">key</param>
        /// <param name="lifeStyle">生命周期</param>
        /// <returns></returns>
        public void RegisterCallBack<TService>(string key, ComponentLifeStyle lifeStyle, Func<ILifetimeScope, TService> mission)
        {
            if (this.option.Value.Unabled)
                throw new InvalidException("the builder is builded,can not update rules");

            var rule = new RegisterRuleCollector(1);
            rule.RegisterCallBack(key, lifeStyle, mission);
            register.Update(rule);
        }

        /// <summary>
        /// 创建对象，可以是未在容器注册
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        public void RegisterObject(Type serviceType)
        {
            if (this.option.Value.Unabled)
                throw new InvalidException("the builder is builded,can not update rules");

            this.register.CreateRule(serviceType);
        }

        /// <summary>
        /// 创建对象，可以是未在容器注册
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns></returns>
        public void RegisterObject<TService>()
        {
            if (this.option.Value.Unabled)
                throw new InvalidException("the builder is builded,can not update rules");

            this.register.CreateRule(typeof(TService));
        }

        /// <summary>
        /// 是否注册了组件
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns></returns>
        public bool Contain<TService>()
        {
            return this.register.QueryRule(typeof(TService), string.Empty, false) != null;
        }

        /// <summary>
        /// 是否注册了组件
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        public bool Contain(Type serviceType)
        {
            return this.register.QueryRule(serviceType, string.Empty, false) != null;
        }

        #endregion IServiceRegister成员
    }
}