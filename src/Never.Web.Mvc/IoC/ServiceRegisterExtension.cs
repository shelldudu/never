using Never.IoC;
using System;
using System.Linq;
using System.Reflection;

namespace Never.Web.Mvc.IoC
{
    /// <summary>
    /// IoC扩展
    /// </summary>
    public static class ServiceRegisterExtension
    {
        #region mvc

        private static Type GetControllerType()
        {
#if !NET461
            return typeof(Microsoft.AspNetCore.Mvc.ControllerBase);
#else
            return typeof(System.Web.Mvc.ControllerBase);
#endif
        }

        /// <summary>
        /// 注册Controller对象
        /// </summary>
        /// <param name="container"></param>
        /// <param name="controllerAssemblies"></param>
        public static void RegisterMvcControllers(this IServiceRegister container, params Assembly[] controllerAssemblies)
        {
            foreach (var i in controllerAssemblies)
            {
                if (i == null)
                    continue;
                foreach (var j in (from k in i.GetTypes() where GetControllerType().IsAssignableFrom(k) && !k.IsAbstract && k.IsClass && k.Name.EndsWith("Controller", StringComparison.Ordinal) select k))
                    container.RegisterType(j, j, string.Empty, ComponentLifeStyle.Transient);
            }
        }

        #endregion mvc
    }
}