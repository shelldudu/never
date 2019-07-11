using Never.Attributes;
using Never.Exceptions;
using Never.Startups;
using Never.Web.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

#if !NET461
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Never.Web.WebApi.Dispatcher
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

using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Never.Web.WebApi.Dispatcher
{
    /// <summary>
    /// 自定义ApiControllerType获取
    /// </summary>
    public class CustomHttpControllerSelector : DefaultHttpControllerSelector, IHttpControllerSelector, IStartupService, ITypeProcessor
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
        private static readonly Type apiType = typeof(System.Web.Http.Controllers.IHttpController);

        /// <summary>
        /// 配置集合
        /// </summary>
        protected readonly HttpConfiguration Configuration = null;

        /// <summary>
        /// httpGet等集合
        /// </summary>
        private static readonly HashSet<Type> httpMethodProvider = null;

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

            controllerTypes = new SortedList<string, Type>(200);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomHttpControllerSelector"/> class.
        /// </summary>
        public CustomHttpControllerSelector()
            : this(GlobalConfiguration.Configuration)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomHttpControllerSelector"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public CustomHttpControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            this.Configuration = configuration;
        }

#endregion ctor

#region

        /// <summary>
        /// Gets the name of the controller.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public override string GetControllerName(HttpRequestMessage request)
        {
            return base.GetControllerName(request);
        }

        /// <summary>
        /// Selects the controller.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            /*直接找到路由器这种方式了*/
            ActionResultMetadata metadata = default(ActionResultMetadata);
            if (routeDict.TryGetValue((request.RequestUri.AbsolutePath ?? "").ToLower().Trim('/'), out metadata))
            {
                request.Properties["CurrentApiRoute"] = metadata;
                return new HttpControllerDescriptor(this.Configuration, metadata.ControllerName, metadata.ControllerType);
            }

            var route = FindRouteFromRequestMessage(request);
            if (route.IsNotNullOrEmpty() && routeDict.TryGetValue(string.Concat("api/", route), out metadata))
            {
                request.Properties["CurrentApiRoute"] = metadata;
                return new HttpControllerDescriptor(this.Configuration, metadata.ControllerName, metadata.ControllerType);
            }

            var controllerName = (this.GetControllerName(request) ?? "").ToLower();
            /*控制器为空，直接跳出去*/
            if (string.IsNullOrEmpty(controllerName))
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format("No route providing a controller name was found to match request URI '{0}'", request.RequestUri)));

            if (controllerTypes.ContainsKey(controllerName))
                return new HttpControllerDescriptor(this.Configuration, controllerName, controllerTypes[controllerName]);

            /*表示可能是用区域加控制器来访问的*/
            if (!areas.Contains(controllerName) || request.RequestUri.Segments.Length <= 3)
                throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format("No route providing a controller name was found to match request URI '{0}'", request.RequestUri)));

            /*此时要用区域访问*/
            controllerName = string.Concat(controllerName, ".", request.RequestUri.Segments[3], ".", request.RequestUri.Segments.Length > 4 ? request.RequestUri.Segments[4] : "Index").ToLower().Replace("/", "");
            if (routeDict.TryGetValue(controllerName, out metadata))
            {
                request.Properties["CurrentApiRoute"] = metadata;
                return new HttpControllerDescriptor(this.Configuration, metadata.ControllerName, metadata.ControllerType);
            }

            throw new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format("No route providing a controller name was found to match request URI '{0}'", request.RequestUri)));
        }

#endregion

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

                if (allAttributes != null && allAttributes.Any(o => o.GetType() == typeof(NonActionAttribute)))
                    continue;

                var attributes = method.GetCustomAttributes(actonResultType, false) as IEnumerable<ApiActionRemarkAttribute>;

                /*这里说明是没有启用自定义方法的，所以这里要加进去到默认的路由*/
                if (attributes == null || !attributes.Any())
                {
                    if (!controllerTypes.ContainsKey(controllerName.ToLower()))
                        controllerTypes[controllerName.ToLower()] = type;

                    continue;
                }

                if (allAttributes != null && allAttributes.Any(o => httpMethodProvider.Contains(o.GetType())))
                {
                }
                else
                {
                    throw new InvalidException(string.Format("在api控件器中{0}类型中{1}方法没有标识指定某个操作支持 POST HTTP 等特性", type.FullName, method.Name));
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

                var routeName = string.Concat("api/", areaName.IsNullOrEmpty() ? "" : (areaName + "/"), attribute.UniqueId);
                string actionName = method.Name;
                if (routeDict.ContainsKey(routeName))
                    throw new KeyExistedException(routeName, string.Format("在api控件器中找到相同的标识,当前控制器和方法为{0}-{1}，已经存在的控制器和方法为{2}-{3}", type.FullName, actionName, routeDict[routeName].ControllerName, routeDict[routeName].ActionMethod.Name));

                if (this.Configuration.Routes.ContainsKey(routeName))
                    throw new KeyExistedException(routeName, string.Format("已经注册过{0}的路由，请检查当前系统路由配置", routeName));

                routeDict[routeName] = new ActionResultMetadata() { ControllerName = controllerName, ControllerType = type, ActionMethod = method, AreaName = areaName };

                this.Configuration.Routes.MapHttpRoute(
                    name: routeName,
                    routeTemplate: routeName.StartsWith("api/", StringComparison.OrdinalIgnoreCase) ? routeName : string.Concat("api/", routeName, "/{id}"),
                    defaults: new { controller = controllerName, action = method.Name, id = RouteParameter.Optional, namespaces = new[] { type.Namespace } }
                );
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

#endregion

#region utils

        /// <summary>
        /// 查找当前请求是否为统一标识请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual string FindRouteFromRequestMessage(HttpRequestMessage request)
        {
            var controllerTypeName = string.Empty;
            /*带api/area/gid这样的请求，Segments分分割为[/,api/,area/,gid]*/
            var segment = (request.RequestUri.Segments.Length >= 3 ? (request.RequestUri.Segments[2] ?? "") : string.Empty).Trim('/').ToLower();
            if (segment.IsNotNullOrEmpty() && areas.Contains(segment))
                controllerTypeName = string.Concat(segment, "/");

            return string.Empty;
        }

        /// <summary>
        /// 查找当前请求如果是统一标识请求，则返回控制器名与Action名字
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResultMetadata? FindControllerAndActionName(HttpRequestMessage request)
        {
            if (!routeDict.Any())
                return new ActionResultMetadata();

            var uniqueId = FindRouteFromRequestMessage(request);
            if (uniqueId.IsNullOrEmpty() || !routeDict.ContainsKey(uniqueId))
                return null;

            return routeDict[uniqueId];
        }

        /// <summary>
        /// 获取当前程序集中 IHttpController反射集合
        /// </summary>
        /// <returns></returns>
        protected ILookup<string, Type> GetApiControllerTypes()
        {
            IAssembliesResolver assembliesResolver = this.Configuration.Services.GetAssembliesResolver();
            return this.Configuration.Services.GetHttpControllerTypeResolver()
                .GetControllerTypes(assembliesResolver)
                .ToLookup(t => t.Name.ToLower().Substring(0, t.Name.Length - DefaultHttpControllerSelector.ControllerSuffix.Length), t => t);
        }

#endregion
    }
}

#endif