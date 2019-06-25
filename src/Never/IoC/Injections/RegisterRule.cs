using Never.Aop;
using System;
using System.Collections.Generic;

namespace Never.IoC.Injections
{
    /// <summary>
    /// 注册规则
    /// </summary>
    public class RegisterRule : IEquatable<RegisterRule>, ICloneable, IDisposable, IRegisterRule, IParameterRegisterRule, IProxyRegisterRule, IRegisterRuleDescriptor
    {
        #region 增量

        /// <summary>
        /// 当前增量
        /// </summary>
        private static long factor = 0;

        #endregion 增量

        #region field

        /// <summary>
        /// 实现者
        /// </summary>
        private readonly Type implementationType = null;

        /// <summary>
        /// 实现接口或基类
        /// </summary>
        private readonly Type serviceType = null;

        /// <summary>
        /// 注册使用参数
        /// </summary>
        private readonly List<KeyValueTuple<string, Type>> parameters = null;

        /// <summary>
        /// 注册key
        /// </summary>
        private string key = null;

        /// <summary>
        /// 关键字key
        /// </summary>
        private string toStirng = null;

        /// <summary>
        /// 注册生命周期
        /// </summary>
        private readonly ComponentLifeStyle lifeStyle = ComponentLifeStyle.Transient;

        /// <summary>
        /// 构造函数所用到的规则
        /// </summary>
        private RegisterRule[] constructorParameters = null;

        /// <summary>
        /// 构造函数所用到的规则
        /// </summary>
        private RegisterRule[] optionalConstructorParameters = null;

        /// <summary>
        /// 当前增量
        /// </summary>
        private string increment = string.Empty;

        /// <summary>
        /// 代理注册使用参数
        /// </summary>
        private List<KeyValueTuple<string, Type>> proxyParameters = null;

        /// <summary>
        /// 编译配置
        /// </summary>
        private InterceptCompileSetting setting = new InterceptCompileSetting();

        /// <summary>
        /// 代理服务类型
        /// </summary>
        private Type proxyImplementationType = null;

