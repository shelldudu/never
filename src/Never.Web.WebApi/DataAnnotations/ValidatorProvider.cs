using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if !NET461
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
#else

using System.Web.Http.Metadata;
using System.Web.Http.Validation;

#endif

using Never.Aop;
using Never.DataAnnotations;
using Never.IoC;

namespace Never.Web.WebApi.DataAnnotations
{
#if !NET461
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
#else

    /// <summary>
    /// 验证
    /// </summary>
    /// <seealso cref="System.Web.Http.Validation.ModelValidatorProvider" />
    public sealed class ValidatorProvider : System.Web.Http.Validation.ModelValidatorProvider
    {
        /// <summary>
        /// Gets a list of validators associated with this <see cref="T:System.Web.Http.Validation.ModelValidatorProvider" />.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="validatorProviders">The validator providers.</param>
        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders)
        {
            var validator = GetValidator(metadata.ModelType);
            if (validator == null)
                yield break;

            yield return new ValidationModelValidator(validatorProviders, validator);
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

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Web.Http.Validation.ModelValidator" />
    public sealed class ValidationModelValidator : ModelValidator
    {
        private readonly IValidator validator = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationModelValidator"/> class.
        /// </summary>
        /// <param name="validatorProviders">The validator providers.</param>
        /// <param name="validator">The validator.</param>
        public ValidationModelValidator(IEnumerable<ModelValidatorProvider> validatorProviders, IValidator validator) : base(validatorProviders)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Validates a specified object.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="container">The container.</param>
        public override IEnumerable<ModelValidationResult> Validate(ModelMetadata metadata, object container)
        {
            return this.ValidateModel(metadata.Model);
        }

        /// <summary>
        /// Validates a specified object.
        /// </summary>
        /// <param name="model">The model.</param>
        public IEnumerable<ModelValidationResult> ValidateModel(object model)
        {
            var data = validator.Validate(model);
            if (data.IsValid)
                return Anonymous.NewEnumerable<ModelValidationResult>();

            return data.Errors.Select(t => new ModelValidationResult() { MemberName = t.MemberName, Message = t.ErrorMessage });
        }
    }

#endif
}