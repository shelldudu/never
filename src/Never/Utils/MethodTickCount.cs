using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Never.Utils
{
    /// <summary>
    /// 代码执行时间简单计算
    /// </summary>
    public sealed class MethodTickCount : IDisposable
    {
        #region 性能监督

        /// <summary>
        /// 引入外部dll本地类
        /// </summary>
        private static class UnsafeNativeMethods
        {
            /// <summary>
            /// 获取一个启动到当前所经过的毫秒数
            /// </summary>
            /// <returns></returns>
            [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern uint GetTickCount();
        }

        /// <summary>
        /// 获取一个启动到当前所经过的毫秒数
        /// </summary>
        /// <returns></returns>
        public static uint GetTickCount()
        {
            return UnsafeNativeMethods.GetTickCount();
        }

        #endregion 性能监督

        #region 类似用于进行去运算时性能计时

        /// <summary>
        /// 开始时间
        /// </summary>
        private long startTime;

        /// <summary>
        /// 描述文字回调
        /// </summary>
        private Action<string> textcall;

        /// <summary>
        /// GC回收次数
        /// </summary>
        private int collectionCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodTickCount"/> class.
        /// </summary>
        /// <param name="textcall">检查名字</param>
        public MethodTickCount(Action<string> textcall)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            this.textcall = textcall;

            this.collectionCount = GC.CollectionCount(0);
            this.startTime = Stopwatch.GetTimestamp();
        }

        #endregion 类似用于进行去运算时性能计时

        #region IDisposable成员

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void Dispose()
        {
            string result = string.Format(
                "{0} seconds (GCs={1})",
                ((Stopwatch.GetTimestamp() - this.startTime) / (double)Stopwatch.Frequency).ToString(),
                (GC.CollectionCount(0) - this.collectionCount).ToString());

            this.textcall(result);
        }

        #endregion IDisposable成员
    }
}