using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Never
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static partial class ObjectExtension
    {
        #region field

        /// <summary>
        /// 中国身份证验证
        /// </summary>
        private static Regex chineseIDRegex = new Regex(@"\d{17}[\d|X|x]|\d{15}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 中国电话验证
        /// </summary>
        private static Regex chineseTelephoneRegex = new Regex("^(0[0-9]{2,3}/-)?([2-9][0-9]{6,7})+(/-[0-9]{1,4})?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 中国移动电话验证
        /// </summary>
        private static Regex chineseMobileRegex = new Regex(@"^1[3456789]\d{9}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 中国邮编验证
        /// </summary>
        private static Regex chineseZipCodeRegex = new Regex(@"\d{6}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 中文星期描述
        /// </summary>
        private static IDictionary<DayOfWeek, string> weekDict = new Dictionary<DayOfWeek, string>(7)
        {
            { DayOfWeek.Friday, "星期五" },
            { DayOfWeek.Monday, "星期一" },
            { DayOfWeek.Saturday, "星期六" },
            { DayOfWeek.Sunday, "星期天" },
            { DayOfWeek.Thursday, "星期四" },
            { DayOfWeek.Tuesday, "星期二" },
            { DayOfWeek.Wednesday, "星期三" }
        };

        /// <summary>
        /// 月份数字对应中文描述
        /// </summary>
        private static IDictionary<int, string> intDescnDict = new Dictionary<int, string>(7)
        {
            { 1, "一" },
            { 2, "二" },
            { 3, "三" },
            { 4, "四" },
            { 5, "五" },
            { 6, "六" },
            { 7, "七" },
            { 8, "八" },
            { 9, "九" },
            { 10, "十" },
            { 11, "十一" },
            { 12, "十二" }
        };
        #endregion

        #region chinese

        /// <summary>
        /// 中文星期描述
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static string ChineseWeekName(this DateTime time)
        {
            return weekDict[time.DayOfWeek];
        }

        /// <summary>
        /// 中文星期描述
        /// </summary>
        /// <param name="dayOfWeek">时间</param>
        /// <returns></returns>
        public static string ChineseWeekName(this DayOfWeek dayOfWeek)
        {
            return weekDict[dayOfWeek];
        }

        /// <summary>
        /// 中文月份描述
        /// </summary>
        /// <param name="time">数字</param>
        /// <returns></returns>
        public static string ChineseMonthName(this DateTime time)
        {
            return intDescnDict[time.Month];
        }

        /// <summary>
        /// 中文月份描述
        /// </summary>
        /// <param name="number">数字</param>
        /// <returns></returns>
        public static string ChineseMonthName(this int number)
        {
            return intDescnDict[number];
        }

        /// <summary>
        /// 引入外部dll本地类
        /// </summary>
        private static class UnsafeNativeMethods
        {
            /// <summary>
            /// The local e_ syste m_ default
            /// </summary>
            internal const int LOCALE_SYSTEM_DEFAULT = 0x0800;

            /// <summary>
            /// The lcma p_ simplifie d_ chinese
            /// </summary>
            internal const int LCMAP_SIMPLIFIED_CHINESE = 0x02000000;

            /// <summary>
            /// The lcma p_ traditiona l_ chinese
            /// </summary>
            internal const int LCMAP_TRADITIONAL_CHINESE = 0x04000000;

            /// <summary>
            /// Lcs the map string.
            /// </summary>
            /// <param name="Locale">The locale.</param>
            /// <param name="dwMapFlags">The dw map flags.</param>
            /// <param name="lpSrcStr">The lp source string.</param>
            /// <param name="cchSrc">The CCH source.</param>
            /// <param name="lpDestStr">The lp dest string.</param>
            /// <param name="cchDest">The CCH dest.</param>
            /// <returns></returns>
            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern int LCMapString(int Locale, int dwMapFlags, string lpSrcStr, int cchSrc, [System.Runtime.InteropServices.Out] string lpDestStr, int cchDest);
        }

        /// <summary>
        /// 将字符转换成简体中文
        /// </summary>
        /// <param name="source">输入要转换的字符串</param>
        /// <returns>转换完成后的字符串</returns>
        public static string ToSimplified(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            string target = new string(' ', source.Length);
            int ret = UnsafeNativeMethods.LCMapString(UnsafeNativeMethods.LOCALE_SYSTEM_DEFAULT, UnsafeNativeMethods.LCMAP_SIMPLIFIED_CHINESE, source, source.Length, target, source.Length);
            return target;
        }

        /// <summary>
        /// 讲字符转换为繁体中文
        /// </summary>
        /// <param name="source">输入要转换的字符串</param>
        /// <returns>转换完成后的字符串</returns>
        public static string ToTraditional(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            string target = new string(' ', source.Length);
            int ret = UnsafeNativeMethods.LCMapString(UnsafeNativeMethods.LOCALE_SYSTEM_DEFAULT, UnsafeNativeMethods.LCMAP_TRADITIONAL_CHINESE, source, source.Length, target, source.Length);
            return target;
        }

        /// <summary>
        /// 是否为汉字
        /// </summary>
        /// <param name="text">源数据</param>
        /// <returns></returns>
        public static bool IsChineseWord(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] >= 0x4e00 && text[i] <= 0x9fbb)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 当前的对象是否为中国身份证
        /// </summary>
        /// <param name="id">中国身份证数据</param>
        /// <returns></returns>
        public static bool IsChineseId(this string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            return IsChineseIdCard(id);
        }

        /// <summary>
        /// 当前的对象是否为中国号码
        /// </summary>
        /// <param name="phone">中国号码数据</param>
        /// <returns></returns>
        public static bool IsChineseTelephone(this string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return false;
            }

            return chineseTelephoneRegex.IsMatch(phone);
        }

        /// <summary>
        /// 当前的对象是否为中国移动号码
        /// </summary>
        /// <param name="mobile">中国移动号码数据</param>
        /// <returns></returns>
        public static bool IsChineseMobile(this string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return false;
            }

            return chineseMobileRegex.IsMatch(mobile);
        }

        /// <summary>
        /// 当前的对象是否为中国邮编
        /// </summary>
        /// <param name="zipCode">中国邮编数据</param>
        /// <returns></returns>
        public static bool IsChineseXZipCode(this string zipCode)
        {
            if (string.IsNullOrEmpty(zipCode))
            {
                return false;
            }

            return chineseZipCodeRegex.IsMatch(zipCode);
        }

        /// <summary>
        /// 判断是否为18位身份证号码
        /// </summary>
        /// <param name="identity">身份证号码</param>
        /// <returns></returns>
        public static bool IsChinese18IdCard(this string identity)
        {
            if (string.IsNullOrEmpty(identity))
            {
                return false;
            }

            long n = 0;
            if (long.TryParse(identity.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(identity.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }

            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(identity.Remove(2)) == -1)
            {
                return false;//省份验证
            }

            string birth = identity.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }

            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = identity.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }

            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != identity.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }

            return true;//符合GB11643-1999标准
        }

        /// <summary>
        /// 判断是否为15位身份证号码
        /// </summary>
        /// <param name="identity">身份证号码</param>
        /// <returns></returns>
        public static bool IsChinese15IdCard(this string identity)
        {
            if (string.IsNullOrEmpty(identity))
            {
                return false;
            }

            long n = 0;
            if (long.TryParse(identity, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }

            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(identity.Remove(2)) == -1)
            {
                return false;//省份验证
            }

            string birth = identity.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }

            return true;//符合15位身份证标准
        }

        /// <summary>
        /// 是否身份证号码格式
        /// </summary>
        /// <param name="identity">身份证号码</param>
        /// <returns></returns>
        public static bool IsChineseIdCard(this string identity)
        {
            if (string.IsNullOrEmpty(identity))
            {
                return false;
            }

            switch (identity.Length)
            {
                case 15:
                    {
                        return IsChinese15IdCard(identity);
                    }
                case 18:
                    {
                        return IsChinese18IdCard(identity);
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>
        /// 从身份证上获取年龄
        /// </summary>
        /// <param name="identity">身份证号码</param>
        /// <returns></returns>
        public static int GetChineseAge(this string identity)
        {
            if (!IsChineseIdCard(identity))
            {
                return -1;
            }

            switch (identity.Length)
            {
                case 15:
                    {
                        return DateTime.Now.Year - Convert.ToDateTime(identity.Substring(6, 6).Insert(4, "-").Insert(2, "-")).Year;
                    }
                case 18:
                    {
                        return DateTime.Now.Year - Convert.ToDateTime(identity.Substring(6, 8).Insert(6, "-").Insert(4, "-")).Year;
                    }
                default:
                    {
                        return -1;
                    }
            }
        }

        /// <summary>
        /// 根据身份证获取性别,返回值为[0未知1男2女]
        /// </summary>
        /// <param name="identity">身份证号码</param>
        /// <returns>男，女，未知</returns>
        public static int GetChineseGender(this string identity)
        {
            if (string.IsNullOrEmpty(identity))
            {
                return 0;
            }

            switch (identity.Length)
            {
                case 15:
                    {
                        return (Convert.ToInt32(identity.Substring(12, 3)) % 2 == 0) ? 2 : 1;
                    }
                case 18:
                    {
                        return (Convert.ToInt32(identity.Substring(14, 3)) % 2 == 0) ? 2 : 1;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }

        /// <summary>
        /// 根据身份证获取生出年月
        /// </summary>
        /// <param name="identity">身份证号码</param>
        public static DateTime? GetChineseBornDate(this string identity)
        {
            if (string.IsNullOrEmpty(identity))
            {
                return null;
            }

            switch (identity.Length)
            {
                case 15:
                    {
                        return Convert.ToDateTime(identity.Substring(6, 6).Insert(4, "-").Insert(2, "-"));
                    }
                case 18:
                    {
                        return Convert.ToDateTime(identity.Substring(6, 8).Insert(6, "-").Insert(4, "-"));
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        #endregion id5
    }
}