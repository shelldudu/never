using Never.Attributes;
using Never.Exceptions;
using Never.Startups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if !NET461
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Never.Web.Mvc.Dispatcher
{
    /// <summary>
    /// 自定义ApiControllerType获取
    /// </summary>
    public class CustomHttpControllerSelector : IApplicationModelConvention
    {
        #region field

        /// <summary>
        /// 所有的区域
        /// </summary>
        private static readonly HashSet<string> areas = null;

        /// <summary>
        /// 所以的资源字典
        /// </summary>
        private static readonly IDictionary<string, ActionResultMetadata> routeDict = null;

        /// <summary>
        /// httpGet等集合
        /// </summary>
        private static readonly HashSet<Type> httpMethodProvider = null;

        private static bool count = false;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        static CustomHttpControllerSelector()
        {
            routeDict = new Dictionary<string, ActionResultMetadata>(500);
            areas = new HashSet<string>();
            httpMethodProvider = new HashSet<Type>();
            httpMethodProvider.Add(typeof(HttpGetAttribute));
            httpMethodProvider.Add(typeof(HttpPostAttribute));
            httpMethodProvider.Add(typeof(HttpPatchAttribute));
            httpMethodProvider.Add(typeof(HttpOptionsAttribute));
            httpMethodProvider.Add(typeof(HttpPutAttribute));
            httpMethodProvider.Add(typeof(HttpHeadAttribute));
            httpMethodProvider.Add(typeof(AcceptVerbsAttribute));
            httpMethodProvider.Add(typeof(HttpDeleteAttribute));
        }

        #endregion ctor

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var controllerAttribute = controller.Attributes.FirstOrDefault(o => o is ApiAreaRemarkAttribute);
                string areaName = (controllerAttribute == null) ? string.Empty : (((ApiAreaRemarkAttribute)controllerAttribute).Area);
                if (areaName.IsNotNullOrEmpty() && !areas.Contains(areaName))
                    areas.Add(areaName);

                foreach (var action in controller.Actions)
                {
                    var actionAttribute = action.Attributes.FirstOrDefault(o => o is ApiActionRemarkAttribute);
                    if (actionAttribute == null)
                        continue;

                    var attribute = actionAttribute as ApiActionRemarkAttribute;
                    if (attribute.UniqueId.IsNullOrEmpty())
                        continue;

                    var routeName = string.Concat(areaName.IsNullOrEmpty() ? "" : (areaName + "/"), attribute.UniqueId);
                    string actionName = action.ActionName;
                    if (!count && routeDict.ContainsKey(routeName))
                        throw new KeyExistedException(routeName, string.Format("在api控件器中找到相同的标识,当前控制器和方法为{0}-{1}，已经存在的控制器和方法为{2}-{3}", controller.ControllerType.FullName, actionName, routeDict[routeName].ControllerName, routeDict[routeName].ActionMethod.Name));

                    if (!count && action.Selectors.Where(o => o.AttributeRouteModel != null && o.AttributeRouteModel.Template.IsEquals(routeName)).Any())
                        throw new KeyExistedException(routeName, string.Format("已经注册过{0}的路由，请检查当前系统路由配置", routeName));

                    routeDict[routeName] = new ActionResultMetadata() { ControllerName = controller.ControllerName, ControllerType = controller.ControllerType, ActionMethod = action.ActionMethod, AreaName = areaName };
                    var httpMethod = httpMethodProvider.FirstOrDefault(o => o.Name.IndexOf(attribute.HttpMethod, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (httpMethod == null)
                    {
                        action.Selectors.Add(new SelectorModel() { AttributeRouteModel = new AttributeRouteModel(new HttpGetAttribute(routeName)) });
                        continue;
                    }

                    action.Selectors.Add(new SelectorModel() { AttributeRouteModel = new AttributeRouteModel(Activator.CreateInstance(httpMethod, new[] { routeName }) as HttpMethodAttribute) });
                }
            }

            count = true;
        }
    }
}

#else

