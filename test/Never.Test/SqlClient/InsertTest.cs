using Never.EasySql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test
{
    /// <summary>
    ///
    /// </summary>
    public class InsertTest : Program
    {
        public void TestInsert12()
        {
            var ab = new[] { "a", "b" };
            var dao = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build();
            // dao.BeginTransaction();
            //dao.CommitTransaction();
            //var delete = dao.ToEasyDao(EasySqlParameter.NewDeleteObject(new { UserId = 2 })).Delete("delUser");
            var para = new ArrayList
            {
            };

            para.Add(new Hashtable()
            {
                ["Id"] = 3,
                ["UserId"] = 3,
                ["UserName"] = "3"
            });

            para.Add(new Hashtable()
            {
                ["Id"] = 4,
                ["UserId"] = 4,
                ["UserName"] = "4",
            });

            // dao.BeginTransaction();
            try
            {
                //dao.BeginTransaction();
                //dao.BeginTransaction();
                var array = dao.ToEasyXmlDao(para).Insert("batchInsUser");
                // dao.CommitTransaction();
            }
            catch (Exception)
            {
                //  dao.RollBackTransaction();
                throw;
            }
        }

        [Xunit.Fact]
        public void TestSession()
        {
            var builder = ConstructibleDaoBuilder<SqlServerBuilder>.Value.Build;
            Task.Run(() =>
            {
                Console.WriteLine(builder.GetHashCode());
                var dao = builder();
                dao.BeginTransaction();
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
                Console.WriteLine("1:" + dao.GetHashCode());
                dao.RollBackTransaction();
                dao.Dispose();
            });

            Task.Run(() =>
            {
                Console.WriteLine(builder.GetHashCode());
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                var dao = builder();
                dao.BeginTransaction();
                Console.WriteLine("2:" + dao.GetHashCode());
                dao.RollBackTransaction();
                dao.Dispose();
            });

            Task.Run(() =>
            {
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));
                var dao = builder();
                dao.BeginTransaction();
                Console.WriteLine("1:" + dao.GetHashCode());
                dao.RollBackTransaction();
            });

            Console.ReadLine();
        }
    }
}