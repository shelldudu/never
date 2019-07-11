using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Never.Web.WebApi.Encryptions
{
    /// <summary>
    /// 不授权的加密与解密
    /// </summary>
    public sealed class UnAuthorizeApiContentEncryptor : Never.Web.Encryptions.UnAuthorizeContentEncryptor, IApiContentEncryptor
    {
        /// <summary>
        /// 是否可用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool IsValid(HttpRequestMessage request)
        {
            return false;
        }
    }
}