using Never.Web.Encryptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Never.Web.WebApi.Encryptions
{
    /// <summary>
    /// 加密与解密内容
    /// </summary>
    public interface IApiContentEncryptor : IContentEncryptor
    {
        /// <summary>
        /// 当前是否可以适用加密与解密
        /// </summary>
        /// <param name="request"></param>
        bool IsValid(HttpRequestMessage request);
    }
}