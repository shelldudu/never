using System;

namespace Never.Serialization.Json
{
    /// <summary>
    /// 时间格式化
    /// </summary>
    public struct DateTimeFormat : IEquatable<DateTimeFormat>
    {
        #region guid

        /// <summary>
        /// 唯一标识
        /// </summary>
        public byte UniqueId { get; private set; }

        #endregion guid

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="uniqueId"></param>
        public DateTimeFormat(byte uniqueId)
        {
            this.UniqueId = uniqueId;
        }

        #endregion ctor

        #region IEquatable

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DateTimeFormat other)
        {
            return this.UniqueId == other.UniqueId;
        }

        /// <summary>
        /// DateTimes will be formatted as localString
        /// </summary>
        public static DateTimeFormat Default { get; } = new DateTimeFormat(0);

        /// <summary>
        /// DateTimes will be formatted as "yyyy-MM-ddThh:mm:ssZ"
        ///
        /// Examples:
        ///     2012-07-14T11:41:27Z
        ///     2012-09-02T23:14:25Z
        /// </summary>
        public static DateTimeFormat ISO8601Style { get; } = new DateTimeFormat(1);

        /// <summary>
        /// DateTimes will be formatted as "ddd, dd MMM yyyy HH:mm:ss GMT" where
        /// Examples:
        ///     Thu, 10 Apr 2003 13:30:50 GMT
        ///     Tue, 10 Mar 2025 00:16:34 GMT
        /// </summary>
        public static DateTimeFormat RFC1123Style { get; } = new DateTimeFormat(2);

        /// <summary>
        /// DateTimes will be formatted as "\/Date(##...##)\/" where ##...## is the
        /// number of milliseconds since the unix epoch (January 1st, 1970 UTC).
        /// Example:
        ///     "\/Date(628318530718)\/"
        /// </summary>
        public static DateTimeFormat MicrosoftStyle { get; } = new DateTimeFormat(3);

        /// <summary>
        /// DateTimes will be formatted as "yyyy-MM-dd hh:mm:ss"
        /// Examples:
        ///     2011-07-14 19:43:37
        ///     2012-01-02 03:04:05
        /// </summary>
        public static DateTimeFormat ChineseStyle { get; } = new DateTimeFormat(4);

        #endregion IEquatable
    }
}