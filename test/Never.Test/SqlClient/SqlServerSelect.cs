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
            var one = dao.ToEasyLinqDao(new { Id = 1, IdArray = new[] { 22, 23, 24, 25 }.ToNullableParameter(), Name = "ee" })
               .Select<User>()
               .ToEnumerable()
               .Where((p, t) => t.Id >= p.Id)
               //.AndExists<>
               .And((p, t) => p.IdArray.Contains(t.Id) && t.Id >= 2 && p.Name.Like("e") && p.Name.Like("e"))
               .OrderByDescending(t => t.Id)
               .GetSqlTagFormat(true);

            System.Console.WriteLine(one.ToString());
        }
    }
}
