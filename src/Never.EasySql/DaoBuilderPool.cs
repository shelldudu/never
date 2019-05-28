using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// dao构建者
    /// </summary>
    public static class DaoBuilderPool
    {
        /// <summary>
        /// 保存列表
        /// </summary>
        private static readonly ConcurrentBag<BaseDaoBuilder> builders = new ConcurrentBag<BaseDaoBuilder>();

        /// <summary>
        /// 弹出一个DaoBuilder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static T Peek<T>(Func<T> builder) where T : BaseDaoBuilder
        {
            var first = builders.FirstOrDefault(ta => ta is T);
            if (first != null)
            {
                return (T)first;
            }

            lock (typeof(T))
            {
                first = builders.FirstOrDefault(ta => ta is T);
                if (first != null)
                {
                    return (T)first;
                }

                var value = builder();
                builders.Add(value);
                return value;
            }
        }
    }
}
