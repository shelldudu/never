using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test
{
    public class BufferQueueTest
    {
        [Xunit.Fact]
        public void TestIBufferQueue()
        {
            var buffer = new Never.Collections.SingleTrackNodeBufferQueue<int>();
            for (var i = 0; i < 10; i++)
                buffer.Enqueue(i);

            //var a = 0;
            //buffer.Dequeue(out a);
            //buffer.Dequeue(out a);
            //buffer.Dequeue(out a);

            //for (var i = 0; i < 4; i++)
            //    buffer.Enqueue(i);

            foreach (var i in buffer)
            {
                Console.WriteLine(i.ToString());
            };

            //Console.ReadLine();
            //int re = 0;
            //var r = buffer.Dequeue(out re);
        }

        [Xunit.Fact]
        public void TestBufferStack()
        {
            var buffer = new Never.Collections.SingleTrackNodeBufferStack<int>();
            for (var i = 0; i < 10; i++)
                buffer.Push(i);

            foreach (var i in buffer)
            {
                Console.WriteLine(i.ToString());
            };

            Console.ReadLine();
            //int re = 0;
            //var r = buffer.Peek(out re);
        }
    }
}