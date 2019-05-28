using Never;
using Never.Aop;
using Never.Aop.DynamicProxy;
using Never.Utils;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.TestConsole
{
    public class MockTest : Program
    {
        public interface _ITest
        {
            void Write(Program p);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface ITest : _ITest
        {
            //int Id { get; set; }

            // string GetMyName();

            //Program Write(int abc, AEG? ed);

            //Program Write(KeyValuePair<int, int> abc, AEG ed, ref int? a);

            //Program Write(KeyValuePair<int, int> abc, AEG ed, ref Program a, out Program aet);

            //event IFunc<Program, int> ActionCallBack;
        }

        public class MyTest : ITest
        {
            public event Func<Program, int> ActionCallBack;

            public void Write(Program p)
            {
                //Console.WriteLine(typeof(T).Name);
                Console.WriteLine("MyTest.Write");
                if (this.ActionCallBack != null)
                    Console.WriteLine(this.ActionCallBack(p));
            }

            public virtual int Id { get; set; }

            //public virtual string GetMyName()
            //{
            //    return "A";
            //}

            //public string GetMyName(string abc)
            //{
            //    return "ABC";
            //}

            // public Guid UniqueId;
        }

        public class EMyTest : ITest
        {
            public ITest test = null;
            private readonly IList<Aop.IInterceptor> list = new List<Aop.IInterceptor>(1);

            public event Func<Program, int> ActionCallBack;

            public EMyTest()
            {
                //this.test = test;
                list.Add(new Aop.StandardInterceptor());
            }

            public virtual void Write(Program p)
            {
                if (this.ActionCallBack != null)
                    this.ActionCallBack(p);
            }

            //public int Id { get; set; }

            //public string GetMyName()
            //{
            //    return "A";
            //}
        }

        public enum AEG
        {
            A = 1,
            E = 2,
            G = 3
        }

        [Xunit.Fact]
        public void TestCraeteITestProxy()
        {
            //test.Id = 4;
            //Console.Write(test.Id);
            //var mock2 = new Mock<MyTest>();
            //mock2.Setup(o => o.UniqueId).Return(NewId.GenerateGuid());
            //mock2.Setup(o => o.Id);
            //mock2.Setup(o => o.Id).Return((x) => 666);
            //mock2.Setup(o => o.Write(new Program())).Throw(new Exception(""));
            //mock2.Setup(o => o.GetMyName()).Return("A");
            //mock2.Setup(o => o.GetMyName("etf")).Return(new Func<MyTest, string, string>((t, x) => { return "2"; }));

            //var test2 = mock2.CreateIllusive();
        }

        public class RefMyClass
        {
            public virtual void Write(out AEG mock)
            {
                mock = default(AEG);
            }
        }

        public class RefMyClass<T>
        {
            public virtual void Write(out T mock)
            {
                mock = default(T);
            }
        }

        public class IntRefMyclass : RefMyClass<int>
        {
            public override void Write(out int mock)
            {
                mock = 999;
            }
        }

        public class OutMyClass : RefMyClass
        {
            public override void Write(out AEG mock)
            {
                base.Write(out mock);
            }
        }

        [Xunit.Fact]
        public void TestCreateRefMyClassMockObject()
        {
            var mock = new Mock<RefMyClass>();
            AEG moca = AEG.E;
            mock.Setup(x => x.Write(out moca)).Void((x) => { Console.WriteLine(x.GetType()); });
            var test = mock.CreateIllusive();
            //test.Id = 55;
            //Console.WriteLine(test.Id);
            //test.ActionCallBack += Test_ActionCallBack;
            //test.Write(new Program());
            //test.ActionCallBack -= Test_ActionCallBack;

            //test.Write(new Program());

            // object moca = new MyMock();

            test.Write(out moca);

            var typeo = typeof(AEG);
        }

        [Xunit.Fact]
        public void TestCreateITestMockObject()
        {
            var mock = new Mock<ITest>();
            mock.Setup(x => x.Write(new Program())).CallBase();//.Void((p) => { Console.WriteLine("mock itest write"); });

            var test = mock.CreateIllusive();
            test.Write(new Program());
        }

        [Xunit.Fact]
        public void TestCreateMyTestMockObject()
        {
            var mock = new Mock<MyTest>();
            mock.Setup(x => x.Write(new Program())).CallBase();//.Void((p) => { Console.WriteLine("mock itest write"); });
            mock.Setup(x => x.Id).Return(66);

            var test = mock.CreateIllusive();
            test.Id = 55;
            Console.WriteLine(test.Id);
            test.ActionCallBack += Test_ActionCallBack;
            test.Write(new Program());
            test.ActionCallBack -= Test_ActionCallBack;

            test.Write(new Program());
        }

        private int Test_ActionCallBack(Program object1)
        {
            Console.WriteLine("Test_ActionCallBack");
            return 1;
        }

        private int Emy_ActionCallBack(Program object1)
        {
            throw new NotImplementedException();
        }

        [Xunit.Fact]
        public void TestIoCInstance()
        {
            //var mytest = this.ServiceLocator.Resolve<MyTest>();
            //Console.WriteLine(mytest.GetMyName("agd"));
            var mock = new Mock<MyField>();
            mock.Setup(x => x.MyId).Return(6696);

            var test = mock.CreateIllusive();
            Console.WriteLine(test.MyId);
        }

        public class MyField
        {
            public virtual int MyId
            {
                get
                {
                    return 2;
                }
            }
        }

        public class MMMMyTest : MyTest
        {
            private readonly List<Aop.IInterceptor> loca = null;

            public MMMMyTest(Aop.IInterceptor tor)
            {
                this.loca = new List<Aop.IInterceptor>(2);
                this.loca.Add(tor);
            }
        }

        //[Xunit.Fact]
        //public void TestMethodTick()
        //{
        //    var itest = new Mock<ITest>().CreateProxy(new MyTest(), new Aop.InterceptCompileSetting() { BoxArgument = false, StoreArgument = true, NoInvocation = false }, new Aop.IInterceptor[1] { new Aop.StandardInterceptor() });
        //    var mock = new Castle.DynamicProxy.ProxyGenerator();
        //    var it = mock.CreateInterfaceProxyWithTarget<ITest>(new MyTest(), new Castle.DynamicProxy.IInterceptor[1] { new MyIInterceptor() });
        //    var auto = new EMy(new MyTest());

        //    try
        //    {
        //        int time = 10000000;

        //        using (var t = new Utils.MethodTickCount("auto"))
        //        {
        //            for (var i = 0; i < time; i++)
        //            {
        //                auto.Write(new Program());
        //            }
        //        }

        //        Utils.MethodTickCount.ConsoleWriteLine("auto");

        //        using (var t = new Utils.MethodTickCount("Castle"))
        //        {
        //            for (var i = 0; i < time; i++)
        //            {
        //                it.Write(new Program());
        //            }
        //        }

        //        Utils.MethodTickCount.ConsoleWriteLine("Castle");

        //        using (var t = new Utils.MethodTickCount("lite"))
        //        {
        //            for (var i = 0; i < time; i++)
        //            {
        //                itest.Write(new Program());
        //            }
        //        }

        //        Utils.MethodTickCount.ConsoleWriteLine("lite");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //    finally
        //    {
        //    }
        //}

        //[Xunit.Fact]
        //public void TestCraeteMyTestProxy()
        //{
        //    //Aop.DynamicProxy.MockBuilder.BoxArgument = true;
        //    var itest = new Mock<MyTest>().CreateProxy(new MyTest(), new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
        //    var abc = 1;// new int[1] { 1 };
        //    var ed = AEG.A;
        //    int? a = 3;

        //    var aet = new Program();
        //    itest.Write(aet);

        //    //t = ((ITest)itest).Write(2, AEG.E, ref p, out ate);
        //}

        //[Xunit.Fact]
        //public void TestCastleClass()
        //{
        //    var mock = new Castle.DynamicProxy.ProxyGenerator();
        //    var it = mock.CreateInterfaceProxyWithTarget<ITest>(new MyTest(), new Castle.DynamicProxy.IInterceptor[1] { new MyIInterceptor() });
        //    it.Write(null);
        //}

        public class MyIInterceptor : Castle.DynamicProxy.StandardInterceptor
        {
            protected override void PerformProceed(Castle.DynamicProxy.IInvocation invocation)
            {
                base.PerformProceed(invocation);
            }

            protected override void PreProceed(Castle.DynamicProxy.IInvocation invocation)
            {
                base.PreProceed(invocation);
            }

            protected override void PostProceed(Castle.DynamicProxy.IInvocation invocation)
            {
                base.PostProceed(invocation);
            }
        }

        public void Dama()
        {
            var text = "18578609527";
        }
    }
}