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
            var list = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().ToEasyTextDao(new { Id = 1, UserId = DBNull.Value, IdArray = new[] { 1, 2, 3, 4 } }).QueryForEnumerable<User>("select * from [user] where Id = $Id$and Id in ($IdArray)");
            list = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().ToEasyXmlDao(new { Id = 1, UserId = 1, IdArray = new[] { 1, 2, 3, 4 } }).QueryForEnumerable<User>("qryUser");
            list = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().ToEasyTextDao(new { Id = 1, UserId = DBNull.Value, IdArray = new[] { 1, 2, 3, 4 } }).QueryForEnumerable<User>((x, s) =>
            {
                s.Append("select * from user ");
                if (x.Id > 2)
                    s.Append(" where Id = " + x.Id);

                return;
            });

            var dao = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build();
            var builder1 = dao.ToEasyLinqDao(new { Id = 1 }).Select<SqlServerBuilder>().Where((p, t) => t.EmbeddedSqlMaps.Count() >= p.Id).ToSingle().GetResult();
            var builder2 = dao.ToEasyLinqDao(new { }).Select<SqlServerBuilder, SqlServerBuilder>().LeftJoin((p, t1, t2) => t1.EmbeddedSqlMaps == t2.EmbeddedSqlMaps)
                .Where(null).ToList(1, 5).ToSingle().GetResult();

            var builder3 = dao.ToEasyLinqDao(new { }).Select<SqlServerBuilder, SqlServerBuilder, SqlServerBuilder>().LeftJoin((p, t1, t2, t3) => t1.EmbeddedSqlMaps == t2.EmbeddedSqlMaps && t1.EmbeddedSqlMaps == t3.EmbeddedSqlMaps)
                .Where(null).ToList(1, 5).ToSingle().ToSingle().GetResult();

        }

        [Xunit.Fact]
        public void TestId_11()
        {
            var list = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().ToEasyXmlDao(new { Id = 1, UserName = 666, UserId = 666 }).QueryForEnumerable<User>("qryUser");
            var list2 = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().ToEasyXmlDao(new { Id = 1, UserId = 2, UserName = "".ToNullableParameter() }).QueryForEnumerable<User>("qryUser");
        }
    }
}