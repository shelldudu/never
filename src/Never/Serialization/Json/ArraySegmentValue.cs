using System;

namespace Never.Serialization.Json
{
    /// <summary>
    /// 内容索引
    /// </summary>
    public struct ArraySegmentValue : IEquatable<ArraySegmentValue>
    {
        #region field and ctor

        /// <summary>
        /// 
        /// </summary>
        private ArraySegment<char> segment;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segment"></param>
        public ArraySegmentValue(ArraySegment<char> segment) : this()
        {
            this.segment = segment;
            this.Length = this.segment.Count;
            this.IsEmpty = this.segment.Count <= 0;
            this.IsNullOrEmpty = this.IsEmpty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="null"></param>
        private ArraySegmentValue(ArraySegment<char> segment, bool @null) : this()
        {
            this.segment = segment;
            this.Length = 0;
            this.IsEmpty = true;
            this.IsNullOrEmpty = true;
        }
        #endregion

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty { get; }

        /// <summary>
        /// 是否为null或者为空
        /// </summary>
        public bool IsNullOrEmpty { get; }

        /// <summary>
        /// 个数
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public char this[int index]
        {
            get
            {
                return this.segment.Array[this.segment.Offset + index];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public char[] ToArray()
        {
            var @char = new char[this.segment.Count];
            Array.Copy(this.segment.Array, this.segment.Offset, @char, 0, this.segment.Count);
            return @char;
        }

        /// <summary>
        /// 得到字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return new string(this.segment.Array, this.segment.Offset, this.segment.Count);
        }

        /// <summary>
        /// 空对象
        /// </summary>
        public static ArraySegmentValue Empty { get; } = new ArraySegmentValue(new ArraySegment<char>(new char[0]));

        /// <summary>
        /// 空对象
        /// </summary>
        public static ArraySegmentValue Null { get; } = new ArraySegmentValue(new ArraySegment<char>(new char[0]), true);

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="a"></param>
        /// <param name="null"></param>
        /// <returns></returns>
        public static bool operator ==(ArraySegmentValue a, object @null)
        {
            return a.segment == null;
        }

        /// <summary>
        /// 是否不为空
        /// </summary>
        /// <param name="a"></param>
        /// <param name="null"></param>
        public static bool operator !=(ArraySegmentValue a, object @null)
        {
            return a.segment != null;
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (this.segment == null)
                return obj == null;

            if (obj is ArraySegmentValue)
            {
                return this.Equals((ArraySegmentValue)obj);
            }
            else if (obj is ArraySegment<char>)
            {
                return this.segment == (ArraySegment<char>)obj;
            }

            return false;
        }

        /// <summary>
        /// 获取hascode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (this.segment == null)
                return base.GetHashCode();

            return this.segment.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ArraySegmentValue other)
        {
            if (this.segment == null)
                return other.segment == null;

            return this.segment.Equals(other.segment);
        }
    }
}
