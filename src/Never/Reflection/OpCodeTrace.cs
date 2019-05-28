using System.Reflection.Emit;

namespace Never.Reflection
{
    /// <summary>
    /// OpCode轨迹
    /// </summary>
    public struct OpCodeTrace
    {
        /// <summary>
        /// 当前OpCode
        /// </summary>
        public OpCode? OpCode { get; set; }

        /// <summary>
        /// 操作方法的名字
        /// </summary>
        public string MethodName { get; set; }
    }
}