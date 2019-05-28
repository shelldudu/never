using Never.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Serialization.Json.Serialize
{
    /// <summary>
    /// 枚举型处理
    /// </summary>
    internal class EnumSerializerBuilder<T> : Serialize.SerialierBuilder<T> where T : struct, IConvertible
    {
        #region field

        /// <summary>
        /// 委托
        /// </summary>
        private static Func<T, string> function = null;

        /// <summary>
        ///
        /// </summary>
        private readonly static object locker = new object();

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumSerializerBuilder{T}"/> class.
        /// </summary>
        public EnumSerializerBuilder()
        {
        }

        #endregion ctor

        #region static build

        /// <summary>
        /// 进行构建
        /// </summary>
        /// <returns></returns>
        public static Func<T, string> Register(JsonSerializeSetting setting)
        {
            if (function != null)
                return function;

            lock (locker)
            {
                if (function != null)
                    return function;

                function = new EnumSerializerBuilder<T>().Build(setting);
                return function;
            }
        }

        #endregion static build

        #region SerialierBuilder

        /// <summary>
        /// 进行构建
        /// </summary>
        /// <returns></returns>
        public Func<T, string> Build(JsonSerializeSetting setting)
        {
            if (setting.WriteNumberOnEnumType)
                return Enum.GetUnderlyingType(this.TargetType) == typeof(ulong) ? this.BuildWithNumberForULong() : this.BuildWithNumberForLong();

            return Enum.GetUnderlyingType(this.TargetType) == typeof(ulong) ? this.BuildForULong() : this.BuildForLong();
        }

        /// <summary>
        /// ulong
        /// </summary>
        private Func<T, string> BuildForULong()
        {
            var emit = EasyEmitBuilder<Func<T, string>>.NewDynamicMethod();
            IDictionary<ulong, KeyValuePair<string, T>> enumValus = null;
            enumValus = EnumValues<T>.ToULongDictionary();
            if (enumValus.Count == 0)
            {
                emit.LoadConstant("");
                emit.Return();
                return emit.CreateDelegate();
            }

            var type = typeof(ulong);
            var donelbl = emit.DefineLabel();
            var defaultLbl = emit.DefineLabel();

            var local = emit.DeclareLocal(type);
            var stringLocal = emit.DeclareLocal(typeof(string));

            var writeLabels = new List<ILabel>(enumValus.Count);
            var jumpLabels = new List<ILabel>(enumValus.Count);
            foreach (var i in enumValus)
            {
                writeLabels.Add(emit.DefineLabel());
                jumpLabels.Add(emit.DefineLabel());
            }

            emit.LoadArgument(0);
            emit.Convert(type);
            emit.StoreLocal(local);
            emit.Branch(jumpLabels[0]);
            var index = -1;
            foreach (var l in enumValus)
            {
                index++;
                emit.MarkLabel(jumpLabels[index]);
                emit.LoadConstant(l.Key);
                emit.LoadLocal(local);
                if (index == enumValus.Count - 1)
                {
                    emit.UnsignedBranchIfNotEqual(defaultLbl);
                    emit.Nop();
                }
                else
                {
                    emit.UnsignedBranchIfNotEqual(jumpLabels[index + 1]);
                    emit.Nop();
                }
                emit.Branch(writeLabels[index]);
                emit.Nop();
            }

            index = -1;
            foreach (var l in enumValus)
            {
                index++;
                emit.MarkLabel(writeLabels[index]);
                emit.LoadConstant(l.Value.Key);
                emit.StoreLocal(stringLocal);
                emit.Branch(donelbl);
            }

            /*default*/
            emit.MarkLabel(defaultLbl);
            emit.LoadLocalAddress(local);
            emit.CallVirtual(typeof(ulong).GetMethod("ToString", Type.EmptyTypes));
            emit.StoreLocal(stringLocal);
            emit.Branch(donelbl);

            /*done*/
            emit.MarkLabel(donelbl);
            emit.LoadLocal(stringLocal);
            emit.Return();
            return emit.CreateDelegate();
        }

        /// <summary>
        /// long
        /// </summary>
        private Func<T, string> BuildWithNumberForULong()
        {
            var emit = EasyEmitBuilder<Func<T, string>>.NewDynamicMethod();
            var local = emit.DeclareLocal(typeof(ulong));
            emit.LoadArgument(0);
            emit.Convert(typeof(ulong));
            emit.StoreLocal(local);
            emit.LoadLocalAddress(local);
            emit.CallVirtual(typeof(ulong).GetMethod("ToString", Type.EmptyTypes));
            emit.Return();
            return emit.CreateDelegate();
        }

        /// <summary>
        /// long
        /// </summary>
        private Func<T, string> BuildForLong()
        {
            var emit = EasyEmitBuilder<Func<T, string>>.NewDynamicMethod();
            IDictionary<long, KeyValuePair<string, T>> enumValus = null;
            enumValus = EnumValues<T>.ToLongDictionary();
            if (enumValus.Count == 0)
            {
                emit.LoadConstant("");
                emit.Return();
                return emit.CreateDelegate();
            }

            var type = typeof(long);
            var donelbl = emit.DefineLabel();
            var defaultLbl = emit.DefineLabel();
            var local = emit.DeclareLocal(type);
            var stringLocal = emit.DeclareLocal(typeof(string));
            var writeLabels = new List<ILabel>(enumValus.Count);
            var jumpLabels = new List<ILabel>(enumValus.Count);
            foreach (var i in enumValus)
            {
                writeLabels.Add(emit.DefineLabel());
                jumpLabels.Add(emit.DefineLabel());
            }

            emit.LoadArgument(0);
            emit.Convert(type);
            emit.StoreLocal(local);

            emit.Branch(jumpLabels[0]);
            var index = -1;
            foreach (var l in enumValus)
            {
                index++;
                emit.MarkLabel(jumpLabels[index]);
                emit.LoadConstant(l.Key);
                emit.LoadLocal(local);
                emit.BranchIfEqual(writeLabels[index]);
                if (index == enumValus.Count - 1)
                    emit.Branch(defaultLbl);
                else
                    emit.Branch(jumpLabels[index + 1]);
            };

            index = -1;
            /*write lbl*/
            foreach (var l in enumValus)
            {
                index++;
                emit.MarkLabel(writeLabels[index]);
                emit.LoadConstant(l.Value.Key);
                emit.StoreLocal(stringLocal);
                emit.Branch(donelbl);
            };

            /*default*/
            emit.MarkLabel(defaultLbl);
            emit.LoadLocalAddress(local);
            emit.CallVirtual(typeof(long).GetMethod("ToString", Type.EmptyTypes));
            emit.StoreLocal(stringLocal);
            emit.Branch(donelbl);

            /*done*/
            emit.MarkLabel(donelbl);
            emit.LoadLocal(stringLocal);
            emit.Return();
            return emit.CreateDelegate();
        }

        /// <summary>
        /// long
        /// </summary>
        private Func<T, string> BuildWithNumberForLong()
        {
            var emit = EasyEmitBuilder<Func<T, string>>.NewDynamicMethod();
            var local = emit.DeclareLocal(typeof(long));
            emit.LoadArgument(0);
            emit.Convert(typeof(long));
            emit.StoreLocal(local);
            emit.LoadLocalAddress(local);
            emit.CallVirtual(typeof(long).GetMethod("ToString", Type.EmptyTypes));
            emit.Return();
            return emit.CreateDelegate();
        }

        #endregion SerialierBuilder
    }
}