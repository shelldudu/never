using System;
using System.Reflection;

namespace Never.IoC
{
    /// <summary>
    /// 类型发现者
    /// </summary>
    public interface ITypeFinder
    {
        /// <summary>
        /// 返回所有程序集中T类型的属性
        /// </summary>
        /// <typeparam name="TAssemblyAttribute">T特性</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <returns></returns>
        Assembly[] FindAssembliesWithAttribute<TAssemblyAttribute>(Assembly[] assemblies) where TAssemblyAttribute : Attribute;

        /// <summary>
        /// 返回T类型属性的对象
        /// </summary>
        /// <typeparam name="TClasses">T特性</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <returns></returns>
        Type[] FindClassesOfType<TClasses>(Assembly[] assemblies);

        /// <summary>
        /// 返回T类型属性的对象
        /// </summary>
        /// <typeparam name="TClasses">T特性</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <returns></returns>
        Type[] FindClassesOfType<TClasses>(Assembly[] assemblies, bool onlyConcreteClasses);

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceType">来源的类型，则可以从属souceType分配</param>
        /// <returns></returns>

        Type[] FindClassesOfType(Assembly[] assemblies, Type sourceType);

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceType">来源的类型，则可以从属souceType分配</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <returns></returns>
        Type[] FindClassesOfType(Assembly[] assemblies, Type sourceType, bool onlyConcreteClasses);

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceTypes">来源的类型，则可以从属souceType分配</param>
        /// <returns></returns>
        Type[] FindClassesOfType(Assembly[] assemblies, Type[] sourceTypes);

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="sourceTypes">来源的类型，则可以从属souceType分配</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <returns></returns>
        Type[] FindClassesOfType(Assembly[] assemblies, Type[] sourceTypes, bool onlyConcreteClasses);

        /// <summary>
        /// 返回T类型属性的对象
        /// </summary>
        /// <typeparam name="TClasses">T特性</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <param name="inherit">是否继承的</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <returns></returns>
        Type[] FindClassesOfType<TClasses, TClassAttribute>(Assembly[] assemblies, bool inherit) where TClassAttribute : Attribute;

        /// <summary>
        /// 返回T类型属性的对象
        /// </summary>
        /// <typeparam name="TClasses">T特性</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <param name="inherit">是否继承的</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <returns></returns>
        Type[] FindClassesOfType<TClasses, TClassAttribute>(Assembly[] assemblies, bool inherit, bool onlyConcreteClasses) where TClassAttribute : Attribute;

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="inherit">是否继承的</param>
        /// <param name="sourceType">来源的类型，则可以从属souceType分配</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <returns></returns>
        Type[] FindClassesOfType<TClassAttribute>(Assembly[] assemblies, Type sourceType, bool inherit) where TClassAttribute : Attribute;

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="inherit">是否继承的</param>
        /// <param name="sourceType">来源的类型，则可以从属souceType分配</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <returns></returns>
        Type[] FindClassesOfType<TClassAttribute>(Assembly[] assemblies, Type sourceType, bool inherit, bool onlyConcreteClasses) where TClassAttribute : Attribute;

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="inherit">是否继承的</param>
        /// <param name="sourceTypes">来源的类型，则可以从属souceType分配</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <returns></returns>
        Type[] FindClassesOfType<TClassAttribute>(Assembly[] assemblies, Type[] sourceTypes, bool inherit) where TClassAttribute : Attribute;

        /// <summary>
        /// 返回类型属性的对象
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="inherit">是否继承的</param>
        /// <param name="sourceTypes">来源的类型，则可以从属souceType分配</param>
        /// <param name="onlyConcreteClasses">是否为具体对象</param>
        /// <typeparam name="TClassAttribute">T对象某些特性</typeparam>
        /// <returns></returns>
        Type[] FindClassesOfType<TClassAttribute>(Assembly[] assemblies, Type[] sourceTypes, bool inherit, bool onlyConcreteClasses) where TClassAttribute : Attribute;
    }
}