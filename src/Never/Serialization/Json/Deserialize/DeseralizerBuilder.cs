using Never.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Never.Serialization.Json.Deserialize
{
    /// <summary>
    /// 反序列化
    /// </summary>
    public class DeseralizerBuilder<T> : JsonEmitBuilder<T>
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="DeseralizerBuilder{T}"/> class.
        /// </summary>
        protected DeseralizerBuilder()
            : base(new List<string>(2), StringComparison.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeseralizerBuilder{T}"/> class.
        /// </summary>
        /// <param name="ignoredMembers">The ignored members.</param>
        /// <param name="stringComparer">The string comparer.</param>
        protected DeseralizerBuilder(List<string> ignoredMembers, StringComparison stringComparer)
             : base(ignoredMembers, stringComparer)
        {
        }

        #endregion ctor

        #region build

        /// <summary>
        /// 构建信息写入流中,当前只处理基元类型
        /// </summary>
        /// <param name="emit">emit操作</param>
        protected virtual void Build(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit)
        {
            var instanceLocal = emit.DeclareLocal(this.TargetType);
            if (this.TargetType.IsValueType)
            {
                if (Nullable.GetUnderlyingType(this.TargetType) != null)
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallNullableObjectInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(Nullable.GetUnderlyingType(this.TargetType)));
                    return;
                }

                emit.LoadLocalAddress(instanceLocal);
                emit.InitializeObject(this.TargetType);
                this.BuildMembers(emit, instanceLocal, this.TargetType, this.GetMembers(this.TargetType));
                emit.LoadLocal(instanceLocal);
                return;
            }

            var ctor = this.GetConstructor();
            if (ctor == null)
            {
                emit.LoadLocalAddress(instanceLocal);
                emit.InitializeObject(this.TargetType);
                emit.LoadLocal(instanceLocal);
                return;
            }

            emit.NewObject(this.TargetType, Type.EmptyTypes);
            emit.StoreLocal(instanceLocal);
            this.BuildMembers(emit, instanceLocal, this.TargetType, this.GetMembers(this.TargetType));
            emit.LoadLocal(instanceLocal);
        }

        /// <summary>
        /// 查询构造函数
        /// </summary>
        /// <returns></returns>
        protected virtual ConstructorInfo GetConstructor()
        {
            var ctor = this.TargetType.GetConstructor(Type.EmptyTypes);
            if (ctor != null)
                return ctor;

            var ctors = this.TargetType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            foreach (var c in ctors)
            {
                if (c.GetParameters().Length == 0)
                    return c;
            }

            return null;
        }

        #endregion build

        #region build members

        /// <summary>
        /// 构建非洋葱类型的基本信息写入流中
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="cepaSourceType">当前节点的非洋葱类型</param>
        protected virtual void BuildNotCepaType(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, Type cepaSourceType)
        {
            /*type*/
            if (this.BuildForTypeModule(emit, cepaSourceType))
                return;

            /*异常*/
            if (this.BuildForExceptionModule(emit, cepaSourceType))
                return;

            /*enum类型*/
            if (this.BuildForEnumModule(emit, cepaSourceType))
                return;

            /*基元类型*/
            if (this.BuildForPrimitiveModule(emit, cepaSourceType))
                return;

            /*dict*/
            if (this.BuildForDictionaryModule(emit, cepaSourceType))
                return;

            /*array*/
            if (this.BuildForArrayModule(emit, cepaSourceType))
                return;

            if (cepaSourceType == typeof(object))
                this.BuildForStringTypeModule(emit, cepaSourceType);

            return;
        }

        /// <summary>
        /// 构建基本信息写入流中
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="instanceLocal">当前对象</param>
        /// <param name="sourceType">当前节点的成员类型</param>
        /// <param name="members">所有成员</param>
        protected virtual void BuildMembers(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, ILocal instanceLocal, Type sourceType, MemberInfo[] members)
        {
            if (members == null || members.Length == 0)
                return;

            var recursionType = ContainRecursionReference(sourceType);
            if (recursionType != null)
                throw new ArgumentException(string.Format("类型{0}与{1}形成递归引用", sourceType.FullName, recursionType.FullName));

            for (var i = 0; i < members.Length; i++)
            {
                var member = members[i];
                if (this.IsContainIgnoredMember(member))
                    continue;

                /*所有属性*/
                var attributes = this.LoadAttributes(member, true);
                if (this.ContainIgoreAttribute(member, attributes))
                    continue;

                Type memberType = null;
                if (member.MemberType == MemberTypes.Property)
                {
                    if (!((PropertyInfo)member).CanWrite)
                        continue;

                    memberType = ((PropertyInfo)member).PropertyType;
                }
                else
                {
                    memberType = ((FieldInfo)member).FieldType;
                }

                /*自定义序列化*/
                if (CustomSerializationProvider.ContainCustomeSerilizerbuilder(memberType))
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, attributes));
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallBuilderInvoke", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(memberType));
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    continue;
                }

                if (this.BuildForTypeModule(emit, instanceLocal, sourceType, member, memberType, attributes))
                    continue;

                if (this.BuildForExceptionModule(emit, instanceLocal, sourceType, member, memberType, attributes))
                    continue;

                if (this.BuildForPrimitiveModule(emit, instanceLocal, sourceType, member, memberType, attributes))
                    continue;

                if (this.BuildForEnumModule(emit, instanceLocal, sourceType, member, memberType, attributes))
                    continue;

                if (this.BuildForDictionaryModule(emit, instanceLocal, sourceType, member, memberType, attributes))
                    continue;

                if (this.BuildForArrayModule(emit, instanceLocal, sourceType, member, memberType, attributes))
                    continue;

                /*复合对象*/
                if (this.IsComplexType(memberType))
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, attributes));
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(memberType));
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    continue;
                }

                if (memberType == typeof(object))
                    this.BuildForStringTypeModule(emit, instanceLocal, sourceType, member, memberType, attributes);

                continue;
            }
        }

        /// <summary>
        /// 构建字典
        /// </summary>
        /// <param name="emit"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        protected virtual bool BuildForDictionaryModule(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, Type memberType)
        {
            /*不是集合*/
            if (!this.IsAssignableFrom(memberType, typeof(IEnumerable)))
                return false;

            if (this.IsContainType(memberType, typeof(IDictionary<,>)))
            {
                /*接口类型，用Dictionary对象或Hashtable*/
                if (memberType.IsInterface)
                {
                    /*结果集*/
                    var hashtableResultLocal = emit.DeclareLocal(typeof(Dictionary<,>).MakeGenericType(memberType.GetGenericArguments()));
                    emit.NewObject(hashtableResultLocal.LocalType, Type.EmptyTypes);
                    emit.StoreLocal(hashtableResultLocal);
                    emit.LoadLocal(hashtableResultLocal);
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    if (memberType.GetGenericArguments()[0] == typeof(string))
                    {
                        if (memberType.GetGenericArguments()[1] == typeof(string))
                            emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyStringvalueDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic));
                        else
                            emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(memberType.GetGenericArguments()[1]));
                    }
                    else
                    {
                        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(memberType.GetGenericArguments()));
                    }

                    emit.LoadLocal(hashtableResultLocal);
                    emit.Nop();

                    return true;
                }

                /*结果集*/
                var dictionaryResultLocal = emit.DeclareLocal(memberType);
                if (memberType.IsValueType)
                {
                    emit.LoadLocalAddress(dictionaryResultLocal);
                    emit.InitializeObject(dictionaryResultLocal.LocalType);
                }
                else
                {
                    emit.NewObject(dictionaryResultLocal.LocalType, Type.EmptyTypes);
                    emit.StoreLocal(dictionaryResultLocal);
                }

                emit.LoadLocal(dictionaryResultLocal);
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadNull();
                emit.LoadConstant(0);

                /*不是<T,K>这种泛型参数*/
                if (memberType.IsGenericType)
                {
                    var genericArguments = memberType.GetGenericArguments();
                    if (genericArguments[0] == typeof(string))
                    {
                        if (memberType.GetGenericArguments()[1] == typeof(string))
                            emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyStringvalueDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic));
                        else
                            emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(genericArguments[1]));
                    }
                    else
                    {
                        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(genericArguments));
                    }
                }
                else
                {
                    var interfaces = this.GetInterfaces(memberType, typeof(IDictionary<,>));
                    for (var i = 0; i < interfaces.Length; i++)
                    {
                        if (!interfaces[i].IsGenericType)
                            continue;

                        var genericArguments = interfaces[i].GetGenericArguments();
                        if (genericArguments[0] == typeof(string))
                        {
                            if (memberType.GetGenericArguments()[1] == typeof(string))
                                emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyStringvalueDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic));
                            else
                                emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(genericArguments[1]));
                        }
                        else
                        {
                            emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(genericArguments));
                        }

                        break;
                    }
                }

                //if (memberType.GetGenericArguments()[0] == typeof(string))
                //{
                //    if (memberType.GetGenericArguments()[1] == typeof(string))
                //        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyStringvalueDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic));
                //    else
                //        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(memberType.GetGenericArguments()[1]));
                //}
                //else
                //{
                //    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(memberType.GetGenericArguments()));
                //}

                emit.LoadLocal(dictionaryResultLocal);
                emit.Nop();

                return true;
            }

            /*dictionary*/
            if (this.IsContainType(memberType, typeof(IDictionary)))
            {
                /*接口类型，用Dictionary对象或Hashtable*/
                if (memberType.IsInterface)
                {
                    /*结果集*/
                    var hashtableResultLocal = emit.DeclareLocal(typeof(Hashtable));
                    emit.NewObject(hashtableResultLocal.LocalType, Type.EmptyTypes);
                    emit.StoreLocal(hashtableResultLocal);
                    emit.LoadLocal(hashtableResultLocal);
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic));
                    emit.LoadLocal(hashtableResultLocal);
                    emit.Nop();

                    return true;
                }

                /*结果集*/
                var dictionaryResultLocal = emit.DeclareLocal(memberType);
                if (memberType.IsValueType)
                {
                    emit.LoadLocalAddress(dictionaryResultLocal);
                    emit.InitializeObject(dictionaryResultLocal.LocalType);
                }
                else
                {
                    emit.NewObject(dictionaryResultLocal.LocalType, Type.EmptyTypes);
                    emit.StoreLocal(dictionaryResultLocal);
                }

                emit.LoadLocal(dictionaryResultLocal);
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadNull();
                emit.LoadConstant(0);
                emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic));
                emit.LoadLocal(dictionaryResultLocal);
                emit.Nop();

                return true;
            }

            return false;
        }

        /// <summary>
        /// 构建字典
        /// </summary>
        /// <param name="emit"></param>
        /// <param name="instanceLocal"></param>
        /// <param name="sourceType"></param>
        /// <param name="member"></param>
        /// <param name="memberType"></param>
        /// <param name="memberAttributes"></param>
        /// <returns></returns>
        protected virtual bool BuildForDictionaryModule(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, ILocal instanceLocal, Type sourceType, MemberInfo member, Type memberType, Attribute[] memberAttributes)
        {
            /*不是集合*/
            if (!this.IsAssignableFrom(memberType, typeof(IEnumerable)))
                return false;

            /*generic*/
            if (this.IsContainType(memberType, typeof(IDictionary<,>)))
            {
                /*接口类型，用Dictionary对象或Hashtable*/
                if (memberType.IsInterface)
                {
                    /*结果集*/
                    var hashtableResultLocal = emit.DeclareLocal(typeof(Dictionary<,>).MakeGenericType(memberType.GetGenericArguments()));
                    emit.NewObject(hashtableResultLocal.LocalType, Type.EmptyTypes);
                    emit.StoreLocal(hashtableResultLocal);

                    emit.LoadLocal(hashtableResultLocal);
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    if (memberType.GetGenericArguments()[0] == typeof(string))
                    {
                        if (memberType.GetGenericArguments()[1] == typeof(string))
                            emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyStringvalueDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic));
                        else
                            emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(memberType.GetGenericArguments()[1]));
                    }
                    else
                    {
                        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(memberType.GetGenericArguments()));
                    }

                    /*结果集*/
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    emit.LoadLocal(hashtableResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                /*结果集*/
                var dictionaryResultLocal = emit.DeclareLocal(memberType);
                if (memberType.IsValueType)
                {
                    emit.LoadLocalAddress(dictionaryResultLocal);
                    emit.InitializeObject(dictionaryResultLocal.LocalType);
                }
                else
                {
                    emit.NewObject(dictionaryResultLocal.LocalType, Type.EmptyTypes);
                    emit.StoreLocal(dictionaryResultLocal);
                }

                emit.LoadLocal(dictionaryResultLocal);
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                emit.LoadConstant(0);
                /*不是<T,K>这种泛型参数*/
                if (memberType.IsGenericType)
                {
                    var genericArguments = memberType.GetGenericArguments();
                    if (genericArguments[0] == typeof(string))
                    {
                        if (genericArguments[1] == typeof(string))
                            emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyStringvalueDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic));
                        else
                            emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(genericArguments[1]));
                    }
                    else
                    {
                        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(genericArguments));
                    }
                }
                else
                {
                    var interfaces = this.GetInterfaces(memberType, typeof(IDictionary<,>));
                    for (var i = 0; i < interfaces.Length; i++)
                    {
                        if (!interfaces[i].IsGenericType)
                            continue;

                        var genericArguments = interfaces[i].GetGenericArguments();
                        if (genericArguments[0] == typeof(string))
                        {
                            if (genericArguments[1] == typeof(string))
                                emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyStringvalueDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic));
                            else
                                emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(genericArguments[1]));
                        }
                        else
                        {
                            emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(genericArguments));
                        }

                        break;
                    }
                }

                //if (memberType.GetGenericArguments()[0] == typeof(string))
                //{
                //    if (memberType.GetGenericArguments()[1] == typeof(string))
                //        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyStringvalueDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic));
                //    else
                //        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoStringKeyGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(memberType.GetGenericArguments()[1]));
                //}
                //else
                //{
                //    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoGenericDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(memberType.GetGenericArguments()));
                //}

                emit.Nop();
                /*结果集*/
                if (sourceType.IsValueType)
                    emit.LoadLocalAddress(instanceLocal);
                else
                    emit.LoadLocal(instanceLocal);

                emit.LoadLocal(dictionaryResultLocal);
                if (member.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)member).GetSetMethod(true));
                else
                    emit.StoreField((FieldInfo)member);

                return true;
            }

            /*dictionary*/
            if (this.IsContainType(memberType, typeof(IDictionary)))
            {
                /*接口类型，用Dictionary对象或Hashtable*/
                if (memberType.IsInterface)
                {
                    /*结果集*/
                    var hashtableResultLocal = emit.DeclareLocal(typeof(Hashtable));
                    emit.NewObject(hashtableResultLocal.LocalType, Type.EmptyTypes);
                    emit.StoreLocal(hashtableResultLocal);

                    emit.LoadLocal(hashtableResultLocal);
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic));
                    emit.Nop();

                    /*结果集*/
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    emit.LoadLocal(hashtableResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                /*结果集*/
                var dictionaryResultLocal = emit.DeclareLocal(memberType);
                if (memberType.IsValueType)
                {
                    emit.LoadLocalAddress(dictionaryResultLocal);
                    emit.InitializeObject(dictionaryResultLocal.LocalType);
                }
                else
                {
                    emit.NewObject(dictionaryResultLocal.LocalType, Type.EmptyTypes);
                    emit.StoreLocal(dictionaryResultLocal);
                }

                emit.LoadLocal(dictionaryResultLocal);
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                emit.LoadConstant(0);
                emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LoadIntoDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic));
                emit.Nop();

                /*结果集*/
                if (sourceType.IsValueType)
                    emit.LoadLocalAddress(instanceLocal);
                else
                    emit.LoadLocal(instanceLocal);

                emit.LoadLocal(dictionaryResultLocal);
                if (member.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)member).GetSetMethod(true));
                else
                    emit.StoreField((FieldInfo)member);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 构建数组，集合
        /// </summary>
        /// <param name="emit"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        protected virtual bool BuildForArrayModule(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, Type memberType)
        {
            /*不是数组*/
            if (!this.IsAssignableFrom(memberType, typeof(IEnumerable)))
                return false;

            /*array ienumerable*/
            var enumerableGenericArgumentType = GetGenericTypeWhitchAssignableFromArray(memberType);
            if (enumerableGenericArgumentType != null)
            {
                /*不做处理*/
                if (enumerableGenericArgumentType == typeof(object))
                {
                    emit.LoadNull();
                    return true;
                }

                var nullableArrayGenericArgumentType = Nullable.GetUnderlyingType(enumerableGenericArgumentType);
                if (nullableArrayGenericArgumentType != null && this.IsEnumType(nullableArrayGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetNullableEnumArrayParseMethod(enumerableGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(enumerableGenericArgumentType), nullableArrayGenericArgumentType));
                    emit.Nop();

                    return true;
                }

                if (nullableArrayGenericArgumentType != null && this.IsPrimitiveType(nullableArrayGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(enumerableGenericArgumentType)));
                    emit.Nop();

                    return true;
                }

                if (this.IsEnumType(enumerableGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetEnumArrayParseMethod(enumerableGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(enumerableGenericArgumentType)));
                    emit.Nop();

                    return true;
                }

                if (this.IsPrimitiveType(enumerableGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(enumerableGenericArgumentType)));
                    emit.Nop();

                    return true;
                }

                /*集合带集合*/
                if (this.IsEnumerableType(enumerableGenericArgumentType))
                {
                    if (this.IsArrayType(enumerableGenericArgumentType))
                    {
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        emit.LoadNull();
                        emit.LoadArgument(2);
                        emit.LoadConstant(1);
                        emit.Add();
                        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { enumerableGenericArgumentType }));
                        emit.Nop();

                        return true;
                    }

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { enumerableGenericArgumentType }));
                    emit.Nop();

                    return true;
                }

                /*复合对象*/
                if (this.IsComplexType(enumerableGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { enumerableGenericArgumentType }));
                    emit.Nop();

                    return true;
                }
            }

            var listGenericArgumentType = GetGenericTypeWhitchAssignableFromList(memberType);
            /*list*/
            if (listGenericArgumentType != null)
            {
                /*不做处理*/
                if (listGenericArgumentType == typeof(object))
                {
                    emit.LoadNull();
                    return true;
                }

                /*是否可空的类型*/
                var nullableListGenericArgumentType = Nullable.GetUnderlyingType(listGenericArgumentType);
                ILocal listResultLocal = null;
                /*接口类型（不指定实例类型）或直接是List实例，直接用list实例*/
                if (memberType.IsInterface || memberType == typeof(List<>).MakeGenericType(listGenericArgumentType))
                    listResultLocal = emit.DeclareLocal(typeof(List<>).MakeGenericType(listGenericArgumentType));
                else
                    listResultLocal = emit.DeclareLocal(memberType);

                var addRangeMethod = listResultLocal.LocalType.GetMethod("AddRange");
                if (listResultLocal.LocalType.IsValueType)
                {
                    emit.LoadLocalAddress(listResultLocal);
                    emit.InitializeObject(listResultLocal.LocalType);
                }
                else
                {
                    emit.NewObject(listResultLocal.LocalType, Type.EmptyTypes);
                    emit.StoreLocal(listResultLocal);
                }

                emit.LoadLocal(listResultLocal);

                if (nullableListGenericArgumentType != null && this.IsEnumType(nullableListGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetNullableEnumArrayParseMethod(listGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(listGenericArgumentType), nullableListGenericArgumentType));
                    emit.Call(addRangeMethod);
                    emit.LoadLocal(listResultLocal);
                    emit.Nop();

                    return true;
                }

                if (nullableListGenericArgumentType != null && this.IsPrimitiveType(nullableListGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(listGenericArgumentType)));
                    emit.Call(addRangeMethod);
                    emit.LoadLocal(listResultLocal);
                    emit.Nop();

                    return true;
                }

                if (this.IsEnumType(listGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetEnumArrayParseMethod(listGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(listGenericArgumentType)));
                    emit.Call(addRangeMethod);
                    emit.LoadLocal(listResultLocal);
                    emit.Nop();

                    return true;
                }

                if (this.IsPrimitiveType(listGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(listGenericArgumentType)));
                    emit.Call(addRangeMethod);
                    emit.LoadLocal(listResultLocal);
                    emit.Nop();

                    return true;
                }

                /*集合带集合*/
                if (this.IsEnumerableType(listGenericArgumentType))
                {
                    if (this.IsArrayType(listGenericArgumentType))
                    {
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        emit.LoadNull();
                        emit.LoadArgument(2);
                        emit.LoadConstant(1);
                        emit.Add();
                        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { listGenericArgumentType }));
                        emit.Call(addRangeMethod);
                        emit.LoadLocal(listResultLocal);
                        emit.Nop();

                        return true;
                    }

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { listGenericArgumentType }));
                    emit.Call(addRangeMethod);
                    emit.LoadLocal(listResultLocal);
                    emit.Nop();

                    return true;
                }

                /*复合对象*/
                if (this.IsComplexType(listGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { listGenericArgumentType }));
                    emit.Call(addRangeMethod);
                    emit.LoadLocal(listResultLocal);
                    emit.Nop();

                    return true;
                }

                return true;
            }

            var iSetGenericArgumentType = GetGenericTypeWhitchAssignableFromHashset(memberType);
            /*list*/
            if (iSetGenericArgumentType != null)
            {
                /*不做处理*/
                if (iSetGenericArgumentType == typeof(object))
                {
                    emit.LoadNull();
                    return true;
                }

                /*是否可空的类型*/
                var nullableISetGenericArgumentType = Nullable.GetUnderlyingType(iSetGenericArgumentType);
                ILocal iSetResultLocal = null;
                /*接口类型（不指定实例类型）或直接是List实例，直接用list实例*/
                if (memberType.IsInterface || memberType == typeof(HashSet<>).MakeGenericType(listGenericArgumentType))
                    iSetResultLocal = emit.DeclareLocal(typeof(HashSet<>).MakeGenericType(listGenericArgumentType));
                else
                    iSetResultLocal = emit.DeclareLocal(memberType);

                var iSetCopyMethod = typeof(DeseralizerBuilderHelper).GetMethod("CopyToICollection", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(iSetGenericArgumentType);
                /*实现了ICollection接口*/
                if (memberType.IsInterface)
                {
                    emit.NewObject(typeof(HashSet<>).MakeGenericType(iSetGenericArgumentType), Type.EmptyTypes);
                    emit.StoreLocal(iSetResultLocal);
                }
                else
                {
                    /*值对象实现的接口*/
                    if (memberType.IsValueType)
                    {
                        emit.LoadLocalAddress(iSetResultLocal);
                        emit.InitializeObject(iSetResultLocal.LocalType);
                    }
                    else
                    {
                        emit.NewObject(iSetResultLocal.LocalType, Type.EmptyTypes);
                        emit.StoreLocal(iSetResultLocal);
                    }
                }

                emit.LoadLocal(iSetResultLocal);

                if (nullableISetGenericArgumentType != null && this.IsEnumType(nullableISetGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetNullableEnumArrayParseMethod(iSetGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(iSetGenericArgumentType), nullableISetGenericArgumentType));
                    emit.Call(iSetCopyMethod);
                    emit.LoadLocal(iSetResultLocal);
                    emit.Nop();

                    return true;
                }

                if (nullableISetGenericArgumentType != null && this.IsPrimitiveType(nullableISetGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(iSetGenericArgumentType)));
                    emit.Call(iSetCopyMethod);
                    emit.LoadLocal(iSetResultLocal);
                    emit.Nop();

                    return true;
                }

                if (this.IsEnumType(iSetGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetEnumArrayParseMethod(iSetGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(iSetGenericArgumentType)));
                    emit.Call(iSetCopyMethod);
                    emit.LoadLocal(iSetResultLocal);
                    emit.Nop();

                    return true;
                }

                if (this.IsPrimitiveType(iSetGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(iSetGenericArgumentType)));
                    emit.Call(iSetCopyMethod);
                    emit.LoadLocal(iSetResultLocal);
                    emit.Nop();

                    return true;
                }

                /*集合带集合*/
                if (this.IsEnumerableType(iSetGenericArgumentType))
                {
                    if (this.IsArrayType(iSetGenericArgumentType))
                    {
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        emit.LoadNull();
                        emit.LoadArgument(2);
                        emit.LoadConstant(1);
                        emit.Add();
                        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { iSetGenericArgumentType }));
                        emit.Call(iSetCopyMethod);
                        emit.LoadLocal(iSetResultLocal);
                        emit.Nop();

                        return true;
                    }

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { iSetGenericArgumentType }));
                    emit.Call(iSetCopyMethod);
                    emit.LoadLocal(iSetResultLocal);
                    emit.Nop();

                    return true;
                }

                /*复合对象*/
                if (this.IsComplexType(iSetGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { iSetGenericArgumentType }));
                    emit.Call(iSetCopyMethod);
                    emit.LoadLocal(iSetResultLocal);
                    emit.Nop();

                    return true;
                }

                return true;
            }

            /*ICollection接口*/
            var iCollectionGenericArgumentType = GetGenericTypeWhitchAssignableFromICollection(memberType);
            if (iCollectionGenericArgumentType != null)
            {
                /*不做处理*/
                if (iCollectionGenericArgumentType == typeof(object))
                {
                    emit.LoadNull();
                    return true;
                }

                /*是否可空的类型*/
                var nullableICollectionGenericArgumentType = Nullable.GetUnderlyingType(iCollectionGenericArgumentType);

                var iCollectiontResultLocal = emit.DeclareLocal(memberType);
                var copyMethod = typeof(DeseralizerBuilderHelper).GetMethod("CopyToICollection", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(iCollectionGenericArgumentType);
                /*实现了ICollection接口*/
                if (memberType.IsInterface)
                {
                    emit.NewObject(typeof(List<>).MakeGenericType(iCollectionGenericArgumentType), Type.EmptyTypes);
                    emit.StoreLocal(iCollectiontResultLocal);
                }
                else
                {
                    /*值对象实现的接口*/
                    if (memberType.IsValueType)
                    {
                        emit.LoadLocalAddress(iCollectiontResultLocal);
                        emit.InitializeObject(iCollectiontResultLocal.LocalType);
                    }
                    else
                    {
                        emit.NewObject(iCollectiontResultLocal.LocalType, Type.EmptyTypes);
                        emit.StoreLocal(iCollectiontResultLocal);
                    }
                }

                emit.LoadLocal(iCollectiontResultLocal);

                if (nullableICollectionGenericArgumentType != null && this.IsEnumType(nullableICollectionGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetNullableEnumArrayParseMethod(iCollectionGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(iCollectionGenericArgumentType), nullableICollectionGenericArgumentType));
                    emit.Call(copyMethod);
                    emit.LoadLocal(iCollectiontResultLocal);
                    emit.Nop();
                    return true;
                }

                if (nullableICollectionGenericArgumentType != null && this.IsPrimitiveType(nullableICollectionGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(iCollectionGenericArgumentType)));
                    emit.Call(copyMethod);
                    emit.LoadLocal(iCollectiontResultLocal);
                    emit.Nop();
                    return true;
                }

                if (this.IsEnumType(iCollectionGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetEnumArrayParseMethod(iCollectionGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(iCollectionGenericArgumentType)));
                    emit.Call(copyMethod);
                    emit.LoadLocal(iCollectiontResultLocal);
                    emit.Nop();
                    return true;
                }

                if (this.IsPrimitiveType(iCollectionGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(iCollectionGenericArgumentType)));
                    emit.Call(copyMethod);
                    emit.LoadLocal(iCollectiontResultLocal);
                    emit.Nop();
                    return true;
                }

                /*集合带集合*/
                if (this.IsEnumerableType(iCollectionGenericArgumentType))
                {
                    if (this.IsArrayType(iCollectionGenericArgumentType))
                    {
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        emit.LoadNull();
                        emit.LoadArgument(2);
                        emit.LoadConstant(1);
                        emit.Add();
                        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { iCollectionGenericArgumentType }));
                        emit.Call(copyMethod);
                        emit.LoadLocal(iCollectiontResultLocal);
                        emit.Nop();
                        return true;
                    }

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Add();
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { iCollectionGenericArgumentType }));
                    emit.Call(copyMethod);
                    emit.LoadLocal(iCollectiontResultLocal);
                    emit.Nop();
                    return true;
                }

                /*复合对象*/
                if (this.IsComplexType(iCollectionGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadNull();
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { iCollectionGenericArgumentType }));
                    emit.Call(copyMethod);
                    emit.LoadLocal(iCollectiontResultLocal);
                    emit.Nop();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 构建数组，集合
        /// </summary>
        /// <param name="emit"></param>
        /// <param name="instanceLocal"></param>
        /// <param name="sourceType"></param>
        /// <param name="member"></param>
        /// <param name="memberType"></param>
        /// <param name="memberAttributes"></param>
        /// <returns></returns>
        protected virtual bool BuildForArrayModule(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, ILocal instanceLocal, Type sourceType, MemberInfo member, Type memberType, Attribute[] memberAttributes)
        {
            /*不是数组*/
            if (!this.IsAssignableFrom(memberType, typeof(IEnumerable)))
                return false;

            /*array ienumerable*/
            var enumerableGenericArgumentType = GetGenericTypeWhitchAssignableFromArray(memberType);
            if (enumerableGenericArgumentType != null)
            {
                /*不做处理*/
                if (enumerableGenericArgumentType == typeof(object))
                {
                    emit.LoadNull();
                    return true;
                }

                var nullableArrayGenericArgumentType = Nullable.GetUnderlyingType(enumerableGenericArgumentType);
                if (nullableArrayGenericArgumentType != null && this.IsEnumType(nullableArrayGenericArgumentType))
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetNullableEnumArrayParseMethod(enumerableGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(enumerableGenericArgumentType), nullableArrayGenericArgumentType));
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                if (nullableArrayGenericArgumentType != null && this.IsPrimitiveType(nullableArrayGenericArgumentType))
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(enumerableGenericArgumentType)));
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                if (this.IsEnumType(enumerableGenericArgumentType))
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetEnumArrayParseMethod(enumerableGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(enumerableGenericArgumentType)));
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                if (this.IsPrimitiveType(enumerableGenericArgumentType))
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(enumerableGenericArgumentType)));
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                /*集合带集合*/
                if (this.IsEnumerableType(enumerableGenericArgumentType))
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    if (this.IsArrayType(enumerableGenericArgumentType))
                    {
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                        emit.LoadArgument(2);
                        emit.LoadConstant(1);
                        emit.Add();
                        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { enumerableGenericArgumentType }));
                        if (member.MemberType == MemberTypes.Property)
                            emit.Call(((PropertyInfo)member).GetSetMethod(true));
                        else
                            emit.StoreField((FieldInfo)member);

                        return true;
                    }

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { enumerableGenericArgumentType }));
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                /*复合对象*/
                if (this.IsComplexType(enumerableGenericArgumentType))
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { enumerableGenericArgumentType }));
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }
            }

            var listGenericArgumentType = GetGenericTypeWhitchAssignableFromList(memberType);
            /*list*/
            if (listGenericArgumentType != null)
            {
                /*不做处理*/
                if (listGenericArgumentType == typeof(object))
                {
                    emit.LoadNull();
                    return true;
                }

                /*是否可空的类型*/
                var nullableListGenericArgumentType = Nullable.GetUnderlyingType(listGenericArgumentType);
                ILocal listResultLocal = null;
                /*接口类型（不指定实例类型）或直接是List实例，直接用list实例*/
                if (memberType.IsInterface || memberType == typeof(List<>).MakeGenericType(listGenericArgumentType))
                    listResultLocal = emit.DeclareLocal(typeof(List<>).MakeGenericType(listGenericArgumentType));
                else
                    listResultLocal = emit.DeclareLocal(memberType);

                var addRangeMethod = listResultLocal.LocalType.GetMethod("AddRange");
                if (listResultLocal.LocalType.IsValueType)
                {
                    emit.LoadLocalAddress(listResultLocal);
                    emit.InitializeObject(listResultLocal.LocalType);
                }
                else
                {
                    emit.NewObject(listResultLocal.LocalType, Type.EmptyTypes);
                    emit.StoreLocal(listResultLocal);
                }

                emit.LoadLocal(listResultLocal);

                if (nullableListGenericArgumentType != null && this.IsEnumType(nullableListGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetNullableEnumArrayParseMethod(listGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(listGenericArgumentType), nullableListGenericArgumentType));
                    emit.Call(addRangeMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);
                    emit.LoadLocal(listResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                if (nullableListGenericArgumentType != null && this.IsPrimitiveType(nullableListGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(listGenericArgumentType)));
                    emit.Call(addRangeMethod);
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);
                    emit.LoadLocal(listResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                if (this.IsEnumType(listGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetEnumArrayParseMethod(listGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(listGenericArgumentType)));
                    emit.Call(addRangeMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);
                    emit.LoadLocal(listResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                if (this.IsPrimitiveType(listGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(listGenericArgumentType)));
                    emit.Call(addRangeMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);
                    emit.LoadLocal(listResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                /*集合带集合*/
                if (this.IsEnumerableType(listGenericArgumentType))
                {
                    if (this.IsArrayType(listGenericArgumentType))
                    {
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                        emit.LoadArgument(2);
                        emit.LoadConstant(1);
                        emit.Add();
                        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { listGenericArgumentType }));
                        emit.Call(addRangeMethod);

                        if (sourceType.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);
                        emit.LoadLocal(listResultLocal);
                        if (member.MemberType == MemberTypes.Property)
                            emit.Call(((PropertyInfo)member).GetSetMethod(true));
                        else
                            emit.StoreField((FieldInfo)member);

                        return true;
                    }

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { listGenericArgumentType }));
                    emit.Call(addRangeMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);
                    emit.LoadLocal(listResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                /*复合对象*/
                if (this.IsComplexType(listGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { listGenericArgumentType }));
                    emit.Call(addRangeMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);
                    emit.LoadLocal(listResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                return true;
            }

            var iSetGenericArgumentType = GetGenericTypeWhitchAssignableFromHashset(memberType);
            /*list*/
            if (iSetGenericArgumentType != null)
            {
                /*不做处理*/
                if (iSetGenericArgumentType == typeof(object))
                {
                    emit.LoadNull();
                    return true;
                }

                /*是否可空的类型*/
                var nullableISetGenericArgumentType = Nullable.GetUnderlyingType(iSetGenericArgumentType);
                ILocal iSetResultLocal = null;
                /*接口类型（不指定实例类型）或直接是List实例，直接用list实例*/
                if (memberType.IsInterface || memberType == typeof(HashSet<>).MakeGenericType(listGenericArgumentType))
                    iSetResultLocal = emit.DeclareLocal(typeof(HashSet<>).MakeGenericType(listGenericArgumentType));
                else
                    iSetResultLocal = emit.DeclareLocal(memberType);

                var iSetCopyMethod = typeof(DeseralizerBuilderHelper).GetMethod("CopyToICollection", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(iSetGenericArgumentType);
                /*实现了ICollection接口*/
                if (memberType.IsInterface)
                {
                    emit.NewObject(typeof(HashSet<>).MakeGenericType(iSetGenericArgumentType), Type.EmptyTypes);
                    emit.StoreLocal(iSetResultLocal);
                }
                else
                {
                    /*值对象实现的接口*/
                    if (memberType.IsValueType)
                    {
                        emit.LoadLocalAddress(iSetResultLocal);
                        emit.InitializeObject(iSetResultLocal.LocalType);
                    }
                    else
                    {
                        emit.NewObject(iSetResultLocal.LocalType, Type.EmptyTypes);
                        emit.StoreLocal(iSetResultLocal);
                    }
                }

                emit.LoadLocal(iSetResultLocal);

                if (nullableISetGenericArgumentType != null && this.IsEnumType(nullableISetGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetNullableEnumArrayParseMethod(iSetGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(iSetGenericArgumentType), nullableISetGenericArgumentType));
                    emit.Call(iSetCopyMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    emit.LoadLocal(iSetResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                if (nullableISetGenericArgumentType != null && this.IsPrimitiveType(nullableISetGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(iSetGenericArgumentType)));
                    emit.Call(iSetCopyMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    emit.LoadLocal(iSetResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                if (this.IsEnumType(iSetGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetEnumArrayParseMethod(iSetGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(iSetGenericArgumentType)));
                    emit.Call(iSetCopyMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    emit.LoadLocal(iSetResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                if (this.IsPrimitiveType(iSetGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(iSetGenericArgumentType)));
                    emit.Call(iSetCopyMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    emit.LoadLocal(iSetResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                /*集合带集合*/
                if (this.IsEnumerableType(iSetGenericArgumentType))
                {
                    if (this.IsArrayType(iSetGenericArgumentType))
                    {
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                        emit.LoadArgument(2);
                        emit.LoadConstant(1);
                        emit.Add();
                        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { iSetGenericArgumentType }));
                        emit.Call(iSetCopyMethod);

                        if (sourceType.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);

                        emit.LoadLocal(iSetResultLocal);
                        if (member.MemberType == MemberTypes.Property)
                            emit.Call(((PropertyInfo)member).GetSetMethod(true));
                        else
                            emit.StoreField((FieldInfo)member);

                        return true;
                    }

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { iSetGenericArgumentType }));
                    emit.Call(iSetCopyMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    emit.LoadLocal(iSetResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                /*复合对象*/
                if (this.IsComplexType(iSetGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { iSetGenericArgumentType }));
                    emit.Call(iSetCopyMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);

                    emit.LoadLocal(iSetResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                return true;
            }

            /*ICollection接口*/
            var iCollectionGenericArgumentType = GetGenericTypeWhitchAssignableFromICollection(memberType);
            if (iCollectionGenericArgumentType != null)
            {
                /*不做处理*/
                if (iCollectionGenericArgumentType == typeof(object))
                {
                    emit.LoadNull();
                    return true;
                }

                /*是否可空的类型*/
                var nullableICollectionGenericArgumentType = Nullable.GetUnderlyingType(iCollectionGenericArgumentType);

                var iCollectiontResultLocal = emit.DeclareLocal(memberType);
                var copyMethod = typeof(DeseralizerBuilderHelper).GetMethod("CopyToICollection", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(iCollectionGenericArgumentType);
                /*实现了ICollection接口*/
                if (memberType.IsInterface)
                {
                    emit.NewObject(typeof(List<>).MakeGenericType(iCollectionGenericArgumentType), Type.EmptyTypes);
                    emit.StoreLocal(iCollectiontResultLocal);
                }
                else
                {
                    /*值对象实现的接口*/
                    if (memberType.IsValueType)
                    {
                        emit.LoadLocalAddress(iCollectiontResultLocal);
                        emit.InitializeObject(iCollectiontResultLocal.LocalType);
                    }
                    else
                    {
                        emit.NewObject(iCollectiontResultLocal.LocalType, Type.EmptyTypes);
                        emit.StoreLocal(iCollectiontResultLocal);
                    }
                }

                emit.LoadLocal(iCollectiontResultLocal);

                if (nullableICollectionGenericArgumentType != null && this.IsEnumType(nullableICollectionGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetNullableEnumArrayParseMethod(iCollectionGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(iCollectionGenericArgumentType), nullableICollectionGenericArgumentType));
                    emit.Call(copyMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);
                    emit.LoadLocal(iCollectiontResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                if (nullableICollectionGenericArgumentType != null && this.IsPrimitiveType(nullableICollectionGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(iCollectionGenericArgumentType)));
                    emit.Call(copyMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);
                    emit.LoadLocal(iCollectiontResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                if (this.IsEnumType(iCollectionGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetEnumArrayParseMethod(iCollectionGenericArgumentType, typeof(IEnumerable<>).MakeGenericType(iCollectionGenericArgumentType)));
                    emit.Call(copyMethod);
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);
                    emit.LoadLocal(iCollectiontResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                if (this.IsPrimitiveType(iCollectionGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(DeseralizerBuilderHelper.GetPrimitiveArrayParseMethod(typeof(IEnumerable<>).MakeGenericType(iCollectionGenericArgumentType)));
                    emit.Call(copyMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);
                    emit.LoadLocal(iCollectiontResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                /*集合带集合*/
                if (this.IsEnumerableType(iCollectionGenericArgumentType))
                {
                    if (this.IsArrayType(iCollectionGenericArgumentType))
                    {
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                        emit.LoadArgument(2);
                        emit.LoadConstant(1);
                        emit.Add();
                        emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { iCollectionGenericArgumentType }));
                        emit.Call(copyMethod);

                        if (sourceType.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);
                        emit.LoadLocal(iCollectiontResultLocal);
                        if (member.MemberType == MemberTypes.Property)
                            emit.Call(((PropertyInfo)member).GetSetMethod(true));
                        else
                            emit.StoreField((FieldInfo)member);

                        return true;
                    }

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { iCollectionGenericArgumentType }));
                    emit.Call(copyMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);
                    emit.LoadLocal(iCollectiontResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }

                /*复合对象*/
                if (this.IsComplexType(iCollectionGenericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                    emit.LoadConstant(0);
                    emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("CallObjectArrayInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new[] { iCollectionGenericArgumentType }));
                    emit.Call(copyMethod);

                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);
                    emit.LoadLocal(iCollectiontResultLocal);
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetSetMethod(true));
                    else
                        emit.StoreField((FieldInfo)member);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 构建异常
        /// </summary>
        /// <param name="emit"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        protected virtual bool BuildForExceptionModule(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, Type memberType)
        {
            if (!this.IsAssignableFrom(memberType, typeof(Exception)))
                return false;

            emit.LoadArgument(0);
            emit.LoadArgument(1);
            emit.LoadNull();
            emit.Call(DeseralizerBuilderHelper.GetExceptionParseMethod(memberType));
            emit.Nop();

            return true;
        }

        /// <summary>
        /// 构建异常
        /// </summary>
        /// <param name="emit"></param>
        /// <param name="instanceLocal"></param>
        /// <param name="sourceType"></param>
        /// <param name="member"></param>
        /// <param name="memberType"></param>
        /// <param name="memberAttributes"></param>
        /// <returns></returns>
        protected virtual bool BuildForExceptionModule(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, ILocal instanceLocal, Type sourceType, MemberInfo member, Type memberType, Attribute[] memberAttributes)
        {
            if (!this.IsAssignableFrom(memberType, typeof(Exception)))
                return false;

            if (sourceType.IsValueType)
                emit.LoadLocalAddress(instanceLocal);
            else
                emit.LoadLocal(instanceLocal);

            emit.LoadArgument(0);
            emit.LoadArgument(1);
            emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
            emit.Call(DeseralizerBuilderHelper.GetExceptionParseMethod(memberType));
            if (member.MemberType == MemberTypes.Property)
                emit.Call(((PropertyInfo)member).GetSetMethod(true));
            else
                emit.StoreField((FieldInfo)member);

            return true;
        }

        /// <summary>
        /// 构建Type
        /// </summary>
        /// <param name="emit"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        protected virtual bool BuildForTypeModule(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, Type memberType)
        {
            if (!this.IsType(memberType))
                return false;

            emit.LoadArgument(0);
            emit.LoadArgument(1);
            emit.LoadNull();
            emit.Call(DeseralizerBuilderHelper.GetParseMethod(memberType));
            emit.Nop();

            return true;
        }

        /// <summary>
        /// 构建Type
        /// </summary>
        /// <param name="emit"></param>
        /// <param name="instanceLocal"></param>
        /// <param name="sourceType"></param>
        /// <param name="member"></param>
        /// <param name="memberType"></param>
        /// <param name="memberAttributes"></param>
        /// <returns></returns>
        protected virtual bool BuildForTypeModule(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, ILocal instanceLocal, Type sourceType, MemberInfo member, Type memberType, Attribute[] memberAttributes)
        {
            if (!this.IsType(memberType))
                return false;

            if (sourceType.IsValueType)
                emit.LoadLocalAddress(instanceLocal);
            else
                emit.LoadLocal(instanceLocal);

            emit.LoadArgument(0);
            emit.LoadArgument(1);
            emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
            emit.Call(DeseralizerBuilderHelper.GetParseMethod(memberType));
            if (member.MemberType == MemberTypes.Property)
                emit.Call(((PropertyInfo)member).GetSetMethod(true));
            else
                emit.StoreField((FieldInfo)member);

            return true;
        }

        /// <summary>
        /// 构建Type
        /// </summary>
        /// <param name="emit"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        protected virtual bool BuildForEnumModule(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, Type memberType)
        {
            if (this.IsEnumType(memberType))
            {
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadNull();
                emit.Call(DeseralizerBuilderHelper.GetEnumParseMethod(memberType));
                emit.Nop();

                return true;
            }

            if (this.IsNullableEnumType(memberType))
            {
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadNull();
                emit.Call(DeseralizerBuilderHelper.GetNullableEnumParseMethod(memberType, Nullable.GetUnderlyingType(memberType)));
                emit.Nop();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 构建枚举
        /// </summary>
        /// <param name="emit"></param>
        /// <param name="instanceLocal"></param>
        /// <param name="sourceType"></param>
        /// <param name="member"></param>
        /// <param name="memberType"></param>
        /// <param name="memberAttributes"></param>
        /// <returns></returns>
        protected virtual bool BuildForEnumModule(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, ILocal instanceLocal, Type sourceType, MemberInfo member, Type memberType, Attribute[] memberAttributes)
        {
            if (this.IsEnumType(memberType))
            {
                if (sourceType.IsValueType)
                    emit.LoadLocalAddress(instanceLocal);
                else
                    emit.LoadLocal(instanceLocal);
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                emit.Call(DeseralizerBuilderHelper.GetEnumParseMethod(memberType));
                if (member.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)member).GetSetMethod(true));
                else
                    emit.StoreField((FieldInfo)member);

                return true;
            }

            if (this.IsNullableEnumType(memberType))
            {
                var nullableMemberType = Nullable.GetUnderlyingType(memberType);
                if (sourceType.IsValueType)
                    emit.LoadLocalAddress(instanceLocal);
                else
                    emit.LoadLocal(instanceLocal);

                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                emit.Call(DeseralizerBuilderHelper.GetNullableEnumParseMethod(memberType, nullableMemberType));
                if (member.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)member).GetSetMethod(true));
                else
                    emit.StoreField((FieldInfo)member);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 构建基元
        /// </summary>
        /// <param name="emit"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        protected virtual bool BuildForPrimitiveModule(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, Type memberType)
        {
            if (this.IsPrimitiveType(memberType))
            {
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadNull();
                emit.Call(DeseralizerBuilderHelper.GetParseMethod(memberType));
                emit.Nop();
                return true;
            }

            if (this.IsNullablePrimitiveType(memberType))
            {
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadNull();
                emit.Call(DeseralizerBuilderHelper.GetParseMethod(memberType));
                emit.Nop();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 构建基元
        /// </summary>
        /// <param name="emit"></param>
        /// <param name="instanceLocal"></param>
        /// <param name="sourceType"></param>
        /// <param name="member"></param>
        /// <param name="memberType"></param>
        /// <param name="memberAttributes"></param>
        /// <returns></returns>
        protected virtual bool BuildForPrimitiveModule(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, ILocal instanceLocal, Type sourceType, MemberInfo member, Type memberType, Attribute[] memberAttributes)
        {
            if (this.IsPrimitiveType(memberType))
            {
                if (sourceType.IsValueType)
                    emit.LoadLocalAddress(instanceLocal);
                else
                    emit.LoadLocal(instanceLocal);

                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                emit.Call(DeseralizerBuilderHelper.GetParseMethod(memberType));
                if (member.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)member).GetSetMethod(true));
                else
                    emit.StoreField((FieldInfo)member);

                return true;
            }

            if (this.IsNullablePrimitiveType(memberType))
            {
                if (sourceType.IsValueType)
                    emit.LoadLocalAddress(instanceLocal);
                else
                    emit.LoadLocal(instanceLocal);

                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
                emit.Call(DeseralizerBuilderHelper.GetParseMethod(memberType));
                if (member.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)member).GetSetMethod(true));
                else
                    emit.StoreField((FieldInfo)member);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 构建string
        /// </summary>
        /// <param name="emit"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        protected virtual void BuildForStringTypeModule(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, Type memberType)
        {
            emit.LoadArgument(0);
            emit.LoadArgument(1);
            emit.LoadNull();
            emit.Call(DeseralizerBuilderHelper.GetParseMethod(typeof(string)));
            emit.Nop();
        }

        /// <summary>
        /// 构建string
        /// </summary>
        /// <param name="emit"></param>
        /// <param name="instanceLocal"></param>
        /// <param name="sourceType"></param>
        /// <param name="member"></param>
        /// <param name="memberType"></param>
        /// <param name="memberAttributes"></param>
        /// <returns></returns>
        protected virtual void BuildForStringTypeModule(EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, T>> emit, ILocal instanceLocal, Type sourceType, MemberInfo member, Type memberType, Attribute[] memberAttributes)
        {
            if (sourceType.IsValueType)
                emit.LoadLocalAddress(instanceLocal);
            else
                emit.LoadLocal(instanceLocal);

            emit.LoadArgument(0);
            emit.LoadArgument(1);
            emit.LoadConstant(this.LoadNotNullMemberName(member, memberAttributes));
            emit.Call(DeseralizerBuilderHelper.GetParseMethod(typeof(string)));
            if (member.MemberType == MemberTypes.Property)
                emit.Call(((PropertyInfo)member).GetSetMethod(true));
            else
                emit.StoreField((FieldInfo)member);
        }

        #endregion build members

        #region load

        /// <summary>
        /// 获取只能以数组分配的集合
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public virtual Type GetGenericTypeWhitchAssignableFromArray(Type sourceType)
        {
            if (sourceType.IsArray)
                return this.GetInterfaces(sourceType, typeof(IList<>))[0].GetGenericArguments()[0];

            var targetType = sourceType;
            if (!targetType.IsGenericType)
            {
                var interfaces = this.GetInterfaces(targetType, typeof(IEnumerable<>));
                for (var i = 0; i < interfaces.Length; i++)
                {
                    if (!interfaces[i].IsGenericType)
                        continue;

                    targetType = interfaces[i];

                    /*不是接口了*/
                    if (!targetType.IsInterface)
                        return null;

                    var notgenericTypes = targetType.GetGenericArguments();
                    if (targetType == typeof(IEnumerable<>).MakeGenericType(notgenericTypes[0]))
                        return notgenericTypes[0];
                }

                return null;
            }

            /*不是接口了*/
            if (!targetType.IsInterface)
                return null;

            var genericTypes = targetType.GetGenericArguments();
            if (targetType == typeof(IEnumerable<>).MakeGenericType(genericTypes[0]))
                return genericTypes[0];

            return null;
        }

        /// <summary>
        /// 获取能以List分配的集合
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public virtual Type GetGenericTypeWhitchAssignableFromList(Type sourceType)
        {
            if (sourceType.IsArray)
                return null;

            var targetType = sourceType;
            if (!targetType.IsGenericType)
            {
                var interfaces = this.GetInterfaces(targetType, typeof(IList<>));
                for (var i = 0; i < interfaces.Length; i++)
                {
                    if (!interfaces[i].IsGenericType)
                        continue;

                    targetType = interfaces[i];
                    /*直接是list对象的*/
                    var notgenericTypes = targetType.GetGenericArguments();
                    if (targetType == typeof(List<>).MakeGenericType(notgenericTypes[0]))
                        return notgenericTypes[0];

                    /*不是接口了*/
                    if (!targetType.IsInterface)
                        return null;

                    if (this.IsContainType(targetType, typeof(IList<>)))
                        return notgenericTypes[0];

                    if (this.IsContainType(targetType, typeof(ICollection<>)))
                        return notgenericTypes[0];

                    if (this.IsContainType(targetType, typeof(IEnumerable<>)))
                        return notgenericTypes[0];
                }

                return null;
            }

            /*直接是list对象的*/
            var genericTypes = targetType.GetGenericArguments();
            if (targetType == typeof(List<>).MakeGenericType(genericTypes[0]))
                return genericTypes[0];

            /*不是接口了*/
            if (!targetType.IsInterface)
                return null;

            if (this.IsContainType(targetType, typeof(IList<>)))
                return genericTypes[0];

            if (this.IsContainType(targetType, typeof(ICollection<>)))
                return genericTypes[0];

            if (this.IsContainType(targetType, typeof(IEnumerable<>)))
                return genericTypes[0];

            return null;
        }

        /// <summary>
        /// 获取能以hasset分配的集合
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public virtual Type GetGenericTypeWhitchAssignableFromHashset(Type sourceType)
        {
            if (sourceType.IsArray)
                return null;

            var targetType = sourceType;
            if (!targetType.IsGenericType)
            {
                var interfaces = this.GetInterfaces(targetType, typeof(HashSet<>));
                for (var i = 0; i < interfaces.Length; i++)
                {
                    if (!interfaces[i].IsGenericType)
                        continue;

                    targetType = interfaces[i];
                    /*直接是list对象的*/
                    var notgenericTypes = targetType.GetGenericArguments();
                    if (targetType == typeof(HashSet<>).MakeGenericType(notgenericTypes[0]))
                        return notgenericTypes[0];

                    /*不是接口了*/
                    if (!targetType.IsInterface)
                        return null;

                    if (this.IsContainType(targetType, typeof(ISet<>)))
                        return notgenericTypes[0];
                }

                return null;
            }

            /*直接是list对象的*/
            var genericTypes = targetType.GetGenericArguments();
            if (targetType == typeof(HashSet<>).MakeGenericType(genericTypes[0]))
                return genericTypes[0];

            /*不是接口了*/
            if (!targetType.IsInterface)
                return null;

            if (this.IsContainType(targetType, typeof(ISet<>)))
                return genericTypes[0];

            return null;
        }

        /// <summary>
        /// 获取能以ICollection分配的集合
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public virtual Type GetGenericTypeWhitchAssignableFromICollection(Type sourceType)
        {
            if (sourceType.IsArray)
                return null;

            var targetType = sourceType;
            if (!targetType.IsGenericType)
            {
                var interfaces = this.GetInterfaces(targetType, typeof(IList<>));
                for (var i = 0; i < interfaces.Length; i++)
                {
                    if (!interfaces[i].IsGenericType)
                        continue;

                    targetType = interfaces[i];
                    /*直接是list对象的*/
                    var notgenericTypes = targetType.GetGenericArguments();
                    return notgenericTypes[0];
                }

                interfaces = this.GetInterfaces(targetType, typeof(ISet<>));
                for (var i = 0; i < interfaces.Length; i++)
                {
                    if (!interfaces[i].IsGenericType)
                        continue;

                    targetType = interfaces[i];
                    /*直接是list对象的*/
                    var notgenericTypes = targetType.GetGenericArguments();
                    return notgenericTypes[0];
                }

                interfaces = this.GetInterfaces(targetType, typeof(ICollection<>));
                for (var i = 0; i < interfaces.Length; i++)
                {
                    if (!interfaces[i].IsGenericType)
                        continue;

                    targetType = interfaces[i];
                    /*直接是list对象的*/
                    var notgenericTypes = targetType.GetGenericArguments();
                    return notgenericTypes[0];
                }

                return null;
            }

            var genericTypes = targetType.GetGenericArguments();
            if (this.IsContainType(targetType, typeof(IList<>)))
                return genericTypes[0];

            if (this.IsContainType(targetType, typeof(ISet<>)))
                return genericTypes[0];

            if (targetType == typeof(ICollection<>).MakeGenericType(genericTypes[0]))
                return genericTypes[0];

            return null;
        }

        /// <summary>
        /// 加载名字
        /// </summary>
        /// <returns></returns>
        private string LoadNotNullMemberName(MemberInfo member, Attribute[] attributes)
        {
            var memberName = LoadMemberName(member, attributes);
            if (string.IsNullOrEmpty(memberName))
                return member.Name ?? string.Empty;

            return memberName ?? string.Empty;
        }

        /// <summary>
        /// 加载名字
        /// </summary>
        /// <returns></returns>
        protected override string LoadMemberName(MemberInfo member, Attribute[] attributes)
        {
            if (attributes == null || attributes.Length == 0)
                return member.Name;

            var jsonMember = ObjectExtension.GetAttribute<DataMemberAttribute>(attributes);
            if (jsonMember != null)
                return jsonMember.Name;

            var dataMember = ObjectExtension.GetAttribute<System.Runtime.Serialization.DataMemberAttribute>(attributes);
            if (dataMember != null)
                return dataMember.Name;

            try
            {
                var jsonNetPropertyType = Type.GetType("Newtonsoft.Json.JsonPropertyAttribute, Newtonsoft.Json");
                if (jsonNetPropertyType != null)
                {
                    foreach (var attribute in attributes)
                    {
                        if (attribute.GetType() == jsonNetPropertyType)
                            return attribute.GetType().GetProperty("PropertyName").GetValue(attribute, null) as string;
                    }
                }
            }
            finally
            {
            }

            return member.Name;
        }

        /// <summary>
        /// 是否包括了忽略的属性
        /// </summary>
        /// <returns></returns>
        protected override bool ContainIgoreAttribute(MemberInfo member, Attribute[] attributes)
        {
            if (attributes == null || attributes.Length == 0)
                return false;

            if (ObjectExtension.GetAttribute<IgnoreDataMemberAttribute>(attributes) != null)
                return true;

            try
            {
                var jsonNetIgnoreType = Type.GetType("Newtonsoft.Json.JsonIgnoreAttribute, Newtonsoft.Json");
                if (jsonNetIgnoreType != null)
                {
                    foreach (var attribute in attributes)
                    {
                        if (attribute.GetType() == jsonNetIgnoreType)
                            return true;
                    }
                }

                var dataMember = ObjectExtension.GetAttribute<System.Runtime.Serialization.IgnoreDataMemberAttribute>(attributes);
                if (dataMember != null)
                    return true;
            }
            finally
            {
            }

            return false;
        }

        /// <summary>
        /// 是否为数组
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsArrayType(Type type)
        {
            if (type.IsArray)
                return true;

            if (!type.IsAssignableFromType(typeof(IEnumerable<>)))
                return false;

            if (type.IsAssignableFromType(typeof(IDictionary<,>)))
                return false;

            return true;
        }

        #endregion load
    }
}