using Never.Aop;
using Never.Aop.DynamicProxy.Builders;
using Never.Exceptions;
using Never.IoC.Injections.Rules;
using System;
using System.Collections.Generic;

namespace Never.IoC.Injections
{
    /// <summary>
    /// 规则集合器
    /// </summary>
    public class RegisterRuleContainer : IRegisterRuleContainer, IRegisterRuleQuery, IRegisterRuleChangeable
    {
        #region field

        /// <summary>
        /// 容器规则
        /// </summary>
        private List<RegisterRule> rules = null;

        /// <summary>
        /// 锁
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// 已经构建了
        /// </summary>
        private ILifetimeScope rootScope = null;

        /// <summary>
        /// 已经构建了
        /// </summary>
        private RegisterRule rootRule = null;

        /// <summary>
        /// 更新规则
        /// </summary>
        private readonly IValuableOption<UnableRegisterRule> option;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="option"></param>
        public RegisterRuleContainer(IValuableOption<UnableRegisterRule> option)
        {
            this.rules = new List<RegisterRule>(16);
            this.option = option;
        }

        #endregion ctor

        #region rules

        /// <summary>
        /// 注册规则
        /// </summary>
        public IEnumerable<RegisterRule> Rules
        {
            get
            {
                return this.rules;
            }
        }

        /// <summary>
        /// 查询某个规则，如果key为空，则可以查询最后一个不为空Key的规则
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="key">规则key</param>
        /// <returns></returns>
        private RegisterRule QueryForObject(Type serviceType, string key)
        {
            /*
             （1）先查询全量匹配的key，找到则直接返回
             （2）当key为空，获取空key的对象
             （3）如果key为空并且找不到空key的对象，则取最后一个注册的，
             （4）如果key不为空，则抛异常或不处理*/

            RegisterRule rule = null, last = null;
            /*因为key不为空是不可能查询为空的规则*/
            if (!string.IsNullOrEmpty(key))
            {
                /*先查询全量匹配的key，找到则直接返回*/
                for (var i = this.rules.Count - 1; i >= 0; i--)
                {
                    rule = rules[i];
                    if (rule.Key.IsEquals(key) && rule.Match(serviceType))
                        return rule;
                }

                return null;
            }

            /*当key为空并且找不到空key的对象，获取最后一个注册的*/
            for (var i = this.rules.Count - 1; i >= 0; i--)
            {       
                if (rules[i].Match(serviceType))
                {
                    rule = rules[i];
                    if (last == null)
                        last = rule;

                    if (rule.Key.IsNullOrEmpty())
                        return rule;
                }
            }

            return last ?? rule;
        }

        /// <summary>
        /// 查询列表规则
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        private IEnumerable<RegisterRule> QueryForEnumerable(Type serviceType)
        {
            for (var i = this.rules.Count - 1; i >= 0; i--)
            {
                if (rules[i].Match(serviceType))
                    yield return rules[i];
            }
        }

        /// <summary>
        /// 查询某个规则，会强制匹配Key的一致性，并且会检查兼容性
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="key">规则key</param>
        /// <param name="topRule">最顶层的注册规则</param>
        /// <returns></returns>
        public RegisterRule FreedomQuery(Type serviceType, string key, RegisterRule topRule)
        {
            /*
             （1）先查询全量匹配的key，找到则直接返回
             （2）当key为空，获取空key的对象
             （3）如果key为空并且找不到空key的对象，则取最后一个注册的，
             （4）如果key不为空，则抛异常或不处理*/

            /*因为key不为空是不可能查询为空的规则*/
            if (!string.IsNullOrEmpty(key))
            {
                for (var i = this.rules.Count - 1; i >= 0; i--)
                {
                    if (key.IsEquals(rules[i].Key) && topRule.LifeStyle.Compatible(rules[i].LifeStyle) && rules[i].Match(serviceType))
                        return rules[i];
                }
            }

            /*当key为空并且找不到空key的对象，获取最后一个注册的*/
            for (var i = this.rules.Count - 1; i >= 0; i--)
            {
                if (rules[i].Key.IsNullOrEmpty() && topRule.LifeStyle.Compatible(rules[i].LifeStyle) && rules[i].Match(serviceType))
                    return rules[i];
            }

            return null;
        }

        /// <summary>
        /// 查询某个规则，会强制匹配Key的一致性，并且不会检查兼容性
        /// </summary>
        /// <param name="serviceType">对象Type</param>
        /// <param name="key">注册的Key</param>
        /// <returns></returns>
        public IEnumerable<RegisterRule> StrictQuery(Type serviceType, string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                for (var i = this.rules.Count - 1; i >= 0; i--)
                {
                    if (this.rules[i].Key.IsEquals(key) && this.rules[i].Match(serviceType))
                        yield return this.rules[i];
                }
            }
            else
            {
                for (var i = this.rules.Count - 1; i >= 0; i--)
                {
                    if (this.rules[i].Match(serviceType))
                        yield return this.rules[i];
                }
            }
        }

