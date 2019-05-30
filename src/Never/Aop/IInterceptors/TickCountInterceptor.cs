using System;

namespace Never.Aop.IInterceptors
{
    /// <summary>
    /// 使用操作系统计时监控主方法执行时间的拦截器
    /// </summary>
    public class TickCountInterceptor : StandardInterceptor, IInterceptor
    {
        /// <summary>
        /// 计时时间，以毫秒为单位
        /// </summary>
        public uint ElapsedMilliseconds { get; private set; }

        /// <summary>
        /// 在对方法进行调用前
        /// </summary>
        /// <param name="invocation"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public override void PreProceed(IInvocation invocation)
        {
            if (invocation.Items == null)
                return;

            invocation.Items["TickCountInterceptor"] = Never.Utils.MethodTickCount.GetTickCount();
        }

        /// <summary>
        /// 对方法进行调用后
        /// </summary>
        /// <param name="invocation"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public override void PostProceed(IInvocation invocation)
        {
            if (invocation.Items == null || !invocation.Items.ContainsKey("TickCountInterceptor"))
                return;

            var tickCount = (uint)invocation.Items["TickCountInterceptor"];
#if DEBUG
            if (invocation != null)
            {
                Console.WriteLine(string.Format("TC:执行代理 {0} 方法 {1} 耗时 {2} 毫秒", invocation.ProxyType.FullName, invocation.Method.Name, tickCount.ToString()));
            }
            else
            {
                Console.WriteLine(string.Format("TC:执行方法耗时 {0} 毫秒", tickCount.ToString()));
            }
#endif
        }
    }
}