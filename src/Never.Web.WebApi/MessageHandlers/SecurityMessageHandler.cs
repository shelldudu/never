using Never.Web.Encryptions;
using Never.Web.WebApi.Encryptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Never.Web.WebApi.MessageHandlers
{
    /// <summary>
    /// 加密数据和解密数据消息处理
    /// </summary>
    public class SecurityMessageHandler : DelegatingHandler
    {
        #region prop

        /// <summary>
        /// The find security model callback
        /// </summary>
        private readonly Func<HttpRequestMessage, IApiContentEncryptor> findSecurityModelCallback = null;

        /// <summary>
        /// The get method dictionary
        /// </summary>
        private readonly IDictionary<string, Regex> getMethodDict = null;

        #endregion prop

        #region ctor

        internal SecurityMessageHandler(Func<HttpRequestMessage, IApiContentEncryptor> findSecurityModelCallback)
        {
            this.findSecurityModelCallback = findSecurityModelCallback;
            this.getMethodDict = new System.Collections.Concurrent.ConcurrentDictionary<string, Regex>(4 * System.Environment.ProcessorCount, 100);
        }

        #endregion ctor

        /// <summary>
        /// Sends the asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            /*获取解密的key，如果没有token，则表示不用加密与解密，此时走父类行为则可*/
            if (this.findSecurityModelCallback == null)
                return base.SendAsync(request, cancellationToken);

            var encryption = this.findSecurityModelCallback(request);
            if (encryption == null)
                return base.SendAsync(request, cancellationToken);

            if (encryption is UnAuthorizeApiContentEncryptor || encryption is UnAuthorizeContentEncryptor)
            {
                var task = new TaskCompletionSource<HttpResponseMessage>();
                task.SetResult(new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized) { ReasonPhrase = "Unauthorized" });
                return task.Task;
            }

            /*如果不是要加密的，返回数据*/
            if (!encryption.IsValid(request))
                return base.SendAsync(request, cancellationToken);

            /*uri*/
            this.RewriteUri(request, encryption);

            /*content*/
            this.RewriteContent(request, encryption);

            return base.SendAsync(request, cancellationToken).ContinueWith(task =>
            {
                /*出现异常的情况就不用处理了*/
                if (task.Result.StatusCode != System.Net.HttpStatusCode.OK)
                    return task.Result;

                /*最后加密*/
                task.Result.Content = new LazyStreamContent(request, encryption).Load(task.Result.Content);
                return task.Result;
            });
        }

        #region 重写Uri

        /// <summary>
        /// 重写Uri
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="encryption">解密的model</param>
        protected void RewriteUri(HttpRequestMessage request, IApiContentEncryptor encryption)
        {
            Regex reg = null;
            if (!getMethodDict.TryGetValue(encryption.ParamKey, out reg))
            {
                reg = new Regex(string.Format("(?<split>[&?])+{0}=(?<security>[^&]*)", encryption.ParamKey), RegexOptions.Compiled | RegexOptions.IgnoreCase);
                getMethodDict[encryption.ParamKey] = reg;
            }

            request.RequestUri = new Uri(reg.Replace(request.RequestUri.OriginalString, o =>
            {
                return string.Concat(o.Groups["split"].Value, string.Concat(encryption.ParamKey, "=", encryption.Decrypt(o.Groups["security"].Value)));
            }));
        }

        #endregion 重写Uri

        #region 重写content

        /// <summary>
        /// Rewrites the content.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="encryption">解密的model</param>
        protected void RewriteContent(HttpRequestMessage request, IApiContentEncryptor encryption)
        {
            request.Content = new LazyStreamContent(request, encryption).Load();
        }

        #endregion 重写content
    }
}