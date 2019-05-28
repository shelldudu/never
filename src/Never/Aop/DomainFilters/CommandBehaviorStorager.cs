using System;
using System.Collections.Generic;

namespace Never.Aop.DomainFilters
{
    /// <summary>
    /// 当前Command特性存储
    /// </summary>
    public sealed class CommandBehaviorStorager
    {
        #region field

        /// <summary>
        /// 只有1个type的命令
        /// </summary>
        private readonly Dictionary<Type, IEnumerable<Attribute>> keyValuePairs = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        private CommandBehaviorStorager()
        {
            keyValuePairs = new Dictionary<Type, IEnumerable<Attribute>>();
        }

        #endregion ctor

        #region singleton

        /// <summary>
        /// 单例
        /// </summary>
        public static CommandBehaviorStorager Default
        {
            get
            {
                if (Singleton<CommandBehaviorStorager>.Instance == null)
                    Singleton<CommandBehaviorStorager>.Instance = new CommandBehaviorStorager();

                return Singleton<CommandBehaviorStorager>.Instance;
            }
        }

        #endregion singleton

        #region add

        /// <summary>
        /// Adds the specified commandType type.
        /// </summary>
        /// <paramm name="commandType">Type of the commandType.</paramm>
        /// <paramm name="attributes">The attributes.</paramm>
        public void Add(Type command, IEnumerable<Attribute> attributes)
        {
            if (command == null)
                return;

            if (!keyValuePairs.ContainsKey(command))
                keyValuePairs.Add(command, attributes);
        }

        #endregion add

        #region value

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <paramm name="commandType">Type of the commandType.</paramm>
        /// <returns></returns>
        public IEnumerable<Attribute> GetAttributes(Type command)
        {
            if (command == null)
                return new Attribute[0];

            IEnumerable<Attribute> result = null;
            if (keyValuePairs.TryGetValue(command, out result))
                return result;

            return new Attribute[0];
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparamm name="T"></typeparamm>
        /// <paramm name="commandType">Type of the commandType.</paramm>
        /// <returns></returns>
        public T GetAttribute<T>(Type command) where T : Attribute
        {
            var attributes = this.GetAttributes(command);
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