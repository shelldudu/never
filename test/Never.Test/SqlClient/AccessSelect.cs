﻿using Never.EasySql;
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
    public class AccessSelect
    {
        [Xunit.Fact]
        public void testSelect1()
        {
            var dao = ConstructibleDaoBuilder<AccessBuilder>.Value.Build();
            //返回条数
            var one = dao.ToEasyLinqDao(new { Id = 1, IdArray = new[] { 22, 23, 24, 25 }.ToNullableParameter(), Name = "ee" })
               .Select<AccessUser>()
               .ToSingle()//单条
               .Where((p, t) => t.Id >= p.Id)
               .GetResult();

            string sql = one.ToString();
            System.Console.WriteLine(sql);
        }

        [Xunit.Fact]
        public void testSelect2()
        {
            var dao = ConstructibleDaoBuilder<AccessBuilder>.Value.Build();
            //返回条数
            var more = dao.ToEasyLinqDao(new { Id = 1, IdArray = new[] { 22, 23, 24, 25 }.ToNullableParameter(), Name = "ee" })
               .Select<AccessUser>()
               //.InnerJoin<User>("t1").On((p, t, t1) => t.Id == t1.Id).And((p, t, t1) => t1.Name != "10123456789")
               //.InnerJoin<User>("t2").On((p, t, t1, t2) => t.Id == t1.Id).And((p, t, t1, t2) => t2.Name != "10123456789")
               //.ToSingle()//单条
               .ToEnumerable()
               .Where((p, t) => t.Id >= p.Id)
               .AndExists<AccessUser>("t1").Where((p, t, t1) => t.Id == t1.Id).And((p, t, t1) => t1.Id == 23).ToWhere()
               //.AndExists<User>("t3").Where((p, t, t1, t2, t3) => t.Id == t3.Id).And((p, t, t1, t2, t3) => t3.Id == 23).ToWhere()
               .And((p, t) => t.Id.In(p.IdArray) && t.Id >= 2 && t.Name.Like("e") && t.Name.LeftLike("e") && t.Name.RightLike("e"))
               .Or((p, t) => t.Id.In(p.IdArray) && t.Id >= 2 || t.Name.Like("e") && t.Name.LeftLike("e") && t.Name.RightLike("e"))
               .OrderBy(t => t.Id)
               //.OrderByDescendingTable1(t => t.Id)
               //.OrderByDescendingTable2(t => t.Id)
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
            var user = new AccessUser() { Name = "sqlserver", UserId = 22334, AggregateId = NewId.GenerateGuid().ToString(), CreateDate = DateTime.Now, EditDate = DateTime.Now };
            var dao = ConstructibleDaoBuilder<AccessBuilder>.Value.Build();
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
            var user1 = new AccessUser() { Name = "sqlserver", UserId = 22339, AggregateId = NewId.GenerateGuid().ToString(), CreateDate = DateTime.Now, EditDate = DateTime.Now };
            var user2 = new AccessUser() { Name = "sqlserver", UserId = 223310, AggregateId = NewId.GenerateGuid().ToString(), CreateDate = DateTime.Now, EditDate = DateTime.Now };
            var dao = ConstructibleDaoBuilder<AccessBuilder>.Value.Build();
            dao.ToEasyLinqDao(new[] { user1, user2 })
                 .Insert<AccessUser>()
                 .UseBulk()
                 .InsertAll()
                 .GetResult();
        }

        [Xunit.Fact]
        public void testUpdate1()
        {
            var dao = ConstructibleDaoBuilder<AccessBuilder>.Value.Build();
            var user = dao.ToEasyLinqDao(new { Id = 260 }).Select<AccessUser>().As("t").ToSingle().Where((p, t) => t.Id == p.Id).GetResult();
            user.Name = string.Concat(user.Name.Trim(), "1");
            var sql = dao.ToEasyLinqDao(user)
                .Cached("UUUUUUUU")
                .Update().As("t")
                .SetColumn(t => t.Name, p => p.Name)
                .SetColumnWithValue(t => t.Version, user.Version + 1)
                .Where((p, t) => t.Id == p.Id);

            int row = sql.GetResult();

            user.Version = 6;
            row = dao.ToEasyLinqDao(user)
                .Cached("UUUUUUUU")
                .Update().As("t")
                .SetColumn(t => t.Name, p => p.Name)
                .SetColumnWithValue(t => t.Version, user.Version + 1)
                .Where((p, t) => t.Id == p.Id).GetResult();
        }

        [Xunit.Fact]
        public void testUpdate2()
        {
            var dao = ConstructibleDaoBuilder<AccessBuilder>.Value.Build();
            var user = dao.ToEasyLinqDao(new { Id = 260 }).Select<AccessUser>().As("t").ToSingle().Where((p, t) => t.Id == p.Id).GetResult();
            user.Name = string.Concat(user.Name.Trim(), "1");
            var sql = dao.ToEasyLinqDao(user)
                .Cached("UUUUUUUU")
                .Update().As("t")
                .InnerJoin<AccessUser>("t1").On((p, t, t1) => t.Id == t1.Id).And((p, t, t1) => t1.Name != "10123456789").ToUpdate()
                .SetColumn(t => t.Name, p => p.Name)
                .SetColumnWithValue(t => t.Version, user.Version + 1)
                .Where((p, t) => t.Id == p.Id && t.Name == "eee")
                .AndExists<AccessUser>("t1").Where((p, t, t1) => t.Id == t1.Id).And((p, t, t1) => t1.Id == 23).ToWhere();

            int row = sql.GetResult();
        }

        [Xunit.Fact]
        public void testDelete1()
        {
            var dao = ConstructibleDaoBuilder<AccessBuilder>.Value.Build();
            var user = dao.ToEasyLinqDao(new { Id = 260 }).Select<AccessUser>().ToSingle().Where((p, t) => t.Id == p.Id).GetResult();
            user.Id = user.Id + 10;
            var sql = dao.ToEasyLinqDao(user).Cached("DDDDDD")
                .Delete().As("t")
                .Where((p, t) => p.Id == t.Id);

            var row = sql.GetResult();
        }

        [Xunit.Fact]
        public void testDelete2()
        {
            var dao = ConstructibleDaoBuilder<AccessBuilder>.Value.Build();
            var user = dao.ToEasyLinqDao(new { Id = 260 }).Select<AccessUser>().ToSingle().Where((p, t) => t.Id == p.Id).GetResult();
            user.Id = user.Id + 10;
            var sql = dao.ToEasyLinqDao(user).Cached("DDDDDD")
                .Delete().As("t")
                .InnerJoin<AccessUser>("t1").On((p, t, t1) => t.Id == t1.Id).And((p, t, t1) => t1.Name != "10123456789").ToDelete()
                .Where((p, t) => p.Id == t.Id)
                .AndExists<AccessUser>("t2").Where((p, t, t1) => t.Id == t1.Id).And((p, t, t1) => t1.Id == 23).ToWhere(); ;

            var row = sql.GetResult();
        }
    }
}