using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Never.Web.Encryptions
{
    /// <summary>
    /// 加密与解密内容
    /// </summary>
    public interface IContentEncryptor
    {
        /// <summary>
        /// 在Get方法下指定参数名字
        /// </summary>
        string ParamKey { get; }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string Encrypt(string text);

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        byte[] Encrypt(byte[] buffer);

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text">The stream.</param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        Stream Encrypt(string text, ICollection<string> contentEncoding);

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text">The stream.</param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        Stream Decrypt(string text, ICollection<string> contentEncoding);

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        Stream Encrypt(byte[] buffer, ICollection<string> contentEncoding);

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        Stream Decrypt(byte[] buffer, ICollection<string> contentEncoding);

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string Decrypt(string text);

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        byte[] Decrypt(byte[] buffer);
    }
}