using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.TestConsole.Serialization
{
    public class FileTextTest
    {
        public void Run()
        {
            var text = System.IO.File.ReadAllLines(@"c:\json.txt");
            var sb = new StringBuilder(300);
            var splits = text.Split(15);
            for (var i = 1; i < splits.Count(); i++)
            {
                var split = splits.ElementAt(i);
                sb.AppendLine("/// <summary>");
                sb.AppendFormat("/// {0}，{1}", split.ElementAt(7), split.ElementAt(9));
                sb.AppendLine();
                sb.AppendLine("/// </summary>");
                sb.AppendFormat("public {0} {1}", split.ElementAt(3), split.ElementAt(1));
                sb.AppendLine(" { get ; set ;}");
            }
        }


        public static void TestMyGuid1()
        {
            var text = @"{orderids:['8fb5716c-e605-42cd-b44b-a8cb0100e4b7','95d4d27a-bc64-45d3-b537-a8cb0100e920']}";
            var myguid = Never.Serialization.EasyJsonSerializer.Deserialize<MyGuid>(text);
        }

        public static void TestMyGuid2()
        {
            var text = @"{  orderids:[  '8fb5716c-e605-42cd-b44b-a8cb0100e4b7'   ,  '95d4d27a-bc64-45d3-b537-a8cb0100e920'       ]
                            }";
            var myguid = Never.Serialization.EasyJsonSerializer.Deserialize<MyGuid>(text);
        }

        public static void TestMyGuid3()
        {
            var text = @"
                        {
                        orderids:

                        ['8fb5716c-e605-42cd-b44b-a8cb0100e4b7',

                        '95d4d27a-bc64-45d3-b537-a8cb0100e920'

                        ]
                        }";
            var myguid = Never.Serialization.EasyJsonSerializer.Deserialize<MyGuid>(text);
        }

        public class MyGuid
        {
            public Guid[] OrderIds { get; set; }
        }
    }
}
