using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.IoC.Injections
{
    /// <summary>
    /// 对象创建者
    /// </summary>
    public sealed class ServiceActivator : IServiceActivator
    {
        #region ctor

        /// <summary>
        /// 容器
        /// </summary>
        private readonly ILifetimeScope rootScope = null;

        /// <summary>
        ///
        /// </summary>
        private readonly IRegisterRuleChangeable registerRule = null;

        /// <summary>
        ///
        /// </summary>
        /// <param name="rootScope"></param>
        /// <param name="registerRule"></param>
        public ServiceActivator(ILifetimeScope rootScope, IRegisterRuleChangeable registerRule)
        {
            this.rootScope = rootScope;
            this.registerRule = registerRule;
        }

        #endregion ctor

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public ActivatorService<TService> CreateService<TService>()
        {
            var serviceType = typeof(TService);
            if (serviceType.IsGenericTypeDefinition)
                throw new ArgumentException("serviceType is generice type");

            var rule = this.registerRule.CreateRule(serviceType);
            if (rule == null)
                return new ActivatorService<TService>(default(TService), null);

            var scope = this.rootScope.BeginLifetimeScope() as LifetimeScope;
            var target = (TService)scope.Resolve(rule);

            return new ActivatorService<TService>(target, scope.Context.Disposer);
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        public ActivatorService<object> CreateService(Type serviceType)
        {
            if (serviceType.IsGenericTypeDefinition)
                throw new ArgumentException("serviceType is generice type");

            var rule = this.registerRule.CreateRule(serviceType);
            if (rule == null)
                return new ActivatorService<object>(default(object), null);

            var scope = this.rootScope.BeginLifetimeScope() as LifetimeScope;
            var target = scope.Resolve(rule);
            return new ActivatorService<object>(target, scope.Context.Disposer);
        }
    }
}