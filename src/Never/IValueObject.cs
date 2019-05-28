using System;

namespace Never
{
    /// <summary>
    /// 值对象父类
    /// </summary>
    /// <typeparam name="T">值对象</typeparam>
    public interface IValueObject<T> : IEquatable<T>, IComparable<T>
    {
    }
}