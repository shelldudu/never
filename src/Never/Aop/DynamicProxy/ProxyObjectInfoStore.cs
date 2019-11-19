using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never.Aop.DynamicProxy
{
    /// <summary>
    /// 代理对象信息保存
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class ProxyObjectInfoStore
    {
        #region invocation

        /// <summary>
        /// 调用信息
        /// </summary>
        public class Invocation : IInvocation
        {
            #region IInvocation

            /// <summary>
            /// 代理的参数
            /// </summary>
            public KeyValuePair<string, object>[] Arguments { get; private set; }

            /// <summary>
            /// 泛型方法参数类型
            /// </summary>
            public Type[] GenericArguments { get; private set; }

            /// <summary>
            /// 代理的对象所实现的接口
            /// </summary>
            public Type[] Interfaces { get; set; }

            /// <summary>
            /// 代理的方法
            /// </summary>
            public MethodInfo Method { get; set; }

            /// <summary>
            /// 该方法的特性，不会返回空对象
            /// </summary>
            public Attribute[] MethodAttributes { get; set; }

            /// <summary>
            /// 代理的对象
            /// </summary>
            public object Proxy { get; private set; }

            /// <summary>
            /// 该代理的特性，不会返回空对象
            /// </summary>
            public Attribute[] ProxyAttributes { get; set; }

            /// <summary>
            /// 代理的对象类型
            /// </summary>
            public Type ProxyType { get; set; }

            /// <summary>
            /// 上下文
            /// </summary>
            public IDictionary<string, object> Items { get; set; }

            #endregion IInvocation

            #region copy

            /// <summary>
            /// 复制调用信息
            /// </summary>
            /// <param name="proxy">代理的对象</param>
            /// <param name="arguments">代理的参数</param>
            /// <param name="genericArguments">泛型方法参数类型</param>
            /// <returns></returns>
            public Invocation NewObject(object proxy, List<KeyValuePair<string, object>> arguments, List<Type> genericArguments)
            {
                return new Invocation()
                {
                    Arguments = arguments == null ? new KeyValuePair<string, object>[0] : arguments.ToArray(),
                    GenericArguments = genericArguments == null ? new Type[0] : genericArguments.ToArray(),
                    Interfaces = this.Interfaces,
                    Method = this.Method,
                    MethodAttributes = this.MethodAttributes,
                    ProxyAttributes = this.ProxyAttributes,
                    ProxyType = this.ProxyType,
                    Proxy = proxy,
                    Items = new System.Collections.Concurrent.ConcurrentDictionary<string, object>()
                };
            }

            /// <summary>
            /// 复制调用中参数的信息
            /// </summary>
            /// <param name="invocation">The invocation.</param>
            /// <param name="arguments">The arguments.</param>
            /// <returns></returns>
            public Invocation CopyArguments(Invocation invocation, List<KeyValuePair<string, object>> arguments)
            {
                invocation.Arguments = arguments == null ? new KeyValuePair<string, object>[0] : arguments.ToArray();
                return invocation;
            }

            #endregion copy

            #region convert

            /// <summary>
            /// 获取方法特性
            /// </summary>
            /// <param name="method">The method.</param>
            /// <returns></returns>
            public static Attribute[] GetAttributes(System.Reflection.MethodInfo method)
            {
                var list = method.GetCustomAttributes(true);
                var attributes = new List<Attribute>(list.Length);
                foreach (var i in list)
                    attributes.Add((Attribute)i);

                return attributes.ToArray();
            }

            /// <summary>
            /// 获取方法特性
            /// </summary>
            /// <param name="sourceType">The sourceType.</param>
            /// <returns></returns>
            public static Attribute[] GetAttributes(System.Type sourceType)
            {
                var list = sourceType.GetCustomAttributes(true);
                var attributes = new List<Attribute>(list.Length);
                foreach (var i in list)
                    attributes.Add((Attribute)i);

                return attributes.ToArray();
            }

            #endregion convert
        }

        #endregion invocation

        #region field

        /// <summary>
        /// 当前增量
        /// </summary>
        private static int factor = 0;

        /// <summary>
        /// 某个方法调用的队列
        /// </summary>
        private static SortedDictionary<int, Invocation> invocationQueue = new SortedDictionary<int, Invocation>();

        #endregion field

        #region query

        /// <summary>
        /// 查询调用信息
        /// </summary>
        /// <param name="index">构建代理的索引</param>
        /// <returns></returns>
        public static Invocation Query(int index)
        {
            return invocationQueue[index];
        }

        /// <summary>
        /// 压进队列里
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns></returns>
        public static int Enqueue(Invocation invocation)
        {
            var fac = System.Threading.Interlocked.Increment(ref factor);
            invocationQueue[fac] = invocation;
            return fac;
        }

        #endregion query
    }
}