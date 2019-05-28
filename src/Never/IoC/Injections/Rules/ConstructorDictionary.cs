using System.Collections.Generic;

namespace Never.IoC.Injections.Rules
{
    /// <summary>
    /// 构造函数Dictionary
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    internal class ConstructorDictionary<K, V> : Dictionary<K, V>, IDictionary<K, V>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="k"></param>
        /// <param name="v"></param>
        public ConstructorDictionary(K k, V v) : base(new Dictionary<K, V>() { { k, v } })
        {
        }

        /// <summary>
        ///
        /// </summary>
        private ConstructorDictionary() : base()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static ConstructorDictionary<K, V> CreateEmpty()
        {
            return new ConstructorDictionary<K, V>();
        }
    }
}