using Never;
using Never.Sockets.AsyncArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Remoting
{
    /// <summary>
    /// 数据协议
    /// </summary>
    public interface IRemoteProtocol
    {
        /// <summary>
        /// 【客户端】=>【服务端】 将当前请求参数，转成socket协议所要求的参数，步骤【一】
        /// </summary>
        /// <param name="currentRequest"></param>
        /// <returns></returns>
        byte[] FromRequest(CurrentRequest currentRequest);

        /// <summary>
        /// 【服务端】=> 【服务端】socket协议得到的数据转成当前请求参数，步骤【二】
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        CurrentRequest ToRequest(OnReceivedSocketEventArgs e);

        /// <summary>
        /// 【服务端】=> 【客户端】将处理到得的参数，转成socket协议所要求的参数，步骤【三】
        /// </summary>
        /// <param name="currentRequest"></param>
        /// <param name="responseResult"></param>
        /// <returns></returns>
        byte[] FromResponse(CurrentRequest currentRequest, IResponseHandlerResult responseResult);


        /// <summary>
        /// 【客户端】socket协议得到的数据转成当前请求响应，步骤【四】
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>

        CurrentResponse ToResponse(OnReceivedSocketEventArgs e);
    }

    /// <summary>
    /// 当前请求
    /// </summary>
    public struct CurrentRequest
    {
        /// <summary>
        /// 请求Id
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// 请求
        /// </summary>
        public IRemoteRequest Request { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="idgenera"></param>
        public CurrentRequest(IRemoteRequest request, Func<ulong> idgenera)
        {
            this.Request = request;
            this.Id = idgenera();
        }
    }

    /// <summary>
    /// 当前响应
    /// </summary>
    public struct CurrentResponse
    {
        /// <summary>
        /// 请求Id
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// 请求
        /// </summary>
        public IRemoteResponse Response { get; set; }
    }
}
