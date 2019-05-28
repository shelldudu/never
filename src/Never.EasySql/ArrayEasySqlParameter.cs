using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 数组参数
    /// </summary>
    internal class ArrayEasySqlParameter<T, V> : EasySqlParameter<T>, IEnumerable<V>, ISqlParameterEnumerable
    {
        public T Value { get; set; }
        public IEnumerable<V> Array { get; set; }

        public ArrayEasySqlParameter(T value, IEnumerable<V> array) : base(value)
        {
            this.Value = value;
            this.Array = array;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="target"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected override IReadOnlyList<KeyValueTuple<string, object>> Convert(T target, Type objectType)
        {
            return base.Convert(target, objectType);
        }

        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<V> GetEnumerator()
        {
            return (this.Array ?? Enumerable.Empty<V>()).GetEnumerator();
        }

        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this.Array ?? Enumerable.Empty<V>()).GetEnumerator();
        }
    }
}