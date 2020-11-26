using Never.EasySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test.SqlClient
{
    /// <summary>
    /// sql server test
    /// </summary>
    public class SqlServerSelect
    {
        [Xunit.Fact]
        public void testSelect1()
        {
            var dao = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build();
            //返回单条
            var more = dao.ToEasyLinqDao(new { Id = 1, IdArray = new[] { 22, 23, 24, 25 }.ToNullableParameter(), Name = "ee" })
               .Select<User>()
               .InnerJoin<User>("t2").On((p, t, t2) => t.Id == t2.Id).And((p, t, t2) => t2.Name == "ee")
               //.InnerJoin<User>("t3").On((p, t, t2, t3) => t.Id == t2.Id).And((p, t, t2, t3) => t2.Name == "ee")
               .ToEnumerable()
               .Where((p, t) => t.Id >= p.Id)
               //.AndExists<>
               .And((p, t) => t.Id.In(p.IdArray) && t.Id >= 2 && t.Name.Like("e") && t.Name.LeftLike("e") && t.Name.RightLike("e"))
               .Or((p, t) => t.Id.In(p.IdArray) && t.Id >= 2 || t.Name.Like("e") && t.Name.LeftLike("e") && t.Name.RightLike("e"))
               .OrderBy(t => t.Id)
               .OrderByDescending(t => t.Id)
               //.OrderByDescendingTable1(t => t.Id)
               //.OrderByDescendingTable2(t => t.Id)
               .AddSql(" and [user].id != @Id ")
               .GetResult(0, 10);
            //.GetSqlTagFormat(true);

            string sql = more.ToString();
            System.Console.WriteLine(sql);
        }
    }
}
