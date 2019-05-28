using System.Collections.Generic;

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
    }
}