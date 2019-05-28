namespace Never.IoC.Injections
{
    /// <summary>
    /// 代理注册规则
    /// </summary>
    public interface IShadowProxyRegisterRule
    {
        /// <summary>
        /// 构造函数参数
        /// </summary>
        /// <typeparam name="TInterceptor">拦截器类型</typeparam>
        /// <param name="key">注册key</param>
        /// <returns></returns>
        IShadowProxyRegisterRule WithInterceptor<TInterceptor>(string key) where TInterceptor : Never.Aop.IInterceptor;
    }
}