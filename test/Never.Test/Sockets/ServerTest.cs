using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test
{
    public class ServerTest
    {
        public static System.Threading.AutoResetEvent alldone = new System.Threading.AutoResetEvent(false);

        public void RunAsAsync()
        {
            var host = "127.0.0.1";
            var port = 2056;
            var ip = IPAddress.Parse(host);
            var ed = new IPEndPoint(ip, port);

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ed);
            socket.Listen(0);

            //try
            //{
            //    while (true)
            //    {
            //        alldone.Reset();
            //        Console.WriteLine("等待下一下连接.....");
            //        socket.BeginAccept(new AsyncCallback((ar) =>
            //        {
            //            alldone.Set();
            //            var listener = (Socket)ar.AsyncState;
            //            var handler = listener.EndAccept(ar);
            //            handler.BeginReceive(null, 0)
            //        }), socket);
            //    }
            //}
            //catch
            //{
            //}
        }

        public void Run()
        {
            var host = "127.0.0.1";
            var port = 2056;
            var ip = IPAddress.Parse(host);
            var ed = new IPEndPoint(ip, port);

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ed);
            socket.Listen(0);

            Console.WriteLine("等待客户端连接");
            var accept = socket.Accept();
            Console.WriteLine("建立连接");
            var buffer = new byte[1024];
            var bytes = accept.Receive(buffer);
            Console.WriteLine("内容如下:");
            Console.WriteLine(Encoding.UTF8.GetString(buffer));

            var content = Encoding.UTF8.GetBytes("ok");
            accept.Send(content);
            accept.Close();
            socket.Close();

            Console.WriteLine("发射完成，请按任何键退出");
            Console.ReadLine();
        }
    }
}