        /// <summary>
        /// 子注册规则，用于动态代理生成的时候
        /// </summary>
        private Queue<RegisterRule> subRuleQueue = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="implementationType"></param>
        /// <param name="serviceType"></param>
        public RegisterRule(Type implementationType, Type serviceType)
            : this(implementationType, serviceType, string.Empty, ComponentLifeStyle.Transient)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="implementationType"></param>
        /// <param name="serviceType"></param>
        /// <param name="key"></param>
        /// <param name="lifeStyle"></param>
        public RegisterRule(Type implementationType, Type serviceType, string key, ComponentLifeStyle lifeStyle)
        {
            this.implementationType = implementationType;
            this.serviceType = serviceType;
            this.parameters = new List<KeyValueTuple<string, Type>>();
            this.key = string.IsNullOrEmpty(key) ? string.Empty : key;
            this.lifeStyle = lifeStyle;
            this.IsGenericTypeDefinition = implementationType.IsGenericTypeDefinition & serviceType.IsGenericTypeDefinition;

            /*当前增量*/
            this.increment = System.Threading.Interlocked.Increment(ref factor).ToString();
            /*当前key*/
            this.toStirng = ConcatCachedKey();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="singletonInstance"></param>
        /// <param name="key"></param>
        /// <param name="lifeStyle"></param>
        public RegisterRule(object singletonInstance, Type serviceType, string key, ComponentLifeStyle lifeStyle)
        {
            this.implementationType = singletonInstance.GetType();
            this.SingletonInstance = singletonInstance;
            this.serviceType = serviceType;
            this.parameters = new List<KeyValueTuple<string, Type>>();
            this.key = string.IsNullOrEmpty(key) ? string.Empty : key;
            this.lifeStyle = lifeStyle;

            /*绑定回调方法，因为实例已经被创建了*/
            var builder = new Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>((x, y, s, z) => { return singletonInstance; });
            this.Builder = builder;
            this.OptionalBuilder = builder;
            this.FromRegisterCallBack = true;
            this.IsGenericTypeDefinition = implementationType.IsGenericTypeDefinition & serviceType.IsGenericTypeDefinition;

            /*当前增量*/
            this.increment = System.Threading.Interlocked.Increment(ref factor).ToString();
            /*当前key*/
            this.toStirng = ConcatCachedKey();
        }

        #endregion ctor

        #region prop

        /// <summary>
        /// 实现者
        /// </summary>
        public Type ImplementationType
        {
            get
            {
                return this.implementationType;
            }
        }

        /// <summary>
        /// 实现接口或基类
        /// </summary>
        public Type ServiceType
        {
            get
            {
                return this.serviceType;
            }
        }

        /// <summary>
        /// 注册使用参数
        /// </summary>
        public string Key
        {
            get
            {
                return this.key;
            }
        }

        /// <summary>
        /// 注册使用参数
        /// </summary>
        public KeyValueTuple<string, Type>[] Parameters
        {
            get
            {
                return this.parameters.ToArray();
            }
        }

        /// <summary>
        /// 注册使用参数个数
        /// </summary>
        public int ParametersCount
        {
            get
            {
                return this.parameters.Count;
            }
        }

        /// <summary>
        /// 注册生命周期
        /// </summary>
        public ComponentLifeStyle LifeStyle
        {
            get
            {
                return this.lifeStyle;
            }
        }

        /// <summary>
        /// 是否为数组
        /// </summary>
        public bool IsArray { get; internal set; }

        /// <summary>
        /// 是否可以resolve
        /// </summary>
        public Exception CanNotResolve { get; set; }

        /// <summary>
        /// 是否可以resolve
        /// </summary>
        public Exception CanNotOptionalResolve { get; set; }

        /// <summary>
        /// 是否已经准备好了
        /// </summary>
        public bool Builded { get { return this.Builder != null; } }

        /// <summary>
        /// 是否已经准备好了
        /// </summary>
        public bool OptionalBuilded { get { return this.OptionalBuilder != null; } }

        /// <summary>
        /// 单例的实现者
        /// </summary>
        public object SingletonInstance { get; set; }

        /// <summary>
        /// 是否使用委托注入的
        /// </summary>
        public bool FromRegisterCallBack { get; set; }

        /// <summary>
        /// 行为构建
        /// </summary>
        public Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object> Builder { get; set; }

        /// <summary>
        /// 行为构建
        /// </summary>
        public Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object> OptionalBuilder { get; set; }

        /// <summary>
        /// 获取一个值，该值指示当前 System.Type 是否表示可以用来构造其他泛型类型的泛型类型定义。
        /// </summary>
        public bool IsGenericTypeDefinition { get; private set; }

        /// <summary>
        /// 注册使用参数
        /// </summary>
        public KeyValueTuple<string, Type>[] ProxyParameters
        {
            get
            {
                return this.proxyParameters == null ? new KeyValueTuple<string, Type>[0] : this.proxyParameters.ToArray();
            }
        }

        /// <summary>
        /// 代理编译配置
        /// </summary>
        public InterceptCompileSetting ProxySetting
        {
            get
            {
                return this.setting;
            }
        }

        /// <summary>
        /// 代理实现者
        /// </summary>
        public Type ProxyImplementationType
        {
            get
            {
                return this.proxyImplementationType;
            }
        }

        /// <summary>
        /// 获取子注册规则
        /// </summary>
        public Queue<RegisterRule> SubRuleQueue
        {
            get
            {
                return this.subRuleQueue ?? new Queue<RegisterRule>(0);
            }
        }

        #endregion prop

        #region parameter

        /// <summary>
        /// 构造函数参数
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="key">注册key</param>
        /// <returns></returns>
        public IRegisterRule WithParameter<TService>(string key)
        {
            this.parameters.Add(new KeyValueTuple<string, Type>(string.IsNullOrEmpty(key) ? string.Empty : key, typeof(TService)));
            return this;
        }

        #endregion parameter

        #region proxy rule

        /// <summary>
        /// 检查代理规则
        /// </summary>
        /// <param name="proxyType">代理服务类型</param>
        protected void CheckProxyType(Type proxyType)
        {
            if (typeof(Never.Aop.IInterceptor).IsAssignableFrom(this.serviceType))
                throw new ArgumentNullException(string.Format("using registe the {0} rule as proxy rule, the interceptorType can not been maked proxyType", this.serviceType.FullName));

            if (proxyType == null)
                throw new ArgumentNullException(string.Format("using registe the {0} rule as proxy rule, proxyType is null", this.serviceType.FullName));

            if (proxyType.IsGenericTypeDefinition)
                throw new ArgumentNullException(string.Format("using registe the {0} rule as proxy rule, proxyType can't be the genericType", this.serviceType.FullName));

            if (proxyType.IsClass && !proxyType.IsVisible)
                throw new ArgumentNullException(string.Format("using registe the {0} rule as proxy rule, proxyType is an sealed class,it can't be make proxyType", this.serviceType.FullName));

            if (this.serviceType.IsGenericTypeDefinition)
                throw new ArgumentNullException(string.Format("using registe the {0} rule as proxy rule, genericType serviceType can't be make proxyType", this.serviceType.FullName));

            if (!this.serviceType.IsAssignableFrom(proxyType))
                throw new ArgumentNullException(string.Format("using registe the {0} rule as proxy rule, {1} can't assignable from {2}", this.serviceType.FullName, this.serviceType.FullName, proxyType.FullName));
        }

        #endregion proxy rule

        #region shadow proxy

        /// <summary>
        /// 注册为代理服务
        /// </summary>
        /// <returns></returns>
        public IProxyRegisterRule AsProxy()
        {
            this.CheckProxyType(this.serviceType);

            /*检查及格性*/
            new Aop.DynamicProxy.Builders.ProxyMockBuilder().SupportType(this.serviceType);

            /*重新将Key更新*/
            if (string.IsNullOrEmpty(key))
                this.key = "proxy";

            /*当前key*/
            this.toStirng = ConcatCachedKey();

            /*新的规则*/
            this.proxyParameters = new List<KeyValueTuple<string, Type>>(2);
            return this;
        }

        /// <summary>
        /// 注册为代理服务
        /// </summary>
        /// <param name="setting">配置</param>
        /// <returns></returns>
        public IProxyRegisterRule AsProxy(InterceptCompileSetting setting)
        {
            var rule = this.AsProxy();
            this.setting = setting;
            return rule;
        }

        /// <summary>
        /// 新加拦截器类型到构造函数参数中,key参数不能为空
        /// </summary>
        public IProxyRegisterRule WithInterceptor(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(string.Format("using registe the {0} rule as proxy rule, but key is null", this.serviceType.FullName));

            if (this.key.Equals(key, StringComparison.OrdinalIgnoreCase))
                this.key = this.key + "a6";

            this.proxyParameters.Add(new KeyValueTuple<string, Type>(string.IsNullOrEmpty(key) ? string.Empty : key, typeof(IInterceptor)));
            return this;
        }

        /// <summary>
        /// 构造函数参数,key参数不能为空
        /// </summary>
        /// <typeparam name="TInterceptor">拦截器类型</typeparam>
        /// <param name="key">注册key</param>
        /// <returns></returns>
        public IProxyRegisterRule WithInterceptor<TInterceptor>(string key) where TInterceptor : Never.Aop.IInterceptor
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(string.Format("using registe the {0} rule as proxy rule, but key is null", this.serviceType.FullName));

            if (this.key.Equals(key, StringComparison.OrdinalIgnoreCase))
                this.key = this.key + "a6";

            if (this.subRuleQueue == null)
                this.subRuleQueue = new Queue<RegisterRule>();

            this.subRuleQueue.Enqueue(new RegisterRule(typeof(TInterceptor), typeof(Never.Aop.IInterceptor), key, this.lifeStyle));
            this.proxyParameters.Add(new KeyValueTuple<string, Type>(string.IsNullOrEmpty(key) ? string.Empty : key, typeof(IInterceptor)));
            return this;
        }

