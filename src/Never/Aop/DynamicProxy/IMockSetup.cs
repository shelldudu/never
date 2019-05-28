using System;

namespace Never.Aop.DynamicProxy
{
    /// <summary>
    /// 模拟行为
    /// </summary>
    public interface IMockSetup
    {
        /// <summary>
        /// 调用基类的方法
        /// </summary>
        void CallBase();

        /// <summary>
        /// 抛出异常
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="ex"></param>
        void Throw<TException>(TException ex) where TException : Exception;
    }

    /// <summary>
    /// 模拟行为
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMockSetup<T> : IMockSetup
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        /// <param name="voidActiontion"></param>
        void Void(Action<T> voidActiontion);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="voidActiontion"></param>
        void Void<T1>(Action<T, T1> voidActiontion);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="voidActiontion"></param>
        void Void<T1, T2>(Action<T, T1, T2> voidActiontion);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="voidActiontion"></param>
        void Void<T1, T2, T3>(Action<T, T1, T2, T3> voidActiontion);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="voidActiontion"></param>
        void Void<T1, T2, T3, T4>(Action<T, T1, T2, T3, T4> voidActiontion);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="voidActiontion"></param>
        void Void<T1, T2, T3, T4, T5>(Action<T, T1, T2, T3, T4, T5> voidActiontion);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <param name="voidActiontion"></param>
        void Void<T1, T2, T3, T4, T5, T6>(Action<T, T1, T2, T3, T4, T5, T6> voidActiontion);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <param name="voidActiontion"></param>
        void Void<T1, T2, T3, T4, T5, T6, T7>(Action<T, T1, T2, T3, T4, T5, T6, T7> voidActiontion);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <param name="voidActiontion"></param>
        void Void<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8> voidActiontion);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <param name="voidActiontion"></param>
        void Void<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> voidActiontion);

        /// <summary>
        /// 返回结果
        /// </summary>
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
        /// <param name="voidActiontion"></param>
        void Void<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> voidActiontion);

        /// <summary>
        /// 返回结果
        /// </summary>
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
        /// <param name="voidActiontion"></param>
        void Void<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> voidActiontion);

        /// <summary>
        /// 返回结果
        /// </summary>
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
        /// <param name="voidActiontion"></param>
        void Void<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> voidActiontion);

        /// <summary>
        /// 返回结果
        /// </summary>
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
        /// <param name="voidActiontion"></param>
        void Void<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> voidActiontion);

        /// <summary>
        /// 返回结果
        /// </summary>
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
        /// <param name="voidActiontion"></param>
        void Void<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> voidActiontion);

        /// <summary>
        /// 返回结果
        /// </summary>
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
        /// <param name="voidActiontion"></param>
        void Void<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> voidActiontion);
    }

    /// <summary>
    /// 模拟行为
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IMockSetup<T, TResult> : IMockSetup
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        /// <param name="result"></param>
        void Return(TResult result);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <param name="valueFunction"></param>
        void Return(Func<T, TResult> valueFunction);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="valueFunction"></param>
        void Return<T1>(Func<T, T1, TResult> valueFunction);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="valueFunction"></param>
        void Return<T1, T2>(Func<T, T1, T2, TResult> valueFunction);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="valueFunction"></param>
        void Return<T1, T2, T3>(Func<T, T1, T2, T3, TResult> valueFunction);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="valueFunction"></param>
        void Return<T1, T2, T3, T4>(Func<T, T1, T2, T3, T4, TResult> valueFunction);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="valueFunction"></param>
        void Return<T1, T2, T3, T4, T5>(Func<T, T1, T2, T3, T4, T5, TResult> valueFunction);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <param name="valueFunction"></param>
        void Return<T1, T2, T3, T4, T5, T6>(Func<T, T1, T2, T3, T4, T5, T6, TResult> valueFunction);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <param name="valueFunction"></param>
        void Return<T1, T2, T3, T4, T5, T6, T7>(Func<T, T1, T2, T3, T4, T5, T6, T7, TResult> valueFunction);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <param name="valueFunction"></param>
        void Return<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, TResult> valueFunction);

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <typeparam name="T9"></typeparam>
        /// <param name="valueFunction"></param>
        void Return<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> valueFunction);

        /// <summary>
        /// 返回结果
        /// </summary>
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
        /// <param name="valueFunction"></param>
        void Return<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> valueFunction);

        /// <summary>
        /// 返回结果
        /// </summary>
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
        /// <param name="valueFunction"></param>
        void Return<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> valueFunction);

        /// <summary>
        /// 返回结果
        /// </summary>
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
        /// <param name="valueFunction"></param>
        void Return<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> valueFunction);

        /// <summary>
        /// 返回结果
        /// </summary>
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
        /// <param name="valueFunction"></param>
        void Return<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> valueFunction);

        /// <summary>
        /// 返回结果
        /// </summary>
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
        /// <param name="valueFunction"></param>
        void Return<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> valueFunction);

        /// <summary>
        /// 返回结果
        /// </summary>
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
        /// <param name="valueFunction"></param>
        void Return<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> valueFunction);
    }
}