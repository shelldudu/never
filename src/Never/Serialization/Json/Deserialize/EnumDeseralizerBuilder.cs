using Never.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Serialization.Json.Deserialize
{
    /// <summary>
    /// 枚举型处理
    /// </summary>
    internal class EnumDeseralizerBuilder<T> : DeseralizerBuilder<T> where T : struct, IConvertible
    {
        #region field

        /// <summary>
        /// 委托
        /// </summary>
        private static Func<string, T> function = null;

        /// <summary>
        ///
        /// </summary>
        private readonly static object locker = new object();

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumDeseralizerBuilder{T}"/> class.
        /// </summary>
        public EnumDeseralizerBuilder()
        {
        }

        #endregion ctor

        #region static build

        /// <summary>
        /// 进行构建
        /// </summary>
        /// <returns></returns>
        public static Func<string, T> Register(JsonDeserializeSetting setting)
        {
            if (function != null)
                return function;

            lock (locker)
            {
                if (function != null)
                    return function;

                function = new EnumDeseralizerBuilder<T>().Build(setting);
                return function;
            }
        }

        #endregion static build

        #region parse

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public long LongParse(string str)
        {
            long result = 0L;
            long.TryParse(str, out result);
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public ulong UlongParse(string str)
        {
            ulong result = 0L;
            ulong.TryParse(str, out result);
            return result;
        }

        #endregion parse

        #region DeseralizerBuilder

        /// <summary>
        /// 进行构建
        /// </summary>
        /// <returns></returns>
        public Func<string, T> Build(JsonDeserializeSetting setting)
        {
            return Enum.GetUnderlyingType(this.TargetType) == typeof(ulong) ? this.BuildForULong() : this.BuildForLong();
        }

        private Func<string, T> BuildForLong()
        {
            var emit = EasyEmitBuilder<Func<string, T>>.NewDynamicMethod();
            var longValues = EnumValues<T>.ToLongDictionary();
            var enumValus = new Dictionary<string, long>(longValues.Count * 2);
            foreach (var x in longValues)
            {
                enumValus[x.Key.ToString()] = x.Key;
                enumValus[x.Value.Key] = x.Key;
            }
            if (enumValus.Count == 0)
            {
                emit.LoadConstant(0L);
                emit.Return();
                return emit.CreateDelegate();
            }
            var writeLabels = new List<ILabel>(enumValus.Count);
            var jumpLabels = new List<ILabel>(enumValus.Count);
            foreach (var i in enumValus)
            {
                writeLabels.Add(emit.DefineLabel());
                jumpLabels.Add(emit.DefineLabel());
            }

            var longLocal = emit.DeclareLocal(typeof(long));
            var equalLocal = emit.DeclareLocal(typeof(bool));
            var donelbl = emit.DefineLabel();
            var defaultLbl = emit.DefineLabel();

            emit.Branch(jumpLabels[0]);
            var index = -1;
            foreach (var l in enumValus)
            {
                index++;
                emit.MarkLabel(jumpLabels[index]);
                emit.LoadConstant(l.Key);
                emit.LoadArgument(0);
                //emit.Call(typeof(String).GetMethod("op_Equality", new[] { typeof(string), typeof(string) }));
                emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("StringEqualityOnDeseralizing", new[] { typeof(string), typeof(string) }));
                emit.StoreLocal(equalLocal);
                emit.LoadLocal(equalLocal);
                emit.BranchIfTrue(writeLabels[index]);
                if (index == enumValus.Count - 1)
                    emit.Branch(defaultLbl);
                else
                    emit.Branch(jumpLabels[index + 1]);
            };

            /*write lbl*/
            index = -1;
            foreach (var l in enumValus)
            {
                index++;
                emit.MarkLabel(writeLabels[index]);
                emit.LoadConstant(l.Value);
                emit.StoreLocal(longLocal);
                emit.Branch(donelbl);
            };

            /*default*/
            emit.MarkLabel(defaultLbl);
            emit.LoadArgument(0);
            emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LongParseFromStringOnDeseralizing", new[] { typeof(string) }));
            //emit.Convert(typeof(long));
            emit.StoreLocal(longLocal);
            emit.Branch(donelbl);

            /*done*/
            emit.MarkLabel(donelbl);
            emit.LoadLocal(longLocal);
            emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("LongParseToEnumOnDeseralizing", new[] { typeof(long) }).MakeGenericMethod(new[] { typeof(T) }));
            emit.Return();
            //Console.WriteLine(emit.ToString());
            return emit.CreateDelegate();
        }

        private Func<string, T> BuildForULong()
        {
            var emit = EasyEmitBuilder<Func<string, T>>.NewDynamicMethod();
            var ulongValues = EnumValues<T>.ToULongDictionary();
            var enumValus = new Dictionary<string, ulong>(ulongValues.Count * 2);
            foreach (var x in ulongValues)
            {
                enumValus[x.Key.ToString()] = x.Key;
                enumValus[x.Value.Key] = x.Key;
            }
            if (enumValus.Count == 0)
            {
                emit.LoadConstant(0UL);
                emit.Return();
                return emit.CreateDelegate();
            }

            var writeLabels = new List<ILabel>(enumValus.Count);
            var jumpLabels = new List<ILabel>(enumValus.Count);
            foreach (var i in enumValus)
            {
                writeLabels.Add(emit.DefineLabel());
                jumpLabels.Add(emit.DefineLabel());
            }

            var ulongLocal = emit.DeclareLocal(typeof(ulong));
            var equalLocal = emit.DeclareLocal(typeof(bool));
            var donelbl = emit.DefineLabel();
            var defaultLbl = emit.DefineLabel();

            emit.Branch(jumpLabels[0]);
            var index = -1;
            foreach (var l in enumValus)
            {
                index++;
                emit.MarkLabel(jumpLabels[index]);
                emit.LoadConstant(l.Key);
                emit.LoadArgument(0);
                //emit.Call(typeof(String).GetMethod("op_Equality", new[] { typeof(string), typeof(string) }));
                emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("StringEqualityOnDeseralizing", new[] { typeof(string), typeof(string) }));
                emit.StoreLocal(equalLocal);
                emit.LoadLocal(equalLocal);
                emit.BranchIfTrue(writeLabels[index]);
                if (index == enumValus.Count - 1)
                    emit.Branch(defaultLbl);
                else
                    emit.Branch(jumpLabels[index + 1]);
            };

            /*write lbl*/
            index = -1;
            foreach (var l in enumValus)
            {
                index++;
                emit.MarkLabel(writeLabels[index]);
                emit.LoadConstant(l.Value);
                // emit.LoadConstant(3L);
                emit.StoreLocal(ulongLocal);
                emit.Branch(donelbl);
            };

            /*default*/
            emit.MarkLabel(defaultLbl);
            emit.LoadArgument(0);
            emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("UlongParseFromStringOnDeseralizing", new[] { typeof(string) }));
            emit.StoreLocal(ulongLocal);
            emit.Branch(donelbl);

            /*done*/
            emit.MarkLabel(donelbl);
            emit.LoadLocal(ulongLocal);
            emit.Call(typeof(DeseralizerBuilderHelper).GetMethod("UlongParseToEnumOnDeseralizing", new[] { typeof(long) }).MakeGenericMethod(new[] { typeof(T) }));
            emit.Return();
            //Console.WriteLine(emit.ToString());
            return emit.CreateDelegate();
        }

        #endregion DeseralizerBuilder
    }
}