        #endregion rules

        #region resolve

        /// <summary>
        /// 查询规则
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="key"></param>
        /// <param name="throw">是否throw异常</param>
        /// <returns></returns>
        public RegisterRule QueryRule(Type serviceType, string key, bool @throw)
        {
            var rule = this.QueryForObject(serviceType, key);
            if (rule != null)
                return rule;

            if (serviceType.IsArray)
            {
                var typeTemp = typeof(ConstructorList<>).MakeGenericType(serviceType.GetElementType());
                rule = this.QueryForObject(typeTemp, key);
                if (rule != null)
                    return rule;

                lock (locker)
                {
                    var containerRule = new RegisterRuleCollector(1);
                    var registerRule = (RegisterRule)containerRule.RegisterArrayType(typeTemp, serviceType, key, ComponentLifeStyle.Scoped);
                    /*更新容器*/
                    this.Update(containerRule);
                    return registerRule;
                }
            }

            if (!serviceType.IsGenericType)
            {
                if (!@throw)
                    return null;

                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(string.Format("{0} not registed", serviceType.FullName));

                throw new ArgumentNullException(string.Format("{0} with key {1} not registed", serviceType.FullName, key));
            }

            rule = this.QueryForObject(serviceType.GetGenericTypeDefinition(), key);
            if (rule == null)
            {
                if (!@throw)
                    return null;

                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(string.Format("{0} not registed", serviceType.GetGenericTypeDefinition().FullName));

                throw new ArgumentNullException(string.Format("{0} with key {1} not registed", serviceType.FullName, key));
            }

            lock (locker)
            {
                var ruletemp = this.QueryForObject(rule.ServiceType.MakeGenericType(serviceType.GetGenericArguments()), key);
                if (ruletemp != null)
                    return ruletemp;

                var containerRule = new RegisterRuleCollector(1);
                var registerRule = (RegisterRule)containerRule.RegisterType(rule.ImplementationType.MakeGenericType(serviceType.GetGenericArguments()), rule.ServiceType.MakeGenericType(serviceType.GetGenericArguments()), key, rule.LifeStyle);
                if (rule.ParametersCount > 0)
                    registerRule.PasteParameters(rule);

                /*更新容器*/
                this.Update(containerRule);

                return registerRule;
            }
        }

        /// <summary>
        /// 查询规则
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public RegisterRule[] QueryAllRule(Type serviceType)
        {
            var rules = new List<RegisterRule>(5);
            rules.AddRange(this.QueryForEnumerable(serviceType));
            return rules.ToArray();
        }

        /// <summary>
        /// 构建信息
        /// </summary>
        /// <param name="rule">服务类型规则</param>
        /// <param name="scope">scope</param>
        /// <param name="context">构建行为在执行过程的上下文</param>
        /// <returns></returns>
        public object ResolveRule(RegisterRule rule, ILifetimeScope scope, IResolveContext context)
        {
            if (rule.CanNotResolve != null)
                throw new Exception(string.Format("can not resolve the {0}", rule.ServiceType.FullName), rule.CanNotResolve);

            try
            {
                return RegisterRuleBuilder.Build(rule, this, this).Invoke(rule, this, scope, context);
            }
            catch (Exception ex)
            {
                rule.CanNotResolve = new Exception(ex.Message) { Source = ex.Source, HelpLink = ex.HelpLink };
                throw new Exception(string.Format("can not resolve the {0}", rule.ServiceType.FullName), rule.CanNotResolve);
            }
        }

        /// <summary>
        /// 构建信息
        /// </summary>
        /// <param name="rule">服务类型规则</param>
        /// <param name="scope">scope</param>
        /// <param name="context">构建行为在执行过程的上下文</param>
        /// <returns></returns>
        public object OptionalResolveRule(RegisterRule rule, ILifetimeScope scope, IResolveContext context)
        {
            if (rule.CanNotOptionalResolve != null)
                throw new Exception(string.Format("can not resolve the {0}", rule.ServiceType.FullName), rule.CanNotOptionalResolve);

            try
            {
                /*执行新的规则行为*/
                return RegisterRuleBuilder.OptionalBuild(rule, this, this).Invoke(rule, this, scope, context);
            }
            catch (Exception ex)
            {
                rule.CanNotOptionalResolve = new Exception(ex.Message) { Source = ex.Source, HelpLink = ex.HelpLink };
                throw new Exception(string.Format("can not resolve the {0}", rule.ServiceType.FullName), rule.CanNotOptionalResolve);
            }
        }

        /// <summary>
        /// 构建信息
        /// </summary>
        /// <param name="rules">服务类型规则</param>
        /// <param name="scope"></param>
        /// <param name="context">构建行为在执行过程的上下文</param>
        /// <returns></returns>
        public object[] ResolveAll(RegisterRule[] rules, ILifetimeScope scope, IResolveContext context)
        {
            var list = new List<object>(rules.Length);
            foreach (var rule in rules)
            {
                list.Add(RegisterRuleBuilder.Build(rule, this, this).Invoke(rule, this, scope, context));
            }

