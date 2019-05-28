using System;
using System.Diagnostics;

namespace Never.Aop.IInterceptors
{
    /// <summary>
    /// 使用stopwatch监控主方法执行时间的拦截器
    /// </summary>
    public class StopwatchInterceptor : StandardInterceptor, IInterceptor
    {
        /// <summary>
        /// 在对方法进行调用前
        /// </summary>
        /// <param name="invocation"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public override void PreProceed(IInvocation invocation)
        {
            if (invocation.Items == null)
                return;

            invocation.Items["StopwatchInterceptor"] = Stopwatch.StartNew();
        }

        /// <summary>
        /// 对方法进行调用后
        /// </summary>
        /// <param name="invocation"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public override void PostProceed(IInvocation invocation)
        {
            if (invocation.Items == null || !invocation.Items.ContainsKey("StopwatchInterceptor"))
                return;

            var stopwatch = invocation.Items["StopwatchInterceptor"] as Stopwatch;
            if (stopwatch == null)
                return;

#if DEBUG
            if (invocation != null)
            {
                Console.WriteLine(string.Format("SW:执行代理 {0} 方法 {1} 耗时 {2} 毫秒", invocation.ProxyType.FullName, invocation.Method.Name, stopwatch.ElapsedMilliseconds.ToString()));
            }
            else
            {
                Console.WriteLine(string.Format("SW:执行方法耗时 {0} 毫秒", stopwatch.ElapsedMilliseconds.ToString()));
            }

#endif

            stopwatch.Stop();
        }
    }
}