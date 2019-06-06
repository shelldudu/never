using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Memcached
{
    /// <summary>
    /// 压缩协议
    /// </summary>
    public interface ICompressProtocol : IDisposable
    {
        /// <summary>
        /// 压缩
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="object">对象</param>
        /// <param name="flag">flag</param>
        /// <returns></returns>
        byte[] Compress<T>(T @object, out byte flag);

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="object">对象</param>
        /// <param name="flag">flag</param>
        /// <returns></returns>
        byte[] Compress(object @object, out byte flag);


        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="byte">源数据</param>
        /// <param name="flag">flag</param>
        /// <param name="object"></param>
        /// <returns></returns>
        bool TryDecompress(byte[] @byte, byte flag, out object @object);

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="byte">源数据</param>
        /// <param name="flag">flag</param>
        /// <param name="object"></param>
        /// <returns></returns>
        bool TryDecompress<T>(byte[] @byte, byte flag, out T @object);
    }
}
