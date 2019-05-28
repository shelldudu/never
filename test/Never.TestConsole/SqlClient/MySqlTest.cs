using Never.EasySql;
using Never.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Never.TestConsole.SqlClient
{
    public class MySqlTest
    {
        [Xunit.Fact]
        public void TestGetInt()
        {
            var type = typeof(int?);
            using (var sql = Never.SqlClient.SqlExecuterFactory.MySql("server=127.0.0.1;port=3306; initial catalog = b2c_message;uid = sa;pwd=gg123456;SslMode=none;"))
            {
                var ints = sql.QueryForObject<TestDB>("select 1 as Id, 1 as UserId,'[1,2,3]' as UserName from asset where UserName = @UserName limit 1;", new TestDB { UserName = new[] { '1', '2', '3' } });
                //var str = sql.QueryForList<string>("select VideoDescn from article", null);
            };
        }

        public T ReadInt<T>(IDataRecord rd)
        {
            var ordinal = rd.GetOrdinal("Hits");
            ordinal = rd.GetOrdinal("Id");

            return (T)(object)rd.GetInt32(0);
        }

        public int? ReadIntNull(IDataRecord rd)
        {
            if (rd.IsDBNull(0))
                return 0;

            return rd.GetInt32(0);
        }

        public Nullable<T> ReadNull<T>(IDataRecord reader) where T : struct, IConvertible
        {
            if (reader.IsDBNull(0))
                return default(Nullable<T>);

            return (Nullable<T>)(object)reader.GetInt32(0);
        }

        public class TestDB
        {
            public long? UserId;

            [TypeHandler(typeof(UserNameTypeHandler))]
            public char[] UserName { get; set; }

            // public long RegId { get; set; }

            //  public string LoginIP { get; set; }

            //  public DateTime LoginDate { get; set; }

            public TestEnum? ResultType { get; set; }
        }

        public class UserNameTypeHandler : IReadingFromDataRecordToValueTypeHandler<char[]>, ICastingValueToParameterTypeHandler<string>
        {
            /// <summary>
            ///
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public string ToParameter(object value)
            {
                return Never.Serialization.EasyJsonSerializer.SerializeObject(value);
            }

            /// <summary>
            /// 获取结果
            /// </summary>
            /// <param name="dataRecord">读取器</param>
            /// <param name="ordinal">column的位置，如果未-1表示没有找到这个值</param>
            /// <param name="columnName">行名字</param>
            /// <returns></returns>
            public char[] ToValue(IDataRecord dataRecord, int ordinal, string columnName)
            {
                var value = dataRecord.GetString(ordinal);
                return value == null ? new char[0] : value.ToCharArray();
            }
        }

        public enum TestEnum
        {
            A = 1,
        }

        public TestDB ReadFromDb(IDataRecord reader)
        {
            var db = new TestDB();
            var local = reader.GetOrdinal("Id");
            //if (local > 0)
            //    db.Id = reader.GetInt32(local);

            return db;
        }

        [Xunit.Fact]
        public void InsertSTring()
        {
            var ist = typeof(UserNameTypeHandler).IsAssignableFromType(typeof(IReadingFromDataRecordToValueTypeHandler<>));
            ist = typeof(UserNameTypeHandler).IsAssignableFromType(typeof(ICastingValueToParameterTypeHandler<>));
            var interfaces = typeof(UserNameTypeHandler).MatchTargetType(typeof(IReadingFromDataRecordToValueTypeHandler<>)).ToArray();
            //var eles = typeof(UserNameTypeHandler).MatchElementType(typeof(IReadingValueFromDataRecordTypeHandler<>));

            if (Regex.IsMatch(@"insert into\s+", "insert into sfs", RegexOptions.IgnoreCase))
                Console.WriteLine("true");
            else
                Console.WriteLine("false");
        }

        [Xunit.Fact]
        public void TestSql()
        {
            var count = new SqlServerBuilder().Build().ToEasyTextDao(new { }).QueryForObject<int>("select count(0) from [user];");
            var sql1 = "?value";
            var sql2 = "??value";

            var regx = @"(?<![?!@<])[?!@<](?![?!@<])";
            Console.WriteLine(Regex.IsMatch(sql1, regx));
            Console.WriteLine(Regex.IsMatch(sql2, regx));
        }

        public void TestH()
        {
            Console.WriteLine(ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().GetHashCode());
            Task.Run(() =>
            {
                var st = string.Concat("1::::::", System.Threading.Thread.CurrentThread.ManagedThreadId, "::", ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().GetHashCode().ToString());
                Console.WriteLine(st);
            }).ContinueWith((t) =>
            {
                var st = string.Concat("12::::::", System.Threading.Thread.CurrentThread.ManagedThreadId, "::", ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().GetHashCode().ToString());
                Console.WriteLine(st);
            });

            Task.Run(() =>
            {
                var st = string.Concat("2::::::", System.Threading.Thread.CurrentThread.ManagedThreadId, "::", ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().GetHashCode().ToString());
                Console.WriteLine(st);
            }).ContinueWith((t) =>
            {
                var st = string.Concat("22::::::", System.Threading.Thread.CurrentThread.ManagedThreadId, "::", ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().GetHashCode().ToString());
                Console.WriteLine(st);
            });

            Task.Run(() =>
            {
                var st = string.Concat("3::::::", System.Threading.Thread.CurrentThread.ManagedThreadId, "::", ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().GetHashCode().ToString());
                Console.WriteLine(st);
            }).ContinueWith((t) =>
            {
                var st = string.Concat("32::::::", System.Threading.Thread.CurrentThread.ManagedThreadId, "::", ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().GetHashCode().ToString());
                Console.WriteLine(st);
            });

            Console.ReadLine();
        }
    }
}