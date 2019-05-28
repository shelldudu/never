using Never.Deployment;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Never.TestConsole.Serialization
{
    public class TeseDemo
    {
        public char C { get; set; }
        public string Empty { get; set; }
        [System.Runtime.Serialization.DataMember(Name = "tjymulti.all.v01.first_to_now.tm_duration_from_last_loan.days")]
        public int Id { get; set; }
        public Byte Byte { get; set; }
        public short Short { get; set; }
        public long Long { get; set; }
        public ushort UShort { get; set; }
        public ulong ULong { get; set; }
        public uint UId { get; set; }
        public string Name { get; set; }
        public ABC ABC { get; set; }
        public string DDD { get; set; }
        public string DDD2 { get; set; }
        public string M { get; set; }
        public string DDD3 { get; set; }
        public string DDD4 { get; set; }
        public string DDD5 { get; set; }
        public string DDD6 { get; set; }
        public Guid Guid { get; set; }
        public decimal Amount { get; set; }
        public DateTime Time { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }

    public class TeseDemo1 : TeseDemo
    {
        public TwoLevel Object { get; set; }
        public TwoLevel[][] Array { get; set; }
        public ABC[] EnumArray { get; set; }
        public object[][] Enumables { get; set; }
    }

    public class TeseDemo999 //: TeseDemo
    {
        //public TwoLevel Object { get; set; }
        public TwoLevel[][] Array { get; set; }

        //public ABC[] EnumArray { get; set; }
        //public object[][] Enumables { get; set; }
    }

    public class TestDemo2
    {
        public ulong ULong { get; set; }

        public uint UId { get; set; }
        public string Name { get; set; }
        public char C { get; set; }
        public TestDemo3 Demo3 { get; set; }
    }

    public class TestDemo3
    {
        public ulong ULong { get; set; }
        public uint UId { get; set; }
        public string Name { get; set; }
    }

    public struct TeseDemo4
    {
        public TwoLevel Object { get; set; }
    }

    public class TestDemo5
    {
        public string Name { get; set; }
        public Hashtable Table { get; set; }
    }

    public class TestDemo6
    {
        public string Name { get; set; }

        public TestDemo3 Demo3 { get; set; }
    }

    public class TeseDemo7
    {
        public string Name { get; set; }

        [Never.Serialization.Json.DataMember("Demo3")]
        public string Demo35 { get; set; }
    }

    public class TeseDemo8
    {
        public Dictionary<string, string> OrderBy { get; set; }
    }

    public class GuidTestDemo
    {
        public Guid AggregateId { get; set; }
    }

    public class StringTestDemo
    {
        public string AggregateId { get; set; }
    }
}