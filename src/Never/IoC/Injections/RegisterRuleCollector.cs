using System;
using System.Collections.Generic;

namespace Never.IoC.Injections
{
    /// <summary>
    /// 规则收集者
    /// </summary>
    public class RegisterRuleCollector
    {
        #region prop

        /// <summary>
        ///
        /// </summary>
        public readonly List<RegisterRule> Rules = null;

        #endregion prop

        #region ctor

        /// <summary>
        ///
        /// </summary>
        public RegisterRuleCollector()
            : this(20)
        {
        }

        /// <summary>
        ///
        /// </summary>
        public RegisterRuleCollector(int capacity)
        {
            this.Rules = new List<RegisterRule>(capacity);
        }

        #endregion ctor

        #region check

        /// <summary>
        /// 检查注册规则
        /// </summary>
        /// <param name="implServiceType">Type of the implementation service.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public static void Rule(Type implServiceType, Type serviceType)
        {
            if (implServiceType.IsInterface)
                throw new ArgumentException(string.Format("{0} isInterface ", implServiceType.FullName));

            if (implServiceType.IsAbstract)
                throw new ArgumentException(string.Format("{0} isAbstract ", implServiceType.FullName));

            /*注册规则*/
            var isGenericTypeDefinition = serviceType.IsGenericTypeDefinition | implServiceType.IsGenericTypeDefinition;
            /*说明是泛型注入*/
            if (isGenericTypeDefinition)
            {
                /*接口是泛型注入，此时要求实例也是泛型注入的*/
                if (!implServiceType.IsGenericTypeDefinition)
                    throw new ArgumentException(string.Format("{0} is genericTypeDefinition,{1} is not genericTypeDefinition", serviceType.FullName, implServiceType.FullName));

                /*实例是泛型注入，此时要求接口也是泛型注入的*/
                if (!serviceType.IsGenericTypeDefinition)
                    throw new ArgumentException(string.Format("{0} is genericTypeDefinition,{1} is not genericTypeDefinition", implServiceType.FullName, serviceType.FullName));
            }

            if (!ObjectExtension.IsAssignableFromType(implServiceType, serviceType))
                throw new ArgumentNullException(string.Format("{0} can't assignableFromType {1}", serviceType.FullName, implServiceType.FullName));
        }

