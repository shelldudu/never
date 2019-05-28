using System;
using System.Collections.Generic;

namespace Never.Serialization.Json
{
    /// <summary>
    /// 对象与流内容操作的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ConvertMethodProvider<T> : IConvertMethodProvider<T>
    {
        #region field

        /// <summary>
        /// 10的倍数
        /// </summary>
        protected static KeyValuePair<int, ulong>[] TenPloidy = null;

        /// <summary>
        /// 0-100的数字
        /// </summary>
        protected static readonly KeyValuePair<char, char>[] ZeroToHundred = null;

        #endregion field

        #region ctor

        static ConvertMethodProvider()
        {
            TenPloidy = new KeyValuePair<int, ulong>[20];
            for (var i = 0; i < 20; i++)
                TenPloidy[i] = new KeyValuePair<int, ulong>(i, (ulong)Math.Pow(10, i));

            ZeroToHundred = new KeyValuePair<char, char>[101];
            for (var i = 0; i <= 100; i++)
                ZeroToHundred[i] = new KeyValuePair<char, char>((char)('0' + (i / 10)), (char)+('0' + (i % 10)));
        }

        #endregion ctor

        #region IMethodProvider

        /// <summary>
        /// 在流中读取字节后转换为对象
        /// </summary>
        /// <param name="setting">配置项</param>
        /// <param name="reader">字符读取器</param>
        /// <param name="node">节点流内容</param>
        /// <param name="checkNullValue">是否检查空值</param>
        /// <returns></returns>
        public abstract T Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue);

        /// <summary>
        /// 从对象中写入流中
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="setting">配置项</param>
        /// <param name="source">源数据</param>
        public abstract void Write(ISerializerWriter writer, JsonSerializeSetting setting, T source);

        #endregion IMethodProvider

        #region utils

        /// <summary>
        /// 转换为char数组
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="object"></param>
        /// <returns></returns>
        public char[] ToCharEnumerator<TValue>(TValue @object) where TValue : struct, IConvertible
        {
            return this.ToCharEnumerator(@object.ToString());
        }

        /// <summary>
        /// 转换为char数组
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public char[] ToCharEnumerator(string source)
        {
            if (string.IsNullOrEmpty(source))
                return new char[0];

            var buffer = new char[source.Length];
            for (var i = 0; i < source.Length; i++)
                buffer[i] = source[i];

            return buffer;
        }

        /// <summary>
        /// 转换为char数组
        /// </summary>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <returns></returns>
        public char[] ToCharEnumerator(string source1, string source2)
        {
            if (string.IsNullOrEmpty(source1))
            {
                if (string.IsNullOrEmpty(source2))
                    return new char[0];

                var buffer1 = new char[source2.Length];
                for (var i = 0; i < source2.Length; i++)
                    buffer1[i] = source2[i];

                return buffer1;
            }

            if (string.IsNullOrEmpty(source2))
            {
                var buffer1 = new char[source1.Length];
                for (var i = 0; i < source1.Length; i++)
                    buffer1[i] = source1[i];

                return buffer1;
            }

            var buffer = new char[source1.Length + source2.Length];
            for (var i = 0; i < source1.Length; i++)
                buffer[i] = source1[i];

            for (var i = 0; i < source2.Length; i++)
                buffer[i + source1.Length] = source2[i];

            return buffer;
        }

        #endregion utils

        #region datetime

        /// <summary>
        ///
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public char[] GetYearCharArray(DateTime time)
        {
            return this.ToCharEnumerator(time.Year);
        }

        #endregion datetime

        #region digit

        /// <summary>
        /// 获取从0到9之间的数值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static char GetDigitCharInZeroToNight(int value)
        {
            switch (value)
            {
                case 0:
                    return '0';

                case 1:
                    return '1';

                case 2:
                    return '2';

                case 3:
                    return '3';

                case 4:
                    return '4';

                case 5:
                    return '5';

                case 6:
                    return '6';

                case 7:
                    return '7';

                case 8:
                    return '8';

                case 9:
                    return '9';
            }

            return ' ';
        }

