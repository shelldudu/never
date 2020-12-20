using Never.EasySql;
using Never.SqlClient;
using Never.SqlClient.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Test
{
    [Never.SqlClient.TableName(Name = "user")]
    public class AccessUser
    {
        public string AggregateId { get; set; }

        [Never.SqlClient.Column(Optional = ColumnAttribute.ColumnOptional.AutoIncrement | ColumnAttribute.ColumnOptional.Primary, Name = "ID")]
        public int Id { get; set; }
        [TypeHandler(typeof(LongIntegerTypeHandler))]
        public long UserId { get; set; }
        [Never.SqlClient.Column(Name = "UserName")]
        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime EditDate { get; set; }

        public int Version { get; set; }
    }

    [Never.SqlClient.TableName(Name = "user")]
    public class SqlServerUser
    {
        public Guid AggregateId { get; set; }

        [Never.SqlClient.Column(Optional = Never.SqlClient.ColumnAttribute.ColumnOptional.AutoIncrement | Never.SqlClient.ColumnAttribute.ColumnOptional.Primary)]
        public int Id { get; set; }

        public long UserId { get; set; }
        [Never.SqlClient.Column(Name = "UserName")]
        public string Name { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime EditDate { get; set; }

        public int Version { get; set; }
    }

    [Never.SqlClient.TableName(Name = "user")]
    public class MySqlUser
    {
        public Guid AggregateId { get; set; }

        [Never.SqlClient.Column(Optional = Never.SqlClient.ColumnAttribute.ColumnOptional.AutoIncrement | Never.SqlClient.ColumnAttribute.ColumnOptional.Primary)]
        public int Id { get; set; }

        public long UserId { get; set; }
        [Never.SqlClient.Column(Name = "UserName")]
        public string Name { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime EditDate { get; set; }

        public int Version { get; set; }
    }

    public struct MyUserParameter
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public int[] IdArray { get; set; }
        public string UserName { get; set; }
    }

    #region builder

    public class SqlServerBuilder : BaseDaoBuilder
    {
        public string[] EmbeddedSqlMaps
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
                return @"server=192.168.137.110,1433;uid=sa;pwd=gg123456;database=p2p_login;";
            }
        }

        protected override IEasySqlExecuter CreateSqlExecuter()
        {
            return new EasySql.Client.SqlServerExecuter(this.ConnectionString);
        }
    }

    public class AccessBuilder : BaseDaoBuilder
    {
        public string[] EmbeddedSqlMaps
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
                return @"PROVIDER=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\shelldudu\Documents\p2p_login.mdb;";
            }
        }

        protected override IEasySqlExecuter CreateSqlExecuter()
        {
            return new EasySql.Client.AccessExecuter(this.ConnectionString);
        }
    }

    public class MySqlBuilder : BaseDaoBuilder
    {
        public string[] EmbeddedSqlMaps
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
                return "server=192.168.137.110;uid=sa;pwd=gg123456;database=login_user;port=3306;";
            }
        }

        protected override IEasySqlExecuter CreateSqlExecuter()
        {
            return new EasySql.Client.MySqlExecuter(this.ConnectionString);
        }
    }

    public class PostgreSqlBuilder : BaseDaoBuilder
    {
        public string[] EmbeddedSqlMaps
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
                return "server=192.168.137.110;uid=sa;pwd=gg123456;database=p2p_login;port=3306;";
            }
        }

        protected override IEasySqlExecuter CreateSqlExecuter()
        {
            return new EasySql.Client.PostgreSqlExecuter(this.ConnectionString);
        }
    }
    public class SqliteSqlBuilder : BaseDaoBuilder
    {
        public string[] EmbeddedSqlMaps
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
                return "server=192.168.137.110;uid=sa;pwd=gg123456;database=p2p_login;port=3306;";
            }
        }

        protected override IEasySqlExecuter CreateSqlExecuter()
        {
            return new EasySql.Client.SqliteExecuter(this.ConnectionString);
        }
    }

    #endregion builder
}