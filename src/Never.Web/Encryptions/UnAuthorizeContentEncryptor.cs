using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Never.Web.Encryptions
{
    /// <summary>
    /// 不授权的加密与解密
    /// </summary>
    public class UnAuthorizeContentEncryptor : IContentEncryptor
    {
        /// <summary>
        /// 在Get方法下指定参数名字，在Post因是直接post内容过来，因此不用该值去获取数据
        /// </summary>
        public string ParamKey { get; set; }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Decrypt(string text)
        {
            return text;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] buffer)
        {
            return buffer;
        }


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Encrypt(string text)
        {
            return text;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] buffer)
        {
            return buffer;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text">The stream.</param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Stream Encrypt(string text, ICollection<string> contentEncoding)
        {
            return null;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text">The stream.</param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        public Stream Decrypt(string text, ICollection<string> contentEncoding)
        {
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        public Stream Encrypt(byte[] buffer, ICollection<string> contentEncoding)
        {
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        public Stream Decrypt(byte[] buffer, ICollection<string> contentEncoding)
        {
            return null;
        }
    }
}