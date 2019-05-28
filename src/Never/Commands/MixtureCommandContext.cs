using Never.Domains;
using System;

namespace Never.Commands
{
    /// <summary>
    /// 混合内存与仓库模式，当key为默认的时候，不会入内存
    /// </summary>
    public class MixtureCommandContext : MemoryCommandContext, ICommandContext
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MixtureCommandContext"/> class.
        /// </summary>
        public MixtureCommandContext()
        {
        }

        #endregion ctor

        #region context

        /// <summary>
        /// 获取聚合根，仅支持【事件流的内存模式】
        /// </summary>
        /// <typeparam name="TAggregateRootKey">聚合根标识Id，目前只有Guid,long,string,int四种</typeparam>
        /// <typeparam name="TAggregateRoot">聚合根</typeparam>
        /// <param name="key">内存模式下的key</param>
        /// <returns></returns>
        public override TAggregateRoot GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(TAggregateRootKey key)
        {
            return GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(key, default(Func<TAggregateRoot>));
        }

        /// <summary>
        /// 获取聚合根，支持【内存模式】
        /// </summary>
        /// <typeparam name="TAggregateRoot">聚合根</typeparam>
        /// <typeparam name="TAggregateRootKey">聚合根标识Id，目前只有Guid,long,string,int四种</typeparam>
        /// <param name="key">内存模式下的key</param>
        /// <param name="getAggregate">如果找不到，则从仓库里面初始化</param>
        /// <returns></returns>
        public override TAggregateRoot GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(TAggregateRootKey key, Func<TAggregateRoot> getAggregate)
        {
            var type = this.QuerykeyType<TAggregateRootKey>();
            switch (type)
            {
                case 1:
                    {
                        var aggregateRootId = (Guid)Convert.ChangeType(key, typeof(Guid));
                        if (aggregateRootId == Guid.Empty)
                            return getAggregate == null ? default(TAggregateRoot) : getAggregate();

                        var value = GuidCountCache.GetValue(aggregateRootId);
                        var root = value as TAggregateRoot;
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            return root;
                        }

                        root = getAggregate == null ? default(TAggregateRoot) : getAggregate();
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            GuidCountCache.SetValue(aggregateRootId, root, this.CacheSpan());
                        }

                        return root;
                    }
                case 2:
                    {
                        var aggregateRootId = (string)Convert.ChangeType(key, typeof(string));
                        if (string.IsNullOrEmpty(aggregateRootId))
                            return getAggregate == null ? default(TAggregateRoot) : getAggregate();

                        var value = StringCountCache.GetValue(aggregateRootId);
                        var root = value as TAggregateRoot;
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            return root;
                        }

                        root = getAggregate == null ? default(TAggregateRoot) : getAggregate();
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            StringCountCache.SetValue(aggregateRootId, root, this.CacheSpan());
                        }

                        return root;
                    }
                case 3:
                    {
                        var aggregateRootId = (long)Convert.ChangeType(key, typeof(long));
                        if (aggregateRootId == 0)
                            return getAggregate == null ? default(TAggregateRoot) : getAggregate();

                        var value = LongCountCache.GetValue(aggregateRootId);
                        var root = value as TAggregateRoot;
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            return root;
                        }

                        root = getAggregate == null ? default(TAggregateRoot) : getAggregate();
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            LongCountCache.SetValue(aggregateRootId, root, this.CacheSpan());
                        }

                        return root;
                    }
                case 4:
                    {
                        var aggregateRootId = (int)Convert.ChangeType(key, typeof(int));
                        if (aggregateRootId == 0)
                            return getAggregate == null ? default(TAggregateRoot) : getAggregate();

                        var value = IntCountCache.GetValue(aggregateRootId);
                        var root = value as TAggregateRoot;
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            return root;
                        }

                        root = getAggregate == null ? default(TAggregateRoot) : getAggregate();
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            if (aggregateRootId != 0)
                                IntCountCache.SetValue(aggregateRootId, root, this.CacheSpan());
                        }

                        return root;
                    }
            }

            return default(TAggregateRoot);
        }

        /// <summary>
        /// 获取聚合根，支持【内存模式】
        /// </summary>
        /// <typeparam name="TAggregateRoot">聚合根</typeparam>
        /// <typeparam name="TAggregateRootKey">聚合根标识Id，目前只有Guid,long,string,int四种</typeparam>
        /// <param name="key">内存模式下的key</param>
        /// <param name="getAggregate">如果找不到，则从仓库里面初始化</param>
        /// <returns></returns>
        public override TAggregateRoot GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(TAggregateRootKey key, Func<TAggregateRootKey, TAggregateRoot> getAggregate)
        {
            var type = this.QuerykeyType<TAggregateRootKey>();
            switch (type)
            {
                case 1:
                    {
                        var aggregateRootId = (Guid)Convert.ChangeType(key, typeof(Guid));
                        if (aggregateRootId == Guid.Empty)
                            return getAggregate == null ? default(TAggregateRoot) : getAggregate(key);

                        var value = GuidCountCache.GetValue(aggregateRootId);
                        var root = value as TAggregateRoot;
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            return root;
                        }

                        root = getAggregate == null ? default(TAggregateRoot) : getAggregate(key);
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            GuidCountCache.SetValue(aggregateRootId, root, this.CacheSpan());
                        }

                        return root;
                    }
                case 2:
                    {
                        var aggregateRootId = (string)Convert.ChangeType(key, typeof(string));
                        if (string.IsNullOrEmpty(aggregateRootId))
                            return getAggregate == null ? default(TAggregateRoot) : getAggregate(key);

                        var value = StringCountCache.GetValue(aggregateRootId);
                        var root = value as TAggregateRoot;
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            return root;
                        }

                        root = getAggregate == null ? default(TAggregateRoot) : getAggregate(key);
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            StringCountCache.SetValue(aggregateRootId, root, this.CacheSpan());
                        }

                        return root;
                    }
                case 3:
                    {
                        var aggregateRootId = (long)Convert.ChangeType(key, typeof(long));
                        if (aggregateRootId == 0)
                            return getAggregate == null ? default(TAggregateRoot) : getAggregate(key);

                        var value = LongCountCache.GetValue(aggregateRootId);
                        var root = value as TAggregateRoot;
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            return root;
                        }

                        root = getAggregate == null ? default(TAggregateRoot) : getAggregate(key);
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            LongCountCache.SetValue(aggregateRootId, root, this.CacheSpan());
                        }

                        return root;
                    }
                case 4:
                    {
                        var aggregateRootId = (int)Convert.ChangeType(key, typeof(int));
                        if (aggregateRootId == 0)
                            return getAggregate == null ? default(TAggregateRoot) : getAggregate(key);

                        var value = IntCountCache.GetValue(aggregateRootId);
                        var root = value as TAggregateRoot;
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            return root;
                        }

                        root = getAggregate == null ? default(TAggregateRoot) : getAggregate(key);
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            if (aggregateRootId != 0)
                                IntCountCache.SetValue(aggregateRootId, root, this.CacheSpan());
                        }

                        return root;
                    }
            }

            return default(TAggregateRoot);
        }

        #endregion context
    }
}