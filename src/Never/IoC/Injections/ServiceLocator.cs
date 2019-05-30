using Never.Exceptions;
using System;
using System.Linq;

namespace Never.IoC.Injections
{
    /// <summary>
    /// 注册组件接口实现对象
    /// </summary>
    public sealed class ServiceLocator : IServiceLocator
    {
        #region field

        /// <summary>
        /// 容器
        /// </summary>
        private readonly ILifetimeScope rootScope = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocator"/> class.
        /// </summary>
        /// <param name="rootScope">The rootScope.</param>
        internal ServiceLocator(ILifetimeScope rootScope)
        {
            this.rootScope = rootScope;
        }

        #endregion ctor

        #region IServiceLocator

        /// <summary>
        /// 跟踪者
        /// </summary>
        public ILifetimeScopeTracker ScopeTracker { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public ILifetimeScope BeginLifetimeScope()
        {
            return this.rootScope.BeginLifetimeScope();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public ILifetimeScope BeginLifetimeScope(ILifetimeScope scope)
        {
            if (this.ScopeTracker == null)
                throw new ArgumentNullException("scopeTracker is null");

            return this.ScopeTracker.StartScope(scope);
        }

        /// <summary>
        /// 返回所有T对象的实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns></returns>
        public TService[] ResolveAll<TService>()
        {
            var all = this.BeginLifetimeScope(this.rootScope).ResolveAll(typeof(TService));
            var result = all?.Select(t => (TService)t).ToArray();
            return result;
        }

        /// <summary>
        /// 返回所有T对象的实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        public object[] ResolveAll(Type serviceType)
        {
            return this.BeginLifetimeScope(this.rootScope).ResolveAll(serviceType);
        }

        /// <summary>
        /// 返回T对象的实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns></returns>
        public TService Resolve<TService>()
        {
            return (TService)this.BeginLifetimeScope(this.rootScope).Resolve(typeof(TService), string.Empty);
        }

        /// <summary>
        /// 返回T对象的实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="key">key</param>
        /// <returns></returns>
        public TService Resolve<TService>(string key)
        {
            return (TService)this.BeginLifetimeScope(this.rootScope).Resolve(typeof(TService), key);
        }

        /// <summary>
        /// 返回T对象的实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns></returns>
        public TService ResolveOptional<TService>()
        {
            TService service = default(TService);
            this.ResolveOptional<TService>(string.Empty, out service);
            return service;
        }

        /// <summary>
        /// 返回某对象实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        public object ResolveOptional(Type serviceType)
        {
            object service = default(object);
            this.ResolveOptional(serviceType, string.Empty, out service);
            return service;
        }

        /// <summary>
        /// 返回T对象的实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="key"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        private bool ResolveOptional<TService>(string key, out TService service)
        {
            var scope = this.BeginLifetimeScope(this.rootScope);
            try
            {
                service = (TService)scope.Resolve(typeof(TService), key);
                return true;
            }
            catch
            {
            }

            try
            {
                service = (TService)scope.ResolveOptional(typeof(TService));
                return true;
            }
            catch
            {
            }

            service = default(TService);
            return false;
        }

        /// <summary>
        /// 返回某对象实体
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool ResolveOptional(Type serviceType, string key, out object service)
        {
            var scope = this.BeginLifetimeScope(this.rootScope);
            try
            {
                service = scope.Resolve(serviceType, key);
                return true;
            }
            catch
            {
            }

            try
            {
                service = scope.ResolveOptional(serviceType);
                return true;
            }
            catch
            {
            }

            service = default(object);
            return false;
        }

        /// <summary>
        /// 返回某对象实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        public object Resolve(Type serviceType)
        {
            return this.BeginLifetimeScope(this.rootScope).Resolve(serviceType, string.Empty);
        }

        /// <summary>
        /// 返回某对象实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public object Resolve(Type serviceType, string key)
        {
            return this.BeginLifetimeScope(this.rootScope).Resolve(serviceType, key);
        }

        /// <summary>
        /// 尝试返回已注册的T对象实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">服务对象</param>
        /// <returns></returns>
        public bool TryResolve<TService>(ref TService instance)
        {
            return this.TryResolve<TService>(ref instance, string.Empty);
        }

        /// <summary>
        /// 尝试返回已注册的T对象实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">服务对象</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public bool TryResolve<TService>(ref TService instance, string key)
        {
            try
            {
                return this.ResolveOptional<TService>(key, out instance);
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// 尝试返回已注册的serviceType对象实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="instance">服务对象</param>
        /// <returns></returns>
        public bool TryResolve(Type serviceType, ref object instance)
        {
            return this.TryResolve(serviceType, ref instance, string.Empty);
        }

        /// <summary>
        /// 尝试返回已注册的serviceType对象实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="instance">服务对象</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public bool TryResolve(Type serviceType, ref object instance, string key)
        {
            try
            {
                return this.ResolveOptional(serviceType, key, out instance);
            }
            catch
            {
            }

            return false;
        }

        #endregion IServiceLocator
    }
}