using Never.Commands;
using Never.Domains;
using Never.Exceptions;
using Never.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Never
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static partial class ObjectExtension
    {
        #region field

        /// <summary>
        /// The array generic type
        /// </summary>
        private readonly static Type arrayGenericType = typeof(IEnumerable<>);

        /// <summary>
        /// The int generic type
        /// </summary>
        private readonly static Type intGenericType = typeof(IEnumerable<int>);

        /// <summary>
        /// The int type
        /// </summary>
        private readonly static Type intType = typeof(int);

        /// <summary>
        /// The string generic type
        /// </summary>
        private readonly static Type stringGenericType = typeof(IEnumerable<string>);

        /// <summary>
        /// The string type
        /// </summary>
        private readonly static Type stringType = typeof(string);

        /// <summary>
        /// The decimal generic type
        /// </summary>
        private readonly static Type decimalGenericType = typeof(IEnumerable<decimal>);

        /// <summary>
        /// The decimal type
        /// </summary>
        private readonly static Type decimalType = typeof(decimal);

        /// <summary>
        /// The short generic type
        /// </summary>
        private readonly static Type shortGenericType = typeof(IEnumerable<short>);

        /// <summary>
        /// The short type
        /// </summary>
        private readonly static Type shortType = typeof(short);

        /// <summary>
        /// The byte generic type
        /// </summary>
        private readonly static Type byteGenericType = typeof(IEnumerable<byte>);

        /// <summary>
        /// The byte type
        /// </summary>
        private readonly static Type byteType = typeof(byte);

        /// <summary>
        /// The character generic type
        /// </summary>
        private readonly static Type charGenericType = typeof(IEnumerable<char>);

        /// <summary>
        /// The character type
        /// </summary>
        private readonly static Type charType = typeof(char);

        /// <summary>
        /// The long generic type
        /// </summary>
        private readonly static Type longGenericType = typeof(IEnumerable<long>);

        /// <summary>
        /// The long type
        /// </summary>
        private readonly static Type longType = typeof(long);

        /// <summary>
        /// The float generic type
        /// </summary>
        private readonly static Type floatGenericType = typeof(IEnumerable<float>);

        /// <summary>
        /// The float type
        /// </summary>
        private readonly static Type floatType = typeof(float);

        /// <summary>
        /// The double generic type
        /// </summary>
        private readonly static Type doubleGenericType = typeof(IEnumerable<double>);

        /// <summary>
        /// The double type
        /// </summary>
        private readonly static Type doubleType = typeof(double);

        /// <summary>
        /// The bool generic type
        /// </summary>
        private readonly static Type boolGenericType = typeof(IEnumerable<bool>);

        /// <summary>
        /// The bool type
        /// </summary>
        private readonly static Type boolType = typeof(bool);

        /// <summary>
        /// The date time generic type
        /// </summary>
        private readonly static Type dateTimeGenericType = typeof(IEnumerable<DateTime>);

        /// <summary>
        /// The date time type
        /// </summary>
        private readonly static Type dateTimeType = typeof(DateTime);

        /// <summary>
        /// The object type
        /// </summary>
        private readonly static Type objectType = typeof(object);

        /// <summary>
        /// The unique identifier type
        /// </summary>
        private readonly static Type guidType = typeof(Guid);

        /// <summary>
        /// The unique identifier generic typey
        /// </summary>
        private readonly static Type guidGenericTypey = typeof(IEnumerable<Guid>);

        /// <summary>
        /// IP验证
        /// </summary>
        private static Regex ipRegex = new Regex(@"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// URI验证
        /// </summary>
        private static Regex uriRegex = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Email验证
        /// </summary>
        private static Regex emailRegex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion field

        #region exception

        /// <summary>
        /// 拿某一异常的信息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns></returns>
        public static string GetMessage(this Exception ex)
        {
            if (ex == null)
                return string.Empty;

            Exception next = ex;
            while (true)
            {
                if (next.InnerException != null)
                    next = next.InnerException;
                else
                    return next.Message;
            }
        }

        /// <summary>
        /// 拿某一异常的信息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns></returns>
        public static string GetFullMessage(this Exception ex)
        {
            if (ex == null)
                return string.Empty;

            Exception next = ex;
            while (true)
            {
                if (next.InnerException == null)
                {
                    var now = DateTime.Now;
                    var sb = new StringBuilder();
                    sb.AppendLine("错误时间：" + now.ToLongDateString() + ",在" + now.ToShortDateString());
                    sb.AppendLine("错误信息：" + next.Message);
                    sb.AppendLine("错误来源：" + next.Source);
                    sb.AppendLine("引发当前异常的方法" + next.TargetSite);
                    sb.AppendLine("堆栈信息：" + next.StackTrace);
                    return sb.ToString();
                }

                next = next.InnerException;
            }
        }

        /// <summary>
        /// 查找最内层异常
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="ex">异常</param>
        /// <returns></returns>
        public static TException GetInnerException<TException>(this Exception ex)
            where TException : Exception
        {
            return (IsInnerException(ex) ? ex.GetBaseException() : ex) as TException;
        }

        /// <summary>
        /// 查找最内层异常
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns></returns>
        public static Exception GetInnerException(this Exception ex)
        {
            return IsInnerException(ex) ? ex.GetBaseException() : ex;
        }

        /// <summary>
        /// 该异常是否还有InnerException异常
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static bool IsInnerException(this Exception ex)
        {
            return ex != null && ex.InnerException != null;
        }

        /// <summary>
        /// 该异常是否是最顶层[公共模块为CommonLanguageRuntimeLibrary]异常
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static bool IsBuiltInException(this Exception ex)
        {
            return ex.GetType().Module.ScopeName == "CommonLanguageRuntimeLibrary";
        }

        #endregion exception

        #region eventHandler

        /// <summary>
        /// 带处理事件的方法扩展
        /// </summary>
        /// <typeparam name="TEventArgs">事件来源</typeparam>
        /// <param name="e">事件数据来源</param>
        /// <param name="sender">事件监听者</param>
        /// <param name="eventDelegate">事件集合</param>
        public static void Raise<TEventArgs>(this TEventArgs e, object sender, ref EventHandler<TEventArgs> eventDelegate) where TEventArgs : EventArgs
        {
            EventHandler<TEventArgs> temp = Interlocked.CompareExchange(ref eventDelegate, null, null);
            if (temp == null)
                return;

            temp(sender, e);
        }

        /// <summary>
        /// 不包含事件数据的事件的方法扩展
        /// </summary>
        /// <param name="e">事件数据来源，表示为空</param>
        /// <param name="sender">事件监听者</param>
        /// <param name="eventDelegate">事件集合</param>
        public static void Raise(this EventArgs e, object sender, ref EventHandler eventDelegate)
        {
            EventHandler temp = Interlocked.CompareExchange(ref eventDelegate, null, null);
            if (temp == null)
                return;

            temp(sender, e);
        }

        #endregion eventHandler

        #region clone

        /// <summary>
        /// 深克隆对象实体
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <param name="object">源对象</param>
        /// <returns></returns>
        public static T Clone<T>(this T @object)
        {
            if (@object == null)
                throw new ArgumentNullException("当前对象为空，不可序列化");

            if (@object.GetType() != typeof(T))
                throw new ArgumentException("当前对象类型不相同，无法序列化");

            var ser = new BinarySerializer();

            return ser.Deserialize<T>(ser.SerializeObject(@object));
        }

        #endregion clone

        #region serialize

        /// <summary>
        /// xml序列化
        /// </summary>
        /// <param name="object">源数据对象</param>
        /// <returns></returns>
        public static string XmlSerialize(this object @object)
        {
            if (@object == null)
                return string.Empty;

            return new XmlSerializer().SerializeObject(@object);
        }

        /// <summary>
        /// xml反序列化
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="text">源数据对象</param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return default(T);

            return new XmlSerializer().Deserialize<T>(text);
        }

        #endregion serialize

        #region worker

        /// <summary>
        /// 获取工作者名字
        /// </summary>
        /// <param name="worker">The worker.</param>
        /// <param name="defaultName"></param>
        /// <returns></returns>
        public static string GetWorkerName(this Security.IWorker worker, string defaultName = "sys")
        {
            return worker == null ? defaultName ?? "sys" : worker.WorkerName;
        }

        /// <summary>
        /// 获取工作者名字
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="defaultName"></param>
        /// <returns></returns>
        public static string GetWorkerName(this IWorkContext context, string defaultName = "sys")
        {
            return context == null || context.Worker == null ? defaultName ?? "sys" : context.Worker.WorkerName;
        }

        #endregion worker

        #region root

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <param name="roots"></param>
        /// <returns></returns>
        public static TAggregateRoot MatchAggregateRoot<TAggregateRoot>(this IEnumerable<IAggregateRoot> roots)
            where TAggregateRoot : IAggregateRoot
        {
            if (roots == null)
                return default(TAggregateRoot);

            foreach (var root in roots)
            {
                if (root is TAggregateRoot)
                    return (TAggregateRoot)root;
            }

            return default(TAggregateRoot);
        }

        /// <summary>
        /// 处理结果是否为ok,
        /// 返回true的条件是看<see cref="CommandHandlerStatus.Success"/>或<see cref="CommandHandlerStatus.Idempotent"/>或<see cref="CommandHandlerStatus.NothingChanged"/>
        /// 返回false的条件是看<see cref="CommandHandlerStatus.Fail"/>或<see cref="CommandHandlerStatus.NotExists"/>或<see cref="CommandHandlerStatus.PoorInventory"/>或handlerResult==null
        /// </summary>
        /// <param name="handlerResult">命令处理结果</param>
        /// <returns></returns>
        public static bool Ok(this ICommandHandlerResult handlerResult)
        {
            if (handlerResult == null)
                throw new ArgumentNullException("handlerResult");

            switch (handlerResult.Status)
            {
                case CommandHandlerStatus.Success:
                case CommandHandlerStatus.Idempotent:
                case CommandHandlerStatus.NothingChanged:
                    return true;

                case CommandHandlerStatus.Fail:
                case CommandHandlerStatus.NotExists:
                case CommandHandlerStatus.PoorInventory:
                    return false;

                default:
                    return false;
            }
        }

        #endregion root

        #region enumerable

        /// <summary>
        /// 分割数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="records">The records.</param>
        /// <param name="number">The split numbers.</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> records, int number)
        {
            if (records == null)
                return new IEnumerable<T>[] { records };

            if (records == null || !records.Any())
                return new IEnumerable<T>[] { records };

            var times = (int)Math.Ceiling(records.Count() / (double)number);
            var result = new List<IEnumerable<T>>(times);

            for (var i = 0; i < times; i++)
                result.AddRange(new IEnumerable<T>[] { records.Skip(i * number).Take(number).ToArray() });

            return result.AsEnumerable();
        }

        /// <summary>
        /// 分割数组
        /// </summary>
        /// <param name="records"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static IEnumerable<byte[]> Split(this byte[] records, int number)
        {
            if (records.Length <= number)
                return new List<byte[]>(1) { records };

            var times = records.Length / number;
            var fix = records.Length - times * number;
            var list = new List<byte[]>(fix == 0 ? times : times + 1);
            for (var i = 0; i < times; i++)
            {
                var temp = new byte[number];
                Buffer.BlockCopy(records, i * number, temp, 0, number);
                list.Add(temp);
            }

            if (fix > 0)
            {
                var temp = new byte[fix];
                Buffer.BlockCopy(records, times * number, temp, 0, fix);
                list.Add(temp);
            }

            return list;
        }

        /// <summary>
        /// 使用Foreach遍历
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="invoke">The invoke.</param>
        public static void UseForEach<T>(this IEnumerable<T> array, Action<T> invoke)
        {
            if (array == null)
                return;

            if (invoke == null)
                return;

            foreach (var i in array)
            {
                invoke(i);
            }
        }

        /// <summary>
        /// 使用Foreach遍历
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="invoke">The invoke.</param>
        public static void UseForEach<T>(this IEnumerable<T> array, Action<T, int> invoke)
        {
            if (array == null)
                return;

            if (invoke == null)
                return;

            int j = 0;
            foreach (var i in array)
            {
                invoke(i, j);
                j++;
            }
        }

        /// <summary>
        /// 使用Foreach遍历
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="invoke">如果要中断，则返回false</param>
        public static void UseForEach<T>(this IEnumerable<T> array, Func<T, bool> invoke)
        {
            if (array == null)
                return;

            if (invoke == null)
                return;

            foreach (var i in array)
            {
                if (!invoke(i))
                    return;
            }
        }

        /// <summary>
        /// 使用Foreach遍历
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="invoke">如果要中断，则返回false</param>
        public static void UseForEach<T>(this IEnumerable<T> array, Func<T, int, bool> invoke)
        {
            if (array == null)
                return;

            if (invoke == null)
                return;

            int j = 0;
            foreach (var i in array)
            {
                if (!invoke(i, j))
                    return;

                j++;
            }
        }

        /// <summary>
        /// 返回只读的集合
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static IReadOnlyList<T> AsReadOnly<T>(this IEnumerable<T> array)
        {
            if (array == null)
                return Array.AsReadOnly(new T[0]);

            if (array is IReadOnlyList<T>)
                return (IReadOnlyList<T>)array;
            else if (array is List<T>)
                return (IReadOnlyList<T>)array;
            else if (array is T[])
                return Array.AsReadOnly((T[])array);

            return new List<T>(array).AsReadOnly();
        }

        /// <summary>
        /// 返回只读的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this IEnumerable<T> array)
        {
            if (array == null)
                return Array.AsReadOnly(new T[0]);

            if (array is IReadOnlyCollection<T>)
                return (IReadOnlyCollection<T>)array;
            else if (array is List<T>)
                return (IReadOnlyCollection<T>)array;
            else if (array is SortedSet<T>)
                return (IReadOnlyCollection<T>)array;
            else if (array is T[])
                return Array.AsReadOnly((T[])array);

            return new List<T>(array).AsReadOnly();
        }

        /// <summary>
        /// 是否为空集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> array)
        {
            if (array == null)
                return true;

            if (array.Any())
                return false;

            return true;
        }

        /// <summary>
        /// 是否不为空集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> array)
        {
            if (array == null)
                return false;

            if (array.Any())
                return true;

            return false;
        }

        /// <summary>
        /// 返回数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="one"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsEnumerable<T>(T one)
        {
            return new T[] { one };
        }

        #endregion enumerable
    }
}