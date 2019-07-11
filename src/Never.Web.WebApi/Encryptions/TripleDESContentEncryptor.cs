using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Never.Web.WebApi.Encryptions
{
    /// <summary>
    /// 3des加密
    /// </summary>
    /// <seealso cref="Never.Web.WebApi.Encryptions.DefaultContentEncryptor" />
    public class TripleDESContentEncryptor : DefaultContentEncryptor
    {
        #region field

        private readonly string SecurityKey = string.Empty;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="TripleDESContentEncryptor"/> class.
        /// </summary>
        /// <param name="securityKey">The security key.</param>
        public TripleDESContentEncryptor(string securityKey)
        {
            this.SecurityKey = securityKey;
        }

        #endregion ctor

        /// <summary>
        /// Determines whether the specified request is valid.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public override bool IsValid(System.Net.Http.HttpRequestMessage request)
        {
            if (request.Method == HttpMethod.Get && this.ParamKey.IsNotNullOrEmpty() && request.RequestUri.Query.IsNullOrEmpty())
                return true;

            if (request.Method == HttpMethod.Post)
                return true;

            return base.IsValid(request);
        }

        /// <summary>
        /// Decrypts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public override string Decrypt(string text)
        {
            if (text.IsNullOrEmpty())
                return text;

            return text.From3DES(this.SecurityKey);
        }

        /// <summary>
        /// Encrypts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public override string Encrypt(string text)
        {
            if (text.IsNullOrEmpty())
                return text;

            return text.To3DES(this.SecurityKey);
        }

        /// <summary>
        /// Decrypts the specified byte[].
        /// </summary>
        /// <param name="byte"></param>
        /// <returns></returns>
        public override byte[] Decrypt(byte[] @byte)
        {
            if (@byte == null || @byte.Length == 0)
                return @byte;

            var @char = new char[@byte.Length];
            @byte.CopyTo(@char, 0);

            return Convert.FromBase64CharArray(@char, 0, @char.Length).From3DES(this.SecurityKey);
        }

        /// <summary>
        /// Encrypts the specified byte[].
        /// </summary>
        /// <param name="byte"></param>
        /// <returns></returns>
        public override byte[] Encrypt(byte[] @byte)
        {
            if (@byte == null || @byte.Length == 0)
                return @byte;

            var buffer = @byte.To3DES(this.SecurityKey);
            var arrayLength = (long)((4.0d / 3.0d) * buffer.Length);
            if (arrayLength % 4 != 0)
                arrayLength += 4 - arrayLength % 4;

            var @char = new char[arrayLength];
            var length = Convert.ToBase64CharArray(buffer, 0, buffer.Length, @char, 0);
            buffer = new byte[length];
            for (var i = 0; i < @char.Length && i < length; i++)
                buffer[i] = (byte)@char[i];

            return buffer;
        }
    }
}