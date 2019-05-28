using Never.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Never.TestConsole.CQRS
{
    [EventStreams.EventDomain(Domain = "app")]
    public class TestEvent1 : Never.Events.IEvent
    {
        public int Version { get; set; }
    }

    public class TestEvent2 : AggregateRootCreateEvent<Guid>
    {
    }

    public class TestConvert
    {
        [Xunit.Fact]
        public void TestConvertChange()
        {
            var txt = "Convert";
            int time = 10000000;
            IEvent e = new TestEvent1();
            using (var t = new Never.Utils.MethodTickCount((x) => { Console.WriteLine(x); }))
            {
            }

            using (var t = new Never.Utils.MethodTickCount((x) => { Console.WriteLine(x); }))
            {
                var type = e.GetType();
                for (var i = 1; i <= time; i++)
                {
                    //Handle((dynamic)Convert.ChangeType(e, type));
                }
            }

            using (var t = new Never.Utils.MethodTickCount((x) => { Console.WriteLine(x); }))
            {
                for (var i = 1; i <= time; i++)
                {
                    //Handle((dynamic)e);
                }
            }
        }

        public static void Handle(TestEvent1 e)
        {
            if (e == null)
                Console.WriteLine("null");
        }
    }
}