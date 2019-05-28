using System.Collections.Generic;

namespace Never.Aop.DynamicProxy
{
    /// <summary>
    /// 代理对象信息保存
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class MockSetupInfoStore
    {
        #region field

        /// <summary>
        /// 当前增量
        /// </summary>
        private static int factor = 0;

        /// <summary>
        /// 某个方法调用的队列
        /// </summary>
        private static SortedDictionary<int, MockSetup> invocationQueue = new SortedDictionary<int, MockSetup>();

        #endregion field

        #region query

        /// <summary>
        /// 查询调用信息
        /// </summary>
        /// <param name="index">构建代理的索引</param>
        /// <returns></returns>
        public static MockSetup Query(int index)
        {
            return invocationQueue[index];
        }

        /// <summary>
        /// 压进队列里
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns></returns>
        public static int Enqueue(MockSetup invocation)
        {
            var fac = System.Threading.Interlocked.Increment(ref factor);
            invocationQueue[fac] = invocation;
            return fac;
        }

        #endregion query

        #region call

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult ReturnDefault<TResult>()
        {
            return default(TResult);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        public static void Call_ExceptionWithNoResult<T>(T @object, int index)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ExceptionCallbackMethod.Invoke(@object);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        public static TResult Call_ExceptionWithResult<T, TResult>(T @object, int index)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            call.ExceptionCallbackMethod.Invoke(@object);
            return default(TResult);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static TResult Call_10<T, TResult>(T @object, int index)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static TResult Call_11<T, T1, TResult>(T @object, int index, T1 t1)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, t1, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static TResult Call_12<T, T1, T2, TResult>(T @object, int index, T1 t1, T2 t2)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, t1, t2, null, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <returns></returns>
        public static TResult Call_13<T, T1, T2, T3, TResult>(T @object, int index, T1 t1, T2 t2, T3 t3)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, t1, t2, t3, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <returns></returns>
        public static TResult Call_14<T, T1, T2, T3, T4, TResult>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, t1, t2, t3, t4, null, null, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <returns></returns>
        public static TResult Call_15<T, T1, T2, T3, T4, T5, TResult>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, null, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <returns></returns>
        public static TResult Call_16<T, T1, T2, T3, T4, T5, T6, TResult>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <returns></returns>
        public static TResult Call_17<T, T1, T2, T3, T4, T5, T6, T7, TResult>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        /// <returns></returns>
        public static TResult Call_18<T, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        /// <param name="t9"></param>
        /// <returns></returns>
        public static TResult Call_19<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, t9, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        /// <param name="t9"></param>
        /// <param name="t10"></param>
        /// <returns></returns>
        public static TResult Call_20<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        /// <param name="t9"></param>
        /// <param name="t10"></param>
        /// <param name="t11"></param>
        /// <returns></returns>
        public static TResult Call_21<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        /// <param name="t9"></param>
        /// <param name="t10"></param>
        /// <param name="t11"></param>
        /// <param name="t12"></param>
        /// <returns></returns>
        public static TResult Call_22<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        /// <param name="t9"></param>
        /// <param name="t10"></param>
        /// <param name="t11"></param>
        /// <param name="t12"></param>
        /// <param name="t13"></param>
        /// <returns></returns>
        public static TResult Call_23<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <typeparam name="T14"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        /// <param name="t9"></param>
        /// <param name="t10"></param>
        /// <param name="t11"></param>
        /// <param name="t12"></param>
        /// <param name="t13"></param>
        /// <param name="t14"></param>
        /// <returns></returns>
        public static TResult Call_24<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <typeparam name="T14"></typeparam>
        /// <typeparam name="T15"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        /// <param name="t9"></param>
        /// <param name="t10"></param>
        /// <param name="t11"></param>
        /// <param name="t12"></param>
        /// <param name="t13"></param>
        /// <param name="t14"></param>
        /// <param name="t15"></param>
        /// <returns></returns>
        public static TResult Call_25<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15)
        {
            var call = Query(index) as MockSetup<T, TResult>;
            if (call == null)
                return default(TResult);

            return call.FunctionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        public static void Call_30<T>(T @object, int index)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        public static void Call_31<T, T1>(T @object, int index, T1 t1)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, t1, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public static void Call_32<T, T1, T2>(T @object, int index, T1 t1, T2 t2)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, t1, t2, null, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        public static void Call_33<T, T1, T2, T3>(T @object, int index, T1 t1, T2 t2, T3 t3)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, t1, t2, t3, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        public static void Call_34<T, T1, T2, T3, T4>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, t1, t2, t3, t4, null, null, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        public static void Call_35<T, T1, T2, T3, T4, T5>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, null, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        public static void Call_36<T, T1, T2, T3, T4, T5, T6>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, null, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        public static void Call_37<T, T1, T2, T3, T4, T5, T6, T7>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, null, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        public static void Call_38<T, T1, T2, T3, T4, T5, T6, T7, T8>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, null, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        /// <param name="t9"></param>
        public static void Call_39<T, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, t9, null, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        /// <param name="t9"></param>
        /// <param name="t10"></param>
        public static void Call_40<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, null, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        /// <param name="t9"></param>
        /// <param name="t10"></param>
        /// <param name="t11"></param>
        public static void Call_41<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, null, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        /// <param name="t9"></param>
        /// <param name="t10"></param>
        /// <param name="t11"></param>
        /// <param name="t12"></param>
        public static void Call_42<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, null, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        /// <param name="t9"></param>
        /// <param name="t10"></param>
        /// <param name="t11"></param>
        /// <param name="t12"></param>
        /// <param name="t13"></param>
        public static void Call_43<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, null, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <typeparam name="T14"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        /// <param name="t9"></param>
        /// <param name="t10"></param>
        /// <param name="t11"></param>
        /// <param name="t12"></param>
        /// <param name="t13"></param>
        /// <param name="t14"></param>
        public static void Call_44<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <typeparam name="T10"></typeparam>
        /// <typeparam name="T11"></typeparam>
        /// <typeparam name="T12"></typeparam>
        /// <typeparam name="T13"></typeparam>
        /// <typeparam name="T14"></typeparam>
        /// <typeparam name="T15"></typeparam>
        /// <param name="object"></param>
        /// <param name="index"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <param name="t6"></param>
        /// <param name="t7"></param>
        /// <param name="t8"></param>
        /// <param name="t9"></param>
        /// <param name="t10"></param>
        /// <param name="t11"></param>
        /// <param name="t12"></param>
        /// <param name="t13"></param>
        /// <param name="t14"></param>
        /// <param name="t15"></param>
        public static void Call_45<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T @object, int index, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15)
        {
            var call = Query(index) as MockSetup<T, object>;
            if (call == null)
                return;

            call.ActionCallbackMethod.Invoke(@object, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
        }

        #endregion call
    }
}