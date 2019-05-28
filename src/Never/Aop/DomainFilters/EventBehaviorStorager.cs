using System;
using System.Collections.Generic;

namespace Never.Aop.DomainFilters
{
    /// <summary>
    /// 当前Event特性存储
    /// </summary>
    public sealed class EventBehaviorStorager
    {
        #region field

        /// <summary>
        /// 只有1个type的命令
        /// </summary>
        private readonly Dictionary<Type, IEnumerable<Attribute>> one = new Dictionary<Type, IEnumerable<Attribute>>();

        #endregion field

        #region add

        #region ctor

        private EventBehaviorStorager()
        {
        }

        #endregion ctor

        #region singleton

        /// <summary>
        /// 单例
        /// </summary>
        public static EventBehaviorStorager Default
        {
            get
            {
                if (Singleton<EventBehaviorStorager>.Instance == null)
                    Singleton<EventBehaviorStorager>.Instance = new EventBehaviorStorager();

                return Singleton<EventBehaviorStorager>.Instance;
            }
        }

        #endregion singleton

        /// <summary>
        /// Adds the specified EventType type.
        /// </summary>
        /// <paramm name="eventType">Type of the eventType.</paramm>
        /// <paramm name="attributes">The attributes.</paramm>
        public void Add(Type eventType, IEnumerable<Attribute> attributes)
        {
            if (eventType == null)
                return;

            if (!one.ContainsKey(eventType))
                one.Add(eventType, attributes);
        }

        #endregion add

        #region value

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <paramm name="eventType">Type of the eventType.</paramm>
        /// <returns></returns>
        public IEnumerable<Attribute> GetAttributes(Type eventType)
        {
            if (eventType == null)
                return new Attribute[0];

            IEnumerable<Attribute> result = null;
            if (one.TryGetValue(eventType, out result))
                return result;

            return new Attribute[0];
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparamm name="T"></typeparamm>
        /// <paramm name="eventType">Type of the eventType.</paramm>
        /// <returns></returns>
        public T GetAttribute<T>(Type eventType) where T : Attribute
        {
            var attributes = this.GetAttributes(eventType);
            foreach (var attribute in attributes)
            {
                var attr = attribute as T;
                if (attr != null)
                    return attr;
            }

            return default(T);
        }

        #endregion value
    }
}