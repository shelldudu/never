using Never.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Never.Aop.DynamicProxy.Builders
{
    /// <summary>
    /// 构建T对象的代理
    /// </summary>
    /// <typeparam name="Target"></typeparam>
    public class ProxyMockBuilder<Target> : ProxyMockBuilder
    {
        #region function

        /// <summary>
        /// 构建函数
        /// </summary>
        private static Func<Target, IInterceptor[], Target> func = null;

        /// <summary>
        /// 目标类型
        /// </summary>
        private readonly Type TargetType = typeof(Target);

        #endregion function

        #region register

        /// <summary>
        /// 注册构建行为
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="setting">配置</param>
        /// <param name="interceptors">所实现的拦截器接口</param>
        /// <returns></returns>
        public static Func<Target, IInterceptor[], Target> Register(Target target, InterceptCompileSetting setting, Type[] interceptors)
        {
            if (func != null)
                return func;

            return func = new ProxyMockBuilder<Target>().Build(target, setting, interceptors);
        }

        #endregion register

        #region supportType

        /// <summary>
        /// 支持的类型
        /// </summary>
        /// <param name="type">The type.</param>
        public override void SupportType(Type type)
        {
            /*不可见，则是private或者protected或者internal的*/
            if (!type.IsVisible)
                throw new ArgumentException(string.Format("mock need the visible type,and {0} is not visible", type.FullName));

            /*支持公开的接口*/
            if (type.IsInterface)
                return;

            /*支持非密封类*/
            if (type.IsClass && !type.IsSealed)
                return;

            throw new ArgumentException(string.Format("mock need the public and not sealed class type or public interface,and {0} is not passed", type.FullName));
        }

        #endregion supportType

        #region build

        /// <summary>
        /// 构建非洋葱类型(则不是接口，类）等对象
        /// </summary>
        /// <param name="object">代理类</param>
        /// <returns></returns>
        protected virtual Func<Target, IInterceptor[], Target> BuildNotCepaType(Target @object)
        {
            var type = typeof(Target);
            if (this.IsPrimitiveOrInsideHandleType(type) || this.IsEnumType(type))
            {
                return new Func<Target, IInterceptor[], Target>((x, y) => { return default(Target); });
            }

            if (this.IsNullablePrimitiveOrInsideHandleType(type) || this.IsNullableEnumType(type))
            {
                return new Func<Target, IInterceptor[], Target>((x, y) => { return default(Target); });
            }

            if (type.IsArray)
            {
                return new Func<Target, IInterceptor[], Target>((x, y) => { return default(Target); });
            }

            if (this.IsAssignableFrom(type, typeof(IEnumerable)))
            {
                return new Func<Target, IInterceptor[], Target>((x, y) => { return default(Target); });
            }

            if (this.IsType(type))
            {
                return new Func<Target, IInterceptor[], Target>((x, y) => { return default(Target); });
            }

            if (this.IsAssignableFrom(type, typeof(Exception)))
            {
                return new Func<Target, IInterceptor[], Target>((x, y) => { return default(Target); });
            }

            if (type.IsValueType)
            {
                return new Func<Target, IInterceptor[], Target>((x, y) => { return default(Target); });
            }

            return null;
        }

        /// <summary>
        /// 进行构建
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="setting">配置</param>
        /// <param name="interceptors">所实现的拦截器接口</param>
        /// <returns></returns>
        public Func<Target, IInterceptor[], Target> Build(Target target, InterceptCompileSetting setting, Type[] interceptors)
        {
            var build = this.BuildNotCepaType(target);
            if (build != null)
                return build;

            var newType = default(Type);
            /*构建接口，较为容易*/
            if (this.TargetType.IsInterface)
            {
                newType = BuildInterface(this.TargetType, typeof(Target), interceptors, setting);
                var interfaceEmit = EasyEmitBuilder<Func<Target, IInterceptor[], Target>>.NewDynamicMethod();
                interfaceEmit.LoadArgument(0);
                interfaceEmit.LoadArgument(1);
                interfaceEmit.NewObject(newType.GetConstructor(new[] { typeof(Target), typeof(IInterceptor[]) }));
                interfaceEmit.Return();
                return interfaceEmit.CreateDelegate();
            }

            newType = BuildClass(target.GetType(), interceptors, setting);
            var classEmit = EasyEmitBuilder<Func<Target, IInterceptor[], Target>>.NewDynamicMethod();
            //classEmit.LoadArgument(0);
            classEmit.LoadArgument(1);
            classEmit.NewObject(newType.GetConstructor(new[] { typeof(IInterceptor[]) }));
            classEmit.Return();

            return classEmit.CreateDelegate();
        }

        /// <summary>
        /// 构建构造函数【用于代理接口】，因为IoC注入和Proxy使用的方式是不同的，Proxy可能直接用数组去构造，而IoC只能一个一个去注入
        /// </summary>
        /// <param name="typeBuilder">类型构造</param>
        /// <param name="sourceTypeOrInterfaceType">代理（接口或类）类型</param>
        /// <param name="sourceType">目标类型</param>
        /// <param name="interceptors">拦截器集合</param>
        /// <param name="sourceObjectFieldBuilder">目标类型构建</param>
        /// <param name="interceptorFieldBuilder">拦截器构建，被定义为List接口</param>
        protected override void DefineConstructor(TypeBuilder typeBuilder, Type sourceTypeOrInterfaceType, Type sourceType, Type[] interceptors, FieldBuilder sourceObjectFieldBuilder, FieldBuilder interceptorFieldBuilder)
        {
            /*构造函数*/
            var ctorParamters = new List<Type>(2) { sourceTypeOrInterfaceType, typeof(IInterceptor[]) };
            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctorParamters.ToArray());

            /*fix argument length*/
            ctorParamters.Insert(0, sourceTypeOrInterfaceType);
            var ctorIL = new MockEmitBuilder(ctorBuilder.GetILGenerator(), ctorBuilder.CallingConvention, typeof(void), ctorParamters.ToArray());

            ctorIL.LoadArgument(0);
            ctorIL.LoadArgument(1);
            ctorIL.StoreField(sourceObjectFieldBuilder);

            ctorIL.LoadArgument(0);
            ctorIL.LoadConstant(2);
            ctorIL.NewObject(typeof(List<IInterceptor>).GetConstructor(new[] { typeof(int) }));
            ctorIL.StoreField(interceptorFieldBuilder);

            ctorIL.LoadArgument(0);
            ctorIL.LoadField(interceptorFieldBuilder);
            ctorIL.LoadArgument(2);
            ctorIL.Call(typeof(List<IInterceptor>).GetMethod("AddRange", new[] { typeof(IInterceptor[]) }));

            ctorIL.Return();
        }

        /// <summary>
        /// 构建构造函数【用于代理类】，因为IoC注入和Proxy使用的方式是不同的，Proxy可能直接用数组去构造，而IoC只能一个一个去注入
        /// </summary>
        /// <param name="typeBuilder">类型构造</param>
        /// <param name="sourceTypeOrInterfaceType">代理（接口或类）类型</param>
        /// <param name="interceptors">拦截器集合</param>
        /// <param name="interceptorFieldBuilder">拦截器构建，被定义为List接口</param>
        protected override void DefineConstructor(TypeBuilder typeBuilder, Type sourceTypeOrInterfaceType, Type[] interceptors, FieldBuilder interceptorFieldBuilder)
        {
            /*构造函数*/
            var ctorParamters = new List<Type>(1) { typeof(IInterceptor[]) };
            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctorParamters.ToArray());

            /*fix argument length*/
            ctorParamters.Insert(0, sourceTypeOrInterfaceType);
            var ctorIL = new MockEmitBuilder(ctorBuilder.GetILGenerator(), ctorBuilder.CallingConvention, typeof(void), ctorParamters.ToArray());

            ctorIL.LoadArgument(0);
            ctorIL.LoadConstant(2);
            ctorIL.NewObject(typeof(List<IInterceptor>).GetConstructor(new[] { typeof(int) }));
            ctorIL.StoreField(interceptorFieldBuilder);

            ctorIL.LoadArgument(0);
            ctorIL.LoadField(interceptorFieldBuilder);

            ctorIL.LoadArgument(1);
            ctorIL.Call(typeof(List<IInterceptor>).GetMethod("AddRange", new[] { typeof(IInterceptor[]) }));

            ctorIL.Return();
        }

        #endregion build
    }
}