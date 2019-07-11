using Never.Serialization;
using Never.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test
{
    public class SerializationPTest
    {
        [Xunit.Fact]
        public void TestTimeSpan()
        {
            var timeSpan = TimeSpan.FromHours(2);
            var text = " " + EasyJsonSerializer.Serialize(timeSpan);
            EasyJsonSerializer.Deserialize<TimeSpan>(text, new JsonDeserializeSetting() { DateTimeFormat = DateTimeFormat.ChineseStyle });
            Newtonsoft.Json.JsonConvert.DeserializeObject<TimeSpan>(text);
            Jil.JSON.Deserialize<TimeSpan>(text);

            int times = 10000000;
            Console.WriteLine("begin........");

            using (var t = new Utils.MethodTickCount((x) => { Console.WriteLine(x); }))
            {
                for (var i = 0; i < times; i++)
                {
                    Newtonsoft.Json.JsonConvert.DeserializeObject<TimeSpan>(text);
                }
            }

            using (var t = new Utils.MethodTickCount((x) => { Console.WriteLine(x); }))
            {
                var setting = new JsonDeserializeSetting() { DateTimeFormat = DateTimeFormat.ChineseStyle };
                for (var i = 0; i < times; i++)
                {
                    EasyJsonSerializer.Deserialize<TimeSpan>(text, setting);
                }
            }

            using (var t = new Utils.MethodTickCount((x) => { Console.WriteLine(x); }))
            {
                for (var i = 0; i < times; i++)
                {
                    Jil.JSON.Deserialize<TimeSpan>(text);
                }
            }

            Console.WriteLine("..........end");
            Console.ReadLine();
        }
    }
}