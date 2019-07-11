#if !NET461
#else

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Never.Web.WebApi.Dispatcher
{
    /// <summary>
    /// 自定义ApiActionType获取
    /// </summary>
    public class CustomHttpActionSelector : ApiControllerActionSelector, IHttpActionSelector
    {
        #region field

        /// <summary>
        /// The custom HTTP controller selector
        /// </summary>
        private readonly CustomHttpControllerSelector customHttpControllerSelector = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomHttpActionSelector"/> class.
        /// </summary>
        /// <param name="customHttpControllerSelector">The custom HTTP controller selector.</param>
        public CustomHttpActionSelector(CustomHttpControllerSelector customHttpControllerSelector)
        {
            this.customHttpControllerSelector = customHttpControllerSelector;
        }

        #endregion ctor

        /// <summary>
        /// Selects the action.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <returns></returns>
        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            ActionResultMetadata? item = null;
            if (controllerContext.Request.Properties.ContainsKey("CurrentApiRoute"))
                item = controllerContext.Request.Properties["CurrentApiRoute"] as ActionResultMetadata?;
            else
                item = customHttpControllerSelector.FindControllerAndActionName(controllerContext.Request);

            if (!item.HasValue || item.Value.ControllerName.IsNullOrEmpty() || item.Value.ActionMethod == null)
                return base.SelectAction(controllerContext);

            var attributes = ActionBehaviorStorager.GetAttributes(item.Value.ActionMethod);
            /*当为空的时候，则直接返回*/
            if (attributes == null || !attributes.Any())
                return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, item.Value.ActionMethod);

            /*匿名方法直接返回*/
            if (attributes.GetAttribute<AllowAnonymousAttribute>() != null)
                return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, item.Value.ActionMethod);

            /*遍历是否带上各种attribute*/
            switch ((controllerContext.Request.Method.Method ?? "get").ToLower())
            {
                case "get":
                    {
                        if (attributes.GetAttribute<HttpGetAttribute>() != null)
                            return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, item.Value.ActionMethod);
                    }
                    break;

                case "head":
                    {
                        if (attributes.GetAttribute<HttpHeadAttribute>() != null)
                            return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, item.Value.ActionMethod);
                    }
                    break;

                case "options":
                    {
                        if (attributes.GetAttribute<HttpOptionsAttribute>() != null)
                            return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, item.Value.ActionMethod);
                    }
                    break;

                case "post":
                    {
                        if (attributes.GetAttribute<HttpPostAttribute>() != null)
                            return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, item.Value.ActionMethod);
                    }
                    break;

                case "put":
                    {
                        if (attributes.GetAttribute<HttpPutAttribute>() != null)
                            return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, item.Value.ActionMethod);
                    }
                    break;

                case "trace":
                    {
                        if (attributes.GetAttribute<HttpPatchAttribute>() != null)
                            return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, item.Value.ActionMethod);
                    }
                    break;

                case "delete":
                    {
                        if (attributes.GetAttribute<HttpDeleteAttribute>() != null)
                            return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, item.Value.ActionMethod);
                    }
                    break;
            }

            throw new HttpResponseException(controllerContext.Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, string.Format("The requested resource does not support http method {0}", controllerContext.Request.Method.Method)));
        }
    }
}

#endif