using Never.EasySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test
{
    /// <summary>
    ///
    /// </summary>
    public class SelectTest : Program
    {
        [Xunit.Fact]
        public void TestId_1()
        {
            //var list = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().ToEasyTextDao(new { Id = 1, UserId = DBNull.Value, IdArray = new[] { 1, 2, 3, 4 } }).QueryForEnumerable<User>("select * from [user] where Id = $Id$and Id in ($IdArray)");
            //list = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().ToEasyXmlDao(new { Id = 1, UserId = 1, IdArray = new[] { 1, 2, 3, 4 } }).QueryForEnumerable<User>("qryUser");
            //list = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().ToEasyTextDao(new { Id = 1, UserId = DBNull.Value, IdArray = new[] { 1, 2, 3, 4 } }).QueryForEnumerable<User>((x, s) =>
            //{
            //    s.Append("select * from user ");
            //    if (x.Id > 2)
            //        s.Append(" where Id = " + x.Id);

            //    return;
            //});

            int a = 0; int b = 0; var c = a == b;

            var dao = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build();

            //返回单条
            var one = dao.ToEasyLinqDao(new { Id = 1 }).Select<SqlServerBuilder>()
                .Where((p, t) => t.EmbeddedSqlMaps.Count() >= p.Id)
                .ToSingle()
                .GetResult();

            //返回列表
            var array = dao.ToEasyLinqDao(new { Id = 1 }).Select<SqlServerBuilder>().Where(null).ToList(1, 5).GetResult();
            //返回列表，里面join了其他表
            var array2 = dao.ToEasyLinqDao(new { Id = 1 }).Select<SqlServerBuilder>().LeftJoin<SqlServerBuilder>((p, t1, t2) => t1.EmbeddedSqlMaps == t2.EmbeddedSqlMaps)
                .Where(null).ToList(1, 5).GetResult();

            //更新
            var update = dao.ToEasyLinqDao(new SqlServerBuilder()).Update()
                .SetColum(m => m.EmbeddedSqlMaps)
                .SetColumWithFunc(m => m.ConnectionString, "now()")
                .Where(p => p.ConnectionString.Length)
                .NotExists<SqlServerBuilder>((p, t1) => t1.ConnectionString == p.ConnectionString)
                .GetResult();

            //删除
            var delete = dao.ToEasyLinqDao(new SqlServerBuilder()).Delete()
                .Where(p => p.ConnectionString == "abc")
                .AndNotExists<SqlServerBuilder>((p, t1) => t1.ConnectionString == p.ConnectionString)
                .GetResult();

            //推入
            var insert = dao.ToEasyLinqDao(new SqlServerBuilder()).Insert()
                .ValueColum(m => m.EmbeddedSqlMaps)
                .ValueColumFunc(m => m.ConnectionString, "uuid()")
                .LastInsertId()
                .GetResult<int>();
        }

        [Xunit.Fact]
        public void TestId_11()
        {
            var list = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().ToEasyXmlDao(new { Id = 1, UserName = 666, UserId = 666 }).QueryForEnumerable<User>("qryUser");
            var list2 = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().ToEasyXmlDao(new { Id = 1, UserId = 2, UserName = "".ToNullableParameter() }).QueryForEnumerable<User>("qryUser");
        }
    }
}