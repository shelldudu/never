using Never.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never
{
    /// <summary>
    /// 匿名类
    /// </summary>
    public static partial class Anonymous
    {
        #region api

        /// <summary>
        /// 创建一个新的数据结果
        /// </summary>
        /// <typeparam name="T">结果对象</typeparam>
        /// <param name="status">结果状态</param>
        /// <param name="result">结果对象</param>
        /// <returns></returns>
        public static ApiResult<T> NewApiResult<T>(ApiStatus status, T result)
        {
            return new ApiResult<T>(status, result, string.Empty);
        }

        /// <summary>
        /// 创建一个新的数据结果
        /// </summary>
        /// <typeparam name="T">结果对象</typeparam>
        /// <param name="status">结果状态</param>
        /// <param name="result">结果对象</param>
        /// <param name="message">消息</param>
        public static ApiResult<T> NewApiResult<T>(ApiStatus status, T result, string message)
        {
            return new ApiResult<T>(status, result, message);
        }

        #endregion api

        #region page

        /// <summary>
        /// 创建一个新的数据结果
        /// </summary>
        /// <typeparam name="T">结果对象</typeparam>
        /// <param name="pageNow">当前页.</param>
        /// <param name="pageSize">分页大小.</param>
        /// <param name="records">结果集.</param>
        public static PagedData<T> NewPagedData<T>(int pageNow, int pageSize, IEnumerable<T> records)
        {
            return new PagedData<T>(pageNow, pageSize, records);
        }

        /// <summary>
        /// 创建一个新的数据结果
        /// </summary>
        /// <typeparam name="T">结果对象</typeparam>
        /// <param name="pageNow">当前页.</param>
        /// <param name="pageSize">分页大小.</param>
        /// <param name="totalCount">总条数.</param>
        /// <param name="records">结果集.</param>
        public static PagedData<T> NewPagedData<T>(int pageNow, int pageSize, int totalCount, IEnumerable<T> records)
        {
            return new PagedData<T>(pageNow, pageSize, totalCount, records);
        }

        /// <summary>
        /// 创建一个新的数据结果
        /// </summary>
        /// <typeparam name="T">结果对象</typeparam>
        /// <param name="search">分页条件.</param>
        public static PagedData<T> NewPagedData<T>(PagedSearch search)
        {
            return new PagedData<T>(search.PageNow, search.PageSize, 0, new T[] { });
        }

        /// <summary>
        /// 创建一个新的数据结果
        /// </summary>
        /// <typeparam name="T">结果对象</typeparam>
        /// <param name="search">分页条件.</param>
        /// <param name="records">结果集.</param>
        public static PagedData<T> NewPagedData<T>(PagedSearch search, IEnumerable<T> records)
        {
            return new PagedData<T>(search.PageNow, search.PageSize, records ?? new T[] { });
        }

        /// <summary>
        /// 创建一个新的数据结果
        /// </summary>
        /// <typeparam name="T">结果对象</typeparam>
        /// <param name="search">分页条件.</param>
        /// <param name="totalCount">总条数.</param>
        /// <param name="records">结果集.</param>
        public static PagedData<T> NewPagedData<T>(PagedSearch search, int totalCount, IEnumerable<T> records)
        {
            return new PagedData<T>(search.PageNow, search.PageSize, totalCount, records ?? new T[] { });
        }

        #endregion page

        #region 生成数组

        /// <summary>
        /// 创建一个新的Enumerable结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> NewEnumerable<T>()
        {
            return System.Linq.Enumerable.Empty<T>();
        }

        /// <summary>
        /// 创建一个新的Collection结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ICollection<T> NewCollection<T>(int capacity)
        {
            return new List<T>(capacity);
        }

        #endregion 生成数组

        #region new object

        /// <summary>
        /// 创造对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T NewObject<T>()
        {
            return MyObject<T>.Go();
        }

        /// <summary>
        /// 创造对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private struct MyObject<T>
        {
            public static Func<T> Go;

            static MyObject()
            {
                var type = typeof(T);
                var emit = EasyEmitBuilder<Func<T>>.NewDynamicMethod();
                var local = emit.DeclareLocal(type);
                if (type.IsValueType)
                {
                    emit.LoadLocalAddress(local);
                    emit.InitializeObject(type);
                    emit.LoadLocal(local);
                    emit.Return();
                    Go = emit.CreateDelegate();
                    return;
                }

                var ctors = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (var ctor in ctors)
                {
                    if (ctor.GetParameters().Length == 0)
                    {
                        emit.NewObject(ctor);
                        emit.StoreLocal(local);
                        emit.LoadLocal(local);
                        emit.Return();
                        Go = emit.CreateDelegate();
                        return;
                    }
                }

                emit.LoadConstant(type);
                emit.Call(typeof(Activator).GetMethod("CreateInstance", new[] { typeof(Type) }));
                emit.StoreLocal(local);
                emit.LoadLocal(local);
                emit.Return();
                Go = emit.CreateDelegate();
                return;
            }
        }

        #endregion new object
    }
}