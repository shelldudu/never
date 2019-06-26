namespace Never.Utils
{
    /// <summary>
    /// 频道信息
    /// </summary>
    public static class Channel
    {
        /// <summary>
        /// Get holder16
        /// </summary>
        /// <param name="ftype">ftypeId</param>
        /// <param name="channelId">channelId</param>
        /// <returns></returns>
        public static int GetFtypeId(int ftype, int channelId)
        {
            const int left = 16;
            const int max = 1 << left;
            channelId = channelId == 0 ? 1024 : channelId;
            return ftype < max ? (channelId << left) | ftype : ftype;
        }

        /// <summary>
        /// return holder16
        /// </summary>
        /// <param name="ftype">ftypeId</param>
        /// <param name="channelId">channelId</param>
        /// <returns></returns>
        public static int GetOriginalFtypeId(int ftype, int channelId)
        {
            const int left = 16;
            const int max = 1 << left;
            channelId = channelId == 0 ? 1024 : channelId;
            return ftype < max ? ftype : ftype - (channelId << left);
        }

        /// <summary>
        /// get newsId
        /// </summary>
        /// <param name="newsId">newsId</param>
        /// <param name="channelId">channelId</param>
        /// <returns></returns>
        public static long GetNewsId(int newsId, int channelId)
        {
            const int left = 32;
            return ((long)channelId << left) | (uint)newsId;
        }

        /// <summary>
        /// get newsId
        /// </summary>
        /// <param name="newsId">newsId</param>
        /// <param name="channelId">channelId</param>
        /// <returns></returns>
        public static int GetOriginalNewsId(long newsId, int channelId)
        {
            const int left = 32;
            return (int)(((long)channelId << left) ^ newsId);
        }
    }
}