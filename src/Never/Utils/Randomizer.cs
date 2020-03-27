using Never.Collections;
using System;

namespace Never.Utils
{
    /// <summary>
    /// 随机数
    /// </summary>
    public static class Randomizer
    {
        #region ctor

        /// <summary>
        /// Initializes static members of the <see cref="Randomizer"/> class.
        /// </summary>
        static Randomizer()
        {
        }

        #endregion

        #region next

        /// <summary>
        /// Nexts this instance.
        /// </summary>
        /// <returns></returns>
        public static int Next()
        {
            return Next(int.MaxValue - 1);
        }

        /// <summary>
        /// Nexts the specified maximum value.
        /// </summary>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">当前随机数的取值超出了所定义的范围</exception>
        public static int Next(int maxValue)
        {
            return Next(maxValue, new Random());
        }

        /// <summary>
        ///  Nexts the specified maximum value.
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int Next(int minValue, int maxValue)
        {
            return Next(minValue, maxValue, new Random());
        }

        /// <summary>
        /// Nexts the specified maximum value.
        /// </summary>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="random"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception">当前随机数的取值超出了所定义的范围</exception>
        public static int Next(int maxValue, Random random)
        {
            try
            {
                return (random ?? new Random()).Next(maxValue);
            }
            catch (ArgumentOutOfRangeException outEx)
            {
                throw new Exception("当前随机数的取值超出了所定义的范围", outEx);
            }
        }

        /// <summary>
        /// Nexts the specified maximum value.
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="random"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception">当前随机数的取值超出了所定义的范围</exception>
        public static int Next(int minValue, int maxValue, Random random)
        {
            try
            {
                return (random ?? new Random()).Next(minValue, maxValue);
            }
            catch (ArgumentOutOfRangeException outEx)
            {
                throw new Exception("当前随机数的取值超出了所定义的范围", outEx);
            }
        }

        /// <summary>
        /// 扑克牌一样从目标数量中随即抽取牌
        /// </summary>
        /// <param name="capacity">容量</param>
        /// <returns></returns>
        public static int[] PokerArray(int capacity)
        {
            return PokerArray(capacity, capacity);
        }

        /// <summary>
        /// 扑克牌一样从目标数量中随即抽取牌
        /// </summary>
        /// <param name="capacity">容量</param>
        /// <param name="targetNum">要抽取目标数，不能大于容量</param>
        /// <returns></returns>
        public static int[] PokerArray(int capacity, int targetNum)
        {
            return PokerArray(capacity, targetNum, new Random());
        }

        /// <summary>
        /// 扑克牌一样从目标数量中随即抽取牌
        /// </summary>
        /// <param name="capacity">容量</param>
        /// <param name="targetNum">要抽取目标数，不能大于容量</param>
        /// <param name="random">随机数</param>
        /// <returns></returns>
        public static int[] PokerArray(int capacity, int targetNum, Random random)
        {
            if (targetNum <= 0 || capacity <= 0)
                return new int[0];

            if (targetNum > capacity)
                throw new ArgumentOutOfRangeException(string.Format("the targetNum {0} can not > capaicity {1}", targetNum, capacity));

            var temp = new int[capacity];
            for (int i = 0; i < capacity; i++)
                temp[i] = i;

            var a = new int[targetNum];
            for (int i = 0; i < targetNum; i++)
            {
                int j = Next(capacity - i, random);
                a[i] = temp[j];
                temp[j] = temp[capacity - i - 1];
            }

            return a;
        }

        /// <summary>
        /// 扑克牌一样从目标数量中随即抽取牌
        /// </summary>
        /// <param name="array">源数据容量</param>
        /// <returns></returns>
        public static T[] PokerArray<T>(T[] array)
        {
            if (array == null)
                return new T[0];

            return PokerArray(array, array.Length, new Random());
        }
        /// <summary>
        /// 扑克牌一样从目标数量中随即抽取牌
        /// </summary>
        /// <param name="array">源数据容量</param>
        /// <param name="targetNum">要抽取目标数，不能大于源数据容量的长度</param>
        /// <returns></returns>
        public static T[] PokerArray<T>(T[] array, int targetNum)
        {
            return PokerArray(array, targetNum, new Random());
        }

        /// <summary>
        /// 扑克牌一样从目标数量中随即抽取牌
        /// </summary>
        /// <param name="array">源数据容量</param>
        /// <param name="targetNum">要抽取目标数，不能大于源数据容量的长度</param>
        /// <param name="random">随机数</param>
        /// <returns></returns>
        public static T[] PokerArray<T>(T[] array, int targetNum, Random random)
        {
            if (array == null || array.Length <= 0)
                return new T[0];

            if (targetNum <= 0)
                return new T[0];

            var capacity = array.Length;
            if (targetNum > capacity)
                throw new ArgumentOutOfRangeException(string.Format("the targetNum {0} can not > capaicity {1}", targetNum, capacity));

            var a = new T[targetNum];
            for (int i = 0; i < targetNum; i++)
            {
                int j = Next(capacity - i, random);
                a[i] = array[j];
                array[j] = array[capacity - i - 1];
            }

            return a;
        }

