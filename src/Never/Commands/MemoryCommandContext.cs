using Never.Caching;
using Never.Domains;
using Never.Exceptions;
using System;

namespace Never.Commands
{
    /// <summary>
    /// 内存模式使用的聚合根保存，使用事件流方式，如需使用仓库行为，请使用<see cref="DefaultCommandContext"/>
    /// </summary>
    public class MemoryCommandContext : DefaultCommandContext, ICommandContext
    {
        #region prop

        /// <summary>
        /// Guid聚合根字典
        /// </summary>
        protected static readonly CounterDictCache<Guid, IAggregateRoot> GuidCountCache = null;

        /// <summary>
        /// long聚合根字典
        /// </summary>
        protected static readonly CounterDictCache<long, IAggregateRoot> LongCountCache = null;

        /// <summary>
        /// int聚合根字典
        /// </summary>
        protected static readonly CounterDictCache<int, IAggregateRoot> IntCountCache = null;

        /// <summary>
        /// string聚合根字典
        /// </summary>
        protected static readonly CounterDictCache<string, IAggregateRoot> StringCountCache = null;

        #endregion prop

        #region ctor

        static MemoryCommandContext()
        {
            /*一个系统如果有上w个聚合根一起活动已经很厉害了*/
            int capacity = 10000;

            /*guid使用最多，所以可以是4W,int使用最少，用200个*/
            GuidCountCache = new CounterDictCache<Guid, IAggregateRoot>(capacity * 4);
            LongCountCache = new CounterDictCache<long, IAggregateRoot>(500);
            IntCountCache = new CounterDictCache<int, IAggregateRoot>(200);
            StringCountCache = new CounterDictCache<string, IAggregateRoot>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCommandContext"/> class.
        /// </summary>
        public MemoryCommandContext()
        {
        }

        #endregion ctor

        #region context

        /// <summary>
        /// 分析Type的类型,1为guid,2为string,3为long,4为int
        /// </summary>
        /// <typeparam name="TAggregateRootKey">聚合根标识Id，目前只有Guid,long,string,int四种</typeparam>
        /// <returns></returns>
        protected virtual int QuerykeyType<TAggregateRootKey>()
        {
            var type = typeof(TAggregateRootKey);

            if (type == typeof(Guid))
                return 1;

            if (type == typeof(string))
                return 2;

            if (type == typeof(long))
                return 3;

            if (type == typeof(int))
                return 4;

            throw new InvalidException("当前系统只支持guid,string,long,int这四种聚合根Id设计");
        }

        /// <summary>
        /// 获取聚合根，仅支持【事件流的内存模式】
        /// </summary>
        /// <typeparam name="TAggregateRootKey">聚合根标识Id，目前只有Guid,long,string,int四种</typeparam>
        /// <typeparam name="TAggregateRoot">聚合根</typeparam>
        /// <param name="key">内存模式下的key</param>
        /// <returns></returns>
        public override TAggregateRoot GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(TAggregateRootKey key)
        {
            var type = this.QuerykeyType<TAggregateRootKey>();
            switch (type)
            {
                case 1:
                    {
                        var aggregateRootId = (Guid)Convert.ChangeType(key, typeof(Guid));
                        if (aggregateRootId == Guid.Empty)
                            return default(TAggregateRoot);

                        var value = GuidCountCache.GetValue(aggregateRootId);
                        var root = value as TAggregateRoot;
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            return root;
                        }

                        return default(TAggregateRoot);
                    }
                case 2:
                    {
                        var aggregateRootId = (string)Convert.ChangeType(key, typeof(string));
                        if (string.IsNullOrEmpty(aggregateRootId))
                            return default(TAggregateRoot);

                        var value = StringCountCache.GetValue(aggregateRootId);
                        var root = value as TAggregateRoot;
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            return root;
                        }

                        return default(TAggregateRoot);
                    }
                case 3:
                    {
                        var aggregateRootId = (long)Convert.ChangeType(key, typeof(long));
                        if (aggregateRootId == 0)
                            return default(TAggregateRoot);

                        var value = LongCountCache.GetValue(aggregateRootId);
                        var root = value as TAggregateRoot;
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            return root;
                        }

                        return default(TAggregateRoot);
                    }
                case 4:
                    {
                        var aggregateRootId = (int)Convert.ChangeType(key, typeof(int));
                        if (aggregateRootId == 0)
                            return default(TAggregateRoot);

                        var value = IntCountCache.GetValue(aggregateRootId);
                        var root = value as TAggregateRoot;
                        if (root != null)
                        {
                            LifetimeScopeQueue.Enqueue(root);
                            return root;
                        }

                        return default(TAggregateRoot);
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
        public override TAggregateRoot GetAggregateRoot<TAggregateRootKey, TAggregateRoot>(TAggregateRootKey key, Func<TAggregateRoot> getAggregate)
        {
            throw new NotImplementedException("内存模式不支持仓库方法执行");
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
            throw new NotImplementedException("内存模式不支持仓库方法执行");
        }

        #endregion context

        #region cache

        /// <summary>
        /// 放在缓存中的时间间隔
        /// </summary>
        /// <returns></returns>
        protected virtual TimeSpan CacheSpan()
        {
            return TimeSpan.FromMinutes(10);
        }

        #endregion cache
    }
}