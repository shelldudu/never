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
            dao.ToEasyLinqDao().Update(new User() { }).乐观锁(ta => ta.Balance, 0).Change();
        }

        [Xunit.Fact]
        public void TestId_11()
        {
            var list = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().ToEasyXmlDao(new { Id = 1, UserName = 666, UserId = 666 }).QueryForEnumerable<User>("qryUser");
            var list2 = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build().ToEasyXmlDao(new { Id = 1, UserId = 2, UserName = "".ToNullableParameter() }).QueryForEnumerable<User>("qryUser");
        }
    }
}