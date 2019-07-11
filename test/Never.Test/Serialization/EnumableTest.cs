using Never.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test
{
    /// <summary>
    ///
    /// </summary>
    public class EnumableTest
    {
        [Xunit.Fact]
        public void Test0()
        {
            int?[][] vs = new int?[2][];
            vs[0] = new int?[2];
            vs[0][0] = 0;
            vs[0][1] = 1;
            vs[1] = new int?[2];
            vs[1][0] = 1;
            vs[1][1] = 1;

            var ser = EasyJsonSerializer.Serialize(vs);
            Console.WriteLine(ser);

            vs = EasyJsonSerializer.Deserialize<int?[][]>(ser);
        }

        [Xunit.Fact]
        public void Test1()
        {
            IEnumerable<int[][]> vs = null;
            var ser = Never.Serialization.EasyJsonSerializer.Serialize(vs);
            Console.WriteLine(ser);
        }

        [Xunit.Fact]
        public void Test2()
        {
            IEnumerable<int[]>[] vs = null;
            var ser = Never.Serialization.EasyJsonSerializer.Serialize(vs);
            Console.WriteLine(ser);
        }

        [Xunit.Fact]
        public void Test3()
        {
            IEnumerable<int[][][]> vs = null;
            var ser = Never.Serialization.EasyJsonSerializer.Serialize(vs);
            Console.WriteLine(ser);
        }

        [Xunit.Fact]
        public void Test4()
        {
            IEnumerable<int[][]>[] vs = null;
            var ser = Never.Serialization.EasyJsonSerializer.Serialize(vs);
            Console.WriteLine(ser);
        }
    }
}