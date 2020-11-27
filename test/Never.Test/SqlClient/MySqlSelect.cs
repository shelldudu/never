using Never.EasySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test.SqlClient
{
    /// <summary>
    /// mysql test
    /// </summary>
    public class MySqlSelect
    {
        [Xunit.Fact]
        public void testSelect1()
        {
            var dao = ConstructibleDaoBuilder<MySqlBuilder>.Value.Build();
            //返回条数
            var more = dao.ToEasyLinqDao(new { Id = 1, IdArray = new[] { 22, 23, 24, 25 }.ToNullableParameter(), Name = "ee" })
               .Select<User>()
               .InnerJoin<User>("t1").On((p, t, t1) => t.Id == t1.Id).And((p, t, t1) => t1.Name != "10123456789")
               .InnerJoin<User>("t2").On((p, t, t1, t2) => t.Id == t1.Id).And((p, t, t1, t2) => t2.Name != "10123456789")
               //.ToSingle()//单条
               .ToEnumerable()
               .Where((p, t) => t.Id >= p.Id)
               .AndExists<User>("t3").Where((p, t, t1, t2, t3) => t.Id == t3.Id).And((p, t, t1, t2, t3) => t3.Id == 23).ToWhere()
               .And((p, t) => t.Id.In(p.IdArray) && t.Id >= 2 && t.Name.Like("e") && t.Name.LeftLike("e") && t.Name.RightLike("e"))
               .Or((p, t) => t.Id.In(p.IdArray) && t.Id >= 2 || t.Name.Like("e") && t.Name.LeftLike("e") && t.Name.RightLike("e"))
               .OrderBy(t => t.Id)
               .OrderByDescendingTable1(t => t.Id)
               .OrderByDescendingTable2(t => t.Id)
               .AddSql(" and {user}.{id} != @Id ", true)
               //.GetResult()
               .GetResult(0, 1);
            //.GetSqlTagFormat(true);

            string sql = more.ToString();
            System.Console.WriteLine(sql);
        }
    }
}
