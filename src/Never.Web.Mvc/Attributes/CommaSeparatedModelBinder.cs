using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

#if !NET461

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Never.Web.Mvc.Attributes
{
    public class CommaSeparatedModelBinder : IModelBinder
    {

        /// <summary>
        ///
        /// </summary>
        private static readonly MethodInfo ToArrayMethod = typeof(Enumerable).GetMethod("ToArray");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelMetadata.ModelType == null)
                return null;

            if (bindingContext.ModelMetadata.ModelType.GetInterface(typeof(IEnumerable).Name) == null)
                return null;

            var modelValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (modelValue == null)
                return null;

            var valueType = bindingContext.ModelMetadata.ModelType.GetElementType() ?? bindingContext.ModelMetadata.ModelType.GetGenericArguments().FirstOrDefault();
            if (valueType == null)
                return null;

            var result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(valueType));
            if (valueType == typeof(Guid))
            {
                foreach (var splitValue in (modelValue.FirstValue ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrWhiteSpace(splitValue))
                        result.Add(Guid.Parse(splitValue));
                }

                if (bindingContext.ModelMetadata.IsEnumerableType)
                    bindingContext.Result = ModelBindingResult.Success(ToArrayMethod.MakeGenericMethod(valueType).Invoke(this, new[] { result }));
                else
                    bindingContext.Result = ModelBindingResult.Success(result);

                return Task.CompletedTask;
            }

            if (valueType.GetInterface(typeof(IConvertible).Name) == null)
                return null;

            foreach (var splitValue in (modelValue.FirstValue ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!string.IsNullOrWhiteSpace(splitValue))
                    result.Add(Convert.ChangeType(splitValue, valueType));
            }

            if (bindingContext.ModelMetadata.IsEnumerableType)
                bindingContext.Result = ModelBindingResult.Success(ToArrayMethod.MakeGenericMethod(valueType).Invoke(this, new[] { result }));
            else
                bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}

#else

using System.Web.Mvc;

namespace Never.Web.Mvc.Attributes
{
    /// <summary>
    /// 逗号分割绑定类型
    /// </summary>
    public sealed class CommaSeparatedModelBinder : System.Web.Mvc.DefaultModelBinder
    {
        private static readonly MethodInfo ToArrayMethod = typeof(Enumerable).GetMethod("ToArray");

        /// <summary>
        /// 
        /// </summary>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == null)
                return base.BindModel(controllerContext, bindingContext);

            if (bindingContext.ModelType.GetInterface(typeof(IEnumerable).Name) == null)
                return base.BindModel(controllerContext, bindingContext);

            var modelValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (modelValue == null)
                return base.BindModel(controllerContext, bindingContext);

            var valueType = bindingContext.ModelType.GetElementType() ?? bindingContext.ModelType.GetGenericArguments().FirstOrDefault();
            if (valueType == null)
                return base.BindModel(controllerContext, bindingContext);

            var result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(valueType));
            if (valueType == typeof(Guid))
            {
                foreach (var splitValue in (modelValue.AttemptedValue ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrWhiteSpace(splitValue))
                        result.Add(Guid.Parse(splitValue));
                }

                return bindingContext.ModelType.IsArray ? ToArrayMethod.MakeGenericMethod(valueType).Invoke(this, new[] { result }) : result;
            }

            if (valueType.GetInterface(typeof(IConvertible).Name) == null)
                return null;

            foreach (var splitValue in (modelValue.AttemptedValue ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!string.IsNullOrWhiteSpace(splitValue))
                    result.Add(Convert.ChangeType(splitValue, valueType));
            }

            return bindingContext.ModelType.IsArray ? ToArrayMethod.MakeGenericMethod(valueType).Invoke(this, new[] { result }) : result;
        }
    }
}

#endif