using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace Never.Web.Mvc.Dispatcher
{
    /// <summary>
    /// 控制器选择
    /// </summary>
    /// <summary>
    /// 自定义ApiControllerType获取
    /// </summary>
    public class CustomHttpControllerSelector : IStartupService, ITypeProcessor
    {
#region field

        /// <summary>
        /// 所以的资源字典
        /// </summary>
        private static readonly IDictionary<string, ActionResultMetadata> routeDict = null;

        /// <summary>
        /// 所有存放controller类型的对象
        /// </summary>
        private static readonly SortedList<string, Type> controllerTypes = null;

        /// <summary>
        /// 所有的区域
        /// </summary>
        private static readonly HashSet<string> areas = null;

        /// <summary>
        /// Action资源标识字典
        /// </summary>
        private static readonly Type actonResultType = typeof(ApiActionRemarkAttribute);

        /// <summary>
        /// api控制器
        /// </summary>
        private static readonly Type apiType = typeof(System.Web.Mvc.IController);

        /// <summary>
        /// 配置集合
        /// </summary>
        protected readonly RouteCollection RouteCollection = null;

#endregion field

#region ctor

        /// <summary>
        ///
        /// </summary>
        static CustomHttpControllerSelector()
        {
            routeDict = new Dictionary<string, ActionResultMetadata>(500);
            areas = new HashSet<string>();
            controllerTypes = new SortedList<string, Type>(200);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomHttpControllerSelector"/> class.
        /// </summary>
        public CustomHttpControllerSelector()
            : this(RouteTable.Routes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomHttpControllerSelector"/> class.
        /// </summary>
        /// <param name="routeCollection">The configuration.</param>
        public CustomHttpControllerSelector(RouteCollection routeCollection)
        {
            this.RouteCollection = routeCollection;
        }

#endregion ctor

#region start

        /// <summary>
        /// Called when [starting].
        /// </summary>
        /// <param name="context">The context.</param>
        public void OnStarting(StartupContext context)
        {
            /*查询所有程序集带有了ActionRemarkAttribute属性的对象*/
            context.ProcessType(this);
        }

        /// <summary>
        /// Processings the specified application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="Never.Exceptions.KeyExistedException"></exception>
        public void Processing(IApplicationStartup application, Type type)
        {
            if (type == null || !apiType.IsAssignableToType(type) || type.IsAbstract || !type.Name.EndsWith("Controller", StringComparison.Ordinal))
                return;

            string controllerName = type.Name.Replace("Controller", "");
            var areaAttributes = type.GetCustomAttributes(typeof(ApiAreaRemarkAttribute), false) as IEnumerable<ApiAreaRemarkAttribute>;
            string areaName = (areaAttributes == null || !areaAttributes.Any()) ? string.Empty : (areaAttributes.FirstOrDefault().Area);
            if (areaName.IsNotNullOrEmpty() && !areas.Contains(areaName))
                areas.Add(areaName);

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (method == null)
                    continue;

                /*把所以当前attributes的都加入来到字典中*/
                var allAttributes = method.GetCustomAttributes(false);
                if (allAttributes != null && allAttributes.Any())
                    ActionBehaviorStorager.Add(method, (from n in allAttributes select (Attribute)n).ToList());

                var attributes = method.GetCustomAttributes(actonResultType, false) as IEnumerable<ApiActionRemarkAttribute>;

                /*这里说明是没有启用自定义方法的，所以这里要加进去到默认的路由*/
                if (attributes == null || !attributes.Any())
                {
                    if (!controllerTypes.ContainsKey(controllerName.ToLower()))
                        controllerTypes[controllerName.ToLower()] = type;

                    continue;
                }

                var attribute = attributes.FirstOrDefault();
                if (attribute.UniqueId.IsNullOrEmpty())
                    continue;

                /*支持区域方法访问*/
                if (!string.IsNullOrEmpty(areaName))
                {
                    var areaController = string.Concat(areaName, ".", controllerName.ToLower(), ".", method.Name.ToLower());
                    if (!routeDict.ContainsKey(areaController))
                        routeDict[areaController] = new ActionResultMetadata() { ControllerName = controllerName, ControllerType = type, ActionMethod = method, AreaName = areaName };
                }

                var routeName = string.Concat(areaName.IsNullOrEmpty() ? "" : (areaName + "/"), attribute.UniqueId);
                string actionName = method.Name;
                if (routeDict.ContainsKey(routeName))
                    throw new KeyExistedException(routeName, string.Format("在api控件器中找到相同的标识,当前控制器和方法为{0}-{1}，已经存在的控制器和方法为{2}-{3}", type.FullName, actionName, routeDict[routeName].ControllerName, routeDict[routeName].ActionMethod.Name));

                if (this.RouteCollection[routeName] != null)
                    throw new KeyExistedException(routeName, string.Format("已经注册过{0}的路由，请检查当前系统路由配置", routeName));

                routeDict[routeName] = new ActionResultMetadata() { ControllerName = controllerName, ControllerType = type, ActionMethod = method, AreaName = areaName };

                this.RouteCollection.Add(routeName,
                    new Route(routeName,
                    new RouteValueDictionary(new { controller = controllerName, action = method.Name }),
                    new RouteValueDictionary(),
                    new RouteValueDictionary(new { namespaces = new[] { type.Namespace } }),
                    new System.Web.Mvc.MvcRouteHandler()));
            }
        }

        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order
        {
            get
            {
                return 100;
            }
        }

#endregion start

#region utils

        /// <summary>
        /// 查找当前请求是否为统一标识请求
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        public virtual string FindRouteFromRequestMessage(RequestContext requestContext)
        {
            return string.Empty;
        }

        /// <summary>
        /// 查找当前请求如果是统一标识请求，则返回控制器名与Action名字
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        public ActionResultMetadata? FindControllerAndActionName(RequestContext requestContext)
        {
            return null;
            //if (!routeDict.Any())
            //    return new ActionResultMetadata();

            //var uniqueId = FindRouteFromRequestMessage(request);
            //if (uniqueId.IsNullOrEmpty() || !routeDict.ContainsKey(uniqueId))
            //    return null;

            //return routeDict[uniqueId];
        }

        ///// <summary>
        ///// 获取当前程序集中 IHttpController反射集合
        ///// </summary>
        ///// <returns></returns>
        //protected ILookup<string, Type> GetMvcControllerTypes()
        //{
        //    IAssembliesResolver assembliesResolver = this.Configuration.Services.GetAssembliesResolver();
        //    return this.Configuration.Services.GetHttpControllerTypeResolver()
        //        .GetControllerTypes(assembliesResolver)
        //        .ToLookup(t => t.Name.ToLower().Substring(0, t.Name.Length - DefaultHttpControllerSelector.ControllerSuffix.Length), t => t);
        //}

#endregion utils
    }
}

#endif