using Never.Reflection;
using Never.Serialization.Json.MethodProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Never.Serialization.Json.Serialize
{
    /// <summary>
    /// emit创建
    /// </summary>
    public abstract class SerialierBuilder<T> : JsonEmitBuilder<T>
    {
        #region field

        /// <summary>
        /// string  写入方法，请不要直接使用该方法，有些字符串是关键字，除非特殊处理，要统一调用该方法<see cref="StringMethodProvider"/>
        /// </summary>
        private static readonly MethodInfo WriteStringMethod = typeof(ISerializerWriter).GetMethod("Write", new[] { typeof(string) });

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialierBuilder{T}"/> class.
        /// </summary>
        protected SerialierBuilder()
            : base(new List<string>(2), StringComparison.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialierBuilder{T}"/> class.
        /// </summary>
        /// <param name="ignoredMembers">The ignored members.</param>
        /// <param name="stringComparer">The string comparer.</param>
        protected SerialierBuilder(List<string> ignoredMembers, StringComparison stringComparer)
            : base(ignoredMembers, stringComparer)
        {
        }

        #endregion ctor

        #region build

        /// <summary>
        /// 构建信息写入流中,当前只处理基元类型
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="setting">配置</param>
        protected virtual void Build(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting)
        {
            if (this.TargetType.IsValueType)
            {
                if (Nullable.GetUnderlyingType(this.TargetType) != null)
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadArgument(2);
                    emit.LoadArgument(3);
                    emit.Call(typeof(SerialierBuilderHelper).GetMethod("CallNullablePrimitiveBuilderInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(Nullable.GetUnderlyingType(this.TargetType)));
                    return;
                }
                else
                {
                    this.WriteObjectFrontSigil(emit, setting);
                    this.BuildMembers(emit, setting, null, this.TargetType, this.GetMembers(this.TargetType));
                    this.WriteObjectLastSigil(emit, setting);
                }

                return;
            }

            ILabel nullLabel = emit.DefineLabel();
            ILabel returnLabel = emit.DefineLabel();

            emit.LoadArgument(2);
            emit.LoadNull();
            emit.CompareGreaterThan();
            /*null*/
            emit.BranchIfFalse(nullLabel);
            this.WriteObjectFrontSigil(emit, setting);
            this.BuildMembers(emit, setting, null, this.TargetType, this.GetMembers(this.TargetType));
            this.WriteObjectLastSigil(emit, setting);
            emit.Branch(returnLabel);
            emit.MarkLabel(nullLabel);
            if (setting.WriteNullWhenObjectIsNull)
                this.WriteNull(emit, setting);
            else
                this.WriteObjectSigil(emit, setting);
            emit.Branch(returnLabel);
            emit.MarkLabel(returnLabel);
            emit.Nop();

            return;
        }

        #endregion build

        #region build members

        /// <summary>
        /// 构建非洋葱类型的基本信息写入流中
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="setting">配置</param>
        /// <param name="cepaSourceType">当前节点的非洋葱类型</param>
        protected virtual void BuildNotCepaType(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, Type cepaSourceType)
        {
            if (this.BuildForJsonObject(emit, setting, cepaSourceType))
                return;

            if (this.BuildForTypeModule(emit, setting, cepaSourceType))
                return;

            if (this.BuildForExceptionModule(emit, setting, cepaSourceType))
                return;

            if (this.BuildForEnumModule(emit, setting, cepaSourceType))
                return;

            if (this.BuildForPrimitiveModule(emit, setting, cepaSourceType))
                return;

            if (this.BuildForArrayModule(emit, setting, cepaSourceType))
                return;

            if (this.BuildForDictionaryModule(emit, setting, cepaSourceType))
                return;

            return;
        }

        /// <summary>
        /// 构建基本信息写入流中
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="setting">配置</param>
        /// <param name="instanceLocal">当前对象</param>
        /// <param name="sourceType">当前节点的成员类型</param>
        /// <param name="members">所有成员</param>
        protected virtual void BuildMembers(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, ILocal instanceLocal, Type sourceType, MemberInfo[] members)
        {
            if (members == null || members.Length == 0)
                return;

            var newMembers = new MemberInfo[members.Length];
            var newAttributes = new List<Attribute[]>(members.Length);

            int index = -1;
            for (var i = 0; i < members.Length; i++)
            {
                if (this.IsContainIgnoredMember(members[i]))
                    continue;

                if (members[i].MemberType == MemberTypes.Property)
                {
                    var prop = (PropertyInfo)members[i];
                    if (!prop.CanRead)
                        continue;
                }

                /*所有属性*/
                var attributes = this.LoadAttributes(members[i], true);
                if (this.ContainIgoreAttribute(members[i], attributes))
                    continue;

                index++;
                newAttributes.Add(attributes);
                newMembers[index] = members[i];
            }

            for (var i = 0; i <= index; i++)
            {
                var member = newMembers[i];

                /*所有属性*/
                var attributes = newAttributes[i];

                Type memberType = null;
                if (member.MemberType == MemberTypes.Property)
                {
                    if (!((PropertyInfo)member).CanRead)
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
                    this.WriteMemberName(emit, setting, member, attributes);
                    this.WriteColon(emit, setting);
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    if (instanceLocal == null)
                    {
                        if (sourceType.IsValueType)
                            emit.LoadArgumentAddress(2);
                        else
                            emit.LoadArgument(2);
                    }
                    else
                    {
                        if (sourceType.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);                             // @object
                    }

                    if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                        emit.Call(((PropertyInfo)member).GetGetMethod(true));
                    else
                        emit.LoadField(((FieldInfo)member));

                    emit.LoadArgument(3);
                    emit.Call(typeof(SerialierBuilderHelper).GetMethod("CallCustomeBuilderInvoke", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(memberType));
                    if (i < index)
                        WriteCommaSeparated(emit, setting);

                    continue;
                }

                if (this.BuildForJsonObjectModule(emit, setting, instanceLocal, sourceType, member, memberType, attributes))
                {
                    if (i < index)
                        WriteCommaSeparated(emit, setting);

                    continue;
                }

                if (this.BuildForEnumModule(emit, setting, instanceLocal, sourceType, member, memberType, attributes))
                {
                    if (i < index)
                        WriteCommaSeparated(emit, setting);

                    continue;
                }

                if (this.BuildForPrimitiveModule(emit, setting, instanceLocal, sourceType, member, memberType, attributes))
                {
                    if (i < index)
                        WriteCommaSeparated(emit, setting);

                    continue;
                }

                if (this.BuildForTypeModule(emit, setting, instanceLocal, sourceType, member, memberType, attributes))
                {
                    if (i < index)
                        WriteCommaSeparated(emit, setting);

                    continue;
                }

                if (this.BuildForExceptionModule(emit, setting, instanceLocal, sourceType, member, memberType, attributes))
                {
                    if (i < index)
                        WriteCommaSeparated(emit, setting);

                    continue;
                }

                /*dictionary*/
                if (this.BuildForDictionaryModule(emit, setting, instanceLocal, sourceType, member, memberType, attributes))
                {
                    if (i < index)
                        WriteCommaSeparated(emit, setting);

                    continue;
                }

                /*array*/
                if (this.BuildForArrayModule(emit, setting, instanceLocal, sourceType, member, memberType, attributes))
                {
                    if (i < index)
                        WriteCommaSeparated(emit, setting);

                    continue;
                }

                /*在运行时查询该对象*/
                if (memberType == typeof(object))
                {
                    this.WriteMemberName(emit, setting, member, attributes);
                    this.WriteColon(emit, setting);
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    if (instanceLocal == null)
                    {
                        if (sourceType.IsValueType)
                            emit.LoadArgumentAddress(2);
                        else
                            emit.LoadArgument(2);
                    }
                    else
                    {
                        if (sourceType.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);                             // @object
                    }

                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetGetMethod(true));
                    else
                        emit.LoadField(((FieldInfo)member));

                    emit.LoadArgument(3);
                    emit.Call(typeof(SerialierBuilderHelper).GetMethod("CallObjectBuilderInvoke", BindingFlags.NonPublic | BindingFlags.Static));
                    if (i < index)
                        WriteCommaSeparated(emit, setting);

                    continue;
                }

                /*complex*/
                if (IsComplexType(member))
                {
                    this.WriteMemberName(emit, setting, member, attributes);
                    this.WriteColon(emit, setting);

                    var recursionLabel = emit.DefineLabel();
                    var nopLabel = emit.DefineLabel();

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadArgument(2);
                    emit.LoadArgument(3);
                    emit.Call(typeof(SerialierBuilderHelper).GetMethod("CallSerializeMaxDepthCompare", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(memberType));
                    //emit.LoadArgument(3);
                    //emit.LoadArgumentAddress(1);
                    //emit.Call(typeof(JsonSerializeSetting).GetProperty("MaxDepth").GetGetMethod(true));
                    //emit.CompareLessThan();
                    emit.BranchIfFalse(recursionLabel);

                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    if (instanceLocal == null)
                    {
                        if (sourceType.IsValueType)
                            emit.LoadArgumentAddress(2);
                        else
                            emit.LoadArgument(2);
                    }
                    else
                    {
                        if (sourceType.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);                             // @object
                    }
                    if (member.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)member).GetGetMethod(true));
                    else
                        emit.LoadField(((FieldInfo)member));

                    emit.LoadArgument(3);
                    emit.Call(typeof(SerialierBuilderHelper).GetMethod("CallBuilderInvoke", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(memberType));
                    emit.Branch(nopLabel);
                    emit.MarkLabel(recursionLabel);
                    if (setting.WriteNullWhenObjectIsNull)
                        this.WriteNull(emit, setting);
                    else
                        this.WriteObjectSigil(emit, setting);

                    emit.Nop();
                    emit.Branch(nopLabel);

                    emit.MarkLabel(nopLabel);
                    emit.Nop();

                    if (i < index)
                        WriteCommaSeparated(emit, setting);
                    continue;
                }
            }

            emit.Nop();
        }

        /// <summary>
        /// 构建数组模块
        /// </summary>
        /// <param name="emit">emit构建</param>
        /// <param name="setting">配置</param>
        /// <param name="memberType">成员类型</param>
        protected virtual bool BuildForArrayModule(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, Type memberType)
        {
            /*不是数组*/
            if (!this.IsAssignableFrom(memberType, typeof(IEnumerable)))
                return false;

            /*过滤字典*/
            if (this.IsAssignableFrom(memberType, typeof(IDictionary<,>)))
                return false;

            /*过滤字典*/
            if (this.IsAssignableFrom(memberType, typeof(IDictionary)))
                return false;

            ILabel returnLbl = null;
            ILabel nullableLbel = null;

            Type genericArgumentType = null;
            if (memberType.IsArray || this.IsContainType(memberType, typeof(IList<>)))
            {
                returnLbl = emit.DefineLabel();
                nullableLbel = emit.DefineLabel();
                emit.LoadArgument(2);
                emit.LoadNull();
                emit.CompareGreaterThan();
                /*null*/
                emit.BranchIfFalse(nullableLbel);
                genericArgumentType = this.GetInterfaces(memberType, typeof(IList<>))[0].GetGenericArguments()[0];
                goto _WriteGenericType;
            }

            if (this.IsContainType(memberType, typeof(IEnumerable<>)))
            {
                returnLbl = emit.DefineLabel();
                nullableLbel = emit.DefineLabel();
                emit.LoadArgument(2);
                emit.LoadNull();
                emit.CompareGreaterThan();
                /*null*/
                emit.BranchIfFalse(nullableLbel);
                genericArgumentType = this.GetInterfaces(memberType, typeof(IEnumerable<>))[0].GetGenericArguments()[0];
                goto _WriteGenericType;
            }

            if (this.IsContainType(memberType, typeof(IList)))
            {
                returnLbl = emit.DefineLabel();
                nullableLbel = emit.DefineLabel();
                emit.LoadArgument(2);
                emit.LoadNull();
                emit.CompareGreaterThan();
                /*null*/
                emit.BranchIfFalse(nullableLbel);
                goto _WriteNoGenericType;
            }

            return false;

        _WriteGenericType:
            {
                if (this.IsEnumType(genericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadArgument(2);
                    emit.LoadArgument(3);
                    emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteEnumArray").MakeGenericMethod(new[] { genericArgumentType }));
                    emit.Branch(returnLbl);
                    emit.MarkLabel(nullableLbel);
                    if (setting.WriteNullWhenObjectIsNull)
                        this.WriteNull(emit, setting);
                    else
                        this.WriteObjectSigil(emit, setting);
                    emit.Branch(returnLbl);
                    emit.MarkLabel(returnLbl);
                    emit.Nop();
                    return true;
                }

                /*primitive*/
                if (this.IsPrimitiveOrInsideHandleType(genericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadArgument(2);
                    emit.Call(SerialierBuilderHelper.GetPrimitiveArrayWriteMethod(typeof(IEnumerable<>).MakeGenericType(new[] { genericArgumentType })));
                    emit.Branch(returnLbl);
                    emit.MarkLabel(nullableLbel);
                    if (setting.WriteNullWhenObjectIsNull)
                        this.WriteNull(emit, setting);
                    else
                        this.WriteObjectSigil(emit, setting);
                    emit.Branch(returnLbl);
                    emit.MarkLabel(returnLbl);
                    emit.Nop();
                    return true;
                }

                var nullableGenericArgumentType = Nullable.GetUnderlyingType(genericArgumentType);
                if (nullableGenericArgumentType != null)
                {
                    /*nullable enum*/
                    if (this.IsEnumType(nullableGenericArgumentType))
                    {
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        emit.LoadArgument(2);
                        emit.LoadArgument(3);
                        emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteNullableEnumArray").MakeGenericMethod(new[] { nullableGenericArgumentType }));
                        emit.Branch(returnLbl);
                        emit.MarkLabel(nullableLbel);
                        if (setting.WriteNullWhenObjectIsNull)
                            this.WriteNull(emit, setting);
                        else
                            this.WriteObjectSigil(emit, setting);
                        emit.Branch(returnLbl);
                        emit.MarkLabel(returnLbl);
                        emit.Nop();
                        return true;
                    }

                    /*primitive*/
                    if (this.IsPrimitiveOrInsideHandleType(nullableGenericArgumentType))
                    {
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        emit.LoadArgument(2);
                        emit.Call(SerialierBuilderHelper.GetPrimitiveArrayWriteMethod(typeof(IEnumerable<>).MakeGenericType(new[] { genericArgumentType })));
                        emit.Branch(returnLbl);
                        emit.MarkLabel(nullableLbel);
                        if (setting.WriteNullWhenObjectIsNull)
                            this.WriteNull(emit, setting);
                        else
                            this.WriteObjectSigil(emit, setting);
                        emit.Branch(returnLbl);
                        emit.MarkLabel(returnLbl);
                        emit.Nop();
                        return true;
                    }
                }

                /* object */
                if (genericArgumentType == typeof(object))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadArgument(2);
                    emit.LoadArgument(3);
                    emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteObjectArray", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<object>), typeof(byte) }));
                    emit.Branch(returnLbl);
                    emit.MarkLabel(nullableLbel);
                    if (setting.WriteNullWhenObjectIsNull)
                        this.WriteNull(emit, setting);
                    else
                        this.WriteObjectSigil(emit, setting);
                    emit.Branch(returnLbl);
                    emit.MarkLabel(returnLbl);
                    emit.Nop();
                    return true;
                }

                if (genericArgumentType.IsArray || this.IsAssignableFrom(genericArgumentType, typeof(IEnumerable<>)))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadArgument(2);
                    emit.LoadArgument(3);
                    emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteObjectArray", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<object>), typeof(byte) }));
                    emit.Branch(returnLbl);
                    emit.MarkLabel(nullableLbel);
                    if (setting.WriteNullWhenObjectIsNull)
                        this.WriteNull(emit, setting);
                    else
                        this.WriteObjectSigil(emit, setting);
                    emit.Branch(returnLbl);
                    emit.MarkLabel(returnLbl);
                    emit.Nop();
                    return true;
                }

                /* complex*/
                if (IsComplexType(genericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadArgument(2);
                    emit.LoadArgument(3);
                    emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteGenericArray").MakeGenericMethod(new[] { genericArgumentType }));
                    emit.Branch(returnLbl);
                    emit.MarkLabel(nullableLbel);
                    if (setting.WriteNullWhenObjectIsNull)
                        this.WriteNull(emit, setting);
                    else
                        this.WriteObjectSigil(emit, setting);
                    emit.Branch(returnLbl);
                    emit.MarkLabel(returnLbl);
                    emit.Nop();
                    return true;
                }

                goto _WriteNoGenericType;
            };

        _WriteNoGenericType:
            {
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadArgument(2);
                emit.LoadArgument(3);
                emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteEnumerableArray", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable), typeof(byte) }));
                emit.Branch(returnLbl);
                emit.MarkLabel(nullableLbel);
                if (setting.WriteNullWhenObjectIsNull)
                    this.WriteNull(emit, setting);
                else
                    this.WriteObjectSigil(emit, setting);
                emit.Branch(returnLbl);
                emit.MarkLabel(returnLbl);
                emit.Nop();
                return true;
            };
        }

        /// <summary>
        /// 构建数组模块
        /// </summary>
        /// <param name="emit">emit构建</param>
        /// <param name="setting">配置</param>
        /// <param name="instanceLocal">当前对象变量</param>
        /// <param name="sourceType">节点成员</param>
        /// <param name="member">成员</param>
        /// <param name="memberType">成员类型</param>
        /// <param name="attributes">特性</param>
        protected virtual bool BuildForArrayModule(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, ILocal instanceLocal, Type sourceType, MemberInfo member, Type memberType, Attribute[] attributes)
        {
            /*不是数组*/
            if (!this.IsAssignableFrom(memberType, typeof(IEnumerable)))
                return false;

            /*过滤字典*/
            if (this.IsAssignableFrom(memberType, typeof(IDictionary<,>)))
                return false;

            /*过滤字典*/
            if (this.IsAssignableFrom(memberType, typeof(IDictionary)))
                return false;

            this.WriteMemberName(emit, setting, member, attributes);
            this.WriteColon(emit, setting);

            ILabel returnLbl = null;
            ILabel nullableLbel = null;

            Type genericArgumentType = null;
            if (memberType.IsArray || this.IsContainType(memberType, typeof(IList<>)))
            {
                returnLbl = emit.DefineLabel();
                nullableLbel = emit.DefineLabel();
                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }

                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod(true));
                else
                    emit.LoadField(((FieldInfo)member));
                emit.LoadNull();
                emit.CompareGreaterThan();
                /*null*/
                emit.BranchIfFalse(nullableLbel);

                genericArgumentType = this.GetInterfaces(memberType, typeof(IList<>))[0].GetGenericArguments()[0];
                goto _WriteGenericType;
            }

            if (this.IsContainType(memberType, typeof(IEnumerable<>)))
            {
                returnLbl = emit.DefineLabel();
                nullableLbel = emit.DefineLabel();
                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }

                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod(true));
                else
                    emit.LoadField(((FieldInfo)member));
                emit.LoadNull();
                emit.CompareGreaterThan();
                /*null*/
                emit.BranchIfFalse(nullableLbel);
                genericArgumentType = this.GetInterfaces(memberType, typeof(IEnumerable<>))[0].GetGenericArguments()[0];
                goto _WriteGenericType;
            }

            if (this.IsContainType(memberType, typeof(IList)))
            {
                returnLbl = emit.DefineLabel();
                nullableLbel = emit.DefineLabel();
                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }

                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod(true));
                else
                    emit.LoadField(((FieldInfo)member));
                emit.LoadNull();
                emit.CompareGreaterThan();
                /*null*/
                emit.BranchIfFalse(nullableLbel);

                goto _WriteNoGenericType;
            }

            returnLbl = emit.DefineLabel();
            nullableLbel = emit.DefineLabel();
            if (instanceLocal == null)
            {
                if (sourceType.IsValueType)
                    emit.LoadArgumentAddress(2);
                else
                    emit.LoadArgument(2);
            }
            else
            {
                if (sourceType.IsValueType)
                    emit.LoadLocalAddress(instanceLocal);
                else
                    emit.LoadLocal(instanceLocal);                             // @object
            }

            if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                emit.Call(((PropertyInfo)member).GetGetMethod(true));
            else
                emit.LoadField(((FieldInfo)member));

            emit.LoadNull();
            emit.CompareGreaterThan();
            /*null*/
            emit.BranchIfFalse(nullableLbel);

            goto _WriteNoGenericType;

        _WriteGenericType:
            {
                if (this.IsEnumType(genericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    if (instanceLocal == null)
                    {
                        if (sourceType.IsValueType)
                            emit.LoadArgumentAddress(2);
                        else
                            emit.LoadArgument(2);
                    }
                    else
                    {
                        if (sourceType.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);                             // @object
                    }

                    if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                        emit.Call(((PropertyInfo)member).GetGetMethod(true));
                    else
                        emit.LoadField(((FieldInfo)member));

                    emit.LoadArgument(3);
                    emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteEnumArray").MakeGenericMethod(new[] { genericArgumentType }));
                    emit.Branch(returnLbl);

                    emit.MarkLabel(nullableLbel);
                    if (setting.WriteNullWhenObjectIsNull)
                        this.WriteNull(emit, setting);
                    else
                        this.WriteObjectSigil(emit, setting);
                    emit.Branch(returnLbl);
                    emit.MarkLabel(returnLbl);
                    emit.Nop();
                    return true;
                }

                /*primitive*/
                if (this.IsPrimitiveOrInsideHandleType(genericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    if (instanceLocal == null)
                    {
                        if (sourceType.IsValueType)
                            emit.LoadArgumentAddress(2);
                        else
                            emit.LoadArgument(2);
                    }
                    else
                    {
                        if (sourceType.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);                             // @object
                    }

                    if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                        emit.Call(((PropertyInfo)member).GetGetMethod(true));
                    else
                        emit.LoadField(((FieldInfo)member));

                    emit.Call(SerialierBuilderHelper.GetPrimitiveArrayWriteMethod(typeof(IEnumerable<>).MakeGenericType(new[] { genericArgumentType })));
                    emit.Branch(returnLbl);

                    emit.MarkLabel(nullableLbel);
                    if (setting.WriteNullWhenObjectIsNull)
                        this.WriteNull(emit, setting);
                    else
                        this.WriteObjectSigil(emit, setting);
                    emit.Branch(returnLbl);
                    emit.MarkLabel(returnLbl);
                    emit.Nop();
                    return true;
                }

                var nullableGenericArgumentType = Nullable.GetUnderlyingType(genericArgumentType);
                if (nullableGenericArgumentType != null)
                {
                    /*nullable enum*/
                    if (this.IsEnumType(nullableGenericArgumentType))
                    {
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        if (instanceLocal == null)
                        {
                            if (sourceType.IsValueType)
                                emit.LoadArgumentAddress(2);
                            else
                                emit.LoadArgument(2);
                        }
                        else
                        {
                            if (sourceType.IsValueType)
                                emit.LoadLocalAddress(instanceLocal);
                            else
                                emit.LoadLocal(instanceLocal);                             // @object
                        }
                        if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                            emit.Call(((PropertyInfo)member).GetGetMethod(true));
                        else
                            emit.LoadField(((FieldInfo)member));

                        emit.LoadArgument(3);
                        emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteNullableEnumArray").MakeGenericMethod(new[] { nullableGenericArgumentType }));
                        emit.Branch(returnLbl);

                        emit.MarkLabel(nullableLbel);
                        if (setting.WriteNullWhenObjectIsNull)
                            this.WriteNull(emit, setting);
                        else
                            this.WriteObjectSigil(emit, setting);
                        emit.Branch(returnLbl);
                        emit.MarkLabel(returnLbl);
                        emit.Nop();
                        return true;
                    }

                    /*primitive*/
                    if (this.IsPrimitiveOrInsideHandleType(nullableGenericArgumentType))
                    {
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        if (instanceLocal == null)
                        {
                            if (sourceType.IsValueType)
                                emit.LoadArgumentAddress(2);
                            else
                                emit.LoadArgument(2);
                        }
                        else
                        {
                            if (sourceType.IsValueType)
                                emit.LoadLocalAddress(instanceLocal);
                            else
                                emit.LoadLocal(instanceLocal);                             // @object
                        }

                        if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                            emit.Call(((PropertyInfo)member).GetGetMethod(true));
                        else
                            emit.LoadField(((FieldInfo)member));

                        emit.Call(SerialierBuilderHelper.GetPrimitiveArrayWriteMethod(typeof(IEnumerable<>).MakeGenericType(new[] { genericArgumentType })));
                        emit.Branch(returnLbl);

                        emit.MarkLabel(nullableLbel);
                        if (setting.WriteNullWhenObjectIsNull)
                            this.WriteNull(emit, setting);
                        else
                            this.WriteObjectSigil(emit, setting);
                        emit.Branch(returnLbl);
                        emit.MarkLabel(returnLbl);
                        emit.Nop();
                        return true;
                    }
                }

                /* object */
                if (genericArgumentType == typeof(object))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    if (instanceLocal == null)
                    {
                        if (sourceType.IsValueType)
                            emit.LoadArgumentAddress(2);
                        else
                            emit.LoadArgument(2);
                    }
                    else
                    {
                        if (sourceType.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);                             // @object
                    }
                    if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                        emit.Call(((PropertyInfo)member).GetGetMethod(true));
                    else
                        emit.LoadField(((FieldInfo)member));

                    emit.LoadArgument(3);
                    emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteObjectArray", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<object>), typeof(byte) }));
                    emit.Branch(returnLbl);

                    emit.MarkLabel(nullableLbel);
                    if (setting.WriteNullWhenObjectIsNull)
                        this.WriteNull(emit, setting);
                    else
                        this.WriteObjectSigil(emit, setting);
                    emit.Branch(returnLbl);
                    emit.MarkLabel(returnLbl);
                    emit.Nop();

                    return true;
                }

                if (genericArgumentType.IsArray || this.IsAssignableFrom(genericArgumentType, typeof(IEnumerable<>)))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    if (instanceLocal == null)
                    {
                        if (sourceType.IsValueType)
                            emit.LoadArgumentAddress(2);
                        else
                            emit.LoadArgument(2);
                    }
                    else
                    {
                        if (sourceType.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);                             // @object
                    }
                    if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                        emit.Call(((PropertyInfo)member).GetGetMethod(true));
                    else
                        emit.LoadField(((FieldInfo)member));

                    emit.LoadArgument(3);
                    emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteObjectArray", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable<object>), typeof(byte) }));
                    emit.Branch(returnLbl);

                    emit.MarkLabel(nullableLbel);
                    if (setting.WriteNullWhenObjectIsNull)
                        this.WriteNull(emit, setting);
                    else
                        this.WriteObjectSigil(emit, setting);
                    emit.Branch(returnLbl);
                    emit.MarkLabel(returnLbl);
                    emit.Nop();
                    return true;
                }

                /* complex*/
                if (IsComplexType(genericArgumentType))
                {
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    if (instanceLocal == null)
                    {
                        if (sourceType.IsValueType)
                            emit.LoadArgumentAddress(2);
                        else
                            emit.LoadArgument(2);
                    }
                    else
                    {
                        if (sourceType.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);                             // @object
                    }
                    if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                        emit.Call(((PropertyInfo)member).GetGetMethod(true));
                    else
                        emit.LoadField(((FieldInfo)member));

                    emit.LoadArgument(3);
                    emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteGenericArray").MakeGenericMethod(new[] { genericArgumentType }));
                    emit.Branch(returnLbl);

                    emit.MarkLabel(nullableLbel);
                    if (setting.WriteNullWhenObjectIsNull)
                        this.WriteNull(emit, setting);
                    else
                        this.WriteObjectSigil(emit, setting);
                    emit.Branch(returnLbl);
                    emit.MarkLabel(returnLbl);
                    emit.Nop();

                    return true;
                }

                return true;
            };

        _WriteNoGenericType:
            {
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }
                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod(true));
                else
                    emit.LoadField(((FieldInfo)member));

                emit.LoadArgument(3);
                emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteEnumerableArray", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable), typeof(byte) }));
                emit.Branch(returnLbl);

                emit.MarkLabel(nullableLbel);
                if (setting.WriteNullWhenObjectIsNull)
                    this.WriteNull(emit, setting);
                else
                    this.WriteObjectSigil(emit, setting);
                emit.Branch(returnLbl);
                emit.MarkLabel(returnLbl);
                emit.Nop();

                return true;
            };
        }

        /// <summary>
        /// 构建JsonObject模块
        /// </summary>
        /// <param name="emit">emit构建</param>
        /// <param name="setting">配置</param>
        /// <param name="sourceType">成员类型</param>
        protected virtual bool BuildForJsonObject(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, Type sourceType)
        {
            if (!TypeHelper.IsAssignableFrom(sourceType, typeof(JsonObject)))
                return false;

            var returnLbl = emit.DefineLabel();
            var nullableLbel = emit.DefineLabel();
            emit.LoadArgument(2);
            emit.LoadNull();
            emit.CompareGreaterThan();
            /*null*/
            emit.BranchIfFalse(nullableLbel);

            emit.LoadArgument(0);
            emit.LoadArgument(1);
            emit.LoadArgument(2);
            emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteJsonObject", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable), typeof(byte) }));
            emit.Branch(returnLbl);
            emit.MarkLabel(nullableLbel);
            if (setting.WriteNullWhenObjectIsNull)
                this.WriteNull(emit, setting);
            else
                this.WriteObjectSigil(emit, setting);
            emit.Branch(returnLbl);
            emit.MarkLabel(returnLbl);
            emit.Nop();
            return true;
        }

        /// <summary>
        /// 构建JsonObject模块
        /// </summary>
        /// <param name="emit">emit构建</param>
        /// <param name="setting">配置</param>
        /// <param name="memberType">成员类型</param>
        protected virtual bool BuildForJsonObjectModule(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, ILocal instanceLocal, Type sourceType, MemberInfo member, Type memberType, Attribute[] attributes)
        {
            if (memberType != typeof(JsonObject))
                return false;

            this.WriteMemberName(emit, setting, member, attributes);
            this.WriteColon(emit, setting);
            ILabel returnLbl = null;
            ILabel nullableLbel = null;
            if (instanceLocal == null)
            {
                if (sourceType.IsValueType)
                    emit.LoadArgumentAddress(2);
                else
                    emit.LoadArgument(2);
            }
            else
            {
                if (sourceType.IsValueType)
                    emit.LoadLocalAddress(instanceLocal);
                else
                    emit.LoadLocal(instanceLocal);                             // @object
            }

            if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                emit.Call(((PropertyInfo)member).GetGetMethod(true));
            else
                emit.LoadField(((FieldInfo)member));

            emit.LoadNull();
            emit.CompareGreaterThan();
            /*null*/
            emit.BranchIfFalse(nullableLbel);
            emit.LoadArgument(0);
            emit.LoadArgument(1);
            if (instanceLocal == null)
            {
                if (sourceType.IsValueType)
                    emit.LoadArgumentAddress(2);
                else
                    emit.LoadArgument(2);
            }
            else
            {
                if (sourceType.IsValueType)
                    emit.LoadLocalAddress(instanceLocal);
                else
                    emit.LoadLocal(instanceLocal);                             // @object
            }
            if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                emit.Call(((PropertyInfo)member).GetGetMethod(true));
            else
                emit.LoadField(((FieldInfo)member));

            emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteJsonObject", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IEnumerable) }));
            emit.Branch(returnLbl);

            emit.MarkLabel(nullableLbel);
            if (setting.WriteNullWhenObjectIsNull)
                this.WriteNull(emit, setting);
            else
                this.WriteObjectSigil(emit, setting);
            emit.Branch(returnLbl);
            emit.MarkLabel(returnLbl);
            emit.Nop();

            return true;
        }

        /// <summary>
        /// 构建字典模块
        /// </summary>
        /// <param name="emit">emit构建</param>
        /// <param name="setting">配置</param>
        /// <param name="instanceLocal">当前对象变量</param>
        /// <param name="sourceType">节点成员</param>
        /// <param name="member">成员</param>
        /// <param name="memberType">成员类型</param>
        /// <param name="attributes">特性</param>
        protected virtual bool BuildForDictionaryModule(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, ILocal instanceLocal, Type sourceType, MemberInfo member, Type memberType, Attribute[] attributes)
        {
            /*不是数组*/
            if (!this.IsAssignableFrom(memberType, typeof(IEnumerable)))
                return false;

            /*字典*/
            if (this.IsContainType(memberType, typeof(IDictionary<,>)))
            {
                /*非泛型，要获取*/
                if (!memberType.IsGenericTypeDefinition)
                {
                    foreach (var @interface in this.GetInterfaces(memberType, typeof(IDictionary<,>)))
                    {
                        if (@interface == memberType)
                            continue;

                        if (!setting.SupportComplexTypeKeyInDictionary)
                        {
                            if (this.IsComplexType(@interface.GetGenericArguments()[0]))
                                throw new ArgumentException("字典的key不能为数组或复合对象，因为其他工具无法识别该Json字符串");

                            if (this.IsContainType(@interface.GetGenericArguments()[0], typeof(IEnumerable)))
                            {
                                if (@interface.GetGenericArguments()[0] != typeof(string))
                                    throw new ArgumentException("字典的key不能为数组或复合对象，因为其他工具无法识别该Json字符串");
                            }

                            break;
                        }
                    }
                }
                else
                {
                    if (!setting.SupportComplexTypeKeyInDictionary)
                    {
                        if (this.IsComplexType(memberType.GetGenericArguments()[0]))
                            throw new ArgumentException("字典的key不能为数组或复合对象，因为其他工具无法识别该Json字符串");

                        if (this.IsContainType(memberType.GetGenericArguments()[0], typeof(IEnumerable)))
                        {
                            if (memberType.GetGenericArguments()[0] != typeof(string))
                                throw new ArgumentException("字典的key不能为数组或复合对象，因为其他工具无法识别该Json字符串");
                        }
                    }
                }

                var genericArgumentTypes = this.GetInterfaces(memberType, typeof(IDictionary<,>))[0].GetGenericArguments();
                this.WriteMemberName(emit, setting, member, attributes);
                this.WriteColon(emit, setting);

                var returnLbl = emit.DefineLabel();
                var nullableLbel = emit.DefineLabel();
                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }
                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod(true));
                else
                    emit.LoadField(((FieldInfo)member));

                emit.LoadNull();
                emit.CompareGreaterThan();
                /*null*/
                emit.BranchIfFalse(nullableLbel);

                emit.LoadArgument(0);
                emit.LoadArgument(1);
                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }
                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod(true));
                else
                    emit.LoadField(((FieldInfo)member));

                emit.LoadArgument(3);

                if (genericArgumentTypes[1] == typeof(object))
                {
                    emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteGenericKeyObjectValue").MakeGenericMethod(new[] { genericArgumentTypes[0] }));
                }
                else
                {
                    emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteGenericKeyValue").MakeGenericMethod(new[] { genericArgumentTypes[0], genericArgumentTypes[1] }));
                }

                emit.Branch(returnLbl);

                emit.MarkLabel(nullableLbel);
                if (setting.WriteNullWhenObjectIsNull)
                    this.WriteNull(emit, setting);
                else
                    this.WriteObjectSigil(emit, setting);

                emit.Branch(returnLbl);
                emit.MarkLabel(returnLbl);
                emit.Nop();
                return true;
            }

            /*非字典*/
            if (this.IsContainType(memberType, typeof(IDictionary)))
            {
                this.WriteMemberName(emit, setting, member, attributes);
                this.WriteColon(emit, setting);

                var returnLbl = emit.DefineLabel();
                var nullableLbel = emit.DefineLabel();
                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }
                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod(true));
                else
                    emit.LoadField(((FieldInfo)member));

                emit.LoadNull();
                emit.CompareGreaterThan();
                /*null*/
                emit.BranchIfFalse(nullableLbel);

                emit.LoadArgument(0);
                emit.LoadArgument(1);
                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }
                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod(true));
                else
                    emit.LoadField(((FieldInfo)member));

                emit.LoadArgument(3);
                emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteKeyValue", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IDictionary), typeof(byte) }));
                emit.Branch(returnLbl);

                emit.MarkLabel(nullableLbel);
                if (setting.WriteNullWhenObjectIsNull)
                    this.WriteNull(emit, setting);
                else
                    this.WriteObjectSigil(emit, setting);

                emit.Branch(returnLbl);
                emit.MarkLabel(returnLbl);
                emit.Nop();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 构建字典模块
        /// </summary>
        /// <param name="emit">emit构建</param>
        /// <param name="setting">配置</param>
        /// <param name="memberType">成员类型</param>
        protected virtual bool BuildForDictionaryModule(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, Type memberType)
        {
            /*不是数组*/
            if (!this.IsAssignableFrom(memberType, typeof(IEnumerable)))
                return false;

            /*字典*/
            if (this.IsContainType(memberType, typeof(IDictionary<,>)))
            {
                /*非泛型，要获取*/
                if (!memberType.IsGenericTypeDefinition)
                {
                    foreach (var @interface in this.GetInterfaces(memberType, typeof(IDictionary<,>)))
                    {
                        if (@interface == memberType)
                            continue;

                        if (!setting.SupportComplexTypeKeyInDictionary)
                        {
                            if (this.IsComplexType(@interface.GetGenericArguments()[0]))
                                throw new ArgumentException("字典的key不能为数组或复合对象，因为其他工具无法识别该Json字符串");

                            if (this.IsContainType(@interface.GetGenericArguments()[0], typeof(IEnumerable)))
                            {
                                if (@interface.GetGenericArguments()[0] != typeof(string))
                                    throw new ArgumentException("字典的key不能为数组或复合对象，因为其他工具无法识别该Json字符串");
                            }

                            break;
                        }
                    }
                }
                else
                {
                    if (!setting.SupportComplexTypeKeyInDictionary)
                    {
                        if (this.IsComplexType(memberType.GetGenericArguments()[0]))
                            throw new ArgumentException("字典的key不能为数组或复合对象，因为其他工具无法识别该Json字符串");

                        if (this.IsContainType(memberType.GetGenericArguments()[0], typeof(IEnumerable)))
                        {
                            if (memberType.GetGenericArguments()[0] != typeof(string))
                                throw new ArgumentException("字典的key不能为数组或复合对象，因为其他工具无法识别该Json字符串");
                        }
                    }
                }

                var returnLbl = emit.DefineLabel();
                var nullableLbel = emit.DefineLabel();
                emit.LoadArgument(2);
                emit.LoadNull();
                emit.CompareGreaterThan();
                /*null*/
                emit.BranchIfFalse(nullableLbel);
                var genericArgumentTypes = this.GetInterfaces(memberType, typeof(IDictionary<,>))[0].GetGenericArguments();

                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadArgument(2);
                emit.LoadArgument(3);

                if (genericArgumentTypes[1] == typeof(object))
                {
                    emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteGenericKeyObjectValue").MakeGenericMethod(new[] { genericArgumentTypes[0] }));
                }
                else
                {
                    emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteGenericKeyValue").MakeGenericMethod(new[] { genericArgumentTypes[0], genericArgumentTypes[1] }));
                }

                emit.Branch(returnLbl);

                emit.MarkLabel(nullableLbel);
                if (setting.WriteNullWhenObjectIsNull)
                    this.WriteNull(emit, setting);
                else
                    this.WriteObjectSigil(emit, setting);
                emit.Branch(returnLbl);
                emit.MarkLabel(returnLbl);
                emit.Nop();
                return true;
            }

            /*非字典*/
            if (this.IsContainType(memberType, typeof(IDictionary)))
            {
                var returnLbl = emit.DefineLabel();
                var nullableLbel = emit.DefineLabel();
                emit.LoadArgument(2);
                emit.LoadNull();
                emit.CompareGreaterThan();
                /*null*/
                emit.BranchIfFalse(nullableLbel);

                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadArgument(2);
                emit.LoadArgument(3);
                emit.Call(typeof(ParseMethodProviderEngrafting).GetMethod("WriteKeyValue", new[] { typeof(ISerializerWriter), typeof(JsonSerializeSetting), typeof(IDictionary), typeof(byte) }));
                emit.Branch(returnLbl);

                emit.MarkLabel(nullableLbel);
                if (setting.WriteNullWhenObjectIsNull)
                    this.WriteNull(emit, setting);
                else
                    this.WriteObjectSigil(emit, setting);
                emit.Branch(returnLbl);
                emit.MarkLabel(returnLbl);
                emit.Nop();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 构建异常模块
        /// </summary>
        /// <param name="emit">emit构建</param>
        /// <param name="setting">配置</param>
        /// <param name="sourceType">成员类型</param>
        protected virtual bool BuildForExceptionModule(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, Type sourceType)
        {
            if (!TypeHelper.IsAssignableFrom(sourceType, typeof(Exception)))
                return false;

            var returnLbl = emit.DefineLabel();
            var nullableLbel = emit.DefineLabel();
            emit.LoadArgument(2);
            emit.LoadNull();
            emit.CompareGreaterThan();
            /*null*/
            emit.BranchIfFalse(nullableLbel);

            emit.LoadArgument(0);
            emit.LoadArgument(1);
            emit.LoadArgument(2);
            emit.Call(SerialierBuilderHelper.GetExceptionParseMethod(sourceType));
            emit.Branch(returnLbl);
            emit.MarkLabel(nullableLbel);
            if (setting.WriteNullWhenObjectIsNull)
                this.WriteNull(emit, setting);
            else
                this.WriteObjectSigil(emit, setting);
            emit.Branch(returnLbl);
            emit.MarkLabel(returnLbl);
            emit.Nop();
            return true;
        }

        /// <summary>
        /// 构建异常模块
        /// </summary>
        /// <param name="emit">emit构建</param>
        /// <param name="setting">配置</param>
        /// <param name="instanceLocal">当前对象变量</param>
        /// <param name="sourceType">节点成员</param>
        /// <param name="member">成员</param>
        /// <param name="memberType">成员类型</param>
        /// <param name="attributes">特性</param>
        protected virtual bool BuildForExceptionModule(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, ILocal instanceLocal, Type sourceType, MemberInfo member, Type memberType, Attribute[] attributes)
        {
            if (!TypeHelper.IsAssignableFrom(memberType, typeof(Exception)))
                return false;

            this.WriteMemberName(emit, setting, member, attributes);
            this.WriteColon(emit, setting);

            var returnLbl = emit.DefineLabel();
            var nullableLbel = emit.DefineLabel();

            if (instanceLocal == null)
            {
                if (sourceType.IsValueType)
                    emit.LoadArgumentAddress(2);
                else
                    emit.LoadArgument(2);
            }
            else
            {
                if (sourceType.IsValueType)
                    emit.LoadLocalAddress(instanceLocal);
                else
                    emit.LoadLocal(instanceLocal);                             // @object
            }
            if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                emit.Call(((PropertyInfo)member).GetGetMethod(true));
            else
                emit.LoadField(((FieldInfo)member));
            emit.LoadNull();
            emit.CompareGreaterThan();
            /*null*/
            emit.BranchIfFalse(nullableLbel);

            emit.LoadArgument(0);
            emit.LoadArgument(1);
            if (instanceLocal == null)
            {
                if (sourceType.IsValueType)
                    emit.LoadArgumentAddress(2);
                else
                    emit.LoadArgument(2);
            }
            else
            {
                if (sourceType.IsValueType)
                    emit.LoadLocalAddress(instanceLocal);
                else
                    emit.LoadLocal(instanceLocal);                             // @object
            }
            if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                emit.Call(((PropertyInfo)member).GetGetMethod(true));
            else
                emit.LoadField(((FieldInfo)member));

            emit.Call(SerialierBuilderHelper.GetExceptionParseMethod(memberType));
            emit.Branch(returnLbl);

            emit.MarkLabel(nullableLbel);
            if (setting.WriteNullWhenObjectIsNull)
                this.WriteNull(emit, setting);
            else
                this.WriteObjectSigil(emit, setting);
            emit.Branch(returnLbl);

            emit.MarkLabel(returnLbl);
            emit.Nop();
            return true;
        }

        /// <summary>
        /// 构建Type
        /// </summary>
        /// <param name="emit">emit构建</param>
        /// <param name="setting">配置</param>
        /// <param name="memberType">成员类型</param>
        /// <returns></returns>
        protected virtual bool BuildForTypeModule(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, Type memberType)
        {
            if (!this.IsType(memberType))
                return false;

            var returnLbl = emit.DefineLabel();
            var nullableLbel = emit.DefineLabel();

            emit.LoadArgument(2);
            emit.LoadNull();
            emit.CompareGreaterThan();
            /*null*/
            emit.BranchIfFalse(nullableLbel);

            this.WriteQuotes(emit, setting);
            emit.LoadArgument(0);                                      // writer
            emit.LoadArgument(1);
            emit.LoadArgument(2);
            emit.Call(SerialierBuilderHelper.GetWriteMethod(memberType));
            this.WriteQuotes(emit, setting);
            emit.Branch(returnLbl);

            emit.MarkLabel(nullableLbel);
            if (setting.WriteNullWhenObjectIsNull)
                this.WriteNull(emit, setting);
            else
                this.WriteObjectSigil(emit, setting);
            emit.Branch(returnLbl);
            emit.MarkLabel(returnLbl);
            emit.Nop();

            return true;
        }

        /// <summary>
        /// 构建Type
        /// </summary>
        /// <param name="emit">emit构建</param>
        /// <param name="setting">配置</param>
        /// <param name="instanceLocal">当前对象变量</param>
        /// <param name="sourceType">节点成员</param>
        /// <param name="member">成员</param>
        /// <param name="memberType">成员类型</param>
        /// <param name="attributes">特性</param>
        protected virtual bool BuildForTypeModule(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, ILocal instanceLocal, Type sourceType, MemberInfo member, Type memberType, Attribute[] attributes)
        {
            /*type类型*/
            if (this.IsType(memberType))
            {
                this.WriteMemberName(emit, setting, member, attributes);
                this.WriteColon(emit, setting);

                var returnLbl = emit.DefineLabel();
                var nullableLbel = emit.DefineLabel();

                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }
                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod(true));
                else
                    emit.LoadField(((FieldInfo)member));
                emit.LoadNull();
                emit.CompareGreaterThan();
                /*null*/
                emit.BranchIfFalse(nullableLbel);

                this.WriteQuotes(emit, setting);
                emit.LoadArgument(0);                                      // writer
                emit.LoadArgument(1);                                      // setting
                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }

                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod(true));
                else
                    emit.LoadField(((FieldInfo)member));

                emit.Call(SerialierBuilderHelper.GetWriteMethod(memberType));
                this.WriteQuotes(emit, setting);
                emit.Branch(returnLbl);

                emit.MarkLabel(nullableLbel);
                if (setting.WriteNullWhenObjectIsNull)
                    this.WriteNull(emit, setting);
                else
                    this.WriteObjectSigil(emit, setting);
                emit.Branch(returnLbl);
                emit.MarkLabel(returnLbl);
                emit.Nop();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 构建enum
        /// </summary>
        /// <param name="emit">emit构建</param>
        /// <param name="setting">配置</param>
        /// <param name="memberType">成员类型</param>
        /// <returns></returns>
        protected virtual bool BuildForEnumModule(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, Type memberType)
        {
            if (this.IsEnumType(memberType))
            {
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadArgument(2);
                this.WriteQuotes(emit, setting);
                emit.Call(SerialierBuilderHelper.GetEnumParseMethod(memberType));
                this.WriteQuotes(emit, setting);
                return true;
            }

            /*可空的enum类型*/
            if (this.IsNullableEnumType(memberType))
            {
                emit.LoadArgument(0);
                emit.LoadArgument(1);
                emit.LoadArgument(2);
                emit.Call(SerialierBuilderHelper.GetNullableEnumParseMethod(memberType, Nullable.GetUnderlyingType(memberType)));
                return true;
            }

            return false;
        }

        /// <summary>
        /// 构建enum
        /// </summary>
        /// <param name="emit">emit构建</param>
        /// <param name="setting">配置</param>
        /// <param name="instanceLocal">当前对象变量</param>
        /// <param name="sourceType">节点成员</param>
        /// <param name="member">成员</param>
        /// <param name="memberType">成员类型</param>
        /// <param name="attributes">特性</param>
        protected virtual bool BuildForEnumModule(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, ILocal instanceLocal, Type sourceType, MemberInfo member, Type memberType, Attribute[] attributes)
        {
            /*可空的enum类型*/
            if (this.IsNullableEnumType(memberType))
            {
                this.WriteMemberName(emit, setting, member, attributes);
                this.WriteColon(emit, setting);
                emit.LoadArgument(0);                                      // writer
                emit.LoadArgument(1);
                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }
                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod(true));
                else
                    emit.LoadField(((FieldInfo)member));

                emit.Call(SerialierBuilderHelper.GetNullableEnumParseMethod(memberType, Nullable.GetUnderlyingType(memberType)));
                return true;
            }

            /*不可空的enum类型*/
            if (this.IsEnumType(memberType))
            {
                this.WriteMemberName(emit, setting, member, attributes);
                this.WriteColon(emit, setting);
                emit.LoadArgument(0);                                      // writer
                emit.LoadArgument(1);
                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }
                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod(true));
                else
                    emit.LoadField(((FieldInfo)member));

                emit.Call(SerialierBuilderHelper.GetEnumParseMethod(memberType));

                return true;
            }

            return false;
        }

        /// <summary>
        /// 构建基元
        /// </summary>
        /// <param name="emit">emit构建</param>
        /// <param name="setting">配置</param>
        /// <param name="memberType">成员类型</param>
        /// <returns></returns>
        protected virtual bool BuildForPrimitiveModule(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, Type memberType)
        {
            /*不可空的基元类型*/
            if (this.IsPrimitiveOrInsideHandleType(memberType))
            {
                /*string*/
                if (memberType == typeof(string))
                {
                    emit.LoadArgument(0);                                      // writer
                    emit.LoadArgument(1);
                    emit.LoadArgument(2);
                    emit.Call(SerialierBuilderHelper.GetWriteMethod(memberType));
                    return true;
                }

                //当为数字的时候写引号，否则不写引号。但是默认都是写引号的
                var writeQuote = true;
                /*bool 类型在javascript中用引号的话不会正确转换，必需不能带引号的*/
                if (memberType == typeof(bool))
                {
                    if (setting.WriteNumberOnBoolenType && setting.WriteQuoteWhenObjectIsNumber)
                        writeQuote = true;
                    else
                        writeQuote = false;
                }
                else
                {
                    if (this.IsNoQuotationMarkType(memberType))
                        writeQuote = false;

                    if (writeQuote == false && setting.WriteQuoteWhenObjectIsNumber)
                        writeQuote = true;
                }

                if (writeQuote)
                    this.WriteQuotes(emit, setting);

                emit.LoadArgument(0);                                      // writer
                emit.LoadArgument(1);
                emit.LoadArgument(2);
                if (memberType == typeof(DateTime))
                    emit.Call(SerialierBuilderHelper.GetDateTimeWriteMethod());
                else
                    emit.Call(SerialierBuilderHelper.GetWriteMethod(memberType));

                if (writeQuote)
                    this.WriteQuotes(emit, setting);

                return true;
            }

            if (this.IsNullablePrimitiveOrInsideHandleType(memberType))
            {
                emit.LoadArgument(0);                                      // writer
                emit.LoadArgument(1);
                emit.LoadArgument(2);
                if (memberType == typeof(DateTime?))
                    emit.Call(SerialierBuilderHelper.GetNullableDateTimeWriteMethod());
                else
                    emit.Call(SerialierBuilderHelper.GetWriteMethod(memberType));

                return true;
            }

            return false;
        }

        /// <summary>
        /// 构建基元
        /// </summary>
        /// <param name="emit">emit构建</param>
        /// <param name="setting">配置</param>
        /// <param name="instanceLocal">当前对象变量</param>
        /// <param name="sourceType">节点成员</param>
        /// <param name="member">成员</param>
        /// <param name="memberType">成员类型</param>
        /// <param name="attributes">特性</param>
        protected virtual bool BuildForPrimitiveModule(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, ILocal instanceLocal, Type sourceType, MemberInfo member, Type memberType, Attribute[] attributes)
        {
            /*可空的基元类型*/
            if (this.IsNullablePrimitiveOrInsideHandleType(memberType))
            {
                this.WriteMemberName(emit, setting, member, attributes);
                this.WriteColon(emit, setting);

                emit.LoadArgument(0);                                      // writer
                emit.LoadArgument(1);                                      // setting
                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }
                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod(true));
                else
                    emit.LoadField(((FieldInfo)member));

                if (memberType == typeof(DateTime?))
                    emit.Call(SerialierBuilderHelper.GetNullableDateTimeWriteMethod());
                else
                    emit.Call(SerialierBuilderHelper.GetWriteMethod(memberType));

                return true;
            }

            /*不可空的基元类型*/
            if (this.IsPrimitiveOrInsideHandleType(memberType))
            {
                /*string*/
                if (memberType == typeof(string))
                {
                    this.WriteMemberName(emit, setting, member, attributes);
                    this.WriteColon(emit, setting);
                    emit.LoadArgument(0);                                      // writer
                    emit.LoadArgument(1);
                    if (instanceLocal == null)
                    {
                        if (sourceType.IsValueType)
                            emit.LoadArgumentAddress(2);
                        else
                            emit.LoadArgument(2);
                    }
                    else
                    {
                        if (sourceType.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);                             // @object
                    }
                    if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                        emit.Call(((PropertyInfo)member).GetGetMethod(true));
                    else
                        emit.LoadField(((FieldInfo)member));

                    emit.Call(SerialierBuilderHelper.GetWriteMethod(memberType));
                    return true;
                }

                this.WriteMemberName(emit, setting, member, attributes);
                this.WriteColon(emit, setting);

                //当为数字的时候写引号，否则不写引号。但是默认都是写引号的
                var writeQuote = true;
                /*bool 类型在javascript中用引号的话不会正确转换，必需不能带引号的*/
                if (memberType == typeof(bool))
                {
                    if (setting.WriteNumberOnBoolenType && setting.WriteQuoteWhenObjectIsNumber)
                        writeQuote = true;
                    else
                        writeQuote = false;
                }
                else
                {
                    if (this.IsNoQuotationMarkType(memberType))
                        writeQuote = false;

                    if (writeQuote == false && setting.WriteQuoteWhenObjectIsNumber)
                        writeQuote = true;
                }

                if (writeQuote)
                    this.WriteQuotes(emit, setting);

                emit.LoadArgument(0);                                      // writer
                emit.LoadArgument(1);                                      // setting
                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }
                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod(true));
                else
                    emit.LoadField(((FieldInfo)member));

                if (memberType == typeof(DateTime))
                    emit.Call(SerialierBuilderHelper.GetDateTimeWriteMethod());
                else
                    emit.Call(SerialierBuilderHelper.GetWriteMethod(memberType));

                if (writeQuote)
                    this.WriteQuotes(emit, setting);

                return true;
            }

            return false;
        }

        #endregion build members

        #region write type

        /// <summary>
        /// 写逗号分割
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="setting">配置</param>
        protected virtual void WriteCommaSeparated(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting)
        {
            emit.LoadArgument(0);
            emit.LoadConstant(",");
            emit.Call(WriteStringMethod);
        }

        /// <summary>
        /// 写成员key
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="setting">配置</param>
        /// <param name="member">成员</param>
        /// <param name="attributes">特性</param>
        protected virtual void WriteMemberName(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting, MemberInfo member, Attribute[] attributes)
        {
            emit.LoadArgument(0);
            emit.LoadConstant("\"");
            emit.Call(WriteStringMethod);

            emit.LoadArgument(0);
            emit.LoadConstant(this.LoadNotNullMemberName(member, attributes));
            emit.Call(WriteStringMethod);

            emit.LoadArgument(0);
            emit.LoadConstant("\"");
            emit.Call(WriteStringMethod);
        }

        /// <summary>
        /// 写空字符串
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="setting">配置</param>
        protected virtual void WriteNull(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting)
        {
            emit.LoadArgument(0);
            emit.LoadConstant("null");

            emit.Call(WriteStringMethod);
        }

        /// <summary>
        /// 写空字符串
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="setting">配置</param>
        protected virtual void WriteObjectSigil(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting)
        {
            emit.LoadArgument(0);
            emit.LoadConstant("{}");
            emit.Call(WriteStringMethod);
        }

        /// <summary>
        /// 写引号
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="setting">配置</param>
        protected virtual void WriteQuotes(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting)
        {
            emit.LoadArgument(0);
            emit.LoadConstant("\"");
            emit.Call(WriteStringMethod);
        }

        /// <summary>
        /// 写冒号
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="setting">配置</param>
        protected virtual void WriteColon(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting)
        {
            emit.LoadArgument(0);
            emit.LoadConstant(":");
            emit.Call(WriteStringMethod);
        }

        /// <summary>
        /// 将{写入流中
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="setting">配置</param>
        protected virtual void WriteObjectFrontSigil(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting)
        {
            emit.LoadArgument(0);
            emit.LoadConstant("{");
            emit.Call(WriteStringMethod);
        }

        /// <summary>
        /// 将}写入流中
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="setting">配置</param>
        protected virtual void WriteObjectLastSigil(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting)
        {
            emit.LoadArgument(0);
            emit.LoadConstant("}");
            emit.Call(WriteStringMethod);
        }

        /// <summary>
        /// 将[写入流中
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="setting">配置</param>
        protected virtual void WriteArrayFrontSigil(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting)
        {
            emit.LoadArgument(0);
            emit.LoadConstant("[");
            emit.Call(WriteStringMethod);
        }

        /// <summary>
        /// 将]写入流中
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="setting">配置</param>
        protected virtual void WriteArrayLastSigil(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, T, byte>> emit, JsonSerializeSetting setting)
        {
            emit.LoadArgument(0);
            emit.LoadConstant("]");
            emit.Call(WriteStringMethod);
        }

        #endregion write type

        #region load

        /// <summary>
        /// 加载名字
        /// </summary>
        /// <returns></returns>
        private string LoadNotNullMemberName(MemberInfo member, Attribute[] attributes)
        {
            var memberName = LoadMemberName(member, attributes);
            if (string.IsNullOrEmpty(memberName))
                return member.Name;

            return memberName;
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

        #endregion load
    }
}