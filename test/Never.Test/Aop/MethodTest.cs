using Never.Aop.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test
{
    public enum AEG
    {
        A = 1,
        E = 2,
        G = 3
    }

    public interface IMock
    {
        void Write(int abc);

        void Write(AEG ed);

        void Write(int abc, AEG ed);

        void Write(Program p);

        void Write(Program p, int abc);

        void Write(Program p, AEG ed);

        void Write(int? abc);

        void Write(AEG? ed);

        void Write(int? abc, AEG? ed);

        void WriteRef(int abc, ref AEG ed);

        void WriteRef(int abc, ref Program p);

        void WriteRef(int abc, ref int ed);

        void WriteRef(int abc, ref Action ed);

        void WriteOut(int abc, out AEG ed);

        void WriteOut(int abc, out AEG? ed);

        void WriteOut(int abc, out Program p);

        void WriteOut(int abc, out int ed);

        void WriteOut(int abc, out int? ed);

        void WriteOut(int abc, out Action ed);

        event Action<Program> CallBack;
    }

    public class MyMock : IMock
    {
        public event Action<Program> CallBack;

        public void Write(Test.AEG ed)
        {
            var list = new List<object>();
            list.Add(ed.ToString());
            list.Add(ed);
        }

        public void Write(Program p)
        {
            if (this.CallBack != null)
                this.CallBack(p);

            var list = new List<object>();
            if (p == null)
                return;

            list.Add(p.ToString());
            list.Add(p);
        }

        public void Write(int? abc)
        {
            var list = new List<object>();
            if (!abc.HasValue)
                return;

            list.Add(abc.ToString());
            list.Add(abc);
        }

        public void Write(Test.AEG? ed)
        {
            var list = new List<object>();
            if (!ed.HasValue)
                return;

            list.Add(ed.ToString());
            list.Add(ed);
        }

        public void Write(int abc)
        {
            var list = new List<object>();
            list.Add(abc.ToString());
            list.Add(abc);
        }

        public void Write(int? abc, Test.AEG? ed)
        {
            var list = new List<object>();
            list.Add(abc.ToString());
            list.Add(abc);

            if (!ed.HasValue)
                return;

            list.Add(ed.ToString());
            list.Add(ed);
        }

        public void Write(Program p, Test.AEG ed)
        {
            var list = new List<object>();
            list.Add(p.ToString());
            list.Add(p);
            list.Add(ed.ToString());
            list.Add(ed);
        }

        public void Write(Program p, int abc)
        {
            var list = new List<object>();
            list.Add(abc.ToString());
            list.Add(abc);
            if (p == null)
                return;

            list.Add(p.ToString());
            list.Add(p);
        }

        public void Write(int abc, Test.AEG ed)
        {
            var list = new List<object>();
            list.Add(abc.ToString());
            list.Add(abc);
            list.Add(ed.ToString());
            list.Add(ed);
        }

        public void WriteOut(int abc, out Program p)
        {
            p = new Program();
            var list = new List<object>();
            list.Add(abc.ToString());
            list.Add(abc);
            list.Add(p.ToString());
            list.Add(p);
        }

        public void WriteOut(int abc, out Test.AEG? ed)
        {
            ed = AEG.A;
            var list = new List<object>();
            list.Add(abc.ToString());
            list.Add(abc);
            list.Add(ed.ToString());
            list.Add(ed);
        }

        public void WriteOut(int abc, out int ed)
        {
            ed = 9999;
            var list = new List<object>();
            list.Add(abc.ToString());
            list.Add(abc);
            list.Add(ed.ToString());
            list.Add(ed);
        }

        public void WriteOut(int abc, out int? ed)
        {
            ed = 9999;
            var list = new List<object>();
            list.Add(abc.ToString());
            list.Add(abc);
            list.Add(ed.ToString());
            list.Add(ed);
        }

        public void WriteOut(int abc, out Action ed)
        {
            ed = null;
            var list = new List<object>();
            list.Add(abc.ToString());
            list.Add(abc);
            if (ed == null)
                return;

            list.Add(ed.ToString());
            list.Add(ed);
        }

        public void WriteOut(int abc, out Test.AEG ed)
        {
            ed = AEG.A | AEG.G;
            var list = new List<object>();
            list.Add(abc.ToString());
            list.Add(abc);
            list.Add(ed.ToString());
            list.Add(ed);
        }

        public void WriteRef(int abc, ref int ed)
        {
            var list = new List<object>();
            list.Add(abc.ToString());
            list.Add(abc);
            list.Add(ed.ToString());
            list.Add(ed);
        }

        public void WriteRef(int abc, ref Action ed)
        {
            var list = new List<object>();
            list.Add(abc.ToString());
            list.Add(abc);
            if (ed == null)
                return;
            list.Add(ed.ToString());
            list.Add(ed);
        }

        public void WriteRef(int abc, ref Program p)
        {
            var list = new List<object>();
            list.Add(abc.ToString());
            list.Add(abc);
            if (p == null)
            {
                p = new Program();
                return;
            }
            list.Add(p.ToString());
            list.Add(p);
        }

        public void WriteRef(int abc, ref Test.AEG ed)
        {
            var list = new List<object>();
            list.Add(abc.ToString());
            list.Add(abc);
            list.Add(ed.ToString());
            list.Add(ed);
        }
    }

    public class EmMyMock : MyMock
    {
        private readonly MyMock mock = null;

        public EmMyMock(MyMock mock)
        {
            this.mock = mock;
        }

        public new event Action<Program> CallBack
        {
            add
            {
                this.mock.CallBack += value;
            }

            remove
            {
                this.mock.CallBack -= value;
            }
        }
    }

    public sealed class MyMockTwo : IMock
    {
        private readonly IMock mock = null;

        #region field

        public event Action<Program> CallBack
        {
            add
            {
                this.mock.CallBack += value;
            }

            remove
            {
                this.mock.CallBack -= value;
            }
        }

        public MyMockTwo(IMock mock)
        {
            this.mock = mock;
        }

        public void Write(Program p)
        {
            mock.Write(p);
        }

        public void Write(int? abc)
        {
            mock.Write(abc);
        }

        public void Write(AEG? ed)
        {
            mock.Write(ed);
        }

        public void Write(AEG ed)
        {
            mock.Write(ed);
        }

        public void Write(int abc)
        {
            mock.Write(abc);
        }

        public void Write(int? abc, AEG? ed)
        {
            mock.Write(abc, ed);
        }

        public void Write(Program p, AEG ed)
        {
            mock.Write(p, ed);
        }

        public void Write(Program p, int abc)
        {
            mock.Write(p, abc);
        }

        public void Write(int abc, AEG ed)
        {
            mock.Write(abc, ed);
        }

        public void WriteOut(int abc, out AEG? ed)
        {
            mock.WriteOut(abc, out ed);
        }

        public void WriteOut(int abc, out Program p)
        {
            mock.WriteOut(abc, out p);
        }

        public void WriteOut(int abc, out int ed)
        {
            mock.WriteOut(abc, out ed);
        }

        public void WriteOut(int abc, out Action ed)
        {
            mock.WriteOut(abc, out ed);
        }

        public void WriteOut(int abc, out int? ed)
        {
            mock.WriteOut(abc, out ed);
        }

        public void WriteOut(int abc, out AEG ed)
        {
            mock.WriteOut(abc, out ed);
        }

        public void WriteRef(int abc, ref int ed)
        {
            mock.WriteRef(abc, ref ed);
        }

        public void WriteRef(int abc, ref Action ed)
        {
            mock.WriteRef(abc, ref ed);
        }

        public void WriteRef(int abc, ref Program p)
        {
            mock.WriteRef(abc, ref p);
        }

        public void WriteRef(int abc, ref AEG ed)
        {
            mock.WriteRef(abc, ref ed);
        }

        #endregion field
    }

    public class MethodTest
    {
        [Xunit.Fact]
        public void TestWriteInt()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            test.Write(2);
        }

        [Xunit.Fact]
        public void TestWriteInt2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            test.Write(2);
        }

        [Xunit.Fact]
        public void TestWriteEnum()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            test.Write(AEG.A);
        }

        [Xunit.Fact]
        public void TestWriteEnum2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            test.Write(AEG.A);
        }

        [Xunit.Fact]
        public void TestWriteIntEnum()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            test.Write(2, AEG.A);
        }

        [Xunit.Fact]
        public void TTestWriteIntEnum2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            test.Write(2, AEG.A);
        }

        [Xunit.Fact]
        public void TestWriteObject()
        {
            //fail

            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            test.Write(new Program());
        }

        [Xunit.Fact]
        public void TestWriteObject2()
        {
            //fail

            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            test.Write(new Program());
        }

        [Xunit.Fact]
        public void TestWriteObjectInt()
        {
            //fail

            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            test.Write(new Program(), 4);
        }

        [Xunit.Fact]
        public void TestWriteObjectInt2()
        {
            //fail

            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            test.Write(new Program(), 4);
        }

        [Xunit.Fact]
        public void TestWriteObjectEnum()
        {
            //fail

            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            test.Write(new Program(), AEG.A);
        }

        [Xunit.Fact]
        public void TTestWriteObjectEnum2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            test.Write(new Program(), AEG.A);
        }

        [Xunit.Fact]
        public void TestWriteNullInt()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int? a = 2;
            test.Write(a);
        }

        [Xunit.Fact]
        public void TestWriteNullInt2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int? a = 2;
            test.Write(a);
        }

        [Xunit.Fact]
        public void TestWriteNullInt_N1()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int? a = null;
            test.Write(a);
        }

        [Xunit.Fact]
        public void TestWriteNullInt_N2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int? a = null;
            test.Write(a);
        }

        [Xunit.Fact]
        public void TestWriteNullEnum()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            AEG? a = AEG.E;
            test.Write(a);
        }

        [Xunit.Fact]
        public void TestWriteNullEnum2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            AEG? a = AEG.E;
            test.Write(a);
        }

        [Xunit.Fact]
        public void TestWriteNullEnum_N1()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            AEG? a = null;
            test.Write(a);
        }

        [Xunit.Fact]
        public void TestWriteNullEnum_N2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            AEG? a = null;
            test.Write(a);
        }

        [Xunit.Fact]
        public void TestWriteNullIntNullEnum()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int? a = 2;
            AEG? ed = AEG.E;
            test.Write(a, ed);
        }

        [Xunit.Fact]
        public void TestWriteNullIntNullEnum2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int? a = 2;
            AEG? ed = AEG.E;
            test.Write(a, ed);
        }

        [Xunit.Fact]
        public void TestWriteNullIntNullEnum_N1()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int? a = null;
            AEG? ed = null;
            test.Write(a, ed);
        }

        [Xunit.Fact]
        public void TestWriteNullIntNullEnum_N2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int? a = null;
            AEG? ed = null;
            test.Write(a, ed);
        }

        [Xunit.Fact]
        public void TestWriteIntRefEnum()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            AEG ed = AEG.E;
            test.WriteRef(a, ref ed);
        }

        [Xunit.Fact]
        public void TestWriteIntRefEnum2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            AEG ed = AEG.E;
            test.WriteRef(a, ref ed);
        }

        [Xunit.Fact]
        public void TestWriteIntRefObject()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            var ed = new Program();
            test.WriteRef(a, ref ed);
        }

        [Xunit.Fact]
        public void TestWriteIntRefObject2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            var ed = new Program();
            test.WriteRef(a, ref ed);
        }

        [Xunit.Fact]
        public void TestWriteIntRefNullObject()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            Program ed = null;
            test.WriteRef(a, ref ed);
        }

        [Xunit.Fact]
        public void TestWriteIntRefNullObject2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            Program ed = null;
            test.WriteRef(a, ref ed);
        }

        [Xunit.Fact]
        public void TestWriteIntRefInt()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            var ed = 6;
            test.WriteRef(a, ref ed);
        }

        [Xunit.Fact]
        public void TestWriteIntRefInt2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            var ed = 6;
            test.WriteRef(a, ref ed);
        }

        [Xunit.Fact]
        public void TestWriteIntRefDelegate()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            Action ed = null;
            test.WriteRef(a, ref ed);
        }

        [Xunit.Fact]
        public void TestWriteIntRefDelegate2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            Action ed = null;
            test.WriteRef(a, ref ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutEnum()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            AEG ed = AEG.E;
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutEnum2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            AEG ed = AEG.E;
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutNullEnum()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            AEG? ed = AEG.E;
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutNullEnum2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            AEG? ed = AEG.E;
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutNullEnum_N1()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            AEG? ed = null;
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutNullEnum_N2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            AEG? ed = null;
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutObject()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            Program ed = new Program();
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutObject2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            Program ed = new Program();
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutObject_N1()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            Program ed = null;
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutObject_N2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            Program ed = null;
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutInt()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            var ed = 0;
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutInt2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            var ed = 0;
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutNullInt()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            int? ed = 4;
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutNullInt2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            int? ed = 4;
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutNullInt_N1()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            int? ed = null;
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutNullInt_N2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            int? ed = null;
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutDelegate()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = true, StoreArgument = true }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            Action ed = null;
            test.WriteOut(a, out ed);
        }

        [Xunit.Fact]
        public void TestWriteIntOutDelegate2()
        {
            var test = new Mock<IMock>().CreateProxy(new MyMock(), new Aop.InterceptCompileSetting() { BoxArgument = false }, new Aop.IInterceptor[1] { new Aop.IInterceptors.OutputAllArgsInterceptor() });
            int a = 2;
            Action ed = null;
            test.WriteOut(a, out ed);
        }
    }
}