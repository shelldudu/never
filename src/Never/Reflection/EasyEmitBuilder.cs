using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Never.Reflection
{
    /// <summary>
    /// 定义Emit操作接口，当前IL指令只有3个地方生成，
    /// <see cref="System.Reflection.Emit.DynamicMethod"/>,
    /// <see cref="System.Reflection.Emit.MethodBuilder"/>,
    /// <see cref="System.Reflection.Emit.ConstructorBuilder"/>
    /// </summary>
    /// <typeparam name="DelegateType">委托类型</typeparam>
    public class EasyEmitBuilder<DelegateType> : EmitBuilder, IEmitBuilder
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly DynamicMethod dynamicMethod = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyEmitBuilder{DelegateType}"/> class.
        /// </summary>
        /// <param name="dynamicMethod">动态方法名字</param>
        /// <param name="parameterTypes"></param>
        private EasyEmitBuilder(DynamicMethod dynamicMethod, Type[] parameterTypes)
            : base(dynamicMethod.GetILGenerator(), dynamicMethod.CallingConvention, dynamicMethod.ReturnType, parameterTypes)
        {
            this.dynamicMethod = dynamicMethod;
        }

        #endregion ctor

        #region dynamic method

        /// <summary>
        /// 创建一个新的动态方法
        /// </summary>
        /// <returns></returns>
        public static EasyEmitBuilder<DelegateType> NewDynamicMethod()
        {
            return NewDynamicMethod(string.Empty);
        }

        /// <summary>
        /// 创建一个新的动态方法
        /// </summary>
        /// <param name="name">方法名字</param>
        /// <returns></returns>
        public static EasyEmitBuilder<DelegateType> NewDynamicMethod(string name)
        {
            if (string.IsNullOrEmpty(name))
                name = IncrementName.NextMethodName("DynamicMethod");
            else if (IncrementName.Contain(name))
                throw new Exception(string.Format("dynamic method {0} had been defined", name));

            var delType = typeof(DelegateType);
            var invoke = delType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameters = invoke.GetParameters();
            var parameterTypes = new List<Type>(parameters.Length);
            foreach (var p in parameters)
                parameterTypes.Add(p.ParameterType);

            var ret = new EasyEmitBuilder<DelegateType>(new DynamicMethod(name, returnType, parameterTypes.ToArray(), EmitBuilder.ModuleBuilder, true), parameterTypes.ToArray());
            return ret;
        }

        /// <summary>
        /// 创建一个委托
        /// </summary>
        /// <returns></returns>
        public DelegateType CreateDelegate()
        {
            return (DelegateType)(object)(this.dynamicMethod.CreateDelegate(typeof(DelegateType)));
        }

        #endregion dynamic method

        #region dynamic type

        /// <summary>
        /// 声明一个TypeBuilder
        /// </summary>
        /// <param name="typeName">类名</param>
        /// <returns></returns>
        public static TypeBuilder NewTypeBuilder(string typeName)
        {
            return NewTypeBuilder(typeName, null, Type.EmptyTypes);
        }

        /// <summary>
        /// 声明一个TypeBuilder
        /// </summary>
        /// <param name="typeName">类名</param>
        /// <param name="parent">父类</param>
        /// <param name="interfaces">实现的接口</param>
        /// <returns></returns>
        public static TypeBuilder NewTypeBuilder(string typeName, Type parent, Type[] interfaces)
        {
            return NewTypeBuilder(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, parent, interfaces);
        }

        /// <summary>
        /// 声明一个TypeBuilder
        /// </summary>
        /// <param name="typeName">类名</param>
        /// <param name="parent">父类</param>
        /// <param name="attributes">类型特性</param>
        /// <param name="interfaces">实现的接口</param>
        /// <returns></returns>
        public static TypeBuilder NewTypeBuilder(string typeName, TypeAttributes attributes, Type parent, Type[] interfaces)
        {
            return NewTypeBuilder(string.Empty, typeName, attributes, parent, interfaces);
        }

        /// <summary>
        /// 声明一个TypeBuilder
        /// </summary>
        /// <param name="namespace">命名空间</param>
        /// <param name="typeName">类名</param>
        /// <returns></returns>
        public static TypeBuilder NewTypeBuilder(string @namespace, string typeName)
        {
            return NewTypeBuilder(@namespace, typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, null, Type.EmptyTypes);
        }

        /// <summary>
        /// 声明一个TypeBuilder
        /// </summary>
        /// <param name="namespace">命名空间</param>
        /// <param name="typeName">类名</param>
        /// <param name="parent">父类</param>
        /// <param name="interfaces">实现的接口</param>
        /// <returns></returns>
        public static TypeBuilder NewTypeBuilder(string @namespace, string typeName, Type parent, Type[] interfaces)
        {
            return NewTypeBuilder(@namespace, typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, parent, interfaces);
        }

        /// <summary>
        /// 声明一个TypeBuilder
        /// </summary>
        /// <param name="namespace">命名空间</param>
        /// <param name="typeName">类名</param>
        /// <param name="attributes">类型特性</param>
        /// <param name="parent">父类</param>
        /// <param name="interfaces">实现的接口</param>
        /// <returns></returns>
        public static TypeBuilder NewTypeBuilder(string @namespace, string typeName, TypeAttributes attributes, Type parent, Type[] interfaces)
        {
            if (string.IsNullOrEmpty(@namespace))
            {
                @namespace = "Never.CustomTypes.";
            }
            else
            {
                @namespace = @namespace.Trim('.');
                @namespace = string.Concat(@namespace, ".");
            }

            @typeName = @typeName.Trim('.');

            var fullTypeName = IncrementName.NextTypeName(@namespace, typeName.Trim('.').Replace("+", "_").Replace("<", "_").Replace(">", "_").Replace("~", "_"));
            return EmitBuilder.ModuleBuilder.DefineType(fullTypeName, attributes, parent, interfaces);
        }

        /// <summary>
        /// 创建类型
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <returns></returns>
        public static Type CreateTypeInfo(TypeBuilder typeBuilder)
        {
#if NET461
            return typeBuilder.CreateType();
#else
            return typeBuilder.CreateTypeInfo();
#endif
        }

        #endregion dynamic type
    }
}