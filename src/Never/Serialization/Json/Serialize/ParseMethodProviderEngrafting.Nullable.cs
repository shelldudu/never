using Never.Serialization.Json.MethodProviders.Nullables;
using Never.Serialization.Json.MethodProviders.Nullables.DateTimes;
using Never.Serialization.Json.Serialize.Enumerators;
using System;
using System.Collections.Generic;

namespace Never.Serialization.Json.Serialize
{
    /// <summary>
    /// 转换方法嫁接
    /// </summary>
    internal class NullableParseMethodProviderEngrafting
    {
        #region array writer

        private static readonly PrimitiveEnumerableProvider<bool?> _nullableBoolArrayProvider = new PrimitiveEnumerableProvider<bool?>(MethodProviders.Nullables.BooleanMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<byte?> _nullableByteArrayProvider = new PrimitiveEnumerableProvider<byte?>(MethodProviders.Nullables.ByteMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<char?> _nullableCharArrayProvider = new PrimitiveEnumerableProvider<char?>(MethodProviders.Nullables.CharMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<DateTime?> _nullableDateTimeArrayProvider = new PrimitiveEnumerableProvider<DateTime?>(MethodProviders.Nullables.DateTimeMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<decimal?> _nullableDecimalArrayProvider = new PrimitiveEnumerableProvider<decimal?>(MethodProviders.Nullables.DecimalMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<double?> _nullableDoubleArrayProvider = new PrimitiveEnumerableProvider<double?>(MethodProviders.Nullables.DoubleMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<float?> _nullableFloatArrayProvider = new PrimitiveEnumerableProvider<float?>(MethodProviders.Nullables.FloatMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<Guid?> _nullableGuidArrayProvider = new PrimitiveEnumerableProvider<Guid?>(MethodProviders.Nullables.GuidMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<short?> _nullableInt16ArrayProvider = new PrimitiveEnumerableProvider<short?>(MethodProviders.Nullables.Int16MethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<int?> _nullableInt32ArrayProvider = new PrimitiveEnumerableProvider<int?>(MethodProviders.Nullables.Int32MethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<long?> _nullableInt64ArrayProvider = new PrimitiveEnumerableProvider<long?>(MethodProviders.Nullables.Int64MethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<TimeSpan?> _nullableTimeSpanArrayProvider = new PrimitiveEnumerableProvider<TimeSpan?>(MethodProviders.Nullables.TimeSpanMethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<ushort?> _nullableUint16ArrayProvider = new PrimitiveEnumerableProvider<ushort?>(MethodProviders.Nullables.UInt16MethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<uint?> _nullableUint32ArrayProvider = new PrimitiveEnumerableProvider<uint?>(MethodProviders.Nullables.UInt32MethodProvider.Default);
        private static readonly PrimitiveEnumerableProvider<ulong?> _nullableUint64ArrayProvider = new PrimitiveEnumerableProvider<ulong?>(MethodProviders.Nullables.UInt64MethodProvider.Default);

        #endregion array writer

        #region ctor

        static NullableParseMethodProviderEngrafting()
        {
        }

        #endregion ctor

        #region bool

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, bool? source)
        {
            BooleanMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion bool

        #region byte

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, byte? source)
        {
            ByteMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion byte

        #region char

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, char? source)
        {
            CharMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion char

        #region datetime

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, DateTime? source)
        {
            if (setting.DateTimeFormat.Equals(DateTimeFormat.Default))
            {
                ISO8601StyleDateMethodProvider.Default.Write(writer, setting, source);
                return;
            }

            if (setting.DateTimeFormat.Equals(DateTimeFormat.ChineseStyle))
            {
                ChineseStyleDateMethodProvider.Default.Write(writer, setting, source);
                return;
            }
            if (setting.DateTimeFormat.Equals(DateTimeFormat.ISO8601Style))
            {
                ISO8601StyleDateMethodProvider.Default.Write(writer, setting, source);
                return;
            }

            if (setting.DateTimeFormat.Equals(DateTimeFormat.MicrosoftStyle))
            {
                MicrosoftStyleDateMethodProvider.Default.Write(writer, setting, source);
                return;
            }

            if (setting.DateTimeFormat.Equals(DateTimeFormat.RFC1123Style))
            {
                RFC1123StyleDateMethodProvider.Default.Write(writer, setting, source);
                return;
            }
            IConvertMethodProvider<DateTime?> customMethodProvider = CustomSerializationProvider.QueryCustomDatetimeFormat(setting.DateTimeFormat) as IConvertMethodProvider<DateTime?>;
            if (customMethodProvider == null)
                throw new ArgumentException("format is not defined");

            customMethodProvider.Write(writer, setting, source);
        }

        #endregion datetime

        #region decimal

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, decimal? source)
        {
            DecimalMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion decimal

        #region double

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, double? source)
        {
            DoubleMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion double

        #region float

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, float? source)
        {
            FloatMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion float

        #region guid

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, Guid? source)
        {
            GuidMethodProvider.Default.Write(writer, setting, source);
        }

        #endregion guid

        #region short

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, short? source)
        {
            Int16MethodProvider.Default.Write(writer, setting, source);
        }

        #endregion short

        #region int

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, int? source)
        {
            Int32MethodProvider.Default.Write(writer, setting, source);
        }

        #endregion int

        #region long

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, long? source)
        {
            Int64MethodProvider.Default.Write(writer, setting, source);
        }

        #endregion long

        #region ushort

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, ushort? source)
        {
            UInt16MethodProvider.Default.Write(writer, setting, source);
        }

        #endregion ushort

        #region uint

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, uint? source)
        {
            UInt32MethodProvider.Default.Write(writer, setting, source);
        }

        #endregion uint

        #region ulong

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, ulong? source)
        {
            UInt64MethodProvider.Default.Write(writer, setting, source);
        }

        #endregion ulong

        #region enum

        public static void EnumWrite<T>(ISerializerWriter writer, JsonSerializeSetting setting, Nullable<T> source) where T : struct, IConvertible
        {
            EnumMethodProvider<T>.Default.Write(writer, setting, source);
        }

        #endregion enum

        #region arraybool

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<bool?> array)
        {
            _nullableBoolArrayProvider.Write(writer, setting, array);
        }

        #endregion arraybool

        #region arraybyte

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<byte?> array)
        {
            _nullableByteArrayProvider.Write(writer, setting, array);
        }

        #endregion arraybyte

        #region arraychar

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<char?> array)
        {
            _nullableCharArrayProvider.Write(writer, setting, array);
        }

        #endregion arraychar

        #region arraydatetime

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<DateTime?> array)
        {
            _nullableDateTimeArrayProvider.Write(writer, setting, array);
        }

        #endregion arraydatetime

        #region arraydecimal

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<decimal?> array)
        {
            _nullableDecimalArrayProvider.Write(writer, setting, array);
        }

