using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Never.Web.Mvc.Dispatcher
{
    /// <summary>
    /// 当前Action是否可不可进行消费
    /// </summary>
    internal static class ActionBehaviorStorager
    {
        #region field

        /// <summary>
        /// 只有1个type的命令
        /// </summary>
        private static readonly Dictionary<MethodInfo, IEnumerable<Attribute>> one = new Dictionary<MethodInfo, IEnumerable<Attribute>>();

        #endregion field

        #region add

        /// <summary>
        /// Adds the specified method type.
        /// </summary>
        /// <paramm name="handlerType">Type of the method.</paramm>
        /// <paramm name="attributes">The attributes.</paramm>
        internal static void Add(MethodInfo method, IEnumerable<Attribute> attributes)
        {
            if (method == null)
                return;

            if (!one.ContainsKey(method))
                one.Add(method, attributes);
        }

        #endregion add

        #region value

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <paramm name="handlerType">Type of the method.</paramm>
        /// <returns></returns>
        public static IEnumerable<Attribute> GetAttributes(MethodInfo method)
        {
            if (method == null)
                return new Attribute[0];

            IEnumerable<Attribute> result = null;
            if (one.TryGetValue(method, out result))
                return result;

            return new Attribute[0];
        }

        #endregion value

        #region value

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparamm name="T"></typeparamm>
        /// <paramm name="handlerType">Type of the method.</paramm>
        /// <returns></returns>
        public static T GetAttribute<T>(MethodInfo method) where T : Attribute
        {
            var attributes = ActionBehaviorStorager.GetAttributes(method);
            for (var i = 0; i < attributes.Count(); i++)
            {
                var attribute = attributes.ElementAt(i) as T;
                if (attribute != null)
                    return attribute;
            }

            return default(T);
        }

        #endregion value
    }
}