using System;
using System.Collections.Generic;

namespace Never.Mappers
{
    /// <summary>
    /// 配置
    /// </summary>
    public struct MapperSetting : IEqualityComparer<MapperSetting>, IEquatable<MapperSetting>
    {
        #region prop

        /// <summary>
        /// 配置名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 要忽略的成员
        /// </summary>
        public string[] IgnoredMembers { get; set; }

        /// <summary>
        /// 是复合对象，如果是浅复制，则该值所描述的对象都使用相同副本
        /// </summary>
        [System.Obsolete]
        public string[] ComplexMembers { get; set; }

        /// <summary>
        /// 当两个类型不相同，但是名字相同，是否进行强制转换（在转换过程可能会有异常，溢出等行为）
        /// </summary>
        public bool ForceConvertWhenTypeNotSame { get; set; }

        /// <summary>
        /// 是否浅复制，【建议使用】，因为对于一些对象来说，如果不是浅复制，会不断地创建新的对象，但是这些对象没有特定的接口去读取和写入数据，
        /// 因此只会选成不断地创建新对象（这些新对象并没有什么赋值行为），只是一种浪费
        /// </summary>
        public bool ShallowCopy { get; set; }

        /// <summary>
        /// 总是构造一个新的对象
        /// </summary>
        public bool AlwaysNewTraget { get; set; }

        #endregion prop

        #region ctor

        /// <summary>
        ///
        /// </summary>
        public MapperSetting(string name)
        {
            this.Name = name ?? "Map";
            this.IgnoredMembers = null;
            this.ComplexMembers = null;
            this.ForceConvertWhenTypeNotSame = true;
            this.ShallowCopy = true;
            this.AlwaysNewTraget = false;
        }

        #endregion ctor

        #region equal

        bool IEqualityComparer<MapperSetting>.Equals(MapperSetting x, MapperSetting y)
        {
            return x.Name == y.Name;
        }

        int IEqualityComparer<MapperSetting>.GetHashCode(MapperSetting obj)
        {
            return obj.Name == null ? obj.GetHashCode() : obj.Name.GetHashCode();
        }

        bool IEquatable<MapperSetting>.Equals(MapperSetting other)
        {
            return this.Name == other.Name;
        }

        #endregion equal
    }
}