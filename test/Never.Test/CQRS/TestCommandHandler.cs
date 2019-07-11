using Never.Commands;
using Never.Events;
using Never.Exceptions;
using Never.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Never.Test
{
    [Aop.Logger]
    // [PermissionCommandFilter]
    //[Aop.DomainFilters.CommandHandlerFilterAttribute]
    [PermissionCommandFilter]
    public class TestCommandHandler : ICommandHandler<TestCommand>
      , ICommandHandler<TestMyCommand>, ICommandHandler<TestExceptionCommand>, IMessageSubscriber<TestMessage>
    {
        // private readonly IEventBus eventBus = null;

        public TestCommandHandler()
        {
            //this.eventBus = eventBus;
        }

        public void Execute(IMessageContext context, TestMessage e)
        {
        }

        //[PermissionCommandExcuteMethodFilterAttribute()]
        public virtual ICommandHandlerResult Execute(ICommandContext context, TestExceptionCommand command)
        {
            var root = context.GetAggregateRoot(2L, () => { return new TestAggregateRoot() { }; });
            return context.CreateResult(CommandHandlerStatus.Success);
            //context.WriteEventAndFlush(new[] { new TestEvent1() });
            //throw new Exception("this is on excetpion test");
        }

        public ICommandHandlerResult Execute(Commands.ICommandContext context, TestCommand command)
        {
            //System.Threading.Interlocked.Increment(ref TestCQRS.inter);

            //Console.WriteLine("this command is test command");
            var root = context.GetAggregateRoot(2L, () => { return new TestAggregateRoot() { }; });
            //context.WriteEvent(new[] { new TestEvent1() }, command);
            //context.WriteEvent(new[] { new TestEvent1() }, command);
            //var t = context.Flush();
            return context.CreateResult(CommandHandlerStatus.Success);
            // eventBus.AsyncPublish(context, new TestEvent1());
        }

        public ICommandHandlerResult Execute(Commands.ICommandContext context, TestMyCommand command)
        {
            var root = context.GetAggregateRoot<Guid, TestAggregateRoot>(Guid.NewGuid(), () => new TestAggregateRoot());
            Console.WriteLine("this command is test my command");
            return context.CreateResult(CommandHandlerStatus.Success);
        }
    }
}