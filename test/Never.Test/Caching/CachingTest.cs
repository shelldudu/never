using Never.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test
{
    public class CachingTest
    {
        [Xunit.Fact]
        public void TestThreadContextCache()
        {
            var cache = new ThreadContextCache();
            cache.Set("A", "120");
            Task.Run(() =>
            {
                Console.WriteLine(cache.Get<string>("A"));
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                cache = new ThreadContextCache();
                cache.Set("B", "12");
                cache.Dispose();
                GC.Collect();
            });

            Console.WriteLine(cache.Get<string>("A"));
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(6));
            Console.WriteLine(cache.Get<string>("B"));
            Console.ReadLine();
        }
    }
}