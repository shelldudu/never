using System;
using System.Collections.Generic;

namespace Never.Utils
{
    /// <summary>
    /// 简单状态机扩展
    /// </summary>
    public static class SimpleStatusMachineExtension
    {
        /// <summary>
        /// 是否能切换：在状态机中只要满足一条即能切换
        /// </summary>
        /// <typeparam name="T">数据操作类型</typeparam>
        /// <param name="machine">状态机</param>
        /// <param name="current">当前状态</param>
        /// <param name="expect">期望状态</param>
        /// <returns></returns>
        public static ApiResult<bool> Can<T>(this SimpleStatusMachine<T> machine, T current, T expect) where T : struct, IConvertible
        {
            return SimpleStatusMachineHelper.Can<T>(machine, current, expect);
        }

        /// <summary>
        /// 是否能切换：在所有状态机中只要满足一条即能切换
        /// </summary>
        /// <typeparam name="T">数据操作类型</typeparam>
        /// <param name="machines">状态机</param>
        /// <param name="current">当前状态</param>
        /// <param name="expect">期望状态</param>
        /// <returns></returns>
        public static ApiResult<bool> Can<T>(this IEnumerable<SimpleStatusMachine<T>> machines, T current, T expect) where T : struct, IConvertible
        {
            return SimpleStatusMachineHelper.Can<T>(machines, current, expect);
        }

        /// <summary>
        /// 是否不能切换：在所有状态机中全部不满足
        /// </summary>
        /// <typeparam name="T">数据操作类型</typeparam>
        /// <param name="machine">状态机</param>
        /// <param name="current">当前状态</param>
        /// <param name="expect">期望状态</param>
        /// <returns></returns>
        public static ApiResult<bool> CanNot<T>(this SimpleStatusMachine<T> machine, T current, T expect) where T : struct, IConvertible
        {
            return SimpleStatusMachineHelper.CanNot<T>(machine, current, expect);
        }

        /// <summary>
        /// 是否不能切换：在所有状态机中全部不满足
        /// </summary>
        /// <typeparam name="T">数据操作类型</typeparam>
        /// <param name="machines">状态机</param>
        /// <param name="current">当前状态</param>
        /// <param name="expect">期望状态</param>
        /// <returns></returns>
        public static ApiResult<bool> CanNot<T>(this IEnumerable<SimpleStatusMachine<T>> machines, T current, T expect) where T : struct, IConvertible
        {
            return SimpleStatusMachineHelper.CanNot<T>(machines, current, expect);
        }
    }
}