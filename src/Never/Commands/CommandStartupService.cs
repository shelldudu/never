using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never.Commands
{
    internal class CommandStartupService
    {
        public class CommandHandlerCtorParameterProcessor : Never.Startups.ITypeProcessor
        {
            /// <summary>
            /// 事件类型
            /// </summary>
            private readonly static Type commandHandlerType = typeof(ICommandHandler);

            /// <summary>
            /// 已经分析过的了
            /// </summary>
            private readonly static List<Type> exists = new List<Type>();

            /// <summary>
            ///
            /// </summary>
            /// <param name="ignoreTypes"></param>
            public CommandHandlerCtorParameterProcessor(IEnumerable<Type> ignoreTypes)
            {
                exists.AddRange(ignoreTypes);
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="application"></param>
            /// <param name="type"></param>
            public void Processing(IApplicationStartup application, Type type)
            {
                if (type == null || type.IsAbstract || !type.IsClass || type.IsInterface || type.IsGenericTypeDefinition || type.IsValueType)
                    return;

                if (!ObjectExtension.IsAssignableToType(commandHandlerType, type))
                    return;

                if (exists.Contains(type))
                    return;

                /*构造函数查询*/
                var ctorTemp = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (ctorTemp == null || ctorTemp.Length == 0)
                {
                    exists.Add(type);
                    return;
                }

                /*获取第一个，再排序找到第一个*/
                var ctor = ctorTemp[0];
                if (ctorTemp.Length > 1)
                {
                    /*这里有选择规则，因为注入的时候是可以带参数的，所以如果带参数的个数为0，则用构造函数参数个数去排序，如果
                    带参数的个数不为0，则可以先找到有这些参数的构造，然后再排序（只是这种情况可能在开发过程中由于修改了构造而没有
                    取消注册过正中带参数的行为，这个时候可能会得到不恰当的构造函数，但是一个对象如果存在多个不同参数的构造方法，
                    这个应该是不允许的，因此大多数甚者全部都只有一个构造函数)*/
                    for (var i = 1; i < ctorTemp.Length; i++)
                    {
                        if (ctorTemp[i].GetParameters().Length <= ctor.GetParameters().Length)
                        {
                            ctor = ctorTemp[i];
                        }
                    }
                }

                if (ctor.GetParameters().Length == 0)
                {
                    exists.Add(type);
                    return;
                }

                var ctorParameters = ctor.GetParameters();
                foreach (var parameter in ctorParameters)
                {
                    if (parameter.ParameterType.IsValueType)
                        continue;

                    if (application.ServiceRegister.Contain(parameter.ParameterType))
                    {
                        exists.Add(type);
                        return;
                    }

                    /*说明是不是泛型类型，是泛型参数*/
                    if (parameter.ParameterType.IsGenericType)
                    {
                        if (application.ServiceRegister.Contain(parameter.ParameterType.GetGenericTypeDefinition()))
                        {
                            exists.Add(type);
                            return;
                        }
                    }

                    throw new ArgumentNullException(string.Format("The type {0} need the paramater type {1} on ctor", type.FullName, parameter.ParameterType.FullName));
                }
            }
        }

        public class CommandNoCtorParamaterProcessor : Never.Startups.ITypeProcessor
        {
            /// <summary>
            /// 事件类型
            /// </summary>
            private static readonly Type commandType = typeof(ICommand);

            /// <summary>
            /// 已经分析过的了
            /// </summary>
            private readonly static List<Type> exists = new List<Type>();

            /// <summary>
            ///
            /// </summary>
            /// <param name="ignoreTypes"></param>
            public CommandNoCtorParamaterProcessor(IEnumerable<Type> ignoreTypes)
            {
                exists.AddRange(ignoreTypes);
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="application"></param>
            /// <param name="type"></param>
            public void Processing(IApplicationStartup application, Type type)
            {
                if (type == null || type.IsAbstract || !type.IsClass || type.IsInterface || type.IsGenericTypeDefinition || type.IsValueType)
                    return;

                if (!ObjectExtension.IsAssignableToType(commandType, type))
                    return;

                if (exists.Contains(type))
                    return;

                var attributes = type.GetCustomAttributes(typeof(Never.Attributes.IgnoreAnalyseAttribute), false);
                if (attributes != null && attributes.Length > 0)
                    return;

                /*构造函数查询*/
                if (type.GetConstructor(Type.EmptyTypes) != null)
                    return;

                var ctors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                foreach (var c in ctors)
                {
                    if (c.GetParameters().Length == 0)
                        return;
                }

                throw new ArgumentNullException(string.Format("type command Type {0} must has no-paramater ctor", type.FullName));
            }
        }
    }
}