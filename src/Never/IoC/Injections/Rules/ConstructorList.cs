using System.Collections.Generic;

namespace Never.IoC.Injections.Rules
{
    /// <summary>
    /// 构造函数List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ConstructorList<T> : List<T>, IEnumerable<T>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        public ConstructorList(T value) : base(new[] { value })
        {
        }

        /// <summary>
        ///
        /// </summary>
        private ConstructorList() : base()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="values"></param>
        private ConstructorList(IEnumerable<T> values) : base(values)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static ConstructorList<T> CreateEmpty()
        {
            return new ConstructorList<T>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static ConstructorList<T> CreateArray(IEnumerable<T> array)
        {
            return new ConstructorList<T>(array);
        }
    }
}