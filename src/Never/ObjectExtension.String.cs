using System;
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
        #region 加码为unicode

        /// <summary>
        /// 将字符串加码成标准的Unicode字符串
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <returns></returns>
        public static string StringToUnicode(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            string txt = string.Empty;
            byte[] str = Encoding.Unicode.GetBytes(text);
            for (int i = 0; i < str.Length; i = i + 2)
            {
                txt = string.Concat(txt, "\\u", str[i + 1].ToString("x").PadLeft(2, '0'), str[i].ToString("x").PadLeft(2, '0'));
            }

            return txt;
        }

        /// <summary>
        /// 将Uncode字符串解码为字符串
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <returns></returns>
        public static string UnicodeToString(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            string txt = string.Empty;
            MatchCollection ms = Regex.Matches(text, @"\\u([\w]{2})([\w]{2})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            byte[] bys = new byte[2];
            foreach (Match m in ms)
            {
                bys[0] = byte.Parse(m.Groups[2].Value, NumberStyles.HexNumber);
                bys[1] = byte.Parse(m.Groups[1].Value, NumberStyles.HexNumber);
                txt = string.Concat(txt, Encoding.Unicode.GetString(bys));
            }

            return txt;
        }

        #endregion 加码为unicode

        #region 空字符串

        /// <summary>
        /// 某对象是否为空或长度为0
        /// </summary>
        /// <param name="text">尝试的对象</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// 某对象是否为空或长度为0或者空格
        /// </summary>
        /// <param name="text">尝试的对象</param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        /// <summary>
        /// 某对象是否为不为空或长度大于0
        /// </summary>
        /// <param name="text">尝试的对象</param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string text)
        {
            return !string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// 某对象是否为不为空或长度大于0或者非空格
        /// </summary>
        /// <param name="text">尝试的对象</param>
        /// <returns></returns>
        public static bool IsNotNullOrWhiteSpace(this string text)
        {
            return !string.IsNullOrWhiteSpace(text);
        }

        /// <summary>
        /// 两个字符串是否相等
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public static bool IsEquals(this string source, string other)
        {
            return IsEquals(source, other, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// 两个字符串是否相等
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="other">The other.</param>
        /// <param name="comparison">comparison</param>
        /// <returns></returns>
        public static bool IsEquals(this string source, string other, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(source) && string.IsNullOrEmpty(other))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(other))
            {
                return source.Equals(other, comparison);
            }

            return (source ?? "").Equals(other, comparison);
        }

        /// <summary>
        /// 两个字符串是否不相等
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public static bool IsNotEquals(this string source, string other)
        {
            return IsNotEquals(source, other, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// 两个字符串是否不相等
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="other">The other.</param>
        /// <param name="comparison">comparison.</param>
        /// <returns></returns>
        public static bool IsNotEquals(this string source, string other, StringComparison comparison)
        {
            return !IsEquals(source, other, comparison);
        }

        #endregion 空字符串

        #region 长度相关

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="text">源字符串</param>
        /// <param name="begin">开始位置</param>
        /// <returns></returns>
        public static string Sub(this string text, int begin)
        {
            return Sub(text, begin, text.Length - begin);
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="text">源字符串</param>
        /// <param name="begin">开始位置</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string Sub(this string text, int begin, int length)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (length <= 0)
            {
                length = 0;
            }

            if (begin <= 0)
            {
                begin = 0;
            }

            if (text.Length <= begin)
            {
                return string.Empty;
            }

            var last = length == 0 || (text.Length - begin) < length ? text.Length - begin : length;

            return text.Substring(begin, last);
        }

        /// <summary>
        /// 字符串倒序
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string Reverse(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var array = text.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }

        #endregion 长度相关

        #region IP,Email,Http验证

        /// <summary>
        /// 当前的对象是否为IP地址
        /// </summary>
        /// <param name="ip">IP数据</param>
        /// <returns></returns>
        public static bool IsIP(this string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }

            return ipRegex.IsMatch(ip);
        }

        /// <summary>
        /// 当前的对象是否为URi地址
        /// </summary>
        /// <param name="uri">Uri数据</param>
        /// <returns></returns>
        public static bool IsUri(this string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return false;
            }

            return uriRegex.IsMatch(uri);
        }

        /// <summary>
        /// 当前的对象是否为Email地址
        /// </summary>
        /// <param name="email">Email数据</param>
        /// <returns></returns>
        public static bool IsEmail(this string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            return emailRegex.IsMatch(email);
        }

        #endregion IP,Email,Http验证
    }
}