namespace Never
{
    /// <summary>
    /// 常量Key
    /// </summary>
    public static class GlobalConstantSetting
    {
        #region serialize

        /// <summary>
        /// 序列化
        /// </summary>
        public static class SerializerKey
        {
            /// <summary>
            /// 二进制 序列化IoC注册的key
            /// </summary>
            public const string Binary = "ioc.ser.bin";

            /// <summary>
            /// DataContract序列化IoC注册的key
            /// </summary>
            public const string DataContract = "ioc.ser.data";

            /// <summary>
            /// js序列化IoC注册的key
            /// </summary>
            public const string JavaScript = "ioc.ser.java";

            /// <summary>
            /// soap序列化IoC注册的key
            /// </summary>
            public const string Soap = "ioc.ser.soap";

            /// <summary>
            /// xml序列化IoC注册的key
            /// </summary>
            public const string Xml = "ioc.ser.xml";

            /// <summary>
            /// Easy序列化IoC注册的key
            /// </summary>
            public const string EasyJson = "ioc.ser.easyj";

            /// <summary>
            /// jsonnet序列化IoC注册的key
            /// </summary>
            public const string JsonNet = "ioc.ser.jsonnet";
        }

        #endregion serialize

        #region cache

        /// <summary>
        /// Cache的key
        /// </summary>
        public static class CacheKey
        {
            #region cache

            /// <summary>
            /// 用HttpRuntime.Cache在注册中的key
            /// </summary>
            public const string HttpContext = "cache.httpcontext";

            /// <summary>
            /// 用HttpRuntime.Cache在注册中的key
            /// </summary>
            public const string HttpRuntime = "cache.httpruntime";

            /// <summary>
            /// 用Runtime.Caching.MemoryCache在注册中的key
            /// </summary>
            public const string MemoryCache = "cache.memory";

            /// <summary>
            /// 用CounterDict在注册中的key
            /// </summary>
            public const string CounterDict = "cache.counterdict";

            /// <summary>
            /// 用CounterDict在注册中的key
            /// </summary>
            public const string ConcurrentCouterDict = "cache.concouterdict";

            /// <summary>
            /// 用MemCached在注册中的key
            /// </summary>
            public const string MemCached = "cache.memcached";

            /// <summary>
            /// 用RedisCached在注册中的key
            /// </summary>
            public const string RedisCached = "cache.rediscached";

            /// <summary>
            /// 当前使用缓存类型对象的key
            /// </summary>
            private static string currentCacheKey = string.Empty;

            /// <summary>
            /// 当前使用缓存类型对象的key
            /// </summary>
            public static string CurrentCacheKey
            {
                get
                {
                    return string.IsNullOrEmpty(currentCacheKey) ? MemoryCache : currentCacheKey;
                }
                set
                {
                    currentCacheKey = value;
                }
            }

            #endregion cache
        }

        #endregion cache
    }
}