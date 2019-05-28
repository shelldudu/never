using Never.Commands;
using Never.Events;
using Never.EventStreams;
using Never.IoC;
using Never.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace Never.TestConsole.CQRS
{
    public class TestCQRS : Program
    {
        public static long inter = 0;

        [Xunit.Fact]
        public void TestSendCommand()
        {
            var bool2 = typeof(ICommandHandler<>).IsAssignableFrom(typeof(TestCommandHandler));
            var commandBus = ContainerContext.Current.ServiceLocator.Resolve<ICommandBus>();
            //var eventBus = ContainerContext.Current.ServiceLocator.Resolve<IEventBus>();
            var cmd = new TestCommand();
            var dict = new HandlerCommunication();
            //CommandBusExtension.
            // dict["Worker"] = new TestUser();

            commandBus.Send(cmd, dict);

            Thread.Sleep(20000);
            Console.ReadLine();
        }

        [Fact]
        public void TestSendExcetionCommand()
        {
            var commandBus = ContainerContext.Current.ServiceLocator.Resolve<ICommandBus>();
            var cmd = new TestExceptionCommand();
            var dic = new Dictionary<string, object>();

            //commandBus.Send(cmd, dic);
            //dic["gg"] = cmd.CommandId;
            //commandBus.Send(new TestMyCommand() { }, dic);

            Console.ReadLine();

            Thread.Sleep(1000);
        }

        [Fact]
        public void TestTwoCommand()
        {
            var cmd1 = new TestCommand();
            var cmd2 = new TestMyCommand();
            var commandBus = ContainerContext.Current.ServiceLocator.Resolve<ICommandBus>();

            commandBus.Send(cmd1, null);
            Console.ReadLine();
        }

        [Fact]
        public void TestDomain()
        {
            var root = new TestAggregateRoot();
            var e = new TestEvent1();
            root.ReplyEvent(new[] { e });
            root.ReplyEvent(new[] { e });
        }

        [Xunit.Fact]
        public void TeseSerialCommand()
        {
            var command = new TestCommand()
            {
                Id = 4,
                Version = 0
            };

            var commandbus = this.Resolve<ICommandBus>();
            var count = 1;
            var time = 1000;
            var threads = new System.Threading.Thread[count];
            for (var i = 0; i < count; i++)
            {
                threads[i] = new System.Threading.Thread(() =>
                {
                    for (var j = 0; j < time; j++)
                    {
                        commandbus.Send(command);
                    }
                });
            }

            for (var i = 0; i < count; i++)
                threads[i].Start();

            Console.ReadLine();
            Console.WriteLine(inter);
        }

        [Xunit.Fact]
        public void TestSendMessage()
        {
            var message = new TestMessage();
            var publisher = this.Resolve<IMessagePublisher>();
            publisher.Publish(new DefaultMessageContext(), message);
        }
    }
}