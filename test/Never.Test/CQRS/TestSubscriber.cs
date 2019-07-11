using Never.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Never.Test
{
    public class TestSubscriber : IMessageSubscriber<TestMessage>
    {
        public void Execute(IMessageContext context, TestMessage e)
        {
        }
    }
}