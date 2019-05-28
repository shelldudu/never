using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never.IoC.Providers
{
    /// <summary>
    /// 默认类型查找
    /// </summary>
    public class DefaultTypeFinder : Never.IoC.ITypeFinder
    {
        #region ITypeFinder成员

        /// <summary>
        /// 返回所有程序集中T类型的属性
        /// </summary>
        /// <typeparam name="TAssemblyAttribute">T特性</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <returns></returns>
        public Assembly[] FindAssembliesWithAttribute<TAssemblyAttribute>(Assembly[] assemblies) where TAssemblyAttribute : Attribute
        {
            if (assemblies == null)
                return null;

            var list = new List<Assembly>();
            var targetType = typeof(TAssemblyAttribute);
            /*查询源程序集每一个程序是否带有了T属性*/
            foreach (var assembly in assemblies)
            {
                if (assembly == null)
                    continue;
                if (this.IsAssemblyContainAttribute(assembly, targetType))
                    list.Add(assembly);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 返回T类型属性的对象
        /// </summary>
        /// <typeparam name="TClasses">T特性</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <returns></returns>
        public Type[] FindClassesOfType<TClasses>(Assembly[] assemblies)
        {
            return this.FindClassesOfType<TClasses>(assemblies, true);
        }

        /// <summary>
        /// 返回T类型属性的对象
        /// </summary>
        /// <typeparam name="TClasses">T特性</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <returns></returns>
        public Type[] FindClassesOfType<TClasses>(Assembly[] assemblies, bool onlyConcreteClasses)
        {
            return this.FindClassesOfType(assemblies, typeof(TClasses), onlyConcreteClasses);
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceType">来源的类型，则可以从属souceType分配</param>
        /// <returns></returns>
        public Type[] FindClassesOfType(Assembly[] assemblies, Type sourceType)
        {
            return this.FindClassesOfType(assemblies, sourceType, true);
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceType">来源的类型，则可以从属souceType分配</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <returns></returns>
        public Type[] FindClassesOfType(Assembly[] assemblies, Type sourceType, bool onlyConcreteClasses)
        {
            return this.FindClassesOfType(assemblies, new[] { sourceType }, onlyConcreteClasses);
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceTypes">来源的类型，则可以从属souceType分配</param>
        /// <returns></returns>
        public Type[] FindClassesOfType(Assembly[] assemblies, Type[] sourceTypes)
        {
            return this.FindClassesOfType(assemblies, sourceTypes, true);
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceTypes">来源的类型，则可以从属souceType分配</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <returns></returns>
        public Type[] FindClassesOfType(Assembly[] assemblies, Type[] sourceTypes, bool onlyConcreteClasses)
        {
            if (assemblies == null)
                return null;

            var list = new List<Type>();
            IEnumerable<Type> assemblyTypes = null;
            var sourceDistinctTypes = new List<Type>(sourceTypes.Length);
            foreach (var type in sourceTypes)
            {
                if (sourceDistinctTypes.Contains(type))
                    continue;

                sourceDistinctTypes.Add(type);
            }

            foreach (var assembly in assemblies)
            {
                if (assembly == null)
                    continue;
                try
                {
                    assemblyTypes = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Exception fex = null;
                    if (ex.LoaderExceptions != null)
                        fex = ex.LoaderExceptions[0];
                    else
                        fex = ex;

                    throw new TypeInitializationException(assembly.FullName, fex);
                }
                catch (Exception ex)
                {
                    throw new TypeInitializationException(assembly.FullName, ex);
                }

                if (assemblyTypes == null)
                    continue;

                foreach (var type in assemblyTypes)
                {
                    foreach (var assignTypeFrom in sourceDistinctTypes)
                    {
                        if (!assignTypeFrom.IsAssignableFrom(type) && !(assignTypeFrom.IsGenericTypeDefinition && this.IsTargetTypeImplementSourceGenericType(type, assignTypeFrom)))
                            continue;

                        if (type.IsInterface)
                            continue;

                        if (onlyConcreteClasses)
                        {
                            if (type.IsClass && !type.IsAbstract && !list.Contains(type))
                                list.Add(type);
                        }
                        else if (!list.Contains(type))
                            list.Add(type);
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 返回T类型属性的对象
        /// </summary>
        /// <typeparam name="TClasses">T特性</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <param name="inherit">是否继承的</param>
        /// <returns></returns>
        public Type[] FindClassesOfType<TClasses, TClassAttribute>(Assembly[] assemblies, bool inherit) where TClassAttribute : Attribute
        {
            return this.FindClassesOfType<TClasses, TClassAttribute>(assemblies, inherit, true);
        }

        /// <summary>
        /// 返回T类型属性的对象
        /// </summary>
        /// <typeparam name="TClasses">T特性</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <param name="inherit">是否继承的</param>
        /// <returns></returns>
        public Type[] FindClassesOfType<TClasses, TClassAttribute>(Assembly[] assemblies, bool inherit, bool onlyConcreteClasses) where TClassAttribute : Attribute
        {
            return this.FindClassesOfType<TClassAttribute>(assemblies, typeof(TClasses), inherit, onlyConcreteClasses);
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceType">来源的类型，则可以从属souceType分配</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <param name="inherit">是否继承的</param>
        /// <returns></returns>
        public Type[] FindClassesOfType<TClassAttribute>(Assembly[] assemblies, Type sourceType, bool inherit) where TClassAttribute : Attribute
        {
            return this.FindClassesOfType<TClassAttribute>(assemblies, sourceType, inherit, true);
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceType">来源的类型，则可以从属souceType分配</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <param name="inherit">是否继承的</param>
        /// <returns></returns>
        public Type[] FindClassesOfType<TClassAttribute>(Assembly[] assemblies, Type sourceType, bool inherit, bool onlyConcreteClasses) where TClassAttribute : Attribute
        {
            return this.FindClassesOfType<TClassAttribute>(assemblies, new[] { sourceType }, inherit, onlyConcreteClasses);
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceTypes">来源的类型，则可以从属souceType分配</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <param name="inherit">是否继承的</param>
        /// <returns></returns>
        public Type[] FindClassesOfType<TClassAttribute>(Assembly[] assemblies, Type[] sourceTypes, bool inherit) where TClassAttribute : Attribute
        {
            return this.FindClassesOfType<TClassAttribute>(assemblies, sourceTypes, inherit, true);
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceTypes">来源的类型，则可以从属souceType分配</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <param name="inherit">是否继承的</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <returns></returns>
        public Type[] FindClassesOfType<TClassAttribute>(Assembly[] assemblies, Type[] sourceTypes, bool inherit, bool onlyConcreteClasses) where TClassAttribute : Attribute
        {
            if (assemblies == null)
                return null;

            var list = new List<Type>();
            Type classAttribute = typeof(TClassAttribute);
            IEnumerable<Type> assemblyTypes = null;
            var sourceDistinctTypes = new List<Type>(sourceTypes.Length);
            foreach (var type in sourceTypes)
            {
                if (sourceDistinctTypes.Contains(type))
                    continue;

                sourceDistinctTypes.Add(type);
            }

            foreach (var assembly in assemblies)
            {
                if (assembly == null)
                    continue;

                try
                {
                    assemblyTypes = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Exception fex = null;
                    if (ex.LoaderExceptions != null)
                        fex = ex.LoaderExceptions[0];
                    else
                        fex = ex;

                    throw new TypeInitializationException(assembly.FullName, fex);
                }
                catch (Exception ex)
                {
                    throw new TypeInitializationException(assembly.FullName, ex);
                }

                if (assemblyTypes == null)
                    continue;

                foreach (var type in assemblyTypes)
                {
                    foreach (var assignTypeFrom in sourceDistinctTypes)
                    {
                        if (!assignTypeFrom.IsAssignableFrom(type) && !(assignTypeFrom.IsGenericTypeDefinition && this.IsTargetTypeImplementSourceGenericType(type, assignTypeFrom)))
                            continue;

                        if (type.IsInterface)
                            continue;

                        var attributes = type.GetCustomAttributes(classAttribute, inherit);
                        if (attributes == null || attributes.Length == 0)
                            continue;

                        if (onlyConcreteClasses)
                        {
                            if (type.IsClass && !type.IsAbstract && !list.Contains(type))
                                list.Add(type);
                        }
                        else if (!list.Contains(type))
                            list.Add(type);
                    }
                }
            }

            return list.ToArray();
        }

        #endregion ITypeFinder成员

        #region utils

        /// <summary>
        /// 确定该程序集中是否带有属性T
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="attribute">某特性类型</param>
        /// <param name="inherit">是否继承</param>
        /// <returns></returns>
        protected bool IsAssemblyContainAttribute(Assembly assembly, Type attribute, bool inherit = false)
        {
            if (assembly == null)
                return false;
            var attributes = assembly.GetCustomAttributes(attribute, inherit);

            return attributes != null && attributes.Length > 0;
        }

        /// <summary>
        /// 确定目标类型是否可以从源泛型类型的实例分配
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="sourceGenericType">源类型</param>
        /// <returns></returns>
        protected virtual bool IsTargetTypeImplementSourceGenericType(Type targetType, Type sourceGenericType)
        {
            if (targetType == null || sourceGenericType == null)
                return false;

            var genericTypeDefinition = sourceGenericType.GetGenericTypeDefinition();
            Type[] targetTypeInterfaces = targetType.FindInterfaces((objType, objCriteria) => true, null);
            foreach (var type in targetTypeInterfaces)
            {
                if (type == null || !type.IsGenericType)
                    continue;

                if (genericTypeDefinition.IsAssignableFrom(type.GetGenericTypeDefinition()))
                    return true;
            }

            return false;
        }

        #endregion utils
    }
}