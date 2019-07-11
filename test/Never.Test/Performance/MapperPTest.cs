using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test
{
    public class MapperPTest
    {
        [Xunit.Fact]
        public void TestMapper()
        {
            int times = 10000000;
            Console.WriteLine("begin........");

            var from = new From()
            {
                Guid = Guid.NewGuid(),
                Id = 666,
                Name = 2233
            };

            var emitmapper = EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<From, To>();
            emitmapper.Map(from);

            AutoMapper.Mapper.Initialize(x =>
            {
            });
            var automapper = AutoMapper.Mapper.Map<From, To>(from);

            var easymapper = Never.Mappers.EasyMapper.Map<From, To>(from);

            using (var t = new Utils.MethodTickCount((x) => { Console.WriteLine("emitmapper" + x); }))
            {
                for (var i = 0; i < times; i++)
                {
                    emitmapper.Map(from);
                }
            }

            using (var t = new Utils.MethodTickCount((x) => { Console.WriteLine("automapper" + x); }))
            {
                AutoMapper.Mapper.Map<From, To>(from);
            }

            using (var t = new Utils.MethodTickCount((x) => { Console.WriteLine("easymapper" + x); }))
            {
                Never.Mappers.EasyMapper.Map<From, To>(from);
            }

            Console.WriteLine("..........end");
            Console.ReadLine();
        }

        public class From
        {
            public long Id { get; set; }
            public int Name { get; set; }
            public Guid Guid { get; set; }
        }

        public class To
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Guid { get; set; }
        }
    }
}