        #endregion shadow proxy

        #region obvious proxy

        /// <summary>
        /// 构造函数参数
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="key">注册key</param>
        /// <returns></returns>
        IParameterRegisterRule IParameterRegisterRule.WithParameter<TService>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(string.Format("using registe the {0} rule as proxy rule, but key is null", this.serviceType.FullName));

            this.proxyParameters.Add(new KeyValueTuple<string, Type>(string.IsNullOrEmpty(key) ? string.Empty : key, typeof(TService)));
            return this;
        }

        /// <summary>
        /// 注册为代理服务
        /// </summary>
        /// <param name="proxyType">代理服务</param>
        /// <returns></returns>
        public IParameterRegisterRule AsProxy(Type proxyType)
        {
            this.CheckProxyType(proxyType);

            /*重新将Key更新*/
            if (string.IsNullOrEmpty(key))
                this.key = "proxy";

            /*当前key*/
            this.toStirng = ConcatCachedKey();

            /*代理类*/
            this.proxyImplementationType = proxyType;

            /*新的规则*/
            this.proxyParameters = new List<KeyValueTuple<string, Type>>(2);
            return this;
        }

        /// <summary>
        /// 注册为代理服务
        /// </summary>
        /// <typeparam name="TProxyType">代理服务</typeparam>
        /// <returns></returns>
        public IParameterRegisterRule AsProxy<TProxyType>()
        {
            return this.AsProxy(typeof(TProxyType));
        }

