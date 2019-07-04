using Never.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Never.Mappers
{
    /// <summary>
    /// 自动映射创建，To对象在传入之前已经创建好
    /// </summary>
    public partial class MapperBuilder<From, To> : MapperBuilder
    {
        #region build

        /// <summary>
        /// Build
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <returns></returns>
        public static Action<From, MapperContext, To> ActionBuild(MapperSetting setting)
        {
            Action<From, MapperContext, To> action = null;
            if (actionCaching.TryGetValue(setting, out action))
                return action;

            lock (actionCaching)
            {
                if (actionCaching.TryGetValue(setting, out action))
                    return action;

                var toType = typeof(To);
                var fromType = typeof(From);
                var emit = EasyEmitBuilder<Action<From, MapperContext, To>>.NewDynamicMethod();
                action = new MapperBuilder<From, To>().ActionBuild(emit, fromType, toType, setting);
                actionCaching.TryAdd(setting, action);
                return action;
            }
        }

        private DelegateType ActionBuild<DelegateType>(EasyEmitBuilder<DelegateType> emit, Type fromType, Type toType, MapperSetting setting)
        {
            if (MapperBuilderHelper.TryGetConvertMethod(toType, fromType, out var primitiveTypeMethod))
            {
                emit.Return();
                return emit.CreateDelegate();
            }
            if (Nullable.GetUnderlyingType(toType) != null || Nullable.GetUnderlyingType(fromType) != null)
            {
                emit.Return();
                return emit.CreateDelegate();
            }

            var toMembers = GetMembers(toType);
            var fromMembers = GetMembers(fromType);
            var removes = new List<MemberInfo>(toMembers.Count);

            foreach (var toMember in toMembers)
            {
                if (toMember.MemberType == MemberTypes.Property)
                {
                    var p = (PropertyInfo)toMember;
                    if (!p.CanWrite)
                    {
                        removes.Add(toMember);
                        continue;
                    }

                    if (p.GetSetMethod(true) != null && p.GetSetMethod(true).GetParameters().Length != 1)
                    {
                        removes.Add(toMember);
                        continue;
                    }
                }
                else if (toMember.MemberType == MemberTypes.Field)
                {
                    var f = (FieldInfo)toMember;
                    if (f.IsInitOnly)
                    {
                        removes.Add(toMember);
                        continue;
                    }
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
                    if (!p.CanRead)
                    {
                        removes.Add(fromMember);
                        continue;
                    }
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
                emit.Return();
                return emit.CreateDelegate();
            }

            /*总是构造新对象*/
            if (toType.IsValueType)
            {
                if (toType.IsValueType)
                {
                    emit.LoadArgument(2);
                    emit.StoreLocal(toInstanceLocal);
                }
                else
                {
                    emit.LoadArgument(2);
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
                    ActionBuildNormalMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

                    /*复合对象*/
                    ActionBuildComplexTypeMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

                    emit.MarkLabel(returnLabel);
                    emit.Nop();
                    //emit.LoadLocal(toInstanceLocal);
                    emit.Return();
                    return emit.CreateDelegate();
                }

                emit.LoadArgument(0);
                emit.StoreLocal(fromInstanceLocal);

                /*常见的类型转换*/
                ActionBuildNormalMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

                /*复合对象*/
                ActionBuildComplexTypeMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

                emit.Nop();
                //emit.LoadLocal(toInstanceLocal);
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
                emit.LoadArgument(2);
                emit.StoreLocal(toInstanceLocal);

                emit.LoadArgument(0);
                emit.StoreLocal(fromInstanceLocal);

                /*常见的类型转换*/
                ActionBuildNormalMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

                /*复合对象*/
                ActionBuildComplexTypeMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

                emit.Branch(returnLabel);
                /*null*/
                emit.MarkLabel(nullLabel);
                emit.LoadNull();
                emit.StoreLocal(toInstanceLocal);
                emit.Branch(returnLabel);

                /*done*/
                emit.MarkLabel(returnLabel);
                emit.Nop();
                //emit.LoadLocal(toInstanceLocal);

                emit.Return();
                return emit.CreateDelegate();
            }

            emit.LoadArgument(2);
            emit.StoreLocal(toInstanceLocal);
            emit.LoadArgument(0);
            emit.StoreLocal(fromInstanceLocal);

            /*常见的类型转换*/
            ActionBuildNormalMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

            /*复合对象*/
            ActionBuildComplexTypeMembers(setting, toInstanceLocal, fromInstanceLocal, emit, toType, toMembers, fromType, fromMembers, 0);

            emit.Nop();
            //emit.LoadLocal(toInstanceLocal);
            emit.Return();
            return emit.CreateDelegate();
        }

        #endregion build

        #region memberInfo

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
        protected void ActionBuildNormalMembers<DelegateType>(MapperSetting setting, ILocal toLocal, ILocal fromLocal, EasyEmitBuilder<DelegateType> emit, Type toType, List<MemberInfo> toMembers, Type fromType, List<MemberInfo> fromMembers, int level)
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

            var members = toMembers.ToArray();
            for (var i = 0; i < members.Length; i++)
            {
                var toMember = members[i];
                /*在源对象中找不到该目标对象*/
                var fromMember = fromMembers.Find(o => toMember.Name.Equals(o.Name, StringComparison.OrdinalIgnoreCase));
                if (fromMember == null)
                {
                    toMembers.Remove(toMember);
                    continue;
                }

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
                        ActionBuildTargetNullableSourceNotNullable(setting, toLocal, fromLocal, emit, toType, toMember, nullableToMemberType, fromType, fromMember);
                        toMembers.Remove(toMember);
                        continue;
                    }

                    /*2的情况*/
                    ActionBuildTargetNullableSourceNullable(setting, toLocal, fromLocal, emit, toType, toMember, nullableToMemberType, fromType, fromMember, nullableFromMemberType);
                    toMembers.Remove(toMember);
                    continue;
                }

                /*3的情况*/
                if (nullableFromMemberType == null)
                {
                    ActionBuildTargetNotNullableSourceNotNullable(setting, toLocal, fromLocal, emit, toType, toMember, fromType, fromMember);
                    toMembers.Remove(toMember);
                    continue;
                }

                /*4的情况*/
                ActionBuildTargetNotNullableSourceNullable(setting, toLocal, fromLocal, emit, toType, toMember, fromType, fromMember, nullableFromMemberType);
                toMembers.Remove(toMember);
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
        private void ActionBuildComplexTypeMembers<DelegateType>(MapperSetting setting, ILocal toLocal, ILocal fromLocal, EasyEmitBuilder<DelegateType> emit, Type toType, List<MemberInfo> toMembers, Type fromType, List<MemberInfo> fromMembers, int level)
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

            var members = toMembers.ToArray();
            for (var i = 0; i < members.Length; i++)
            {
                var toMember = members[i];
                /*在源对象中找不到该目标对象*/
                var fromMember = fromMembers.Find(o => toMember.Name.Equals(o.Name, StringComparison.OrdinalIgnoreCase));
                if (fromMember == null)
                {
                    toMembers.Remove(toMember);
                    continue;
                }

                /*目标类型不是复合对象*/
                var toMemberType = toMember.MemberType == MemberTypes.Property ? ((PropertyInfo)toMember).PropertyType : ((FieldInfo)toMember).FieldType;
                if (!IsComplexType(toMemberType))
                    continue;

                /*源类型不是复合对象*/
                var fromMemberType = fromMember.MemberType == MemberTypes.Property ? ((PropertyInfo)fromMember).PropertyType : ((FieldInfo)fromMember).FieldType;
                if (!IsComplexType(fromMemberType))
                    continue;

                toMembers.Remove(toMember);
                var subToLocal = emit.DeclareLocal(toMemberType);
                var subFromLocal = emit.DeclareLocal(fromMemberType);

                //处理数组
                if (toMemberType.IsAssignableFromType(typeof(IEnumerable<>)))
                {
                    //没有匹配
                    var notMatchs = new List<bool>(5);
                    //来源一定要是IEnumerable
                    if (fromMemberType.IsAssignableFromType(typeof(IEnumerable<>)) == true)
                    {
                        //目标是字典
                        if (toMemberType.IsAssignableFromType(typeof(IDictionary<,>)))
                        {
                            var fromkeyvaluePairTypeType = this.FindIEnumerableKeyValuePairGenericType(fromMemberType);
                            if (fromkeyvaluePairTypeType == null)
                            {
                                notMatchs.Add(true);
                                continue;
                            }
                            var tokeyvaluePairTypeType = this.FindIEnumerableKeyValuePairGenericType(toMemberType);
                            var dictionaryTypes = new Type[4];
                            Array.Copy(tokeyvaluePairTypeType.GetGenericArguments()[0].GetGenericArguments(), 0, dictionaryTypes, 0, 2);
                            Array.Copy(fromkeyvaluePairTypeType.GetGenericArguments()[0].GetGenericArguments(), 0, dictionaryTypes, 2, 2);
                            if (toMemberType.IsClass)
                            {
                                if (toType.IsValueType)
                                    emit.LoadLocalAddress(toLocal); //to
                                else
                                    emit.LoadLocal(toLocal);

                                if (toMember.MemberType == MemberTypes.Property)
                                    emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                                else
                                    emit.LoadField((FieldInfo)toMember);

                                var dictionaryNullLabel = emit.DefineLabel();
                                var dictionaryDoneLabel = emit.DefineLabel();
                                var dictionaryContinueLabel = emit.DefineLabel();
                                emit.LoadNull();
                                emit.CompareGreaterThan();
                                emit.BranchIfFalse(dictionaryNullLabel);
                                if (toType.IsValueType)
                                    emit.LoadLocalAddress(toLocal); //to
                                else
                                    emit.LoadLocal(toLocal);

                                if (toMember.MemberType == MemberTypes.Property)
                                    emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                                else
                                    emit.LoadField((FieldInfo)toMember);
                                emit.StoreLocal(subToLocal);
                                emit.Branch(dictionaryDoneLabel);
                                emit.MarkLabel(dictionaryNullLabel);
                                if (emit.TryNewObject(toMemberType, Type.EmptyTypes) == false)
                                {
                                    emit.Branch(dictionaryContinueLabel);
                                }
                                else
                                {
                                    emit.StoreLocal(subToLocal);
                                }
                                emit.Branch(dictionaryDoneLabel);
                                emit.MarkLabel(dictionaryContinueLabel);
                                emit.Nop();
                                emit.MarkLabel(dictionaryDoneLabel);
                                emit.LoadLocal(subToLocal);

                                /*局部变量*/
                                if (fromType.IsValueType)
                                    emit.LoadLocalAddress(fromLocal); // from
                                else
                                    emit.LoadLocal(fromLocal);

                                if (fromMember.MemberType == MemberTypes.Property)
                                    emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                                else
                                    emit.LoadField((FieldInfo)fromMember);

                                emit.LoadArgument(1);
                                if (toMemberType.IsAssignableFromType(typeof(ICollection<>)))
                                {
                                    emit.LoadArgument(1);
                                    emit.Call(typeof(MapperBuilderHelper).GetMethod("KeyValuePairLoadIntoCollection").MakeGenericMethod(dictionaryTypes));
                                }
                                else
                                {
                                    emit.LoadArgument(1);
                                    emit.Call(typeof(MapperBuilderHelper).GetMethod("KeyValuePairLoadIntoDictionary").MakeGenericMethod(dictionaryTypes));
                                }

                                /*局部变量*/
                                if (toType.IsValueType)
                                    emit.LoadLocalAddress(toLocal); // from
                                else
                                    emit.LoadLocal(toLocal);

                                emit.LoadLocal(subToLocal);
                                if (toMember.MemberType == MemberTypes.Property)
                                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                                else
                                    emit.StoreField((FieldInfo)toMember);

                                emit.Nop();
                                continue;
                            }
                            else if (toMemberType.IsValueType)
                            {
                                if (toType.IsValueType)
                                    emit.LoadLocalAddress(toLocal); //to
                                else
                                    emit.LoadLocal(toLocal);

                                if (toMember.MemberType == MemberTypes.Property)
                                    emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                                else
                                    emit.LoadField((FieldInfo)toMember);

                                emit.StoreLocal(subToLocal);
                                emit.LoadLocal(subToLocal);
                                emit.Box(toMemberType);
                                /*局部变量*/
                                if (fromType.IsValueType)
                                    emit.LoadLocalAddress(fromLocal); // from
                                else
                                    emit.LoadLocal(fromLocal);

                                if (fromMember.MemberType == MemberTypes.Property)
                                    emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                                else
                                    emit.LoadField((FieldInfo)fromMember);

                                if (toMemberType.IsAssignableFromType(typeof(ICollection<>)))
                                {
                                    emit.LoadArgument(1);
                                    emit.Call(typeof(MapperBuilderHelper).GetMethod("KeyValuePairLoadIntoCollection").MakeGenericMethod(dictionaryTypes));
                                }
                                else
                                {
                                    emit.LoadArgument(1);
                                    emit.Call(typeof(MapperBuilderHelper).GetMethod("KeyValuePairLoadIntoDictionary").MakeGenericMethod(dictionaryTypes));
                                }

                                /*局部变量*/
                                if (toType.IsValueType)
                                    emit.LoadLocalAddress(toLocal); // from
                                else
                                    emit.LoadLocal(toLocal);

                                emit.LoadLocal(subToLocal);
                                if (toMember.MemberType == MemberTypes.Property)
                                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                                else
                                    emit.StoreField((FieldInfo)toMember);

                                emit.Nop();
                                continue;
                            }
                            else
                            {
                                if (toType.IsValueType)
                                    emit.LoadLocalAddress(toLocal); //to
                                else
                                    emit.LoadLocal(toLocal);

                                if (toMember.MemberType == MemberTypes.Property)
                                    emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                                else
                                    emit.LoadField((FieldInfo)toMember);

                                var dictionaryNullLabel = emit.DefineLabel();
                                var dictionaryDoneLabel = emit.DefineLabel();
                                var dictionaryContinueLabel = emit.DefineLabel();
                                emit.LoadNull();
                                emit.CompareGreaterThan();
                                emit.BranchIfFalse(dictionaryNullLabel);
                                if (toType.IsValueType)
                                    emit.LoadLocalAddress(toLocal); //to
                                else
                                    emit.LoadLocal(toLocal);

                                if (toMember.MemberType == MemberTypes.Property)
                                    emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                                else
                                    emit.LoadField((FieldInfo)toMember);
                                emit.StoreLocal(subToLocal);
                                emit.Branch(dictionaryDoneLabel);
                                emit.MarkLabel(dictionaryNullLabel);
                                emit.NewObject(typeof(Dictionary<,>).MakeGenericType(dictionaryTypes[0], dictionaryTypes[1]), Type.EmptyTypes);
                                emit.StoreLocal(subToLocal);
                                emit.Branch(dictionaryDoneLabel);
                                emit.MarkLabel(dictionaryContinueLabel);
                                emit.Nop();
                                emit.MarkLabel(dictionaryDoneLabel);
                                emit.LoadLocal(subToLocal);
                                /*局部变量*/
                                if (fromType.IsValueType)
                                    emit.LoadLocalAddress(fromLocal); // from
                                else
                                    emit.LoadLocal(fromLocal);

                                if (fromMember.MemberType == MemberTypes.Property)
                                    emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                                else
                                    emit.LoadField((FieldInfo)fromMember);

                                emit.LoadArgument(1);
                                emit.Call(typeof(MapperBuilderHelper).GetMethod("KeyValuePairLoadIntoDictionary").MakeGenericMethod(dictionaryTypes));
                                /*局部变量*/
                                if (toType.IsValueType)
                                    emit.LoadLocalAddress(toLocal); // from
                                else
                                    emit.LoadLocal(toLocal);

                                emit.LoadLocal(subToLocal);
                                if (toMember.MemberType == MemberTypes.Property)
                                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                                else
                                    emit.StoreField((FieldInfo)toMember);

                                emit.Nop();
                                continue;
                            }
                        }

                        //目标是ICollection
                        if (toMemberType.IsAssignableFromType(typeof(ICollection<>)) && fromMemberType.IsAssignableFromType(typeof(ICollection<>)))
                        {
                            var tokeyvaluePairTypeType = this.FindICollectionKeyValuePairGenericType(toMemberType);
                            var fromkeyvaluePairTypeType = this.FindICollectionKeyValuePairGenericType(fromMemberType);
                            if (tokeyvaluePairTypeType != null && fromkeyvaluePairTypeType != null)
                            {
                                var dictionaryTypes = new Type[4];
                                Array.Copy(tokeyvaluePairTypeType.GetGenericArguments()[0].GetGenericArguments(), 0, dictionaryTypes, 0, 2);
                                Array.Copy(fromkeyvaluePairTypeType.GetGenericArguments()[0].GetGenericArguments(), 0, dictionaryTypes, 2, 2);
                                if (toMemberType.IsClass)
                                {
                                    if (toType.IsValueType)
                                        emit.LoadLocalAddress(toLocal); //to
                                    else
                                        emit.LoadLocal(toLocal);

                                    if (toMember.MemberType == MemberTypes.Property)
                                        emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                                    else
                                        emit.LoadField((FieldInfo)toMember);

                                    var dictionaryNullLabel = emit.DefineLabel();
                                    var dictionaryDoneLabel = emit.DefineLabel();
                                    var dictionaryContinueLabel = emit.DefineLabel();
                                    emit.LoadNull();
                                    emit.CompareGreaterThan();
                                    emit.BranchIfFalse(dictionaryNullLabel);
                                    if (toType.IsValueType)
                                        emit.LoadLocalAddress(toLocal); //to
                                    else
                                        emit.LoadLocal(toLocal);

                                    if (toMember.MemberType == MemberTypes.Property)
                                        emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                                    else
                                        emit.LoadField((FieldInfo)toMember);
                                    emit.StoreLocal(subToLocal);
                                    emit.Branch(dictionaryDoneLabel);
                                    emit.MarkLabel(dictionaryNullLabel);
                                    if (emit.TryNewObject(toMemberType, Type.EmptyTypes) == false)
                                    {
                                        emit.Branch(dictionaryContinueLabel);
                                    }
                                    else
                                    {
                                        emit.StoreLocal(subToLocal);
                                    }
                                    emit.Branch(dictionaryDoneLabel);
                                    emit.MarkLabel(dictionaryContinueLabel);
                                    emit.Nop();
                                    emit.MarkLabel(dictionaryDoneLabel);
                                    emit.LoadLocal(subToLocal);

                                    /*局部变量*/
                                    if (fromType.IsValueType)
                                        emit.LoadLocalAddress(fromLocal); // from
                                    else
                                        emit.LoadLocal(fromLocal);

                                    if (fromMember.MemberType == MemberTypes.Property)
                                        emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                                    else
                                        emit.LoadField((FieldInfo)fromMember);

                                    if (toMemberType.IsAssignableFromType(typeof(ICollection<>)))
                                    {
                                        emit.LoadArgument(1);
                                        emit.Call(typeof(MapperBuilderHelper).GetMethod("KeyValuePairLoadIntoCollection").MakeGenericMethod(dictionaryTypes));
                                    }
                                    else
                                    {
                                        emit.LoadArgument(1);
                                        emit.Call(typeof(MapperBuilderHelper).GetMethod("KeyValuePairLoadIntoDictionary").MakeGenericMethod(dictionaryTypes));
                                    }

                                    /*局部变量*/
                                    if (toType.IsValueType)
                                        emit.LoadLocalAddress(toLocal); // from
                                    else
                                        emit.LoadLocal(toLocal);

                                    emit.LoadLocal(subToLocal);
                                    if (toMember.MemberType == MemberTypes.Property)
                                        emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                                    else
                                        emit.StoreField((FieldInfo)toMember);

                                    emit.Nop();
                                    continue;
                                }
                                else if (toMemberType.IsValueType)
                                {
                                    if (toType.IsValueType)
                                        emit.LoadLocalAddress(toLocal); //to
                                    else
                                        emit.LoadLocal(toLocal);

                                    if (toMember.MemberType == MemberTypes.Property)
                                        emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                                    else
                                        emit.LoadField((FieldInfo)toMember);

                                    emit.StoreLocal(subToLocal);
                                    emit.LoadLocal(subToLocal);
                                    emit.Box(toMemberType);
                                    /*局部变量*/
                                    if (fromType.IsValueType)
                                        emit.LoadLocalAddress(fromLocal); // from
                                    else
                                        emit.LoadLocal(fromLocal);

                                    if (fromMember.MemberType == MemberTypes.Property)
                                        emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                                    else
                                        emit.LoadField((FieldInfo)fromMember);

                                    if (toMemberType.IsAssignableFromType(typeof(ICollection<>)))
                                    {
                                        emit.LoadArgument(1);
                                        emit.Call(typeof(MapperBuilderHelper).GetMethod("KeyValuePairLoadIntoCollection").MakeGenericMethod(dictionaryTypes));
                                    }
                                    else
                                    {
                                        emit.LoadArgument(1);
                                        emit.Call(typeof(MapperBuilderHelper).GetMethod("KeyValuePairLoadIntoDictionary").MakeGenericMethod(dictionaryTypes));
                                    }

                                    /*局部变量*/
                                    if (toType.IsValueType)
                                        emit.LoadLocalAddress(toLocal); // from
                                    else
                                        emit.LoadLocal(toLocal);

                                    emit.LoadLocal(subToLocal);
                                    if (toMember.MemberType == MemberTypes.Property)
                                        emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                                    else
                                        emit.StoreField((FieldInfo)toMember);

                                    emit.Nop();
                                    continue;
                                }
                                else
                                {
                                    if (toType.IsValueType)
                                        emit.LoadLocalAddress(toLocal); //to
                                    else
                                        emit.LoadLocal(toLocal);

                                    if (toMember.MemberType == MemberTypes.Property)
                                        emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                                    else
                                        emit.LoadField((FieldInfo)toMember);

                                    var dictionaryNullLabel = emit.DefineLabel();
                                    var dictionaryDoneLabel = emit.DefineLabel();
                                    var dictionaryContinueLabel = emit.DefineLabel();
                                    emit.LoadNull();
                                    emit.CompareGreaterThan();
                                    emit.BranchIfFalse(dictionaryNullLabel);
                                    if (toType.IsValueType)
                                        emit.LoadLocalAddress(toLocal); //to
                                    else
                                        emit.LoadLocal(toLocal);

                                    if (toMember.MemberType == MemberTypes.Property)
                                        emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                                    else
                                        emit.LoadField((FieldInfo)toMember);
                                    emit.StoreLocal(subToLocal);
                                    emit.Branch(dictionaryDoneLabel);
                                    emit.MarkLabel(dictionaryNullLabel);
                                    emit.NewObject(typeof(Dictionary<,>).MakeGenericType(dictionaryTypes[0], dictionaryTypes[1]), Type.EmptyTypes);
                                    emit.StoreLocal(subToLocal);
                                    emit.Branch(dictionaryDoneLabel);
                                    emit.MarkLabel(dictionaryContinueLabel);
                                    emit.Nop();
                                    emit.MarkLabel(dictionaryDoneLabel);
                                    emit.LoadLocal(subToLocal);
                                    /*局部变量*/
                                    if (fromType.IsValueType)
                                        emit.LoadLocalAddress(fromLocal); // from
                                    else
                                        emit.LoadLocal(fromLocal);

                                    if (fromMember.MemberType == MemberTypes.Property)
                                        emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                                    else
                                        emit.LoadField((FieldInfo)fromMember);

                                    emit.LoadArgument(1);
                                    emit.Call(typeof(MapperBuilderHelper).GetMethod("KeyValuePairLoadIntoCollection").MakeGenericMethod(dictionaryTypes));

                                    /*局部变量*/
                                    if (toType.IsValueType)
                                        emit.LoadLocalAddress(toLocal); // from
                                    else
                                        emit.LoadLocal(toLocal);

                                    emit.LoadLocal(subToLocal);
                                    if (toMember.MemberType == MemberTypes.Property)
                                        emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                                    else
                                        emit.StoreField((FieldInfo)toMember);

                                    emit.Nop();
                                    continue;
                                }
                            }
                        }

                        //目标是数组
                        var enumerableTypes = new Type[2];
                        Array.Copy(this.FindInterfaceOrGenericInterface(toMemberType, typeof(IEnumerable<>)).GetGenericArguments(), 0, enumerableTypes, 0, 1);
                        Array.Copy(this.FindInterfaceOrGenericInterface(fromMemberType, typeof(IEnumerable<>)).GetGenericArguments(), 0, enumerableTypes, 1, 1);
                        if (toMemberType.IsArray)
                        {
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);

                            var dictionaryNullLabel = emit.DefineLabel();
                            var dictionaryDoneLabel = emit.DefineLabel();
                            var dictionaryContinueLabel = emit.DefineLabel();
                            emit.LoadNull();
                            emit.CompareGreaterThan();
                            emit.BranchIfFalse(dictionaryNullLabel);
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryNullLabel);
                            /*局部变量*/
                            if (fromType.IsValueType)
                                emit.LoadLocalAddress(fromLocal); // from
                            else
                                emit.LoadLocal(fromLocal);

                            if (fromMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)fromMember);

                            emit.LoadArgument(1);
                            emit.Call(typeof(MapperBuilderHelper).GetMethod("MakeSureEnumerableCount").MakeGenericMethod(enumerableTypes[1]));
                            emit.NewArray(enumerableTypes[0]);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryContinueLabel);
                            emit.Nop();
                            emit.MarkLabel(dictionaryDoneLabel);
                            emit.LoadLocal(subToLocal);
                            /*局部变量*/
                            if (fromType.IsValueType)
                                emit.LoadLocalAddress(fromLocal); // from
                            else
                                emit.LoadLocal(fromLocal);

                            if (fromMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)fromMember);

                            emit.LoadArgument(1);
                            emit.Call(typeof(MapperBuilderHelper).GetMethod("LoadIntoArray").MakeGenericMethod(enumerableTypes));

                            /*局部变量*/
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); // from
                            else
                                emit.LoadLocal(toLocal);

                            emit.LoadLocal(subToLocal);
                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                            else
                                emit.StoreField((FieldInfo)toMember);

                            emit.Nop();
                            continue;
                        }
                        else if (toMemberType.IsClass)
                        {
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);

                            var dictionaryNullLabel = emit.DefineLabel();
                            var dictionaryDoneLabel = emit.DefineLabel();
                            var dictionaryContinueLabel = emit.DefineLabel();
                            emit.LoadNull();
                            emit.CompareGreaterThan();
                            emit.BranchIfFalse(dictionaryNullLabel);
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryNullLabel);
                            if (emit.TryNewObject(toMemberType, Type.EmptyTypes) == false)
                            {
                                emit.Branch(dictionaryContinueLabel);
                            }
                            else
                            {
                                emit.StoreLocal(subToLocal);
                            }
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryContinueLabel);
                            emit.Nop();
                            emit.MarkLabel(dictionaryDoneLabel);
                            emit.LoadLocal(subToLocal);
                            /*局部变量*/
                            if (fromType.IsValueType)
                                emit.LoadLocalAddress(fromLocal); // from
                            else
                                emit.LoadLocal(fromLocal);

                            if (fromMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)fromMember);

                            if (toMemberType.IsAssignableFromType(typeof(IList<>)))
                            {
                                emit.LoadArgument(1);
                                emit.Call(typeof(MapperBuilderHelper).GetMethod("LoadIntoList").MakeGenericMethod(enumerableTypes));
                                /*局部变量*/
                                if (toType.IsValueType)
                                    emit.LoadLocalAddress(toLocal); // from
                                else
                                    emit.LoadLocal(toLocal);

                                emit.LoadLocal(subToLocal);
                                if (toMember.MemberType == MemberTypes.Property)
                                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                                else
                                    emit.StoreField((FieldInfo)toMember);
                            }
                            else if (toMemberType.IsAssignableFromType(typeof(ICollection<>)))
                            {
                                emit.LoadArgument(1);
                                emit.Call(typeof(MapperBuilderHelper).GetMethod("LoadIntoCollection").MakeGenericMethod(enumerableTypes));
                                /*局部变量*/
                                if (toType.IsValueType)
                                    emit.LoadLocalAddress(toLocal); // from
                                else
                                    emit.LoadLocal(toLocal);

                                emit.LoadLocal(subToLocal);
                                if (toMember.MemberType == MemberTypes.Property)
                                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                                else
                                    emit.StoreField((FieldInfo)toMember);
                            }

                            emit.Nop();
                            continue;
                        }
                        else if (toMemberType.IsValueType)
                        {
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);

                            emit.StoreLocal(subToLocal);
                            emit.LoadLocal(subToLocal);
                            emit.Box(toMemberType);
                            /*局部变量*/
                            if (fromType.IsValueType)
                                emit.LoadLocalAddress(fromLocal); // from
                            else
                                emit.LoadLocal(fromLocal);

                            if (fromMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)fromMember);

                            if (toMemberType.IsAssignableFromType(typeof(IList<>)))
                            {
                                emit.LoadArgument(1);
                                emit.Call(typeof(MapperBuilderHelper).GetMethod("LoadIntoList").MakeGenericMethod(enumerableTypes));

                                /*局部变量*/
                                if (toType.IsValueType)
                                    emit.LoadLocalAddress(toLocal); // from
                                else
                                    emit.LoadLocal(toLocal);

                                emit.LoadLocal(subToLocal);
                                if (toMember.MemberType == MemberTypes.Property)
                                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                                else
                                    emit.StoreField((FieldInfo)toMember);
                            }
                            else if (toMemberType.IsAssignableFromType(typeof(ICollection<>)))
                            {
                                emit.LoadArgument(1);
                                emit.Call(typeof(MapperBuilderHelper).GetMethod("LoadIntoCollection").MakeGenericMethod(enumerableTypes));

                                /*局部变量*/
                                if (toType.IsValueType)
                                    emit.LoadLocalAddress(toLocal); // from
                                else
                                    emit.LoadLocal(toLocal);

                                emit.LoadLocal(subToLocal);
                                if (toMember.MemberType == MemberTypes.Property)
                                    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                                else
                                    emit.StoreField((FieldInfo)toMember);
                            }

                            emit.Nop();
                            continue;
                        }
                        else if (toMemberType.IsAssignableFromType(typeof(IList<>)))
                        {
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);

                            var dictionaryNullLabel = emit.DefineLabel();
                            var dictionaryDoneLabel = emit.DefineLabel();
                            var dictionaryContinueLabel = emit.DefineLabel();
                            emit.LoadNull();
                            emit.CompareGreaterThan();
                            emit.BranchIfFalse(dictionaryNullLabel);
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryNullLabel);
                            emit.NewObject(typeof(List<>).MakeGenericType(enumerableTypes[0]), Type.EmptyTypes);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryContinueLabel);
                            emit.Nop();
                            emit.MarkLabel(dictionaryDoneLabel);
                            emit.LoadLocal(subToLocal);
                            /*局部变量*/
                            if (fromType.IsValueType)
                                emit.LoadLocalAddress(fromLocal); // from
                            else
                                emit.LoadLocal(fromLocal);

                            if (fromMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)fromMember);

                            emit.LoadArgument(1);
                            emit.Call(typeof(MapperBuilderHelper).GetMethod("LoadIntoList").MakeGenericMethod(enumerableTypes));

                            /*局部变量*/
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); // from
                            else
                                emit.LoadLocal(toLocal);

                            emit.LoadLocal(subToLocal);
                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                            else
                                emit.StoreField((FieldInfo)toMember);

                            emit.Nop();
                            continue;
                        }
                        else if (toMemberType.IsAssignableFromType(typeof(ICollection<>)))
                        {
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);

                            var dictionaryNullLabel = emit.DefineLabel();
                            var dictionaryDoneLabel = emit.DefineLabel();
                            var dictionaryContinueLabel = emit.DefineLabel();
                            emit.LoadNull();
                            emit.CompareGreaterThan();
                            emit.BranchIfFalse(dictionaryNullLabel);
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryNullLabel);
                            emit.NewObject(typeof(List<>).MakeGenericType(enumerableTypes[0]), Type.EmptyTypes);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryContinueLabel);
                            emit.Nop();
                            emit.MarkLabel(dictionaryDoneLabel);
                            emit.LoadLocal(subToLocal);
                            /*局部变量*/
                            if (fromType.IsValueType)
                                emit.LoadLocalAddress(fromLocal); // from
                            else
                                emit.LoadLocal(fromLocal);

                            if (fromMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)fromMember);

                            emit.LoadArgument(1);
                            emit.Call(typeof(MapperBuilderHelper).GetMethod("LoadIntoCollection").MakeGenericMethod(enumerableTypes));

                            /*局部变量*/
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); // from
                            else
                                emit.LoadLocal(toLocal);

                            emit.LoadLocal(subToLocal);
                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                            else
                                emit.StoreField((FieldInfo)toMember);

                            emit.Nop();
                            continue;
                        }
                        else
                        {
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);

                            var dictionaryNullLabel = emit.DefineLabel();
                            var dictionaryDoneLabel = emit.DefineLabel();
                            var dictionaryContinueLabel = emit.DefineLabel();
                            emit.LoadNull();
                            emit.CompareGreaterThan();
                            emit.BranchIfFalse(dictionaryNullLabel);
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryNullLabel);
                            /*局部变量*/
                            if (fromType.IsValueType)
                                emit.LoadLocalAddress(fromLocal); // from
                            else
                                emit.LoadLocal(fromLocal);

                            if (fromMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)fromMember);

                            emit.LoadArgument(1);
                            emit.Call(typeof(MapperBuilderHelper).GetMethod("MakeSureEnumerableCount").MakeGenericMethod(enumerableTypes[1]));
                            emit.NewArray(enumerableTypes[0]);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryContinueLabel);
                            emit.Nop();
                            emit.MarkLabel(dictionaryDoneLabel);
                            emit.LoadLocal(subToLocal);
                            /*局部变量*/
                            if (fromType.IsValueType)
                                emit.LoadLocalAddress(fromLocal); // from
                            else
                                emit.LoadLocal(fromLocal);

                            if (fromMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)fromMember);

                            emit.LoadArgument(1);
                            emit.Call(typeof(MapperBuilderHelper).GetMethod("LoadIntoArray").MakeGenericMethod(enumerableTypes));

                            /*局部变量*/
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); // from
                            else
                                emit.LoadLocal(toLocal);

                            emit.LoadLocal(subToLocal);
                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                            else
                                emit.StoreField((FieldInfo)toMember);

                            emit.Nop();
                            continue;

                            //if (toType.IsValueType)
                            //    emit.LoadLocalAddress(toLocal); //to
                            //else
                            //    emit.LoadLocal(toLocal);

                            //if (toMember.MemberType == MemberTypes.Property)
                            //    emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            //else
                            //    emit.LoadField((FieldInfo)toMember);

                            //var dictionaryNullLabel = emit.DefineLabel();
                            //var dictionaryDoneLabel = emit.DefineLabel();
                            //var dictionaryContinueLabel = emit.DefineLabel();
                            //emit.LoadNull();
                            //emit.CompareGreaterThan();
                            //emit.BranchIfFalse(dictionaryNullLabel);
                            //if (toType.IsValueType)
                            //    emit.LoadLocalAddress(toLocal); //to
                            //else
                            //    emit.LoadLocal(toLocal);

                            //if (toMember.MemberType == MemberTypes.Property)
                            //    emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            //else
                            //    emit.LoadField((FieldInfo)toMember);
                            //emit.StoreLocal(subToLocal);
                            //emit.Branch(dictionaryDoneLabel);
                            //emit.MarkLabel(dictionaryNullLabel);
                            //emit.Call(typeof(System.Linq.Enumerable).GetMethod("Empty").MakeGenericMethod(enumerableTypes[0]));
                            //emit.StoreLocal(subToLocal);
                            //emit.Branch(dictionaryDoneLabel);
                            //emit.MarkLabel(dictionaryContinueLabel);
                            //emit.Nop();
                            //emit.MarkLabel(dictionaryDoneLabel);
                            //emit.LoadLocal(subToLocal);
                            ///*局部变量*/
                            //if (fromType.IsValueType)
                            //    emit.LoadLocalAddress(fromLocal); // from
                            //else
                            //    emit.LoadLocal(fromLocal);

                            //if (fromMember.MemberType == MemberTypes.Property)
                            //    emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                            //else
                            //    emit.LoadField((FieldInfo)fromMember);

                            //emit.LoadArgument(1);
                            //emit.Call(typeof(MapperBuilderHelper).GetMethod("LoadIntoEnumerable").MakeGenericMethod(enumerableTypes));

                            ///*局部变量*/
                            //if (toType.IsValueType)
                            //    emit.LoadLocalAddress(toLocal); // from
                            //else
                            //    emit.LoadLocal(toLocal);

                            //emit.LoadLocal(subToLocal);
                            //if (toMember.MemberType == MemberTypes.Property)
                            //    emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                            //else
                            //    emit.StoreField((FieldInfo)toMember);

                            //emit.Nop();
                            //continue;
                        }
                    }
                    else
                    {
                        notMatchs.Insert(0, true);
                    }

                    //没有匹配
                    if (notMatchs.Any(tt => tt))
                    {
                        if (setting.AlwaysNewTraget == false)
                            continue;

                        if (toMemberType.IsArray)
                        {
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);

                            var dictionaryNullLabel = emit.DefineLabel();
                            var dictionaryDoneLabel = emit.DefineLabel();
                            var dictionaryContinueLabel = emit.DefineLabel();
                            emit.LoadNull();
                            emit.CompareGreaterThan();
                            emit.BranchIfFalse(dictionaryNullLabel);
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryNullLabel);
                            /*局部变量*/
                            if (fromType.IsValueType)
                                emit.LoadLocalAddress(fromLocal); // from
                            else
                                emit.LoadLocal(fromLocal);

                            if (fromMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)fromMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)fromMember);

                            emit.LoadArgument(1);
                            emit.Call(typeof(MapperBuilderHelper).GetMethod("MakeSureEnumerableCount").MakeGenericMethod(toMemberType.GetElementType()));
                            emit.NewArray(toMemberType.GetElementType());
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryContinueLabel);
                            emit.Nop();
                            emit.MarkLabel(dictionaryDoneLabel);
                            /*局部变量*/
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); // from
                            else
                                emit.LoadLocal(toLocal);

                            emit.LoadLocal(subToLocal);
                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                            else
                                emit.StoreField((FieldInfo)toMember);
                        }
                        else if (toMemberType.IsClass)
                        {
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);

                            var dictionaryNullLabel = emit.DefineLabel();
                            var dictionaryDoneLabel = emit.DefineLabel();
                            var dictionaryContinueLabel = emit.DefineLabel();
                            emit.LoadNull();
                            emit.CompareGreaterThan();
                            emit.BranchIfFalse(dictionaryNullLabel);
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryNullLabel);
                            if (emit.TryNewObject(toMemberType, Type.EmptyTypes) == false)
                            {
                                emit.Branch(dictionaryContinueLabel);
                            }
                            else
                            {
                                emit.StoreLocal(subToLocal);
                            }
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryContinueLabel);
                            emit.Nop();
                            emit.MarkLabel(dictionaryDoneLabel);

                            /*局部变量*/
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); // from
                            else
                                emit.LoadLocal(toLocal);

                            emit.LoadLocal(subToLocal);
                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                            else
                                emit.StoreField((FieldInfo)toMember);
                        }
                        else if (toMemberType.IsValueType)
                        {
                            emit.Nop();
                        }
                        else if (toMemberType.IsAssignableFromType(typeof(IDictionary<,>)))
                        {
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);

                            var dictionaryNullLabel = emit.DefineLabel();
                            var dictionaryDoneLabel = emit.DefineLabel();
                            var dictionaryContinueLabel = emit.DefineLabel();
                            emit.LoadNull();
                            emit.CompareGreaterThan();
                            emit.BranchIfFalse(dictionaryNullLabel);
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryNullLabel);
                            var tokeyvaluePairTypeType = this.FindIEnumerableKeyValuePairGenericType(toMemberType);
                            emit.NewObject(typeof(Dictionary<,>).MakeGenericType(tokeyvaluePairTypeType.GetGenericArguments()[0].GetGenericArguments()), Type.EmptyTypes);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryContinueLabel);
                            emit.Nop();
                            emit.MarkLabel(dictionaryDoneLabel);
                            /*局部变量*/
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); // from
                            else
                                emit.LoadLocal(toLocal);

                            emit.LoadLocal(subToLocal);
                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                            else
                                emit.StoreField((FieldInfo)toMember);
                        }
                        else if (toMemberType.IsAssignableFromType(typeof(ICollection<>)) && this.FindIEnumerableKeyValuePairGenericType(toMemberType) != null && toMemberType.IsAssignableFromType(typeof(ICollection<>).MakeGenericType(this.FindIEnumerableKeyValuePairGenericType(toMemberType).GetGenericArguments())))
                        {
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);

                            var dictionaryNullLabel = emit.DefineLabel();
                            var dictionaryDoneLabel = emit.DefineLabel();
                            var dictionaryContinueLabel = emit.DefineLabel();
                            emit.LoadNull();
                            emit.CompareGreaterThan();
                            emit.BranchIfFalse(dictionaryNullLabel);
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryNullLabel);
                            var tokeyvaluePairTypeType = this.FindIEnumerableKeyValuePairGenericType(toMemberType);
                            emit.NewObject(typeof(Dictionary<,>).MakeGenericType(tokeyvaluePairTypeType.GetGenericArguments()[0].GetGenericArguments()), Type.EmptyTypes);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryContinueLabel);
                            emit.Nop();
                            emit.MarkLabel(dictionaryDoneLabel);
                            /*局部变量*/
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); // from
                            else
                                emit.LoadLocal(toLocal);

                            emit.LoadLocal(subToLocal);
                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                            else
                                emit.StoreField((FieldInfo)toMember);
                        }
                        else if (toMemberType.IsAssignableFromType(typeof(IList<>)))
                        {
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);

                            var dictionaryNullLabel = emit.DefineLabel();
                            var dictionaryDoneLabel = emit.DefineLabel();
                            var dictionaryContinueLabel = emit.DefineLabel();
                            emit.LoadNull();
                            emit.CompareGreaterThan();
                            emit.BranchIfFalse(dictionaryNullLabel);
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryNullLabel);
                            emit.Call(typeof(System.Linq.Enumerable).GetMethod("Empty").MakeGenericMethod(toMemberType.GetGenericArguments()));
                            emit.Call(typeof(System.Linq.Enumerable).GetMethod("ToList").MakeGenericMethod(toMemberType.GetGenericArguments()));
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryContinueLabel);
                            emit.Nop();
                            emit.MarkLabel(dictionaryDoneLabel);
                            /*局部变量*/
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); // from
                            else
                                emit.LoadLocal(toLocal);

                            emit.LoadLocal(subToLocal);
                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                            else
                                emit.StoreField((FieldInfo)toMember);
                        }
                        else if (toMemberType.IsAssignableFromType(typeof(ICollection<>)))
                        {
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);

                            var dictionaryNullLabel = emit.DefineLabel();
                            var dictionaryDoneLabel = emit.DefineLabel();
                            var dictionaryContinueLabel = emit.DefineLabel();
                            emit.LoadNull();
                            emit.CompareGreaterThan();
                            emit.BranchIfFalse(dictionaryNullLabel);
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryNullLabel);
                            emit.Call(typeof(System.Linq.Enumerable).GetMethod("Empty").MakeGenericMethod(toMemberType.GetGenericArguments()));
                            emit.Call(typeof(System.Linq.Enumerable).GetMethod("ToList").MakeGenericMethod(toMemberType.GetGenericArguments()));
                            emit.StoreLocal(subToLocal);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryContinueLabel);
                            emit.Nop();
                            emit.MarkLabel(dictionaryDoneLabel);
                            /*局部变量*/
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); // from
                            else
                                emit.LoadLocal(toLocal);

                            emit.LoadLocal(subToLocal);
                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                            else
                                emit.StoreField((FieldInfo)toMember);
                        }
                        else
                        {
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);

                            var dictionaryNullLabel = emit.DefineLabel();
                            var dictionaryDoneLabel = emit.DefineLabel();
                            var dictionaryContinueLabel = emit.DefineLabel();
                            emit.LoadNull();
                            emit.CompareGreaterThan();
                            emit.BranchIfFalse(dictionaryNullLabel);
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); //to
                            else
                                emit.LoadLocal(toLocal);

                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                            else
                                emit.LoadField((FieldInfo)toMember);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryNullLabel);
                            emit.Call(typeof(System.Linq.Enumerable).GetMethod("Empty").MakeGenericMethod(toMemberType.GetGenericArguments()));
                            emit.StoreLocal(subToLocal);
                            emit.StoreLocal(subToLocal);
                            emit.StoreLocal(subToLocal);
                            emit.Branch(dictionaryDoneLabel);
                            emit.MarkLabel(dictionaryContinueLabel);
                            emit.Nop();
                            emit.MarkLabel(dictionaryDoneLabel);
                            /*局部变量*/
                            if (toType.IsValueType)
                                emit.LoadLocalAddress(toLocal); // from
                            else
                                emit.LoadLocal(toLocal);

                            emit.LoadLocal(subToLocal);
                            if (toMember.MemberType == MemberTypes.Property)
                                emit.Call(((PropertyInfo)toMember).GetSetMethod(true)); // from get method
                            else
                                emit.StoreField((FieldInfo)toMember);
                        }
                    }

                    emit.Nop();
                    continue;
                }

                if (toMemberType.IsAbstract || toMemberType.IsInterface || toMemberType.IsImport)
                    continue;

                if (!toMemberType.IsValueType && toMemberType.GetConstructor(Type.EmptyTypes) == null)
                    continue;

                /*局部变量*/
                var subToMembers = GetMembers(toMemberType);
                var subFromMembers = GetMembers(fromMemberType);
                /*复合类型*/
                var subComplexType = subToMembers.FindAll(s => IsComplexType(s));

                //值对象在自己构造的时候是new了的，所以只能重新构造并且赋值
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
                    ActionBuildNormalMembers(setting, subToLocal, subFromLocal, emit, toMemberType, subToMembers, fromMemberType, subFromMembers, level + 1);

                    if (subComplexType != null && subComplexType.Count > 0)
                        ActionBuildComplexTypeMembers(setting, subToLocal, subFromLocal, emit, toMemberType, subComplexType, fromMemberType, subFromMembers.FindAll(s => IsComplexType(s)), level + 1);

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

                ILabel nullLabel = emit.DefineLabel();
                ILabel doneLabel = emit.DefineLabel();
                if (toType.IsValueType)
                    emit.LoadLocalAddress(toLocal); //to
                else
                    emit.LoadLocal(toLocal);

                if (toMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                else
                    emit.LoadField((FieldInfo)toMember);

                emit.LoadNull();
                emit.CompareGreaterThan();
                emit.BranchIfFalse(nullLabel);
                /*not null*/
                if (toType.IsValueType)
                    emit.LoadLocalAddress(toLocal); //to
                else
                    emit.LoadLocal(toLocal);

                if (toMember.MemberType == MemberTypes.Property)
                    emit.Call(((PropertyInfo)toMember).GetGetMethod()); // from get method
                else
                    emit.LoadField((FieldInfo)toMember);
                emit.StoreLocal(subToLocal);
                emit.Branch(doneLabel);
                /*null*/
                emit.MarkLabel(nullLabel);
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
                emit.Branch(doneLabel);

                emit.MarkLabel(doneLabel);
                emit.Nop();

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
                ActionBuildNormalMembers(setting, subToLocal, subFromLocal, emit, toMemberType, subToMembers, fromMemberType, subFromMembers, level + 1);

                if (subComplexType != null && subComplexType.Count > 0)
                    ActionBuildComplexTypeMembers(setting, subToLocal, subFromLocal, emit, toMemberType, subComplexType, fromMemberType, subFromMembers.FindAll(s => IsComplexType(s)), level + 1);

                continue;
            }

            if (breakLbl != null)
            {
                emit.MarkLabel(breakLbl);
                emit.Nop();
            }
        }

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
        protected void ActionBuildTargetNullableSourceNotNullable<DelegateType>(MapperSetting setting, ILocal toLocal, ILocal fromLocal, EasyEmitBuilder<DelegateType> emit, Type toType, MemberInfo toMember, Type innerToMemberType, Type fromType, MemberInfo fromMember)
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
        protected void ActionBuildTargetNullableSourceNullable<DelegateType>(MapperSetting setting, ILocal toLocal, ILocal fromLocal, EasyEmitBuilder<DelegateType> emit, Type toType, MemberInfo toMember, Type innerToMemberType, Type fromType, MemberInfo fromMember, Type innerFromMemberType)
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
        protected void ActionBuildTargetNotNullableSourceNotNullable<DelegateType>(MapperSetting setting, ILocal toLocal, ILocal fromLocal, EasyEmitBuilder<DelegateType> emit, Type toType, MemberInfo toMember, Type fromType, MemberInfo fromMember)
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
        protected void ActionBuildTargetNotNullableSourceNullable<DelegateType>(MapperSetting setting, ILocal toLocal, ILocal fromLocal, EasyEmitBuilder<DelegateType> emit, Type toType, MemberInfo toMember, Type fromType, MemberInfo fromMember, Type innerFromMemberType)
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
            return;
        }

        #endregion memberInfo
    }
}