        /// <summary>
        /// 获取从0到9之间的数值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetGigitInZeroToNightChar(char value)
        {
            switch (value)
            {
                case '0':
                case '\x0':
                    return 0;

                case '1':
                case '\x1':
                    return 1;

                case '2':
                case '\x2':
                    return 2;

                case '3':
                case '\x3':
                    return 3;

                case '4':
                case '\x4':
                    return 4;

                case '5':
                case '\x5':
                    return 5;

                case '6':
                case '\x6':
                    return 6;

                case '7':
                case '\x7':
                    return 7;

                case '8':
                case '\x8':
                    return 8;

                case '9':
                case '\x9':
                    return 9;
            }

            return -1;
        }

        /// <summary>
        /// 根据长度获取整数
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static ulong GetInteger(int length)
        {
            switch (length)
            {
                case 21:
                    return 10000000000000000000;

                case 20:
                    return 1000000000000000000;

                case 19:
                    return 100000000000000000;

                case 18:
                    return 100000000000000000;

                case 17:
                    return 10000000000000000;

                case 16:
                    return 1000000000000000;

                case 15:
                    return 100000000000000;

                case 14:
                    return 10000000000000;

                case 13:
                    return 1000000000000;

                case 12:
                    return 100000000000;

                case 11:
                    return 10000000000;

                case 10:
                    return 1000000000;

                case 9:
                    return 100000000;

                case 8:
                    return 10000000;

                case 7:
                    return 1000000;

                case 6:
                    return 100000;

                case 5:
                    return 10000;

                case 4:
                    return 1000;

                case 3:
                    return 100;

                case 2:
                    return 10;

                case 1:
                    return 1;
            }

            return 0;
        }

        /// <summary>
        /// 获取无符号的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="max">最大值</param>
        /// <param name="bufferLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static ulong ParseUnsignedGigit(char[] buffer, ulong max, int bufferLength)
        {
            if (buffer == null || buffer.Length == 0)
                return 0;

            if (buffer[0] == '-')
                throw new ArgumentException("正数取值第一位不可能为'-'");

            ulong result = 0;

            for (var i = 0; i < bufferLength; i++)
            {
                switch (buffer[i])
                {
                    case '0':
                    case '\x0':
                        {
                            result += GetInteger(bufferLength - i) * 0;
                        }
                        break;

                    case '1':
                    case '\x1':
                        {
                            result += GetInteger(bufferLength - i) * 1;
                        }
                        break;

                    case '2':
                    case '\x2':
                        {
                            result += GetInteger(bufferLength - i) * 2;
                        }
                        break;

                    case '3':
                    case '\x3':
                        {
                            result += GetInteger(bufferLength - i) * 3;
                        }
                        break;

                    case '4':
                    case '\x4':
                        {
                            result += GetInteger(bufferLength - i) * 4;
                        }
                        break;

                    case '5':
                    case '\x5':
                        {
                            result += GetInteger(bufferLength - i) * 5;
                        }
                        break;

                    case '6':
                    case '\x6':
                        {
                            result += GetInteger(bufferLength - i) * 6;
                        }
                        break;

                    case '7':
                    case '\x7':
                        {
                            result += GetInteger(bufferLength - i) * 7;
                        }
                        break;

                    case '8':
                    case '\x8':
                        {
                            result += GetInteger(bufferLength - i) * 8;
                        }
                        break;

                    case '9':
                    case '\x9':
                        {
                            result += GetInteger(bufferLength - i) * 9;
                        }
                        break;

                    default:
                        {
                            throw new ArgumentException("");
                        }
                }

                if (result > max)
                    throw new ArgumentOutOfRangeException("");
            }

            return result;
        }