        /// <summary>
        /// 检查注册规则
        /// </summary>
        /// <param name="implServiceType">Type of the implementation service.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public static bool TryRule(Type implServiceType, Type serviceType)
        {
            try
            {
                Rule(implServiceType, serviceType);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion check

        #region register

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <typeparam name="TImplementation">继承TSservice对象的具体对象</typeparam>
        /// <typeparam name="TService">服务类型</typeparam>
        public IRegisterRule RegisterType<TImplementation, TService>()
        {
            return this.RegisterType<TImplementation, TService>(string.Empty, ComponentLifeStyle.Transient);
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <typeparam name="TImplementation">继承TSservice对象的具体对象</typeparam>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="lifeStyle">生命周期</param>
        public IRegisterRule RegisterType<TImplementation, TService>(string key, ComponentLifeStyle lifeStyle)
        {
            /*注册规则*/
            Rule(typeof(TImplementation), typeof(TService));

            /*开始注册*/
            var rule = new RegisterRule(typeof(TImplementation), typeof(TService), key == null ? string.Empty : key, lifeStyle);
            this.Rules.Add(rule);
            if (lifeStyle == ComponentLifeStyle.Transient)
                return rule;

            return rule;
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <param name="implementationType">继承TSservice对象的具体对象</param>
        /// <param name="serviceType">服务类型</param>
        public IRegisterRule RegisterType(Type implementationType, Type serviceType)
        {
            return this.RegisterType(implementationType, serviceType, string.Empty, ComponentLifeStyle.Transient);
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <param name="implementationType">继承TSservice对象的具体对象</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="key">key</param>
        /// <param name="lifeStyle">生命周期</param>
        public IRegisterRule RegisterType(Type implementationType, Type serviceType, string key, ComponentLifeStyle lifeStyle)
        {
            /*注册规则*/
            Rule(implementationType, serviceType);

            /*开始注册*/
            var rule = new RegisterRule(implementationType, serviceType, key == null ? string.Empty : key, lifeStyle);
            this.Rules.Add(rule);
            if (lifeStyle == ComponentLifeStyle.Transient)
                return rule;

            return rule;
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <param name="implementationType">继承TSservice对象的具体对象</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="key">key</param>
        /// <param name="lifeStyle">生命周期</param>
        internal IRegisterRule RegisterArrayType(Type implementationType, Type serviceType, string key, ComponentLifeStyle lifeStyle)
        {
            /*开始注册*/
            var rule = new RegisterRule(implementationType, serviceType, key == null ? string.Empty : key, lifeStyle);
            this.Rules.Add(rule);
            rule.IsArray = true;
            if (lifeStyle == ComponentLifeStyle.Transient)
                return rule;

            return rule;
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">服务对象</param>
        public IRegisterRule RegisterInstance<TService>(TService instance)
        {
            return this.RegisterInstance<TService>(instance, string.Empty);
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">服务对象</param>
        /// <param name="key">key</param>
        public IRegisterRule RegisterInstance<TService>(TService instance, string key)
        {
            if (((object)instance) == null)
                throw new ArgumentNullException("instance is null");

            var rule = new RegisterRule(instance, typeof(TService), key == null ? string.Empty : key, ComponentLifeStyle.Singleton);
            this.Rules.Add(rule);

            return rule;
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <param name="instance">服务对象</param>
        /// <param name="serviceType">服务类型</param>
        public IRegisterRule RegisterInstance(object instance, Type serviceType)
        {
            return this.RegisterInstance(instance, serviceType, string.Empty);
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <param name="instance">服务对象</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="key">key</param>
        public IRegisterRule RegisterInstance(object instance, Type serviceType, string key)
        {
            if (instance == null)
                throw new ArgumentNullException("instance is null");

            /*注册规则*/
            Rule(instance.GetType(), serviceType);

            /*开始注册*/
            var rule = new RegisterRule(instance, serviceType, key == null ? string.Empty : key, ComponentLifeStyle.Singleton);
            this.Rules.Add(rule);

            return rule;
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="mission">回调生成</param>
        /// <param name="key">key</param>
        /// <param name="lifeStyle">生命周期</param>
        /// <returns></returns>
        public IRegisterRule RegisterCallBack<TService>(string key, ComponentLifeStyle lifeStyle, Func<TService> mission)
        {
            if (mission == null)
                throw new ArgumentNullException("mission is null");

            /*开始注册*/
            var rule = new RegisterRule(typeof(TService), typeof(TService), key == null ? string.Empty : key, lifeStyle);
            rule.Builder = new Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>((x, y, s, z) => mission.Invoke());
            rule.OptionalBuilder = rule.Builder;
            rule.FromRegisterCallBack = true;
            this.Rules.Add(rule);
            return rule;
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="mission">回调生成</param>
        /// <param name="key">key</param>
        /// <param name="lifeStyle">生命周期</param>
        /// <returns></returns>
        public IRegisterRule RegisterCallBack<TService>(string key, ComponentLifeStyle lifeStyle, Func<ILifetimeScope, TService> mission)
        {
            if (mission == null)
                throw new ArgumentNullException("mission is null");

            /*开始注册*/
            var rule = new RegisterRule(typeof(TService), typeof(TService), key == null ? string.Empty : key, lifeStyle);
            rule.Builder = new Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>((x, y, s, z) => mission.Invoke(s));
            rule.OptionalBuilder = rule.Builder;
            rule.FromRegisterCallBack = true;
            this.Rules.Add(rule);
            return rule;
        }

        #endregion register
    }
}