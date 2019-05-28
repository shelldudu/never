using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Never.Reflection.TypeConverters
{
    /// <summary>
    /// 字符串转换泛型转制器
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    public sealed class ArrayStringTypeConverter<T> : TypeConverter
    {
        #region field

        /// <summary>
        /// The type converter
        /// </summary>
        private readonly TypeConverter typeConverter = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayStringTypeConverter{T}"/> class.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">找不到T类型的类型转换器 + typeof(T).FullName</exception>
        public ArrayStringTypeConverter()
        {
            this.typeConverter = TypeDescriptor.GetConverter(typeof(T));

            if (this.typeConverter == null)
                throw new InvalidOperationException("找不到T类型的类型转换器" + typeof(T).FullName);
        }

        #endregion ctor

        #region method

        /// <summary>
        /// Splits the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="separator">The separator.</param>
        /// <returns></returns>
        public string[] SplitString(string text, params char[] separator)
        {
            if (string.IsNullOrEmpty(text))
                return new string[0];

            var splits = text.Split(separator);
            for (var i = 0; i < splits.Length; i++)
                splits[i] = splits[i].Trim();

            return splits;
        }

        #endregion method

        #region TypeConverter成员

        /// <summary>
        /// 返回该转换器是否可以使用指定的上下文将给定类型的对象转换为此转换器的类型。
        /// </summary>
        /// <param name="context">一个提供格式上下文的 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />。</param>
        /// <param name="sourceType">一个 <see cref="T:System.Type" />，表示要转换的类型。</param>
        /// <returns>
        /// 如果该转换器能够执行转换，则为 true；否则为 false。
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType != typeof(string))
                return base.CanConvertFrom(context, sourceType);

            var splits = this.SplitString(sourceType.ToString(), ',');
            return splits.Length > 0;
        }

        /// <summary>
        /// 返回此转换器是否可以使用指定的上下文将该对象转换为指定的类型。
        /// </summary>
        /// <param name="context">一个提供格式上下文的 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />。</param>
        /// <param name="destinationType">一个 <see cref="T:System.Type" />，表示要转换到的类型。</param>
        /// <returns>
        /// 如果该转换器能够执行转换，则为 true；否则为 false。
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的对象转换为此转换器的类型。
        /// </summary>
        /// <param name="context">一个提供格式上下文的 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />。</param>
        /// <param name="culture">用作当前区域性的 <see cref="T:System.Globalization.CultureInfo" />。</param>
        /// <param name="value">要转换的 <see cref="T:System.Object" />。</param>
        /// <returns>
        /// 表示转换的 value 的 <see cref="T:System.Object" />。
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var newValue = value as string;

            if (string.IsNullOrEmpty(newValue))
                return base.ConvertFrom(context, culture, value);

            var items = this.SplitString(newValue, ',');
            var result = new List<T>(items.Length);
            foreach (var item in items)
            {
                object convert = typeConverter.ConvertFromInvariantString(item);
                if (item != null)
                    result.Add((T)convert);
            }

            return result;
        }

        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的值对象转换为指定的类型。
        /// </summary>
        /// <param name="context">一个提供格式上下文的 <see cref="T:System.ComponentModel.ITypeDescriptorContext" />。</param>
        /// <param name="culture"><see cref="T:System.Globalization.CultureInfo" />。如果传递 null，则采用当前区域性。</param>
        /// <param name="value">要转换的 <see cref="T:System.Object" />。</param>
        /// <param name="destinationType"><paramref name="value" /> 参数要转换到的 <see cref="T:System.Type" />。</param>
        /// <returns>
        /// 表示转换的 value 的 <see cref="T:System.Object" />。
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!destinationType.IsAssignableFrom(typeof(string)))
                return base.ConvertTo(context, culture, value, destinationType);

            string result = string.Empty;
            var valueList = (IEnumerable<T>)value;

            if (valueList == null)
                return result;

            foreach (var item in valueList)
            {
                result = string.Concat(result, Convert.ToString(item, CultureInfo.InvariantCulture), ",");
            }

            return result.TrimEnd(',');
        }

        #endregion TypeConverter成员
    }
}