        /// <summary>
        /// 获取有符号的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="minus">是否为负数</param>
        /// <param name="max">最大值</param>
        /// <param name="min">最小值</param>
        /// <param name="bufferLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static long ParseGigit(char[] buffer, bool minus, long max, long min, int bufferLength)
        {
            if (buffer == null || buffer.Length == 0)
                return 0;

            long result = 0;
            if (minus)
            {
                var minu = min == long.MinValue ? 9223372036854775807 : -min;
                for (var i = 1; i < bufferLength; i++)
                {
                    switch (buffer[i])
                    {
                        case '0':
                        case '\x0':
                            {
                                result += (long)GetInteger(bufferLength - i) * 0;
                            }
                            break;

                        case '1':
                        case '\x1':
                            {
                                result += (long)GetInteger(bufferLength - i) * 1;
                            }
                            break;

                        case '2':
                        case '\x2':
                            {
                                result += (long)GetInteger(bufferLength - i) * 2;
                            }
                            break;

                        case '3':
                        case '\x3':
                            {
                                result += (long)GetInteger(bufferLength - i) * 3;
                            }
                            break;

                        case '4':
                        case '\x4':
                            {
                                result += (long)GetInteger(bufferLength - i) * 4;
                            }
                            break;

                        case '5':
                        case '\x5':
                            {
                                result += (long)GetInteger(bufferLength - i) * 5;
                            }
                            break;

                        case '6':
                        case '\x6':
                            {
                                result += (long)GetInteger(bufferLength - i) * 6;
                            }
                            break;

                        case '7':
                        case '\x7':
                            {
                                result += (long)GetInteger(bufferLength - i) * 7;
                            }
                            break;

                        case '8':
                        case '\x8':
                            {
                                result += (long)GetInteger(bufferLength - i) * 8;
                            }
                            break;

                        case '9':
                        case '\x9':
                            {
                                result += (long)GetInteger(bufferLength - i) * 9;
                            }
                            break;

                        default:
                            {
                                throw new ArgumentException("");
                            }
                    }

                    if (result > minu)
                        throw new ArgumentOutOfRangeException("");
                }

                return -result;
            }

            for (var i = 0; i < bufferLength; i++)
            {
                switch (buffer[i])
                {
                    case '0':
                    case '\x0':
                        {
                            result += (long)GetInteger(bufferLength - i) * 0;
                        }
                        break;

                    case '1':
                    case '\x1':
                        {
                            result += (long)GetInteger(bufferLength - i) * 1;
                        }
                        break;

                    case '2':
                    case '\x2':
                        {
                            result += (long)GetInteger(bufferLength - i) * 2;
                        }
                        break;

                    case '3':
                    case '\x3':
                        {
                            result += (long)GetInteger(bufferLength - i) * 3;
                        }
                        break;

                    case '4':
                    case '\x4':
                        {
                            result += (long)GetInteger(bufferLength - i) * 4;
                        }
                        break;

                    case '5':
                    case '\x5':
                        {
                            result += (long)GetInteger(bufferLength - i) * 5;
                        }
                        break;

                    case '6':
                    case '\x6':
                        {
                            result += (long)GetInteger(bufferLength - i) * 6;
                        }
                        break;

                    case '7':
                    case '\x7':
                        {
                            result += (long)GetInteger(bufferLength - i) * 7;
                        }
                        break;

                    case '8':
                    case '\x8':
                        {
                            result += (long)GetInteger(bufferLength - i) * 8;
                        }
                        break;

                    case '9':
                    case '\x9':
                        {
                            result += (long)GetInteger(bufferLength - i) * 9;
                        }
                        break;

                    default:
                        {
                            throw new ArgumentException("");
                        }
                }

                if (result > max)
                    throw new ArgumentOutOfRangeException("");
            }

            return result;
        }

        /// <summary>
        /// 获取long的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <returns></returns>
        public static long ParseInt64(char[] buffer)
        {
            return ParseGigit(buffer, buffer[0] == '-', long.MaxValue, long.MinValue, buffer.Length);
        }

