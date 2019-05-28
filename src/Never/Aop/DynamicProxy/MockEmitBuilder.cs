using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Never.Aop.DynamicProxy
{
    /// <summary>
    /// 定义Emit操作接口
    /// </summary>
    public class MockEmitBuilder : Never.Reflection.EmitBuilder
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MockEmitBuilder"/> class.
        /// </summary>
        /// <param name="il">il</param>
        /// <param name="callConvention"></param>
        /// <param name="returnType"></param>
        /// <param name="parameterTypes"></param>
        public MockEmitBuilder(ILGenerator il, CallingConventions callConvention, Type returnType, Type[] parameterTypes)
            : base(il, callConvention, returnType, parameterTypes)
        {
        }

        #endregion ctor

        #region init

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Type[] Convert(ParameterInfo[] parameters)
        {
            var parameterTypes = new List<Type>(parameters.Length);
            foreach (var parameter in parameters)
            {
                parameterTypes.Add(parameter.ParameterType);
            }

            return parameterTypes.ToArray();
        }

        /// <summary>
        /// 获取方法的参数类型
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Type[] Convert(MethodInfo method)
        {
            var parameters = method.GetParameters();
            var parameterTypes = new List<Type>(parameters.Length);
            foreach (var parameter in parameters)
            {
                parameterTypes.Add(parameter.ParameterType);
            }

            return parameterTypes.ToArray();
        }

        #endregion init

        #region support

        /// <summary>
        /// 支持同一命名空间下定义对象的访问
        /// </summary>
        internal static void SupportInternalDefinedObject()
        {
            AppDomain.CurrentDomain.TypeResolve += (o, s) =>
            {
                return ModuleBuilder.Assembly;
            };
        }

        #endregion support
    }
}