        /// <summary>
        /// 从0到capacity中随机抽取数字
        /// </summary>
        /// <param name="capacity">容量</param>
        /// <returns></returns>
        public static int[] RandomArrary(int capacity)
        {
            return PokerArray(capacity, capacity, new Random());
        }

        /// <summary>
        /// 从0到capacity中随机抽取数字
        /// </summary>
        /// <param name="capacity">容量</param>
        /// <param name="targetNum">要抽取目标数，如果大于容量，则每一次起至位置数字(targetNum/capacity)重新设定为0到capacity</param>
        /// <returns></returns>
        public static int[] RandomArrary(int capacity, int targetNum)
        {
            return RandomArrary(capacity, targetNum, new Random());
        }

        /// <summary>
        /// 从0到capacity中随机抽取数字
        /// </summary>
        /// <param name="capacity">容量</param>
        /// <param name="targetNum">要抽取目标数，如果大于容量，则每一次起至位置数字(targetNum/capacity)重新设定为0到capacity</param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static int[] RandomArrary(int capacity, int targetNum, Random random)
        {
            if (targetNum <= 0 || capacity <= 0)
                return new int[0];

            var temp = new int[capacity];
            for (int i = 0; i < capacity; i++)
                temp[i] = i;

            if (targetNum <= capacity)
            {
                var a = new int[targetNum];
                for (int i = 0; i < targetNum; i++)
                {
                    int j = Next(capacity - i, random);
                    a[i] = temp[j];
                    temp[j] = temp[capacity - i - 1];
                }

                return a;
            }

            var b = new int[targetNum];
            for (int i = 0; i < capacity; i++)
            {
                int j = Next(capacity - i, random);
                b[i] = temp[j];
                temp[j] = temp[capacity - i - 1];
            }

            var k = targetNum / capacity;
            for (var u = 1; u <= k; u++)
            {
                for (int i = 0; i < capacity; i++)
                    temp[i] = i;

                var newc = capacity * u;
                for (int i = newc; i < newc + capacity && i < targetNum; i++)
                {
                    int j = Next(newc + capacity - i, random);
                    b[i] = temp[j];
                    temp[j] = temp[newc + capacity - i - 1];
                }
            }

            return b;
        }

        /// <summary>
        /// 从0到capacity中随机抽取数字
        /// </summary>
        /// <param name="array">源数据容量</param>
        /// <returns></returns>
        public static T[] RandomArrary<T>(T[] array)
        {
            if (array == null)
                return new T[0];

            return RandomArrary(array, array.Length, new Random());
        }

        /// <summary>
        /// 从0到capacity中随机抽取数字
        /// </summary>
        /// <param name="array">源数据容量</param>
        /// <param name="targetNum">要抽取目标数，如果大于源数据容量长度，则每一次起至位置数字(targetNum/capacity)重新设定为0到capacity</param>
        /// <returns></returns>
        public static T[] RandomArrary<T>(T[] array, int targetNum)
        {
            return RandomArrary(array, targetNum, new Random());
        }

        /// <summary>
        /// 从0到capacity中随机抽取数字
        /// </summary>
        /// <param name="array">源数据容量</param>
        /// <param name="targetNum">要抽取目标数，如果大于源数据容量长度，则每一次起至位置数字(targetNum/capacity)重新设定为0到capacity</param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static T[] RandomArrary<T>(T[] array, int targetNum, Random random)
        {
            if (array == null || array.Length <= 0)
                return new T[0];

            var capacity = array.Length;
            if (targetNum <= 0)
                return new T[0];

            var temp = new T[capacity];
            for (int i = 0; i < capacity; i++)
                temp[i] = array[i];

            if (targetNum <= capacity)
            {
                var a = new T[targetNum];
                for (int i = 0; i < targetNum; i++)
                {
                    int j = Next(capacity - i, random);
                    a[i] = temp[j];
                    temp[j] = temp[capacity - i - 1];
                }

                return a;
            }

            var b = new T[targetNum];
            for (int i = 0; i < capacity; i++)
            {
                int j = Next(capacity - i, random);
                b[i] = temp[j];
                temp[j] = temp[capacity - i - 1];
            }

            var k = targetNum / capacity;
            for (var u = 1; u <= k; u++)
            {
                for (int i = 0; i < capacity; i++)
                    temp[i] = array[i];

                var newc = capacity * u;
                for (int i = newc; i < newc + capacity && i < targetNum; i++)
                {
                    int j = Next(newc + capacity - i, random);
                    b[i] = temp[j];
                    temp[j] = temp[newc + capacity - i - 1];
                }
            }

            return b;
        }
        #endregion
    }
}