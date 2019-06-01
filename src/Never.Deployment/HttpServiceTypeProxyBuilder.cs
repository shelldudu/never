using Never.Aop.DynamicProxy;
using Never.Attributes;
using Never.Reflection;
using Never.Serialization;
using Never.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Never.Deployment
{
    /// <summary>
    /// 代理构造者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HttpServiceTypeProxyBuilder<T> : MockBuilder where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        internal static Func<IApiUriDispatcher> Provider { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal static IJsonSerializer JsonSerializer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal static Action<HttpServiceProxyFactory.OnCallingEventArgs> Callback { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Type Build()
        {
            var type = default(Type);
            if (typeof(T).IsInterface)
            {
                return type = new HttpServiceTypeProxyBuilder<T>().CreateInterfaceProxy();
            }

            throw new Exception("target type just can be interface");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Type CreateInterfaceProxy()
        {
            var interfaceType = typeof(T);
            /*检查支持类型*/
            this.SupportType(interfaceType);
            var typeBuilder = EasyEmitBuilder<Type>.NewTypeBuilder(interfaceType.Namespace, interfaceType.Name, null, new[] { typeof(T) });

            //ctor
            var ctorBuilder = typeBuilder.DefineDefaultConstructor(System.Reflection.MethodAttributes.Public);

            /*获取成员，接口全部都是方法*/
            var members = this.GetInterfaceMembers(interfaceType);
            var exists = new List<MemberInfo>(members.Length);
            foreach (var member in members)
            {
                if (exists.Contains(member))
                {
                    continue;
                }

                if (member.MemberType == MemberTypes.Event)
                {
                    this.BuildEvent(interfaceType, new[] { interfaceType }, interfaceType, member, members, exists, typeBuilder);
                    continue;
                }
            }

            foreach (var member in members)
            {
                if (exists.Contains(member))
                {
                    continue;
                }

                if (member.MemberType == MemberTypes.Method)
                {
                    this.BuildMethod(interfaceType, new[] { interfaceType }, interfaceType, member, members, exists, typeBuilder);
                    continue;
                }
            }

            return EasyEmitBuilder<Type>.CreateTypeInfo(typeBuilder);
        }

        /// <summary>
        /// 是否为基元类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool IsPrimitiveOrInsideHandleType(Type type)
        {
            if (TypeHelper.IsPrimitiveType(type))
                return true;

            if (type == typeof(string) || type == typeof(DateTime) || type == typeof(decimal))
                return true;

            return false;
        }

        /// <summary>
        /// 是否为基元类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool IsNullablePrimitiveOrInsideHandleType(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType == null)
                return false;

            if (TypeHelper.IsPrimitiveType(nullableType))
                return true;

            if (nullableType == typeof(string) || nullableType == typeof(DateTime) || nullableType == typeof(decimal))
                return true;

            return false;
        }

        /// <summary>
        /// 构建参数信息
        /// </summary>
        /// <param name="sourceTypeOrInterfaceType">代理或者接口类型</param>
        /// <param name="interfaces">代理所实现的接口</param>
        /// <param name="proxyType">代理类型</param>
        /// <param name="member">方法成员</param>
        /// <param name="members">方法成员</param>
        /// <param name="exists">已经构建的对象</param>
        /// <param name="typeBuilder">类型构建</param>
        protected virtual void BuildEvent(Type sourceTypeOrInterfaceType, Type[] interfaces, Type proxyType, MemberInfo member, MemberInfo[] members, List<MemberInfo> exists, TypeBuilder typeBuilder)
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
                        {
                            break;
                        }
                    }
                    else if (removeName == m.Name)
                    {
                        removeMethod = (MethodInfo)m;
                        exists.Add(m);
                        if (addMethod != null)
                        {
                            break;
                        }
                    }
                }
            }

            var fieldBuilder = typeBuilder.DefineField(@event.Name, @event.EventHandlerType, FieldAttributes.Private);
            var eventBuilder = typeBuilder.DefineEvent(@event.Name, @event.Attributes, @event.EventHandlerType);

            if (addMethod != null)
            {
                if (fieldBuilder == null)
                {
                    fieldBuilder = typeBuilder.DefineField(@event.Name, @event.EventHandlerType, FieldAttributes.Private);
                }

                /*泛型方法和基本方法有什么不同*/
                var parameters = addMethod.GetParameters();
                var parameterTypes = new List<Type>(parameters.Length);
                var voidReturn = addMethod.ReturnType == typeof(void);

                foreach (var parameter in parameters)
                {
                    parameterTypes.Add(parameter.ParameterType);
                }

                var attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
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
                {
                    fieldBuilder = typeBuilder.DefineField(@event.Name, @event.EventHandlerType, FieldAttributes.Private);
                }

                /*泛型方法和基本方法有什么不同*/
                var parameters = removeMethod.GetParameters();
                var parameterTypes = new List<Type>(parameters.Length);
                var voidReturn = removeMethod.ReturnType == typeof(void);

                foreach (var parameter in parameters)
                {
                    parameterTypes.Add(parameter.ParameterType);
                }

                var attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
                var methodBuilder = typeBuilder.DefineMethod(removeMethod.Name,
                    attributes,
                    removeMethod.CallingConvention,
                    removeMethod.ReturnType, parameterTypes.ToArray());

                /*fix argument length*/
                parameterTypes.Insert(0, sourceTypeOrInterfaceType);
                var il = new MockEmitBuilder(methodBuilder.GetILGenerator(), removeMethod.CallingConvention, removeMethod.ReturnType, parameterTypes.ToArray());

                /*调用原来的目标*/
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
        /// 构建参数信息
        /// </summary>
        /// <param name="sourceTypeOrInterfaceType">代理或者接口类型</param>
        /// <param name="interfaces">代理所实现的接口</param>
        /// <param name="proxyType">代理类型</param>
        /// <param name="member">方法成员</param>
        /// <param name="members">方法成员</param>
        /// <param name="exists">已经构建的对象</param>
        /// <param name="typeBuilder">类型构建</param>
        private void BuildMethod(Type sourceTypeOrInterfaceType, Type[] interfaces, Type proxyType, MemberInfo member, MemberInfo[] members, List<MemberInfo> exists, TypeBuilder typeBuilder)
        {
            if (member.MemberType != MemberTypes.Method)
            {
                return;
            }

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
            var attribute = method.GetAttribute<ApiActionRemarkAttribute>();
            if (attribute == null)
            {
                il.LoadConstant(string.Format("该类型{0}方法{1}没有添加ApiActionRemarkAttribute特性，", sourceTypeOrInterfaceType.Name, method.Name));
                il.NewObject(typeof(NotImplementedException).GetConstructor(new[] { typeof(string) }));
                il.Throw();
                il.Return();
                return;
            }

            switch ((attribute.HttpMethod ?? "").ToLower())
            {
                case "get":
                case "httpget":
                    {
                        var dictionlocal = il.DeclareLocal(typeof(Dictionary<string, string>));
                        il.LoadConstant(parameters.Length);
                        il.NewObject(typeof(Dictionary<string, string>), new[] { typeof(int) });
                        il.StoreLocal(dictionlocal);

                        var firstRoutePrimaryKeySelect = string.Empty;
                        for (var i = 0; i < parameters.Length; i++)
                        {
                            if (parameters[i].ParameterType.IsAssignableFromType(typeof(IRoutePrimaryKeySelect)))
                            {
                                il.LoadLocal(dictionlocal);
                                il.LoadConstant(parameters[i].Name);
                                il.LoadArgument((ushort)(i + 1));
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadRoutePrimaryKeySelectParameter", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(parameters[i].ParameterType));
                                firstRoutePrimaryKeySelect = parameters[i].Name;
                                break;
                            }
                        }

                        for (var i = 0; i < parameters.Length; i++)
                        {
                            if (parameters[i].Name == firstRoutePrimaryKeySelect)
                            {
                                continue;
                            }

                            if (parameters[i].IsOut)
                            {
                                var outType = parameters[i].ParameterType.GetElementType();
                                if (outType.IsValueType)
                                {
                                    il.LoadArgumentAddress((ushort)(i + 1));
                                    il.InitializeObject(outType);
                                    il.StoreIndirect(outType);
                                }
                                else
                                {
                                    var ctor = outType.GetConstructor(Type.EmptyTypes);
                                    if (ctor == null)
                                    {
                                        il.LoadArgument((ushort)(i + 1));
                                        il.LoadNull();
                                        il.StoreIndirect(parameters[i].ParameterType);
                                    }
                                    else
                                    {
                                        il.LoadArgument((ushort)(i + 1));
                                        il.NewObject(ctor);
                                        il.StoreIndirect(parameters[i].ParameterType);
                                    }
                                }

                                continue;
                            }

                            if (this.IsPrimitiveOrInsideHandleType(parameters[i].ParameterType))
                            {
                                il.LoadLocal(dictionlocal);
                                il.LoadConstant(parameters[i].Name);
                                il.LoadArgument((ushort)(i + 1));
                                if (parameters[i].ParameterType == typeof(string))
                                {
                                    il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadStringTypeParameter", BindingFlags.Public | BindingFlags.Static));
                                }
                                else
                                {
                                    il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadPrimitiveTypeParameter", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(parameters[i].ParameterType));
                                }

                                continue;
                            }

                            if (parameters[i].ParameterType == typeof(Guid))
                            {
                                il.LoadLocal(dictionlocal);
                                il.LoadConstant(parameters[i].Name);
                                il.LoadArgument((ushort)(i + 1));
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadGuidTypeParameter", BindingFlags.Public | BindingFlags.Static));
                                continue;
                            }

                            if (this.IsNullablePrimitiveOrInsideHandleType(parameters[i].ParameterType))
                            {
                                il.LoadLocal(dictionlocal);
                                il.LoadConstant(parameters[i].Name);
                                il.LoadArgument((ushort)(i + 1));
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadNullPrimitiveTypeParameter", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(Nullable.GetUnderlyingType(parameters[i].ParameterType)));
                                continue;
                            }

                            if (parameters[i].ParameterType == typeof(Guid?))
                            {
                                il.LoadLocal(dictionlocal);
                                il.LoadConstant(parameters[i].Name);
                                il.LoadArgument((ushort)(i + 1));
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadNullGuidTypeParameter", BindingFlags.Public | BindingFlags.Static));
                                continue;
                            }

                            if (parameters[i].ParameterType.IsAssignableFromType(typeof(IRoutePrimaryKeySelect)))
                            {
                                il.LoadLocal(dictionlocal);
                                il.LoadConstant(parameters[i].Name);
                                il.LoadArgument((ushort)(i + 1));
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadRoutePrimaryKeySelectParameter", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(parameters[i].ParameterType));
                                continue;
                            }

                            il.LoadLocal(dictionlocal);
                            il.LoadConstant(parameters[i].Name);
                            il.LoadArgument((ushort)(i + 1));
                            il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadObjectParameter", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), parameters[i].ParameterType));
                            continue;
                        }

                        if (method.ReturnType == typeof(void))
                        {
                            il.LoadLocal(dictionlocal);
                            il.LoadConstant(attribute.UniqueId);
                            il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("VoidGet", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T)));
                            il.Return();
                            return;
                        }

                        if (method.ReturnType == typeof(Task))
                        {
                            il.LoadLocal(dictionlocal);
                            il.LoadConstant(attribute.UniqueId);
                            il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("TaskGet", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T)));
                            il.Return();
                            return;
                        }

                        if (method.ReturnType.IsAssignableFromType(typeof(Task<>)))
                        {
                            il.LoadLocal(dictionlocal);
                            il.LoadConstant(attribute.UniqueId);
                            il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("TaskResultGet", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), method.ReturnType.GetGenericArguments()[0]));
                            il.Return();
                            return;
                        }

                        il.LoadLocal(dictionlocal);
                        il.LoadConstant(attribute.UniqueId);
                        il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("Get", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), method.ReturnType));
                        il.Return();
                        return;
                    }
                case "post":
                case "httppost":
                    {
                        if (parameters.Length <= 0)
                        {
                            if (method.ReturnType == typeof(void))
                            {
                                il.NewObject(typeof(Dictionary<string, string>), Type.EmptyTypes);
                                il.LoadConstant(attribute.UniqueId);
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("VoidPostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T)));
                                il.Return();
                                return;
                            }

                            if (method.ReturnType == typeof(Task))
                            {
                                il.NewObject(typeof(Dictionary<string, string>), Type.EmptyTypes);
                                il.LoadConstant(attribute.UniqueId);
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("TaskPostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T)));
                                il.Return();
                                return;
                            }

                            if (method.ReturnType.IsAssignableFromType(typeof(Task<>)))
                            {
                                il.NewObject(typeof(Dictionary<string, string>), Type.EmptyTypes);
                                il.LoadConstant(attribute.UniqueId);
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("TaskResultPostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), method.ReturnType.GetGenericArguments()[0]));
                                il.Return();
                                return;
                            }

                            il.NewObject(typeof(Dictionary<string, string>), Type.EmptyTypes);
                            il.LoadConstant(attribute.UniqueId);
                            il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("PostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), method.ReturnType));
                            il.Return();
                            return;
                        }

                        if (parameters.Length == 1)
                        {
                            if (parameters[0].IsOut)
                            {
                                var outType = parameters[0].ParameterType.GetElementType();
                                if (outType.IsValueType)
                                {
                                    il.LoadArgumentAddress(1);
                                    il.InitializeObject(outType);
                                }
                                else
                                {
                                    var ctor = outType.GetConstructor(Type.EmptyTypes);
                                    if (ctor == null)
                                    {
                                        il.LoadArgument(1);
                                        il.LoadNull();
                                        il.StoreIndirect(parameters[0].ParameterType);
                                    }
                                    else
                                    {
                                        il.LoadArgument(1);
                                        il.NewObject(ctor);
                                        il.StoreIndirect(parameters[0].ParameterType);
                                    }
                                }
                            }

                            else if (this.IsPrimitiveOrInsideHandleType(parameters[0].ParameterType))
                            {
                                var dictionlocal = il.DeclareLocal(typeof(Dictionary<string, string>));
                                il.LoadConstant(parameters.Length);
                                il.NewObject(typeof(Dictionary<string, string>), new[] { typeof(int) });
                                il.StoreLocal(dictionlocal);
                                il.LoadLocal(dictionlocal);
                                il.LoadConstant(parameters[0].Name);
                                il.LoadArgument(1);
                                if (parameters[0].ParameterType == typeof(string))
                                {
                                    il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadStringTypeParameter", BindingFlags.Public | BindingFlags.Static));
                                }
                                else
                                {
                                    il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadPrimitiveTypeParameter", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(parameters[0].ParameterType));
                                }

                                if (method.ReturnType == typeof(void))
                                {
                                    il.LoadLocal(dictionlocal);
                                    il.LoadConstant(attribute.UniqueId);
                                    il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("VoidPostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T)));
                                    il.Return();
                                    return;
                                }

                                if (method.ReturnType == typeof(Task))
                                {
                                    il.LoadLocal(dictionlocal);
                                    il.LoadConstant(attribute.UniqueId);
                                    il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("TaskPostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T)));
                                    il.Return();
                                    return;
                                }

                                if (method.ReturnType.IsAssignableFromType(typeof(Task<>)))
                                {
                                    il.LoadLocal(dictionlocal);
                                    il.LoadConstant(attribute.UniqueId);
                                    il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("TaskResultPostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), method.ReturnType.GetGenericArguments()[0]));
                                    il.Return();
                                    return;
                                }

                                il.LoadLocal(dictionlocal);
                                il.LoadConstant(attribute.UniqueId);
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("PostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), method.ReturnType));
                                il.Return();
                                return;
                            }

                            else if (this.IsNullablePrimitiveOrInsideHandleType(parameters[0].ParameterType))
                            {
                                var dictionlocal = il.DeclareLocal(typeof(Dictionary<string, string>));
                                il.LoadConstant(parameters.Length);
                                il.NewObject(typeof(Dictionary<string, string>), new[] { typeof(int) });
                                il.StoreLocal(dictionlocal);
                                il.LoadLocal(dictionlocal);
                                il.LoadConstant(parameters[0].Name);
                                il.LoadArgument(1);
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadNullPrimitiveTypeParameter", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(Nullable.GetUnderlyingType(parameters[0].ParameterType)));

                                if (method.ReturnType == typeof(void))
                                {
                                    il.LoadLocal(dictionlocal);
                                    il.LoadConstant(attribute.UniqueId);
                                    il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("VoidPostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T)));
                                    il.Return();
                                    return;
                                }

                                if (method.ReturnType == typeof(Task))
                                {
                                    il.LoadLocal(dictionlocal);
                                    il.LoadConstant(attribute.UniqueId);
                                    il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("TaskPostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T)));
                                    il.Return();
                                    return;
                                }

                                if (method.ReturnType.IsAssignableFromType(typeof(Task<>)))
                                {
                                    il.LoadLocal(dictionlocal);
                                    il.LoadConstant(attribute.UniqueId);
                                    il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("TaskResultPostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), method.ReturnType.GetGenericArguments()[0]));
                                    il.Return();
                                    return;
                                }

                                il.LoadLocal(dictionlocal);
                                il.LoadConstant(attribute.UniqueId);
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("PostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), method.ReturnType));
                                il.Return();
                                return;
                            }

                            if (method.ReturnType == typeof(void))
                            {
                                il.LoadArgument(1);
                                il.LoadConstant(attribute.UniqueId);
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("VoidPost", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), parameters[0].ParameterType));
                                il.Return();
                                return;
                            }
                            if (method.ReturnType == typeof(Task))
                            {
                                il.LoadArgument(1);
                                il.LoadConstant(attribute.UniqueId);
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("TaskPost", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), parameters[0].ParameterType));
                                il.Return();
                                return;
                            }

                            if (method.ReturnType.IsAssignableFromType(typeof(Task<>)))
                            {
                                il.LoadArgument(1);
                                il.LoadConstant(attribute.UniqueId);
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("TaskResultPost", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), parameters[0].ParameterType, method.ReturnType.GetGenericArguments()[0]));
                                il.Return();
                                return;
                            }

                            il.LoadArgument(1);
                            il.LoadConstant(attribute.UniqueId);
                            il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("Post", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), parameters[0].ParameterType, method.ReturnType));
                            il.Return();
                            return;
                        }
                        else
                        {
                            var dictionlocal = il.DeclareLocal(typeof(Dictionary<string, string>));
                            il.LoadConstant(parameters.Length);
                            il.NewObject(typeof(Dictionary<string, string>), new[] { typeof(int) });
                            il.StoreLocal(dictionlocal);
                            var firstRoutePrimaryKeySelect = string.Empty;
                            for (var i = 0; i < parameters.Length; i++)
                            {
                                if (parameters[i].ParameterType.IsAssignableFromType(typeof(IRoutePrimaryKeySelect)))
                                {
                                    il.LoadLocal(dictionlocal);
                                    il.LoadConstant(parameters[i].Name);
                                    il.LoadArgument((ushort)(i + 1));
                                    il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadObjectParameter", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(parameters[i].ParameterType));
                                    firstRoutePrimaryKeySelect = parameters[i].Name;
                                    break;
                                }
                            }

                            for (var i = 0; i < parameters.Length; i++)
                            {
                                if (parameters[i].Name == firstRoutePrimaryKeySelect)
                                {
                                    continue;
                                }

                                if (parameters[i].IsOut)
                                {
                                    var outType = parameters[i].ParameterType.GetElementType();
                                    if (outType.IsValueType)
                                    {
                                        il.LoadArgumentAddress((ushort)(i + 1));
                                        il.InitializeObject(outType);
                                    }
                                    else
                                    {
                                        var ctor = outType.GetConstructor(Type.EmptyTypes);
                                        if (ctor == null)
                                        {
                                            il.LoadArgument((ushort)(i + 1));
                                            il.LoadNull();
                                            il.StoreIndirect(parameters[i].ParameterType);
                                        }
                                        else
                                        {
                                            il.LoadArgument((ushort)(i + 1));
                                            il.NewObject(ctor);
                                            il.StoreIndirect(parameters[i].ParameterType);
                                        }
                                    }

                                    continue;
                                }

                                if (this.IsPrimitiveOrInsideHandleType(parameters[i].ParameterType))
                                {
                                    il.LoadLocal(dictionlocal);
                                    il.LoadConstant(parameters[i].Name);
                                    il.LoadArgument((ushort)(i + 1));
                                    if (parameters[i].ParameterType == typeof(string))
                                    {
                                        il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadStringTypeParameter", BindingFlags.Public | BindingFlags.Static));
                                    }
                                    else
                                    {
                                        il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadPrimitiveTypeParameter", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(parameters[i].ParameterType));
                                    }

                                    continue;
                                }

                                if (this.IsNullablePrimitiveOrInsideHandleType(parameters[i].ParameterType))
                                {
                                    il.LoadLocal(dictionlocal);
                                    il.LoadConstant(parameters[i].Name);
                                    il.LoadArgument((ushort)(i + 1));
                                    il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadNullPrimitiveTypeParameter", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(Nullable.GetUnderlyingType(parameters[i].ParameterType)));
                                    continue;
                                }

                                il.LoadLocal(dictionlocal);
                                il.LoadConstant(parameters[i].Name);
                                il.LoadArgument((ushort)(i + 1));
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("LoadObjectParameter", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), parameters[i].ParameterType));
                                continue;
                            }

                            if (method.ReturnType == typeof(void))
                            {
                                il.LoadLocal(dictionlocal);
                                il.LoadConstant(attribute.UniqueId);
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("VoidPostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T)));
                                il.Return();
                                return;
                            }

                            if (method.ReturnType == typeof(Task))
                            {
                                il.LoadLocal(dictionlocal);
                                il.LoadConstant(attribute.UniqueId);
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("TaskPostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T)));
                                il.Return();
                                return;
                            }

                            if (method.ReturnType.IsAssignableFromType(typeof(Task<>)))
                            {
                                il.LoadLocal(dictionlocal);
                                il.LoadConstant(attribute.UniqueId);
                                il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("TaskPostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), method.ReturnType.GetGenericArguments()[0]));
                                il.Return();
                                return;
                            }

                            il.LoadLocal(dictionlocal);
                            il.LoadConstant(attribute.UniqueId);
                            il.Call(typeof(HttpServiceTypeProxyBuilder<object>).GetMethod("PostByDict", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T), method.ReturnType));
                            il.Return();
                            return;
                        }
                    }
            }

            il.LoadConstant(string.Format("该类型{0}方法{1}的ApiActionRemarkAttribute特性只支持httpget和httppost，", sourceTypeOrInterfaceType.Name, method.Name));
            il.NewObject(typeof(NotImplementedException).GetConstructor(new[] { typeof(string) }));
            il.Throw();
            il.Return();
            return;

        }

        #region 参数
        /// <summary>
        /// 加载参数
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void LoadPrimitiveTypeParameter<TParameter>(Dictionary<string, string> request, string name, TParameter value) where TParameter : struct, IConvertible
        {
            request[name] = value.ToString();
        }

        /// <summary>
        /// 加载参数
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void LoadStringTypeParameter(Dictionary<string, string> request, string name, string value)
        {
            if (value != null)
            {
                request[name] = value.ToString();
            }
            else
            {
                request[name] = string.Empty;
            }
        }

        /// <summary>
        /// 加载参数
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void LoadGuidTypeParameter(Dictionary<string, string> request, string name, Guid value)
        {
            request[name] = value.ToString();
        }

        /// <summary>
        /// 加载参数
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void LoadNullGuidTypeParameter(Dictionary<string, string> request, string name, Guid? value)
        {
            if (value.HasValue)
            {
                request[name] = value.ToString();
            }
            else
            {
                request[name] = string.Empty;
            }
        }

        /// <summary>
        /// 加载参数
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void LoadNullPrimitiveTypeParameter<TParameter>(Dictionary<string, string> request, string name, TParameter? value) where TParameter : struct, IConvertible
        {
            if (value.HasValue)
            {
                request[name] = value.ToString();
            }
            else
            {
                request[name] = string.Empty;
            }
        }

        /// <summary>
        /// 加载参数
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void LoadRoutePrimaryKeySelectParameter<TParameter>(Dictionary<string, string> request, string name, TParameter value) where TParameter : IRoutePrimaryKeySelect
        {
            if (value == null)
            {
                request[name] = string.Empty;
                return;
            }
            else
            {
                request[name] = value.PrimaryKey;
            }
        }

        /// <summary>
        /// 加载参数
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void LoadObjectParameter<Target, TParameter>(Dictionary<string, string> request, string name, TParameter value) where Target : class
        {
            if (value == null)
            {
                request[name] = string.Empty;
                return;
            }
            else
            {
                request[name] = HttpServiceTypeProxyBuilder<Target>.JsonSerializer.Serialize(value);
            }
        }

        #endregion

        #region 请求
        /// <summary>
        /// 发送请求
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Result Post<Target, Request, Result>(Request request, string route) where Target : class
        {
            var dispather = HttpServiceTypeProxyBuilder<Target>.Provider();
            if (dispather == null)
            {
                throw new ArgumentNullException(string.Format("请初始化{0}所关联的ApiUriDispatcher<IApiRouteProvider>对象", typeof(Target).FullName));
            }
            var jsonserialize = HttpServiceTypeProxyBuilder<Target>.JsonSerializer;
            var callback = HttpServiceTypeProxyBuilder<Target>.Callback;
            var jsonData = jsonserialize.Serialize(request);
            UrlConcat url;
            if (request is IRoutePrimaryKeySelect)
            {
                url = dispather.ConcatApiUrl((IRoutePrimaryKeySelect)request, route);
                if (callback != null)
                {
                    callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                    {
                        HttpMethod = "HttpPost",
                        JsonSerializer = jsonserialize,
                        Provider = dispather,
                        Request = jsonData,
                        ReturnType = typeof(Result),
                        Route = route,
                        Url = url
                    });
                }

                return dispather.Post<Result>(jsonserialize, url, jsonData, null);
            }

            url = dispather.ConcatApiUrl(jsonData.Length.ToString(), route);
            if (callback != null)
            {
                callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                {
                    HttpMethod = "HttpPost",
                    JsonSerializer = jsonserialize,
                    Provider = dispather,
                    Request = jsonData,
                    ReturnType = typeof(Result),
                    Route = route,
                    Url = url
                });
            }

            return dispather.Post<Result>(jsonserialize, url, jsonData, null);
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void VoidPost<Target, Request>(Request request, string route) where Target : class
        {
            var dispather = HttpServiceTypeProxyBuilder<Target>.Provider();
            if (dispather == null)
            {
                throw new ArgumentNullException(string.Format("请初始化{0}所关联的ApiUriDispatcher<IApiRouteProvider>对象", typeof(Target).FullName));
            }
            var jsonserialize = HttpServiceTypeProxyBuilder<Target>.JsonSerializer;
            var callback = HttpServiceTypeProxyBuilder<Target>.Callback;
            UrlConcat url;
            var jsonData = jsonserialize.Serialize(request);
            if (request is IRoutePrimaryKeySelect)
            {
                url = dispather.ConcatApiUrl((IRoutePrimaryKeySelect)request, route);
                if (callback != null)
                {
                    callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                    {
                        HttpMethod = "HttpPost",
                        JsonSerializer = jsonserialize,
                        Provider = dispather,
                        Request = jsonData,
                        ReturnType = typeof(void),
                        Route = route,
                        Url = url
                    });
                }

                dispather.Post(jsonserialize, url, jsonData, null);
                return;
            }

            url = dispather.ConcatApiUrl(jsonData.Length.ToString(), route);
            if (callback != null)
            {
                callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                {
                    HttpMethod = "HttpPost",
                    JsonSerializer = jsonserialize,
                    Provider = dispather,
                    Request = jsonData,
                    ReturnType = typeof(void),
                    Route = route,
                    Url = url
                });
            }

            dispather.Post(jsonserialize, url, jsonData, null);
            return;
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Result PostByDict<Target, Result>(Dictionary<string, string> request, string route) where Target : class
        {
            var dispather = HttpServiceTypeProxyBuilder<Target>.Provider();
            if (dispather == null)
            {
                throw new ArgumentNullException(string.Format("请初始化{0}所关联的ApiUriDispatcher<IApiRouteProvider>对象", typeof(Target).FullName));
            }
            var jsonserialize = HttpServiceTypeProxyBuilder<Target>.JsonSerializer;
            var callback = HttpServiceTypeProxyBuilder<Target>.Callback;
            var jsonData = jsonserialize.Serialize(request);
            UrlConcat url;
            if (request.Any())
            {
                url = dispather.ConcatApiUrl(request.FirstOrDefault().Value, route);
                if (callback != null)
                {
                    callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                    {
                        HttpMethod = "HttpPost",
                        JsonSerializer = jsonserialize,
                        Provider = dispather,
                        Request = jsonData,
                        ReturnType = typeof(Result),
                        Route = route,
                        Url = url
                    });
                }

                return dispather.Post<Result>(jsonserialize, url, jsonData, null);
            }

            url = dispather.ConcatApiUrl(jsonData.Length.ToString(), route);
            if (callback != null)
            {
                callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                {
                    HttpMethod = "HttpPost",
                    JsonSerializer = jsonserialize,
                    Provider = dispather,
                    Request = jsonData,
                    ReturnType = typeof(Result),
                    Route = route,
                    Url = url
                });
            }

            return dispather.Post<Result>(jsonserialize, url, jsonData, null);
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void VoidPostByDict<Target>(Dictionary<string, string> request, string route) where Target : class
        {
            var dispather = HttpServiceTypeProxyBuilder<Target>.Provider();
            if (dispather == null)
            {
                throw new ArgumentNullException(string.Format("请初始化{0}所关联的ApiUriDispatcher<IApiRouteProvider>对象", typeof(Target).FullName));
            }
            var jsonserialize = HttpServiceTypeProxyBuilder<Target>.JsonSerializer;
            var callback = HttpServiceTypeProxyBuilder<Target>.Callback;
            var jsonData = jsonserialize.Serialize(request);
            UrlConcat url;

            if (request.Any())
            {
                url = dispather.ConcatApiUrl(request.FirstOrDefault().Value, route);
                if (callback != null)
                {
                    callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                    {
                        HttpMethod = "HttpPost",
                        JsonSerializer = jsonserialize,
                        Provider = dispather,
                        Request = jsonData,
                        ReturnType = typeof(void),
                        Route = route,
                        Url = url
                    });
                }

                dispather.Post(jsonserialize, url, jsonData, null);
                return;
            }

            url = dispather.ConcatApiUrl(jsonData.Length.ToString(), route);
            if (callback != null)
            {
                callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                {
                    HttpMethod = "HttpPost",
                    JsonSerializer = jsonserialize,
                    Provider = dispather,
                    Request = jsonData,
                    ReturnType = typeof(void),
                    Route = route,
                    Url = url
                });
            }

            dispather.Post(jsonserialize, url, jsonData, null);
            return;
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Result Get<Target, Result>(Dictionary<string, string> request, string route) where Target : class
        {
            var dispather = HttpServiceTypeProxyBuilder<Target>.Provider();
            if (dispather == null)
            {
                throw new ArgumentNullException(string.Format("请初始化{0}所关联的ApiUriDispatcher<IApiRouteProvider>对象", typeof(Target).FullName));
            }
            var jsonserialize = HttpServiceTypeProxyBuilder<Target>.JsonSerializer;
            var callback = HttpServiceTypeProxyBuilder<Target>.Callback;
            UrlConcat url;
            if (request.Any())
            {
                url = dispather.ConcatApiUrl(request.FirstOrDefault().Value, route);
                if (callback != null)
                {
                    callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                    {
                        HttpMethod = "HttpGet",
                        JsonSerializer = jsonserialize,
                        Provider = dispather,
                        Request = request,
                        ReturnType = typeof(Result),
                        Route = route,
                        Url = url
                    });
                }

                return dispather.Get<Result>(jsonserialize, url, request, null);
            }

            url = dispather.ConcatApiUrl(request.Count().ToString(), route);
            if (callback != null)
            {
                callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                {
                    HttpMethod = "HttpGet",
                    JsonSerializer = jsonserialize,
                    Provider = dispather,
                    Request = request,
                    ReturnType = typeof(Result),
                    Route = route,
                    Url = url
                });
            }

            return dispather.Get<Result>(jsonserialize, url, request, null);
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void VoidGet<Target>(Dictionary<string, string> request, string route) where Target : class
        {
            var dispather = HttpServiceTypeProxyBuilder<Target>.Provider();
            if (dispather == null)
            {
                throw new ArgumentNullException(string.Format("请初始化{0}所关联的ApiUriDispatcher<IApiRouteProvider>对象", typeof(Target).FullName));
            }
            var jsonserialize = HttpServiceTypeProxyBuilder<Target>.JsonSerializer;
            var callback = HttpServiceTypeProxyBuilder<Target>.Callback;
            UrlConcat url;
            if (request.Any())
            {
                url = dispather.ConcatApiUrl(request.FirstOrDefault().Value, route);
                if (callback != null)
                {
                    callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                    {
                        HttpMethod = "HttpGet",
                        JsonSerializer = jsonserialize,
                        Provider = dispather,
                        Request = request,
                        ReturnType = typeof(void),
                        Route = route,
                        Url = url
                    });
                }

                dispather.Get(jsonserialize, url, request, null);
                return;
            }

            url = dispather.ConcatApiUrl(request.Count().ToString(), route);
            if (callback != null)
            {
                callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                {
                    HttpMethod = "HttpGet",
                    JsonSerializer = jsonserialize,
                    Provider = dispather,
                    Request = request,
                    ReturnType = typeof(void),
                    Route = route,
                    Url = url
                });
            }

            dispather.Get(jsonserialize, url, request, null);
            return;
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Task<Result> TaskResultPost<Target, Request, Result>(Request request, string route) where Target : class
        {
            var dispather = HttpServiceTypeProxyBuilder<Target>.Provider();
            if (dispather == null)
            {
                throw new ArgumentNullException(string.Format("请初始化{0}所关联的ApiUriDispatcher<IApiRouteProvider>对象", typeof(Target).FullName));
            }
            var jsonserialize = HttpServiceTypeProxyBuilder<Target>.JsonSerializer;
            var callback = HttpServiceTypeProxyBuilder<Target>.Callback;
            var jsonData = jsonserialize.Serialize(request);
            UrlConcat url;
            if (request is IRoutePrimaryKeySelect)
            {
                url = dispather.ConcatApiUrl((IRoutePrimaryKeySelect)request, route);
                if (callback != null)
                {
                    callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                    {
                        HttpMethod = "HttpPost",
                        JsonSerializer = jsonserialize,
                        Provider = dispather,
                        Request = request,
                        ReturnType = typeof(Task<Result>),
                        Route = route,
                        Url = url
                    });
                }

                return Task<Result>.Run(() =>
                {
                    return dispather.Post<Result>(jsonserialize, url, jsonData, null);
                });
            }
            else
            {
                url = dispather.ConcatApiUrl(jsonData.Length.ToString(), route);
                if (callback != null)
                {
                    callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                    {
                        HttpMethod = "HttpPost",
                        JsonSerializer = jsonserialize,
                        Provider = dispather,
                        Request = request,
                        ReturnType = typeof(Task<Result>),
                        Route = route,
                        Url = url
                    });
                }

                return Task<Result>.Run(() =>
                {
                    return dispather.Post<Result>(jsonserialize, url, jsonData, null);
                });
            }
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Task TaskPost<Target, Request>(Request request, string route) where Target : class
        {
            var dispather = HttpServiceTypeProxyBuilder<Target>.Provider();
            if (dispather == null)
            {
                throw new ArgumentNullException(string.Format("请初始化{0}所关联的ApiUriDispatcher<IApiRouteProvider>对象", typeof(Target).FullName));
            }
            var jsonserialize = HttpServiceTypeProxyBuilder<Target>.JsonSerializer;
            var callback = HttpServiceTypeProxyBuilder<Target>.Callback;
            var jsonData = jsonserialize.Serialize(request);
            UrlConcat url;
            if (request is IRoutePrimaryKeySelect)
            {
                url = dispather.ConcatApiUrl((IRoutePrimaryKeySelect)request, route);
                if (callback != null)
                {
                    callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                    {
                        HttpMethod = "HttpPost",
                        JsonSerializer = jsonserialize,
                        Provider = dispather,
                        Request = request,
                        ReturnType = typeof(Task),
                        Route = route,
                        Url = url
                    });
                }

                return Task.Run(() =>
                {
                    dispather.Post(jsonserialize, url, jsonData, null);
                });
            }

            url = dispather.ConcatApiUrl(jsonData.Length.ToString(), route);
            if (callback != null)
            {
                callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                {
                    HttpMethod = "HttpPost",
                    JsonSerializer = jsonserialize,
                    Provider = dispather,
                    Request = request,
                    ReturnType = typeof(Task),
                    Route = route,
                    Url = url
                });
            }

            return Task.Run(() =>
            {
                dispather.Post(jsonserialize, url, jsonData, null);
            });
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Task<Result> TaskResultPostByDict<Target, Result>(Dictionary<string, string> request, string route) where Target : class
        {
            var dispather = HttpServiceTypeProxyBuilder<Target>.Provider();
            if (dispather == null)
            {
                throw new ArgumentNullException(string.Format("请初始化{0}所关联的ApiUriDispatcher<IApiRouteProvider>对象", typeof(Target).FullName));
            }
            var jsonserialize = HttpServiceTypeProxyBuilder<Target>.JsonSerializer;
            var callback = HttpServiceTypeProxyBuilder<Target>.Callback;
            var jsonData = jsonserialize.Serialize(request);
            UrlConcat url;
            if (request.Any())
            {
                url = dispather.ConcatApiUrl(request.FirstOrDefault().Value, route);
                if (callback != null)
                {
                    callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                    {
                        HttpMethod = "HttpPost",
                        JsonSerializer = jsonserialize,
                        Provider = dispather,
                        Request = request,
                        ReturnType = typeof(Task<Result>),
                        Route = route,
                        Url = url
                    });
                }

                return Task<Result>.Run(() =>
                {
                    return dispather.Post<Result>(jsonserialize, url, jsonData, null);
                });
            }

            url = dispather.ConcatApiUrl(jsonData.Length.ToString(), route);
            if (callback != null)
            {
                callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                {
                    HttpMethod = "HttpPost",
                    JsonSerializer = jsonserialize,
                    Provider = dispather,
                    Request = request,
                    ReturnType = typeof(Task<Result>),
                    Route = route,
                    Url = url
                });
            }

            return Task<Result>.Run(() =>
            {
                return dispather.Post<Result>(jsonserialize, url, jsonData, null);
            });
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Task TaskPostByDict<Target>(Dictionary<string, string> request, string route) where Target : class
        {
            var dispather = HttpServiceTypeProxyBuilder<Target>.Provider();
            if (dispather == null)
            {
                throw new ArgumentNullException(string.Format("请初始化{0}所关联的ApiUriDispatcher<IApiRouteProvider>对象", typeof(Target).FullName));
            }
            var jsonserialize = HttpServiceTypeProxyBuilder<Target>.JsonSerializer;
            var callback = HttpServiceTypeProxyBuilder<Target>.Callback;
            var jsonData = jsonserialize.Serialize(request);
            UrlConcat url;
            if (request.Any())
            {
                url = dispather.ConcatApiUrl(request.FirstOrDefault().Value, route);
                if (callback != null)
                {
                    callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                    {
                        HttpMethod = "HttpPost",
                        JsonSerializer = jsonserialize,
                        Provider = dispather,
                        Request = request,
                        ReturnType = typeof(Task),
                        Route = route,
                        Url = url
                    });
                }

                return Task.Run(() =>
                {
                    dispather.Post(jsonserialize, url, jsonData, null);
                });
            }

            url = dispather.ConcatApiUrl(jsonData.Length.ToString(), route);
            if (callback != null)
            {
                callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                {
                    HttpMethod = "HttpPost",
                    JsonSerializer = jsonserialize,
                    Provider = dispather,
                    Request = request,
                    ReturnType = typeof(Task),
                    Route = route,
                    Url = url
                });
            }

            return Task.Run(() =>
            {
                dispather.Post(jsonserialize, url, jsonData, null);
            });
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Task<Result> TaskResultGet<Target, Result>(Dictionary<string, string> request, string route) where Target : class
        {
            var dispather = HttpServiceTypeProxyBuilder<Target>.Provider();
            if (dispather == null)
            {
                throw new ArgumentNullException(string.Format("请初始化{0}所关联的ApiUriDispatcher<IApiRouteProvider>对象", typeof(Target).FullName));
            }
            var jsonserialize = HttpServiceTypeProxyBuilder<Target>.JsonSerializer;
            var callback = HttpServiceTypeProxyBuilder<Target>.Callback;
            UrlConcat url;
            if (request.Any())
            {
                url = dispather.ConcatApiUrl(request.FirstOrDefault().Value, route);
                if (callback != null)
                {
                    callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                    {
                        HttpMethod = "HttpGet",
                        JsonSerializer = jsonserialize,
                        Provider = dispather,
                        Request = request,
                        ReturnType = typeof(Task<Result>),
                        Route = route,
                        Url = url
                    });
                }

                return Task<Result>.Run(() =>
                {
                    return dispather.Get<Result>(jsonserialize, url, request, null);
                });

            }

            url = dispather.ConcatApiUrl(request.Count().ToString(), route);
            if (callback != null)
            {
                callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                {
                    HttpMethod = "HttpGet",
                    JsonSerializer = jsonserialize,
                    Provider = dispather,
                    Request = request,
                    ReturnType = typeof(Task<Result>),
                    Route = route,
                    Url = url
                });
            }

            return Task<Result>.Run(() =>
            {
                return dispather.Get<Result>(jsonserialize, url, request, null);
            });
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static Task TaskGet<Target>(Dictionary<string, string> request, string route) where Target : class
        {
            var dispather = HttpServiceTypeProxyBuilder<Target>.Provider();
            if (dispather == null)
            {
                throw new ArgumentNullException(string.Format("请初始化{0}所关联的ApiUriDispatcher<IApiRouteProvider>对象", typeof(Target).FullName));
            }
            var jsonserialize = HttpServiceTypeProxyBuilder<Target>.JsonSerializer;
            var callback = HttpServiceTypeProxyBuilder<Target>.Callback;
            UrlConcat url;
            if (request.Any())
            {
                url = dispather.ConcatApiUrl(request.FirstOrDefault().Value, route);
                if (callback != null)
                {
                    callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                    {
                        HttpMethod = "HttpGet",
                        JsonSerializer = jsonserialize,
                        Provider = dispather,
                        Request = request,
                        ReturnType = typeof(Task),
                        Route = route,
                        Url = url
                    });
                }

                return Task.Run(() =>
                {
                    dispather.Get(jsonserialize, url, request, null);
                });
            }

            url = dispather.ConcatApiUrl(request.Count().ToString(), route);
            if (callback != null)
            {
                callback.Invoke(new HttpServiceProxyFactory.OnCallingEventArgs()
                {
                    HttpMethod = "HttpGet",
                    JsonSerializer = jsonserialize,
                    Provider = dispather,
                    Request = request,
                    ReturnType = typeof(Task),
                    Route = route,
                    Url = url
                });
            }

            return Task.Run(() =>
            {
                dispather.Get(jsonserialize, url, request, null);
            });
        }
        #endregion
    }
}