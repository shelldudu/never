using Never.IoC.Injections;
using System.Collections.Generic;
using System.Linq;

namespace Never.IoC
{
    /// <summary>
    /// 扩展信息
    /// </summary>
    public static class EasyExtension
    {
        /// <summary>
        /// 是否相容的周期
        /// </summary>
        /// <param name="current">当前周期</param>
        /// <param name="target">目标周期</param>
        /// <returns></returns>
        public static bool Compatible(this ComponentLifeStyle target, ComponentLifeStyle current)
        {
            switch (current)
            {
                /*单例可以注入到任何实例中，其构造只能是单例对象*/
                case ComponentLifeStyle.Singleton:
                    {
                        return true;
                    }
                /*短暂只能注入到短暂，其构造可接受任何实例对象*/
                case ComponentLifeStyle.Transient:
                    {
                        return target == ComponentLifeStyle.Transient;
                    }
                /*作用域其构造不能接受短暂，可接受有作用域和单例*/
                case ComponentLifeStyle.Scoped:
                    {
                        return target != ComponentLifeStyle.Singleton;
                    }
            }

            return false;
        }

        /// <summary>
        /// 是否相容的周期
        /// </summary>
        /// <param name="current">当前周期</param>
        /// <param name="target">目标周期</param>
        /// <returns></returns>
        public static string Compatible(this RegisterRule target, RegisterRule current)
        {
            switch (current.LifeStyle)
            {
                /*单例可以注入到任何实例中，其构造只能是单例对象*/
                case ComponentLifeStyle.Singleton:
                    {
                        return string.Empty;
                    }
                /*短暂只能注入到短暂，其构造可接受任何实例对象*/
                case ComponentLifeStyle.Transient:
                    {
                        if (target.LifeStyle != ComponentLifeStyle.Transient)
                            return string.Format("构建当前对象{0}为{1}，期望对象{2}为短暂，不能相容",
                                target.ServiceType.FullName,
                                target.LifeStyle == ComponentLifeStyle.Scoped ? "作用域" : "单例",
                                current.ServiceType.FullName);

                        return string.Empty;
                    }
                /*作用域其构造不能接受短暂，可接受有作用域和单例*/
                case ComponentLifeStyle.Scoped:
                    {
                        if (target.LifeStyle == ComponentLifeStyle.Singleton)
                            return string.Format("构建当前对象{0}为单例，期望对象{1}为作用域，不能相容",
                                target.ServiceType.FullName,
                                current.ServiceType.FullName);

                        return string.Empty;
                    }
            }

            return string.Empty;
        }

        #region optional

        /// <summary>
        /// 最优匹配
        /// </summary>
        /// <param name="topRule"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        internal static RegisterRule OptionalCompatible(this RegisterRule topRule, IEnumerable<RegisterRule> collection)
        {
            if (collection == null)
                return null;

            switch (topRule.LifeStyle)
            {
                case ComponentLifeStyle.Singleton:
                    {
                        var first = collection.FirstOrDefault(o => o.Key.IsNullOrEmpty() && o.LifeStyle == ComponentLifeStyle.Singleton && o.Builded);
                        if (first != null)
                            return first;

                        first = collection.FirstOrDefault(o => o.Key.IsNullOrEmpty() && o.LifeStyle == ComponentLifeStyle.Singleton);
                        if (first != null)
                            return first;

                        first = first = collection.FirstOrDefault(o => o.Key.IsNullOrEmpty() && o.Builded);
                        if (first != null)
                            return first;

                        first = collection.FirstOrDefault(o => o.Key.IsNullOrEmpty() && o.LifeStyle == ComponentLifeStyle.Scoped);
                        if (first != null)
                            return first;
                    }
                    break;

                case ComponentLifeStyle.Scoped:
                    {
                        var first = collection.FirstOrDefault(o => o.Key.IsNullOrEmpty() && o.LifeStyle == ComponentLifeStyle.Singleton && o.Builded);
                        if (first != null)
                            return first;

                        first = collection.FirstOrDefault(o => o.Key.IsNullOrEmpty() && o.LifeStyle == ComponentLifeStyle.Scoped && o.Builded);
                        if (first != null)
                            return first;

                        first = first = collection.FirstOrDefault(o => o.Key.IsNullOrEmpty() && o.Builded);
                        if (first != null)
                            return first;

                        first = collection.FirstOrDefault(o => o.Key.IsNullOrEmpty() && o.LifeStyle != ComponentLifeStyle.Transient);
                        if (first != null)
                            return first;
                    }
                    break;

                case ComponentLifeStyle.Transient:
                    {
                        var first = collection.FirstOrDefault(o => o.Key.IsNullOrEmpty() && o.Builded);
                        if (first != null)
                            return first;
                    }
                    break;
            }

            switch (topRule.LifeStyle)
            {
                case ComponentLifeStyle.Singleton:
                    {
                        var first = collection.FirstOrDefault(o => o.LifeStyle == ComponentLifeStyle.Singleton && o.Builded);
                        if (first != null)
                            return first;

                        first = collection.FirstOrDefault(o => o.LifeStyle == ComponentLifeStyle.Singleton);
                        if (first != null)
                            return first;

                        first = first = collection.FirstOrDefault(o => o.Builded);
                        if (first != null)
                            return first;

                        first = collection.FirstOrDefault(o => o.LifeStyle == ComponentLifeStyle.Scoped);
                        if (first != null)
                            return first;

                        return collection.FirstOrDefault();
                    }
                case ComponentLifeStyle.Scoped:
                    {
                        var first = collection.FirstOrDefault(o => o.LifeStyle == ComponentLifeStyle.Singleton && o.Builded);
                        if (first != null)
                            return first;

                        first = collection.FirstOrDefault(o => o.LifeStyle == ComponentLifeStyle.Scoped && o.Builded);
                        if (first != null)
                            return first;

                        first = first = collection.FirstOrDefault(o => o.Builded);
                        if (first != null)
                            return first;

                        first = collection.FirstOrDefault(o => o.LifeStyle != ComponentLifeStyle.Transient);
                        if (first != null)
                            return first;

                        return collection.FirstOrDefault();
                    }
                case ComponentLifeStyle.Transient:
                    {
                        var first = collection.FirstOrDefault(o => o.Builded);
                        if (first != null)
                            return first;

                        return collection.FirstOrDefault();
                    }
            }

            return null;
        }

        /// <summary>
        /// 最优匹配
        /// </summary>
        /// <param name="topRule"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        internal static RegisterRule LastCompatible(this RegisterRule topRule, IEnumerable<RegisterRule> collection)
        {
            if (collection == null)
                return null;

            for (var i = collection.Count() - 1; i >= 0; i--)
            {
                var rule = collection.ElementAt(i);
                if (rule.Builded || rule.OptionalBuilded)
                    return rule;

                if ((rule.ImplementationType.IsInterface || rule.ImplementationType.IsAbstract))
                    continue;
            }

            return collection.LastOrDefault();
        }

        #endregion optional
    }
}