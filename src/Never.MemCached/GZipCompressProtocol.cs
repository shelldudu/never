using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Never.Serialization;

namespace Never.Memcached
{
    /// <summary>
    /// GZip压缩
    /// </summary>
    public class GZipCompressProtocol : BinaryCompressProtocol, ICompressProtocol
    {
        #region field and ctor

        /// <summary>
        /// 
        /// </summary>
        public GZipCompressProtocol() : this(new BinarySerializer(), null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoding">对value为string类型的直接使用Encoding.GetBytes</param>
        public GZipCompressProtocol(Encoding encoding) : this(new BinarySerializer(), encoding) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binarySerializer"></param>
        /// <param name="encoding">对value为string类型的直接使用Encoding.GetBytes</param>
        public GZipCompressProtocol(IBinarySerializer binarySerializer, Encoding encoding) : base(binarySerializer, encoding)
        {
        }
        #endregion

        #region utils

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="str"></param>
        public byte[] Compress(byte[] bytes)
        {
            using (var stream = new MemoryStream())
            {
                using (var gzip = new GZipStream(stream, CompressionMode.Compress, true))
                {
                    gzip.Write(bytes, 0, bytes.Length);
                    gzip.Close();
                    return stream.ToArray();
                }
            }
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="str"></param>
        public byte[] Decompress(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
                {
                    if (gzip.CanSeek)
                    {
                        var @byte = new byte[gzip.Length];
                        gzip.Position = 0;
                        gzip.Read(@byte, 0, @byte.Length);
                        gzip.Close();
                        return @byte;
                    }

                    using (var reader = new MemoryStream())
                    {
                        gzip.CopyTo(reader);
                        gzip.Close();
                        return reader.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Compress(string input, Encoding encoding = null)
        {
            var @byte = this.Compress((encoding ?? Encoding.UTF8).GetBytes(input));
            return Convert.ToBase64String(@byte);
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Decompress(string input, Encoding encoding = null)
        {
            var @byte = this.Decompress(Convert.FromBase64String(input));
            return (encoding ?? Encoding.UTF8).GetString(@byte);
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileToCompress"></param>
        public void Compress(FileInfo fileInfo, Action<FileInfo, GZipStream> action)
        {
            using (var stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var gzip = new GZipStream(stream, CompressionMode.Compress))
            {
                action(fileInfo, gzip);
            }
        }

        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="fileInfo"></param>
        public void Decompress(FileInfo fileInfo, Action<FileInfo, GZipStream> action)
        {
            using (var stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
            {
                action(fileInfo, gzip);
            }
        }

        #endregion

        #region icompressprotocol

        /// <summary>
        /// 对数据加一层信息，穿上衣服
        /// </summary>
        /// <param name="byte"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected override byte[] Clothing(byte[] @byte, out byte flag)
        {
            return base.Clothing(this.Compress(@byte), out flag);
        }

        /// <summary>
        /// 对数据脱一层信息，脱衣服
        /// </summary>
        /// <param name="byte"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected override byte[] Undressing(byte[] @byte, byte flag)
        {
            return base.Undressing(this.Decompress(@byte), flag);
        }

        #endregion
    }
}
