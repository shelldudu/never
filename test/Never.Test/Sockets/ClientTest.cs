using Never.Remoting;
using Never.Remoting.Http;
using System;
using System.IO;
using System.Text;

namespace Never.Test
{
    public class ClientTest
    {
        public void TestData()
        {
            var usertoken = new System.Collections.Concurrent.ConcurrentQueue<byte[]>();
            var protocol = new Never.Sockets.AsyncArgs.Connection.DataProtocol();
            var helloworld = Encoding.UTF8.GetBytes("hello world");

            var split = helloworld.Split(2);
            var data = protocol.To(helloworld);
            usertoken.Enqueue(data);

            var next = protocol.From(usertoken);
            var tip = Encoding.UTF8.GetString(next);

            var l = BitConverter.GetBytes(Encoding.UTF8.GetBytes("hello world").Length);
            var s1 = Encoding.UTF8.GetBytes("hello ");
            var s2 = Encoding.UTF8.GetBytes("world");
            usertoken.Enqueue(l);
            usertoken.Enqueue(s1);
            usertoken.Enqueue(s2);

            next = protocol.From(usertoken);
            tip = Encoding.UTF8.GetString(next);

            usertoken.Enqueue(l);
            usertoken.Enqueue(s2);

            next = protocol.From(usertoken);

            usertoken.Enqueue(s1);
            next = protocol.From(usertoken);
            tip = Encoding.UTF8.GetString(next);
        }

        public void TestEmpty()
        {
            var value = string.Empty;
            var len = BitConverter.GetBytes(value.Length);
            var data = Encoding.UTF8.GetBytes(value);

            var s = ObjectExtension.Combine(len, data);
            var usertoken = new System.Collections.Concurrent.ConcurrentQueue<byte[]>();
            var protocol = new Never.Sockets.AsyncArgs.Connection.DataProtocol();
            usertoken.Enqueue(s);
            usertoken.Enqueue(Encoding.UTF8.GetBytes("hello world"));
            var next = protocol.From(usertoken);
            var tip = Encoding.UTF8.GetString(next);
        }

        public void TestRemoteProtocol()
        {
            var protocol = new Protocol();
            var request = new CurrentRequest()
            {
                Request = new Request(Encoding.UTF8, "test"),
                Id = 636
            };

            ((Request)request.Request).Writer.Write("abc");

            var bytes = protocol.FromRequest(request);
            var request2 = protocol.ToRequest(new Never.Sockets.AsyncArgs.OnReceivedSocketEventArgs(null, bytes) { });
            var abc = Encoding.UTF8.GetString((((Request)request2.Request).Body as MemoryStream).ToArray());

            var response1 = protocol.ToResponse(new Never.Sockets.AsyncArgs.OnReceivedSocketEventArgs(null, bytes) { });
            abc = Encoding.UTF8.GetString((((Response)response1.Response).Body as MemoryStream).ToArray());
            var response = new CurrentResponse()
            {
                Response = new Response(Encoding.UTF8, "test"),
                Id = 636,
            };

            var handlerresponse = new ResponseResult()
            {
                Query = new System.Collections.Specialized.NameValueCollection(),
            };

            handlerresponse.Query.Add("abc", "efg");
            handlerresponse.Body = new MemoryStream(Encoding.UTF8.GetBytes("abc"));

            bytes = protocol.FromResponse(request, handlerresponse);
            var response2 = protocol.ToResponse(new Never.Sockets.AsyncArgs.OnReceivedSocketEventArgs(null, bytes) { });
            abc = Encoding.UTF8.GetString((((Response)response2.Response).Body as MemoryStream).ToArray());
        }
    }
}