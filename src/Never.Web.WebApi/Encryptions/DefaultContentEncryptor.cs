using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Never.Web.WebApi.Encryptions
{
    /// <summary>
    /// 默认加密行为
    /// </summary>
    public abstract class DefaultContentEncryptor : IApiContentEncryptor
    {
        /// <summary>
        /// 在Get方法下指定参数名字，在Post因是直接post内容过来，因此不用该值去获取数据
        /// </summary>
        public string ParamKey { get; set; }

        /// <summary>
        /// 当前是否可以适用加密与解密
        /// </summary>
        /// <param name="request"></param>
        public virtual bool IsValid(HttpRequestMessage request)
        {
            if (request.Method == HttpMethod.Get && this.ParamKey.IsNotNullOrEmpty() && request.RequestUri.Query.IsNullOrEmpty())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public abstract string Decrypt(string text);

        /// <summary>
        ///
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public abstract byte[] Decrypt(byte[] text);

        /// <summary>
        ///
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public abstract string Encrypt(string text);

        /// <summary>
        ///
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public abstract byte[] Encrypt(byte[] text);

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text">The stream.</param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual Stream Encrypt(string text, ICollection<string> contentEncoding)
        {
            var encoding = this.GetEncoding(contentEncoding) ?? Encoding.UTF8;
            return new MemoryStream(encoding.GetBytes(this.Encrypt(text)));
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual Stream Encrypt(byte[] buffer, ICollection<string> contentEncoding)
        {
            return new MemoryStream(this.Encrypt(buffer));
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text">The stream.</param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        public virtual Stream Decrypt(string text, ICollection<string> contentEncoding)
        {
            var encoding = this.GetEncoding(contentEncoding) ?? Encoding.UTF8;
            return new MemoryStream(encoding.GetBytes(this.Decrypt(text)));
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        public virtual Stream Decrypt(byte[] buffer, ICollection<string> contentEncoding)
        {
            return new MemoryStream(this.Decrypt(buffer));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        protected virtual Encoding GetEncoding(ICollection<string> contentEncoding)
        {
            var encoding = Encoding.UTF8;
            if (contentEncoding.IsNotNullOrEmpty())
            {
                foreach (var i in contentEncoding)
                {
                    encoding = Encoding.GetEncoding(i);
                    if (encoding != null)
                    {
                        break;
                    }
                }
            }

            return encoding;
        }
    }
}