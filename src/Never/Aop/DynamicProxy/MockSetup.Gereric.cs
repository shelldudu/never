using System;
using System.Reflection;

namespace Never.Aop.DynamicProxy
{
    internal class MockSetup<T, TResult> : MockSetup, IMockSetup<T, TResult>, IMockSetup<T>, IMockSetup
    {
        /// <summary>
        /// 带返回值的执行方法
        /// </summary>
        public Func<T, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, TResult> FunctionCallbackMethod = null;

        /// <summary>
        /// 没有返回值的执行方法
        /// </summary>
        public Action<T, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> ActionCallbackMethod = null;

        /// <summary>
        /// 异常的执行方法
        /// </summary>
        public Action<T> ExceptionCallbackMethod = null;

        #region IMockSetup

        public void Throw<TException>(TException ex) where TException : Exception
        {
            this.MethodIndex = -1;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.MethodToCallType = MethodToCall.Exception;
            this.ExceptionCallbackMethod = (t) =>
            {
                throw new Exception(ObjectExtension.GetInnerException(ex).Message, ex);
            };
        }

        public void CallBase()
        {
            this.MethodIndex = 0;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return default(TResult); };
        }

        #endregion IMockSetup

        #region IMockSetup<T,TResult>

        public void Return(TResult result)
        {
            this.Return((t) => { return result; });
            this.MethodIndex = 10;
            this.CallbackMethodParameters = new ParameterInfo[0];
            this.ExceptionCallbackMethod = null;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return result; };
        }

        public void Return(Func<T, TResult> returnFunction)
        {
            this.MethodIndex = 10;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t); };
        }

        public void Return<T1>(Func<T, T1, TResult> returnFunction)
        {
            this.MethodIndex = 11;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t, (T1)t1); };
        }

        public void Return<T1, T2>(Func<T, T1, T2, TResult> returnFunction)
        {
            this.MethodIndex = 12;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t, (T1)t1, (T2)t2); };
        }

        public void Return<T1, T2, T3>(Func<T, T1, T2, T3, TResult> returnFunction)
        {
            this.MethodIndex = 13;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t, (T1)t1, (T2)t2, (T3)t3); };
        }

        public void Return<T1, T2, T3, T4>(Func<T, T1, T2, T3, T4, TResult> returnFunction)
        {
            this.MethodIndex = 14;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4); };
        }

        public void Return<T1, T2, T3, T4, T5>(Func<T, T1, T2, T3, T4, T5, TResult> returnFunction)
        {
            this.MethodIndex = 15;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5); };
        }

        public void Return<T1, T2, T3, T4, T5, T6>(Func<T, T1, T2, T3, T4, T5, T6, TResult> returnFunction)
        {
            this.MethodIndex = 16;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6); };
        }

        public void Return<T1, T2, T3, T4, T5, T6, T7>(Func<T, T1, T2, T3, T4, T5, T6, T7, TResult> returnFunction)
        {
            this.MethodIndex = 17;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7); };
        }

        public void Return<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, TResult> returnFunction)
        {
            this.MethodIndex = 18;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8); };
        }

        public void Return<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> returnFunction)
        {
            this.MethodIndex = 19;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8, (T9)t9); };
        }

        public void Return<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> returnFunction)
        {
            this.MethodIndex = 20;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8, (T9)t9, (T10)t10); };
        }

        public void Return<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> returnFunction)
        {
            this.MethodIndex = 21;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8, (T9)t9, (T10)t10, (T11)t11); };
        }

        public void Return<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> returnFunction)
        {
            this.MethodIndex = 22;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8, (T9)t9, (T10)t10, (T11)t11, (T12)t12); };
        }

        public void Return<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> returnFunction)
        {
            this.MethodIndex = 23;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8, (T9)t9, (T10)t10, (T11)t11, (T12)t12, (T13)t13); };
        }

        public void Return<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> returnFunction)
        {
            this.MethodIndex = 24;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8, (T9)t9, (T10)t10, (T11)t11, (T12)t12, (T13)t13, (T14)t14); };
        }

        public void Return<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> returnFunction)
        {
            this.MethodIndex = 25;
            this.MethodToCallType = MethodToCall.Return;
            this.CallbackMethodParameters = returnFunction.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.ActionCallbackMethod = null;
            this.FunctionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { return returnFunction.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8, (T9)t9, (T10)t10, (T11)t11, (T12)t12, (T13)t13, (T14)t14, (T15)t15); };
        }

        #endregion IMockSetup<T,TResult>

        #region IMockSetup<T>

        public void Void(Action<T> voidActiontion)
        {
            this.MethodIndex = 30;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t); };
        }

        public void Void<T1>(Action<T, T1> voidActiontion)
        {
            this.MethodIndex = 31;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t, (T1)t1); };
        }

        public void Void<T1, T2>(Action<T, T1, T2> voidActiontion)
        {
            this.MethodIndex = 32;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t, (T1)t1, (T2)t2); };
        }

        public void Void<T1, T2, T3>(Action<T, T1, T2, T3> voidActiontion)
        {
            this.MethodIndex = 33;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t, (T1)t1, (T2)t2, (T3)t3); };
        }

        public void Void<T1, T2, T3, T4>(Action<T, T1, T2, T3, T4> voidActiontion)
        {
            this.MethodIndex = 34;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4); };
        }

        public void Void<T1, T2, T3, T4, T5>(Action<T, T1, T2, T3, T4, T5> voidActiontion)
        {
            this.MethodIndex = 35;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5); };
        }

        public void Void<T1, T2, T3, T4, T5, T6>(Action<T, T1, T2, T3, T4, T5, T6> voidActiontion)
        {
            this.MethodIndex = 36;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6); };
        }

        public void Void<T1, T2, T3, T4, T5, T6, T7>(Action<T, T1, T2, T3, T4, T5, T6, T7> voidActiontion)
        {
            this.MethodIndex = 37;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7); };
        }

        public void Void<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8> voidActiontion)
        {
            this.MethodIndex = 38;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8); };
        }

        public void Void<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> voidActiontion)
        {
            this.MethodIndex = 39;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8, (T9)t9); };
        }

        public void Void<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> voidActiontion)
        {
            this.MethodIndex = 40;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8, (T9)t9, (T10)t10); };
        }

        public void Void<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> voidActiontion)
        {
            this.MethodIndex = 41;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8, (T9)t9, (T10)t10, (T11)t11); };
        }

        public void Void<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> voidActiontion)
        {
            this.MethodIndex = 42;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8, (T9)t9, (T10)t10, (T11)t11, (T12)t12); };
        }

        public void Void<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> voidActiontion)
        {
            this.MethodIndex = 43;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8, (T9)t9, (T10)t10, (T11)t11, (T12)t12, (T13)t13); };
        }

        public void Void<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> voidActiontion)
        {
            this.MethodIndex = 44;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8, (T9)t9, (T10)t10, (T11)t11, (T12)t12, (T13)t13, (T14)t14); };
        }

        public void Void<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> voidActiontion)
        {
            this.MethodIndex = 45;
            this.MethodToCallType = MethodToCall.Void;
            this.CallbackMethodParameters = voidActiontion.Method.GetParameters();
            this.ExceptionCallbackMethod = null;
            this.FunctionCallbackMethod = null;
            this.ActionCallbackMethod = (t, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15) => { voidActiontion.Invoke(t, (T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7, (T8)t8, (T9)t9, (T10)t10, (T11)t11, (T12)t12, (T13)t13, (T14)t14, (T15)t15); };
        }

        #endregion IMockSetup<T>
    }
}