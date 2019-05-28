using Never.Aop;
using Never.Aop.DomainFilters;
using Never.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Never.TestConsole.CQRS
{
    [Aop.DomainFilters.EventHandlerPriority(Order = 2)]
    //[OverallBlock(BlockSubject = OverallBlockSubject.ActionExecuting)]
    public class TestEventHandler1 : Never.Events.IEventHandler<TestEvent1>, IEventHandler<TestEvent2>
    {
        public TestEventHandler1()
        {
        }

        public void Execute(IEventContext context, TestEvent2 e)
        {
            Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        public void Execute(Events.IEventContext context, TestEvent1 e)
        {
            Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("e_1_one");
            //context.AddCommand(new TestCommand());
        }

        public void Execute(IEventContext context, TestEvent1 e1, TestEvent2 e2)
        {
            //Console.WriteLine("1_two");
        }
    }

    [Aop.DomainFilters.EventHandlerPriority(Order = 1)]
    //[LogEventFilterAttribute]
    //[OverallBlock(BlockSubject = OverallBlockSubject.All)]
    public class TestEventHandler2 : Never.Events.IEventHandler<TestEvent1>, Never.Events.IEventHandler<TestEvent2>
    {
        [LogEventFilterAttribute]
        public void Execute(Events.IEventContext context, TestEvent1 e)
        {
            Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("e_2_one");
        }

        [LogEventFilterAttribute]
        public void Execute(Events.IEventContext context, TestEvent2 e)
        {
            Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("e_2_one");
        }
    }
}