        #endregion arraydecimal

        #region arraydouble

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<double?> array)
        {
            _nullableDoubleArrayProvider.Write(writer, setting, array);
        }

        #endregion arraydouble

        #region arrayfloat

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<float?> array)
        {
            _nullableFloatArrayProvider.Write(writer, setting, array);
        }

        #endregion arrayfloat

        #region arrayguid

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<Guid?> array)
        {
            _nullableGuidArrayProvider.Write(writer, setting, array);
        }

        #endregion arrayguid

        #region arrayshort

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<short?> array)
        {
            _nullableInt16ArrayProvider.Write(writer, setting, array);
        }

        #endregion arrayshort

        #region arrayint

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<int?> array)
        {
            _nullableInt32ArrayProvider.Write(writer, setting, array);
        }

        #endregion arrayint

        #region arraylong

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<long?> array)
        {
            _nullableInt64ArrayProvider.Write(writer, setting, array);
        }

        #endregion arraylong

        #region arrayushort

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<ushort?> array)
        {
            _nullableUint16ArrayProvider.Write(writer, setting, array);
        }

        #endregion arrayushort

        #region arrayuint

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<uint?> array)
        {
            _nullableUint32ArrayProvider.Write(writer, setting, array);
        }

        #endregion arrayuint

        #region arrayulong

        public static void Write(ISerializerWriter writer, JsonSerializeSetting setting, IEnumerable<ulong?> array)
        {
            _nullableUint64ArrayProvider.Write(writer, setting, array);
        }

        #endregion arrayulong
    }
}