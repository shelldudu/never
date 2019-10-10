using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Never.DataAnnotations
{
    /// <summary>
    /// 验证结果
    /// </summary>
    public struct ValidationResult
    {
        #region prop

        /// <summary>
        /// 错误结果集
        /// </summary>
        public IReadOnlyList<ValidationFailure> Errors { get; }

        /// <summary>
        /// 是否通过验证
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (this.Errors == null)
                    return true;

                return this.Errors.Count <= 0;
            }
        }

        #endregion prop

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="errors"></param>
        public ValidationResult(IEnumerable<ValidationFailure> errors)
        {
            this.Errors = errors.AsReadOnly();
        }

        #endregion ctor

        /// <summary>
        /// 成功对象
        /// </summary>
        public static ValidationResult Success { get { return new ValidationResult(); } }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Target"></typeparam>
        /// <param name="target"></param>
        /// <param name="express"></param>
        /// <returns></returns>
        public static ValidationResult Expression<Target>(Target target, Func<Target, IEnumerable<KeyValuePair<Expression<System.Func<Target, object>>, string>>> express)
        {
            var collection = express(target);
            if (collection.IsNullOrEmpty())
                return ValidationResult.Success;

            var list = new List<ValidationFailure>();
            foreach (var r in collection)
                Validator<object>.AddErrors(list, r.Key.Body, r.Value);

            return new ValidationResult(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Target"></typeparam>
        /// <param name="target"></param>
        /// <param name="express"></param>
        /// <returns></returns>
        public static ValidationResult Expression<Target>(Target target, Action<ICollection<KeyValuePair<Expression<System.Func<Target, object>>, string>>, Target> express)
        {
            var collection = new List<KeyValuePair<Expression<System.Func<Target, object>>, string>>();
            express(collection, target);
            if (collection.Count == 0)
                return ValidationResult.Success;

            var list = new List<ValidationFailure>();
            foreach (var r in collection)
                Validator<object>.AddErrors(list, r.Key.Body, r.Value);

            return new ValidationResult(list);
        }

    }
}