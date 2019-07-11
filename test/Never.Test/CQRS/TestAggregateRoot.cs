using System;
using System.Collections.Generic;
using System.Text;

namespace Never.Test
{
    public class TestAggregateRoot : Never.Domains.AggregateRoot<Guid>, Never.Domains.IHandle<TestEvent1>
    {
        public TestAggregateRoot()
            : base(Guid.NewGuid())
        {
            this.ApplyEvent(new TestEvent1());
        }

        public void Handle(TestEvent1 e)
        {

        }
    }
}
