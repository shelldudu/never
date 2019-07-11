using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Never.Test
{
    public class TestCommand : Domains.GuidAggregateCommand, Never.Commands.IAbortedSerialCommand, DataAnnotations.IValidator<TestCommand>
    {
        public TestCommand() : base(Guid.Empty)
        {
        }

        public string Body
        {
            get
            {
                return this.Id.ToString();
            }
        }

        public int Id { get; set; }

        public IEnumerable<KeyValuePair<Expression<System.Func<TestCommand, object>>, string>> Validate()
        {
            if (this.Id < 0)
                yield return new KeyValuePair<Expression<System.Func<TestCommand, object>>, string>(t => t.Id, "Id不可能小于0");
        }
    }

    public class TestMyCommand : Domains.GuidAggregateCommand
    {
        public TestMyCommand() : base(Guid.Empty)
        {
        }

        //public int Version { get; set; }
    }

    public class TestExceptionCommand : Domains.GuidAggregateCommand
    {
        public TestExceptionCommand() : base(Guid.Empty)
        {
        }

        //public int Version { get; set; }
    }
}