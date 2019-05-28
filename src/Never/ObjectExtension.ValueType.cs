using System;
using System.Collections.Generic;
using System.Linq;

namespace Never
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static partial class ObjectExtension
    {
        #region enum

        /// <summary>
        /// 尝试将对象转制为枚举类型
        /// </summary>
        /// <typeparam name="T">枚举对象</typeparam>
        /// <param name="object">源数据</param>
        /// <returns></returns>
        public static T AsEnum<T>(this string @object) where T : struct, IConvertible
        {
            return AsEnum(@object, default(T));
        }

        /// <summary>
        /// 尝试将对象转制为枚举类型
        /// </summary>
        /// <typeparam name="T">枚举对象</typeparam>
        /// <param name="object">源数据</param>
        /// <param name="defaultItem">默认值</param>
        /// <returns></returns>
        public static T AsEnum<T>(this string @object, T defaultItem) where T : struct, IConvertible
        {
            if (!Enum.TryParse<T>(@object, out T result))
            {
                return defaultItem;
            }

            return result;
        }

        #endregion enum

        #region normal

        /// <summary>
        /// 将对象尝试改为一个short的值
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <returns></returns>
        public static short AsShort(this string value)
        {
            return AsShort(value, 0);
        }

        /// <summary>
        /// 将对象尝试改为一个short的值
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <param name="defaultValue">转制不成功的默认值</param>
        /// <returns></returns>
        public static short AsShort(this string value, short defaultValue)
        {
            if (!short.TryParse(value, out short temp))
            {
                temp = defaultValue;
            }

            return temp;
        }

        /// <summary>
        /// 将对象尝试改为一个byte的值
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <returns></returns>
        public static byte AsByte(this string value)
        {
            return AsByte(value, 0);
        }

        /// <summary>
        /// 将对象尝试改为一个byte的值
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <param name="defaultValue">转制不成功的默认值</param>
        /// <returns></returns>
        public static byte AsByte(this string value, byte defaultValue)
        {
            if (!byte.TryParse(value, out byte temp))
            {
                temp = defaultValue;
            }

            return temp;
        }

        /// <summary>
        /// 将对象尝试改为一个int的值
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <returns></returns>
        public static int AsInt(this string value)
        {
            return AsInt(value, 0);
        }

        /// <summary>
        /// 将对象尝试改为一个int的值
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <param name="defaultValue">转制不成功的默认值</param>
        /// <returns></returns>
        public static int AsInt(this string value, int defaultValue)
        {
            if (!int.TryParse(value, out int temp))
            {
                temp = defaultValue;
            }

            return temp;
        }

        /// <summary>
        /// 将对象尝试改为一个long的值
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <returns></returns>
        public static long AsLong(this string value)
        {
            return AsLong(value, 0);
        }

        /// <summary>
        /// 将对象尝试改为一个long的值
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <param name="defaultValue">转制不成功的默认值</param>
        /// <returns></returns>
        public static long AsLong(this string value, long defaultValue)
        {
            if (!long.TryParse(value, out long temp))
            {
                temp = defaultValue;
            }

            return temp;
        }

        /// <summary>
        /// 将对象尝试改为一个float的值
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <returns></returns>
        public static float AsFloat(this string value)
        {
            return AsFloat(value, 0);
        }

        /// <summary>
        /// 将对象尝试改为一个float的值
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <param name="defaultValue">转制不成功的默认值</param>
        /// <returns></returns>
        public static float AsFloat(this string value, float defaultValue)
        {
            if (!float.TryParse(value, out float temp))
            {
                temp = defaultValue;
            }

            return temp;
        }

        /// <summary>
        /// 将对象尝试改为一个long的值
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <returns></returns>
        public static double AsDouble(this string value)
        {
            return AsDouble(value, 0);
        }

        /// <summary>
        /// 将对象尝试改为一个long的值
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <param name="defaultValue">转制不成功的默认值</param>
        /// <returns></returns>
        public static double AsDouble(this string value, double defaultValue)
        {
            if (!double.TryParse(value, out double temp))
            {
                temp = defaultValue;
            }

            return temp;
        }

        /// <summary>
        /// 尝试把某一对象转制为bool类型，如果obj为0或'false'表示为false,
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <returns></returns>
        public static bool AsBool(this string value)
        {
            return AsBool(value, false);
        }

        /// <summary>
        /// 尝试把某一对象转制为bool类型，如果obj为0或'false'表示为false,
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static bool AsBool(this string value, bool defaultValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            if (bool.TryParse(value, out bool success))
            {
                return success;
            }

            if (value.Equals("1") || value.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return defaultValue;
        }

        /// <summary>
        /// 尝试把某一对象转制为时间类型
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <returns></returns>
        public static DateTime AsDateTime(this string value)
        {
            return AsDateTime(value, DateTime.MinValue);
        }

        /// <summary>
        /// 尝试把某一对象转制为时间类型
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static DateTime AsDateTime(this string value, DateTime defaultValue)
        {
            DateTime time = DateTime.MinValue;
            if (!DateTime.TryParse(value, out time))
            {
                time = defaultValue;
            }

            return time;
        }

        /// <summary>
        /// 尝试把某一对象转制为十进制类型
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <returns></returns>
        public static decimal AsDecimal(this string value)
        {
            return AsDecimal(value, 0);
        }

        /// <summary>
        /// 尝试把某一对象转制为十进制类型
        /// </summary>
        /// <param name="value">尝试的对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static decimal AsDecimal(this string value, decimal defaultValue)
        {
            if (!decimal.TryParse(value, out decimal temp))
            {
                temp = defaultValue;
            }

            return temp;
        }

        /// <summary>
        /// 是否在某个区域
        /// </summary>
        /// <param name="item">某个值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static bool IsBetween(this int item, int min, int max)
        {
            return min <= item && max >= item;
        }

        /// <summary>
        /// 是否在某个区域
        /// </summary>
        /// <param name="item">某个值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static bool IsBetween(this float item, float min, float max)
        {
            return min <= item && max >= item;
        }

        /// <summary>
        /// 是否在某个区域
        /// </summary>
        /// <param name="item">某个值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static bool IsBetween(this DateTime item, DateTime min, DateTime max)
        {
            return min <= item && max >= item;
        }

        /// <summary>
        /// 是否在某个区域
        /// </summary>
        /// <param name="item">某个值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static bool IsBetween(this double item, double min, double max)
        {
            return min <= item && max >= item;
        }

        /// <summary>
        /// 是否在某个区域
        /// </summary>
        /// <param name="item">某个值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static bool IsBetween(this byte item, byte min, byte max)
        {
            return min <= item && max >= item;
        }

        /// <summary>
        /// 是否在某个区域
        /// </summary>
        /// <param name="item">某个值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static bool IsBetween(this decimal item, decimal min, decimal max)
        {
            return min <= item && max >= item;
        }

        /// <summary>
        /// 是否在某个区域
        /// </summary>
        /// <param name="item">某个值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static bool IsBetween(this long item, long min, long max)
        {
            return min <= item && max >= item;
        }

        /// <summary>
        /// 格式化金额，【不进行四舍五入，小数点n位后截取】，小数点不够n位的，用0补上，比如1.23895，,如果n为0返回1，n为1返回1.2,n为2返回1.23，n为3返回1.238
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        public static decimal FormatC(this decimal amount, int n)
        {
            /*不合法数据*/
            if (n < 0)
            {
                return amount;
            }

            var str = amount.ToString();
            if (n == 0)
            {
                return AsDecimal(Sub(str, 0, str.IndexOf('.')));
            }

            if (str.IndexOf('.') < 0)
            {
                str = string.Concat(str, ".");
            }

            var intr = Sub(str, 0, str.IndexOf('.'));
            str = Sub(Sub(str, str.IndexOf('.') + 1), 0, n);
            /*要补0*/
            if (str.Length < n)
            {
                for (var i = str.Length; i < n; i++)
                {
                    str = string.Concat(str, "0");
                }
            }

            return AsDecimal(string.Concat(intr.ToString(), ".", str));
        }

        /// <summary>
        /// 格式化金额，【进行四舍五入中如果下一位大于等于5，则进行舍入】，小数点不够n位的，用0补上，比如1.23895，如果n为0返回1，n为1返回1.2,n为2返回1.24，n为3返回1.239
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        public static decimal FormatR(this decimal amount, int n)
        {
            /*不合法数据*/
            if (n < 0)
            {
                return amount;
            }

            string _100st = "1";
            for (var i = 1; i < n; i++)
            {
                _100st = string.Concat(_100st, "0");
            }

            var unit = 0.1m / (long.Parse(_100st));
            var rm = amount % unit;
            var rs = amount - rm;
            if (rm >= unit / 2)
            {
                rs += unit;
            }

            var str = rs.ToString();
            if (n == 0)
            {
                return AsDecimal(Sub(str, 0, str.IndexOf('.')));
            }

            if (str.IndexOf('.') < 0)
            {
                str = string.Concat(str, ".");
            }

            var intr = Sub(str, 0, str.IndexOf('.'));
            str = Sub(Sub(str, str.IndexOf('.') + 1), 0, n);
            /*要补0*/
            if (str.Length < n)
            {
                for (var i = str.Length; i < n; i++)
                {
                    str = string.Concat(str, "0");
                }
            }

            return AsDecimal(string.Concat(intr.ToString(), ".", str));
        }

        /// <summary>
        /// 格式化金额，【四舍五入中如果下一位不为0，则进行舍入】，小数点不够n位的，用0补上，比如1.23895，如果n为0返回1，n为1返回1.2,n为2返回1.24，n为3返回1.239
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        public static decimal FormatRN(this decimal amount, int n)
        {
            /*不合法数据*/
            if (n < 0)
            {
                return amount;
            }

            string _100st = "1";
            for (var i = 1; i < n; i++)
            {
                _100st = string.Concat(_100st, "0");
            }

            var unit = 0.1m / (long.Parse(_100st));
            var rm = amount % unit;
            var rs = amount - rm;
            if (rm > unit / 10)
            {
                rs += unit;
            }

            var str = rs.ToString();
            if (n == 0)
            {
                return AsDecimal(Sub(str, 0, str.IndexOf('.')));
            }

            if (str.IndexOf('.') < 0)
            {
                str = string.Concat(str, ".");
            }

            var intr = Sub(str, 0, str.IndexOf('.'));
            str = Sub(Sub(str, str.IndexOf('.') + 1), 0, n);
            /*要补0*/
            if (str.Length < n)
            {
                for (var i = str.Length; i < n; i++)
                {
                    str = string.Concat(str, "0");
                }
            }

            return AsDecimal(string.Concat(intr.ToString(), ".", str));
        }

        /// <summary>
        /// 格式化金额，【四舍五入中只要后面不为0，则总是舍入】，小数点不够n位的，用0补上，比如1.23895,如果n为0返回2，n为1返回1.3,n为2返回1.24，n为3返回1.239
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        public static decimal FormatRA(this decimal amount, int n)
        {
            /*不合法数据*/
            if (n < 0)
            {
                return amount;
            }

            string _100st = "1";
            for (var i = 1; i < n; i++)
            {
                _100st = string.Concat(_100st, "0");
            }

            var unit = 0.1m / (long.Parse(_100st));
            var rm = amount % unit;
            var rs = amount - rm;
            var str = rm == 0 ? rs.ToString() : (rs + unit).ToString();
            if (n == 0)
            {
                return AsDecimal(Sub(str, 0, str.IndexOf('.')));
            }

            if (str.IndexOf('.') < 0)
            {
                str = string.Concat(str, ".");
            }

            var intr = Sub(str, 0, str.IndexOf('.'));
            str = Sub(Sub(str, str.IndexOf('.') + 1), 0, n);

            /*要补0*/
            if (str.Length < n)
            {
                for (var i = str.Length; i < n; i++)
                {
                    str = string.Concat(str, "0");
                }
            }

            return AsDecimal(string.Concat(intr.ToString(), ".", str));
        }

        /// <summary>
        /// 转换为Guid
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid AsGuid(this string value)
        {
            return AsGuid(value, Guid.Empty);
        }

        /// <summary>
        /// 转换为Guid
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Guid AsGuid(this string value, Guid defaultValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            Guid result = Guid.Empty;
            if (!Guid.TryParse(value, out result))
            {
                return defaultValue;
            }

            return result;
        }

        #endregion normal

        #region 转换为char

        /// <summary>
        /// 将对象尝试改为一个char的值
        /// </summary>
        /// <param name="value">对象</param>
        /// <returns></returns>
        public static char AsChar(this string value)
        {
            return AsChar(value, ' ');
        }

        /// <summary>
        /// 将对象尝试改为一个char的值
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="defaultValue">转制不成功的默认值</param>
        /// <returns></returns>
        public static char AsChar(this string value, char defaultValue)
        {
            if (!char.TryParse(value, out char temp))
            {
                temp = defaultValue;
            }

            return temp;
        }

        #endregion 转换为char

        #region guid

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="guid">guid值</param>
        /// <returns></returns>
        public static bool IsEmpty(this Guid guid)
        {
            return guid == Guid.Empty;
        }

        #endregion guid

        #region 小数点个数

        /// <summary>
        /// 得到分割信息
        /// </summary>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        public static DigitSplit<decimal> Split(this decimal amount)
        {
            var @string = amount.ToString();
            var @point = @string.LastIndexOf('.');
            if (@point <= 0)
            {
                return new DigitSplit<decimal>()
                {
                    Left = amount,
                    LeftLength = @string.Length,
                    Right = 0m,
                    RightLength = 0,
                    TotalLength = @string.Length,
                    SplitLength = 0,
                };
            }

            var @sub = @string.Sub(point + 1);
            var @right = decimal.Parse(string.Concat("0.", @sub));
            var @leftLength = @string.Length - (@sub.Length + 1);
            return new DigitSplit<decimal>()
            {
                Left = decimal.Parse(@string.Substring(0, @leftLength)),
                LeftLength = @leftLength,
                Right = @right,
                RightLength = @sub.Length,
                TotalLength = @string.Length,
                SplitLength = 1,
            };
        }

        /// <summary>
        /// 得到分割信息
        /// </summary>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        public static DigitSplit<float> Split(this float amount)
        {
            var @string = amount.ToString();
            var @point = @string.LastIndexOf('.');
            if (@point <= 0)
            {
                return new DigitSplit<float>()
                {
                    Left = amount,
                    LeftLength = @string.Length,
                    Right = 0F,
                    RightLength = 0,
                    TotalLength = @string.Length,
                    SplitLength = 0,
                };
            }

            var @sub = @string.Sub(point + 1);
            var @right = float.Parse(string.Concat("0.", @sub));
            var @leftLength = @string.Length - (@sub.Length + 1);
            return new DigitSplit<float>()
            {
                Left = float.Parse(@string.Substring(0, @leftLength)),
                LeftLength = @leftLength,
                Right = @right,
                RightLength = @sub.Length,
                TotalLength = @string.Length,
                SplitLength = 1,
            };
        }

        /// <summary>
        /// 得到分割信息
        /// </summary>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        public static DigitSplit<double> Split(this double amount)
        {
            var @string = amount.ToString();
            var @point = @string.LastIndexOf('.');
            if (@point <= 0)
            {
                return new DigitSplit<double>()
                {
                    Left = amount,
                    LeftLength = @string.Length,
                    Right = 0D,
                    RightLength = 0,
                    TotalLength = @string.Length,
                    SplitLength = 0,
                };
            }

            var @sub = @string.Sub(point + 1);
            var @right = double.Parse(string.Concat("0.", @sub));
            var @leftLength = @string.Length - (@sub.Length + 1);
            return new DigitSplit<double>()
            {
                Left = double.Parse(@string.Substring(0, @leftLength)),
                LeftLength = @leftLength,
                Right = @right,
                RightLength = @sub.Length,
                TotalLength = @string.Length,
                SplitLength = 1,
            };
        }
        #endregion

        #region byte

        /// <summary>
        /// 将其合并成新的数组
        /// </summary>
        public static T[] Combine<T>(this IEnumerable<T[]> arrays)
        {
            var buffer = new T[arrays.Sum(x => x.Length)];
            int offset = 0;
            foreach (var data in arrays)
            {
                Buffer.BlockCopy(data, 0, buffer, offset, data.Length);
                offset += data.Length;
            }
            return buffer;
        }

        /// <summary>
        /// 将其合并成新的数组
        /// </summary>
        public static T[] Combine<T>(params T[][] arrays)
        {
            return arrays.Combine();
        }

        /// <summary>
        /// 将结构体变成byte[]数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="struct"></param>
        /// <returns></returns>
        public static byte[] ToByte<T>(this T @struct) where T : struct
        {
            var size = System.Runtime.InteropServices.Marshal.SizeOf(@struct);
            var @byte = new byte[size];
            var @intptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
            System.Runtime.InteropServices.Marshal.StructureToPtr(@struct, @intptr, false);
            System.Runtime.InteropServices.Marshal.Copy(intptr, @byte, 0, size);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(intptr);
            return @byte;
        }

        /// <summary>
        /// 将byte[变成]结构体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="byte"></param>
        /// <param name="struct"></param>
        /// <returns></returns>
        public static bool TryFromByte<T>(this byte[] @byte, out T @struct) where T : struct
        {
            var size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
            if (size > @byte.Length)
            {
                @struct = default(T);
                return false;
            }

            var @intptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
            System.Runtime.InteropServices.Marshal.Copy(@byte, 0, @intptr, size);
            @struct = System.Runtime.InteropServices.Marshal.PtrToStructure<T>(@intptr);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(intptr);
            return true;
        }
        #endregion
    }
}