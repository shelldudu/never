using Never.IoC;
using Never.Startups;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Never.Web.WebApi.IoC
{
    /// <summary>
    /// IoC扩展
    /// </summary>
    public static class ServiceRegisterExtension
    {
        #region api

        private static Type GetControllerType()
        {
#if !NET461
            return typeof(Microsoft.AspNetCore.Mvc.ControllerBase);
#else
            return typeof(System.Web.Http.Controllers.IHttpController);
#endif
        }

        /// <summary>
        /// 注册ApiController对象
        /// </summary>
        /// <param name="container"></param>
        /// <param name="controllerAssemblies"></param>
        public static void RegisterApiControllers(this IServiceRegister container, params Assembly[] controllerAssemblies)
        {
            foreach (var i in controllerAssemblies)
            {
                if (i == null)
                    continue;
                foreach (var j in (from k in i.GetTypes() where GetControllerType().IsAssignableFrom(k) && !k.IsAbstract && k.IsClass && k.Name.EndsWith("Controller", StringComparison.Ordinal) select k))
                    container.RegisterType(j, j, string.Empty, ComponentLifeStyle.Transient);
            }
        }

        ///// <summary>
        ///// 注册WcfService对象，所注册的服务对象必须实现IWcfServiceIoCObject接口
        ///// </summary>
        ///// <param name="container"></param>
        ///// <param name="controllerAssemblies"></param>
        //public static void RegisterWcfServiceObjects(this IServiceRegister container, params Assembly[] controllerAssemblies)
        //{
        //    var attributeType = typeof(ServiceBehaviorAttribute);
        //    IEnumerable<Attribute> attributes = null;
        //    var proxyType = typeof(IProxyServiceObject);

        //    foreach (var i in controllerAssemblies)
        //    {
        //        if (i == null)
        //            continue;
        //        foreach (var j in (from k in i.GetTypes()
        //                           where !k.IsAbstract
        //                           && k.IsClass
        //                           && (proxyType.IsAssignableToType(k) || (attributes = k.GetCustomAttributes(attributeType, true) as IEnumerable<Attribute>) != null && attributes.Any())
        //                           select k))
        //        {
        //            container.RegisterType(j, j, string.Empty, ComponentLifeStyle.Transient);
        //        }
        //    }
        //}

        #endregion api
    }
}