using System.Collections.Generic;

namespace Never.Reflection
{
    /// <summary>
    /// 组合与自动数据增加的名字
    /// </summary>
    public static class IncrementName
    {
        #region field

        /// <summary>
        /// 当前增量
        /// </summary>
        private static long factor = 0;

        /// <summary>
        /// 缓存的字典
        /// </summary>
        private static readonly IDictionary<string, string> state = new Dictionary<string, string>();

        #endregion field

        #region

        /// <summary>
        /// 移动到一下名字
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string NextMethodName(string key)
        {
            string result = string.Empty;
            if (state.TryGetValue(key, out result))
                return result;

            System.Threading.Interlocked.Increment(ref factor);
            result = string.Format("DynamicMethod{0}{1}", factor.ToString(), key);
            state[key] = result;
            return result;
        }

        /// <summary>
        /// 移动到一下名字
        /// </summary>
        /// <param name="namespace">命名空间</param>
        /// <param name="typeName">类名</param>
        /// <returns></returns>
        public static string NextTypeName(string @namespace, string typeName)
        {
            string key = string.Concat(@namespace, typeName);
            string result = string.Empty;
            if (state.TryGetValue(key, out result))
                return result;

            System.Threading.Interlocked.Increment(ref factor);
            result = string.Format("{0}DynamicType{1}{2}", @namespace, factor.ToString(), typeName);
            state[key] = result;
            return result;
        }

        /// <summary>
        /// 是否包含了该Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Contain(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            return state.ContainsKey(key);
        }

        #endregion
    }
}