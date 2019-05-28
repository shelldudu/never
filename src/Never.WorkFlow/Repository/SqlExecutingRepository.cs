using Never.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.WorkFlow.Repository
{
    /// <summary>
    /// sql仓储
    /// </summary>
    public abstract class SqlExecutingRepository
    {
        #region abst

        /// <summary>
        /// 打开sql excuter
        /// </summary>
        /// <returns></returns>
        protected abstract SqlClient.ISqlExecuter Open();

        /// <summary>
        /// 表的前缀名
        /// </summary>
        protected virtual string TablePrefixName
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取自增id的sql
        /// </summary>
        protected virtual string SelectIdentity
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion abst

        #region utils

        /// <summary>
        /// 转义
        /// </summary>
        /// <param name="source"></param>
        protected string Transferred(string source)
        {
            var writer = new Serialization.Json.ThunderWriter(source.Length);
            return this.Transferred(writer, source);
        }

        /// <summary>
        /// 转义
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="source"></param>
        protected string Transferred(Serialization.Json.ThunderWriter writer, string source)
        {
            writer.Clear();
            foreach (var i in source)
            {
                Serialization.Json.MethodProviders.CharMethodProvider.Default.Write(writer, new Serialization.Json.JsonSerializeSetting(), i);
            }
            return writer.ToString();
        }


        #endregion

        #region template

        private class MyTemplate
        {
            public string Name { get; set; }
            public string Text { get; set; }
            public DateTime EditDate { get; set; }
            public DateTime CreateDate { get; set; }
            public int Version { get; set; }
        }

        /// <summary>
        /// 更新模板
        /// </summary>
        public virtual int Change(IJsonSerializer jsonSerializer, Template template)
        {
            const string sqlText = "update {0}template set Text = @Text, EditDate = @EditDate,Version = Version + 1 where Name = @Name;";
            using (var sql = this.Open())
            {
                return sql.Update(string.Format(sqlText, this.TablePrefixName), new { @Name = template.Name, @EditDate = DateTime.Now, @Text = template.ToString(jsonSerializer) });
            }
        }

        /// <summary>
        /// 保存模板
        /// </summary>
        public virtual int Create(IJsonSerializer jsonSerializer, Template template)
        {
            const string sqlText = "insert into {0}template(Name,Text,CreateDate,EditDate,Version)values(@Name, @Text,@CreateDate, @EditDate,1);";
            using (var sql = this.Open())
            {
                return sql.Insert(string.Concat(string.Format(sqlText, this.TablePrefixName), this.SelectIdentity), new { @Name = template.Name, @CreateDate = DateTime.Now, @EditDate = DateTime.Now, @Text = template.ToString(jsonSerializer) }).ToString().AsInt();
            }
        }

        /// <summary>
        /// 保存，更新 模板
        /// </summary>
        public void SaveAndChange(IJsonSerializer jsonSerializer, Template[] addTemplates, Template[] changeTemplates)
        {
            const string addSqlText = "insert into {0}template(Name,Text,CreateDate,EditDate,Version)values(@Name, @Text,@CreateDate, @EditDate,1);";
            const string changeSqlText = "update {0}template set Text = @Text,EditDate = @EditDate,Version = Version + 1 where Name = @Name";

            using (var sql = this.Open())
            {
                var tran = sql as Never.SqlClient.ITransactionExecuter;
                if (tran != null)
                {
                    tran.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                    foreach (var template in addTemplates)
                    {
                        sql.Insert(string.Concat(string.Format(addSqlText, this.TablePrefixName), this.SelectIdentity), new { @Name = template.Name, @CreateDate = DateTime.Now, @EditDate = DateTime.Now, @Text = template.ToString(jsonSerializer) }).ToString().AsInt();
                    }

                    foreach (var template in changeTemplates)
                    {
                        sql.Update(string.Format(changeSqlText, this.TablePrefixName), new { @EditDate = DateTime.Now, @Text = template.ToString(jsonSerializer), @Name = template.Name });
                    }

                    tran.CommitTransaction();
                    return;
                }

                foreach (var template in addTemplates)
                {
                    sql.Insert(string.Concat(string.Format(addSqlText, this.TablePrefixName), this.SelectIdentity), new { @Name = template.Name, @CreateDate = DateTime.Now, @EditDate = DateTime.Now, @Text = template.ToString(jsonSerializer) }).ToString().AsInt();
                }

                foreach (var template in changeTemplates)
                {
                    sql.Update(string.Format(changeSqlText, this.TablePrefixName), new { @EditDate = DateTime.Now, @Text = template.ToString(jsonSerializer), @Name = template.Name });
                }
            }
        }

        /// <summary>
        /// 获取所有模板
        /// </summary>
        public virtual Template[] GetAll(IJsonSerializer jsonSerializer)
        {
            const string sqlText = "select Name,Text,CreateDate,EditDate,Version from {0}template;";
            IEnumerable<MyTemplate> list = null;
            using (var sql = this.Open())
            {
                list = sql.QueryForEnumerable<MyTemplate>(string.Format(sqlText, this.TablePrefixName), null);
            }
            var result = new List<Template>(list.Count());
            foreach (var i in list)
            {
                if (i.Text.IsNullOrEmpty())
                {
                    continue;
                }

                var t = Template.FromJson(jsonSerializer, i.Text);
                if (t != null)
                {
                    result.Add(t);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// 获取模板
        /// </summary>
        public virtual Template Get(IJsonSerializer jsonSerializer, string templateName)
        {
            const string sqlText = "select Name,Text,CreateDate,EditDate,Version from {0}template where Name = @Name;";
            MyTemplate @object = null;
            using (var sql = this.Open())
            {
                @object = sql.QueryForObject<MyTemplate>(string.Format(sqlText, this.TablePrefixName), new { @Name = templateName });
            }
            if (@object == null || @object.Text.IsNullOrEmpty())
            {
                return null;
            }

            return Template.FromJson(jsonSerializer, @object.Text);
        }

        /// <summary>
        /// 删除模板
        /// </summary>
        public virtual int Remove(string templateName)
        {
            const string sqlText = "delete {0}template from {0}template where Name = @Name;";
            using (var sql = this.Open())
            {
                return sql.Delete(string.Format(sqlText, this.TablePrefixName), new { @Name = templateName });
            }
        }

        #endregion template
    }
}
