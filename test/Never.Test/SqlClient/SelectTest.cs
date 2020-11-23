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
            var text = new StringBuilder().Append("abcdef", 0, 1).ToString();
            text = new StringBuilder().Append("abcdef", 1, 1).ToString();
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

            var dao = ConstructibleDaoBuilder<MySqlBuilder>.Value.Build();

            //var more = dao.ToEasyTextDao(new { Id = new[] { 1, 2 }, UserName = "666" }).QueryForEnumerable<MyTable>("select a.* from `user` as a where a.Id in (@Id) or a.UserName = @UserName;");

            //返回单条
            var one = dao.ToEasyLinqDao(new { Id = 1, IdArray = new[] { 1, 2, 3, 4 }.ToNullableParameter(), Name = "ee" })
               .Select<User>()
               .ToEnumerable()
               .Where((p, t) => t.Id >= p.Id)
               //.AndNotExists<MyTable2>("t1").Where((p, t, t1) => (t.Id == p.Id && p.Id >= t.Id) || (p.Id > 0) || t.Id != 2).And((p, t, t1) => t1.Id != 2).ToWhere()
               //.And((p, t) => p.Id == t.Id)
               .And((p, t) => p.IdArray.Contains(222) && t.Id >= 2 && p.Name.Like("e") && p.Name.Like("e"))
               .OrderByDescending(t => t.Id)
               .GetSqlTagFormat();

            //return;
            //返回列表
            //var array = dao.ToEasyLinqDao(new { Id = 1 }).Select<SqlServerBuilder>().Where(null).ToList(1, 5).GetResult();
            //返回列表，里面join了其他表
            //var array2 = dao.ToEasyLinqDao(new { Id = 1 }).Select<SqlServerBuilder>().LeftJoin<SqlServerBuilder>((p, t1, t2) => t1.EmbeddedSqlMaps == t2.EmbeddedSqlMaps)
            //     .Where(null).ToList(1, 5).GetResult();

            //更新
            var update = dao.ToEasyLinqDao(new { Id = 7 }).Cached("AAA").Update()
               .From("user").As("u")
            //.Join<User>("t1").On((t, t2, p) => p.Id >= 1).And((t, t2, p) => p.Name == "3").ToUpdate()
            //  .Join<MyTable2>("t2").On((p, t1, t2) => p.Id == t1.Id).And((p, t1, t2) => t2.Name == "3")
            //.Join<MyTable2>("t3")
            //.Join<MyTable2>("t4")
            //.ToUpdate()
            .SetColumn(m => m.Id)
            //.SetColumnWithFunc(m => m.CreateTime, "now()")
            // .SetColumnWithValue(m => m.Name, "abcd")
            .Where((p, t) => p.Id == t.Id)
            .AndNotExists<User>("t1").Where((p, t, t1) => (t.Id == p.Id && p.Id >= t.Id) || (p.Id > 0) || t.Id != 2).And((p, t, t1) => t1.Id != 2).ToWhere()
            //.Join<MyTable2>("t2").On((p, t1, t2) => (t1.Id == p.Id && p.Id >= t1.Id) || (p.Id > 0) || t1.Id != 2).And((p, t1, t2) => t1.Id != 2)
            //.Join<MyTable2>("t3").On((p, t1, t2, t3) => (t1.Id == p.Id && p.Id >= t1.Id) || (p.Id > 0) || t1.Id != 2).And((p, t1, t2, t3) => t1.Id != 2).ToWhere()
            //.AndExists<MyTable2>("t1").Where((p, t) => (t.Id == p.Id && p.Id >= t.Id) || (p.Id > 0) || t.Id != 2).And((p, t) => t.Id != 2)
            //.Join<MyTable2>("t2").On((p, t1, t2) => (t1.Id == p.Id && p.Id >= t1.Id) || (p.Id > 0) || t1.Id != 2).And((p, t1, t2) => t1.Id != 2)
            //.Join<MyTable2>("t3").On((p, t1, t2, t3) => (t1.Id == p.Id && p.Id >= t1.Id) || (p.Id > 0) || t1.Id != 2).And((p, t1, t2, t3) => t1.Id != 2).ToWhere()
            //.OrNotExists<MyTable2>("t1").Where((p, t1) => (t1.Id == p.Id && p.Id >= t1.Id) || (p.Id > 0) || t1.Id != 2).And((p, t1) => t1.Id != 2).ToWhere()
            // .OrExists<MyTable2>("t1").Where((p, t) => (t.Id == p.Id && p.Id >= t.Id) || (p.Id > 0) || t.Id != 2).And((p, t) => t.Id != 2)
            //.Join<MyTable2>("t2").On((p, t1, t2) => (t1.Id == p.Id && p.Id >= t1.Id) || (p.Id > 0) || t1.Id != 2).And((p, t1, t2) => t1.Id != 2)
            //.Join<MyTable2>("t3").On((p, t1, t2, t3) => (t1.Id == p.Id && p.Id >= t1.Id) || (p.Id > 0) || t1.Id != 2).And((p, t1, t2, t3) => t1.Id != 2).ToWhere()
            // .AndNotIn<MyTable2>("t1").Field((p, t) => p.Id == t.Id).Where((p, t) => t.Name == "ee").ToWhere()
            // .AndIn<MyTable2>("t1").Field((p, t) => p.Id == t.Id).Where((p, t) => t.Name == "ee").ToWhere()
            // .OrNotIn<MyTable2>("t1").Field((p, t) => p.Id == t.Id).Where((p, t) => t.Name == "ee")
            // .Join<MyTable2>("t2").On((p, t1, t2) => p.Id >= 1).And((p, t1, t2) => t1.Name == "3").ToWhere()
            //.OrIn<MyTable2>("t1").Field((p, t) => p.Id == t.Id).Where((p, t) => t.Name == "ee").ToWhere()
            .Append(";")
            .GetResult();

            //删除

            var delete = dao.ToEasyLinqDao(new User()).Delete<User>()
             .Where((p, t) => t.Name == "abc")
             .GetResult();

            //推入
            var insert = dao.ToEasyLinqDao(new User() { Id = 1, Name = "666", CreateTime = DateTime.Now, UserId = 555 }).Insert<User>()
                .UseSingle()
                .InsertAll()
                //.Colum(m => m.UserId)
                //.Colum(m => m.Name)
                .LastInsertId<int>()
                .GetResult<int>();
        }
    }
}