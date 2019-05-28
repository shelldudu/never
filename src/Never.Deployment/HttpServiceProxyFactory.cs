using Never.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Never.Deployment
{
    /// <summary>
    /// 创建代理，目前只支持接口类型
    /// </summary>
    public class HttpServiceProxyFactory
    {
        /// <summary>
        /// 回调时候的事件
        /// </summary>
        /// <seealso cref="System.EventArgs" />
        public class OnCallingEventArgs : EventArgs
        {
            /// <summary>
            /// 路由
            /// </summary>
            public string Route { get; set; }

            /// <summary>
            /// url
            /// </summary>
            public UrlConcat Url { get; set; }

            /// <summary>
            /// httpget,httppost
            /// </summary>
            public string HttpMethod { get; set; }

            /// <summary>
            /// 路由提供者
            /// </summary>
            public IApiUriDispatcher Provider { get; set; }

            /// <summary>
            /// json序列提供者
            /// </summary>
            public IJsonSerializer JsonSerializer { get; set; }

            /// <summary>
            /// 请求参数
            /// </summary>
            public object Request { get; set; }

            /// <summary>
            /// 返回类型
            /// </summary>
            public Type ReturnType { get; set; }
        }

        /// <summary>
        /// 字典缓存
        /// </summary>
        private static ConcurrentDictionary<Type, Type> sort = new ConcurrentDictionary<Type, Type>();

        /// <summary>
        /// 创建代理
        /// </summary>
        /// <param name="type"></param>
        /// <param name="provider"></param>
        /// <param name="jsonSerializer"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Type CreateProxy(Type type, Func<IApiUriDispatcher> provider, IJsonSerializer jsonSerializer, Action<OnCallingEventArgs> callback = null)
        {
            if (sort.TryGetValue(type, out var target))
            {
                return target;
            }

            var methods = typeof(HttpServiceProxyFactory).GetMethods();
            foreach (var method in methods)
            {
                if (method.Name == "CreateProxy" && method.IsGenericMethod)
                {
                    var @delegate = (Func<Func<IApiUriDispatcher>, IJsonSerializer, Action<OnCallingEventArgs>, Type>)Delegate.CreateDelegate(typeof(Func<Func<IApiUriDispatcher>, IJsonSerializer, Action<OnCallingEventArgs>, Type>), method.MakeGenericMethod(type));
                    return @delegate(provider, jsonSerializer, callback);
                }
            }

            throw new KeyNotFoundException("找不到CreateProxy方法");
        }

        /// <summary>
        /// 创建代理
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="jsonSerializer"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Type CreateProxy<T>(Func<IApiUriDispatcher> provider, IJsonSerializer jsonSerializer, Action<OnCallingEventArgs> callback = null) where T : class
        {
            if (sort.TryGetValue(typeof(T), out var target))
            {
                return target;
            }

            HttpServiceTypeProxyBuilder<T>.Provider = provider;
            HttpServiceTypeProxyBuilder<T>.JsonSerializer = jsonSerializer ?? SerializeEnvironment.JsonSerializer;
            HttpServiceTypeProxyBuilder<T>.Callback = callback;

            lock (sort)
            {
                target = HttpServiceTypeProxyBuilder<T>.Build();
                sort.TryAdd(typeof(T), target);
                return target;
            }
        }
    }
}