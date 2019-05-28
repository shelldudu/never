using System;
using System.Collections.Generic;

namespace Never.Aop.DomainFilters
{
    /// <summary>
    /// 当前handler行为储存
    /// </summary>
    public sealed class HandlerBehaviorStorager
    {
        #region field

        /// <summary>
        /// handlerype的特性
        /// </summary>
        private readonly Dictionary<Type, IEnumerable<Attribute>> handlerValues = null;

        /// <summary>
        /// handlerype执行方法的特性
        /// </summary>
        private readonly Dictionary<KeyValuePair<Type, Type>, IEnumerable<Attribute>> executingValues = null;

        #endregion field

        #region ctor

        private HandlerBehaviorStorager()
        {
            handlerValues = new Dictionary<Type, IEnumerable<Attribute>>();
            executingValues = new Dictionary<KeyValuePair<Type, Type>, IEnumerable<Attribute>>();
        }

        #endregion ctor

        #region singleton

        /// <summary>
        /// 单例
        /// </summary>
        public static HandlerBehaviorStorager Default
        {
            get
            {
                if (Singleton<HandlerBehaviorStorager>.Instance == null)
                    Singleton<HandlerBehaviorStorager>.Instance = new HandlerBehaviorStorager();

                return Singleton<HandlerBehaviorStorager>.Instance;
            }
        }

        #endregion singleton

        #region add

        /// <summary>
        /// Adds the specified handlerType type.
        /// </summary>
        /// <paramm name="handlerType">Type of the handlerType.</paramm>
        /// <paramm name="attributes">The attributes.</paramm>
        public void Add(Type handlerType, IEnumerable<Attribute> attributes)
        {
            if (handlerType == null)
                return;

            if (!handlerValues.ContainsKey(handlerType))
                handlerValues.Add(handlerType, attributes);
        }

        /// <summary>
        /// Adds the specified handlerType type.
        /// </summary>
        /// <paramm name="handlerType">Type of the handlerType.</paramm>
        /// <paramm name="paramType">Type of the event.</paramm>
        /// <paramm name="attributes">The attributes.</paramm>
        public void Add(Type handlerType, Type paramType, IEnumerable<Attribute> attributes)
        {
            if (handlerType == null || paramType == null)
                return;

            var key = new KeyValuePair<Type, Type>(handlerType, paramType);
            if (!executingValues.ContainsKey(key))
                executingValues.Add(key, attributes);
        }

        #endregion add

        #region value

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <paramm name="handlerType">Type of the handlerType.</paramm>
        /// <returns></returns>
        public IEnumerable<Attribute> GetAttributes(Type handlerType)
        {
            if (handlerType == null)
                return new Attribute[0];

            IEnumerable<Attribute> result = null;
            if (handlerValues.TryGetValue(handlerType, out result))
                return result;

            return new Attribute[0];
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <paramm name="handlerType">Type of the handlerType.</paramm>
        /// <paramm name="paramType">Type of the event.</paramm>
        /// <returns></returns>
        public IEnumerable<Attribute> GetAttributes(Type handlerType, Type paramType)
        {
            if (handlerType == null || paramType == null)
                return new Attribute[0];

            IEnumerable<Attribute> result = null;
            if (executingValues.TryGetValue(new KeyValuePair<Type, Type>(handlerType, paramType), out result))
                return result;

            return new Attribute[0];
        }

        #endregion value

        #region value

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparamm name="T"></typeparamm>
        /// <paramm name="handlerType">Type of the handlerType.</paramm>
        /// <returns></returns>
        public T GetAttribute<T>(Type handlerType) where T : Attribute
        {
            var attributes = this.GetAttributes(handlerType);
            foreach (var attribute in attributes)
            {
                var attr = attribute as T;
                if (attr != null)
                    return attr;
            }

            return default(T);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparamm name="T"></typeparamm>
        /// <paramm name="handlerType">Type of the handlerType.</paramm>
        /// <paramm name="paramType">Type of the event.</paramm>
        /// <returns></returns>
        public T GetAttribute<T>(Type handlerType, Type paramType) where T : Attribute
        {
            var attributes = this.GetAttributes(handlerType, paramType);
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