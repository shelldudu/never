using Never.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Never.Aop.DynamicProxy.Builders
{
    /// <summary>
    /// 构建T对象的虚拟实现
    /// </summary>
    public class IllusiveMockBuilder : MockBuilder
    {
        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="targetType"></param>
        protected IllusiveMockBuilder(List<MockSetup> rules, Type targetType)
        {
            this.rules = rules ?? new List<MockSetup>();
            this.TargetType = targetType;
        }

        #endregion ctor

        #region field prop

        /// <summary>
        /// 规则集合
        /// </summary>
        private readonly ICollection<MockSetup> rules = null;

        /// <summary>
        /// 目标类型
        /// </summary>
        private readonly Type TargetType = null;

        #endregion field prop

        #region supportType

        /// <summary>
        /// 支持的类型
        /// </summary>
        /// <param name="type">The type.</param>
        public override void SupportType(Type type)
        {
            /*不可见，则是private或者protected或者internal的*/
            if (!type.IsVisible)
                throw new ArgumentNullException("mock service type only support visible interface and class , and the class is not an sealed classs");

            /*支持公开的接口*/
            if (type.IsInterface)
                return;

            /*支持非密封类*/
            if (type.IsClass && !type.IsSealed)
                return;

            throw new ArgumentNullException("mock service type only support visible interface and class , and the class is not an sealed classs");
        }

        #endregion supportType

        #region build

        /// <summary>
        /// 进行构建
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public virtual Type BuildClass(Type sourceType)
        {
            /*检查支持类型*/
            this.SupportType(sourceType);

            /*是接口类型，则要在内存中生成一个类，用于实现该接口所有定义*/
            /*生命一个类对象构造*/
            var nextTypename = this.GetNextTypeName(sourceType);
            var typeBuilder = EasyEmitBuilder<Type>.NewTypeBuilder(nextTypename.NameSplace, nextTypename.TypeName, TypeAttributes.Public, sourceType, Type.EmptyTypes);

            /*获取成员，接口全部都是方法，对象可能返回，属性，字段等成员，但我们主要是代理方法*/
            var members = this.GetMembers(sourceType);
            var exists = new List<MemberInfo>(members.Length);
            foreach (var member in members)
            {
                if (exists.Contains(member))
                    continue;

                if (member.MemberType == MemberTypes.Field)
                {
                    this.BuildField(member, members, exists, typeBuilder, sourceType, false);
                    continue;
                }

                if (member.MemberType == MemberTypes.Property)
                {
                    this.BuildProperty(member, members, exists, typeBuilder, sourceType, false);
                    continue;
                }

                if (member.MemberType == MemberTypes.Event)
                {
                    this.BuildEvent(member, members, exists, typeBuilder, sourceType, false);
                    continue;
                }
            }

            foreach (var member in members)
            {
                if (exists.Contains(member))
                    continue;

                if (member.MemberType == MemberTypes.Method)
                {
                    this.BuildMethod(member, members, exists, typeBuilder, sourceType, false);
                    continue;
                }
            }

            return EasyEmitBuilder<Type>.CreateTypeInfo(typeBuilder);
        }

        /// <summary>
        /// 进行构建
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public virtual Type BuildInterface(Type interfaceType)
        {
            /*检查支持类型*/
            this.SupportType(interfaceType);

            /*是接口类型，则要在内存中生成一个类，用于实现该接口所有定义*/
            /*生命一个类对象构造*/
            var nextTypename = this.GetNextTypeName(interfaceType);
            var typeBuilder = EasyEmitBuilder<Type>.NewTypeBuilder(nextTypename.NameSplace, nextTypename.TypeName, typeof(object), new[] { interfaceType });

            /*构造函数*/
            var ctorBuilder = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            /*获取成员，接口全部都是方法*/
            var members = this.GetInterfaceMembers(interfaceType);
            var exists = new List<MemberInfo>(members.Length);
            foreach (var member in members)
            {
                if (exists.Contains(member))
                    continue;

                if (member.MemberType == MemberTypes.Event)
                {
                    this.BuildEvent(member, members, exists, typeBuilder, interfaceType, true);
                    continue;
                }

                if (member.MemberType == MemberTypes.Property)
                {
                    this.BuildProperty(member, members, exists, typeBuilder, interfaceType, true);
                    continue;
                }
            }

            foreach (var member in members)
            {
                if (exists.Contains(member))
                    continue;

                if (member.MemberType == MemberTypes.Method)
                {
                    this.BuildMethod(member, members, exists, typeBuilder, interfaceType, true);
                    continue;
                }
            }

            return EasyEmitBuilder<Type>.CreateTypeInfo(typeBuilder);
        }

        /// <summary>
        /// 构建属性
        /// </summary>
        /// <param name="member"></param>
        /// <param name="members"></param>
        /// <param name="exists"></param>
        /// <param name="typeBuilder">类型构建</param>
        /// <param name="sourceTypeOrInterfaceType">假如是承继父类的，则callBase就能生效</param>
        /// <param name="buildInterface">构建接口</param>
        protected virtual void BuildProperty(MemberInfo member, MemberInfo[] members, List<MemberInfo> exists, TypeBuilder typeBuilder, Type sourceTypeOrInterfaceType, bool @buildInterface)
        {
            var property = member as PropertyInfo;
            exists.Add(property);
            var getName = string.Concat("get_", property.Name);
            var setName = string.Concat("set_", property.Name);
            MethodInfo getMethod = null, setMethod = null;
            foreach (var m in members)
            {
                if (m.MemberType == MemberTypes.Method)
                {
                    if (getName == m.Name)
                    {
                        getMethod = (MethodInfo)m;
                        exists.Add(m);
                        if (setMethod != null)
                            break;
                    }
                    else if (setName == m.Name)
                    {
                        setMethod = (MethodInfo)m;
                        exists.Add(m);
                        if (getMethod != null)
                            break;
                    }
                }
            }

            foreach (var found in this.rules)
            {
                if (found.Property == property)
                {
                    if (getMethod != null)
                    {
                        this.BuildMethod(getMethod, members, exists, typeBuilder, sourceTypeOrInterfaceType, buildInterface, found);
                    }

                    if (setMethod != null)
                    {
                        /*泛型方法和基本方法有什么不同*/
                        var parameters = setMethod.GetParameters();
                        var parameterTypes = new List<Type>(parameters.Length);
                        foreach (var parameter in parameters)
                            parameterTypes.Add(parameter.ParameterType);

                        var attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
                        if (!buildInterface)
                            attributes = MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig;

                        var methodBuilder = typeBuilder.DefineMethod(setMethod.Name,
                            attributes,
                            setMethod.CallingConvention,
                            setMethod.ReturnType, parameterTypes.ToArray());

                        /*fix argument length*/
                        parameterTypes.Insert(0, sourceTypeOrInterfaceType);

                        var il = new MockEmitBuilder(methodBuilder.GetILGenerator(), setMethod.CallingConvention, setMethod.ReturnType, parameterTypes.ToArray());
                        il.Return();
                    }
                }
            }
        }

        /// <summary>
        /// 构建事件
        /// </summary>
        /// <param name="member"></param>
        /// <param name="members"></param>
        /// <param name="exists"></param>
        /// <param name="typeBuilder">类型构建</param>
        /// <param name="sourceTypeOrInterfaceType">假如是承继父类的，则callBase就能生效</param>
        /// <param name="buildInterface">构建接口</param>
        protected virtual void BuildEvent(MemberInfo member, MemberInfo[] members, List<MemberInfo> exists, TypeBuilder typeBuilder, Type sourceTypeOrInterfaceType, bool @buildInterface)
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

            FieldBuilder fieldBuilder = null;
            EventBuilder eventBuilder = null;

            if (!@buildInterface && (addMethod == null || addMethod.IsFinal) && (removeMethod == null || removeMethod.IsFinal))
                return;

            eventBuilder = typeBuilder.DefineEvent(@event.Name, @event.Attributes, @event.EventHandlerType);

            if (addMethod != null)
            {
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
                if (!buildInterface)
                    attributes = MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig;

                var methodBuilder = typeBuilder.DefineMethod(addMethod.Name,
                    attributes,
                    addMethod.CallingConvention,
                    addMethod.ReturnType, parameterTypes.ToArray());

                /*fix argument length*/
                parameterTypes.Insert(0, sourceTypeOrInterfaceType);
                var il = new MockEmitBuilder(methodBuilder.GetILGenerator(), addMethod.CallingConvention, addMethod.ReturnType, parameterTypes.ToArray());
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
                if (!buildInterface)
                    attributes = MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig;

                var methodBuilder = typeBuilder.DefineMethod(removeMethod.Name,
                    attributes,
                    removeMethod.CallingConvention,
                    removeMethod.ReturnType, parameterTypes.ToArray());

                /*fix argument length*/
                parameterTypes.Insert(0, sourceTypeOrInterfaceType);
                var il = new MockEmitBuilder(methodBuilder.GetILGenerator(), removeMethod.CallingConvention, removeMethod.ReturnType, parameterTypes.ToArray());
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

                @eventBuilder.SetRemoveOnMethod(methodBuilder);
            }
        }

        /// <summary>
        /// 构建事件
        /// </summary>
        /// <param name="member"></param>
        /// <param name="members"></param>
        /// <param name="exists"></param>
        /// <param name="typeBuilder">类型构建</param>
        /// <param name="sourceTypeOrInterfaceType">假如是承继父类的，则callBase就能生效</param>
        /// <param name="buildInterface">构建接口</param>
        protected virtual void BuildField(MemberInfo member, MemberInfo[] members, List<MemberInfo> exists, TypeBuilder typeBuilder, Type sourceTypeOrInterfaceType, bool @buildInterface)
        {
            var field = member as FieldInfo;
            exists.Add(field);
        }

        /// <summary>
        /// 构建方法
        /// </summary>
        /// <param name="member"></param>
        /// <param name="members"></param>
        /// <param name="exists"></param>
        /// <param name="typeBuilder">类型构建</param>
        /// <param name="sourceTypeOrInterfaceType">假如是承继父类的，则callBase就能生效</param>
        /// <param name="buildInterface">构建接口</param>
        protected virtual void BuildMethod(MemberInfo member, MemberInfo[] members, List<MemberInfo> exists, TypeBuilder typeBuilder, Type sourceTypeOrInterfaceType, bool @buildInterface)
        {
            var method = member as MethodInfo;
            foreach (var rule in this.rules)
            {
                if (rule.Method == method)
                {
                    exists.Add(member);
                    this.BuildMethod(member, members, exists, typeBuilder, sourceTypeOrInterfaceType, buildInterface, rule);
                    return;
                }
            }

            exists.Add(member);
            this.BuildMethod(member, members, exists, typeBuilder, sourceTypeOrInterfaceType, buildInterface, null);
            return;
        }

        /// <summary>
        /// 构建方法
        /// </summary>
        /// <param name="member"></param>
        /// <param name="members"></param>
        /// <param name="exists"></param>
        /// <param name="typeBuilder">类型构建</param>
        /// <param name="sourceTypeOrInterfaceType">假如是承继父类的，则callBase就能生效</param>
        /// <param name="buildInterface">构建接口</param>
        /// <param name="found"></param>
        private void BuildMethod(MemberInfo member, MemberInfo[] members, List<MemberInfo> exists, TypeBuilder typeBuilder, Type sourceTypeOrInterfaceType, bool @buildInterface, MockSetup found)
        {
            var method = member as MethodInfo;
            if (!@buildInterface && method.IsFinal)
                return;

            /*泛型方法和基本方法有什么不同*/
            var parameters = method.GetParameters();
            var parameterTypes = new List<Type>(parameters.Length);
            var voidReturn = method.ReturnType == typeof(void);
            foreach (var parameter in parameters)
            {
                parameterTypes.Add(parameter.ParameterType);
            }

            var attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
            if (!buildInterface)
                attributes = MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig;

            var methodBuilder = typeBuilder.DefineMethod(method.Name,
                attributes,
                method.CallingConvention,
                method.ReturnType, parameterTypes.ToArray());

            /*fix argument length*/
            parameterTypes.Insert(0, sourceTypeOrInterfaceType);

            var il = new MockEmitBuilder(methodBuilder.GetILGenerator(), method.CallingConvention, method.ReturnType, parameterTypes.ToArray());
            if (found == null)
                goto _notImpl;

            goto _impl;

            _notImpl:
            {
                if (voidReturn)
                {
                    /*out 参数*/
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        if (parameters[i].IsOut)
                        {
                            il.LoadArgument((ushort)(i + 1));
                            var ele = parameters[i].ParameterType.GetElementType();
                            if (ele.IsValueType)
                            {
                                if (this.IsEnumType(ele) || this.IsNullableEnumType(ele))
                                {
                                    il.Call(typeof(MockSetupInfoStore).GetMethod("ReturnDefault").MakeGenericMethod(new[] { ele }));
                                    il.StoreIndirect(Enum.GetUnderlyingType(ele));
                                }
                                else
                                {
                                    il.Call(typeof(MockSetupInfoStore).GetMethod("ReturnDefault").MakeGenericMethod(new[] { ele }));
                                    il.StoreIndirect(ele);
                                }
                            }
                            else
                            {
                                il.LoadNull();
                                il.StoreIndirect(parameters[i].ParameterType);
                            }
                        }
                    }

                    il.Return();
                    return;
                }

                il.NewObject(typeof(NotImplementedException), Type.EmptyTypes);
                il.Throw();
                il.Return();
                return;
            };
            _impl:
            {
                /*void*/
                if (voidReturn)
                {
                    if (found.MethodToCallType == MockSetup.MethodToCall.Exception || found.MethodToCallType == MockSetup.MethodToCall.Void)
                    {
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(method.Name, string.Format("the method {0}.{1} return type is void, but mock donot  provide action callback;", this.TargetType.Name, method.Name));
                    }
                }
                else
                {
                    /*返回结果类型不匹配*/
                    if (found.GetType() != typeof(MockSetup<,>).MakeGenericType(new[] { this.TargetType, method.ReturnType }))
                        goto _notImpl;
                }

                /*返回基类类型，如果当前是接口类型的，则抛出异常*/
                if (found.MethodIndex == 0)
                {
                    if (@buildInterface)
                        goto _notImpl;

                    //this
                    foreach (var m in this.GetMembers(sourceTypeOrInterfaceType))
                    {
                        if (m.MemberType == MemberTypes.Method)
                        {
                            var pm = m as MethodInfo;
                            var pp = pm.GetParameters();
                            if (pm.Name == found.Method.Name && pp.Length == found.Method.GetParameters().Length)
                            {
                                il.LoadArgument(0);
                                for (ushort i = 1; i < pp.Length + 1; i++)
                                    il.LoadArgument(i);

                                il.Call(pm);
                                il.Return();
                                return;
                            }
                        }
                    }

                    throw new ArgumentOutOfRangeException(method.Name, string.Format("the method {0}.{1} cannot been found;", this.TargetType.Name, method.Name));
                }

                /*用该值定位到执行的是哪个方法，因重载的原因，有15个方法以上，具体要看MockSetup<,>*/
                var idx = MockSetupInfoStore.Enqueue(found);

                /*抛出异常*/
                if (found.MethodIndex == -1)
                {
                    il.LoadArgument(0);
                    il.LoadConstant(idx);
                    if (voidReturn)
                        il.Call(typeof(MockSetupInfoStore).GetMethod("Call_ExceptionWithNoResult").MakeGenericMethod(new[] { this.TargetType }));
                    else
                        il.Call(typeof(MockSetupInfoStore).GetMethod("Call_ExceptionWithResult").MakeGenericMethod(new[] { this.TargetType, method.ReturnType }));
                    il.Return();
                    return;
                }

                var callMethodName = typeof(MockSetupInfoStore).GetMethod(string.Concat("Call_", found.MethodIndex.ToString()));
                var makeGenericMethodParameterTypes = new List<Type>(found.MethodIndex);
                makeGenericMethodParameterTypes.Add(this.TargetType);

                if (found.MethodIndex >= 10 && found.MethodIndex <= 25)
                {
                    /*没有参数的方法*/
                    if (found.MethodIndex == 10)
                    {
                        /*out 参数*/
                        for (var i = 0; i < parameters.Length; i++)
                        {
                            if (parameters[i].IsOut)
                            {
                                il.LoadArgument((ushort)(i + 1));
                                var ele = parameters[i].ParameterType.GetElementType();
                                if (ele.IsValueType)
                                {
                                    if (this.IsEnumType(ele) || this.IsNullableEnumType(ele))
                                    {
                                        il.Call(typeof(MockSetupInfoStore).GetMethod("ReturnDefault").MakeGenericMethod(new[] { ele }));
                                        il.StoreIndirect(Enum.GetUnderlyingType(ele));
                                    }
                                    else
                                    {
                                        il.Call(typeof(MockSetupInfoStore).GetMethod("ReturnDefault").MakeGenericMethod(new[] { ele }));
                                        il.StoreIndirect(ele);
                                    }
                                }
                                else
                                {
                                    il.LoadNull();
                                    il.StoreIndirect(parameters[i].ParameterType);
                                }
                            }
                        }

                        makeGenericMethodParameterTypes.Add(method.ReturnType);
                        il.LoadArgument(0);
                        il.LoadConstant(idx);
                        il.Call(callMethodName.MakeGenericMethod(makeGenericMethodParameterTypes.ToArray()));
                        il.Return();
                        return;
                    }

                    if (found.CallbackMethodParameters == null || found.CallbackMethodParameters.Length == 0)
                        throw new ArgumentOutOfRangeException(method.Name, string.Format("{0}.{1} method need {2} parameters,but mock provider 0 parameters", this.TargetType.Name, method.Name, parameterTypes.Count));

                    if (parameterTypes.Count != found.CallbackMethodParameters.Length)
                        throw new ArgumentException(string.Format("{0}.{1} method need {2} parameters,but the mock provide {3} parameters", this.TargetType.Name, method.Name, parameters.Length.ToString(), found.CallbackMethodParameters.Length.ToString()), method.Name);

                    for (var i = 1; i < found.CallbackMethodParameters.Length; i++)
                    {
                        if (parameterTypes[i] != found.CallbackMethodParameters[i].ParameterType)
                            throw new ArgumentException(string.Format("{0}.{1} method the {2} parameter type is {3},but the mock provide {4} type", this.TargetType.Name, method.Name, i.ToString(), parameterTypes[i].Name, found.CallbackMethodParameters[i].ParameterType.Name), method.Name);

                        makeGenericMethodParameterTypes.Add(found.CallbackMethodParameters[i].ParameterType);
                    }

                    makeGenericMethodParameterTypes.Add(method.ReturnType);

                    /*out 参数*/
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        if (parameters[i].IsOut)
                        {
                            il.LoadArgument((ushort)(i + 1));
                            var ele = parameters[i].ParameterType.GetElementType();
                            if (ele.IsValueType)
                            {
                                if (this.IsEnumType(ele) || this.IsNullableEnumType(ele))
                                {
                                    il.Call(typeof(MockSetupInfoStore).GetMethod("ReturnDefault").MakeGenericMethod(new[] { ele }));
                                    il.StoreIndirect(Enum.GetUnderlyingType(ele));
                                }
                                else
                                {
                                    il.Call(typeof(MockSetupInfoStore).GetMethod("ReturnDefault").MakeGenericMethod(new[] { ele }));
                                    il.StoreIndirect(ele);
                                }
                            }
                            else
                            {
                                il.LoadNull();
                                il.StoreIndirect(parameters[i].ParameterType);
                            }
                        }
                    }

                    il.LoadArgument(0);
                    il.LoadConstant(idx);
                    for (ushort i = 1; i < makeGenericMethodParameterTypes.Count - 1; i++)
                        il.LoadArgument(i);

                    il.Call(callMethodName.MakeGenericMethod(makeGenericMethodParameterTypes.ToArray()));
                    il.Return();
                    return;
                }

                if (found.MethodIndex >= 30 && found.MethodIndex <= 45)
                {
                    /*没有参数的方法*/
                    if (found.MethodIndex == 30)
                    {
                        /*out 参数*/
                        for (var i = 0; i < parameters.Length; i++)
                        {
                            if (parameters[i].IsOut)
                            {
                                il.LoadArgument((ushort)(i + 1));
                                var ele = parameters[i].ParameterType.GetElementType();
                                if (ele.IsValueType)
                                {
                                    if (this.IsEnumType(ele) || this.IsNullableEnumType(ele))
                                    {
                                        il.Call(typeof(MockSetupInfoStore).GetMethod("ReturnDefault").MakeGenericMethod(new[] { ele }));
                                        il.StoreIndirect(Enum.GetUnderlyingType(ele));
                                    }
                                    else
                                    {
                                        il.Call(typeof(MockSetupInfoStore).GetMethod("ReturnDefault").MakeGenericMethod(new[] { ele }));
                                        il.StoreIndirect(ele);
                                    }
                                }
                                else
                                {
                                    il.LoadNull();
                                    il.StoreIndirect(parameters[i].ParameterType);
                                }
                            }
                        }

                        il.LoadArgument(0);
                        il.LoadConstant(idx);
                        il.Call(callMethodName.MakeGenericMethod(makeGenericMethodParameterTypes.ToArray()));
                        il.Return();
                        return;
                    }

                    if (parameterTypes.Count != found.CallbackMethodParameters.Length)
                        throw new ArgumentException(string.Format("{0}.{1} method need {2} parameters,but the mock provide {3} parameters", this.TargetType.Name, method.Name, parameters.Length.ToString(), found.CallbackMethodParameters.Length.ToString()), method.Name);

                    for (var i = 1; i < found.CallbackMethodParameters.Length; i++)
                    {
                        if (parameterTypes[i] != found.CallbackMethodParameters[i].ParameterType)
                            throw new ArgumentException(string.Format("{0}.{1} method the {2} parameter type is {3},but the mock provide {4} type", this.TargetType.Name, method.Name, i.ToString(), parameterTypes[i].Name, found.CallbackMethodParameters[i].ParameterType.Name), method.Name);

                        makeGenericMethodParameterTypes.Add(found.CallbackMethodParameters[i].ParameterType);
                    }

                    /*out 参数*/
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        if (parameters[i].IsOut)
                        {
                            il.LoadArgument((ushort)(i + 1));
                            var ele = parameters[i].ParameterType.GetElementType();
                            if (ele.IsValueType)
                            {
                                if (this.IsEnumType(ele) || this.IsNullableEnumType(ele))
                                {
                                    il.Call(typeof(MockSetupInfoStore).GetMethod("ReturnDefault").MakeGenericMethod(new[] { ele }));
                                    il.StoreIndirect(Enum.GetUnderlyingType(ele));
                                }
                                else
                                {
                                    il.Call(typeof(MockSetupInfoStore).GetMethod("ReturnDefault").MakeGenericMethod(new[] { ele }));
                                    il.StoreIndirect(ele);
                                }
                            }
                            else
                            {
                                il.LoadNull();
                                il.StoreIndirect(parameters[i].ParameterType);
                            }
                        }
                    }

                    il.LoadArgument(0);
                    il.LoadConstant(idx);
                    for (ushort i = 1; i < makeGenericMethodParameterTypes.Count; i++)
                        il.LoadArgument(i);

                    il.Call(callMethodName.MakeGenericMethod(makeGenericMethodParameterTypes.ToArray()));
                    il.Return();
                    return;
                }
            };
        }

        #endregion build
    }
}