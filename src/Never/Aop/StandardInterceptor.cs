namespace Never.Aop
{
    /// <summary>
    /// 基本的拦截器
    /// </summary>
    public class StandardInterceptor : IInterceptor
    {
        /// <summary>
        /// 在对方法进行调用前
        /// </summary>
        /// <param name="invocation"></param>
        public virtual void PreProceed(IInvocation invocation)
        {
        }

        /// <summary>
        /// 对方法进行调用后
        /// </summary>
        /// <param name="invocation"></param>
        public virtual void PostProceed(IInvocation invocation)
        {
        }
    }
}