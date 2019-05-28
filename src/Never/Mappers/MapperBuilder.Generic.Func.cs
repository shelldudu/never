using Never.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Never.Mappers
{
    /// <summary>
    /// 自动映射创建
    /// </summary>
    public partial class MapperBuilder<From, To> : MapperBuilder
    {
        #region build

        /// <summary>
        /// Build
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <returns></returns>
        public static Func<From, To> FuncBuild(MapperSetting setting)
        {
            Func<From, To> func = null;
            if (funcCaching.TryGetValue(setting, out func))
                return func;

            lock (funcCaching)
            {
                if (funcCaching.TryGetValue(setting, out func))
                    return func;

                var toType = typeof(To);
                var fromType = typeof(From);
                var emit = EasyEmitBuilder<Func<From, To>>.NewDynamicMethod();
                func = new MapperBuilder<From, To>().FuncBuild(emit, fromType, toType, setting);
                funcCaching.TryAdd(setting, func);
                return func;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="DelegateType"></typeparam>
        /// <param name="emit"></param>
        /// <param name="fromType"></param>
        /// <param name="toType"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        private DelegateType FuncBuild<DelegateType>(EasyEmitBuilder<DelegateType> emit, Type fromType, Type toType, MapperSetting setting)
        {
            var toMembers = GetMembers(toType);
            var fromMembers = GetMembers(fromType);
            var removes = new List<MemberInfo>(toMembers.Count);

            foreach (var toMember in toMembers)
            {
                if (toMember.MemberType == MemberTypes.Property)
                {
                    var p = (PropertyInfo)toMember;
                    if (!p.CanRead)
                        removes.Add(toMember);
                }
            }

            foreach (var r in removes)
            {
                toMembers.Remove(r);
            }

            removes.Clear();

            foreach (var fromMember in fromMembers)
            {
                if (fromMember.MemberType == MemberTypes.Property)
                {
                    var p = (PropertyInfo)fromMember;
                    if (!p.CanWrite)
                        removes.Add(fromMember);
                }
                else if (fromMember.MemberType == MemberTypes.Field)
                {
                    var f = (FieldInfo)fromMember;
                    if (f.IsInitOnly)
                        removes.Add(fromMember);
                }
            }

            foreach (var r in removes)
            {
                fromMembers.Remove(r);
            }

            /*要忽略的成员*/
            if (setting.IgnoredMembers != null && setting.IgnoredMembers.Length > 0)
            {
                foreach (var ig in setting.IgnoredMembers)
                {
                    var f = toMembers.Find(o => o.Name.Equals(ig, StringComparison.OrdinalIgnoreCase));
                    if (f != null)
                        toMembers.Remove(f);
                }
            }

            ILabel nullLabel = emit.DefineLabel();
            ILabel returnLabel = emit.DefineLabel();
            var toInstanceLocal = emit.DeclareLocal(toType);
            var fromInstanceLocal = emit.DeclareLocal(fromType);

            /*两都其中一处没有属性，直接返回，算优化*/
            if (toMembers.Count == 0 || fromMembers.Count == 0)
            {
                if (toType.IsValueType)
                {
                    var structlocal = emit.DeclareLocal(toType);
                    emit.LoadLocalAddress(structlocal);
                    emit.InitializeObject(toType);
                    emit.LoadLocal(structlocal);
                    emit.Return();
                    return emit.CreateDelegate();
                }

                /*总是构造新对象*/
                if (setting.AlwaysNewTraget)
                {
                    emit.NewObject(toType.GetConstructor(Type.EmptyTypes));
                    emit.Return();
                    return emit.CreateDelegate();
                }

                if (!fromType.IsValueType)
                {
                    emit.LoadArgument(0);
                    emit.LoadNull();
                    emit.CompareGreaterThan();
                    /*null*/
                    emit.BranchIfFalse(nullLabel);
                    emit.NewObject(toType.GetConstructor(Type.EmptyTypes));
                    emit.StoreLocal(toInstanceLocal);
                    emit.Branch(returnLabel);
                    emit.MarkLabel(nullLabel);
                    emit.LoadNull();
                    emit.StoreLocal(toInstanceLocal);
                    emit.Branch(returnLabel);
                    emit.MarkLabel(returnLabel);
                    emit.LoadLocal(toInstanceLocal);
                    emit.Return();
                    return emit.CreateDelegate();
                }

                emit.NewObject(toType.GetConstructor(Type.EmptyTypes));
                emit.Return();
                return emit.CreateDelegate();
            }

            /*总是构造新对象*/
            if (setting.AlwaysNewTraget || toType.IsValueType)
            {
                if (toType.IsValueType)
                {
                    emit.LoadLocalAddress(toInstanceLocal);
                    emit.InitializeObject(toType);
                }
                else
                {
                    emit.NewObject(toType.GetConstructor(Type.EmptyTypes));
                    emit.StoreLocal(toInstanceLocal);
                }

                /*非值对象，要判断是否为空*/
                if (!fromType.IsValueType)
                {
                    emit.LoadArgument(0);
                    emit.LoadNull();
                    emit.CompareGreaterThan();
                    /*null*/
                    emit.BranchIfFalse(returnLabel);
                    emit.Nop();
                    emit.LoadArgument(0);
                    emit.StoreLocal(fromInstanceLocal);

                    /*常见的类型转换*/
                    FuncBuildNormalMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

                    /*复合对象*/
                    FuncBuildComplexTypeMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

                    emit.MarkLabel(returnLabel);
                    emit.LoadLocal(toInstanceLocal);
                    emit.Return();
                    return emit.CreateDelegate();
                }

                emit.LoadArgument(0);
                emit.StoreLocal(fromInstanceLocal);

                /*常见的类型转换*/
                FuncBuildNormalMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

                /*复合对象*/
                FuncBuildComplexTypeMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

                emit.LoadLocal(toInstanceLocal);
                emit.Return();
                return emit.CreateDelegate();
            }

            if (!fromType.IsValueType)
            {
                emit.LoadArgument(0);
                emit.LoadNull();
                emit.CompareGreaterThan();
                /*null*/
                emit.BranchIfFalse(nullLabel);
                emit.NewObject(toType.GetConstructor(Type.EmptyTypes));
                emit.StoreLocal(toInstanceLocal);

                emit.LoadArgument(0);
                emit.StoreLocal(fromInstanceLocal);

                /*常见的类型转换*/
                FuncBuildNormalMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

                /*复合对象*/
                FuncBuildComplexTypeMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

                emit.Branch(returnLabel);
                /*null*/
                emit.MarkLabel(nullLabel);
                emit.LoadNull();
                emit.StoreLocal(toInstanceLocal);
                emit.Branch(returnLabel);

                /*done*/
                emit.MarkLabel(returnLabel);
                emit.LoadLocal(toInstanceLocal);

                emit.Return();
                return emit.CreateDelegate();
            }

            emit.NewObject(toType.GetConstructor(Type.EmptyTypes));
            emit.StoreLocal(toInstanceLocal);
            emit.LoadArgument(0);
            emit.StoreLocal(fromInstanceLocal);

            /*常见的类型转换*/
            FuncBuildNormalMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

            /*复合对象*/
            FuncBuildComplexTypeMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

            emit.LoadLocal(toInstanceLocal);
            emit.Return();
            return emit.CreateDelegate();
        }

        #endregion build

        #region normal memberInfo

        /// <summary>
        /// 常见的对象类型映射
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="toLocal"></param>
        /// <param name="fromLocal"></param>
        /// <param name="emit"></param>
        /// <param name="toType"></param>
        /// <param name="toMembers"></param>
        /// <param name="fromType"></param>
        /// <param name="fromMembers"></param>
        /// <param name="level"></param>
        private void FuncBuildNormalMembers<DelegateType>(MapperSetting setting, ILocal toLocal, ILocal fromLocal, EasyEmitBuilder<DelegateType> emit, Type toType, List<MemberInfo> toMembers, Type fromType, List<MemberInfo> fromMembers, int level)
        {
            ILabel breakLbl = null;
            if (level > 0 && !fromType.IsValueType)
            {
                breakLbl = emit.DefineLabel();
                emit.DefineLabel();
                emit.LoadLocal(fromLocal);
                emit.LoadNull();
                emit.CompareGreaterThan();
                emit.BranchIfFalse(breakLbl);
            }

            for (var i = 0; i < toMembers.Count; i++)
            {
                var toMember = toMembers[i];
                if (toMember.MemberType == MemberTypes.Property)
                {
                    var prop = (PropertyInfo)toMember;
                    if (!prop.CanWrite)
                        continue;

                    if (prop.GetSetMethod(true).GetParameters().Length != 1)
                        continue;
                }

                /*在源对象中找不到该目标对象*/
                var fromMember = fromMembers.Find(o => toMember.Name.Equals(o.Name, StringComparison.OrdinalIgnoreCase));
                if (fromMember == null)
                    continue;

                if (fromMember.MemberType == MemberTypes.Property && !((PropertyInfo)fromMember).CanRead)
                    continue;

                var toMemberType = toMember.MemberType == MemberTypes.Property ? ((PropertyInfo)toMember).PropertyType : ((FieldInfo)toMember).FieldType;
                var nullableToMemberType = Nullable.GetUnderlyingType(toMemberType);
                if (nullableToMemberType == null)
                {
                    if (!toMemberType.IsEnum)
                    {
                        if (!MapperBuilderHelper.ContainType(toMemberType))
                            continue;
                    }
                }
                else
                {
                    if (!nullableToMemberType.IsEnum)
                    {
                        if (!MapperBuilderHelper.ContainType(nullableToMemberType))
                            continue;
                    }
                }

                var fromMemberType = fromMember.MemberType == MemberTypes.Property ? ((PropertyInfo)fromMember).PropertyType : ((FieldInfo)fromMember).FieldType;
                var nullableFromMemberType = Nullable.GetUnderlyingType(fromMemberType);
                if (nullableFromMemberType == null)
                {
                    if (!fromMemberType.IsEnum)
                    {
                        if (!MapperBuilderHelper.ContainType(fromMemberType))
                            continue;
                    }
                }
                else
                {
                    if (!nullableFromMemberType.IsEnum)
                    {
                        if (!MapperBuilderHelper.ContainType(nullableFromMemberType))
                            continue;
                    }
                }

                /*分四种情况，（1）目标类型为可空，源类型不可空；（2）目标类型为可空，源类型为可空；（3）目标类型不可空，源类型不可空；（4）目标类型不可空，源类型为可空*/
                /*1与2的情况*/
                if (nullableToMemberType != null)
                {
                    /*1的情况*/
                    if (nullableFromMemberType == null)
                    {
                        FuncBuildTargetNullableSourceNotNullable(setting, toLocal, fromLocal, emit, toType, toMember, nullableToMemberType, fromType, fromMember);
                        continue;
                    }

                    /*2的情况*/
                    FuncBuildTargetNullableSourceNullable(setting, toLocal, fromLocal, emit, toType, toMember, nullableToMemberType, fromType, fromMember, nullableFromMemberType);
                    continue;
                }

                /*3的情况*/
                if (nullableFromMemberType == null)
                {
                    FuncBuildTargetNotNullableSourceNotNullable(setting, toLocal, fromLocal, emit, toType, toMember, fromType, fromMember);
                    continue;
                }

                /*4的情况*/
                FuncBuildTargetNotNullableSourceNullable(setting, toLocal, fromLocal, emit, toType, toMember, fromType, fromMember, nullableFromMemberType);
                continue;
            }

            if (breakLbl != null)
            {
                emit.MarkLabel(breakLbl);
                emit.Nop();
            }
        }

        /// <summary>
        /// 复合的对象类型映射
        /// </summary>
        /// <typeparam name="DelegateType"></typeparam>
        /// <param name="setting"></param>
        /// <param name="toLocal"></param>
        /// <param name="fromLocal"></param>
        /// <param name="emit"></param>
        /// <param name="toType"></param>
        /// <param name="toMembers"></param>
        /// <param name="fromType"></param>
        /// <param name="fromMembers"></param>
        /// <param name="level"></param>
        private void FuncBuildComplexTypeMembers<DelegateType>(MapperSetting setting, ILocal toLocal, ILocal fromLocal, EasyEmitBuilder<DelegateType> emit, Type toType, List<MemberInfo> toMembers, Type fromType, List<MemberInfo> fromMembers, int level)
        {
            ILabel breakLbl = null;
            if (level > 0 && !fromType.IsValueType)
            {
                breakLbl = emit.DefineLabel();
                emit.DefineLabel();
                emit.LoadLocal(fromLocal);
                emit.LoadNull();
                emit.CompareGreaterThan();
                emit.BranchIfFalse(breakLbl);
            }

            for (var i = 0; i < toMembers.Count; i++)
            {
                var toMember = toMembers[i];
                if (toMember.MemberType == MemberTypes.Property)
                {
                    var prop = (PropertyInfo)toMember;
                    if (!prop.CanWrite)
                        continue;

                    if (prop.GetSetMethod(true).GetParameters().Length != 1)
                        continue;
                }

                /*在源对象中找不到该目标对象*/
                var fromMember = fromMembers.Find(o => toMember.Name.Equals(o.Name, StringComparison.OrdinalIgnoreCase));
                if (fromMember == null)
                    continue;

                if (fromMember.MemberType == MemberTypes.Property && !((PropertyInfo)fromMember).CanRead)
                    continue;

                bool isComplexType = false;
                /*目标类型不是复合对象*/
                var toMemberType = toMember.MemberType == MemberTypes.Property ? ((PropertyInfo)toMember).PropertyType : ((FieldInfo)toMember).FieldType;
                if (setting.ComplexMembers != null)
                {
                    foreach (var st in setting.ComplexMembers)
                    {
                        if (st.Equals(toMember.Name))
                        {
                            isComplexType = true;
                            break;
                        }
                    }
                }

                if (!isComplexType && IsComplexType(toMemberType))
                    isComplexType = true;

                if (!isComplexType)
                    continue;

                /*源类型不是复合对象*/
                var fromMemberType = fromMember.MemberType == MemberTypes.Property ? ((PropertyInfo)fromMember).PropertyType : ((FieldInfo)fromMember).FieldType;
                if (setting.ComplexMembers != null)
                {
                    foreach (var st in setting.ComplexMembers)
                    {
                        if (st.Equals(fromMemberType.Name))
                        {
                            isComplexType = true;
                            break;
                        }
                    }
                }

                if (!isComplexType && IsComplexType(fromMemberType))
                    isComplexType = true;

                if (!isComplexType)
                    continue;

                if (fromMemberType != toMemberType)
                {
                    if (setting.ShallowCopy)
                        continue;
                }

                if (setting.ShallowCopy)
                {
                    if (toType.IsValueType)
                        emit.LoadLocalAddress(toLocal); //to
                    else
                        emit.LoadLocal(toLocal);
                    if (fromType.IsValueType)
                        emit.LoadLocalAddress(fromLocal); // from
                    else
                        emit.LoadLocal(fromLocal);

                    if (fromMember.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                    else
                        emit.LoadField((FieldInfo)fromMember);

                    if (toMember.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
                    else
                        emit.StoreField((FieldInfo)toMember);

                    continue;
                }

                if (toMemberType.IsAbstract || toMemberType.IsInterface || toMemberType.IsImport)
                    continue;

                if (!toMemberType.IsValueType && toMemberType.GetConstructor(Type.EmptyTypes) == null)
                    continue;

                var subToLocal = emit.DeclareLocal(toMemberType);
                var subFromLocal = emit.DeclareLocal(fromMemberType);

                /*局部变量*/
                var subToMembers = GetMembers(toMemberType);
                var subFromMembers = GetMembers(fromMemberType);
                /*复合类型*/
                var subComplexType = subToMembers.FindAll(s => IsComplexType(s));

                if (toMemberType.IsValueType)
                {
                    emit.LoadLocalAddress(subToLocal);
                    emit.InitializeObject(toMemberType);

                    /*局部变量*/
                    if (fromType.IsValueType)
                        emit.LoadLocalAddress(fromLocal); // from
                    else
                        emit.LoadLocal(fromLocal);

                    if (fromMember.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                    else
                        emit.LoadField((FieldInfo)fromMember);

                    emit.StoreLocal(subFromLocal);

                    /*常见类型*/
                    FuncBuildNormalMembers(setting, subToLocal, subFromLocal, emit, toMemberType, subToMembers, fromMemberType, subFromMembers, level + 1);

                    if (subComplexType != null && subComplexType.Count > 0)
                        FuncBuildComplexTypeMembers(setting, subToLocal, subFromLocal, emit, toMemberType, subComplexType, fromMemberType, subFromMembers.FindAll(s => IsComplexType(s)), level + 1);

                    //emit.LoadLocal(subToLocal);
                    //emit.StoreLocal(subToLocal);

                    if (toType.IsValueType)
                        emit.LoadLocalAddress(toLocal); //to
                    else
                        emit.LoadLocal(toLocal);

                    emit.LoadLocal(subToLocal);
                    if (toMember.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                    else
                        emit.StoreField((FieldInfo)toMember);

                    continue;
                }

                emit.NewObject(toMemberType, Type.EmptyTypes);
                emit.StoreLocal(subToLocal);
                if (toType.IsValueType)
                    emit.LoadLocalAddress(toLocal); //to
                else
                    emit.LoadLocal(toLocal);

                emit.LoadLocal(subToLocal);
                if (toMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                else
                    emit.StoreField((FieldInfo)toMember);

                /*局部变量*/
                if (fromType.IsValueType)
                    emit.LoadLocalAddress(fromLocal); // from
                else
                    emit.LoadLocal(fromLocal);

                if (fromMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                else
                    emit.LoadField((FieldInfo)fromMember);

                emit.StoreLocal(subFromLocal);

                /*常见类型*/
                FuncBuildNormalMembers(setting, subToLocal, subFromLocal, emit, toMemberType, subToMembers, fromMemberType, subFromMembers, level + 1);

                if (subComplexType != null && subComplexType.Count > 0)
                    FuncBuildComplexTypeMembers(setting, subToLocal, subFromLocal, emit, toMemberType, subComplexType, fromMemberType, subFromMembers.FindAll(s => IsComplexType(s)), level + 1);

                continue;
            }

            if (breakLbl != null)
            {
                emit.MarkLabel(breakLbl);
                emit.Nop();
            }
        }

        #endregion normal memberInfo

        #region members

        /// <summary>
        /// （1）目标类型为可空，源类型不可空
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="toLocal"></param>
        /// <param name="fromLocal"></param>
        /// <param name="emit"></param>
        /// <param name="toType"></param>
        /// <param name="toMember"></param>
        /// <param name="innerToMemberType"></param>
        /// <param name="fromMember"></param>
        /// <param name="fromType"></param>
        private void FuncBuildTargetNullableSourceNotNullable<DelegateType>(MapperSetting setting, ILocal toLocal, ILocal fromLocal, EasyEmitBuilder<DelegateType> emit, Type toType, MemberInfo toMember, Type innerToMemberType, Type fromType, MemberInfo fromMember)
        {
            var toMemberType = toMember.MemberType == MemberTypes.Property ? ((PropertyInfo)toMember).PropertyType : ((FieldInfo)toMember).FieldType;
            var fromMemberType = fromMember.MemberType == MemberTypes.Property ? ((PropertyInfo)fromMember).PropertyType : ((FieldInfo)fromMember).FieldType;
            if (innerToMemberType == fromMemberType)
            {
                if (toType.IsValueType)
                    emit.LoadLocalAddress(toLocal); //to
                else
                    emit.LoadLocal(toLocal);
                if (fromType.IsValueType)
                    emit.LoadLocalAddress(fromLocal); // from
                else
                    emit.LoadLocal(fromLocal);

                if (fromMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                else
                    emit.LoadField((FieldInfo)fromMember);

                emit.NewObject(toMemberType, new[] { innerToMemberType });
                if (toMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
                else
                    emit.StoreField((FieldInfo)toMember);

                return;
            }

            /*不进行强制转换*/
            if (!setting.ForceConvertWhenTypeNotSame)
                return;

            /*两者不能转换，退出*/
            if (!CanConvert(innerToMemberType, fromMemberType))
                return;

            /*已定义的类型*/
            if (MapperBuilderHelper.ContainType(innerToMemberType) && MapperBuilderHelper.ContainType(fromMemberType))
            {
                var locals = new[]
                {
                    emit.DeclareLocal(fromMemberType),
                };

                if (toType.IsValueType)
                    emit.LoadLocalAddress(toLocal); //to
                else
                    emit.LoadLocal(toLocal);

                if (fromType.IsValueType)
                    emit.LoadLocalAddress(fromLocal); // from
                else
                    emit.LoadLocal(fromLocal);

                if (fromMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                else
                    emit.LoadField((FieldInfo)fromMember);

                emit.Call(MapperBuilderHelper.GetConvertMethod(innerToMemberType, fromMemberType));
                emit.NewObject(toMemberType, new[] { innerToMemberType });
                if (toMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
                else
                    emit.StoreField((FieldInfo)toMember);

                return;
            }

            /*不在定义内的*/
            if (toType.IsValueType)
                emit.LoadLocalAddress(toLocal); //to
            else
                emit.LoadLocal(toLocal);

            var defautlbl = emit.DeclareLocal(fromMemberType);
            emit.LoadLocalAddress(defautlbl);
            emit.InitializeObject(fromMemberType);
            emit.LoadLocal(defautlbl);
            emit.StoreLocal(defautlbl);
            emit.LoadLocal(defautlbl);
            emit.NewObject(toMemberType, new[] { fromMemberType });

            if (toMember.MemberType == MemberTypes.Property)
                emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
            else
                emit.StoreField((FieldInfo)toMember);

            return;
        }

        /// <summary>
        /// （2）目标类型为可空，源类型为可空
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="toLocal"></param>
        /// <param name="fromLocal"></param>
        /// <param name="emit"></param>
        /// <param name="toType"></param>
        /// <param name="toMember"></param>
        /// <param name="innerToMemberType"></param>
        /// <param name="fromMember"></param>
        /// <param name="fromType"></param>
        /// <param name="innerFromMemberType"></param>
        private void FuncBuildTargetNullableSourceNullable<DelegateType>(MapperSetting setting, ILocal toLocal, ILocal fromLocal, EasyEmitBuilder<DelegateType> emit, Type toType, MemberInfo toMember, Type innerToMemberType, Type fromType, MemberInfo fromMember, Type innerFromMemberType)
        {
            var toMemberType = toMember.MemberType == MemberTypes.Property ? ((PropertyInfo)toMember).PropertyType : ((FieldInfo)toMember).FieldType;
            var fromMemberType = fromMember.MemberType == MemberTypes.Property ? ((PropertyInfo)fromMember).PropertyType : ((FieldInfo)fromMember).FieldType;
            ILocal[] locals = null;
            ILabel nullableNoValue = emit.DefineLabel();

            if (toMemberType == fromMemberType)
            {
                if (toType.IsValueType)
                    emit.LoadLocalAddress(toLocal); //to
                else
                    emit.LoadLocal(toLocal);

                if (fromType.IsValueType)
                    emit.LoadLocalAddress(fromLocal); // from
                else
                    emit.LoadLocal(fromLocal);

                if (fromMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                else
                    emit.LoadField((FieldInfo)fromMember);

                if (toMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
                else
                    emit.StoreField((FieldInfo)toMember);

                return;
            }

            /*不进行强制转换*/
            if (!setting.ForceConvertWhenTypeNotSame)
                return;

            /*两者不能转换，退出*/
            if (!CanConvert(innerToMemberType, innerFromMemberType))
                return;

            /*已定义的类型*/
            if (MapperBuilderHelper.ContainType(innerToMemberType) && MapperBuilderHelper.ContainType(fromMemberType))
            {
                locals = new ILocal[2]
                {
                    emit.DeclareLocal(typeof(bool)),
                    emit.DeclareLocal(fromMemberType),
                };

                if (fromType.IsValueType)
                    emit.LoadLocalAddress(fromLocal); // from
                else
                    emit.LoadLocal(fromLocal);

                if (fromMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                else
                    emit.LoadField((FieldInfo)fromMember);

                emit.StoreLocal(locals[1]);
                emit.LoadLocalAddress(locals[1]);
                emit.Call(locals[1].LocalType.GetMethod("get_HasValue"));
                emit.StoreLocal(locals[0]);
                emit.LoadLocal(locals[0]);
                emit.BranchIfFalse(nullableNoValue);

                if (toType.IsValueType)
                    emit.LoadLocalAddress(toLocal); //to
                else
                    emit.LoadLocal(toLocal);
                emit.LoadLocalAddress(locals[1]);
                emit.Call(locals[1].LocalType.GetMethod("get_Value"));

                emit.Call(MapperBuilderHelper.GetConvertMethod(innerToMemberType, innerFromMemberType));
                emit.NewObject(toMemberType, new[] { innerToMemberType });
                if (toMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
                else
                    emit.StoreField((FieldInfo)toMember);
                emit.MarkLabel(nullableNoValue);
                emit.Nop();

                return;
            }

            /*不在定义内的*/
            emit.LoadLocal(toLocal);
            var defautlbl = emit.DeclareLocal(innerFromMemberType);
            emit.LoadLocalAddress(defautlbl);
            emit.InitializeObject(innerFromMemberType);
            emit.LoadLocal(defautlbl);
            emit.StoreLocal(defautlbl);
            emit.LoadLocal(defautlbl);
            emit.NewObject(toMemberType, new[] { innerToMemberType });
            if (toMember.MemberType == MemberTypes.Property)
                emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
            else
                emit.StoreField((FieldInfo)toMember);

            return;
        }

        /// <summary>
        /// （3）目标类型不可空，源类型不可空
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="toLocal"></param>
        /// <param name="fromLocal"></param>
        /// <param name="emit"></param>
        /// <param name="toType"></param>
        /// <param name="toMember"></param>
        /// <param name="fromType"></param>
        /// <param name="fromMember"></param>
        private void FuncBuildTargetNotNullableSourceNotNullable<DelegateType>(MapperSetting setting, ILocal toLocal, ILocal fromLocal, EasyEmitBuilder<DelegateType> emit, Type toType, MemberInfo toMember, Type fromType, MemberInfo fromMember)
        {
            var toMemberType = toMember.MemberType == MemberTypes.Property ? ((PropertyInfo)toMember).PropertyType : ((FieldInfo)toMember).FieldType;
            var fromMemberType = fromMember.MemberType == MemberTypes.Property ? ((PropertyInfo)fromMember).PropertyType : ((FieldInfo)fromMember).FieldType;
            if (toMemberType == fromMemberType)
            {
                if (toType.IsValueType)
                    emit.LoadLocalAddress(toLocal); //to
                else
                    emit.LoadLocal(toLocal);

                if (fromType.IsValueType)
                    emit.LoadLocalAddress(fromLocal); // from
                else
                    emit.LoadLocal(fromLocal);

                if (fromMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                else
                    emit.LoadField((FieldInfo)fromMember);

                if (toMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
                else
                    emit.StoreField((FieldInfo)toMember);

                return;
            }

            /*不进行强制转换*/
            if (!setting.ForceConvertWhenTypeNotSame)
                return;

            /*两者不能转换，退出*/
            if (!CanConvert(toMemberType, fromMemberType))
                return;

            /*已定义的类型*/
            if (MapperBuilderHelper.ContainType(toMemberType) && MapperBuilderHelper.ContainType(fromMemberType))
            {
                if (toType.IsValueType)
                    emit.LoadLocalAddress(toLocal); //to
                else
                    emit.LoadLocal(toLocal);

                if (fromType.IsValueType)
                    emit.LoadLocalAddress(fromLocal); // from
                else
                    emit.LoadLocal(fromLocal);

                if (fromMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                else
                    emit.LoadField((FieldInfo)fromMember);

                emit.Call(MapperBuilderHelper.GetConvertMethod(toMemberType, fromMemberType));
                if (toMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
                else
                    emit.StoreField((FieldInfo)toMember);

                return;
            }

            /*不在定义内的*/
            if (toMemberType == typeof(string))
            {
                if (TypeHelper.IsAssignableFrom(fromMemberType, typeof(IConvertible)))
                {
                    if (toType.IsValueType)
                        emit.LoadLocalAddress(toLocal); //to
                    else
                        emit.LoadLocal(toLocal);

                    if (fromType.IsValueType)
                        emit.LoadLocalAddress(fromLocal); // from
                    else
                        emit.LoadLocal(fromLocal);

                    if (fromMember.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                    else
                        emit.LoadField((FieldInfo)fromMember);

                    emit.Call(typeof(MapperBuilderHelper).GetMethod("_GenericToString", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(fromMemberType));

                    if (toMember.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
                    else
                        emit.StoreField((FieldInfo)toMember);

                    return;
                }

                if (toType.IsValueType)
                    emit.LoadLocalAddress(toLocal); //to
                else
                    emit.LoadLocal(toLocal);

                if (fromType.IsValueType)
                    emit.LoadLocalAddress(fromLocal); // from
                else
                    emit.LoadLocal(fromLocal);

                if (fromMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                else
                    emit.LoadField((FieldInfo)fromMember);

                if (fromMemberType.IsValueType)
                    emit.Box(fromMemberType);

                emit.Call(typeof(object).GetMethod("ToString"));
                if (toMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
                else
                    emit.StoreField((FieldInfo)toMember);

                return;
            }

            /*default*/
            if (toType.IsValueType)
                emit.LoadLocalAddress(toLocal); //to
            else
                emit.LoadLocal(toLocal);

            var defautlbl = emit.DeclareLocal(toMemberType);
            emit.LoadLocalAddress(defautlbl);
            emit.InitializeObject(toMemberType);
            emit.LoadLocal(defautlbl);
            emit.StoreLocal(defautlbl);
            emit.LoadLocal(defautlbl);
            if (toMember.MemberType == MemberTypes.Property)
                emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
            else
                emit.StoreField((FieldInfo)toMember);

            return;
        }

        /// <summary>
        /// （4）目标类型不可空，源类型为可空
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="toLocal"></param>
        /// <param name="fromLocal"></param>
        /// <param name="emit"></param>
        /// <param name="toMember"></param>
        /// <param name="toType"></param>
        /// <param name="fromMember"></param>
        /// <param name="fromType"></param>
        /// <param name="innerFromMemberType"></param>
        private void FuncBuildTargetNotNullableSourceNullable<DelegateType>(MapperSetting setting, ILocal toLocal, ILocal fromLocal, EasyEmitBuilder<DelegateType> emit, Type toType, MemberInfo toMember, Type fromType, MemberInfo fromMember, Type innerFromMemberType)
        {
            var toMemberType = toMember.MemberType == MemberTypes.Property ? ((PropertyInfo)toMember).PropertyType : ((FieldInfo)toMember).FieldType;
            var fromMemberType = fromMember.MemberType == MemberTypes.Property ? ((PropertyInfo)fromMember).PropertyType : ((FieldInfo)fromMember).FieldType;
            ILocal[] locals = null;
            ILabel nullableNoValue = emit.DefineLabel();

            if (toMemberType == innerFromMemberType)
            {
                locals = new[]
                {
                    emit.DeclareLocal(typeof(bool)),
                    emit.DeclareLocal(fromMemberType),
                };

                if (fromType.IsValueType)
                    emit.LoadLocalAddress(fromLocal); // from
                else
                    emit.LoadLocal(fromLocal);

                if (fromMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                else
                    emit.LoadField((FieldInfo)fromMember);

                emit.StoreLocal(locals[1]);
                emit.LoadLocalAddress(locals[1]);
                emit.Call(locals[1].LocalType.GetMethod("get_HasValue"));
                emit.StoreLocal(locals[0]);
                emit.LoadLocal(locals[0]);
                emit.BranchIfFalse(nullableNoValue);
                if (toType.IsValueType)
                    emit.LoadLocalAddress(toLocal); //to
                else
                    emit.LoadLocal(toLocal);

                emit.LoadLocalAddress(locals[1]);
                emit.Call(locals[1].LocalType.GetMethod("get_Value"));
                if (toMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
                else
                    emit.StoreField((FieldInfo)toMember);

                emit.MarkLabel(nullableNoValue);
                emit.Nop();

                return;
            }

            /*不进行强制转换*/
            if (!setting.ForceConvertWhenTypeNotSame)
                return;

            /*两者不能转换，退出*/
            if (!CanConvert(toMemberType, innerFromMemberType))
                return;

            /*已定义的类型*/
            if (MapperBuilderHelper.ContainType(toMemberType) && MapperBuilderHelper.ContainType(fromMemberType))
            {
                locals = new[]
                {
                    emit.DeclareLocal(typeof(bool)),
                    emit.DeclareLocal(fromMemberType)
                };

                nullableNoValue = emit.DefineLabel();
                if (fromType.IsValueType)
                    emit.LoadLocalAddress(fromLocal); // from
                else
                    emit.LoadLocal(fromLocal);

                if (fromMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                else
                    emit.LoadField((FieldInfo)fromMember);

                emit.StoreLocal(locals[1]);
                emit.LoadLocalAddress(locals[1]);
                emit.Call(locals[1].LocalType.GetMethod("get_HasValue"));
                emit.StoreLocal(locals[0]);
                emit.LoadLocal(locals[0]);
                emit.BranchIfFalse(nullableNoValue);
                if (toType.IsValueType)
                    emit.LoadLocalAddress(toLocal); //to
                else
                    emit.LoadLocal(toLocal);
                emit.LoadLocalAddress(locals[1]);
                emit.Call(locals[1].LocalType.GetMethod("get_Value"));

                emit.Call(MapperBuilderHelper.GetConvertMethod(toMemberType, innerFromMemberType));
                if (toMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
                else
                    emit.StoreField((FieldInfo)toMember);
                emit.MarkLabel(nullableNoValue);
                emit.Nop();

                return;
            }

            /*不在定义内的*/
            if (toMemberType == typeof(string))
            {
                if (TypeHelper.IsAssignableFrom(innerFromMemberType, typeof(IConvertible)))
                {
                    locals = new[]
                    {
                        emit.DeclareLocal(typeof(bool)),
                        emit.DeclareLocal(fromMemberType)
                    };

                    if (fromType.IsValueType)
                        emit.LoadLocalAddress(fromLocal); // from
                    else
                        emit.LoadLocal(fromLocal);

                    if (fromMember.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                    else
                        emit.LoadField((FieldInfo)fromMember);

                    emit.StoreLocal(locals[1]);
                    emit.LoadLocalAddress(locals[1]);
                    emit.Call(locals[1].LocalType.GetMethod("get_HasValue"));
                    emit.StoreLocal(locals[0]);
                    emit.LoadLocal(locals[0]);
                    emit.BranchIfFalse(nullableNoValue);
                    if (toType.IsValueType)
                        emit.LoadLocalAddress(toLocal); //to
                    else
                        emit.LoadLocal(toLocal);
                    emit.LoadLocalAddress(locals[1]);
                    emit.Call(locals[1].LocalType.GetMethod("get_Value"));

                    emit.Call(typeof(MapperBuilderHelper).GetMethod("_GenericToString", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(innerFromMemberType));

                    if (toMember.MemberType == MemberTypes.Property)
                        emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
                    else
                        emit.StoreField((FieldInfo)toMember);

                    emit.MarkLabel(nullableNoValue);
                    emit.Nop();

                    return;
                }

                locals = new[]
                {
                    emit.DeclareLocal(typeof(bool)),
                    emit.DeclareLocal(fromMemberType)
                };

                if (fromType.IsValueType)
                    emit.LoadLocalAddress(fromLocal); // from
                else
                    emit.LoadLocal(fromLocal);

                if (fromMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                else
                    emit.LoadField((FieldInfo)fromMember);

                emit.StoreLocal(locals[1]);
                emit.LoadLocalAddress(locals[1]);
                emit.Call(locals[1].LocalType.GetMethod("get_HasValue"));
                emit.StoreLocal(locals[0]);
                emit.LoadLocal(locals[0]);
                emit.BranchIfFalse(nullableNoValue);
                if (toType.IsValueType)
                    emit.LoadLocalAddress(toLocal); //to
                else
                    emit.LoadLocal(toLocal);
                emit.LoadLocalAddress(locals[1]);
                emit.Call(locals[1].LocalType.GetMethod("get_Value"));
                if (innerFromMemberType.IsValueType)
                    emit.Box(innerFromMemberType);

                emit.Call(typeof(object).GetMethod("ToString"));
                if (toMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
                else
                    emit.StoreField((FieldInfo)toMember);

                emit.MarkLabel(nullableNoValue);
                emit.Nop();

                return;
            }

            /*default*/
            if (toType.IsValueType)
                emit.LoadLocalAddress(toLocal); //to
            else
                emit.LoadLocal(toLocal);

            var defautlbl = emit.DeclareLocal(innerFromMemberType);
            emit.LoadLocalAddress(defautlbl);
            emit.InitializeObject(innerFromMemberType);
            emit.LoadLocal(defautlbl);
            emit.StoreLocal(defautlbl);
            emit.LoadLocal(defautlbl);
            if (toMember.MemberType == MemberTypes.Property)
                emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // to set method
            else
                emit.StoreField((FieldInfo)toMember);

            return;
        }

        #endregion members
    }
}