using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Never.Reflection;

namespace Never.Memcached
{
    /// <summary>
    /// 压缩 + 序列化
    /// </summary>
    public abstract class DefaultCompressProtocol : ICompressProtocol
    {
        #region field and ctor

        /// <summary>
        /// 
        /// </summary>
        protected static readonly Dictionary<Type, TargetTypeFlag> TargetSorted = null;

        /// <summary>
        /// 
        /// </summary>
        static DefaultCompressProtocol()
        {
            TargetSorted = new Dictionary<Type, TargetTypeFlag>(33);
            var @enumType = typeof(TargetTypeFlag);
            foreach (var name in Enum.GetNames(typeof(TargetTypeFlag)))
            {
                Enum.TryParse(name, out TargetTypeFlag @enum);
                var attribute = @enumType.GetField(name).GetAttribute<TargetTypeFlagAttribute>();
                if (attribute == null)
                    throw new ArgumentNullException($"enum type {name} has no TargetTypeFlagAttribute");

                TargetSorted[attribute.Type] = @enum;
            }
        }

        #endregion

        #region targetType

        /// <summary>
        /// 是否为基元类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected bool IsTargetType(Type type, out TargetTypeFlag @enum)
        {
            @enum = 0;
            var targetType = type;
            if (TypeHelper.IsPrimitiveType(targetType))
            {
                @enum = TargetSorted[targetType];
                return true;
            }

            if (targetType == typeof(DateTime) || targetType == typeof(TimeSpan) || targetType == typeof(decimal))
            {
                @enum = TargetSorted[targetType];
                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否为基元类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected bool IsNullableTargetType(Type type, out TargetTypeFlag @enum)
        {
            @enum = 0;
            var targetType = Nullable.GetUnderlyingType(type);
            if (targetType != null)
            {
                return this.IsTargetType(targetType, out @enum);
            }

            return false;
        }

        /// <summary>
        /// 是否为基元类型
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected bool IsTargetType(byte flag, out TargetTypeFlag @enum)
        {
            @enum = 0;
            if (!Enum.IsDefined(typeof(TargetTypeFlag), flag))
            {
                return false;
            }

            @enum = (TargetTypeFlag)flag;
            return true;
        }

        /// <summary>
        /// 一些基元类型压缩
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>

        public bool TryCompressTargetTypeFlag<T>(T @object, out byte[] value, out byte flag)
        {
            var targetType = typeof(T);
            value = null;
            flag = 0;

            if (this.IsTargetType(targetType, out TargetTypeFlag @enum))
            {
                return this.TryCompressTargetTypeFlag(@object, @enum, out value, out flag);
            }

            if (this.IsNullableTargetType(targetType, out @enum))
            {
                return this.TryCompressNullableTargetTypeFlag(@object, @enum, out value, out flag);
            }

            return false;
        }

        /// <summary>
        /// 一些基元类型压缩
        /// </summary>
        /// <param name="object"></param>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool TryCompressTargetTypeFlag(object @object, out byte[] value, out byte flag)
        {
            var targetType = @object.GetType();
            value = null;
            flag = 0;

            if (this.IsTargetType(targetType, out TargetTypeFlag @enum))
            {
                return this.TryCompressTargetTypeFlag(@object, @enum, out value, out flag);
            }

            if (this.IsNullableTargetType(targetType, out @enum))
            {
                return this.TryCompressNullableTargetTypeFlag(@object, @enum, out value, out flag);
            }

            return false;
        }

        /// <summary>
        /// 一些基元类型压缩
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected bool TryCompressTargetTypeFlag<T>(T @object, TargetTypeFlag enumFlag, out byte[] value, out byte flag)
        {
            value = null;
            flag = (byte)enumFlag;

            switch (enumFlag)
            {
                case TargetTypeFlag.@char:
                    {
                        char convert = (dynamic)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@bool:
                    {
                        bool convert = (dynamic)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }

                case TargetTypeFlag.@byte:
                    {
                        byte convert = (dynamic)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@short:
                    {
                        short convert = (dynamic)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@ushort:
                    {
                        ushort convert = (dynamic)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@int:
                    {
                        int convert = (dynamic)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@uint:
                    {
                        uint convert = (dynamic)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@long:
                    {
                        long convert = (dynamic)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@ulong:
                    {
                        ulong convert = (dynamic)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@float:
                    {
                        float convert = (dynamic)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@double:
                    {
                        double convert = (dynamic)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@decimal:
                    {
                        decimal convert = (dynamic)@object;
                        value = BitConverter.GetBytes((double)convert);
                        return true;
                    }
                case TargetTypeFlag.@timespan:
                    {
                        TimeSpan convert = (dynamic)@object;
                        value = BitConverter.GetBytes(convert.TotalMilliseconds);
                        return true;
                    }
                case TargetTypeFlag.@datetime:
                    {
                        DateTime convert = (dynamic)@object;
                        value = BitConverter.GetBytes(convert.Ticks);
                        return true;
                    }
            }

            return false;
        }

        /// <summary>
        /// 一些基元类型压缩
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected bool TryCompressNullableTargetTypeFlag<T>(T @object, TargetTypeFlag enumFlag, out byte[] value, out byte flag)
        {
            value = null;
            flag = (byte)enumFlag;

            switch (enumFlag)
            {
                case TargetTypeFlag.@char:
                    {
                        char? convert = (dynamic)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@bool:
                    {
                        bool? convert = (dynamic)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }

                case TargetTypeFlag.@byte:
                    {
                        byte? convert = (dynamic)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@short:
                    {
                        short? convert = (dynamic)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@ushort:
                    {
                        ushort? convert = (dynamic)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@int:
                    {
                        int? convert = (dynamic)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@uint:
                    {
                        uint? convert = (dynamic)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@long:
                    {
                        long? convert = (dynamic)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@ulong:
                    {
                        ulong? convert = (dynamic)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@float:
                    {
                        float? convert = (dynamic)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@double:
                    {
                        double? convert = (dynamic)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@decimal:
                    {
                        decimal? convert = (dynamic)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes((double)convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@timespan:
                    {
                        TimeSpan? convert = (dynamic)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value.TotalMilliseconds);
                        return true;
                    }
                case TargetTypeFlag.@datetime:
                    {
                        DateTime? convert = (dynamic)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value.Ticks);
                        return true;
                    }
            }

            return false;
        }

        /// <summary>
        /// 一些基元类型压缩
        /// </summary>
        /// <param name="object"></param>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected bool TryCompressTargetTypeFlag(object @object, TargetTypeFlag enumFlag, out byte[] value, out byte flag)
        {
            value = null;
            flag = (byte)enumFlag;

            switch (enumFlag)
            {
                case TargetTypeFlag.@char:
                    {
                        char convert = (char)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@bool:
                    {
                        bool convert = (bool)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }

                case TargetTypeFlag.@byte:
                    {
                        byte convert = (byte)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@short:
                    {
                        short convert = (short)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@ushort:
                    {
                        ushort convert = (ushort)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@int:
                    {
                        int convert = (int)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@uint:
                    {
                        uint convert = (uint)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@long:
                    {
                        long convert = (long)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@ulong:
                    {
                        ulong convert = (ulong)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@float:
                    {
                        float convert = (float)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@double:
                    {
                        double convert = (double)@object;
                        value = BitConverter.GetBytes(convert);
                        return true;
                    }
                case TargetTypeFlag.@decimal:
                    {
                        decimal convert = (decimal)@object;
                        value = BitConverter.GetBytes((double)convert);
                        return true;
                    }
                case TargetTypeFlag.@timespan:
                    {
                        TimeSpan convert = (TimeSpan)@object;
                        value = BitConverter.GetBytes(convert.TotalMilliseconds);
                        return true;
                    }
                case TargetTypeFlag.@datetime:
                    {
                        DateTime convert = (DateTime)@object;
                        value = BitConverter.GetBytes(convert.Ticks);
                        return true;
                    }
            }

            return false;
        }

        /// <summary>
        /// 一些基元类型压缩
        /// </summary>
        /// <param name="object"></param>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected bool TryCompressNullableTargetTypeFlag(object @object, TargetTypeFlag enumFlag, out byte[] value, out byte flag)
        {
            value = null;
            flag = (byte)enumFlag;

            switch (enumFlag)
            {
                case TargetTypeFlag.@char:
                    {
                        char? convert = (char?)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@bool:
                    {
                        bool? convert = (bool?)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }

                case TargetTypeFlag.@byte:
                    {
                        byte? convert = (byte?)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@short:
                    {
                        short? convert = (short?)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@ushort:
                    {
                        ushort? convert = (ushort?)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@int:
                    {
                        int? convert = (int?)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@uint:
                    {
                        uint? convert = (uint?)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@long:
                    {
                        long? convert = (long?)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@ulong:
                    {
                        ulong? convert = (ulong?)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@float:
                    {
                        float? convert = (float?)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@double:
                    {
                        double? convert = (double?)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@decimal:
                    {
                        decimal? convert = (decimal?)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes((double)convert.Value);
                        return true;
                    }
                case TargetTypeFlag.@timespan:
                    {
                        TimeSpan? convert = (TimeSpan?)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value.TotalMilliseconds);
                        return true;
                    }
                case TargetTypeFlag.@datetime:
                    {
                        DateTime? convert = (DateTime?)@object;
                        if (convert.HasValue == false)
                            return false;

                        value = BitConverter.GetBytes(convert.Value.Ticks);
                        return true;
                    }
            }

            return false;
        }

        /// <summary>
        /// 一些基元类型解压缩
        /// </summary>
        /// <param name="byte"></param>
        /// <param name="flag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryDecompressTargetTypeFlag(byte[] @byte, byte flag, out object value)
        {
            value = null;
            if (Enum.IsDefined(typeof(TargetTypeFlag), flag))
            {
                return this.TryDecompressTargetTypeFlag(@byte, flag, (TargetTypeFlag)flag, out value);
            }

            return false;
        }

        /// <summary>
        /// 一些基元类型解压缩
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="byte"></param>
        /// <param name="flag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryDecompressTargetTypeFlag<T>(byte[] @byte, byte flag, out T value)
        {
            value = default(T);
            if (Enum.IsDefined(typeof(TargetTypeFlag), flag))
            {
                return this.TryDecompressTargetTypeFlag(@byte, flag, (TargetTypeFlag)flag, out value);
            }

            return false;
        }

        /// <summary>
        /// 一些基元类型解压缩
        /// </summary>
        /// <param name="byte"></param>
        /// <param name="flag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool TryDecompressTargetTypeFlag(byte[] @byte, byte flag, TargetTypeFlag enumFlag, out object value)
        {
            value = null;

            try
            {
                switch (enumFlag)
                {
                    case TargetTypeFlag.@char:
                        {
                            value = BitConverter.ToChar(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@bool:
                        {
                            value = BitConverter.ToBoolean(@byte, 0);
                            return true;
                        }

                    case TargetTypeFlag.@byte:
                        {
                            value = (byte)BitConverter.ToInt16(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@short:
                        {
                            value = BitConverter.ToInt16(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@ushort:
                        {
                            value = BitConverter.ToUInt16(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@int:
                        {
                            value = BitConverter.ToInt32(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@uint:
                        {
                            value = BitConverter.ToUInt32(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@long:
                        {
                            value = BitConverter.ToInt64(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@ulong:
                        {
                            value = BitConverter.ToUInt64(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@float:
                        {
                            value = BitConverter.ToSingle(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@double:
                        {
                            value = BitConverter.ToDouble(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@decimal:
                        {
                            value = (decimal)BitConverter.ToDouble(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@timespan:
                        {
                            value = TimeSpan.FromMilliseconds(BitConverter.ToDouble(@byte, 0));
                            return true;
                        }
                    case TargetTypeFlag.@datetime:
                        {
                            value = new DateTime(BitConverter.ToInt64(@byte, 0));
                            return true;
                        }
                }
            }
            catch
            {

            }

            return false;
        }

        /// <summary>
        /// 一些基元类型解压缩
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="byte"></param>
        /// <param name="flag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool TryDecompressTargetTypeFlag<T>(byte[] @byte, byte flag, TargetTypeFlag enumFlag, out T value)
        {
            value = default(T);

            try
            {
                switch (enumFlag)
                {
                    case TargetTypeFlag.@char:
                        {
                            value = (T)(dynamic)BitConverter.ToChar(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@bool:
                        {
                            value = (T)(dynamic)BitConverter.ToBoolean(@byte, 0);
                            return true;
                        }

                    case TargetTypeFlag.@byte:
                        {
                            value = (T)(dynamic)((byte)BitConverter.ToInt16(@byte, 0));
                            return true;
                        }
                    case TargetTypeFlag.@short:
                        {
                            value = (T)(dynamic)BitConverter.ToInt16(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@ushort:
                        {
                            value = (T)(dynamic)BitConverter.ToUInt16(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@int:
                        {
                            value = (T)(dynamic)BitConverter.ToInt32(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@uint:
                        {
                            value = (T)(dynamic)BitConverter.ToUInt32(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@long:
                        {
                            value = (T)(dynamic)BitConverter.ToInt64(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@ulong:
                        {
                            value = (T)(dynamic)BitConverter.ToUInt64(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@float:
                        {
                            value = (T)(dynamic)BitConverter.ToSingle(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@double:
                        {
                            value = (T)(dynamic)BitConverter.ToDouble(@byte, 0);
                            return true;
                        }
                    case TargetTypeFlag.@decimal:
                        {
                            value = (T)(dynamic)((decimal)BitConverter.ToDouble(@byte, 0));
                            return true;
                        }
                    case TargetTypeFlag.@timespan:
                        {
                            value = (T)(dynamic)TimeSpan.FromMilliseconds(BitConverter.ToDouble(@byte, 0));
                            return true;
                        }
                    case TargetTypeFlag.@datetime:
                        {
                            value = (T)(dynamic)new DateTime(BitConverter.ToInt64(@byte, 0));
                            return true;
                        }
                }
            }
            catch
            {

            }

            return false;
        }

        #endregion

        #region dispose

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {

        }

        #endregion

        #region ICompressProtocol

        /// <summary>
        /// 压缩
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="object">对象</param>
        /// <param name="flag">flag，由于<see cref="TargetTypeFlag"/>使用了0-13的数值，但又不确定是否被子类调用，因此要么确定不使用<see cref="TargetTypeFlag"/>要么避开0-33</param>
        /// <returns></returns>
        public abstract byte[] Compress<T>(T @object, out byte flag);

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="object">对象</param>
        /// <param name="flag">flag，由于<see cref="TargetTypeFlag"/>使用了0-13的数值，但又不确定是否被子类调用，因此要么确定不使用<see cref="TargetTypeFlag"/>要么避开0-33</param>
        /// <returns></returns>
        public abstract byte[] Compress(object @object, out byte flag);

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="byte">源数据</param>
        /// <param name="flag">flag，由于<see cref="TargetTypeFlag"/>使用了0-13的数值，但又不确定是否被子类调用，因此要么确定不使用<see cref="TargetTypeFlag"/>要么避开0-33</param>
        /// <param name="object"></param>
        /// <returns></returns>
        public abstract bool TryDecompress(byte[] @byte, byte flag, out object @object);

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="byte">源数据</param>
        /// <param name="flag">flag，由于<see cref="TargetTypeFlag"/>使用了0-13的数值，但又不确定是否被子类调用，因此要么确定不使用<see cref="TargetTypeFlag"/>要么避开0-33</param>
        /// <param name="object"></param>
        /// <returns></returns>
        public abstract bool TryDecompress<T>(byte[] @byte, byte flag, out T @object);

        #endregion
    }
}
