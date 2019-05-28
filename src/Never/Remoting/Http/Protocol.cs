using Never;
using Never.Sockets.AsyncArgs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Remoting.Http
{
    /// <summary>
    /// 数据协议
    /// </summary>
    public class Protocol : IRemoteProtocol
    {
        #region utils

        private byte[] ToEncodeNameValueCollection(Encoding encoding, NameValueCollection collection)
        {
            var count = collection != null ? collection.Count : 0;
            var countbytes = BitConverter.GetBytes(count);
            var bytes = new List<byte[]>();
            bytes.Add(countbytes);
            if (count > 0)
            {
                foreach (var key in collection.AllKeys)
                {
                    var value = collection[key];
                    var keylength = BitConverter.GetBytes(key.Length);
                    var keydatas = encoding.GetBytes(key);
                    var valuelength = value == null ? BitConverter.GetBytes(0) : BitConverter.GetBytes(value.Length);
                    var valuedatas = value == null ? encoding.GetBytes(string.Empty) : encoding.GetBytes(value);
                    bytes.Add(keylength);
                    bytes.Add(keydatas);
                    bytes.Add(valuelength);
                    bytes.Add(valuedatas);
                }
            }

            return ObjectExtension.Combine(bytes);
        }

        private short DecodeInt16(byte[] buffer, int offset, out int nextoffect)
        {
            var values = new byte[sizeof(short)];
            Buffer.BlockCopy(buffer, offset, values, 0, sizeof(short));
            offset += sizeof(short);

            nextoffect = offset;
            return BitConverter.ToInt16(values, 0);
        }

        private int DecodeInt32(byte[] buffer, int offset, out int nextoffect)
        {
            var values = new byte[sizeof(int)];
            Buffer.BlockCopy(buffer, offset, values, 0, sizeof(int));
            offset += sizeof(int);

            nextoffect = offset;
            return BitConverter.ToInt32(values, 0);
        }

        private long DecodeInt64(byte[] buffer, int offset, out int nextoffect)
        {
            var values = new byte[sizeof(long)];
            Buffer.BlockCopy(buffer, offset, values, 0, sizeof(long));
            offset += sizeof(long);

            nextoffect = offset;
            return BitConverter.ToInt64(values, 0);
        }

        private ushort DecodeUInt16(byte[] buffer, int offset, out int nextoffect)
        {
            var values = new byte[sizeof(ushort)];
            Buffer.BlockCopy(buffer, offset, values, 0, sizeof(ushort));
            offset += sizeof(ushort);

            nextoffect = offset;
            return BitConverter.ToUInt16(values, 0);
        }

        private uint DecodeUInt32(byte[] buffer, int offset, out int nextoffect)
        {
            var values = new byte[sizeof(uint)];
            Buffer.BlockCopy(buffer, offset, values, 0, sizeof(uint));
            offset += sizeof(uint);

            nextoffect = offset;
            return BitConverter.ToUInt32(values, 0);
        }

        private ulong DecodeUInt64(byte[] buffer, int offset, out int nextoffect)
        {
            var values = new byte[sizeof(ulong)];
            Buffer.BlockCopy(buffer, offset, values, 0, sizeof(ulong));
            offset += sizeof(ulong);

            nextoffect = offset;
            return BitConverter.ToUInt64(values, 0);
        }

        private Encoding DecodeEncoding(byte[] buffer, Encoding defaultEncoding, int offset, out int nextoffect)
        {
            var offsetcount = sizeof(int);
            var values = new byte[offsetcount];
            Buffer.BlockCopy(buffer, offset, values, 0, offsetcount);
            offset += offsetcount;
            offsetcount = BitConverter.ToInt32(values, 0);
            values = new byte[offsetcount];
            Buffer.BlockCopy(buffer, offset, values, 0, offsetcount);
            offset += offsetcount;

            nextoffect = offset;
            return Encoding.GetEncoding(defaultEncoding.GetString(values));
        }

        private string DecodeString(byte[] buffer, Encoding encoding, int offset, out int nextoffect)
        {
            var offsetcount = sizeof(int);
            var values = new byte[offsetcount];
            Buffer.BlockCopy(buffer, offset, values, 0, offsetcount);
            offset += offsetcount;
            offsetcount = BitConverter.ToInt32(values, 0);
            values = new byte[offsetcount];
            Buffer.BlockCopy(buffer, offset, values, 0, offsetcount);
            offset += offsetcount;

            nextoffect = offset;
            return encoding.GetString(values);
        }

        private NameValueCollection DecodeNameValueCollection(byte[] buffer, Encoding encoding, int offset, out int nextoffect)
        {
            var offsetcount = sizeof(int);
            var values = new byte[offsetcount];
            Buffer.BlockCopy(buffer, offset, values, 0, offsetcount);
            offset += offsetcount;
            offsetcount = BitConverter.ToInt32(values, 0);
            var collection = new NameValueCollection(offsetcount);
            for (var i = 0; i < offsetcount; i++)
            {
                var key = this.DecodeString(buffer, encoding, offset, out offset);
                var value = this.DecodeString(buffer, encoding, offset, out offset);
                collection.Add(key, value);
            }

            nextoffect = offset;
            return collection;
        }

        #endregion

        #region IRemoteProtocol
        /// <summary>
        /// 【客户端】=>【服务端】 将当前请求参数，转成socket协议所要求的参数，步骤【一】
        /// </summary>
        /// <param name="currentRequest"></param>
        /// <returns></returns>
        public byte[] FromRequest(CurrentRequest currentRequest)
        {
            var request = currentRequest.Request as Request;
            if (request.Writer != null)
                request.Writer.Flush();

            var idbytes = BitConverter.GetBytes(currentRequest.Id);

            var encodinglength = BitConverter.GetBytes(request.Encoding.BodyName.Length);
            var encodingbytes = Encoding.ASCII.GetBytes(request.Encoding.BodyName);

            var commandlength = BitConverter.GetBytes(request.CommandType.Length);
            var commandbytes = request.Encoding.GetBytes(request.CommandType);

            var querydatas = this.ToEncodeNameValueCollection(request.Encoding, request.Query);
            var formdatas = this.ToEncodeNameValueCollection(request.Encoding, request.Form);
            var headerdatas = this.ToEncodeNameValueCollection(request.Encoding, request.Headers);

            var memorystream = request.Body as MemoryStream;
            var bodylength = BitConverter.GetBytes(memorystream.Length);
            var bodydatas = memorystream.ToArray();

            return ObjectExtension.Combine(idbytes, encodinglength, encodingbytes, commandlength, commandbytes, querydatas, formdatas, headerdatas, bodylength, bodydatas);
        }

        /// <summary>
        /// 【服务端】=> 【服务端】socket协议得到的数据转成当前请求参数，步骤【二】
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public CurrentRequest ToRequest(OnReceivedSocketEventArgs e)
        {
            int offset = 0;
            var currentRequest = new CurrentRequest()
            {
                Id = this.DecodeUInt64(e.Buffer, offset, out offset),
            };

            //读取encoding
            var encoding = this.DecodeEncoding(e.Buffer, Encoding.ASCII, offset, out offset);

            //读取命令
            var command = this.DecodeString(e.Buffer, encoding, offset, out offset);

            //设置命令
            var request = new Request(encoding, command);
            currentRequest.Request = request;

            request.Query = this.DecodeNameValueCollection(e.Buffer, encoding, offset, out offset);
            request.Form = this.DecodeNameValueCollection(e.Buffer, encoding, offset, out offset);
            request.Headers = this.DecodeNameValueCollection(e.Buffer, encoding, offset, out offset);

            //设置body
            var bodylength = this.DecodeInt64(e.Buffer, offset, out offset);
            request.Writer.Write(encoding.GetChars(e.Buffer, offset, e.Buffer.Length - offset));
            request.Writer.Flush();

            return currentRequest;
        }

        /// <summary>
        /// 【服务端】=> 【客户端】将处理到得的参数，转成socket协议所要求的参数，步骤【三】
        /// </summary>
        /// <param name="currentRequest"></param>
        /// <param name="responseResult"></param>
        /// <returns></returns>
        public byte[] FromResponse(CurrentRequest currentRequest, IResponseHandlerResult responseResult)
        {
            var response = responseResult as ResponseResult;
            if (response.Body != null)
                response.Body.Flush();

            var request = currentRequest.Request as Request;
            var idbytes = BitConverter.GetBytes(currentRequest.Id);

            var encodinglength = BitConverter.GetBytes(request.Encoding.BodyName.Length);
            var encodingbytes = Encoding.ASCII.GetBytes(request.Encoding.BodyName);

            var commandlength = BitConverter.GetBytes(request.CommandType.Length);
            var commandbytes = request.Encoding.GetBytes(request.CommandType);

            var querydatas = this.ToEncodeNameValueCollection(request.Encoding, response.Query);
            var formdatas = this.ToEncodeNameValueCollection(request.Encoding, response.Form);
            var headerdatas = this.ToEncodeNameValueCollection(request.Encoding, response.Headers);

            var bodylength = response.Body == null ? BitConverter.GetBytes(0L) : BitConverter.GetBytes(response.Body.Length);
            var bodydatas = response.Body == null ? new byte[0] : response.Body.ToArray();

            return ObjectExtension.Combine(idbytes, encodinglength, encodingbytes, commandlength, commandbytes, querydatas, formdatas, headerdatas, bodylength, bodydatas);
        }

        /// <summary>
        /// 【客户端】socket协议得到的数据转成当前请求响应，步骤【四】
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>

        public CurrentResponse ToResponse(OnReceivedSocketEventArgs e)
        {
            int offset = 0;
            var resp = new CurrentResponse()
            {
                Id = this.DecodeUInt64(e.Buffer, offset, out offset),
            };

            //读取encoding
            var encoding = this.DecodeEncoding(e.Buffer, Encoding.ASCII, offset, out offset);

            //读取命令
            var command = this.DecodeString(e.Buffer, encoding, offset, out offset);

            //设置命令
            var response = new Response(encoding, command);
            resp.Response = response;

            response.Query = this.DecodeNameValueCollection(e.Buffer, encoding, offset, out offset);
            response.Form = this.DecodeNameValueCollection(e.Buffer, encoding, offset, out offset);
            response.Headers = this.DecodeNameValueCollection(e.Buffer, encoding, offset, out offset);

            //设置body
            var bodylength = this.DecodeInt64(e.Buffer, offset, out offset);
            if (bodylength > 0)
            {
                var memorystream = new MemoryStream(e.Buffer, offset, e.Buffer.Length - offset);
                memorystream.Flush();
                response.Body = memorystream;
            }

            return resp;
        }

        #endregion IRemoteProtocol
    }
}
