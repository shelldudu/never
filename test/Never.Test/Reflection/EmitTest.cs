using Never.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test
{
    public class EmitTest
    {
        [Xunit.Fact]
        public void TestCtor()
        {
            var emit = EasyEmitBuilder<Func<MyCommand>>.NewDynamicMethod();
            emit.NewObject(typeof(MyCommandTwo), Type.EmptyTypes);
            emit.Return();

            var com = emit.CreateDelegate()();
        }
    }

    public class MyCommand
    {
        public int Id { get; set; }

        protected MyCommand()
        {
        }
    }

    public class MyCommandTwo : MyCommand
    {
        public MyCommandTwo()
        {
        }
    }
}