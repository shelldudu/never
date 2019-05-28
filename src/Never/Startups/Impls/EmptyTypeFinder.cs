using Never.IoC;
using System;
using System.Reflection;

namespace Never.Startups.Impls
{
    /// <summary>
    /// 服务注册者
    /// </summary>
    public sealed class EmptyTypeFinder : Never.IoC.ITypeFinder
    {
        #region field

        /// <summary>
        /// 空对象
        /// </summary>
        public static EmptyTypeFinder Only
        {
            get
            {
                if (Singleton<EmptyTypeFinder>.Instance == null)
                    Singleton<EmptyTypeFinder>.Instance = new EmptyTypeFinder();

                return Singleton<EmptyTypeFinder>.Instance;
            }
        }

        #endregion field

        #region ctor

        /// <summary>
        /// Prevents a default instance of the <see cref="EmptyTypeFinder"/> class from being created.
        /// </summary>
        private EmptyTypeFinder()
        {
        }

        #endregion ctor

        /// <summary>
        /// 返回所有程序集中T类型的属性
        /// </summary>
        /// <typeparam name="TAssemblyAttribute">T特性</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <returns></returns>
        Assembly[] IoC.ITypeFinder.FindAssembliesWithAttribute<TAssemblyAttribute>(Assembly[] assemblies)
        {
            return new Assembly[] { };
        }

        /// <summary>
        /// 返回T类型属性的对象
        /// </summary>
        /// <typeparam name="TClasses">T特性</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <returns></returns>
        Type[] ITypeFinder.FindClassesOfType<TClasses>(Assembly[] assemblies)
        {
            return new Type[] { };
        }

        /// <summary>
        /// 返回T类型属性的对象
        /// </summary>
        /// <typeparam name="TClasses">T特性</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <returns></returns>
        Type[] IoC.ITypeFinder.FindClassesOfType<TClasses>(Assembly[] assemblies, bool onlyConcreteClasses)
        {
            return new Type[] { };
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceType">来源的类型，则可以从属souceType分配</param>
        /// <returns></returns>
        Type[] ITypeFinder.FindClassesOfType(Assembly[] assemblies, Type sourceType)
        {
            return new Type[] { };
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceType">来源的类型，则可以从属souceType分配</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <returns></returns>
        Type[] IoC.ITypeFinder.FindClassesOfType(Assembly[] assemblies, Type sourceType, bool onlyConcreteClasses)
        {
            return new Type[] { };
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <typeparam name="T">T特性</typeparam>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <param name="inherit">是否继承的</param>
        /// <returns></returns>
        Type[] ITypeFinder.FindClassesOfType<T, TClassAttribute>(Assembly[] assemblies, bool inherit)
        {
            return new Type[] { };
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <typeparam name="T">T特性</typeparam>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <param name="inherit">是否继承的</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <returns></returns>
        Type[] IoC.ITypeFinder.FindClassesOfType<T, TClassAttribute>(Assembly[] assemblies, bool inherit, bool onlyConcreteClasses)
        {
            return new Type[] { };
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceTypes">来源的类型，则可以从属souceType分配</param>
        /// <returns></returns>
        Type[] ITypeFinder.FindClassesOfType(Assembly[] assemblies, Type[] sourceTypes)
        {
            return new Type[] { };
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceTypes">来源的类型，则可以从属souceType分配</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <returns></returns>
        Type[] ITypeFinder.FindClassesOfType(Assembly[] assemblies, Type[] sourceTypes, bool onlyConcreteClasses)
        {
            return new Type[] { };
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="inherit">是否继承的</param>
        /// <param name="sourceType">来源的类型，则可以从属souceType分配</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <returns></returns>
        Type[] ITypeFinder.FindClassesOfType<TClassAttribute>(Assembly[] assemblies, Type sourceType, bool inherit)
        {
            return new Type[] { };
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="inherit">是否继承的</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <param name="sourceType">来源的类型，则可以从属souceType分配</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <returns></returns>
        Type[] ITypeFinder.FindClassesOfType<TClassAttribute>(Assembly[] assemblies, Type sourceType, bool inherit, bool onlyConcreteClasses)
        {
            return new Type[] { };
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="inherit">是否继承的</param>
        /// <param name="sourceTypes">来源的类型，则可以从属souceType分配</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <returns></returns>
        Type[] ITypeFinder.FindClassesOfType<TClassAttribute>(Assembly[] assemblies, Type[] sourceTypes, bool inherit)
        {
            return new Type[] { };
        }

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="inherit">是否继承的</param>
        /// <param name="sourceTypes">来源的类型，则可以从属souceType分配</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <returns></returns>
        Type[] ITypeFinder.FindClassesOfType<TClassAttribute>(Assembly[] assemblies, Type[] sourceTypes, bool inherit, bool onlyConcreteClasses)
        {
            return new Type[] { };
        }
    }
}