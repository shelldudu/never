using Never.Serialization.Json.Deserialize.Enumerators;
using Never.Serialization.Json.MethodProviders;
using System;

namespace Never.Serialization.Json.Deserialize
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

        public static Type TypeParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return TypeMethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion type

        #region bool

        public static bool BoolParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return BooleanMethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion bool

        #region byte

        public static byte ByteParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return ByteMethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion byte

        #region char

        public static char CharParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return CharMethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion char

        #region datetime

        public static DateTime DateTimeParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            IConvertMethodProvider<DateTime> customMethodProvider = CustomSerializationProvider.QueryCustomDatetimeFormat(setting.DateTimeFormat) as IConvertMethodProvider<DateTime>;
            if (customMethodProvider == null)
                return DateTimeMethodProvider.Default.Parse(reader, setting, reader.Read(name), true);

            return customMethodProvider.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion datetime

        #region decimal

        public static decimal DecimalParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return DecimalMethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion decimal

        #region timespan

        public static TimeSpan TimeSpanParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return TimeSpanMethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion timespan

        #region double

        public static double DoubleParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return DoubleMethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion double

        #region float

        public static float FloatParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return FloatMethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion float

        #region guid

        public static Guid GuidParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return GuidMethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion guid

        #region short

        public static short Int16Parse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return Int16MethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion short

        #region int

        public static int Int32Parse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return Int32MethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion int

        #region long

        public static long Int64Parse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return Int64MethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion long

        #region string

        public static string StringParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return StringMethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion string

        #region ushort

        public static ushort UInt16Parse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return UInt16MethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion ushort

        #region uint

        public static uint UInt32Parse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return UInt32MethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion uint

        #region ulong

        public static ulong UInt64Parse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            return UInt64MethodProvider.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion ulong

        #region enum

        public static T EnumParse<T>(IDeserializerReader reader, JsonDeserializeSetting setting, string name) where T : struct, IConvertible
        {
            return EnumMethodProvider<T>.Default.Parse(reader, setting, reader.Read(name), true);
        }

        #endregion enum

        #region exception

        public static T ExceptionParse<T>(IDeserializerReader reader, JsonDeserializeSetting setting, string name) where T : Exception
        {
            return ExceptionMethodProvider<T>.Default.Parse(reader, setting, name);
        }

        #endregion exception

        #region arraybool

        public static bool[] ArrayBoolParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _boolArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arraybool

        #region arraybyte

        public static byte[] ArrayByteParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _byteArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arraybyte

        #region arraychar

        public static char[] ArrayCharParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _charArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arraychar

        #region arraydatetime

        public static DateTime[] ArrayDateTimeParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _dateTimeArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arraydatetime

        #region arraydecimal

        public static decimal[] ArrayDecimalParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _decimalArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arraydecimal

        #region arraydouble

        public static double[] ArrayDoubleParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _doubleArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arraydouble

        #region arrayfloat

        public static float[] ArrayFloatParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _floatArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arrayfloat

        #region arrayguid

        public static Guid[] ArrayGuidParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _guidArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arrayguid

        #region arrayshort

        public static short[] ArrayInt16Parse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _int16ArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arrayshort

        #region arrayint

        public static int[] ArrayInt32Parse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _int32ArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arrayint

        #region arraylong

        public static long[] ArrayInt64Parse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _int64ArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arraylong

        #region arraystring

        public static string[] ArrayStringParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _stringArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arraystring

        #region arrayushort

        public static ushort[] ArrayUInt16Parse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _uint16ArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arrayushort

        #region array timespan

        public static TimeSpan[] ArrayTimeSpanParse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _timeSpanArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion array timespan

        #region arrayuint

        public static uint[] ArrayUInt32Parse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _uint32ArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arrayuint

        #region arrayulong

        public static ulong[] ArrayUInt64Parse(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel)
        {
            return _uint64ArrayProvider.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arrayulong

        #region arrayenum

        public static T[] ArrayEnumParse<T>(IDeserializerReader reader, JsonDeserializeSetting setting, string name, int arrayLevel) where T : struct, IConvertible
        {
            return EnumEnumerableProvider<T>.Default.Parse(reader, setting, name, arrayLevel);
        }

        #endregion arrayenum
    }
}