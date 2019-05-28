using Never.IoC;
using System;

namespace Never.Startups.Impls
{
    /// <summary>
    /// 没有功能的服务定位器
    /// </summary>
    public sealed class EmptyServiceLocator : Never.IoC.IServiceLocator
    {
        #region field

        /// <summary>
        /// 空对象
        /// </summary>
        public static EmptyServiceLocator Only
        {
            get
            {
                if (Singleton<EmptyServiceLocator>.Instance == null)
                    Singleton<EmptyServiceLocator>.Instance = new EmptyServiceLocator();

                return Singleton<EmptyServiceLocator>.Instance;
            }
        }

        #endregion field

        #region ctor

        /// <summary>
        /// Prevents a default instance of the <see cref="EmptyServiceLocator"/> class from being created.
        /// </summary>
        private EmptyServiceLocator()
        {
        }

        #endregion ctor

        /// <summary>
        /// 返回所有T对象的实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        TService[] Never.IoC.IServiceLocator.ResolveAll<TService>(ILifetimeScopeTracker scopeTracker)
        {
            return null;
        }

        /// <summary>
        /// 返回所有T对象的实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        object[] Never.IoC.IServiceLocator.ResolveAll(Type serviceType, ILifetimeScopeTracker scopeTracker)
        {
            return null;
        }

        /// <summary>
        /// 返回T对象的实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        TService IServiceLocator.Resolve<TService>(ILifetimeScopeTracker scopeTracker)
        {
            return default(TService);
        }

        /// <summary>
        /// 返回某对象实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        object IServiceLocator.Resolve(Type serviceType, ILifetimeScopeTracker scopeTracker)
        {
            return null;
        }

        /// <summary>
        /// 返回T对象的实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="scopeTracker">跟踪者</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        TService Never.IoC.IServiceLocator.Resolve<TService>(string key, ILifetimeScopeTracker scopeTracker)
        {
            return default(TService);
        }

        /// <summary>
        /// 返回某对象实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        object Never.IoC.IServiceLocator.Resolve(Type serviceType, string key, ILifetimeScopeTracker scopeTracker)
        {
            return null;
        }

        /// <summary>
        /// 尝试返回已注册的T对象实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="scopeTracker">跟踪者</param>
        /// <param name="instance">服务对象</param>
        /// <returns></returns>
        bool IServiceLocator.TryResolve<TService>(ref TService instance, ILifetimeScopeTracker scopeTracker)
        {
            instance = default(TService);
            return false;
        }

        /// <summary>
        /// 尝试返回已注册的T对象实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="scopeTracker">跟踪者</param>
        /// <param name="instance">服务对象</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        bool IoC.IServiceLocator.TryResolve<TService>(ref TService instance, string key, ILifetimeScopeTracker scopeTracker)
        {
            instance = default(TService);
            return false;
        }

        /// <summary>
        /// 尝试返回已注册的serviceType对象实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <param name="instance">服务对象</param>
        /// <returns></returns>
        bool IServiceLocator.TryResolve(Type serviceType, ref object instance, ILifetimeScopeTracker scopeTracker)
        {
            instance = null;
            return false;
        }

        /// <summary>
        /// 尝试返回已注册的serviceType对象实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <param name="instance">服务对象</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        bool IoC.IServiceLocator.TryResolve(Type serviceType, ref object instance, string key, ILifetimeScopeTracker scopeTracker)
        {
            instance = null;
            return false;
        }

        /// <summary>
        /// 返回T对象的实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns></returns>
        TService IoC.IServiceLocator.ResolveOptional<TService>(ILifetimeScopeTracker scopeTracker)
        {
            return default(TService);
        }

        /// <summary>
        /// 返回某对象实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        object IoC.IServiceLocator.ResolveOptional(Type serviceType, ILifetimeScopeTracker scopeTracker)
        {
            return null;
        }

        ILifetimeScope IServiceLocator.BeginLifetimeScope()
        {
            return null;
        }
    }
}