        /// <summary>
        /// 获取long的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="endLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static long ParseInt64(char[] buffer, int endLength)
        {
            return ParseGigit(buffer, buffer[0] == '-', long.MaxValue, long.MinValue, endLength);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <returns></returns>
        public static int ParseInt32(char[] buffer)
        {
            return (int)ParseGigit(buffer, buffer[0] == '-', int.MaxValue, int.MinValue, buffer.Length);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="endLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static int ParseInt32(char[] buffer, int endLength)
        {
            return (int)ParseGigit(buffer, buffer[0] == '-', int.MaxValue, int.MinValue, endLength);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <returns></returns>
        public static short ParseInt16(char[] buffer)
        {
            return (short)ParseGigit(buffer, buffer[0] == '-', short.MaxValue, short.MinValue, buffer.Length);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="endLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static short ParseInt16(char[] buffer, int endLength)
        {
            return (short)ParseGigit(buffer, buffer[0] == '-', short.MaxValue, short.MinValue, endLength);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <returns></returns>
        public static byte ParseByte(char[] buffer)
        {
            return (byte)ParseGigit(buffer, false, byte.MaxValue, byte.MinValue, buffer.Length);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="endLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static byte ParseByte(char[] buffer, int endLength)
        {
            return (byte)ParseGigit(buffer, false, byte.MaxValue, byte.MinValue, endLength);
        }

        /// <summary>
        /// 获取long的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <returns></returns>
        public static ulong ParseUInt64(char[] buffer)
        {
            return ParseUnsignedGigit(buffer, ulong.MaxValue, buffer.Length);
        }

        /// <summary>
        /// 获取long的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="endLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static ulong ParseUInt64(char[] buffer, int endLength)
        {
            return ParseUnsignedGigit(buffer, ulong.MaxValue, endLength);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <returns></returns>
        public static uint ParseUInt32(char[] buffer)
        {
            return (uint)ParseUnsignedGigit(buffer, uint.MaxValue, buffer.Length);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="endLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static uint ParseUInt32(char[] buffer, int endLength)
        {
            return (uint)ParseUnsignedGigit(buffer, uint.MaxValue, endLength);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <returns></returns>
        public static ushort ParseUInt16(char[] buffer)
        {
            return (ushort)ParseUnsignedGigit(buffer, ushort.MaxValue, buffer.Length);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="endLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static ushort ParseUInt16(char[] buffer, int endLength)
        {
            return (ushort)ParseUnsignedGigit(buffer, ushort.MaxValue, endLength);
        }

        /// <summary>
        /// 获取无符号的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="max">最大值</param>
        /// <param name="bufferLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static ulong ParseUnsignedGigit(ArraySegmentValue buffer, ulong max, int bufferLength)
        {
            if (buffer.IsEmpty)
                return 0;

            if (buffer[0] == '-')
                throw new ArgumentException("正数取值第一位不可能为'-'");

            ulong result = 0;

            for (var i = 0; i < bufferLength; i++)
            {
                switch (buffer[i])
                {
                    case '0':
                    case '\x0':
                        {
                            result += GetInteger(bufferLength - i) * 0;
                        }
                        break;

                    case '1':
                    case '\x1':
                        {
                            result += GetInteger(bufferLength - i) * 1;
                        }
                        break;

                    case '2':
                    case '\x2':
                        {
                            result += GetInteger(bufferLength - i) * 2;
                        }
                        break;

                    case '3':
                    case '\x3':
                        {
                            result += GetInteger(bufferLength - i) * 3;
                        }
                        break;

                    case '4':
                    case '\x4':
                        {
                            result += GetInteger(bufferLength - i) * 4;
                        }
                        break;

                    case '5':
                    case '\x5':
                        {
                            result += GetInteger(bufferLength - i) * 5;
                        }
                        break;

                    case '6':
                    case '\x6':
                        {
                            result += GetInteger(bufferLength - i) * 6;
                        }
                        break;

                    case '7':
                    case '\x7':
                        {
                            result += GetInteger(bufferLength - i) * 7;
                        }
                        break;

                    case '8':
                    case '\x8':
                        {
                            result += GetInteger(bufferLength - i) * 8;
                        }
                        break;

                    case '9':
                    case '\x9':
                        {
                            result += GetInteger(bufferLength - i) * 9;
                        }
                        break;

                    default:
                        {
                            throw new ArgumentException("");
                        }
                }

                if (result > max)
                    throw new ArgumentOutOfRangeException("");
            }

            return result;
        }

        /// <summary>
        /// 获取有符号的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="minus">是否为负数</param>
        /// <param name="max">最大值</param>
        /// <param name="min">最小值</param>
        /// <param name="bufferLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static long ParseGigit(ArraySegmentValue buffer, bool minus, long max, long min, int bufferLength)
        {
            if (buffer.IsEmpty)
                return 0;

            long result = 0;
            if (minus)
            {
                var minu = min == long.MinValue ? 9223372036854775807 : -min;
                for (var i = 1; i < bufferLength; i++)
                {
                    switch (buffer[i])
                    {
                        case '0':
                        case '\x0':
                            {
                                result += (long)GetInteger(bufferLength - i) * 0;
                            }
                            break;

                        case '1':
                        case '\x1':
                            {
                                result += (long)GetInteger(bufferLength - i) * 1;
                            }
                            break;

                        case '2':
                        case '\x2':
                            {
                                result += (long)GetInteger(bufferLength - i) * 2;
                            }
                            break;

                        case '3':
                        case '\x3':
                            {
                                result += (long)GetInteger(bufferLength - i) * 3;
                            }
                            break;

                        case '4':
                        case '\x4':
                            {
                                result += (long)GetInteger(bufferLength - i) * 4;
                            }
                            break;

                        case '5':
                        case '\x5':
                            {
                                result += (long)GetInteger(bufferLength - i) * 5;
                            }
                            break;

                        case '6':
                        case '\x6':
                            {
                                result += (long)GetInteger(bufferLength - i) * 6;
                            }
                            break;

                        case '7':
                        case '\x7':
                            {
                                result += (long)GetInteger(bufferLength - i) * 7;
                            }
                            break;

                        case '8':
                        case '\x8':
                            {
                                result += (long)GetInteger(bufferLength - i) * 8;
                            }
                            break;

                        case '9':
                        case '\x9':
                            {
                                result += (long)GetInteger(bufferLength - i) * 9;
                            }
                            break;

                        default:
                            {
                                throw new ArgumentException("");
                            }
                    }

                    if (result > minu)
                        throw new ArgumentOutOfRangeException("");
                }

                return -result;
            }

            for (var i = 0; i < bufferLength; i++)
            {
                switch (buffer[i])
                {
                    case '0':
                    case '\x0':
                        {
                            result += (long)GetInteger(bufferLength - i) * 0;
                        }
                        break;

                    case '1':
                    case '\x1':
                        {
                            result += (long)GetInteger(bufferLength - i) * 1;
                        }
                        break;

                    case '2':
                    case '\x2':
                        {
                            result += (long)GetInteger(bufferLength - i) * 2;
                        }
                        break;

                    case '3':
                    case '\x3':
                        {
                            result += (long)GetInteger(bufferLength - i) * 3;
                        }
                        break;

                    case '4':
                    case '\x4':
                        {
                            result += (long)GetInteger(bufferLength - i) * 4;
                        }
                        break;

                    case '5':
                    case '\x5':
                        {
                            result += (long)GetInteger(bufferLength - i) * 5;
                        }
                        break;

                    case '6':
                    case '\x6':
                        {
                            result += (long)GetInteger(bufferLength - i) * 6;
                        }
                        break;

                    case '7':
                    case '\x7':
                        {
                            result += (long)GetInteger(bufferLength - i) * 7;
                        }
                        break;

                    case '8':
                    case '\x8':
                        {
                            result += (long)GetInteger(bufferLength - i) * 8;
                        }
                        break;

                    case '9':
                    case '\x9':
                        {
                            result += (long)GetInteger(bufferLength - i) * 9;
                        }
                        break;

                    default:
                        {
                            throw new ArgumentException("");
                        }
                }

                if (result > max)
                    throw new ArgumentOutOfRangeException("");
            }

            return result;
        }

        /// <summary>
        /// 获取long的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <returns></returns>
        public static long ParseInt64(ArraySegmentValue buffer)
        {
            return ParseGigit(buffer, buffer[0] == '-', long.MaxValue, long.MinValue, buffer.Length);
        }

        /// <summary>
        /// 获取long的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="endLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static long ParseInt64(ArraySegmentValue buffer, int endLength)
        {
            return ParseGigit(buffer, buffer[0] == '-', long.MaxValue, long.MinValue, endLength);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <returns></returns>
        public static int ParseInt32(ArraySegmentValue buffer)
        {
            return (int)ParseGigit(buffer, buffer[0] == '-', int.MaxValue, int.MinValue, buffer.Length);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="endLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static int ParseInt32(ArraySegmentValue buffer, int endLength)
        {
            return (int)ParseGigit(buffer, buffer[0] == '-', int.MaxValue, int.MinValue, endLength);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <returns></returns>
        public static short ParseInt16(ArraySegmentValue buffer)
        {
            return (short)ParseGigit(buffer, buffer[0] == '-', short.MaxValue, short.MinValue, buffer.Length);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="endLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static short ParseInt16(ArraySegmentValue buffer, int endLength)
        {
            return (short)ParseGigit(buffer, buffer[0] == '-', short.MaxValue, short.MinValue, endLength);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <returns></returns>
        public static byte ParseByte(ArraySegmentValue buffer)
        {
            return (byte)ParseGigit(buffer, false, byte.MaxValue, byte.MinValue, buffer.Length);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="endLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static byte ParseByte(ArraySegmentValue buffer, int endLength)
        {
            return (byte)ParseGigit(buffer, false, byte.MaxValue, byte.MinValue, endLength);
        }

        /// <summary>
        /// 获取long的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <returns></returns>
        public static ulong ParseUInt64(ArraySegmentValue buffer)
        {
            return ParseUnsignedGigit(buffer, ulong.MaxValue, buffer.Length);
        }

        /// <summary>
        /// 获取long的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="endLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static ulong ParseUInt64(ArraySegmentValue buffer, int endLength)
        {
            return ParseUnsignedGigit(buffer, ulong.MaxValue, endLength);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <returns></returns>
        public static uint ParseUInt32(ArraySegmentValue buffer)
        {
            return (uint)ParseUnsignedGigit(buffer, uint.MaxValue, buffer.Length);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="endLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static uint ParseUInt32(ArraySegmentValue buffer, int endLength)
        {
            return (uint)ParseUnsignedGigit(buffer, uint.MaxValue, endLength);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <returns></returns>
        public static ushort ParseUInt16(ArraySegmentValue buffer)
        {
            return (ushort)ParseUnsignedGigit(buffer, ushort.MaxValue, buffer.Length);
        }

        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="buffer">值数组</param>
        /// <param name="endLength">数组长度，通常是buffer参数的长度，但由于一些空格原因，不会处理空格的</param>
        /// <returns></returns>
        public static ushort ParseUInt16(ArraySegmentValue buffer, int endLength)
        {
            return (ushort)ParseUnsignedGigit(buffer, ushort.MaxValue, endLength);
        }
        #endregion digit

        #region length

        /// <summary>
        /// 获取长度
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>System.Int32.</returns>
        public static int GetStringLength(string source)
        {
            if (string.IsNullOrEmpty(source))
                return 0;

            return source.Length;
        }

        #endregion length

        #region write into char

        /// <summary>
        /// 获取数值的长度
        /// </summary>
        /// <param name="source">源数据</param>
        public static int GetDigitLength(ulong source)
        {
            if (source < 10)
                return 1;

            var length = 0;
            do
            {
                length += 1;
                source = source / 10;
            } while (source != 0);

            return length;
        }

        /// <summary>
        /// 获取数值的长度
        /// </summary>
        /// <param name="source">源数据</param>
        public static int GetDigitLength(long source)
        {
            if (source < 0)
                return GetDigitLength((ulong)(-source)) + 1;

            return GetDigitLength((ulong)(source));
        }

        /// <summary>
        /// 获取数值的长度
        /// </summary>
        /// <param name="source">源数据</param>
        public static int GetDigitLength(ushort source)
        {
            return GetDigitLength((ulong)(source));
        }

        /// <summary>
        /// 获取数值的长度
        /// </summary>
        /// <param name="source">源数据</param>
        public static int GetDigitLength(short source)
        {
            if (source < 0)
                return GetDigitLength((ulong)(-source)) + 1;

            return GetDigitLength((ulong)(source));
        }

        /// <summary>
        /// 获取数值的长度
        /// </summary>
        /// <param name="source">源数据</param>
        public static int GetDigitLength(uint source)
        {
            return GetDigitLength((ulong)(source));
        }

        /// <summary>
        /// 获取数值的长度
        /// </summary>
        /// <param name="source">源数据</param>
        public static int GetDigitLength(int source)
        {
            if (source < 0)
                return GetDigitLength((ulong)(-source)) + 1;

            return GetDigitLength((ulong)(source));
        }

        /// <summary>
        /// 获取数值的长度
        /// </summary>
        /// <param name="source">源数据</param>
        public static int GetDigitLength(byte source)
        {
            if (source < 10)
                return 1;

            if (source < 100)
                return 2;

            return 3;
        }

        /// <summary>
        /// 将source数值在begin长度开始写入到buffer中，请配合GetDigitLength方法先初始化容量
        /// </summary>
        /// <param name="buffer">已经分配好的字节数组</param>
        /// <param name="source">源数据</param>
        /// <param name="digitLength">数值长度</param>
        /// <param name="begin">buffer开始写的长度</param>
        public static void WriteIntoChar(char[] buffer, ulong source, int digitLength, int begin)
        {
            var copy = source;
            var ploidy = Get10Ploidy(digitLength - 1);
            for (var i = digitLength; i > 1; i--)
            {
                buffer[begin + digitLength - i] = GetDigitCharInZeroToNight((int)(copy / ploidy));
                copy = copy % ploidy;
                ploidy = ploidy / 10;
            }

            buffer[begin + digitLength - 1] = GetDigitCharInZeroToNight((int)(copy / 1));
        }

        /// <summary>
        /// 将source数值在begin长度开始写入到buffer中，请配合GetDigitLength方法先初始化容量
        /// </summary>
        /// <param name="buffer">已经分配好的字节数组</param>
        /// <param name="source">源数据</param>
        /// <param name="digitLength">数值长度</param>
        /// <param name="begin">buffer开始写的长度</param>
        public static void WriteIntoChar(char[] buffer, uint source, int digitLength, int begin)
        {
            WriteIntoChar(buffer, (ulong)source, digitLength, begin);
        }

        /// <summary>
        /// 将source数值在begin长度开始写入到buffer中，请配合GetDigitLength方法先初始化容量
        /// </summary>
        /// <param name="buffer">已经分配好的字节数组</param>
        /// <param name="source">源数据</param>
        /// <param name="digitLength">数值长度</param>
        /// <param name="begin">buffer开始写的长度</param>
        public static void WriteIntoChar(char[] buffer, ushort source, int digitLength, int begin)
        {
            WriteIntoChar(buffer, (ulong)source, digitLength, begin);
        }

        /// <summary>
        /// 将source数值在begin长度开始写入到buffer中，请配合GetDigitLength方法先初始化容量
        /// </summary>
        /// <param name="buffer">已经分配好的字节数组</param>
        /// <param name="source">源数据</param>
        /// <param name="digitLength">数值长度</param>
        /// <param name="begin">buffer开始写的长度</param>
        public static void WriteIntoChar(char[] buffer, long source, int digitLength, int begin)
        {
            if (source < 0)
            {
                buffer[begin] = '-';
                WriteIntoChar(buffer, (ulong)(-source), digitLength - 1, begin + 1);
                return;
            }

            WriteIntoChar(buffer, (ulong)(source), digitLength, begin);
        }

        /// <summary>
        /// 将source数值在begin长度开始写入到buffer中，请配合GetDigitLength方法先初始化容量
        /// </summary>
        /// <param name="buffer">已经分配好的字节数组</param>
        /// <param name="source">源数据</param>
        /// <param name="digitLength">数值长度</param>
        /// <param name="begin">buffer开始写的长度</param>
        public static void WriteIntoChar(char[] buffer, int source, int digitLength, int begin)
        {
            WriteIntoChar(buffer, (long)source, digitLength, begin);
        }

        /// <summary>
        /// 将source数值在begin长度开始写入到buffer中，请配合GetDigitLength方法先初始化容量
        /// </summary>
        /// <param name="buffer">已经分配好的字节数组</param>
        /// <param name="source">源数据</param>
        /// <param name="digitLength">数值长度</param>
        /// <param name="begin">buffer开始写的长度</param>
        public static void WriteIntoChar(char[] buffer, short source, int digitLength, int begin)
        {
            WriteIntoChar(buffer, (long)source, digitLength, begin);
        }

        /// <summary>
        /// 将source数值在begin长度开始写入到buffer中，请配合GetDigitLength方法先初始化容量
        /// </summary>
        /// <param name="buffer">已经分配好的字节数组</param>
        /// <param name="source">源数据</param>
        /// <param name="digitLength">数值长度</param>
        /// <param name="begin">buffer开始写的长度</param>
        public static void WriteIntoChar(char[] buffer, byte source, int digitLength, int begin)
        {
            WriteIntoChar(buffer, (long)source, digitLength, begin);
        }

        /// <summary>
        /// 获取10的倍数
        /// </summary>
        /// <param name="digitLength">数值长度</param>
        public static ulong Get10Ploidy(int digitLength)
        {
            return TenPloidy[digitLength].Value;
        }

        #endregion write into char

        #region check nullable

        /// <summary>
        /// 是否为空值
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns></returns>
        public virtual bool IsNullValue(char[] buffer)
        {
            if (buffer.Length == 4 && buffer[1] <= 'u'
                && (buffer[0] == 'n' || buffer[0] == 'N')
                && (buffer[1] == 'u' || buffer[1] == 'U')
                && (buffer[2] == 'l' || buffer[2] == 'L')
                && (buffer[3] == 'l' || buffer[3] == 'L'))
                return true;

            return false;
        }
        /// <summary>
        /// 是否为空值
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns></returns>
        public virtual bool IsNullValue(ArraySegmentValue buffer)
        {
            if (buffer.Length == 4 && buffer[1] <= 'u'
                && (buffer[0] == 'n' || buffer[0] == 'N')
                && (buffer[1] == 'u' || buffer[1] == 'U')
                && (buffer[2] == 'l' || buffer[2] == 'L')
                && (buffer[3] == 'l' || buffer[3] == 'L'))
                return true;

            return false;
        }
        /// <summary>
        /// 是否为空格
        /// </summary>
        /// <param name="char">The character.</param>
        /// <returns></returns>
        public virtual bool IsWhiteSpace(int @char)
        {
            //  \u0020  -    - 33  space
            //  \u0009  - \t - 9   tab
            //  \u000A  - \r - 13  new line
            //  \u000D  - \n - 10  carriage return

            return @char < 0x21 && (@char == 0x20 || @char == 0x09 || @char == 0x0A || @char == 0x0D);
        }

        #endregion check nullable
    }
}