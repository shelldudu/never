using Never;
using Never.Caching;
using System;
using System.Collections.Generic;

namespace Never.IoC.Injections
{
    /// <summary>
    /// 每条线程构建的环境
    /// </summary>
    internal class LifetimeScope : ILifetimeScope
    {
        #region field

        /// <summary>
        /// 容器
        /// </summary>
        private readonly IRegisterRuleContainer container = null;

        /// <summary>
        /// 父级
        /// </summary>
        private LifetimeScope parent = null;

        /// <summary>
        /// 构造上下文
        /// </summary>
        private ResolveContext context;

        /// <summary>
        /// 当前Key
        /// </summary>
        private string cachedKey;

        /// <summary>
        /// 是否已经释放了
        /// </summary>
        private bool isDisposed = false;

        /// <summary>
        /// 完成释放
        /// </summary>
        public event EventHandler OnDisposed;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="LifetimeScope"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="rule"></param>
        public LifetimeScope(IRegisterRuleContainer container, IRegisterRule rule)
        {
            this.container = container;
            this.cachedKey = rule == null ? typeof(LifetimeScope).Name : rule.ToString();
            this.context = new ResolveContext()
            {
                Disposer = null,
                Singleton = new SingletonComponentLifeStyleStorager() { ContextCache = new TransientContextCache() },
                Scope = null
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LifetimeScope"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private LifetimeScope(LifetimeScope parent)
        {
            this.container = parent.container;
            this.parent = parent;
            this.cachedKey = parent.cachedKey;
            this.context = new ResolveContext()
            {
                Disposer = new Disposer(),
                Scope = new ScopeComponentLifeStyleStorager() { ContextCache = new TransientContextCache() },
                Singleton = parent.context.Singleton,
            };

            //自己resolve自己的时候，直接返回自身
            this.context.Scope.ContextCache.Set<ILifetimeScope>(cachedKey, this);
        }

        #endregion ctor

        #region prop

        /// <summary>
        /// 上下文
        /// </summary>
        public ResolveContext Context
        {
            get
            {
                return this.context;
            }
        }

        #endregion prop

        #region ILifetimeScope

        public ILifetimeScope BeginLifetimeScope()
        {
            if (this.isDisposed)
                throw new ObjectDisposedException("current lifetime scope is disposed");

            return new LifetimeScope(this);
        }

        public object[] ResolveAll(Type serviceType)
        {
            return this.ResolveAll(this.container.QueryAllRule(serviceType));
        }

        public object[] ResolveAll(RegisterRule[] rules)
        {
            if (rules == null || rules.Length == 0)
                return new object[0];

            return this.container.ResolveAll(rules, this, this.context);
        }

        public object Resolve(Type serviceType, string key)
        {
            return this.Resolve(this.container.QueryRule(serviceType, key, true));
        }

        public object Resolve(RegisterRule rule)
        {
            if (rule == null)
                return null;

            return this.container.ResolveRule(rule, this, this.context);
        }

        public object ResolveOptional(Type serviceType)
        {
            return this.ResolveOptional(this.container.QueryRule(serviceType, string.Empty, false));
        }

        public object ResolveOptional(RegisterRule rule)
        {
            if (rule == null)
                return null;

            return this.container.OptionalResolveRule(rule, this, this.context);
        }

        #endregion ILifetimeScope

        #region IDisposable

        public void Dispose()
        {
            if (isDisposed)
                return;

            this.parent = null;
            if (this.context.Scope != null && this.context.Scope.ContextCache != null)
                this.context.Scope.ContextCache.Clear();

            if (this.context.Disposer == null)
            {
                if (this.context.Singleton.ContextCache != null)
                {
                    var ator = this.context.Singleton.ContextCache.GetEnumerator();
                    while (ator.MoveNext())
                    {
                        if (ator.Current is IDisposable)
                            ((IDisposable)ator.Current).Dispose();
                    }

                    this.context.Singleton.ContextCache.Clear();
                }
            }
            else
            {
                this.context.Disposer.Dispose();
            }

            this.isDisposed = true;
            if (this.OnDisposed != null)
            {
                this.OnDisposed.Invoke(this, EventArgs.Empty);
                this.OnDisposed = null;
            }
        }

        #endregion IDisposable

        #region scope

        public class ScopeComponentLifeStyleStorager : IComponentLifeStyleStorager
        {
            #region field

            /// <summary>
            /// 缓存
            /// </summary>
            public ContextCache ContextCache { get; set; }

            #endregion field

            public T Query<T>(IRegisterRuleDescriptor rule)
            {
                return ContextCache == null ? default(T) : ContextCache.Get<T>(rule.ToString());
            }

            public T Cache<T>(IRegisterRuleDescriptor rule, T @object)
            {
                if (ContextCache == null)
                    return @object;

                ContextCache.Set(rule.ToString(), @object);
                return @object;
            }
        }

        public class SingletonComponentLifeStyleStorager : IComponentLifeStyleStorager
        {
            #region field

            /// <summary>
            /// 缓存
            /// </summary>
            public ContextCache ContextCache { get; set; }

            #endregion field

            public T Query<T>(IRegisterRuleDescriptor rule)
            {
                if (rule is RegisterRule)
                    return this.Query<T>((RegisterRule)rule);

                return ContextCache == null ? default(T) : ContextCache.Get<T>(rule.ToString());
            }

            public T Query<T>(RegisterRule rule)
            {
                return (T)rule.SingletonInstance;
            }

            public T Cache<T>(IRegisterRuleDescriptor rule, T @object)
            {
                if (rule is RegisterRule)
                    return this.Cache((RegisterRule)rule, @object);

                if (ContextCache == null)
                    return @object;

                ContextCache.Set(rule.ToString(), @object);
                return @object;
            }

            public T Cache<T>(RegisterRule rule, T @object)
            {
                rule.SingletonInstance = @object;
                ContextCache.Set(rule.ToString(), @object);
                return @object;
            }
        }

        /// <summary>
        ///
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
        public class ResolveContext : IResolveContext
        {
            #region prop

            /// <summary>
            /// 单利
            /// </summary>
            public SingletonComponentLifeStyleStorager Singleton { get; set; }

            /// <summary>
            /// 周期
            /// </summary>
            public ScopeComponentLifeStyleStorager Scope { get; set; }

            /// <summary>
            /// 要释放的资源的队列
            /// </summary>
            public Disposer Disposer { get; set; }

            /// <summary>
            /// 将要释放的资源添加到该队列中
            /// </summary>
            /// <param name="disponser"></param>
            public void Push(IDisposable disponser)
            {
                this.Disposer.Push(disponser);
            }

            /// <summary>
            /// 将要释放的资源添加到该队列中
            /// </summary>
            /// <param name="disponser"></param>
            public void Push(object disponser)
            {
                this.Disposer.Push(disponser);
            }

            public T Query<T>(IRegisterRuleDescriptor rule, ILifetimeScope scope)
            {
                switch (rule.LifeStyle)
                {
                    case ComponentLifeStyle.Transient:
                        {
                            return default(T);
                        }
                    case ComponentLifeStyle.Scoped:
                        {
                            if (this.Scope == null)
                                return default(T);

                            return this.Scope.Query<T>(rule);
                        }
                    case ComponentLifeStyle.Singleton:
                        {
                            if (this.Singleton == null)
                                return default(T);

                            return this.Singleton.Query<T>(rule);
                        }
                }

                return default(T);
            }

            public T Cache<T>(IRegisterRuleDescriptor rule, ILifetimeScope scope, T @object)
            {
                switch (rule.LifeStyle)
                {
                    case ComponentLifeStyle.Transient:
                        {
                            this.Push(@object);
                            return @object;
                        }

                    case ComponentLifeStyle.Scoped:
                        {
                            this.Push(@object);
                            if (this.Scope == null)
                                return @object;

                            return this.Scope.Cache(rule, @object);
                        }

                    case ComponentLifeStyle.Singleton:
                        {
                            if (this.Singleton == null)
                                return @object;

                            return this.Singleton.Cache(rule, @object);
                        }
                }
                return @object;
            }

            #endregion prop
        }

        #endregion scope
    }
}