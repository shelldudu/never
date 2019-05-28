namespace Never.Aop
{
    /// <summary>
    /// 拦截接口
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// 在对方法进行调用前
        /// </summary>
        /// <param name="invocation">调用信息</param>
        void PreProceed(IInvocation invocation);

        /// <summary>
        /// 对方法进行调用后
        /// </summary>
        /// <param name="invocation">调用信息</param>
        void PostProceed(IInvocation invocation);
    }
}