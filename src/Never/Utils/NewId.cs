using System;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace Never.Utils
{
    /// <summary>
    /// newId 生成器
    /// </summary>
    public sealed class NewId
    {
        #region guid

        /// <summary>
        /// guid 生成器
        /// </summary>
        private class GuidGerarator
        {
            #region generate

            /// <summary>
            /// 生成一个尾部有序的guid
            /// </summary>
            /// <returns></returns>
            public static Guid Generate()
            {
                /*这段代码来自于cnblogs中某一位园友的贡献,由于时代久远,找不到该文,在此表示谢意*/
                byte[] guidArray = Guid.NewGuid().ToByteArray();

                DateTime now = DateTime.Now;
                var days = new TimeSpan(now.Ticks - 599266080000000000);
                TimeSpan msecs = now.TimeOfDay;

                byte[] daysArray = BitConverter.GetBytes(days.Days);
                byte[] msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

                Array.Reverse(daysArray);
                Array.Reverse(msecsArray);

                Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
                Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

                return new Guid(guidArray);
            }

            #endregion generate
        }

        #endregion guid

        #region string

        /// <summary>
        /// string 生成器
        /// </summary>
        private class StringGerarator
        {
            #region field

            /// <summary>
            /// 当前线程Id
            /// </summary>
            private static readonly int pid;

            /// <summary>
            /// 当前增量
            /// </summary>
            private static long factor = 0;

            /// <summary>
            /// 当前机器名
            /// </summary>
            private static int machineId;

            /// <summary>
            /// 当前时间计时
            /// </summary>
            private static long datetimeId;

            /// <summary>
            /// 网络IP
            /// </summary>
            private static int networkId;

            #endregion field

            #region ctor

            /// <summary>
            /// Initializes the <see cref="NewId"/> class.
            /// </summary>
            static StringGerarator()
            {
                var random = new Random();
                pid = GetProcessId();
                machineId = GetMachineId();
                networkId = GetNetworkId(random);
                factor = random.Next(500);
                datetimeId = GetDateTimeId();
            }

            #endregion ctor

            #region method

            /// <summary>
            /// 获取时间计时周期
            /// </summary>
            /// <returns></returns>
            private static long GetDateTimeId()
            {
                return DateTime.UtcNow.Ticks - 621355968000000000;
            }

            /// <summary>
            /// 移动增量
            /// </summary>
            /// <returns></returns>
            private static long NextIncrement()
            {
                var inc = System.Threading.Interlocked.Increment(ref factor);
                if (inc == int.MinValue || inc == 0)
                {
                    System.Threading.Thread.Sleep(1);
                    datetimeId = GetDateTimeId();
                }

                return inc;
            }

            /// <summary>
            ///
            /// </summary>
            /// <returns></returns>
            private static char[] Format(byte[] bytes)
            {
                var result = new char[bytes.Length * 2];
                int offset = 0;
                for (int i = 0; i < bytes.Length; i++)
                {
                    byte value = bytes[i];
                    result[offset++] = HexToChar(value >> 4, 'a');
                    result[offset++] = HexToChar(value, 'a');
                }

                return result;
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="value"></param>
            /// <param name="alpha"></param>
            /// <returns></returns>
            private static char HexToChar(int value, int alpha)
            {
                value = value & 0xf;
                return (char)((value > 9) ? value - 10 + alpha : value + 0x30);
            }

            #endregion method

            #region generate

            /// <summary>
            /// 生成一个长度为30位有序的string
            /// </summary>
            /// <returns></returns>
            public static string Generate30()
            {
                /*获取自增id*/
                var incerment = NextIncrement();

                var bytes = new byte[15];
                bytes[0] = (byte)(datetimeId >> 24);
                bytes[1] = (byte)(datetimeId >> 16);
                bytes[2] = (byte)(datetimeId >> 8);
                bytes[3] = (byte)datetimeId;

                bytes[4] = (byte)(machineId >> 16);
                bytes[5] = (byte)(machineId >> 8);
                bytes[6] = (byte)machineId;

                bytes[7] = (byte)(pid >> 8);
                bytes[8] = (byte)pid;
                bytes[9] = (byte)(networkId >> 8);
                bytes[10] = (byte)networkId;

                bytes[11] = (byte)(incerment >> 24);
                bytes[12] = (byte)(incerment >> 16);
                bytes[13] = (byte)(incerment >> 8);
                bytes[14] = (byte)incerment;

                /*一共30位*/
                return new string(Format(bytes));
            }

            /// <summary>
            /// 生成一个有序的string
            /// </summary>
            /// <returns></returns>
            public static string Generate24()
            {
                /*获取自增id*/
                var incerment = NextIncrement();

                var bytes = new byte[12];
                bytes[0] = (byte)(datetimeId >> 24);
                bytes[1] = (byte)(datetimeId >> 16);
                bytes[2] = (byte)(datetimeId >> 8);
                bytes[3] = (byte)datetimeId;

                bytes[4] = (byte)(machineId >> 8);
                bytes[5] = (byte)machineId;

                bytes[6] = (byte)(pid >> 8);
                bytes[7] = (byte)pid;

                bytes[8] = (byte)networkId;

                bytes[9] = (byte)(incerment >> 16);
                bytes[10] = (byte)(incerment >> 8);
                bytes[11] = (byte)incerment;

                /*一共24位*/
                return new string(Format(bytes));
            }

            #endregion generate
        }

        #endregion string

        #region long

        /// <summary>
        /// number 生成器
        /// </summary>
        private class NumberGenrator
        {
            #region field

            /// <summary>
            /// 当前线程Id，4位
            /// </summary>
            private static char[] pid;

            /// <summary>
            /// 网络IP,3位
            /// </summary>
            private static char[] networkId;

            /// <summary>
            /// 当前机器名,4位
            /// </summary>
            private static char[] machineId;

            /// <summary>
            /// 当前增量
            /// </summary>
            private static int factor = 0;

            /// <summary>
            /// 当前时间计时
            /// </summary>
            private static int datetimeId;

            #endregion field

            #region ctor

            /// <summary>
            /// Initializes the <see cref="NewId"/> class.
            /// </summary>
            static NumberGenrator()
            {
                var random = new Random();
                pid = new char[4];
                Padding4Bit(GetProcessId(), pid, 0);

                networkId = new char[3];
                Padding3Bit(GetNetworkId(random), networkId, 0);

                machineId = new char[4];

                Padding4Bit(GetMachineId(), machineId, 0);

                factor = random.Next(500);
                datetimeId = random.Next(0, 9);
            }

            #endregion ctor

            #region method

            /// <summary>
            /// 获取时间计时周期
            /// </summary>
            /// <returns></returns>
            private static int GetDateTimeId()
            {
                datetimeId++;
                if (datetimeId > 9)
                {
                    System.Threading.Thread.Sleep(1);
                    datetimeId = 0;
                }

                return datetimeId;
            }

            /// <summary>
            /// 移动增量
            /// </summary>
            /// <returns></returns>
            private static int NextIncrement()
            {
                var inc = System.Threading.Interlocked.Increment(ref factor);
                if (inc == int.MinValue || inc == 0)
                    GetDateTimeId();

                return inc;
            }

            #endregion method

            #region generate

            /// <summary>
            /// 生成30位的纯数字数据
            /// </summary>
            /// <returns></returns>
            public static string Generate30()
            {
                /*获取自增id*/
                var incerment = NextIncrement();
                var chars = new char[32];
                var time = DateTime.Now;
                if (time.Year <= 9999)
                    Padding4Bit(time.Year, chars, 0);

                //time
                chars[4] = GetDigitCharInZeroToNight(time.Month / 10);
                chars[5] = GetDigitCharInZeroToNight(time.Month % 10);
                chars[6] = GetDigitCharInZeroToNight(time.Day / 10);
                chars[7] = GetDigitCharInZeroToNight(time.Day % 10);
                chars[8] = GetDigitCharInZeroToNight(time.Hour / 10);
                chars[9] = GetDigitCharInZeroToNight(time.Hour % 10);
                chars[10] = GetDigitCharInZeroToNight(time.Minute / 10);
                chars[11] = GetDigitCharInZeroToNight(time.Minute % 10);
                chars[12] = GetDigitCharInZeroToNight(time.Second / 10);
                chars[13] = GetDigitCharInZeroToNight(time.Second % 10);

                chars[14] = GetDigitCharInZeroToNight(datetimeId);

                //pid
                chars[15] = pid[0];
                chars[16] = pid[1];
                chars[17] = pid[2];
                chars[18] = pid[3];

                //nid
                chars[19] = networkId[0];
                chars[20] = networkId[1];
                chars[21] = networkId[2];

                //mid
                chars[22] = machineId[0];
                chars[23] = machineId[1];
                chars[24] = machineId[2];
                chars[25] = machineId[2];

                Padding2Bit(GetThreadId(), chars, 26);

                //inc
                var ran = new char[4];
                Padding4Bit(incerment, ran, 0);
                chars[28] = ran[0];
                chars[29] = ran[1];
                chars[30] = ran[2];
                chars[31] = ran[3];

                /*一共30位*/
                return new string(chars, 2, 30);
            }

            /// <summary>
            /// 生成24位纯数字数据
            /// </summary>
            /// <returns></returns>
            public static string Generate24()
            {
                /*获取自增id*/
                var incerment = NextIncrement();
                var chars = new char[26];
                var time = DateTime.Now;
                if (time.Year <= 9999)
                    Padding4Bit(time.Year, chars, 0);

                //time
                chars[4] = GetDigitCharInZeroToNight(time.Month / 10);
                chars[5] = GetDigitCharInZeroToNight(time.Month % 10);
                chars[6] = GetDigitCharInZeroToNight(time.Day / 10);
                chars[7] = GetDigitCharInZeroToNight(time.Day % 10);
                chars[8] = GetDigitCharInZeroToNight(time.Hour / 10);
                chars[9] = GetDigitCharInZeroToNight(time.Hour % 10);
                chars[10] = GetDigitCharInZeroToNight(time.Minute / 10);
                chars[11] = GetDigitCharInZeroToNight(time.Minute % 10);
                chars[12] = GetDigitCharInZeroToNight(time.Second / 10);
                chars[13] = GetDigitCharInZeroToNight(time.Second % 10);

                chars[14] = GetDigitCharInZeroToNight(datetimeId);

                //pid
                chars[15] = pid[0];
                chars[16] = pid[1];
                chars[17] = pid[2];
                chars[18] = pid[3];

                //nid
                chars[19] = networkId[0];
                chars[20] = networkId[1];
                chars[21] = networkId[2];

                //inc
                var ran = new char[4];
                Padding4Bit(incerment, ran, 0);
                chars[22] = ran[0];
                chars[23] = ran[1];
                chars[24] = ran[2];
                chars[25] = ran[3];

                /*一共24位*/
                return new string(chars, 2, 24);
            }

            #endregion generate
        }

        #endregion long

        #region helper

        /// <summary>
        /// 获取当前线程Id
        /// </summary>
        /// <returns></returns>
        private static int GetProcessId()
        {
            return System.Diagnostics.Process.GetCurrentProcess().Id;
        }

        /// <summary>
        /// 获取当前线程Id
        /// </summary>
        /// <returns></returns>
        private static int GetThreadId()
        {
            return System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// 获取machineId
        /// </summary>
        /// <returns></returns>
        private static int GetMachineId()
        {
            /*MD5静态对象，在多线程下会不安全，所以这里不用static MD5 = 这种静态对象*/
            var md5 = new MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(Environment.MachineName));
            return hash[0] << 16 | hash[1] << 8 | hash[2];
        }

        /// <summary>
        /// 获取网络IP
        /// </summary>
        /// <returns></returns>
        private static int GetNetworkId(Random random)
        {
            var @interfaces = NetworkInterface.GetAllNetworkInterfaces();
            /*没有网卡吗?*/
            if (@interfaces == null || @interfaces.Length == 0)
                return random.Next(1000);

            var bytes = @interfaces[random.Next(@interfaces.Length)].GetPhysicalAddress().GetAddressBytes();
            if (bytes.Length < 4)
                return random.Next(1000);

            return bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3];
        }

        /// <summary>
        /// 填充左边字符,使长度总共为length
        /// </summary>
        /// <param name="tickets">The tickets.</param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string Padding(long tickets, byte length)
        {
            var source = tickets.ToString();
            if (source.Length > length)
                throw new ArgumentNullException(string.Format("源数值长度不能大于{0}", length));

            if (source.Length == length)
                return source;

            var buffer = new char[length];
            for (var i = 0; i < buffer.Length; i++)
                buffer[i] = '0';

            int index = 0;
            while (true)
            {
                buffer[length - source.Length + index] = source[index];
                index++;
                if (index >= source.Length)
                    break;
            }

            return new string(buffer);
        }

        /// <summary>
        /// 填充4位
        /// </summary>
        /// <param name="number"></param>
        /// <param name="chars"></param>
        /// <param name="start"></param>
        private static void Padding4Bit(int number, char[] chars, int start)
        {
            var num = number % 10000;
            chars[start] = GetDigitCharInZeroToNight(num / 1000);
            num = num - GetGigitInZeroToNightChar(chars[start]) * 1000;
            chars[start + 1] = GetDigitCharInZeroToNight(num / 100);
            num = num - GetGigitInZeroToNightChar(chars[start + 1]) * 100;
            chars[start + 2] = GetDigitCharInZeroToNight(num / 10);
            chars[start + 3] = GetDigitCharInZeroToNight(num % 10);
        }

        /// <summary>
        /// 填充3位
        /// </summary>
        /// <param name="number"></param>
        /// <param name="chars"></param>
        /// <param name="start"></param>
        private static void Padding3Bit(int number, char[] chars, int start)
        {
            var num = number % 1000;
            chars[start] = GetDigitCharInZeroToNight(num / 100);
            num = num - GetGigitInZeroToNightChar(chars[start]) * 100;
            chars[start + 1] = GetDigitCharInZeroToNight(num / 10);
            chars[start + 2] = GetDigitCharInZeroToNight(num % 10);
        }

        /// <summary>
        /// 填充2位
        /// </summary>
        /// <param name="number"></param>
        /// <param name="chars"></param>
        /// <param name="start"></param>
        private static void Padding2Bit(int number, char[] chars, int start)
        {
            var num = number % 100;
            chars[start] = GetDigitCharInZeroToNight(num / 10);
            chars[start + 1] = GetDigitCharInZeroToNight(num % 10);
        }

        /// <summary>
        /// 获取从0到9之间的数值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static char GetDigitCharInZeroToNight(long value)
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

            return GetDigitCharInZeroToNight(value % 10);
        }

        /// <summary>
        /// 获取从0到9之间的数值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int GetGigitInZeroToNightChar(char value)
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

            return 0;
        }

        #endregion helper

        #region generate

        /// <summary>
        /// 生成一个尾部有序的guid
        /// </summary>
        /// <returns></returns>
        public static Guid GenerateGuid()
        {
            return GuidGerarator.Generate();
        }

        /// <summary>
        /// 生成一个长度为30位有序的string
        /// </summary>
        /// <returns></returns>
        public static string GenerateString()
        {
            return StringGerarator.Generate30();
        }

        /// <summary>
        /// 生成一个有序的string
        /// </summary>
        /// <returns></returns>
        public static string GenerateString(StringLength length)
        {
            if (length == StringLength.L30)
                return StringGerarator.Generate30();

            return StringGerarator.Generate24();
        }

        /// <summary>
        /// 生成30位的纯数字数据
        /// </summary>
        /// <returns></returns>
        public static string GenerateNumber()
        {
            return NumberGenrator.Generate30();
        }

        /// <summary>
        /// 生成纯数字数据
        /// </summary>
        /// <returns></returns>
        public static string GenerateNumber(StringLength length)
        {
            if (length == StringLength.L30)
                return NumberGenrator.Generate30();

            return NumberGenrator.Generate24();
        }

        #endregion generate

        #region enum

        /// <summary>
        /// 字符长度
        /// </summary>
        public enum StringLength : byte
        {
            /// <summary>
            /// 长度为24位
            /// </summary>
            L24,

            /// <summary>
            /// 长度为30位
            /// </summary>
            L30
        }

        #endregion enum
    }
}