            return list.ToArray();
        }

        #endregion resolve

        #region IRegisterRuleContainer

        /// <summary>
        /// 更新容器规则
        /// </summary>
        /// <param name="collector"></param>
        public void Update(RegisterRuleCollector collector)
        {
            if (option != null && option.Value.Unabled)
                return;

            this.AlwayUpdate(collector);
        }

        /// <summary>
        /// 更新容器规则
        /// </summary>
        /// <param name="collector"></param>
        public void AlwayUpdate(RegisterRuleCollector collector)
        {
            lock (locker)
            {
                foreach (var r in collector.Rules)
                {
                    this.rules.Add(r);
                }

                /*子注册规则*/
                foreach (var r in collector.Rules)
                {
                    var subRules = r.SubRuleQueue;
                    if (subRules == null || subRules.Count == 0)
                        continue;

                    foreach (var sr in subRules)
                    {
                        this.rules.Add(sr);
                    }

                    subRules.Clear();
                }

                var proxyRules = new List<RegisterRule>(collector.Rules.Count);
                foreach (var r in collector.Rules)
                {
                    if (r.ProxyImplementationType == null & r.ProxyParameters.Length <= 0)
                        continue;

                    //if (r.ProxyParameters.Length <= 0)
                    //    continue;

                    var interceptors = new List<Type>(r.ProxyParameters.Length);
                    foreach (var p in r.ProxyParameters)
                        interceptors.Add(p.Value);

                    var implementationType = r.ProxyImplementationType == null ? new MyProxyMockBuilder().Build(r.ImplementationType, r.ServiceType, interceptors.ToArray(), r.ProxySetting) : r.ProxyImplementationType;
                    var proxyRule = new RegisterRule(implementationType, r.ServiceType, string.Empty, r.ProxyParameters.Length > 0 ? ComponentLifeStyle.Transient : r.LifeStyle);

                    var paramters = new List<KeyValueTuple<string, Type>>(r.ProxyParameters.Length + 1);
                    paramters.AddRange(r.ProxyParameters);
                    paramters.AddRange(new[] { new KeyValueTuple<string, Type>(r.Key, r.ServiceType) });
                    proxyRule.PasteParameters(paramters.ToArray());

                    proxyRules.Add(proxyRule);
                }

                foreach (var r in proxyRules)
                {
                    this.rules.Add(r);
                }

                return;
            }
        }

        /// <summary>
        /// 创建对象，可以是未在容器注册
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        public RegisterRule CreateRule(Type serviceType)
        {
            var rule = this.QueryRule(serviceType, string.Empty, false);
            if (rule != null)
                return rule;

            if (!RegisterRuleCollector.TryRule(serviceType, serviceType))
                return null;

            lock (locker)
            {
                rule = this.QueryRule(serviceType, string.Empty, false);
                if (rule != null)
                    return rule;

                var containerRule = new RegisterRuleCollector();
                containerRule.RegisterType(serviceType, serviceType);
                this.Update(containerRule);
                return containerRule.Rules[0];
            }
        }

        /// <summary>
        ///
        /// </summary>
        private class MyProxyMockBuilder : ProxyMockBuilder
        {
            public Type Build(Type implementationType, Type baseType, Type[] interceptors, InterceptCompileSetting setting)
            {
                if (interceptors != null && interceptors.Length > 0)
                {
                    foreach (var i in interceptors)
                    {
                        if (!typeof(IInterceptor).IsAssignableFrom(i))
                            throw new ArgumentException(string.Format("类型{0}只能从拦截器{1}中派生", i.FullName, typeof(IInterceptor).FullName));
                    }
                }

                var type = default(Type);

                /*构建接口，较为容易*/
                if (baseType.IsVisible && baseType.IsInterface)
                {
                    type = this.BuildInterface(baseType, implementationType, interceptors, setting);
                    return type;
                }

                /*构建类*/
                type = this.BuildClass(baseType, interceptors, setting);
                return type;
            }
        }

        #endregion IRegisterRuleContainer

        #region build

        /// <summary>
        /// 构建
        /// </summary>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public ILifetimeScope Build(out RegisterRule rule)
        {
            if (this.rootScope == null)
            {
                this.rules.RemoveAll(t => t.ServiceType == typeof(ILifetimeScope));
                rule = new RegisterRule(typeof(LifetimeScope), typeof(ILifetimeScope), string.Empty, ComponentLifeStyle.Scoped);
                this.rootScope = new LifetimeScope(this, rule);
                this.rules.Add(rule);
            }
            else
            {
                rule = this.rootRule;
            }

            return this.rootScope;
        }

        #endregion build
    }
}