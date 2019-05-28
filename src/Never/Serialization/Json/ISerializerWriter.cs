namespace Never.Serialization.Json
{
    /// <summary>
    /// 序列化写入流
    /// </summary>
    public interface ISerializerWriter
    {
        /// <summary>
        ///  将字符数组写入文本流。
        /// </summary>
        /// <param name="buffer">要写入文本流中的字符数组</param>
        void Write(char[] buffer);

        /// <summary>
        ///  将字符数组写入文本流。
        /// </summary>
        /// <param name="buffer">要写入文本流中的字符数组</param>
        /// <param name="startIndex">数组开始索引</param>
        /// <param name="charCount">要追加的字符数</param>
        void Write(char[] buffer, int startIndex, int charCount = 0);

        /// <summary>
        /// 将字符写入文本流。
        /// </summary>
        /// <param name="value">要写入文本流中的字符。</param>
        void Write(char value);

        /// <summary>
        /// 将字符串写入文本流
        /// </summary>
        /// <param name="value">字符串</param>
        void Write(string value);
    }
}