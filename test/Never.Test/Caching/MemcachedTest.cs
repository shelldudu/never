using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test
{
    public class MemcachedTest
    {
        private static readonly string host = "127.0.0.1:11211";

        [Xunit.Fact]
        public void TestSize()
        {
            var size = Never.Memcached.BinaryProtocols.RequestHeader.Size();
            size = Never.Memcached.BinaryProtocols.ResponseHeader.Size();

            var reqs = new Never.Memcached.BinaryProtocols.RequestHeader()
            {
                Magic = Memcached.BinaryProtocols.Magic.Request,
                OpCode = Memcached.Command.add,
                KeyLength = 6,
                DataType = 0,
                CAS = 0,
                Opaque = 0,
                ExtraLength = 0,
                TotalBody = 6,
                VbucketId = 1
            };

            var @byte = reqs.ToByte();
            var req = @byte.TryFromByte(out Never.Memcached.BinaryProtocols.ResponseHeader sizez);
        }

        [Xunit.Fact]
        public void TestAddValueOnTextMode()
        {
            var client = Never.Memcached.MemcachedClient.CreateBinaryCached(new[] { host }, Encoding.UTF8, new Never.Memcached.GZipCompressProtocol(Encoding.UTF8), new Memcached.SocketSetting() { MaxPoolBufferSize = 1 });
            client.TrySetResult("Hello", "text", TimeSpan.FromHours(2));
            var a = client.TryGetResult("Hello", out string aec);
            a = client.TryGetResult("Hello", out int ac);
            a = client.TryGetValue("Hello", out object aac);
        }
    }
}
