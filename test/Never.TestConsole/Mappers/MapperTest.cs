using Never.Mappers;
using Never.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Never.TestConsole.Mappers
{
    /// <summary>
    /// 扩展基本实体
    /// </summary>
    [Serializable]
    public class TheBasicExtModel
    {
        /// <summary>
        /// 标识Id
        /// </summary>
        [DisplayName("标识Id")]
        public virtual int Id { get; set; }

        /// <summary>
        /// 版本信息
        /// </summary>
        [DisplayName("版本信息")]
        public virtual int Version { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间")]
        public virtual DateTime CreateDate { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        [DisplayName("编辑时间")]
        public virtual DateTime EditDate { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [DisplayName("创建者")]
        public virtual string Creator { get; set; }

        /// <summary>
        /// 编辑者
        /// </summary>
        [DisplayName("编辑者")]
        public virtual string Editor { get; set; }

        /// <summary>
        /// 前台是否可见的
        /// </summary>
        [DisplayName("可见状态")]
        public virtual bool Visible { get; set; }

        /// <summary>
        /// 后台是否禁用的
        /// </summary>
        [DisplayName("禁用状态")]
        public virtual bool Disabled { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheBasicExtModel"/> class.
        /// </summary>
        public TheBasicExtModel()
        {
            this.EditDate = this.CreateDate = DateTime.Now;
        }
    }

    /// <summary>
    /// 聚合扩展基本实体
    /// </summary>
    [Serializable]
    public class TheBasicExtModel<T> : TheBasicExtModel
    {
        /// <summary>
        /// 聚合Id
        /// </summary>
        [DisplayName("聚合Id")]
        public virtual T AggregateId { get; set; }
    }

    /// <summary>
    /// 轨迹类型
    /// </summary>
    public enum BookingTrackType
    {
        /// <summary>
        /// booking
        /// </summary>
        Booking = 0,

        /// <summary>
        /// buy
        /// </summary>
        Buying = 1,
    }

    /// <summary>
    /// 自动映射配置
    /// </summary>
    public class MapperTest
    {
        private readonly MapperSetting setting = new MapperSetting() { };

        [Xunit.Fact]
        public void TestTargetNullableSourceNotNullable()
        {
            var from1 = new FromNotNullProp() { Id = 2, Name = "2005", };
            var to1 = EasyMapper.Map<FromNotNullProp, ToNullProp>(from1);
            var to10 = new ToNullProp() { InM = new InClassMapper() };
            var to11 = EasyMapper.Map<FromNotNullProp, ToNullProp>(from1, to10);

            var to2 = EasyMapper.Map<FromNotNullProp, ToNullField>(from1);

            var from2 = new FromNotNullField() { Id = 5, Name = "2005" };
            to1 = EasyMapper.Map<FromNotNullField, ToNullProp>(from2);
            to2 = EasyMapper.Map<FromNotNullField, ToNullField>(from2);
        }

        [Xunit.Fact]
        public void TestTargetNullableSourceNullable()
        {
            var from1 = new FromNullProp() { Id = 2, Name = "2005" };
            var to1 = EasyMapper.Map<FromNullProp, ToNullProp>(from1);
            var to2 = EasyMapper.Map<FromNullProp, ToNullField>(from1);

            var from2 = new FromNullField() { Id = 5, Name = "2005" };
            to1 = EasyMapper.Map<FromNullField, ToNullProp>(from2);
            to2 = EasyMapper.Map<FromNullField, ToNullField>(from2);
        }

        [Xunit.Fact]
        public void TestTargetNotNullableSourceNotNullable()
        {
            var from1 = new FromNotNullProp()
            {
                Id = 2,
                Name = "2005",
                InM = new InClassMapper()
                {
                    Owner = "紫妈",
                    Map = new StructMapper() { Owner = "dld" }
                }
            };
            var to1 = EasyMapper.Map<FromNotNullProp, ToNotNullProp>(from1);
            var to10 = new ToNotNullProp();
            var to11 = EasyMapper.Map<FromNotNullProp, ToNotNullProp>(from1, to10);

            var to2 = EasyMapper.Map<FromNotNullProp, ToNotNullField>(from1);

            var from2 = new FromNotNullField() { Id = 5, Name = "2005" };
            to1 = EasyMapper.Map<FromNotNullField, ToNotNullProp>(from2);
            to2 = EasyMapper.Map<FromNotNullField, ToNotNullField>(from2);
        }

        [Xunit.Fact]
        public void TestTargetNotNullableSourceNullable()
        {
            var from1 = new FromNullProp() { Id = 2, Name = "2005" };
            var to1 = EasyMapper.Map<FromNullProp, ToNotNullProp>(from1);
            var to2 = EasyMapper.Map<FromNullProp, ToNotNullField>(from1);

            var from2 = new FromNullField() { Id = 5, Name = "2005" };
            to1 = EasyMapper.Map<FromNullField, ToNotNullProp>(from2);
            to2 = EasyMapper.Map<FromNullField, ToNotNullField>(from2);
        }

        [Xunit.Fact]
        public void TestConvert()
        {
            var i = 3;
            Console.WriteLine(decimal.Parse(i.ToString()));
        }

        [Xunit.Fact]
        public void TestStruct()
        {
            var from = new StructMapper()
            {
                Owner = "lxl"
            };

            var to = EasyMapper.Map<StructMapper, ClassMapper>(from);

            var a = new AInfoAmount() { Amount = 200 };
            var b = EasyMapper.Map(a, new BInfoAmount());
        }

        [Xunit.Fact]
        public void TestLoadStruct()
        {
            var im = new InClassMapper();
            im.Owner = "abc";
            //var from1 = new FromNotNullProp() { Id = 2, Name = "2005", InM = new InClassMapper() { Owner = "紫妈" } };
            var to = new ToNotNullProp()
            {
            };

            to.InM = im;
        }

        public ClassMapper MappToTest(ClassMapper from)
        {
            if (from == null)
                return null;

            return new ClassMapper();
        }

        [Xunit.FactAttribute]
        public void TestEnumParse()
        {
            var from = new TestFromMappeEnum() { Value = MapperEnum.A | MapperEnum.B };
            var to = EasyMapper.Map(from, new TestToMappeEnum());
        }

        [Xunit.Fact]
        public void TestMapArray()
        {
            var @base = new FromMapArray
            {
                A = 236m,
                Array = new[] { 1, 2, 3 },
                Collection = new Dictionary<int, int>() { { 1, 1 }, { 2, 2 } }
            };

            var mapper = (IMapper)new EasyMapper(new MapperSetting() { ForceConvertWhenTypeNotSame = true });
            var to = mapper.Map<FromMapArray, MapArrayTarget>(@base);
            //var to = mapper.Map(@base, new MapArrayTarget());
            //var aaa = to.Array.ToArray();
        }

        private class FromMapArray
        {
            public decimal A { get; set; }

            public IEnumerable<int> Array { get; set; }

            public Dictionary<int, int> Collection { get; set; }
        }

        private struct MapArrayTarget
        {
            public string A { get; set; }

            public ToNullProp[] Array { get; set; }

            public ICollection<KeyValuePair<int, int>> Collection { get; set; }
        }
    }

    public class TestFromMappeEnum
    {
        public MapperEnum? Value { get; set; }
    }

    public class TestToMappeEnum
    {
        public MapperEnum? Value { get; set; }
    }

    public enum MapperEnum
    {
        A = 1,
        B = 2,
        C = 4
    }

    public class ToNullProp
    {
        public long? Id { get; set; }
        public int Name { get; set; }
        public InClassMapper InM { get; set; }
    }

    public class ToNullField
    {
        public long? Id;
        public int Name;
        public InClassMapper InM;
    }

    public class ToNotNullProp
    {
        public long Id { get; set; }
        public int Name { get; set; }
        public InClassMapper InM { get; set; }
    }

    public class ToNotNullField
    {
        public long Id;
        public int Name;
        public InClassMapper InM;
    }

    public class FromNullProp
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public InClassMapper InM { get; set; }
    }

    public class FromNullField
    {
        public int? Id;
        public string Name;
        public InClassMapper InM;
    }

    public class FromNotNullProp
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public InClassMapper InM { get; set; }
    }

    public class FromNotNullField
    {
        public int Id;
        public string Name;
        public InClassMapper InM;
    }

    public class InClassMapper
    {
        public string Owner { get; set; }

        public StructMapper Map { get; set; }

        public InClassMapper()
        {
            this.Owner = "紫妈";
        }
    }

    public struct StructMapper
    {
        public string Owner { get; set; }
    }

    public class ClassMapper
    {
        public string Owner { get; set; }
    }

    public class BaseAmount
    {
        public decimal Amount { get; set; }
    }

    public class AInfoAmount : BaseAmount
    {
    }

    public class BInfoAmount : BaseAmount
    {
    }
}