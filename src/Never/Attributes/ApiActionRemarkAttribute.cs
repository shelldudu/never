using System;

namespace Never.Attributes
{
    /// <summary>
    /// API资源标识
    /// </summary>
    public class ApiActionRemarkAttribute : Attribute
    {
        #region prop

        /// <summary>
        /// 唯一资源，在api中表示了路由，如果要支持区域，请使用<seealso cref="ApiAreaRemarkAttribute"/>
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// 方法类型，比如Httpet,HttpPost等
        /// </summary>
        public string HttpMethod { get; set; }

        #endregion prop

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiActionRemarkAttribute"/> class.
        /// </summary>
        public ApiActionRemarkAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiActionRemarkAttribute"/> class.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        public ApiActionRemarkAttribute(string uniqueId)
        {
            this.UniqueId = uniqueId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiActionRemarkAttribute"/> class.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <param name="httpMethod"></param>
        public ApiActionRemarkAttribute(string uniqueId, string httpMethod)
        {
            this.UniqueId = uniqueId;
            this.HttpMethod = httpMethod;
        }

        #endregion ctor
    }
}