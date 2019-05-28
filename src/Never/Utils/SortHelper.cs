using System;
using System.Collections.Generic;

namespace Never.Utils
{
    /// <summary>
    /// 排序方法
    /// </summary>
    public static class SortHelper
    {
        #region 冒泡排序

        /// <summary>
        /// 冒泡排序，该方法需要传递实现接口ISortCompartion对象
        /// </summary>
        /// <typeparam name="T">排序类型对象</typeparam>
        /// <param name="source">要排序的数组</param>
        /// <param name="compartion">排序接口对象</param>
        public static void BubbleSort<T>(T[] source, Func<T, T, bool> compartion)
        {
            if (compartion == null)
                return;

            for (int i = 0; i < source.Length; i++)
            {
                for (int j = i + 1; j < source.Length; j++)
                {
                    if (compartion(source[i], source[j]))
                    {
                        T temp = source[i];
                        source[i] = source[j];
                        source[j] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// 冒泡排序，该方法需要传递实现接口ISortCompartion对象
        /// </summary>
        /// <typeparam name="T">排序类型对象</typeparam>
        /// <param name="source">要排序的数组</param>
        /// <param name="compartion">排序委托对象</param>
        public static IEnumerable<T> Sort<T>(IEnumerable<T> source, Func<T, T, bool> compartion)
        {
            if (compartion == null || source == null)
                return source;

            var array = System.Linq.Enumerable.ToArray(source);
            BubbleSort<T>(array, compartion);
            return array;
        }

        #endregion 冒泡排序
    }
}