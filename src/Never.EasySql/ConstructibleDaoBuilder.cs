using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 可构造的DaoBuilder
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConstructibleDaoBuilder<T> : IConstructibleOption<T> where T : BaseDaoBuilder, new()
    {
        /// <summary>
        /// 实例
        /// </summary>
        private readonly T value = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private ConstructibleDaoBuilder(T value)
        {
            this.value = value;
            this.value.Startup();
        }

        /// <summary>
        /// T对象实例
        /// </summary>
        T IValuableOption<T>.Value => this.value;

        #region static

        /// <summary>
        /// instance
        /// </summary>
        static ConstructibleDaoBuilder<T> @static = null;

        /// <summary>
        /// T对象实例
        /// </summary>
        public static T Value
        {
            get
            {
                if (@static == null)
                {
                    lock (typeof(T))
                    {
                        if (@static == null)
                            @static = new ConstructibleDaoBuilder<T>(new T());
                    }
                }

                return @static.value;
            }
        }

        #endregion
    }
}