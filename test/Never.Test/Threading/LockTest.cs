using Never.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test
{
    public class LockTest
    {
        [Xunit.Fact]
        public void TestLock()
        {
            IRigidLocker locker = new Never.Threading.SugerLocker();
            var threads = new System.Threading.Thread[3];
            threads[0] = new System.Threading.Thread(() =>
            {
                locker.EnterLock(true, () =>
                {
                    Console.WriteLine("first");
                    System.Threading.Thread.Sleep(3000);
                    Console.WriteLine("firstdone");
                });
            });

            threads[0].Start();

            threads[1] = new System.Threading.Thread(() =>
            {
                locker.EnterLock(false, () =>
                {
                    Console.WriteLine("two");
                });
            });

            threads[1].Start();

            threads[2] = new System.Threading.Thread(() =>
            {
                locker.EnterLock(true, () =>
                {
                    Console.WriteLine("three");
                });
            });

            threads[2].Start();
        }

        [Xunit.Fact]
        public void TestLock2()
        {
            IRigidLocker locker = new Never.Threading.SugerLocker();
            var threads = new System.Threading.Thread[2];
            threads[0] = new System.Threading.Thread(() =>
            {
                locker.EnterLock(false, () =>
                {
                    Console.WriteLine("first");
                    System.Threading.Thread.Sleep(2000);
                    Console.WriteLine("firstdone");
                    return 1;
                });
            });

            threads[0].Start();

            threads[1] = new System.Threading.Thread(() =>
            {
                locker.EnterLock(false, () =>
                {
                    Console.WriteLine("two");
                    return 1;
                });
            });

            threads[1].Start();
        }

        [Xunit.Fact]
        public void TestMonitorLock()
        {
            IRigidLocker locker = new Never.Threading.MonitorLocker();
            var threads = new System.Threading.Thread[2];
            threads[0] = new System.Threading.Thread(() =>
            {
                locker.EnterLock(false, () =>
                {
                    Console.WriteLine("first");
                    System.Threading.Thread.Sleep(2000);
                    Console.WriteLine("firstdone");
                });
            });

            threads[0].Start();

            threads[1] = new System.Threading.Thread(() =>
            {
                locker.EnterLock(false, () =>
                {
                    Console.WriteLine("two");
                });
            });

            threads[1].Start();
        }

        [Xunit.Fact]
        public void TestMonitorLock2()
        {
            IRigidLocker locker = new Never.Threading.MonitorLocker();
            var threads = new System.Threading.Thread[2];
            threads[0] = new System.Threading.Thread(() =>
            {
                locker.EnterLock(false, () =>
                {
                    Console.WriteLine("first");
                    System.Threading.Thread.Sleep(2000);
                    Console.WriteLine("firstdone");
                    return 1;
                });
            });

            threads[0].Start();

            threads[1] = new System.Threading.Thread(() =>
            {
                locker.EnterLock(false, () =>
                {
                    Console.WriteLine("two");
                    return 1;
                });
            });

            threads[1].Start();
        }

        [Xunit.Fact]
        public void TestReaderWriterLock()
        {
            IWaitableLocker locker = new Never.Threading.ReaderWriterLocker();
            var threads = new System.Threading.Thread[2];
            threads[0] = new System.Threading.Thread(() =>
            {
                locker.TryEnterLock(false, TimeSpan.FromSeconds(3), () =>
                 {
                     Console.WriteLine("first");
                     System.Threading.Thread.Sleep(2000);
                     Console.WriteLine("firstdone");
                 });
            });

            threads[0].Start();

            threads[1] = new System.Threading.Thread(() =>
            {
                locker.TryEnterLock(false, TimeSpan.FromSeconds(3), () =>
                 {
                     Console.WriteLine("two");
                 });
            });

            threads[1].Start();
        }
    }
}