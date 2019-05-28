using Never.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Never.SqlClient
{
    /// <summary>
    /// 对在读每一行进行Emit构建
    /// </summary>
    public class DataRecordBuilder<T>
    {
        #region field

        /// <summary>
        /// The function
        /// </summary>
        public static Func<IDataRecord, T> Func = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes the <see cref="DataRecordBuilder{T}"/> class.
        /// </summary>
        static DataRecordBuilder()
        {
            Func = Build();
        }

        #endregion ctor

        #region build

        /// <summary>
        /// Build
        /// </summary>
        /// <returns></returns>
        public static Func<IDataRecord, T> Build()
        {
            if (Func != null)
                return Func;

            var type = typeof(T);
            var emit = EasyEmitBuilder<Func<IDataRecord, T>>.NewDynamicMethod(string.Format("{0}", type.Name));
            /*normal*/
            if (DataRecordBuilderHelper.DefinedTypeDict.ContainsKey(type))
            {
                BuildDefinedType(emit);
                return emit.CreateDelegate();
            }
            else if (type.IsEnum)
            {
                BuildEnumType(emit);
                return emit.CreateDelegate();
            }

            /* nullable<T> */
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
            {
                var genericType = type.GetGenericArguments()[0];
                if (DataRecordBuilderHelper.DefinedTypeDict.ContainsKey(genericType))
                {
                    BuildNullableDefinedType(emit, genericType);
                    return emit.CreateDelegate();
                }
                else if (genericType.IsEnum)
                {
                    BuildNullableEnumType(emit, genericType);
                    return emit.CreateDelegate();
                }
            }

            BuildObject(emit);
            return emit.CreateDelegate();
        }

        /// <summary>
        /// 对对象进行emit操作
        /// </summary>
        /// <param name="emit">The emit.</param>
        public static void BuildObject(EasyEmitBuilder<Func<IDataRecord, T>> emit)
        {
            var type = typeof(T);
            var targetMembers = GetMembers(type);

            /*实例*/
            var instanceLocal = emit.DeclareLocal(type);
            if (type.IsValueType)
            {
                if (targetMembers == null || targetMembers.Count == 0)
                {
                    emit.LoadLocalAddress(instanceLocal);
                    emit.InitializeObject(type);
                    emit.LoadLocal(instanceLocal);
                    emit.Return();
                    return;
                }

                emit.LoadLocalAddress(instanceLocal);
                emit.InitializeObject(type);
                emit.LoadLocal(instanceLocal);
                emit.StoreLocal(instanceLocal);
                goto _Read;
            }
            else
            {
                /*object*/
                var ctors = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (var ctor in ctors)
                {
                    if (ctor.GetParameters().Length == 0)
                    {
                        emit.NewObject(ctor);
                        emit.StoreLocal(instanceLocal);
                        if (targetMembers == null || targetMembers.Count == 0)
                        {
                            emit.LoadLocal(instanceLocal);
                            emit.Return();
                            return;
                        }

                        goto _Read;
                    }
                }

                throw new ArgumentException(string.Format("the type {0} can not find the no parameter of ctor method", typeof(T).FullName));
            }

        _Read:
            {
                var locals = new[]
                {
                    emit.DeclareLocal(typeof(bool)),
                    emit.DeclareLocal(typeof(int)),
                };

                var retlbl = emit.DefineLabel();
                var labels = new List<ILabel>(targetMembers.Count);
                for (var i = 0; i < targetMembers.Count; i++)
                {
                    labels.Add(emit.DefineLabel());
                }

                for (var i = 0; i < targetMembers.Count; i++)
                {
                    var member = targetMembers[i];
                    Type memberType = member.MemberType == MemberTypes.Property ? ((PropertyInfo)member).PropertyType : ((FieldInfo)member).FieldType;

                    if (i > 0)
                        emit.MarkLabel(labels[i]);

                    var attribute = member.GetCustomAttribute<TypeHandlerAttribute>();
                    if (attribute != null && attribute.TypeHandler.IsAssignableFromType(typeof(IReadingFromDataRecordToValueTypeHandler<>)))
                    {
                        TypeHandlerAttributeStorager<T>.Storage(attribute, member.Name);

                        if (type.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);

                        emit.LoadArgument(0);
                        emit.LoadConstant(member.Name);
                        if (member.MemberType == MemberTypes.Property)
                        {
                            emit.Call(typeof(DataRecordBuilderHelper).GetMethod("ReadingValueFromDataRecordTypeHandler").MakeGenericMethod(new[] { ((PropertyInfo)member).PropertyType, type }));
                            emit.Call(((PropertyInfo)member).GetSetMethod(true));
                        }
                        else
                        {
                            emit.Call(typeof(DataRecordBuilderHelper).GetMethod("ReadingValueFromDataRecordTypeHandler").MakeGenericMethod(new[] { ((FieldInfo)member).FieldType, type }));
                            emit.StoreField(((FieldInfo)member));
                        }

                        if (i < labels.Count - 1)
                            emit.Branch(labels[i + 1]);
                        else
                            emit.Branch(retlbl);
                        continue;
                    }

                    /*reader*/
                    emit.LoadArgument(0);
                    emit.LoadConstant(member.Name);
                    emit.Call(DataRecordBuilderHelper._mGetOrdinal);
                    emit.StoreLocal(locals[1]);
                    emit.LoadLocal(locals[1]);
                    emit.LoadConstant(0);
                    if (i < labels.Count - 1)
                        emit.BranchIfLess(labels[i + 1]);
                    else
                        emit.BranchIfLess(retlbl);

                    emit.LoadArgument(0);
                    emit.LoadLocal(locals[1]);
                    emit.Call(DataRecordBuilderHelper._mIsDBNull);
                    emit.StoreLocal(locals[0]);
                    emit.LoadLocal(locals[0]);
                    if (i < labels.Count - 1)
                        emit.BranchIfTrue(labels[i + 1]);
                    else
                        emit.BranchIfTrue(retlbl);

                    if (DataRecordBuilderHelper.DefinedTypeDict.ContainsKey(memberType))
                    {
                        if (type.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);

                        emit.LoadArgument(0);
                        emit.LoadLocal(locals[1]);
                        emit.Call(DataRecordBuilderHelper.DefinedTypeDict[memberType]);
                        if (member.MemberType == MemberTypes.Property)
                        {
                            emit.Call(((PropertyInfo)member).GetSetMethod(true));
                        }
                        else
                        {
                            emit.StoreField(((FieldInfo)member));
                        }

                        if (i < labels.Count - 1)
                            emit.Branch(labels[i + 1]);
                        else
                            emit.Branch(retlbl);
                        continue;
                    }
                    else if (memberType.IsEnum)
                    {
                        if (type.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);
                        emit.LoadArgument(0);
                        emit.LoadLocal(locals[1]);

                        var underlyingType = Enum.GetUnderlyingType(memberType);
                        emit.Call(DataRecordBuilderHelper.DefinedTypeDict[underlyingType]);
                        emit.Call(typeof(DataRecordBuilderHelper).GetMethod("_EnumParse", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(new[] { memberType, underlyingType }));

                        //emit.Call(DataRecordBuilderHelper.DefinedTypeDict[typeof(long)]);
                        //emit.Call(typeof(DataRecordBuilderHelper).GetMethod("_EnumParse", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(memberType));

                        if (member.MemberType == MemberTypes.Property)
                        {
                            emit.Call(((PropertyInfo)member).GetSetMethod(true));
                        }
                        else
                        {
                            emit.StoreField(((FieldInfo)member));
                        }
                        if (i < labels.Count - 1)
                            emit.Branch(labels[i + 1]);
                        else
                            emit.Branch(retlbl);

                        continue;
                    }
                    else if (Nullable.GetUnderlyingType(memberType) != null)
                    {
                        var nullable = Nullable.GetUnderlyingType(memberType);
                        if (nullable.IsEnum)
                        {
                            if (type.IsValueType)
                                emit.LoadLocalAddress(instanceLocal);
                            else
                                emit.LoadLocal(instanceLocal);
                            emit.LoadArgument(0);
                            emit.LoadLocal(locals[1]);

                            var underlyingType = Enum.GetUnderlyingType(nullable);
                            emit.Call(DataRecordBuilderHelper.DefinedTypeDict[underlyingType]);
                            emit.Call(typeof(DataRecordBuilderHelper).GetMethod("_EnumParse", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(new[] { nullable, underlyingType }));

                            //emit.Call(DataRecordBuilderHelper.DefinedTypeDict[typeof(long)]);
                            //emit.Call(typeof(DataRecordBuilderHelper).GetMethod("_EnumParse", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(nullable));
                            emit.NewObject(memberType, new[] { nullable });
                            if (member.MemberType == MemberTypes.Property)
                            {
                                emit.Call(((PropertyInfo)member).GetSetMethod(true));
                            }
                            else
                            {
                                emit.StoreField(((FieldInfo)member));
                            }
                            if (i < labels.Count - 1)
                                emit.Branch(labels[i + 1]);
                            else
                                emit.Branch(retlbl);
                            continue;
                        }

                        if (!DataRecordBuilderHelper.DefinedTypeDict.ContainsKey(nullable))
                            continue;

                        if (type.IsValueType)
                            emit.LoadLocalAddress(instanceLocal);
                        else
                            emit.LoadLocal(instanceLocal);
                        emit.LoadArgument(0);
                        emit.LoadLocal(locals[1]);
                        emit.Call(DataRecordBuilderHelper.DefinedTypeDict[nullable]);
                        emit.NewObject(memberType, new[] { nullable });
                        if (member.MemberType == MemberTypes.Property)
                        {
                            emit.Call(((PropertyInfo)member).GetSetMethod(true));
                        }
                        else
                        {
                            emit.StoreField(((FieldInfo)member));
                        }
                        if (i < labels.Count - 1)
                            emit.Branch(labels[i + 1]);
                        else
                            emit.Branch(retlbl);

                        continue;
                    }
                    else
                    {
                        emit.Nop();
                    }
                }

                emit.MarkLabel(retlbl);
                emit.LoadLocal(instanceLocal);
                emit.Return();
            };

            return;
        }

        /// <summary>
        /// 对常见的值类型进行emit操作
        /// </summary>
        /// <param name="emit">emit操作</param>
        private static void BuildDefinedType(EasyEmitBuilder<Func<IDataRecord, T>> emit)
        {
            var type = typeof(T);
            var dbnullLabel = emit.DefineLabel();
            var retLbl = emit.DefineLabel();
            var locals = new[]
                {
                    emit.DeclareLocal(typeof(bool)),
                    emit.DeclareLocal(type),
                };

            emit.LoadArgument(0);
            emit.LoadConstant(0);
            emit.Call(DataRecordBuilderHelper._mIsDBNull);
            emit.StoreLocal(locals[0]);
            emit.LoadLocal(locals[0]);
            emit.BranchIfTrue(dbnullLabel);

            emit.LoadArgument(0);
            emit.LoadConstant(0);
            emit.Call(DataRecordBuilderHelper.DefinedTypeDict[type]);
            emit.StoreLocal(locals[1]);
            emit.Branch(retLbl);

            emit.MarkLabel(dbnullLabel);
            emit.LoadLocalAddress(locals[1]);
            emit.InitializeObject(type);
            emit.LoadLocal(locals[1]);
            emit.StoreLocal(locals[1]);
            emit.Branch(retLbl);

            emit.MarkLabel(retLbl);
            emit.LoadLocal(locals[1]);
            emit.Return();
        }

        /// <summary>
        /// 对可空的常见的值类型进行emit操作
        /// </summary>
        /// <param name="emit">emit操作</param>
        /// <param name="innerType">对象类型</param>
        private static void BuildNullableDefinedType(EasyEmitBuilder<Func<IDataRecord, T>> emit, Type innerType)
        {
            var type = typeof(T);
            var dbnullLabel = emit.DefineLabel();
            var retLbl = emit.DefineLabel();
            var locals = new[]
                {
                    emit.DeclareLocal(typeof(bool)),
                    emit.DeclareLocal(type),
                };

            emit.LoadArgument(0);
            emit.LoadConstant(0);
            emit.Call(DataRecordBuilderHelper._mIsDBNull);
            emit.StoreLocal(locals[0]);
            emit.LoadLocal(locals[0]);
            emit.BranchIfTrue(dbnullLabel);

            emit.LoadArgument(0);
            emit.LoadConstant(0);
            emit.Call(DataRecordBuilderHelper.DefinedTypeDict[innerType]);
            emit.NewObject(typeof(T).GetConstructor(new[] { innerType }));
            emit.StoreLocal(locals[1]);
            emit.Branch(retLbl);

            emit.MarkLabel(dbnullLabel);
            emit.LoadLocalAddress(locals[1]);
            emit.InitializeObject(type);
            emit.LoadLocal(locals[1]);
            emit.StoreLocal(locals[1]);
            emit.Branch(retLbl);

            emit.MarkLabel(retLbl);
            emit.LoadLocal(locals[1]);
            emit.Return();
        }

        /// <summary>
        /// 对枚举进行emit操作
        /// </summary>
        /// <param name="emit"></param>
        private static void BuildEnumType(EasyEmitBuilder<Func<IDataRecord, T>> emit)
        {
            var type = typeof(T);
            var underlyingType = Enum.GetUnderlyingType(type);
            var dbnullLabel = emit.DefineLabel();
            var retLbl = emit.DefineLabel();
            var locals = new[]
                {
                    emit.DeclareLocal(typeof(bool)),
                    emit.DeclareLocal(underlyingType),
                    emit.DeclareLocal(type),
                };

            emit.LoadArgument(0);
            emit.LoadConstant(0);
            emit.Call(DataRecordBuilderHelper._mIsDBNull);
            emit.StoreLocal(locals[0]);
            emit.LoadLocal(locals[0]);
            emit.BranchIfTrue(dbnullLabel);

            emit.LoadArgument(0);
            emit.LoadConstant(0);
            emit.Call(DataRecordBuilderHelper.DefinedTypeDict[underlyingType]);
            emit.StoreLocal(locals[1]);
            emit.LoadLocal(locals[1]);
            emit.Call(typeof(DataRecordBuilderHelper).GetMethod("_EnumParse", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(new[] { type, underlyingType }));

            //emit.Call(DataRecordBuilderHelper.DefinedTypeDict[typeof(long)]);
            //emit.StoreLocal(locals[1]);
            //emit.LoadLocal(locals[1]);
            //emit.Call(typeof(DataRecordBuilderHelper).GetMethod("_Int64EnumParse", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(typeof(T)));

            emit.StoreLocal(locals[2]);
            emit.Branch(retLbl);

            emit.MarkLabel(dbnullLabel);
            emit.LoadLocalAddress(locals[2]);
            emit.InitializeObject(type);
            emit.LoadLocal(locals[2]);
            emit.StoreLocal(locals[2]);
            emit.Branch(retLbl);

            emit.MarkLabel(retLbl);
            emit.LoadLocal(locals[2]);
            emit.Return();
        }

        /// <summary>
        /// 对枚举进行emit操作
        /// </summary>
        /// <param name="emit"></param>
        /// <param name="innerType">对象类型</param>
        private static void BuildNullableEnumType(EasyEmitBuilder<Func<IDataRecord, T>> emit, Type innerType)
        {
            var type = typeof(T);
            var dbnullLabel = emit.DefineLabel();
            var retLbl = emit.DefineLabel();
            var underlyingType = Enum.GetUnderlyingType(innerType);
            var locals = new[]
                {
                    emit.DeclareLocal(typeof(bool)),
                    emit.DeclareLocal(underlyingType),
                    emit.DeclareLocal(type),
                };

            emit.LoadArgument(0);
            emit.LoadConstant(0);
            emit.Call(DataRecordBuilderHelper._mIsDBNull);
            emit.StoreLocal(locals[0]);
            emit.LoadLocal(locals[0]);
            emit.BranchIfTrue(dbnullLabel);

            emit.LoadArgument(0);
            emit.LoadConstant(0);
            emit.Call(DataRecordBuilderHelper.DefinedTypeDict[underlyingType]);
            emit.StoreLocal(locals[1]);
            emit.LoadLocal(locals[1]);
            emit.Call(typeof(DataRecordBuilderHelper).GetMethod("_EnumParse", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(new[] { innerType, underlyingType }));

            //emit.Call(DataRecordBuilderHelper.DefinedTypeDict[typeof(long)]);
            //emit.StoreLocal(locals[1]);
            //emit.LoadLocal(locals[1]);
            //emit.Call(typeof(DataRecordBuilderHelper).GetMethod("_Int64EnumParse", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(innerType));

            emit.NewObject(typeof(T).GetConstructor(new[] { innerType }));
            emit.StoreLocal(locals[2]);
            emit.Branch(retLbl);

            emit.MarkLabel(dbnullLabel);
            emit.LoadLocalAddress(locals[2]);
            emit.InitializeObject(type);
            emit.LoadLocal(locals[2]);
            emit.StoreLocal(locals[2]);
            emit.Branch(retLbl);

            emit.MarkLabel(retLbl);
            emit.LoadLocal(locals[2]);
            emit.Return();
        }

        #endregion build

        #region members

        /// <summary>
        /// 获取成员
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static List<MemberInfo> GetMembers(Type targetType)
        {
            var members = targetType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
            if (members == null || members.Length == 0)
                return new List<MemberInfo>(0);

            var list = new List<MemberInfo>(members.Length);
            foreach (var member in members)
            {
                if (member.MemberType == MemberTypes.Property)
                {
                    var p = (PropertyInfo)member;
                    if (p.CanWrite)
                        list.Add(member);
                }
                else if (member.MemberType == MemberTypes.Field)
                {
                    var f = (FieldInfo)member;
                    if (f.IsInitOnly)
                        continue;

                    list.Add(member);
                }
            }

            return list;
        }

        #endregion members
    }
}