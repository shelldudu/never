using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// key-value参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class KeyValueSqlParameter<T> : EasySqlParameter<T>
    {
        #region ctor

        /// <summary>
        /// sql参数，只接受key-value这种形式的对象
        /// </summary>
        /// <param name="object"></param>
        public KeyValueSqlParameter(T @object) : base(@object)
        {
        }

        #endregion ctor
    }
}