using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Never.DataAnnotations
{
    /// <summary>
    /// 验证对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Validator<T> : IValidator
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="Validator{T}"/> class.
        /// </summary>
        protected Validator()
        {
        }

        #endregion ctor

        #region IValidatior

        /// <summary>
        /// 添加错误
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="express">The express.</param>
        /// <param name="message">The message.</param>
        public static void AddErrors(List<ValidationFailure> result, Expression express, string message)
        {
            var model = express as ParameterExpression;
            if (model != null)
            {
                result.Add(new ValidationFailure() { ErrorMessage = message, MemberName = model.Name });
                return;
            }

            var member = express as MemberExpression;
            if (member != null && member.Member != null)
            {
                result.Add(new ValidationFailure() { ErrorMessage = message, MemberName = member.Member.Name });
                return;
            }

            var unary = express as UnaryExpression;
            if (unary != null)
            {
                var property = unary.Operand as MemberExpression;
                if (property != null)
                {
                    result.Add(new ValidationFailure() { ErrorMessage = message, MemberName = property.Member.Name });
                    return;
                }
            }
        }

        /// <summary>
        /// 获取验证规则
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public abstract IEnumerable<KeyValuePair<Expression<System.Func<T, object>>, string>> RuleFor(T target);

        /// <summary>
        ///
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual ValidationResult Validate(object target)
        {
            if (target is T)
            {
                var data = this.RuleFor((T)target);
                if (data == null)
                    return new ValidationResult();

                var rules = data.ToArray();
                if (rules.IsNullOrEmpty())
                    return new ValidationResult();

                var list = new List<ValidationFailure>();
                foreach (var r in rules)
                    AddErrors(list, r.Key.Body, r.Value);

                return new ValidationResult(list);
            }

            return new ValidationResult();
        }

        #endregion IValidatior
    }
}