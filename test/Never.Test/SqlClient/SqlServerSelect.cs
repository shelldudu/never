using Never.EasySql;
using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        [Xunit.Fact]
        public void testInsert1()
        {
            var user = new User() { Name = "sqlserver", UserId = 22334, AggregateId = NewId.GenerateGuid() };
            var dao = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build();
            var more = dao.ToEasyLinqDao(user)
                 .Insert()
                 .UseSingle()
                 .InsertAll()
                 .LastInsertId<int>()
                 .GetResult();

            string sql = more.ToString();
            System.Console.WriteLine(sql);
        }

        [Xunit.Fact]
        public void testInsert2()
        {
            var user1 = new User() { Name = "sqlserver", UserId = 22339, AggregateId = NewId.GenerateGuid(), CreateDate = DateTime.Now, EditDate = DateTime.Now };
            var user2 = new User() { Name = "sqlserver", UserId = 223310, AggregateId = NewId.GenerateGuid(), CreateDate = DateTime.Now, EditDate = DateTime.Now };
            var dao = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build();
            dao.ToEasyLinqDao(new[] { user1, user2 })
                 .Insert<User>()
                 .UseBulk()
                 .InsertAll()
                 .GetResult();
        }

        [Xunit.Fact]
        public void testReges()
        {
            var regex = new Regex(@"\{(?<name>.*?)\}", RegexOptions.Compiled | RegexOptions.Singleline);
            var sql = " and {user}.{id} != @Id ";
            var text = regex.Replace(sql, m => string.Concat("[", m.Groups["name"].Value, "]"));
        }
    }
}
