using System;

namespace Never.Utils
{
    /// <summary>
    /// 短地址转换
    /// </summary>
    public static class ShortUrl
    {
        private static String str62keys = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static String IntToEnode62(string mid)
        {
            long int_mid = Int64.Parse(mid);
            String result = "";
            do
            {
                long a = int_mid % 62;
                result = str62keys[(int)a] + result;
                int_mid = (int_mid - a) / 62;
            }
            while (int_mid > 0);

            return result.PadLeft(4, '0');
        }

        private static Int64 Encode62ToInt(String str62)
        {
            Int64 i10 = 0;
            for (var i = 0; i < str62.Length; i++)
            {
                double n = str62.Length - i - 1;
                i10 += Convert.ToInt64(str62keys.IndexOf(str62[i]) * Math.Pow(62, n));
            }
            string temp = i10.ToString().PadLeft(7, '0');

            Int64.TryParse(temp, out i10);

            return i10;
        }

        /// <summary>
        /// 将Mid转换为Id
        /// </summary>
        /// <param name="str62"></param>
        /// <returns></returns>
        public static string MidToId(string str62)
        {
            String id = "";
            for (int i = str62.Length - 4; i > -4; i = i - 4) //从最后往前以4字节为一组读取字符
            {
                int offset = i < 0 ? 0 : i;
                int len = i < 0 ? str62.Length % 4 : 4;
                String str = Encode62ToInt(str62.Substring(offset, len)).ToString();

                if (offset > 0)
                    str = str.PadLeft(7, '0'); //若不是第一组，则不足7位补0

                id = str + id;
            }
            return id;
        }

        /// <summary>
        /// 将Id转制为Mid
        /// </summary>
        /// <param name="mid"></param>
        /// <returns></returns>
        public static string IdToMid(string mid)
        {
            long int_mid = Int64.Parse(mid);
            String result = "";
            for (int i = mid.Length - 7; i > -7; i -= 7)
            {
                int offset1 = (i < 0) ? 0 : i;
                int offset2 = i + 7;
                String num = IntToEnode62(mid.Substring(offset1, offset2 - offset1));
                result = num + result;
            }
            return result;
        }
    }
}