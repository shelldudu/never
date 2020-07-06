using Never.EasySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test
{
    public class User
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public DateTime EditDate { get; set; }
        public Guid AggregateId { get; set; }
        public decimal Amount { get; set; }
        public float Balance { get; set; }
        public double Freeze { get; set; }
    }

    public struct MyUserParameter
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public int[] IdArray { get; set; }
        public string UserName { get; set; }
    }

    #region builder

    public class SqlServerBuilder : XmlContentDaoBuilder.XmlEmbeddedDaoBuilder
    {
        public override string[] EmbeddedSqlMaps
        {
            get
            {
                return new string[]
                {
                   "Never.EasySql.Xml.easysqldemo.xml,Never.EasySql",
                };
            }
        }

        public override string ConnectionString
        {
            get
            {
                return @"server=192.168.110.130;uid=sa;pwd=gg123456;database=p2p_admin;";
            }
        }

        protected override IEasySqlExecuter CreateSqlExecuter()
        {
            return new EasySql.Client.SqlServerExecuter(this.ConnectionString);
        }
    }

    public class MySqlBuilder : XmlContentDaoBuilder.XmlEmbeddedDaoBuilder
    {
        public override string[] EmbeddedSqlMaps
        {
            get
            {
                return new string[]
                {
                   "Never.EasySql.Xml.easysqldemo.xml,Never.EasySql",
                };
            }
        }

        public override string ConnectionString
        {
            get
            {
                return "server=127.0.0.1;uid=sa;pwd=gg123456;database=test;port=3306;";
            }
        }

        protected override IEasySqlExecuter CreateSqlExecuter()
        {
            return new EasySql.Client.MySqlExecuter(this.ConnectionString);
        }
    }

    public class PostgreSqlBuilder : EmbeddedDaoBuilder
    {
        public override string[] EmbeddedSqlMaps
        {
            get
            {
                return new string[]
                {
                    "Never.EasySql.Xml.easysqldemo.xml,Never.EasySql",
                };
            }
        }

        public override string ConnectionString
        {
            get
            {
                return "server=127.0.0.1;uid=sa;pwd=gg123456;database=b2c_message;port=3306;";
            }
        }

        protected override IEasySqlExecuter CreateSqlExecuter()
        {
            return new EasySql.Client.PostgreSqlExecuter(this.ConnectionString);
        }
    }

    #endregion builder
}