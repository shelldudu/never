namespace Never
{
    /// <summary>
    /// 创建单例对象
    /// </summary>
    /// <typeparam name="T">单例对象</typeparam>
    public sealed class Singleton<T>
    {
        #region field

        /// <summary>
        /// 单例对象
        /// </summary>
        private static T instance;

        #endregion field

        #region instance

        /// <summary>
        /// 单例对象
        /// </summary>
        public static T Instance
        {
            get
            {
                return instance;
            }

            set
            {
                instance = value;
            }
        }

        #endregion instance
    }
}