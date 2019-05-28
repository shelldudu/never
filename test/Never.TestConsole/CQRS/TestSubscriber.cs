using Never.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Never.TestConsole.CQRS
{
    public class TestSubscriber : IMessageSubscriber<TestMessage>
    {
        public void Execute(IMessageContext context, TestMessage e)
        {
        }
    }
}