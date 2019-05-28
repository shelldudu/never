using System.Text;

namespace Never.Serialization.Json
{
    /// <summary>
    /// 快速写入数据
    /// </summary>
    public class ThunderWriter : ISerializerWriter
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private StringBuilder sb;

        private readonly int capacity = 0;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        public ThunderWriter()
            : this(16)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="capacity"></param>
        public ThunderWriter(int capacity)
            : this(new StringBuilder(capacity))
        {
            this.capacity = capacity;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sb"></param>
        public ThunderWriter(StringBuilder sb)
        {
            this.sb = sb;
            this.capacity = 16;
        }

        #endregion ctor

        #region write

        /// <summary>
        /// 将字符数组写入文本流。
        /// </summary>
        /// <param name="buffer">要写入文本流中的字符数组</param>
        public void Write(char[] buffer)
        {
            if (buffer == null)
                return;

            this.sb.Append(buffer);
        }

        /// <summary>
        ///  将字符数组写入文本流。
        /// </summary>
        /// <param name="buffer">要写入文本流中的字符数组</param>
        /// <param name="startIndex">数组开始索引</param>
        public void Write(char[] buffer, int startIndex)
        {
            this.Write(buffer, startIndex, 0);
        }

        /// <summary>
        ///  将字符数组写入文本流。
        /// </summary>
        /// <param name="buffer">要写入文本流中的字符数组</param>
        /// <param name="startIndex">数组开始索引</param>
        /// <param name="charCount">要追加的字符数</param>
        public void Write(char[] buffer, int startIndex, int charCount)
        {
            if (buffer == null)
                return;

            this.sb.Append(buffer, startIndex, charCount <= 0 ? (buffer.Length - startIndex) : charCount);
        }

        /// <summary>
        /// 将字符串写入文本流
        /// </summary>
        /// <param name="value">字符串</param>
        public void Write(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            this.sb.Append(value);
        }

        /// <summary>
        /// 将十进制值的文本表示形式写入文本流。
        /// </summary>
        /// <param name="value">要写入的十进制值</param>
        public void Write(decimal value)
        {
            this.sb.Append(value);
        }

        /// <summary>
        /// 将 8 字节浮点值的文本表示形式写入文本流。
        /// </summary>
        /// <param name="value">要写入的 8 字节浮点值</param>
        public void Write(double value)
        {
            this.sb.Append(value);
        }

        /// <summary>
        /// 将 4 字节浮点值的文本表示形式写入文本流。
        /// </summary>
        /// <param name="value">要写入的 4 字节浮点值。</param>
        public void Write(float value)
        {
            this.sb.Append(value);
        }

        /// <summary>
        /// 将 1 字节有符号整数的文本表示形式写入文本流。
        /// </summary>
        /// <param name="value">要写入的 4 字节有符号整数。</param>
        public void Write(byte value)
        {
            this.sb.Append(value);
        }

        /// <summary>
        /// 将 1 字节有符号整数的文本表示形式写入文本流。
        /// </summary>
        /// <param name="value">要写入的 4 字节有符号整数。</param>
        public void Write(short value)
        {
            this.sb.Append(value);
        }

        /// <summary>
        /// 将 4 字节有符号整数的文本表示形式写入文本流。
        /// </summary>
        /// <param name="value">要写入的 4 字节有符号整数。</param>
        public void Write(int value)
        {
            this.sb.Append(value);
        }

        /// <summary>
        /// 将 8 字节有符号整数的文本表示形式写入文本流。
        /// </summary>
        /// <param name="value">要写入的 8 字节有符号整数。</param>
        public void Write(long value)
        {
            this.sb.Append(value);
        }

        /// <summary>
        /// 将 2 字节无符号整数的文本表示形式写入文本流。
        /// </summary>
        /// <param name="value">要写入的 4 字节有符号整数。</param>
        public void Write(ushort value)
        {
            this.sb.Append(value);
        }

        /// <summary>
        /// 将 4 字节无符号整数的文本表示形式写入文本流。
        /// </summary>
        /// <param name="value">要写入的 4 字节无符号整数。</param>
        public void Write(uint value)
        {
            this.sb.Append(value);
        }

        /// <summary>
        /// 将 8 字节无符号整数的文本表示形式写入文本流。
        /// </summary>
        /// <param name="value">要写入的 8 字节无符号整数</param>
        public void Write(ulong value)
        {
            this.sb.Append(value);
        }

        /// <summary>
        /// 将字符写入文本流。
        /// </summary>
        /// <param name="value">要写入文本流中的字符。</param>
        public void Write(char value)
        {
            this.sb.Append(value);
        }

        #endregion write

        #region tostring

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.sb.ToString();
        }

        #endregion tostring

        #region clear

        /// <summary>
        /// 清空信息内容
        /// </summary>
        public void Clear()
        {
            this.sb.Clear();
        }

        #endregion clear
    }
}