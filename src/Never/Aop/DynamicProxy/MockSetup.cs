using System.Reflection;

namespace Never.Aop.DynamicProxy
{
    /// <summary>
    /// 虚拟步骤
    /// </summary>
    public class MockSetup
    {
        #region prop

        /// <summary>
        /// 执行的方法
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// 字段
        /// </summary>
        public FieldInfo Field { get; set; }

        /// <summary>
        /// 事件
        /// </summary>
        public EventInfo Event { get; set; }

        /// <summary>
        /// 当前执行了哪个方法，有排序的，-1表示抛出异常，0表示执行了Base，10表示执行了Return(TResult)，10表示执行了Func(T, TResult)，11表示执行了Func(T,T1,TResult)等
        /// </summary>
        public int MethodIndex { get; set; }

        /// <summary>
        /// 执行回调方法中的参数
        /// </summary>
        public ParameterInfo[] CallbackMethodParameters { get; set; }

        /// <summary>
        /// 调用的哪种方法
        /// </summary>
        public MethodToCall MethodToCallType { get; set; }

        #endregion prop

        /// <summary>
        /// 方法执行类型
        /// </summary>
        public enum MethodToCall : byte
        {
            /// <summary>
            /// 执行的是异常方法
            /// </summary>
            Exception = 0,

            /// <summary>
            /// 执行没有返回值的方法
            /// </summary>
            Void = 1,

            /// <summary>
            /// 执行有返回值的方法
            /// </summary>
            Return = 2,
        }
    }
}