using System;
using System.Collections.Generic;
using System.Linq;
using Never.DataAnnotations;

#if !NET461
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Never.Web.Mvc.DataAnnotations
{
    public sealed class ValidatorProvider : Microsoft.AspNetCore.Mvc.ModelBinding.Validation.IModelValidatorProvider
    {
        public void CreateValidators(ModelValidatorProviderContext context)
        {
            var validator = GetValidator(context.ModelMetadata.ModelType);
            if (validator != null)
                context.Results.Add(new ValidatorItem() { Validator = new ValidationModelValidator(validator) });
        }

        /// <summary>
        /// 获取IValidator属性对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IValidator GetValidator(Type type)
        {
            if (type == null)
                return null;

            var attribute = default(ValidatorAttribute);
            if (!TypeProcessor.all.TryGetValue(type, out attribute) || attribute == null)
                return null;

            return System.Activator.CreateInstance(attribute.ValidatorType) as IValidator;
        }
    }

    public sealed class ValidationModelValidator : IModelValidator
    {
        private readonly IValidator validator = null;

        public ValidationModelValidator(IValidator validator)
        {
            this.validator = validator;
        }

        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {
            return this.ValidateModel(context.Model);
        }

        public IEnumerable<ModelValidationResult> ValidateModel(object model)
        {
            if (model == null)
                return Anonymous.NewEnumerable<ModelValidationResult>();

            var validation = this.validator.Validate(model);
            if (validation.IsValid)
                return Anonymous.NewEnumerable<ModelValidationResult>();

            var array = validation.Errors.Select(t => new ModelValidationResult(t.MemberName, t.ErrorMessage) { }).ToArray();
            return array;
        }
    }
}

#else

using System.Web.Mvc;
namespace Never.Web.Mvc.DataAnnotations
{
    /// <summary>
    /// 验证
    /// </summary>
    /// <seealso cref="System.Web.Mvc.ModelValidatorProvider" />
    public sealed class ValidatorProvider : ModelValidatorProvider
    {
        /// <summary>
        /// 获取验证对象
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="context">The context.</param>
        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            var validator = GetValidator(metadata.ModelType);
            if (validator == null)
                yield break;

            yield return new ValidationModelValidator(metadata, context, validator);
        }

        #region IValidator

        /// <summary>
        /// 获取IValidator属性对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IValidator GetValidator(Type type)
        {
            if (type == null)
                return null;

            var attribute = default(ValidatorAttribute);
            if (!TypeProcessor.all.TryGetValue(type, out attribute) || attribute == null)
                return null;

            return System.Activator.CreateInstance(attribute.ValidatorType) as IValidator;
        }

        #endregion IValidator
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Web.Mvc.ModelValidator" />
    public sealed class ValidationModelValidator : ModelValidator
    {
        private readonly IValidator validator = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationModelValidator"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="context">The context.</param>
        /// <param name="validator">The validator.</param>
        public ValidationModelValidator(ModelMetadata metadata, ControllerContext context, IValidator validator) : base(metadata, context)
        {
            this.validator = validator;
        }

        /// <summary>
        /// When implemented in a derived class, validates the object.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>
        /// A list of validation results.
        /// </returns>
        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            return this.ValidateModel(this.Metadata.Model);
        }
        /// <summary>
        /// When implemented in a derived class, validates the object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// A list of validation results.
        /// </returns>
        public IEnumerable<ModelValidationResult> ValidateModel(object model)
        {
            var data = this.validator.Validate(model);
            if (data.IsValid)
                return Anonymous.NewEnumerable<ModelValidationResult>();

            return data.Errors.Select(t => new ModelValidationResult() { MemberName = t.MemberName, Message = t.ErrorMessage });
        }
    }
}
#endif