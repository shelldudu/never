namespace Never.Aop
{
    /// <summary>
    /// 拦截器编译配置
    /// </summary>
    public struct InterceptCompileSetting
    {
        /// <summary>
        /// 对参数进行装箱，默认为false
        /// </summary>
        public bool BoxArgument { get; set; }

        /// <summary>
        /// 是否存储参数，默认为false，因为参数不确定性，会影响性能，所以没有必要所有代理均需要获取参数
        /// </summary>
        public bool StoreArgument { get; set; }

        /// <summary>
        /// 没有调用信息
        /// </summary>
        public bool NoInvocation { get; set; }
    }
}