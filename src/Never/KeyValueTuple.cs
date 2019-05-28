namespace Never
{
    /// <summary>
    /// KeyValue对象
    /// </summary>
    /// <typeparam name="TKey">Key类型</typeparam>
    /// <typeparam name="TValue">Value类型</typeparam>
    public class KeyValueTuple<TKey, TValue>
    {
        /// <summary>
        /// Key
        /// </summary>
        public TKey Key { get; private set; }

        /// <summary>
        /// Value
        /// </summary>
        public TValue Value { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValueTuple{TKey, TValue}"/> class.
        /// </summary>
        protected KeyValueTuple()
            : this(default(TKey), default(TValue))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValueTuple{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public KeyValueTuple(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}