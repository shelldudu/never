using Never.IoC.Injections.Rules;
using Never.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Never.IoC.Injections
{
    /// <summary>
    /// 规则构建者
    /// </summary>
    public class RegisterRuleBuilder
    {
        #region lock

        private readonly static object locker = new object();

        #endregion lock

        #region ctor para

        /// <summary>
        /// 构造函数的信息
        /// </summary>
        private struct CtorParameter
        {
            public ConstructorInfo Constructor { get; set; }
            public ParameterInfo[] Parameters { get; set; }
        }

        #endregion ctor para

        #region build

        /// <summary>
        /// 构建行为
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="collection">规则集合</param>
        /// <param name="container">容器</param>
        public static Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object> Build(RegisterRule rule, IRegisterRuleQuery collection, IRegisterRuleChangeable container)
        {
            if (rule == null)
            {
                return null;
            }

            /*已经构建过的*/
            if (rule.Builder != null)
            {
                return rule.Builder;
            }

            lock (locker)
            {
                /*已经构建过的*/
                if (rule.Builder != null)
                {
                    return rule.Builder;
                }

                rule.Builder = BuildDelegate(rule, collection, container, new List<RegisterRule>() { }, 0);
                if (rule.OptionalBuilder == null && rule.Builder != null)
                {
                    rule.OptionalBuilder = rule.Builder;
                }

                return rule.Builder;
            }
        }

        /// <summary>
        /// 构建行为
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="collection">规则集合</param>
        /// <param name="container">容器</param>
        public static Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object> OptionalBuild(RegisterRule rule, IRegisterRuleQuery collection, IRegisterRuleChangeable container)
        {
            if (rule == null)
            {
                return null;
            }

            /*已经构建过的*/
            if (rule.Builder != null)
            {
                return rule.Builder;
            }

            /*已经构建过的*/
            if (rule.OptionalBuilder != null)
            {
                return rule.OptionalBuilder;
            }

            lock (locker)
            {
                /*已经构建过的*/
                if (rule.Builder != null)
                {
                    return rule.Builder;
                }

                /*已经构建过的*/
                if (rule.OptionalBuilder != null)
                {
                    return rule.OptionalBuilder;
                }

                return rule.OptionalBuilder = OptionalBuildDelegate(rule, collection, container, new List<RegisterRule>() { }, 0);
            }
        }

        /// <summary>
        /// 往上规则匹配
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parent"></param>
        private static void RuleMatchUsingNegativeSort(RegisterRule target, RegisterRule parent)
        {
            var message = parent.Compatible(target);
            if (message.IsNotNullOrEmpty())
            {
                throw new ArgumentException(message);
            }

            return;

            //switch (current.LifeStyle)
            //{
            //    /*单例可以注入到任何实例中，其构造只能是单例对象*/
            //    case ComponentLifeStyle.Singleton:
            //        {
            //        }
            //        break;
            //    /*短暂只能注入到短暂，其构造可接受任何实例对象*/
            //    case ComponentLifeStyle.Transient:
            //        {
            //            if (preRule.LifeStyle != ComponentLifeStyle.Transient)
            //                throw new ArgumentException(string.Format("构建当前对象{0}为单例，期望对象{1}为{2}，不能相容",
            //                    preRule.ServiceType.FullName,
            //                    current.ServiceType.FullName,
            //                    current.LifeStyle == ComponentLifeStyle.Scoped ? "线程" : "短暂"));
            //        }
            //        break;
            //    /*作用域只能注入到嵌套，其构造不能接受短暂，可接受有嵌套和单例*/
            //    case ComponentLifeStyle.Scoped:
            //        {
            //            if (preRule.LifeStyle == ComponentLifeStyle.Singleton)
            //                throw new ArgumentException(string.Format("构建当前对象{0}为线程，期望对象{1}为短暂，不能相容",
            //                    preRule.ServiceType.FullName,
            //                    current.ServiceType.FullName));
            //        }
            //        break;
            //}
        }

        /// <summary>
        /// 往下规则匹配
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="target"></param>
        private static void RuleMatchUsingPositivedSort(RegisterRule parent, RegisterRule target)
        {
            var message = parent.Compatible(target);
            if (message.IsNotNullOrEmpty())
            {
                throw new ArgumentException(message);
            }

            return;

            //switch (nextRule.LifeStyle)
            //{
            //    /*单例可以注入到任何实例中，其构造只能是单例对象*/
            //    case ComponentLifeStyle.Singleton:
            //        {
            //        }
            //        break;
            //    /*短暂只能注入到短暂，其构造可接受任何实例对象*/
            //    case ComponentLifeStyle.Transient:
            //        {
            //            if (current.LifeStyle != ComponentLifeStyle.Transient)
            //                throw new ArgumentException(string.Format("当前参数{0}为短暂，期望对象{1}为短暂，不能相容",
            //                    nextRule.ServiceType.FullName,
            //                    current.ServiceType.FullName));
            //        }
            //        break;
            //    /*作用域只能注入到嵌套，其构造不能接受短暂，可接受有嵌套和单例*/
            //    case ComponentLifeStyle.Scoped:
            //        {
            //            if (current.LifeStyle == ComponentLifeStyle.Singleton)
            //                throw new ArgumentException(string.Format("当前参数{0}为作用域，期望对象{1}为单例，不能相容",
            //                    nextRule.ServiceType.FullName,
            //                    current.ServiceType.FullName));
            //        }
            //        break;
            //}
        }

        /// <summary>
        /// 没有构造函数的值对象
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        private static Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object> BuildValueTypeNoCtorParameterDelegate(RegisterRule rule)
        {
            var opemit = EasyEmitBuilder<Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>>.NewDynamicMethod();

            /*labels*/
            var objectResult = opemit.DefineLabel();
            var objectNotNull = opemit.DefineLabel();

            /*locals*/
            var objectLocal = opemit.DeclareLocal(typeof(object));
            var boolLocal = opemit.DeclareLocal(typeof(bool));
            var structlocalAddr = opemit.DeclareLocal(rule.ImplementationType);

            /*find in cached and branch*/
            opemit.LoadArgument(0);
            opemit.LoadArgument(1);
            opemit.LoadArgument(2);
            opemit.LoadArgument(3);
            opemit.Call(typeof(RegisterRuleBuilder).GetMethod("QueryInCache", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(rule.ServiceType));
            opemit.StoreLocal(objectLocal);
            opemit.LoadLocal(objectLocal);
            opemit.LoadNull();
            opemit.CompareEqual();
            opemit.StoreLocal(boolLocal);
            opemit.LoadLocal(boolLocal);
            /*如果没有在缓存中找到，则比较器结果为1，不跳转，否则直接跳转到结果标签*/
            opemit.BranchIfFalse(objectResult);

            /*new struct*/
            opemit.MarkLabel(objectNotNull);
            opemit.LoadLocalAddress(structlocalAddr);
            opemit.InitializeObject(rule.ImplementationType);
            opemit.LoadLocal(structlocalAddr);
            opemit.Box(rule.ImplementationType);
            opemit.StoreLocal(objectLocal);
            opemit.Branch(objectResult);

            /*result*/
            opemit.MarkLabel(objectResult);
            opemit.LoadArgument(0);
            opemit.LoadArgument(1);
            opemit.LoadArgument(2);
            opemit.LoadArgument(3);
            opemit.LoadLocal(objectLocal);
            opemit.Call(typeof(RegisterRuleBuilder).GetMethod("InsertIntoCache", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(rule.ServiceType));
            opemit.Return();

            return opemit.CreateDelegate();
        }

        /// <summary>
        /// 没有构造函数的引用对象
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="ctor"></param>
        /// <returns></returns>
        private static Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object> BuildRefenerceTypeNoCtorParameterDelegate(RegisterRule rule, ConstructorInfo ctor)
        {
            var opemit = EasyEmitBuilder<Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>>.NewDynamicMethod();

            /*labels*/
            var objectResult = opemit.DefineLabel();
            var objectNotNull = opemit.DefineLabel();

            /*locals*/
            var objectLocal = opemit.DeclareLocal(typeof(object));
            var boolLocal = opemit.DeclareLocal(typeof(bool));

            /*find in cached and branch*/
            opemit.LoadArgument(0);
            opemit.LoadArgument(1);
            opemit.LoadArgument(2);
            opemit.LoadArgument(3);
            opemit.Call(typeof(RegisterRuleBuilder).GetMethod("QueryInCache", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(rule.ServiceType));
            opemit.StoreLocal(objectLocal);
            opemit.LoadLocal(objectLocal);
            opemit.LoadNull();
            opemit.CompareEqual();
            opemit.StoreLocal(boolLocal);
            opemit.LoadLocal(boolLocal);
            /*如果没有在缓存中找到，则比较器结果为1，不跳转，否则直接跳转到结果标签*/
            opemit.BranchIfFalse(objectResult);

            /*new object*/
            opemit.NewObject(ctor);
            opemit.StoreLocal(objectLocal);
            opemit.Branch(objectResult);

            /*result*/
            opemit.MarkLabel(objectResult);
            opemit.LoadArgument(0);
            opemit.LoadArgument(1);
            opemit.LoadArgument(2);
            opemit.LoadArgument(3);
            opemit.LoadLocal(objectLocal);
            opemit.Call(typeof(RegisterRuleBuilder).GetMethod("InsertIntoCache", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(rule.ServiceType));
            opemit.Return();

            return opemit.CreateDelegate();
        }

        /// <summary>
        /// 分析构造函数
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        private static List<CtorParameter> Analyse(RegisterRule rule)
        {
            var ctors = rule.ImplementationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            var list = new List<CtorParameter>(ctors.Length);
            foreach (var ctor in ctors)
            {
                list.Add(new CtorParameter() { Constructor = ctor, Parameters = ctor.GetParameters() });
            }

            return list;
        }

        /// <summary>
        /// 构建行为
        /// </summary>
        /// <param name="rule">注册规则</param>
        /// <param name="recursion">构建递归</param>
        /// <param name="collection">规则集合</param>
        /// <param name="container">容器</param>
        /// <param name="level">构建层次，多用于递归使用的</param>
        /// <returns></returns>
        private static RegisterRule BuildDelegateUsingRule(RegisterRule rule, IRegisterRuleQuery collection, IRegisterRuleChangeable container, List<RegisterRule> recursion, int level)
        {
            if (rule == null)
            {
                return rule;
            }

            if (rule.Builder != null)
            {
                return rule;
            }

            rule.Builder = BuildDelegate(rule, collection, container, recursion, level);
            return rule;
        }

        /// <summary>
        /// 构建行为
        /// </summary>
        /// <param name="rule">注册规则</param>
        /// <param name="recursion">构建递归</param>
        /// <param name="collection">规则集合</param>
        /// <param name="container">容器</param>
        /// <param name="level">构建层次，多用于递归使用的</param>
        /// <returns></returns>
        private static RegisterRule OptionBuildDelegateUsingRule(RegisterRule rule, IRegisterRuleQuery collection, IRegisterRuleChangeable container, List<RegisterRule> recursion, int level)
        {
            if (rule == null)
            {
                return rule;
            }

            rule.OptionalBuilder = OptionalBuildDelegate(rule, collection, container, recursion, level);
            return rule;
        }

        /// <summary>
        /// 构建行为委托，搜索构造函数按最少构造参数，并且生命周期要相容
        /// </summary>
        /// <param name="rule">注册规则</param>
        /// <param name="recursion">构建递归</param>
        /// <param name="container">容器</param>
        /// <param name="collection">规则集合</param>
        /// <param name="level">构建层次，多用于递归使用的</param>
        /// <returns></returns>
        private static Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object> BuildDelegate(RegisterRule rule, IRegisterRuleQuery collection, IRegisterRuleChangeable container, List<RegisterRule> recursion, int level)
        {
            /*选择策略*/
            if (level > 0)
            {
                /*递归检查*/
                foreach (var re in recursion)
                {
                    if (re.ImplementationType == rule.ImplementationType)
                    {
                        throw new ArgumentOutOfRangeException(string.Format("{0}和{1}类型形成递归调用", re.ImplementationType.FullName, rule.ImplementationType.FullName));
                    }
                }

                if (recursion[recursion.Count - 1] != null)
                {
                    RuleMatchUsingNegativeSort(rule, recursion[recursion.Count - 1]);
                }
            }

            /*数组和字典的默认构造*/
            var @delete = BuildEnumerableDelegate(rule, collection, container, recursion, level);
            if (@delete != null)
            {
                return @delete;
            }

            /*构造函数查询*/
            var ctors = Analyse(rule).OrderByDescending(o => o.Parameters.Length);
            if (ctors.Count() == 0)
            {
                throw new ArgumentNullException(string.Format("{0} type can not find the public ctors", rule.ImplementationType.FullName));
            }

            /*值对象，并且没有自定义构造函数*/
            if (ctors.Count() == 1 && ctors.FirstOrDefault().Parameters.Length == 0)
            {
                if (rule.ImplementationType.IsValueType)
                {
                    return BuildValueTypeNoCtorParameterDelegate(rule);
                }

                return BuildRefenerceTypeNoCtorParameterDelegate(rule, ctors.FirstOrDefault().Constructor);
            }

            var lasttype = default(Type);
            /*构造函数查询，查询最多构造参数的一个*/
            foreach (var ctor in ctors)
            {
                try
                {
                    var ctorRules = new RegisterRule[ctor.Parameters.Length];
                    /*使用指定参数注入*/
                    if (rule.ParametersCount > 0)
                    {
                        /*全量匹配*/
                        var match = new int[rule.ParametersCount];
                        for (var i = 0; i < rule.ParametersCount; i++)
                        {
                            match[i] = -1;
                        }

                        for (var ic = 0; ic < ctor.Parameters.Length; ic++)
                        {
                            var parameter = ctor.Parameters[ic];
                            RegisterRule item = null;
                            for (var i = 0; i < rule.ParametersCount; i++)
                            {
                                /*标识已经使用了这个参数了*/
                                if (match[i] == 1)
                                {
                                    continue;
                                }

                                if (rule.Parameters[i].Value != parameter.ParameterType)
                                {
                                    continue;
                                }

                                if (ctorRules[ic] != null)
                                {
                                    continue;
                                }

                                var rules = collection.StrictQuery(parameter.ParameterType, rule.Parameters[i].Key).ToArray();
                                if (rules != null && rules.Any())
                                {
                                    /*存在*/
                                    item = rule.OptionalCompatible(rules);
                                    if (item != null)
                                    {
                                        match[i] = 1;
                                        break;
                                    }
                                    else
                                    {
                                        item = rules.FirstOrDefault().Clone(rule);
                                        match[i] = 1;
                                        break;
                                    }
                                }
                                else if (parameter.ParameterType.IsGenericType)
                                {
                                    rules = collection.StrictQuery(parameter.ParameterType.GetGenericTypeDefinition(), rule.Parameters[i].Key).ToArray();
                                    item = rule.OptionalCompatible(rules);
                                    /*存在，此时item为泛型定义*/
                                    if (item != null)
                                    {
                                        var containerRule = new RegisterRuleCollector(1);
                                        var registerRule = (RegisterRule)containerRule.RegisterType(item.ImplementationType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.ServiceType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.Key, item.LifeStyle);
                                        if (rule.ParametersCount > 0)
                                        {
                                            registerRule.PasteParameters(rule);
                                        }

                                        container.Update(containerRule);
                                        item = registerRule;
                                        ctorRules[ic] = item;
                                        continue;
                                    }
                                    if (rules != null && rules.Any())
                                    {
                                        item = rules.FirstOrDefault();
                                        var containerRule = new RegisterRuleCollector(1);
                                        var registerRule = (RegisterRule)containerRule.RegisterType(item.ImplementationType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.ServiceType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.Key, item.LifeStyle);
                                        if (rule.ParametersCount > 0)
                                        {
                                            registerRule.PasteParameters(rule);
                                        }

                                        item = registerRule;
                                        ctorRules[ic] = item;
                                        continue;
                                    }
                                }
                            }

                            if (item != null)
                            {
                                ctorRules[ic] = item;
                                continue;
                            }
                        }
                    }

                    /*再遍历剩余参数*/
                    for (var ic = 0; ic < ctor.Parameters.Length; ic++)
                    {
                        if (ctorRules[ic] != null)
                        {
                            continue;
                        }

                        var parameter = ctor.Parameters[ic];
                        var rules = collection.StrictQuery(parameter.ParameterType, string.Empty).ToArray();
                        if (rules != null && rules.Any())
                        {
                            var item = rule.OptionalCompatible(rules);
                            /*存在*/
                            if (item != null)
                            {
                                ctorRules[ic] = item;
                                continue;
                            }
                            else
                            {
                                item = rules.FirstOrDefault().Clone(rule);
                                ctorRules[ic] = item;
                                continue;
                            }
                        }
                        else if (parameter.ParameterType.IsGenericType)
                        {
                            rules = collection.StrictQuery(parameter.ParameterType.GetGenericTypeDefinition(), string.Empty).ToArray();
                            var item = rule.OptionalCompatible(rules);
                            /*存在，此时item为泛型定义*/
                            if (item != null)
                            {
                                var containerRule = new RegisterRuleCollector(1);
                                var registerRule = (RegisterRule)containerRule.RegisterType(item.ImplementationType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.ServiceType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.Key, item.LifeStyle);
                                if (rule.ParametersCount > 0)
                                {
                                    registerRule.PasteParameters(rule);
                                }

                                container.Update(containerRule);
                                item = registerRule;
                                ctorRules[ic] = item;
                                continue;
                            }
                            if (rules != null && rules.Any())
                            {
                                item = rules.FirstOrDefault();
                                var containerRule = new RegisterRuleCollector(1);
                                var registerRule = (RegisterRule)containerRule.RegisterType(item.ImplementationType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.ServiceType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.Key, item.LifeStyle);
                                if (rule.ParametersCount > 0)
                                {
                                    registerRule.PasteParameters(rule);
                                }

                                item = registerRule;
                                ctorRules[ic] = item;
                                continue;
                            }
                        }
                    }

                    /*因为要遍历所有的构造，所以不相同的构造条件要忽略*/
                    if (ctorRules.Count(t => t != null) != ctor.Parameters.Length)
                    {
                        foreach (var cp in ctor.Parameters)
                        {
                            if (ctorRules.Any(ta => ta != null && ta.ServiceType == cp.ParameterType))
                            {
                                continue;
                            }

                            lasttype = cp.ParameterType;
                            break;
                        }

                        continue;
                    }

                    /*要分别对不同的规则进行Build*/
                    foreach (var ctorRule in ctorRules)
                    {
                        if (ctorRule.Builded)
                        {
                            continue;
                        }

                        recursion.Add(rule);
                        BuildDelegateUsingRule(ctorRule, collection, container, recursion, level + 1);
                        recursion.Remove(rule);
                    }

                    /*填充规则*/
                    rule.FillConstructorParametersOnBuilding(ctorRules.ToArray());

                    /*emit builder*/
                    var emit = EasyEmitBuilder<Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>>.NewDynamicMethod();

                    /*locals*/
                    var objectResultLocal = emit.DeclareLocal(typeof(object));
                    var structObjectResultLocal = emit.DeclareLocal(rule.ImplementationType);
                    var objectResultLabel = emit.DefineLabel();
                    var boolResultLocal = emit.DeclareLocal(typeof(bool));

                    /*begin emit*/
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadArgument(2);
                    emit.LoadArgument(3);
                    emit.Call(typeof(RegisterRuleBuilder).GetMethod("QueryInCache", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(rule.ServiceType));
                    emit.StoreLocal(objectResultLocal);
                    emit.LoadLocal(objectResultLocal);
                    emit.LoadNull();
                    emit.CompareEqual();
                    emit.StoreLocal(boolResultLocal);
                    emit.LoadLocal(boolResultLocal);
                    /*如果没有在缓存中找到，则比较器结果为1，不跳转，否则直接跳转到结果标签*/
                    emit.BranchIfFalse(objectResultLabel);

                    /*new object*/
                    for (var i = 0; i < ctorRules.Length; i++)
                    {
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        emit.LoadArgument(2);
                        emit.LoadArgument(3);
                        emit.LoadConstant(i);
                        emit.Call(typeof(RegisterRuleBuilder).GetMethod("ExcutingRule", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(rule.ServiceType));
                    }

                    /*值对象*/
                    if (rule.ImplementationType.IsValueType)
                    {
                        //emit.LoadLocalAddress(structObjectResultLocal);
                        emit.NewObject(ctor.Constructor);
                        //emit.LoadLocal(structObjectResultLocal);
                        emit.Box(rule.ImplementationType);
                        emit.StoreLocal(objectResultLocal);
                        emit.Branch(objectResultLabel);
                    }
                    else
                    {
                        emit.NewObject(ctor.Constructor);
                        emit.StoreLocal(objectResultLocal);
                        emit.Branch(objectResultLabel);
                    }

                    /*结果*/
                    emit.MarkLabel(objectResultLabel);
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadArgument(2);
                    emit.LoadArgument(3);
                    emit.LoadLocal(objectResultLocal);
                    emit.Call(typeof(RegisterRuleBuilder).GetMethod("InsertIntoCache", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(rule.ServiceType));
                    emit.Return();
                    /*end emit*/

                    return emit.CreateDelegate();
                }
                catch
                {
                    continue;
                }
            }

            throw new ArgumentNullException(string.Format("{0} type can not invoke on the ctor,it miss {1} parameter", rule.ImplementationType.FullName, lasttype.FullName));
        }

        /// <summary>
        /// 构建行为委托，搜索构造函数按最多构造参数排序去构造，如发现生命周期不相容，则以最顶层的周期一直覆盖下去
        /// </summary>
        /// <param name="rule">注册规则</param>
        /// <param name="recursion">构建递归</param>
        /// <param name="container">容器</param>
        /// <param name="collection">规则集合</param>
        /// <param name="level">构建层次，多用于递归使用的</param>
        /// <returns></returns>
        private static Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object> OptionalBuildDelegate(RegisterRule rule, IRegisterRuleQuery collection, IRegisterRuleChangeable container, List<RegisterRule> recursion, int level)
        {
            /*选择策略*/
            if (level > 0)
            {
                /*递归检查*/
                foreach (var re in recursion)
                {
                    if (re.ImplementationType == rule.ImplementationType)
                    {
                        throw new ArgumentOutOfRangeException(string.Format("{0}和{1}类型形成递归调用", re.ImplementationType.FullName, rule.ImplementationType.FullName));
                    }
                }
            }

            /*数组和字典的默认构造*/
            var @delete = BuildEnumerableDelegate(rule, collection, container, recursion, level);
            if (@delete != null)
            {
                return @delete;
            }

            /*构造函数查询*/
            var ctors = Analyse(rule).OrderByDescending(o => o.Parameters.Length);
            if (ctors.Count() == 0)
            {
                return new Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>((x, y, s, z) => { return null; });
            }

            /*值对象，并且没有自定义构造函数*/
            if (ctors.Count() == 1 && ctors.FirstOrDefault().Parameters.Length == 0)
            {
                if (rule.ImplementationType.IsValueType)
                {
                    return BuildValueTypeNoCtorParameterDelegate(rule);
                }

                return BuildRefenerceTypeNoCtorParameterDelegate(rule, ctors.FirstOrDefault().Constructor);
            }

            var lasttype = default(Type);
            foreach (var ctor in ctors)
            {
                try
                {
                    var ctorRules = new RegisterRule[ctor.Parameters.Length];
                    /*使用指定参数注入*/
                    if (rule.ParametersCount > 0)
                    {
                        /*全量匹配*/
                        var match = new int[rule.ParametersCount];
                        for (var i = 0; i < rule.ParametersCount; i++)
                        {
                            match[i] = -1;
                        }

                        for (var ic = 0; ic < ctor.Parameters.Length; ic++)
                        {
                            var parameter = ctor.Parameters[ic];
                            RegisterRule item = null;
                            for (var i = 0; i < rule.ParametersCount; i++)
                            {
                                /*标识已经使用了这个参数了*/
                                if (match[i] == 1)
                                {
                                    continue;
                                }

                                if (rule.Parameters[i].Value != parameter.ParameterType)
                                {
                                    continue;
                                }

                                if (ctorRules[ic] != null)
                                {
                                    continue;
                                }

                                var rules = collection.StrictQuery(parameter.ParameterType, rule.Parameters[i].Key).ToArray();
                                if (rules != null && rules.Any())
                                {
                                    /*存在*/
                                    item = rule.LastCompatible(rules);
                                    if (item != null)
                                    {
                                        match[i] = 1;
                                        break;
                                    }
                                    else
                                    {
                                        item = rules.FirstOrDefault().Clone(rule);
                                        match[i] = 1;
                                        break;
                                    }
                                }
                                else if (parameter.ParameterType.IsGenericType)
                                {
                                    rules = collection.StrictQuery(parameter.ParameterType.GetGenericTypeDefinition(), rule.Parameters[i].Key).ToArray();
                                    item = rule.LastCompatible(rules);
                                    /*存在，此时item为泛型定义*/
                                    if (item != null)
                                    {
                                        var containerRule = new RegisterRuleCollector(1);
                                        var registerRule = (RegisterRule)containerRule.RegisterType(item.ImplementationType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.ServiceType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.Key, item.LifeStyle);
                                        if (rule.ParametersCount > 0)
                                        {
                                            registerRule.PasteParameters(rule);
                                        }

                                        container.Update(containerRule);
                                        item = registerRule;
                                        ctorRules[ic] = item;
                                        continue;
                                    }
                                    if (rules != null && rules.Any())
                                    {
                                        item = rules.FirstOrDefault();
                                        var containerRule = new RegisterRuleCollector(1);
                                        var registerRule = (RegisterRule)containerRule.RegisterType(item.ImplementationType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.ServiceType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.Key, item.LifeStyle);
                                        if (rule.ParametersCount > 0)
                                        {
                                            registerRule.PasteParameters(rule);
                                        }

                                        item = registerRule;
                                        ctorRules[ic] = item;
                                        continue;
                                    }
                                }
                            }

                            if (item != null)
                            {
                                ctorRules[ic] = item;
                                continue;
                            }
                        }
                    }

                    /*再遍历剩余参数*/
                    for (var ic = 0; ic < ctor.Parameters.Length; ic++)
                    {
                        if (ctorRules[ic] != null)
                        {
                            continue;
                        }

                        var parameter = ctor.Parameters[ic];
                        var rules = collection.StrictQuery(parameter.ParameterType, string.Empty).ToArray();
                        if (rules != null && rules.Any())
                        {
                            var item = rule.LastCompatible(rules);
                            /*存在*/
                            if (item != null)
                            {
                                ctorRules[ic] = item;
                                continue;
                            }
                            else
                            {
                                item = rules.FirstOrDefault().Clone(rule);
                                ctorRules[ic] = item;
                                continue;
                            }
                        }
                        else if (parameter.ParameterType.IsGenericType)
                        {
                            rules = collection.StrictQuery(parameter.ParameterType.GetGenericTypeDefinition(), string.Empty).ToArray();
                            var item = rule.LastCompatible(rules);
                            /*存在，此时item为泛型定义*/
                            if (item != null)
                            {
                                var containerRule = new RegisterRuleCollector(1);
                                var registerRule = (RegisterRule)containerRule.RegisterType(item.ImplementationType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.ServiceType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.Key, item.LifeStyle);
                                if (rule.ParametersCount > 0)
                                {
                                    registerRule.PasteParameters(rule);
                                }

                                container.Update(containerRule);
                                item = registerRule;
                                ctorRules[ic] = item;
                                continue;
                            }
                            if (rules != null && rules.Any())
                            {
                                item = rules.FirstOrDefault();
                                var containerRule = new RegisterRuleCollector(1);
                                var registerRule = (RegisterRule)containerRule.RegisterType(item.ImplementationType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.ServiceType.MakeGenericType(parameter.ParameterType.GetGenericArguments()), item.Key, item.LifeStyle);
                                if (rule.ParametersCount > 0)
                                {
                                    registerRule.PasteParameters(rule);
                                }

                                item = registerRule;
                                ctorRules[ic] = item;
                                continue;
                            }
                        }
                    }

                    /*因为要遍历所有的构造，所以不相同的构造条件要忽略*/
                    if (ctorRules.Count(t => t != null) != ctor.Parameters.Length)
                    {
                        foreach (var cp in ctor.Parameters)
                        {
                            if (ctorRules.Any(ta => ta != null && ta.ServiceType == cp.ParameterType))
                            {
                                continue;
                            }

                            lasttype = cp.ParameterType;
                            break;
                        }

                        continue;
                    }

                    /*要分别对不同的规则进行Build*/
                    foreach (var ctorRule in ctorRules)
                    {
                        if (ctorRule.OptionalBuilded)
                        {
                            continue;
                        }

                        recursion.Add(rule);
                        OptionBuildDelegateUsingRule(ctorRule, collection, container, recursion, level + 1);
                        recursion.Remove(rule);
                    }

                    /*填充规则*/
                    rule.OptionalFillConstructorParametersOnBuilding(ctorRules.ToArray());

                    /*emit builder*/
                    var emit = EasyEmitBuilder<Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>>.NewDynamicMethod();

                    /*locals*/
                    var objectResultLocal = emit.DeclareLocal(typeof(object));
                    var structObjectResultLocal = emit.DeclareLocal(rule.ImplementationType);
                    var objectResultLabel = emit.DefineLabel();
                    var boolResultLocal = emit.DeclareLocal(typeof(bool));

                    /*begin emit*/
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadArgument(2);
                    emit.LoadArgument(3);
                    emit.Call(typeof(RegisterRuleBuilder).GetMethod("QueryInCache", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(rule.ServiceType));
                    emit.StoreLocal(objectResultLocal);
                    emit.LoadLocal(objectResultLocal);
                    emit.LoadNull();
                    emit.CompareEqual();
                    emit.StoreLocal(boolResultLocal);
                    emit.LoadLocal(boolResultLocal);
                    /*如果没有在缓存中找到，则比较器结果为1，不跳转，否则直接跳转到结果标签*/
                    emit.BranchIfFalse(objectResultLabel);

                    /*new object*/
                    for (var i = 0; i < ctorRules.Length; i++)
                    {
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        emit.LoadArgument(2);
                        emit.LoadArgument(3);
                        emit.LoadConstant(i);
                        emit.Call(typeof(RegisterRuleBuilder).GetMethod("OptionalExcutingRule", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(rule.ServiceType));
                    }

                    /*值对象*/
                    if (rule.ImplementationType.IsValueType)
                    {
                        //emit.LoadLocalAddress(structObjectResultLocal);
                        emit.NewObject(ctor.Constructor);
                        //emit.LoadLocal(structObjectResultLocal);
                        emit.Box(rule.ImplementationType);
                        emit.StoreLocal(objectResultLocal);
                        emit.Branch(objectResultLabel);
                    }
                    else
                    {
                        emit.NewObject(ctor.Constructor);
                        emit.StoreLocal(objectResultLocal);
                        emit.Branch(objectResultLabel);
                    }

                    /*结果*/
                    emit.MarkLabel(objectResultLabel);
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadArgument(2);
                    emit.LoadArgument(3);
                    emit.LoadLocal(objectResultLocal);
                    emit.Call(typeof(RegisterRuleBuilder).GetMethod("InsertIntoCache", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(rule.ServiceType));
                    emit.Return();
                    /*end emit*/

                    return emit.CreateDelegate();
                }
                catch (Exception)
                {
                    continue;
                }
            }

            if (level == 0)
            {
                return new Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>((x, y, s, z) => { return null; });
            }

            throw new ArgumentNullException(string.Format("{0} type can not invoke on the ctor,it miss {1} parameter", rule.ImplementationType.FullName, lasttype.FullName));
        }

        /// <summary>
        /// 针对一些类型做处理，比如<see cref="IEnumerable"/>泛型类，可以通过new一些常见的类型来替换
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="collection"></param>
        /// <param name="container"></param>
        /// <param name="recursion"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private static Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object> BuildEnumerableDelegate(RegisterRule rule, IRegisterRuleQuery collection, IRegisterRuleChangeable container, List<RegisterRule> recursion, int level)
        {
            if (!rule.ImplementationType.IsGenericType)
            {
                return null;
            }

            if (rule.ImplementationType.GetGenericTypeDefinition() == (typeof(ConstructorList<>)))
            {
                var genericParameters = rule.ImplementationType.GetGenericArguments();
                /*list*/
                if (genericParameters.Length == 1)
                {
                    /*emit builder*/
                    EasyEmitBuilder<Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>> emit = null;

                    /*interface*/
                    if (collection.FreedomQuery(genericParameters[0], string.Empty, rule) != null)
                    {
                        goto _exits;
                    }

                    if (genericParameters[0].IsGenericType && collection.FreedomQuery(genericParameters[0].GetGenericTypeDefinition(), string.Empty, rule) != null)
                    {
                        goto _genericeExits;
                    }

                    /*emit builder*/
                    emit = EasyEmitBuilder<Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>>.NewDynamicMethod();
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.LoadArgument(2);
                    emit.LoadArgument(3);
                    emit.Call(typeof(RegisterRuleBuilder).GetMethod("CreateEmptyEnumerable").MakeGenericMethod(genericParameters));
                    emit.Return();
                    return emit.CreateDelegate();

                _exits:
                    {
                        /*emit builder*/
                        emit = EasyEmitBuilder<Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>>.NewDynamicMethod();
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        emit.LoadArgument(2);
                        emit.LoadArgument(3);
                        emit.Call(typeof(RegisterRuleBuilder).GetMethod("CreateGenericeEnumerable").MakeGenericMethod(genericParameters));
                        emit.Return();
                        return emit.CreateDelegate();
                    }

                _genericeExits:
                    {
                        emit = EasyEmitBuilder<Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>>.NewDynamicMethod();
                        emit.LoadArgument(0);
                        emit.LoadArgument(1);
                        emit.LoadArgument(2);
                        emit.LoadArgument(3);
                        if (rule.IsArray)
                        {
                            emit.Call(typeof(RegisterRuleBuilder).GetMethod("CreateGenericeEnumerableOnArrayType").MakeGenericMethod(genericParameters));
                        }
                        else
                        {
                            emit.Call(typeof(RegisterRuleBuilder).GetMethod("CreateGenericeEnumerableOnEnumerableType").MakeGenericMethod(genericParameters));
                        }

                        emit.Return();
                        return emit.CreateDelegate();
                    }
                }

                return null;
            }

            if (rule.ImplementationType.GetGenericTypeDefinition() == (typeof(ConstructorDictionary<,>)))
            {
                var genericParameters = rule.ImplementationType.GetGenericArguments();
                /*key-value*/
                if (genericParameters.Length == 2)
                {
                    /*interface*/
                    if (genericParameters[0].IsGenericType || genericParameters[1].IsGenericType)
                    {
                        if (genericParameters[0].IsGenericType)
                        {
                            if (collection.FreedomQuery(genericParameters[0].GetGenericTypeDefinition(), string.Empty, rule) == null)
                            {
                                /*emit builder*/
                                var emit = EasyEmitBuilder<Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>>.NewDynamicMethod();
                                emit.LoadArgument(0);
                                emit.LoadArgument(1);
                                emit.LoadArgument(2);
                                emit.LoadArgument(3);
                                emit.Call(typeof(RegisterRuleBuilder).GetMethod("CreateEmptyDictionary").MakeGenericMethod(genericParameters));
                                emit.Return();
                                return emit.CreateDelegate();
                            }
                            if (collection.FreedomQuery(genericParameters[1], string.Empty, rule) == null)
                            {
                                /*emit builder*/
                                var emit = EasyEmitBuilder<Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>>.NewDynamicMethod();
                                emit.LoadArgument(0);
                                emit.LoadArgument(1);
                                emit.LoadArgument(2);
                                emit.LoadArgument(3);
                                emit.Call(typeof(RegisterRuleBuilder).GetMethod("CreateEmptyDictionary").MakeGenericMethod(genericParameters));
                                emit.Return();
                                return emit.CreateDelegate();
                            }
                        }
                        if (genericParameters[1].IsGenericType)
                        {
                            if (collection.FreedomQuery(genericParameters[1].GetGenericTypeDefinition(), string.Empty, rule) == null)
                            {
                                /*emit builder*/
                                var emit = EasyEmitBuilder<Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>>.NewDynamicMethod();
                                emit.LoadArgument(0);
                                emit.LoadArgument(1);
                                emit.LoadArgument(2);
                                emit.LoadArgument(3);
                                emit.Call(typeof(RegisterRuleBuilder).GetMethod("CreateEmptyDictionary").MakeGenericMethod(genericParameters));
                                emit.Return();
                                return emit.CreateDelegate();
                            }
                            if (collection.FreedomQuery(genericParameters[0], string.Empty, rule) == null)
                            {
                                /*emit builder*/
                                var emit = EasyEmitBuilder<Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>>.NewDynamicMethod();
                                emit.LoadArgument(0);
                                emit.LoadArgument(1);
                                emit.LoadArgument(2);
                                emit.LoadArgument(3);
                                emit.Call(typeof(RegisterRuleBuilder).GetMethod("CreateEmptyDictionary").MakeGenericMethod(genericParameters));
                                emit.Return();
                                return emit.CreateDelegate();
                            }
                        }
                    }
                    else
                    {
                        if (collection.FreedomQuery(genericParameters[0], string.Empty, rule) == null || collection.FreedomQuery(genericParameters[1], string.Empty, rule) == null)
                        {
                            /*emit builder*/
                            var emit = EasyEmitBuilder<Func<RegisterRule, IRegisterRuleQuery, ILifetimeScope, IResolveContext, object>>.NewDynamicMethod();
                            emit.LoadArgument(0);
                            emit.LoadArgument(1);
                            emit.LoadArgument(2);
                            emit.LoadArgument(3);
                            emit.Call(typeof(RegisterRuleBuilder).GetMethod("CreateEmptyDictionary").MakeGenericMethod(genericParameters));
                            emit.Return();
                            return emit.CreateDelegate();
                        }
                    }
                }
            }

            return null;
        }

        #endregion build

        #region enumerable

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rule"></param>
        /// <param name="collection"></param>
        /// <param name="scope"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<T> CreateEmptyEnumerable<T>(RegisterRule rule, IRegisterRuleQuery collection, ILifetimeScope scope, IResolveContext context)
        {
            return ConstructorList<T>.CreateEmpty().ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rule"></param>
        /// <param name="collection"></param>
        /// <param name="scope"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<T> CreateGenericeEnumerable<T>(RegisterRule rule, IRegisterRuleQuery collection, ILifetimeScope scope, IResolveContext context)
        {
            if (scope is LifetimeScope)
            {
                var rules = collection.QueryAllRule(typeof(T));
                if (rules == null || rules.Length == 0)
                {
                    return ConstructorList<T>.CreateArray(Anonymous.NewEnumerable<T>()).ToArray();
                }

                var vlues = new List<T>(rules.Length);
                foreach (var r in rules)
                {
                    vlues.Add((T)((LifetimeScope)scope).ResolveOptional(r));
                }

                return ConstructorList<T>.CreateArray(vlues).ToArray();
            }

            var all = scope.ResolveAll(typeof(T));
            var result = all?.Select(t => (T)t).AsEnumerable();
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rule"></param>
        /// <param name="collection"></param>
        /// <param name="scope"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<T> CreateGenericeEnumerableOnEnumerableType<T>(RegisterRule rule, IRegisterRuleQuery collection, ILifetimeScope scope, IResolveContext context)
        {
            if (scope is LifetimeScope)
            {
                var r = collection.QueryRule(typeof(T), string.Empty, false);
                if (r == null)
                {
                    return new T[0];
                }

                return new T[1] { (T)((LifetimeScope)scope).Resolve(r) }.AsEnumerable();
            }

            var value = (T)scope.Resolve(typeof(T), string.Empty);
            if (value == null)
            {
                return new T[0];
            }

            return new T[] { value };
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rule"></param>
        /// <param name="collection"></param>
        /// <param name="scope"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T[] CreateGenericeEnumerableOnArrayType<T>(RegisterRule rule, IRegisterRuleQuery collection, ILifetimeScope scope, IResolveContext context)
        {
            if (scope is LifetimeScope)
            {
                var r = collection.QueryRule(typeof(T), string.Empty, false);
                if (r == null)
                {
                    return new T[0];
                }

                return new T[1] { (T)((LifetimeScope)scope).Resolve(r) };
            }

            var value = (T)scope.Resolve(typeof(T), string.Empty);
            if (value == null)
            {
                return new T[0];
            }

            return new T[] { value };
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="rule"></param>
        /// <param name="collection"></param>
        /// <param name="scope"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IDictionary<T, V> CreateEmptyDictionary<T, V>(RegisterRule rule, IRegisterRuleQuery collection, ILifetimeScope scope, IResolveContext context)
        {
            return ConstructorDictionary<T, V>.CreateEmpty();
        }

        #endregion enumerable

        #region excuting

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="collection"></param>
        /// <param name="scope"></param>
        /// <param name="context"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static object ExcutingRule<T>(RegisterRule rule, IRegisterRuleQuery collection, ILifetimeScope scope, IResolveContext context, int index)
        {
            var r = rule.Query(index);
            var result = r.Builder.Invoke(r, collection, scope, context);
            return result;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="collection"></param>
        /// <param name="scope"></param>
        /// <param name="context"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static object OptionalExcutingRule<T>(RegisterRule rule, IRegisterRuleQuery collection, ILifetimeScope scope, IResolveContext context, int index)
        {
            var r = rule.OptionalQuery(index);
            var result = r.OptionalBuilder.Invoke(r, collection, scope, context);
            return result;
        }

        #endregion excuting

        #region cached

        /// <summary>
        /// 在缓存中查询cache
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="scope"></param>
        /// <param name="collection">The collection.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">当前生命周期不在管理内</exception>
        private static object QueryInCache<T>(RegisterRule rule, IRegisterRuleQuery collection, ILifetimeScope scope, IResolveContext context)
        {
            var @object = context.Query<T>(rule, scope);
            return @object;
        }

        /// <summary>
        /// 记录到缓存中
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="scope"></param>
        /// <param name="collection">The collection.</param>
        /// <param name="context">The context.</param>
        /// <param name="object">The object.</param>
        private static object InsertIntoCache<T>(RegisterRule rule, IRegisterRuleQuery collection, ILifetimeScope scope, IResolveContext context, object @object)
        {
            return context.Cache<T>(rule, scope, (T)@object);
        }

        #endregion cached
    }
}