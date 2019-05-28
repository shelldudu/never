namespace Never.Utils
{
    /// <summary>
    /// 频道信息
    /// </summary>
    public static class Channel
    {
        /// <summary>
        /// 返回频道大类
        /// </summary>
        /// <param name="ftype">ftypeId</param>
        /// <param name="channel">频道Id</param>
        /// <returns></returns>
        public static int GetFtype(int ftype, int channel)
        {
            return AddPlaceHolder16(ftype, channel);
        }

        /// <summary>
        /// 返回频道小类
        /// </summary>
        /// <param name="stype">stypeId</param>
        /// <param name="channel">频道Id</param>
        /// <returns></returns>
        public static int GetStype(int stype, int channel)
        {
            return AddPlaceHolder16(stype, channel);
        }

        /// <summary>
        /// 返回频道专题
        /// </summary>
        /// <param name="special">专题Id</param>
        /// <param name="channel">频道Id</param>
        /// <returns></returns>
        public static int GetSpecial(int special, int channel)
        {
            return AddPlaceHolder16(special, channel);
        }

        /// <summary>
        /// 返回频道原始大类
        /// </summary>
        /// <param name="ftype">ftypeId</param>
        /// <param name="channelId">频道Id</param>
        /// <returns></returns>
        public static int GetOriginalFtype(int ftype, int channelId)
        {
            return SubPlaceHolder16(ftype, channelId);
        }

        /// <summary>
        /// 返回频道原始小类
        /// </summary>
        /// <param name="stype">stypeId</param>
        /// <param name="channelId">频道Id</param>
        /// <returns></returns>
        public static int GetOriginalStype(int stype, int channelId)
        {
            return SubPlaceHolder16(stype, channelId);
        }

        /// <summary>
        /// 返回频道原始专题
        /// </summary>
        /// <param name="special">specialId</param>
        /// <param name="channelId">频道Id</param>
        /// <returns></returns>
        public static int GetOriginalSpecial(int special, int channelId)
        {
            return SubPlaceHolder16(special, channelId);
        }

        /// <summary>
        /// 返回频道新闻
        /// </summary>
        /// <param name="newsId">新闻Id</param>
        /// <param name="channelId">频道Id</param>
        /// <returns></returns>
        public static long GetNewsId(int newsId, int channelId)
        {
            const int left = 32;
            return ((long)channelId << left) | (uint)newsId;
        }

        /// <summary>
        /// 返回频道原始新闻
        /// </summary>
        /// <param name="newsId">新闻Id</param>
        /// <returns></returns>
        public static int GetOriginalNewsId(long newsId)
        {
            return (int)newsId;
        }

        /// <summary>
        /// 返回频道原始新闻
        /// </summary>
        /// <param name="newsId">新闻Id</param>
        /// <param name="channelId">频道Id</param>
        /// <returns></returns>
        public static int GetOriginalNewsId(long newsId, int channelId)
        {
            const int left = 32;
            return (int)(((long)channelId << left) ^ newsId);
        }

        /// <summary>
        /// Adds the place holder16.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        private static int AddPlaceHolder16(int number, int channel)
        {
            const int left = 16;
            const int max = 1 << left;

            channel = channel == 0 ? 1024 : channel;

            return number < max ? (channel << left) | number : number;
        }

        /// <summary>
        /// Subs the place holder16.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        private static int SubPlaceHolder16(int number, int channel)
        {
            const int left = 16;
            const int max = 1 << left;

            channel = channel == 0 ? 1024 : channel;

            return number < max ? number : number - (channel << left);
        }
    }
}