using Never.Serialization.Json.MethodProviders;
using Never.Serialization.Json.MethodProviders.DateTimes;
using Never.Serialization.Json.Serialize.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Never.Serialization.Json.Serialize
{
    /// <summary>
    /// 转换方法嫁接
    /// </summary>
    internal class ParseMethodProviderEngrafting
    {
        #region array writer

        private static readonly PrimitiveEnumerableProvider<bool> _boolArrayProvider = new PrimitiveEnumerableProvider<bool>(MethodProviders.BooleanMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<byte> _byteArrayProvider = new PrimitiveEnumerableProvider<byte>(MethodProviders.ByteMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<char> _charArrayProvider = new PrimitiveEnumerableProvider<char>(MethodProviders.CharMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<DateTime> _dateTimeArrayProvider = new PrimitiveEnumerableProvider<DateTime>(MethodProviders.DateTimeMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<decimal> _decimalArrayProvider = new PrimitiveEnumerableProvider<decimal>(MethodProviders.DecimalMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<double> _doubleArrayProvider = new PrimitiveEnumerableProvider<double>(MethodProviders.DoubleMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<float> _floatArrayProvider = new PrimitiveEnumerableProvider<float>(MethodProviders.FloatMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<Guid> _guidArrayProvider = new PrimitiveEnumerableProvider<Guid>(MethodProviders.GuidMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<short> _int16ArrayProvider = new PrimitiveEnumerableProvider<short>(MethodProviders.Int16MethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<int> _int32ArrayProvider = new PrimitiveEnumerableProvider<int>(MethodProviders.Int32MethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<long> _int64ArrayProvider = new PrimitiveEnumerableProvider<long>(MethodProviders.Int64MethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<string> _stringArrayProvider = new PrimitiveEnumerableProvider<string>(MethodProviders.StringMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<TimeSpan> _timeSpanArrayProvider = new PrimitiveEnumerableProvider<TimeSpan>(MethodProviders.TimeSpanMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<ushort> _uint16ArrayProvider = new PrimitiveEnumerableProvider<ushort>(MethodProviders.UInt16MethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<uint> _uint32ArrayProvider = new PrimitiveEnumerableProvider<uint>(MethodProviders.UInt32MethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<ulong> _uint64ArrayProvider = new PrimitiveEnumerableProvider<ulong>(MethodProviders.UInt64MethodProvider.Default);

        #endregion array writer

        #region ctor

        static ParseMethodProviderEngrafting()
        {
        }

        #endregion ctor

        #region type

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, Type source)
        {
            TypeMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion type

        #region bool

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, bool source)
        {
            BooleanMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion bool

        #region byte

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, byte source)
        {
            ByteMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion byte

        #region char

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, char source)
        {
            CharMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion char

        #region datetime

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, DateTime source)
        {
            if (DateTimeFormat.Default.Equals(setting.DateTimeFormat))
            {
                ISO8601StyleDateMethodProvider.Default.Write(writer, setting, source);
                return;
            }

            if (DateTimeFormat.ChineseStyle.Equals(setting.DateTimeFormat))
            {
                ChineseStyleDateMethodProvider.Default.Write(writer, setting, source);
                return;
            }

            if (DateTimeFormat.ISO8601Style.Equals(setting.DateTimeFormat))
            {
                ISO8601StyleDateMethodProvider.Default.Write(writer, setting, source);
                return;
            }

            if (DateTimeFormat.MicrosoftStyle.Equals(setting.DateTimeFormat))
            {
                MicrosoftStyleDateMethodProvider.Default.Write(writer, setting, source);
                return;
            }

            if (DateTimeFormat.RFC1123Style.Equals(setting.DateTimeFormat))
            {
                RFC1123StyleDateMethodProvider.Default.Write(writer, setting, source);
                return;
            }

            IConvertMethodProvider<DateTime> customMethodProvider = CustomSerializationProvider.QueryCustomDatetimeFormat(setting.DateTimeFormat) as IConvertMethodProvider<DateTime>;
            if (customMethodProvider == null)
                throw new ArgumentException("format is not defined");

            customMethodProvider.Write(writer, setting, source);
        }

        #endregion datetime

        #region decimal

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, decimal source)
        {
            DecimalMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion decimal

        #region timespan

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, TimeSpan source)
        {
            TimeSpanMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion timespan

        #region double

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, double source)
        {
            DoubleMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion double

        #region float

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, float source)
        {
            FloatMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion float

        #region guid

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, Guid source)
        {
            GuidMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion guid

        #region short

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, short source)
        {
            Int16MethodProvider.Default.Write(writer, setting, source);
        }

        #endregion short

        #region int

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, int source)
        {
            Int32MethodProvider.Default.Write(writer, setting, source);
        }

        #endregion int

        #region enum

        public static void EnumWrite<T>(ISerializerWriter writer, JsonSerializeSetting setting, T source) where T : struct, IConvertible
        {
            EnumMethodProvider<T>.Default.Write(writer, setting, source);
        }

        #endregion enum

        #region exception

        public static void ExceptionWrite<T>(ISerializerWriter writer, JsonSerializeSetting setting, T source) where T : Exception
        {
            ExceptionMethodProvider<T>.Default.Write(writer, setting, source);
        }

        #endregion exception

        #region long

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, long source)
        {
            Int64MethodProvider.Default.Write(writer, setting, source);
        }

        #endregion long

        #region string

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, string source)
        {
            StringMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion string

        #region ushort

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, ushort source)
        {
            UInt16MethodProvider.Default.Write(writer, setting, source);
        }

        #endregion ushort

        #region uint

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, uint source)
        {
            UInt32MethodProvider.Default.Write(writer, setting, source);
        }

        #endregion uint

        #region ulong

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, ulong source)
        {
            UInt64MethodProvider.Default.Write(writer, setting, source);
        }

        #endregion ulong

        #region arraybool

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<bool> array)
        {
            _boolArrayProvider.Write(writer, setting, array);
        }

        #endregion arraybool

        #region arraybyte

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<byte> array)
        {
            _byteArrayProvider.Write(writer, setting, array);
        }

        #endregion arraybyte

        #region arraychar

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<char> array)
        {
            _charArrayProvider.Write(writer, setting, array);
        }

        #endregion arraychar

        #region arraydatetime

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<DateTime> array)
        {
            _dateTimeArrayProvider.Write(writer, setting, array);
        }

        #endregion arraydatetime

        #region arraydecimal

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<decimal> array)
        {
            _decimalArrayProvider.Write(writer, setting, array);
        }

        #endregion arraydecimal

        #region arraydouble

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<double> array)
        {
            _doubleArrayProvider.Write(writer, setting, array);
        }

        #endregion arraydouble

        #region arrayfloat

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<float> array)
        {
            _floatArrayProvider.Write(writer, setting, array);
        }

        #endregion arrayfloat

        #region arrayguid

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<Guid> array)
        {
            _guidArrayProvider.Write(writer, setting, array);
        }

        #endregion arrayguid

        #region arrayshort

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<short> array)
        {
            _int16ArrayProvider.Write(writer, setting, array);
        }

        #endregion arrayshort

        #region arrayint

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<int> array)
        {
            _int32ArrayProvider.Write(writer, setting, array);
        }

        #endregion arrayint

        #region arraylong

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<long> array)
        {
            _int64ArrayProvider.Write(writer, setting, array);
        }

        #endregion arraylong

        #region arraystring

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<string> array)
        {
            _stringArrayProvider.WriteString(writer, setting, array);
        }

        #endregion arraystring

        #region arrayushort

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<ushort> array)
        {
            _uint16ArrayProvider.Write(writer, setting, array);
        }

        #endregion arrayushort

        #region array timespan

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<TimeSpan> array)
        {
            _timeSpanArrayProvider.Write(writer, setting, array);
        }

        #endregion array timespan

        #region arrayuint

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<uint> array)
        {
            _uint32ArrayProvider.Write(writer, setting, array);
        }

        #endregion arrayuint

        #region arrayulong

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<ulong> array)
        {
            _uint64ArrayProvider.Write(writer, setting, array);
        }

        #endregion arrayulong

        #region array complex

        public static void WriteGenericArray<T>(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<T> array, byte level)
        {
            IEnumerableProvider<T>.Default.Write(writer, setting, level, array);
        }

        public static void WriteEnumArray<T>(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<T> array, byte level) where T : struct, IConvertible
        {
            EnumEnumerableProvider<T>.Default.Write(writer, setting, level, array);
        }

        public static void WriteNullableEnumArray<T>(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<Nullable<T>> array, byte level) where T : struct, IConvertible
        {
            NullableEnumEnumerableProvider<T>.Default.Write(writer, setting, level, array);
        }

        public static void WriteObjectArray(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<object> array, byte level)
        {
            ObjectEnumerableProvider.Default.Write(writer, setting, level, array);
        }

        public static void WriteEnumerableArray(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable array, byte level)
        {
            ObjectEnumerableProvider.Default.Write(writer, setting, level, array);
        }

        #endregion array complex

        #region key value

        public static void WriteGenericKeyValue<Key, Value>(ISerializerWriter writer, JsonSerializeSetting setting, IDictionary<Key, Value> array, byte level)
        {
            IDictionaryProvider<Key, Value>.Default.Write(writer, setting, level, array);
        }

        public static void WriteGenericKeyObjectValue<Key>(ISerializerWriter writer, JsonSerializeSetting setting, IDictionary<Key, object> array, byte level)
        {
            IDictionaryProvider<Key, object>.Default.WriteObjectValue(writer, setting, level, array);
        }

        public static void WriteKeyValue(ISerializerWriter writer, JsonSerializeSetting setting, IDictionary array, byte level)
        {
            IDictionaryProvider.Default.Write(writer, setting, level, array);
        }

        #endregion key value
    }
}