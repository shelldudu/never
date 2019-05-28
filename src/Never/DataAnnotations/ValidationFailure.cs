namespace Never.DataAnnotations
{
    /// <summary>
    /// 错误信息
    /// </summary>
    public struct ValidationFailure : IValidationOption
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 成员名称
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 结果选项
        /// </summary>
        public ValidationOption Option { get; set; }

        /// <summary>
        /// ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(this.MemberName, "_", this.ErrorMessage);
        }
    }
}