        #endregion obvious proxy

        #region ctorParamters

        /// <summary>
        /// 返回构造函数所需的注册规则
        /// </summary>
        public IEnumerable<RegisterRule> ConstructorParameters
        {
            get
            {
                return this.constructorParameters;
            }
        }

        /// <summary>
        /// 返回构造函数所需的注册规则
        /// </summary>
        public IEnumerable<RegisterRule> OptionalConstructorParameters
        {
            get
            {
                return this.optionalConstructorParameters;
            }
        }

        /// <summary>
        ///查询规则
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public RegisterRule Query(int index)
        {
            return constructorParameters[index];
        }

        /// <summary>
        ///查询规则
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public RegisterRule OptionalQuery(int index)
        {
            return optionalConstructorParameters[index];
        }

        /// <summary>
        /// 填充构造函数其他参数规则
        /// </summary>
        /// <param name="constructorParameters"></param>
        public void FillConstructorParametersOnBuilding(RegisterRule[] constructorParameters)
        {
            this.constructorParameters = constructorParameters;
        }

        /// <summary>
        /// 填充构造函数其他参数规则
        /// </summary>
        /// <param name="constructorParameters"></param>
        public void OptionalFillConstructorParametersOnBuilding(RegisterRule[] constructorParameters)
        {
            this.optionalConstructorParameters = constructorParameters;
        }

        #endregion ctorParamters

        #region IEquatable

        /// <summary>
        /// 指示当前对象是否等于同一类型的另一个对象。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(RegisterRule other)
        {
            return this.toStirng == other.toStirng;
        }

        #endregion IEquatable

        #region mathch

        /// <summary>
        /// 匹配目标类型
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public bool Match(Type serviceType)
        {
            return this.serviceType == serviceType;
        }

        /// <summary>
        /// 匹配目标类型
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool Match(RegisterRule rule)
        {
            return this.key.IsEquals(rule.key) && this.lifeStyle == rule.lifeStyle && this.implementationType == rule.implementationType && this.Match(serviceType);
        }

        #endregion mathch

        #region IDisposable

        /// <summary>
        /// [在每次线程结束后] 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///  [在每次线程结束后] 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        protected virtual void Dispose(bool isDispose)
        {
        }

        #endregion IDisposable

        #region tostring

        /// <summary>
        /// 返回当前对象字符串，通常用于在Key获取缓存
        /// </summary>
        public override string ToString()
        {
            return this.toStirng;
        }

        /// <summary>
        /// 关键字
        /// </summary>
        /// <returns></returns>
        private string ConcatCachedKey()
        {
            switch (this.lifeStyle)
            {
                case ComponentLifeStyle.Singleton:
                    {
                        return string.Concat("s", this.key, "_", increment);
                    }
                case ComponentLifeStyle.Transient:
                    {
                        return string.Concat("t", this.key, "_", increment);
                    }
                case ComponentLifeStyle.Scoped:
                    {
                        return string.Concat("l", this.key, "_", increment);
                    }
            }

            return this.serviceType.FullName;
        }

        #endregion tostring

        #region pasteparameters

        /// <summary>
        /// 粘帖参数
        /// </summary>
        /// <param name="rule">The rule.</param>
        public void PasteParameters(RegisterRule rule)
        {
            this.parameters.Clear();
            this.parameters.AddRange(rule.parameters);
        }

        /// <summary>
        /// 粘帖参数
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public void PasteParameters(KeyValueTuple<string, Type>[] parameters)
        {
            this.parameters.Clear();
            this.parameters.AddRange(parameters);
        }

        #endregion pasteparameters

        #region clone

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new RegisterRule(this.implementationType, this.serviceType);
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public RegisterRule Clone(RegisterRule container)
        {
            return new RegisterRule(this.implementationType, this.serviceType, container.key, container.lifeStyle)
            {
            };
        }

        #endregion clone
    }
}