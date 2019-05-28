using Never.EasySql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.TestConsole.SqlClient
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
    }
}