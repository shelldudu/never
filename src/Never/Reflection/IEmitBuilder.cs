using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Never.Reflection
{
    /// <summary>
    /// 定义Emit操作接口
    /// </summary>
    public interface IEmitBuilder
    {
        /// <summary>
        /// 约定方法的调用
        /// </summary>
        CallingConventions CallingConventions { get; }

        /// <summary>
        /// 方法参数的类型
        /// </summary>
        Type[] ParameterTypes { get; }

        /// <summary>
        /// 返回的类型
        /// </summary>
        Type ReturnType { get; }

        /// <summary>
        /// IL指令
        /// </summary>
        ILGenerator IL { get; }

        /// <summary>
        /// 声明一个新临时变量
        /// </summary>
        /// <param name="localType">变量类型</param>
        /// <returns></returns>
        ILocal DeclareLocal(Type localType);

        /// <summary>
        /// 声明一个新标签
        /// </summary>
        /// <returns></returns>
        ILabel DefineLabel();
    }
}