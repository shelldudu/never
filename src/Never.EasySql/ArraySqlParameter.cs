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
    internal class ArraySqlParameter<T> : EasySqlParameter<T>, IEnumerable<T>, ISqlParameterEnumerable
    {
        public IEnumerable<T> Array { get; set; }

        public ArraySqlParameter(IEnumerable<T> array) : base(array)
        {
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
            return new List<KeyValueTuple<string, object>>().AsReadOnly();
        }

        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return (this.Array ?? Enumerable.Empty<T>()).GetEnumerator();
        }

        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this.Array ?? Enumerable.Empty<T>()).GetEnumerator();
        }
    }
}