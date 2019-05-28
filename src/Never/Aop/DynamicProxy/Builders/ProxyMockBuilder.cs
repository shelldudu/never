using Never.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Never.Aop.DynamicProxy.Builders
{
    /// <summary>
    /// 构建T对象的代理
    /// </summary>
    public class ProxyMockBuilder : MockBuilder
    {
        #region build

        /// <summary>
        /// 构建构造函数【用于代理接口的实现】，因为实现接口中要获取被代理的对象，只能通过构造那里传递过来，而继承类的时候，可以直接重写期类的实现便可
        /// </summary>
        /// <param name="typeBuilder">类型构造</param>
        /// <param name="sourceType">目标类型</param>
        /// <param name="sourceTypeOrInterfaceType">代理（接口或类）类型</param>
        /// <param name="interceptors">拦截器集合</param>
        /// <param name="interceptorFieldBuilder">拦截器构建，被定义为List接口</param>
        /// <param name="sourceObjectFieldBuilder">目标类型构建</param>
        protected virtual void DefineConstructor(TypeBuilder typeBuilder, Type sourceTypeOrInterfaceType, Type sourceType, Type[] interceptors, FieldBuilder sourceObjectFieldBuilder, FieldBuilder interceptorFieldBuilder)
        {
            /*构造函数*/
            var ctorParamters = new List<Type>(1) { sourceTypeOrInterfaceType };
            foreach (var inter in interceptors)
                ctorParamters.Add(inter);

            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctorParamters.ToArray());

            /*fix argument length*/
            ctorParamters.Insert(0, sourceType);
            var ctorIL = new MockEmitBuilder(ctorBuilder.GetILGenerator(), ctorBuilder.CallingConvention, typeof(void), ctorParamters.ToArray());

            ctorIL.LoadArgument(0);
            ctorIL.LoadArgument(1);
            ctorIL.StoreField(sourceObjectFieldBuilder);

            ctorIL.LoadArgument(0);
            ctorIL.LoadConstant(2);
            ctorIL.NewObject(typeof(List<IInterceptor>).GetConstructor(new[] { typeof(int) }));
            ctorIL.StoreField(interceptorFieldBuilder);

            for (var i = 0; i < interceptors.Length; i++)
            {
                ctorIL.LoadArgument(0);
                ctorIL.LoadField(interceptorFieldBuilder);
                ctorIL.LoadArgument((ushort)(i + 2));
                ctorIL.Call(typeof(List<IInterceptor>).GetMethod("Add", new[] { typeof(IInterceptor) }));
            }

            ctorIL.Return();
        }

        /// <summary>
        /// 构建构造函数【用于代理类的实现】，因为实现接口中要获取被代理的对象，只能通过构造那里传递过来，而继承类的时候，可以直接重写期类的实现便可
        /// </summary>
        /// <param name="typeBuilder">类型构造</param>
        /// <param name="sourceTypeOrInterfaceType">目标类型</param>
        /// <param name="interceptors">拦截器集合</param>
        /// <param name="interceptorFieldBuilder">拦截器构建，被定义为List接口</param>
        protected virtual void DefineConstructor(TypeBuilder typeBuilder, Type sourceTypeOrInterfaceType, Type[] interceptors, FieldBuilder interceptorFieldBuilder)
        {
            /*构造函数*/
            var ctorParamters = new List<Type>(0) { };
            foreach (var inter in interceptors)
                ctorParamters.Add(inter);

            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, ctorParamters.ToArray());

            /*fix argument length*/
            ctorParamters.Insert(0, sourceTypeOrInterfaceType);
            var ctorIL = new MockEmitBuilder(ctorBuilder.GetILGenerator(), ctorBuilder.CallingConvention, typeof(void), ctorParamters.ToArray());

            ctorIL.LoadArgument(0);
            ctorIL.LoadConstant(2);
            ctorIL.NewObject(typeof(List<IInterceptor>).GetConstructor(new[] { typeof(int) }));
            ctorIL.StoreField(interceptorFieldBuilder);

            for (var i = 0; i < interceptors.Length; i++)
            {
                ctorIL.LoadArgument(0);
                ctorIL.LoadField(interceptorFieldBuilder);
                ctorIL.LoadArgument((ushort)(i + 1));
                ctorIL.Call(typeof(List<IInterceptor>).GetMethod("Add", new[] { typeof(IInterceptor) }));
            }

            ctorIL.Return();
        }

        /// <summary>
        /// 进行构建
        /// </summary>
        /// <param name="sourceType">目标类型</param>
        /// <param name="setting">配置</param>
        /// <param name="interfaceType">代理（接口）类型</param>
        /// <param name="interceptors">拦截器集合</param>
        /// <returns></returns>
        protected virtual Type BuildInterface(Type interfaceType, Type sourceType, Type[] interceptors, InterceptCompileSetting setting)
        {
            /*检查支持类型*/
            this.SupportType(interfaceType);

            /*是接口类型，则要在内存中生成一个类，用于实现该接口所有定义*/
            /*生命一个类对象构造*/
            var nextTypename = this.GetNextTypeName(interfaceType);
            var typeBuilder = EasyEmitBuilder<Type>.NewTypeBuilder(nextTypename.NameSplace, nextTypename.TypeName, typeof(object), new[] { interfaceType });

            /*构造函数*/
            var sourceObjectFieldBuilder = typeBuilder.DefineField("baseObject", sourceType, FieldAttributes.InitOnly | FieldAttributes.Private);
            var interceptorFieldBuilder = typeBuilder.DefineField("interceptors", typeof(List<IInterceptor>), FieldAttributes.InitOnly | FieldAttributes.Private);
            this.DefineConstructor(typeBuilder, interfaceType, sourceType, interceptors, sourceObjectFieldBuilder, interceptorFieldBuilder);

            /*获取成员，接口全部都是方法*/
            var members = this.GetInterfaceMembers(interfaceType);
            var exists = new List<MemberInfo>(members.Length);
            foreach (var member in members)
            {
                if (exists.Contains(member))
                    continue;

                if (member.MemberType == MemberTypes.Event)
                {
                    this.BuildEvent(interfaceType, interceptors, new[] { interfaceType }, interfaceType, member, members, exists, typeBuilder, interceptorFieldBuilder, sourceObjectFieldBuilder, setting);
                    continue;
                }
            }

            foreach (var member in members)
            {
                if (exists.Contains(member))
                    continue;

                if (member.MemberType == MemberTypes.Method)
                {
                    this.BuildMethod(interfaceType, interceptors, new[] { interfaceType }, interfaceType, member, members, exists, typeBuilder, interceptorFieldBuilder, sourceObjectFieldBuilder, setting);
                    continue;
                }
            }

            return EasyEmitBuilder<Type>.CreateTypeInfo(typeBuilder);
        }

        /// <summary>
        /// 对类进行构建，只能用集成方式
        /// https://msdn.microsoft.com/en-us/library/system.reflection.emit.typebuilder.definemethodoverride(v=vs.110).aspx
        /// </summary>
        /// <param name="setting">配置</param>
        /// <param name="interceptors">拦截器集合</param>
        /// <param name="sourceType">目标类型</param>
        /// <returns></returns>
        protected virtual Type BuildClass(Type sourceType, Type[] interceptors, InterceptCompileSetting setting)
        {
            /*检查支持类型*/
            this.SupportType(sourceType);

            /*是接口类型，则要在内存中生成一个类，用于实现该接口所有定义*/
            /*生命一个类对象构造*/
            var nextTypename = this.GetNextTypeName(sourceType);
            var typeBuilder = EasyEmitBuilder<Type>.NewTypeBuilder(nextTypename.NameSplace, nextTypename.TypeName, sourceType, Type.EmptyTypes);

            /*构造函数*/
            var interceptorFieldBuilder = typeBuilder.DefineField("interceptors", typeof(List<IInterceptor>), FieldAttributes.InitOnly | FieldAttributes.Private);
            this.DefineConstructor(typeBuilder, sourceType, interceptors, interceptorFieldBuilder);

            /*获取成员，接口全部都是方法，对象可能返回，属性，字段等成员，但我们主要是代理方法*/
            var members = this.GetMembers(sourceType);
            var exists = new List<MemberInfo>(members.Length);
            foreach (var member in members)
            {
                if (exists.Contains(member))
                    continue;

                if (member.MemberType == MemberTypes.Field)
                {
                    this.BuildField(sourceType, interceptors, sourceType.GetInterfaces(), sourceType, member, members, exists, typeBuilder, interceptorFieldBuilder, null, setting);
                    continue;
                }

                if (member.MemberType == MemberTypes.Event)
                {
                    this.BuildEvent(sourceType, interceptors, sourceType.GetInterfaces(), sourceType, member, members, exists, typeBuilder, interceptorFieldBuilder, null, setting);
                    continue;
                }
            }

            foreach (var member in members)
            {
                if (exists.Contains(member))
                    continue;

                if (member.MemberType == MemberTypes.Method)
                {
                    this.BuildMethod(sourceType, interceptors, sourceType.GetInterfaces(), sourceType, member, members, exists, typeBuilder, interceptorFieldBuilder, null, setting);
                    continue;
                }
            }

            return EasyEmitBuilder<Type>.CreateTypeInfo(typeBuilder);
        }

        /// <summary>
        /// 构建参数信息
        /// </summary>
        /// <param name="sourceTypeOrInterfaceType">代理或者接口类型</param>
        /// <param name="interceptors">拦截器集合</param>
        /// <param name="interfaces">代理所实现的接口</param>
        /// <param name="proxyType">代理类型</param>
        /// <param name="member">方法成员</param>
        /// <param name="members">方法成员</param>
        /// <param name="exists">已经构建的对象</param>
        /// <param name="typeBuilder">类型构建</param>
        /// <param name="interceptorFieldBuilder">拦截器字段</param>
        /// <param name="sourceObjectFieldBuilder">这个值是用来区分是接口方式还是继承类方式</param>
        /// <param name="setting">配置</param>
        protected virtual void BuildMethod(Type sourceTypeOrInterfaceType, Type[] interceptors, Type[] interfaces, Type proxyType, MemberInfo member, MemberInfo[] members, List<MemberInfo> exists, TypeBuilder typeBuilder, FieldBuilder interceptorFieldBuilder, FieldBuilder sourceObjectFieldBuilder, InterceptCompileSetting setting)
        {
            if (member.MemberType != MemberTypes.Method)
                return;

            /*构建方法*/
            var method = (MethodInfo)member;
            /*泛型方法和基本方法有什么不同*/
            var parameters = method.GetParameters();
            var parameterTypes = new List<Type>(parameters.Length);
            foreach (var parameter in parameters)
            {
                parameterTypes.Add(parameter.ParameterType);
            }

            var methodBuilder = typeBuilder.DefineMethod(method.Name,
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                method.CallingConvention,
                method.ReturnType, parameterTypes.ToArray());

            /*fix argument length*/
            parameterTypes.Insert(0, sourceTypeOrInterfaceType);
            var il = new MockEmitBuilder(methodBuilder.GetILGenerator(), method.CallingConvention, method.ReturnType, parameterTypes.ToArray());

            /*开始构建方法的调用，先是检查拦截器的个数，如果没有拦截器，则直接调用代理类型的方法*/
            /*没有拦截器*/
            if (interceptors.Length <= 0)
            {
                if (sourceObjectFieldBuilder != null)
                {
                    /*调用原来的目标对象*/
                    il.LoadArgument(0);
                    il.LoadField(sourceObjectFieldBuilder);
                }
                else
                {
                    il.LoadArgument(0);
                }

                /*load arguments*/
                for (var i = 1; i < parameterTypes.Count; i++)
                    il.LoadArgument((ushort)i);

                /*find*/
                il.Call(method);
                il.Return();
                return;
            }

            /*不用调用信息*/
            if (setting.NoInvocation)
            {
                /*开始执行方法*/
                il.LoadArgument(0);
                il.LoadField(interceptorFieldBuilder);
                il.LoadNull();
                il.Call(typeof(ProxyMockBuilder).GetMethod("PreProceed"));

                ILocal returnTempLocal = null;
                if (method.ReturnType != null && method.ReturnType != typeof(void))
                    returnTempLocal = il.DeclareLocal(method.ReturnType);

                /*调用原来的目标对象*/
                if (sourceObjectFieldBuilder != null)
                {
                    /*调用原来的目标对象*/
                    il.LoadArgument(0);
                    il.LoadField(sourceObjectFieldBuilder);
                }
                else
                {
                    il.LoadArgument(0);
                }

                /*load arguments*/
                for (var i = 1; i < parameterTypes.Count; i++)
                    il.LoadArgument((ushort)i);

                /*find*/
                il.Call(method);
                if (returnTempLocal != null)
                    il.StoreLocal(returnTempLocal);

                il.LoadArgument(0);
                il.LoadField(interceptorFieldBuilder);
                il.LoadNull();
                il.Call(typeof(ProxyMockBuilder).GetMethod("PostProceed"));

                if (returnTempLocal != null)
                    il.LoadLocal(returnTempLocal);

                il.Return();
                return;
            }

            /*有拦截器，则开始存储信息*/
            var invocation = new ProxyObjectInfoStore.Invocation()
            {
                Interfaces = interfaces,
                Method = method,
                MethodAttributes = ProxyObjectInfoStore.Invocation.GetAttributes(method),
                ProxyAttributes = ProxyObjectInfoStore.Invocation.GetAttributes(sourceTypeOrInterfaceType),
                ProxyType = proxyType,
            };

            /*构建参数Type去Json序列化*/
            //this.BuildMethodArguments(method);

            /*保存该值，有用处*/
            var index = ProxyObjectInfoStore.Enqueue(invocation);

            /*参数集合*/
            // 因为用了Object,存在大量的装箱拆箱，性能可能有点问题
            ILocal argumentLocal = setting.StoreArgument ? il.DeclareLocal(typeof(List<KeyValuePair<string, object>>)) : null;
            if (setting.StoreArgument)
            {
                il.LoadConstant(parameters.Length);
                il.NewObject(typeof(List<KeyValuePair<string, object>>).GetConstructor(new[] { typeof(int) }));
                il.StoreLocal(argumentLocal);

                /*注意泛型方法*/
                if (method.IsGenericMethodDefinition)
                {
                }
                else
                {
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        /*out 参数*/
                        if (parameters[i].IsOut)
                        {
                            this.LoadOutParameter(il, parameters, argumentLocal, i, setting);
                            continue;
                        }

                        /*ref参数*/
                        if (parameters[i].ParameterType.IsByRef)
                        {
                            this.LoadRefParameter(il, parameters, argumentLocal, i, setting);
                            continue;
                        }

                        /*不是ref参数*/
                        if (parameters[i].ParameterType.IsValueType)
                        {
                            this.LoadValueTypeParameter(il, parameters, argumentLocal, i, setting);
                            continue;
                        }

                        this.LoadObjectParameter(il, parameters, argumentLocal, i, setting);
                        continue;
                    }
                }
            }

            /*开始执行调用参数的初始化*/
            /*值类型的*/
            var invocationLocal = il.DeclareLocal(typeof(ProxyObjectInfoStore.Invocation));
            il.LoadConstant(index);
            il.Call(typeof(ProxyObjectInfoStore).GetMethod("Query"));
            il.StoreLocal(invocationLocal);

            /*begin new*/
            il.LoadLocal(invocationLocal);
            if (sourceObjectFieldBuilder != null)
            {
                /*调用原来的目标对象*/
                il.LoadArgument(0);
                il.LoadField(sourceObjectFieldBuilder);
            }
            else
            {
                il.LoadArgument(0);
            }

            if (setting.StoreArgument)
                il.LoadLocal(argumentLocal);
            else
                il.LoadNull();

            il.LoadNull();
            il.Call(typeof(ProxyObjectInfoStore.Invocation).GetMethod("NewObject"));
            il.StoreLocal(invocationLocal);

            /*end copy*/

            /*开始执行方法*/
            il.LoadArgument(0);
            il.LoadField(interceptorFieldBuilder);
            il.LoadLocal(invocationLocal);
            il.Call(typeof(ProxyMockBuilder).GetMethod("PreProceed"));

            ILocal returnLocal = null;
            if (method.ReturnType != null && method.ReturnType != typeof(void))
                returnLocal = il.DeclareLocal(method.ReturnType);

            /*调用原来的目标对象*/
            if (sourceObjectFieldBuilder != null)
            {
                /*调用原来的目标对象*/
                il.LoadArgument(0);
                il.LoadField(sourceObjectFieldBuilder);
            }
            else
            {
                il.LoadArgument(0);
            }

            /*load arguments*/
            for (var i = 1; i < parameterTypes.Count; i++)
                il.LoadArgument((ushort)i);

            /*find*/
            il.Call(method);
            if (returnLocal != null)
                il.StoreLocal(returnLocal);

            if (setting.StoreArgument)
            {
                /*可能有out参数*/
                bool isReload = false;
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].IsOut)
                    {
                        isReload = true;
                        this.LoadOutParameter(il, parameters, argumentLocal, i, setting);
                        continue;
                    }

                    /*ref参数*/
                    if (parameters[i].ParameterType.IsByRef)
                    {
                        isReload = true;
                        this.LoadRefParameter(il, parameters, argumentLocal, i, setting);
                        continue;
                    }

                    /*不是ref参数*/
                    if (parameters[i].ParameterType.IsValueType)
                    {
                        continue;
                    }

                    //this.LoadObjectParameter(il, parameters, argumentLocal, i, true);
                    continue;
                }

                if (isReload)
                {
                    il.LoadLocal(invocationLocal);
                    il.LoadLocal(invocationLocal);
                    il.LoadLocal(argumentLocal);
                    il.Call(typeof(ProxyObjectInfoStore.Invocation).GetMethod("CopyArguments"));
                    il.StoreLocal(invocationLocal);
                }
            }

            il.LoadArgument(0);
            il.LoadField(interceptorFieldBuilder);
            il.LoadLocal(invocationLocal);
            il.Call(typeof(ProxyMockBuilder).GetMethod("PostProceed"));

            if (returnLocal != null)
                il.LoadLocal(returnLocal);

            il.Return();
        }

        /// <summary>
        /// 构建参数信息
        /// </summary>
        /// <param name="sourceTypeOrInterfaceType">代理或者接口类型</param>
        /// <param name="interceptors">拦截器集合</param>
        /// <param name="interfaces">代理所实现的接口</param>
        /// <param name="proxyType">代理类型</param>
        /// <param name="member">方法成员</param>
        /// <param name="members">方法成员</param>
        /// <param name="exists">已经构建的对象</param>
        /// <param name="typeBuilder">类型构建</param>
        /// <param name="interceptorFieldBuilder">拦截器字段</param>
        /// <param name="sourceObjectFieldBuilder">这个值是用来区分是接口方式还是继承类方式</param>
        /// <param name="setting">配置</param>
        protected virtual void BuildEvent(Type sourceTypeOrInterfaceType, Type[] interceptors, Type[] interfaces, Type proxyType, MemberInfo member, MemberInfo[] members, List<MemberInfo> exists, TypeBuilder typeBuilder, FieldBuilder interceptorFieldBuilder, FieldBuilder sourceObjectFieldBuilder, InterceptCompileSetting setting)
        {
            var @event = member as EventInfo;
            exists.Add(@event);
            var addName = string.Concat("add_", @event.Name);
            var removeName = string.Concat("remove_", @event.Name);
            MethodInfo addMethod = null, removeMethod = null;
            foreach (var m in members)
            {
                if (m.MemberType == MemberTypes.Method)
                {
                    if (addName == m.Name)
                    {
                        addMethod = (MethodInfo)m;
                        exists.Add(m);
                        if (removeMethod != null)
                            break;
                    }
                    else if (removeName == m.Name)
                    {
                        removeMethod = (MethodInfo)m;
                        exists.Add(m);
                        if (addMethod != null)
                            break;
                    }
                }
            }

            var fieldBuilder = typeBuilder.DefineField(@event.Name, @event.EventHandlerType, FieldAttributes.Private);
            var eventBuilder = typeBuilder.DefineEvent(@event.Name, @event.Attributes, @event.EventHandlerType);

            if (addMethod != null)
            {
                if (fieldBuilder == null)
                    fieldBuilder = typeBuilder.DefineField(@event.Name, @event.EventHandlerType, FieldAttributes.Private);

                /*泛型方法和基本方法有什么不同*/
                var parameters = addMethod.GetParameters();
                var parameterTypes = new List<Type>(parameters.Length);
                var voidReturn = addMethod.ReturnType == typeof(void);

                foreach (var parameter in parameters)
                {
                    parameterTypes.Add(parameter.ParameterType);
                }

                var attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
                if (sourceObjectFieldBuilder != null)
                    attributes = MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig;

                var methodBuilder = typeBuilder.DefineMethod(addMethod.Name,
                    attributes,
                    addMethod.CallingConvention,
                    addMethod.ReturnType, parameterTypes.ToArray());

                /*fix argument length*/
                parameterTypes.Insert(0, sourceTypeOrInterfaceType);
                var il = new MockEmitBuilder(methodBuilder.GetILGenerator(), addMethod.CallingConvention, addMethod.ReturnType, parameterTypes.ToArray());

                /*调用原来的目标*/
                if (sourceObjectFieldBuilder != null)
                {
                    il.LoadArgument(0);
                    il.LoadField(sourceObjectFieldBuilder);
                    il.LoadArgument(1);
                    il.Call(addMethod);
                    il.Return();
                }
                else
                {
                    var locals = new[]
                                   {
                    il.DeclareLocal(@event.EventHandlerType),
                    il.DeclareLocal(@event.EventHandlerType),
                    il.DeclareLocal(@event.EventHandlerType),
                };
                    var label = il.DefineLabel();

                    il.LoadArgument(0);
                    il.LoadField(fieldBuilder);
                    il.StoreLocal(locals[0]);

                    il.MarkLabel(label);
                    il.LoadLocal(locals[0]);
                    il.StoreLocal(locals[1]);
                    il.LoadLocal(locals[1]);
                    il.LoadArgument(1);

                    MethodInfo delegateMethod = null;
                    foreach (var method in typeof(System.Delegate).GetMethods(BindingFlags.Public | BindingFlags.Static))
                    {
                        if (method.Name == "Combine" && method.GetParameters().Length == 2)
                        {
                            delegateMethod = method;
                            break;
                        }
                    }

                    il.Call(delegateMethod);
                    il.CastClass(@event.EventHandlerType);
                    il.StoreLocal(locals[2]);

                    il.LoadArgument(0);
                    il.LoadFieldAddress(fieldBuilder);
                    il.LoadLocal(locals[2]);
                    il.LoadLocal(locals[1]);

                    foreach (var method in typeof(System.Threading.Interlocked).GetMethods(BindingFlags.Public | BindingFlags.Static))
                    {
                        if (method.Name == "CompareExchange" && method.IsGenericMethod && method.GetParameters().Length == 3)
                        {
                            delegateMethod = method;
                            break;
                        }
                    }
                    il.Call(delegateMethod.MakeGenericMethod(new[] { @event.EventHandlerType }));
                    il.StoreLocal(locals[0]);
                    il.LoadLocal(locals[0]);
                    il.LoadLocal(locals[1]);
                    il.UnsignedBranchIfNotEqual(label);
                    il.Return();
                }

                @eventBuilder.SetAddOnMethod(methodBuilder);
            }

            if (removeMethod != null)
            {
                if (fieldBuilder == null)
                    fieldBuilder = typeBuilder.DefineField(@event.Name, @event.EventHandlerType, FieldAttributes.Private);

                /*泛型方法和基本方法有什么不同*/
                var parameters = removeMethod.GetParameters();
                var parameterTypes = new List<Type>(parameters.Length);
                var voidReturn = removeMethod.ReturnType == typeof(void);

                foreach (var parameter in parameters)
                {
                    parameterTypes.Add(parameter.ParameterType);
                }

                var attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
                if (sourceObjectFieldBuilder != null)
                    attributes = MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig;

                var methodBuilder = typeBuilder.DefineMethod(removeMethod.Name,
                    attributes,
                    removeMethod.CallingConvention,
                    removeMethod.ReturnType, parameterTypes.ToArray());

                /*fix argument length*/
                parameterTypes.Insert(0, sourceTypeOrInterfaceType);
                var il = new MockEmitBuilder(methodBuilder.GetILGenerator(), removeMethod.CallingConvention, removeMethod.ReturnType, parameterTypes.ToArray());

                /*调用原来的目标*/
                if (sourceObjectFieldBuilder != null)
                {
                    il.LoadArgument(0);
                    il.LoadField(sourceObjectFieldBuilder);
                    il.LoadArgument(1);
                    il.Call(removeMethod);
                    il.Return();
                }
                else
                {
                    var locals = new[]
                    {
                        il.DeclareLocal(@event.EventHandlerType),
                        il.DeclareLocal(@event.EventHandlerType),
                        il.DeclareLocal(@event.EventHandlerType),
                    };

                    var label = il.DefineLabel();

                    il.LoadArgument(0);
                    il.LoadField(fieldBuilder);
                    il.StoreLocal(locals[0]);

                    il.MarkLabel(label);
                    il.LoadLocal(locals[0]);
                    il.StoreLocal(locals[1]);
                    il.LoadLocal(locals[1]);
                    il.LoadArgument(1);

                    MethodInfo delegateMethod = null;
                    foreach (var method in typeof(System.Delegate).GetMethods(BindingFlags.Public | BindingFlags.Static))
                    {
                        if (method.Name == "Remove" && method.GetParameters().Length == 2)
                        {
                            delegateMethod = method;
                            break;
                        }
                    }

                    il.Call(delegateMethod);
                    il.CastClass(@event.EventHandlerType);
                    il.StoreLocal(locals[2]);

                    il.LoadArgument(0);
                    il.LoadFieldAddress(fieldBuilder);
                    il.LoadLocal(locals[2]);
                    il.LoadLocal(locals[1]);

                    foreach (var method in typeof(System.Threading.Interlocked).GetMethods(BindingFlags.Public | BindingFlags.Static))
                    {
                        if (method.Name == "CompareExchange" && method.IsGenericMethod && method.GetParameters().Length == 3)
                        {
                            delegateMethod = method;
                            break;
                        }
                    }
                    il.Call(delegateMethod.MakeGenericMethod(new[] { @event.EventHandlerType }));
                    il.StoreLocal(locals[0]);
                    il.LoadLocal(locals[0]);
                    il.LoadLocal(locals[1]);
                    il.UnsignedBranchIfNotEqual(label);
                    il.Return();
                }

                @eventBuilder.SetRemoveOnMethod(methodBuilder);
            }
        }

        /// <summary>
        /// 构建参数信息
        /// </summary>
        /// <param name="sourceTypeOrInterfaceType">代理或者接口类型</param>
        /// <param name="interceptors">拦截器集合</param>
        /// <param name="interfaces">代理所实现的接口</param>
        /// <param name="proxyType">代理类型</param>
        /// <param name="member">方法成员</param>
        /// <param name="members">方法成员</param>
        /// <param name="exists">已经构建的对象</param>
        /// <param name="typeBuilder">类型构建</param>
        /// <param name="interceptorFieldBuilder">拦截器字段</param>
        /// <param name="sourceObjectFieldBuilder">这个值是用来区分是接口方式还是继承类方式</param>
        /// <param name="setting">配置</param>
        protected virtual void BuildField(Type sourceTypeOrInterfaceType, Type[] interceptors, Type[] interfaces, Type proxyType, MemberInfo member, MemberInfo[] members, List<MemberInfo> exists, TypeBuilder typeBuilder, FieldBuilder interceptorFieldBuilder, FieldBuilder sourceObjectFieldBuilder, InterceptCompileSetting setting)
        {
            var field = member as FieldInfo;
            exists.Add(field);
        }

        /// <summary>
        /// 加载Out参数
        /// </summary>
        /// <param name="il">The il.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="argumentLocal">The argument local.</param>
        /// <param name="index">The i.</param>
        /// <param name="setting">配置</param>
        protected void LoadOutParameter(MockEmitBuilder il, ParameterInfo[] parameters, ILocal argumentLocal, int index, InterceptCompileSetting setting)
        {
            var elementType = parameters[index].ParameterType.GetElementType();
            /*ref object*/
            if (!elementType.IsValueType)
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgument((ushort)(index + 1));
                il.LoadIndirect(elementType);
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddParameter"));

                return;
            }

            /*box*/
            if (setting.BoxArgument)
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgument((ushort)(index + 1));
                il.LoadObject(elementType);
                il.Box(elementType);
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddParameter"));

                return;
            }

            /*非可空的*/
            if (this.IsEnumType(elementType))
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgument((ushort)(index + 1));
                il.CallVirtual(typeof(Object).GetMethod("ToString", Type.EmptyTypes), elementType);
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddParameter"));

                return;
            }

            /*可空的*/
            if (this.IsNullableEnumType(elementType))
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgument((ushort)(index + 1));
                il.LoadObject(elementType);
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddNullableParameter").MakeGenericMethod(Nullable.GetUnderlyingType(elementType)));

                return;
            }

            /*非可空的*/
            if (this.IsPrimitiveType(elementType))
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgument((ushort)(index + 1));
                il.Call(elementType.GetMethod("ToString", Type.EmptyTypes));
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddParameter"));

                return;
            }

            /*可空的*/
            if (this.IsNullablePrimitiveType(elementType))
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgument((ushort)(index + 1));
                il.LoadObject(elementType);
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddNullableParameter").MakeGenericMethod(Nullable.GetUnderlyingType(elementType)));

                return;
            }

            /*值对象，可能是结构体*/
            il.LoadLocal(argumentLocal);
            il.LoadConstant(index + 1);
            il.LoadConstant(parameters[index].Name);
            il.LoadArgument((ushort)(index + 1));
            il.LoadObject(elementType);
            il.Box(elementType);
            il.Call(typeof(ProxyMockBuilder).GetMethod("AddParameter"));

            return;
        }

        /// <summary>
        /// 加载Ref参数
        /// </summary>
        /// <param name="il">The il.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="argumentLocal">The argument local.</param>
        /// <param name="index">The i.</param>
        /// <param name="setting">配置</param>
        protected void LoadRefParameter(MockEmitBuilder il, ParameterInfo[] parameters, ILocal argumentLocal, int index, InterceptCompileSetting setting)
        {
            var elementType = parameters[index].ParameterType.GetElementType();
            /*ref object*/
            if (!elementType.IsValueType)
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgument((ushort)(index + 1));
                il.LoadIndirect(elementType);
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddParameter"));

                return;
            }

            /*box*/
            if (setting.BoxArgument)
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgument((ushort)(index + 1));
                il.LoadIndirect(elementType);
                il.Box(elementType);
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddParameter"));

                return;
            }

            /*非可空的*/
            if (this.IsEnumType(elementType))
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgument((ushort)(index + 1));
                il.CallVirtual(typeof(Object).GetMethod("ToString", Type.EmptyTypes), elementType);
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddParameter"));

                return;
            }

            /*可空的*/
            if (this.IsNullableEnumType(elementType))
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgument((ushort)(index + 1));
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddNullableParameter").MakeGenericMethod(Nullable.GetUnderlyingType(elementType)));

                return;
            }

            /*非可空的*/
            if (this.IsPrimitiveType(elementType))
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgument((ushort)(index + 1));
                il.Call(elementType.GetMethod("ToString", Type.EmptyTypes));
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddParameter"));

                return;
            }

            /*可空的*/
            if (this.IsNullablePrimitiveType(elementType))
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgument((ushort)(index + 1));
                il.LoadObject(elementType);
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddNullableParameter").MakeGenericMethod(Nullable.GetUnderlyingType(elementType)));

                return;
            }

            /*值对象，可能是结构体*/
            il.LoadLocal(argumentLocal);
            il.LoadConstant(index + 1);
            il.LoadConstant(parameters[index].Name);
            il.LoadArgument((ushort)(index + 1));
            il.LoadObject(elementType);
            il.Box(elementType);
            il.Call(typeof(ProxyMockBuilder).GetMethod("AddParameter"));

            return;
        }

        /// <summary>
        /// 加载valueType参数
        /// </summary>
        /// <param name="il">The il.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="argumentLocal">The argument local.</param>
        /// <param name="index">The i.</param>
        /// <param name="setting">配置</param>
        protected void LoadValueTypeParameter(MockEmitBuilder il, ParameterInfo[] parameters, ILocal argumentLocal, int index, InterceptCompileSetting setting)
        {
            /*box*/
            if (setting.BoxArgument)
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgument((ushort)(index + 1));
                il.Box(parameters[index].ParameterType);
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddParameter"));
                return;
            }

            /*非可空的*/
            if (this.IsEnumType(parameters[index].ParameterType))
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgumentAddress((ushort)(index + 1));
                il.CallVirtual(typeof(Object).GetMethod("ToString", Type.EmptyTypes), parameters[index].ParameterType);
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddParameter"));
                return;
            }

            /*可空的*/
            if (this.IsNullableEnumType(parameters[index].ParameterType))
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgument((ushort)(index + 1));
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddNullableParameter").MakeGenericMethod(Nullable.GetUnderlyingType(parameters[index].ParameterType)));
                return;
            }

            /*非可空的*/
            if (this.IsPrimitiveType(parameters[index].ParameterType))
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgumentAddress((ushort)(index + 1));
                il.Call(parameters[index].ParameterType.GetMethod("ToString", Type.EmptyTypes));
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddParameter"));
                return;
            }

            /*可空的*/
            if (this.IsNullablePrimitiveType(parameters[index].ParameterType))
            {
                il.LoadLocal(argumentLocal);
                il.LoadConstant(index + 1);
                il.LoadConstant(parameters[index].Name);
                il.LoadArgument((ushort)(index + 1));
                il.Call(typeof(ProxyMockBuilder).GetMethod("AddNullableParameter").MakeGenericMethod(Nullable.GetUnderlyingType(parameters[index].ParameterType)));
                return;
            }

            /*值对象，可能是结构体*/
            il.LoadLocal(argumentLocal);
            il.LoadConstant(index + 1);
            il.LoadConstant(parameters[index].Name);
            il.LoadArgument((ushort)(index + 1));
            il.Box(parameters[index].ParameterType);
            il.Call(typeof(ProxyMockBuilder).GetMethod("AddParameter"));
            return;
        }

        /// <summary>
        /// 加载object参数
        /// </summary>
        /// <param name="il">The il.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="argumentLocal">The argument local.</param>
        /// <param name="index">The i.</param>
        /// <param name="setting">配置</param>
        protected void LoadObjectParameter(MockEmitBuilder il, ParameterInfo[] parameters, ILocal argumentLocal, int index, InterceptCompileSetting setting)
        {
            il.LoadLocal(argumentLocal);
            il.LoadConstant(index + 1);
            il.LoadConstant(parameters[index].Name);
            il.LoadArgument((ushort)(index + 1));
            il.Call(typeof(ProxyMockBuilder).GetMethod("AddParameter"));

            return;
        }

        /// <summary>
        /// 构建方法参数的类型
        /// </summary>
        /// <param name="method">方法</param>
        /// <returns></returns>
        protected virtual int BuildMethodArguments(MethodInfo method)
        {
            return -1;

            //var parameters = method.GetParameters();
            //if (parameters.Length == 0)
            //    return -1;

            //var typeBuilder = EasyEmitBuilder<DelegatePlaceholder>.NewTypeBuilder(string.Empty, "DynaicMethodAgument", TypeAttributes.Public | TypeAttributes.Sealed, typeof(object), Type.EmptyTypes);
            //foreach (var parameter in parameters)
            //{
            //    if (parameter.ParameterType.IsByRef)
            //    {
            //        var elementType = parameter.ParameterType.GetElementType();

            //        typeBuilder.DefineField(parameter.Name, elementType, FieldAttributes.Public | FieldAttributes.SpecialName);
            //        continue;
            //    }

            //    if (parameter.IsOut)
            //    {
            //        var elementType = parameter.ParameterType.GetElementType();
            //        typeBuilder.DefineField(parameter.Name, elementType, FieldAttributes.Public | FieldAttributes.SpecialName);
            //        continue;
            //    }

            //    var parameterTpe = parameter.ParameterType;
            //    typeBuilder.DefineField(parameter.Name, parameterTpe, FieldAttributes.Public | FieldAttributes.SpecialName);
            //    continue;
            //}

            //var type = typeBuilder.CreateType();

            //return ProxyObjectInfoStore.EnqueueType(type);
        }

        /// <summary>
        /// 从原来的类型获取其属性和字段，从新构建一个类出来
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        protected virtual Type NewTypeWitPropAndField(Type source)
        {
            if (source.IsVisible)
                return source;

            if (IsPrimitiveType(source))
                return null;

            if (source.IsEnum)
                return null;

            if (this.IsNullablePrimitiveType(source))
                return null;

            if (this.IsNullableEnumType(source))
                return null;

            if (source.IsArray)
                return null;

            return source;
        }

        #endregion build

        #region excuting

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="paramters">The paramters.</param>
        /// <param name="parameterName"></param>
        /// <param name="paramterValue">The paramter.</param>
        /// <param name="index">The index.</param>
        public static void AddParameter(List<KeyValuePair<string, object>> paramters, int index, string parameterName, object paramterValue)
        {
            /*add*/
            if (paramters.Count <= index - 1)
            {
                paramters.Add(new KeyValuePair<string, object>(parameterName, paramterValue));
                return;
            }

            /*remove*/
            paramters[index - 1] = new KeyValuePair<string, object>(parameterName, paramterValue);
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="paramters">The paramters.</param>
        /// <param name="parameterName"></param>
        /// <param name="paramterValue">The paramter.</param>
        /// <param name="index">The index.</param>
        public static void AddNullableParameter<T>(List<KeyValuePair<string, object>> paramters, int index, string parameterName, Nullable<T> paramterValue) where T : struct, IConvertible
        {
            /*add*/
            if (paramters.Count <= index - 1)
            {
                if (paramterValue.HasValue)
                {
                    paramters.Add(new KeyValuePair<string, object>(parameterName, paramterValue.Value.ToString()));
                    return;
                }

                paramters.Add(new KeyValuePair<string, object>(parameterName, string.Empty));
            }

            /*remove*/
            if (paramterValue.HasValue)
            {
                paramters[index - 1] = (new KeyValuePair<string, object>(parameterName, paramterValue.Value.ToString()));
                return;
            }

            paramters[index - 1] = (new KeyValuePair<string, object>(parameterName, string.Empty));
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="paramters">The paramters.</param>
        /// <param name="parameterName"></param>
        /// <param name="paramterValue">The paramter.</param>
        /// <param name="index">The index.</param>
        public static void AddBoxNullableParameter<T>(List<KeyValuePair<string, object>> paramters, int index, string parameterName, Nullable<T> paramterValue) where T : struct, IConvertible
        {
            /*add*/
            if (paramters.Count <= index - 1)
            {
                if (paramterValue.HasValue)
                {
                    paramters.Add(new KeyValuePair<string, object>(parameterName, paramterValue.Value));
                    return;
                }

                paramters.Add(new KeyValuePair<string, object>(parameterName, null));
            }

            /*remove*/
            if (paramterValue.HasValue)
            {
                paramters[index - 1] = (new KeyValuePair<string, object>(parameterName, paramterValue.Value));
                return;
            }

            paramters[index - 1] = (new KeyValuePair<string, object>(parameterName, null));
        }

        /// <summary>
        /// 在对方法进行调用前
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="invocation">The invocation.</param>
        public static void PreProceed(List<IInterceptor> list, IInvocation invocation)
        {
            foreach (var l in list)
                l.PreProceed(invocation);
        }

        /// <summary>
        /// 在对方法进行调用前
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="invocation">The invocation.</param>
        public static void PostProceed(List<IInterceptor> list, IInvocation invocation)
        {
            foreach (var l in list)
                l.PostProceed(invocation);
        }

        #endregion excuting
    }
}