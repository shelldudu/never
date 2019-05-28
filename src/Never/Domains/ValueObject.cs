using System;
using System.Collections.Generic;

namespace Never.Domains
{
    /// <summary>
    /// 值对象
    /// </summary>
    /// <typeparam name="T">值对象</typeparam>
    public class ValueObject<T> : IValueObject<T>, IEquatable<T>, IComparable<T>, IComparable
    {
        #region field

        /// <summary>
        /// 当前hasCode
        /// </summary>
        private int? hashCode = null;

        #endregion field

        #region 重载修饰符

        /// <summary>
        /// Determines whether the specified left is equal.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool IsEqual(ValueObject<T> left, ValueObject<T> right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
                return false;

            return ReferenceEquals(left, null) || left.Equals(right);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
        {
            return IsEqual(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(ValueObject<T> left, ValueObject<T> right)
        {
            return !IsEqual(left, right);
        }

        #endregion 重载修饰符

        #region 抽象方法

        /// <summary>
        /// 获取内部所有参与计算hashcode的属性，字段等对象
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<object> GetAtomicValues()
        {
            return new object[] { string.Empty };
        }

        #endregion 抽象方法

        #region IEquatable成员

        /// <summary>
        /// 指示当前对象是否等于同一类型的另一个对象。
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public virtual bool Equals(T other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return this.GetHashCode() == other.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (obj.GetType() != this.GetType())
                return false;

            var other = obj as ValueObject<T>;
            if (ReferenceEquals(other, null))
                return false;

            return this.GetHashCode() == other.GetHashCode();
        }

        #endregion IEquatable成员

        #region IComparable成员

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public virtual int CompareTo(T other)
        {
            if (ReferenceEquals(other, null))
                return 1;

            if (this.Equals(other))
                return 0;

            return this.GetHashCode() < other.GetHashCode() ? -1 : 1;
        }

        /// <summary>
        /// 将当前实例与同一类型的另一个对象进行比较，并返回一个整数，该整数指示当前实例在排序顺序中的位置是位于另一个对象之前、之后还是与其位置相同。
        /// </summary>
        /// <param name="obj">与此实例进行比较的对象。</param>
        /// <returns>
        /// 一个值，指示要比较的对象的相对顺序。返回值的含义如下：值含义小于零此实例小于 <paramref name="obj" />。零此实例等于 <paramref name="obj" />。大于零此实例大于 <paramref name="obj" />。
        /// </returns>
        public virtual int CompareTo(object obj)
        {
            var other = (T)obj;
            if (ReferenceEquals(other, null))
                return 1;

            return this.CompareTo(other);
        }

        #endregion IComparable成员

        #region object成员

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            if (this.hashCode.HasValue)
                return this.hashCode.Value;

            var list = new List<int>(20);
            foreach (var x in this.GetAtomicValues())
            {
                list.Add(!ReferenceEquals(x, null) ? x.GetHashCode() : 0);
            }

            this.hashCode = System.Linq.Enumerable.Aggregate(list, (x, y) => x ^ y);
            return this.hashCode.Value;
        }

        #endregion object成员
    }
}