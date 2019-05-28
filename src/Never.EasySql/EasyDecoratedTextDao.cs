using Never.EasySql.Xml;
using Never.SqlClient;
using Never.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Never.EasySql
{
    internal sealed class EasyDecoratedTextDaoHelper
    {
        internal static SqlTag SelectSqlTag(string sql, IDao dao)
        {
            var sqltag = new SqlTag()
            {
                CommandType = null,
                Id = NewId.GenerateString(NewId.StringLength.L24),
                IndentedOnNameSpace = false,
                IndentedOnSqlTag = false,
                NameSpace = null,
                Labels = new List<ILabel>(1),
            };

            var label = sqltag.ReadTextNode(sql, new Serialization.Json.ThunderWriter(sql.Length), new Serialization.Json.SequenceStringReader("1"), dao.SqlExecuter.GetParameterPrefix());
            sqltag.Labels.Add(new DecoratedTextLabel(label));
            return sqltag;
        }
    }

    internal sealed class DecoratedTextLabel : Xml.TextLabel, ILabel
    {
        private readonly Xml.TextLabel lable = null;
        private List<ParameterPosition> parameterPositions = null;
        private readonly int parameterPositionCount = 0;

        public DecoratedTextLabel(Xml.TextLabel lable)
        {
            this.lable = lable;
            this.parameterPositions = new List<ParameterPosition>(lable.ParameterPositions);
            this.parameterPositionCount = this.parameterPositions.Count;
        }

        public override void Format<T>(SqlTagFormat format, EasySqlParameter<T> parameter, IReadOnlyList<KeyValueTuple<string, object>> convert)
        {
            if (this.lable.SqlText.IsNullOrEmpty())
            {
                return;
            }

            if (this.parameterPositions == null || this.parameterPositionCount == 0)
            {
                format.Write(this.lable.SqlText);
                return;
            }

            for (var i = 0; i < this.lable.SqlText.Length; i++)
            {
                var para = this.MathPosition(this.parameterPositions, i);
                if (para == null)
                {
                    format.Write(this.lable.SqlText[i]);
                    continue;
                }

                var firstConvert = convert.FirstOrDefault(o => o.Key.IsEquals(para.Name));
                if (firstConvert == null)
                {
                    throw new Exception(string.Format("当前在sql语句中参数为{0}的值在所提供的参数列表中找不到", para.Name));
                }

                var value = firstConvert.Value;
                if (value is INullableParameter)
                {
                    value = ((IReferceNullableParameter)firstConvert.Value).Value;
                }

                var isArray = value is System.Collections.IEnumerable;
                if (!isArray || value is string)
                {
                    if (format.IfTextParameter(para))
                    {
                        if (value == null)
                        {
                            //format.WriteOnTextMode("\'null\'");
                            //format.WriteOnTextMode("null");
                        }
                        if (value == DBNull.Value)
                        {
                            //format.WriteOnTextMode("\'null\'");
                            format.WriteOnTextMode("null");
                        }
                        else
                        {
                            //format.WriteOnTextMode('\'');
                            format.WriteOnTextMode(value.ToString());
                            //format.WriteOnTextMode('\'');
                        }

                        i += para.PositionLength + 1;
                        format.WriteOnTextMode(this.lable.SqlText[i]);
                    }
                    else
                    {
                        var item = convert.FirstOrDefault(o => o.Key.Equals(para.Name));
                        if (item == null)
                        {
                            throw new Exception(string.Format("当前在sql语句中参数为{0}的值在所提供的参数列表中找不到", para.Name));
                        }

                        format.Write(this.lable.SqlText, para.PrefixStart, para.PositionLength + 1);
                        format.AddParameter(item);
                        i += para.PositionLength + 1;
                        format.Write(this.lable.SqlText[i]);
                    }
                }
                else
                {
                    var item = value as System.Collections.IEnumerable;
                    var ator = item.GetEnumerator();
                    var hadA = false;
                    var arrayLevel = 0;
                    if (format.IfTextParameter(para))
                    {
                        while (ator.MoveNext())
                        {
                            if (ator.Current == null || ator.Current == DBNull.Value)
                            {
                                continue;
                            }

                            if (hadA)
                            {
                                format.WriteOnTextMode(",");
                            }

                            //format.WriteOnTextMode('\'');
                            format.WriteOnTextMode(ator.Current.ToString());
                            //format.WriteOnTextMode('\'');
                            hadA = true;
                        }

                        i += para.PositionLength + 1;
                        format.WriteOnTextMode(this.lable.SqlText[i]);
                    }
                    else
                    {
                        format.Write(this.lable.SqlText[i]);
                        while (ator.MoveNext())
                        {
                            if (ator.Current == null || ator.Current == DBNull.Value)
                            {
                                continue;
                            }

                            var newvalue = (ator.Current == null || ator.Current == DBNull.Value) ? DBNull.Value : ator.Current;
                            var newkey = string.Format("{0}x{1}z", para.Name, arrayLevel);

                            if (hadA)
                            {
                                format.Write(",");
                                format.Write(para.ActualPrefix);
                            }

                            format.Write(newkey);
                            arrayLevel++;

                            format.AddParameter(newkey, newvalue);
                            hadA = true;
                        }

                        i += para.PositionLength + 1;
                        format.Write(this.lable.SqlText[i]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 对难用的语法进行一下装饰查询，更好的使用Idao接口，该对象每次执行一次都会释放IDao接口，请不要重复使用
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    public sealed class EasyDecoratedTextDao<Parameter> : EasyDecoratedDao, IDao, IDisposable
    {
        #region field

        private readonly IDao dao = null;
        private readonly EasySqlParameter<Parameter> parameter = null;

        #endregion field

        #region ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="parameter"></param>
        public EasyDecoratedTextDao(IDao dao,
            EasySqlParameter<Parameter> parameter) : base(dao)
        {
            this.dao = dao;
            this.parameter = parameter;
        }

        #endregion ctor

        #region crud

        /// <summary>
        /// 查询某一行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T QueryForObject<T>(string sql)
        {
            return this.QueryForObject<T>(sql, EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao));
        }

        private T QueryForObject<T>(string sql, SqlTag sqlTag)
        {
            if (this.dao.CurrentSession != null)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.QueryForObject<T, Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return sqlDao.QueryForObject<T, Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }

            using (this.dao)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.QueryForObject<T, Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.QueryForObject<T, Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }
        }

        /// <summary>
        /// 查询可列举的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IEnumerable<T> QueryForEnumerable<T>(string sql)
        {
            return this.QueryForEnumerable<T>(sql, EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao));
        }

        private IEnumerable<T> QueryForEnumerable<T>(string sql, SqlTag sqlTag)
        {
            if (this.dao.CurrentSession != null)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.QueryForEnumerable<T, Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.QueryForEnumerable<T, Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }

            using (this.dao)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.QueryForEnumerable<T, Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.QueryForEnumerable<T, Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }
        }

        /// <summary>
        /// 查询可列举的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IEnumerable<T> QueryForEnumerable<T>(Action<Parameter, StringBuilder> sql)
        {
            var sb = new StringBuilder();
            sql(this.parameter.Object, sb);
            return this.QueryForEnumerable<T>(sb.ToString());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Delete(string sql)
        {
            return this.Delete(sql, EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao));
        }

        private int Delete(string sql, SqlTag sqlTag)
        {
            if (this.dao.CurrentSession != null)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Delete<Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Delete<Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }

            using (this.dao)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Delete<Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Delete<Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Delete(Action<Parameter, StringBuilder> sql)
        {
            var sb = new StringBuilder();
            sql(this.parameter.Object, sb);
            return this.Delete(sb.ToString());
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Update(string sql)
        {
            return this.Update(sql, EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao));
        }

        private int Update(string sql, SqlTag sqlTag)
        {
            if (this.dao.CurrentSession != null)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Update<Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Update<Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }

            using (this.dao)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Update<Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Update<Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Update(Action<Parameter, StringBuilder> sql)
        {
            var sb = new StringBuilder();
            sql(this.parameter.Object, sb);
            return this.Update(sb.ToString());
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object Insert(string sql)
        {
            return this.Insert(sql, EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao));
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T Insert<T>(string sql)
        {
            return (T)this.Insert(sql, EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao));
        }

        private object Insert(string sql, SqlTag sqlTag)
        {
            if (this.dao.CurrentSession != null)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Insert<Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Insert<Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }

            using (this.dao)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Insert<Parameter>(sqlTag, this.parameter);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Insert<Parameter>(sqlTag, this.parameter);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object Insert(Action<Parameter, StringBuilder> sql)
        {
            var sb = new StringBuilder();
            sql(this.parameter.Object, sb);
            return this.Insert(sb.ToString());
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="callmode"></param>
        /// <returns></returns>
        public object Call(string sql, CallMode callmode)
        {
            return this.Call(sql, EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao), callmode);
        }

        private object Call(string sql, SqlTag sqlTag, CallMode callmode)
        {
            if (this.dao.CurrentSession != null)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Call<Parameter>(sqlTag, this.parameter, callmode);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Call<Parameter>(sqlTag, this.parameter, callmode);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }

            using (this.dao)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Call<Parameter>(sqlTag, this.parameter, callmode);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Call<Parameter>(sqlTag, this.parameter, callmode);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="callmode"></param>
        /// <returns></returns>
        public object Call(Action<Parameter, StringBuilder> sql, CallMode callmode = CallMode.ExecuteScalar | CallMode.CommandText)
        {
            var sb = new StringBuilder();
            sql(this.parameter.Object, sb);
            return this.Call(sb.ToString(), callmode);
        }

        /// <summary>
        /// 开启新事务
        /// </summary>
        /// <returns></returns>
        public ISession BeginTransaction()
        {
            return this.dao.BeginTransaction();
        }

        /// <summary>
        /// 开启新事务
        /// </summary>
        /// <param name="level"></param>
        public ISession BeginTransaction(IsolationLevel level)
        {
            return this.dao.BeginTransaction(level);
        }

        /// <summary>
        /// 提交
        /// </summary>
        public void CommitTransaction()
        {
            this.dao.CommitTransaction();
            this.dao.Dispose();
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="closeConnection">关闭连接</param>
        public void CommitTransaction(bool closeConnection)
        {
            this.dao.CommitTransaction(closeConnection);
            this.dao.Dispose();
        }

        /// <summary>
        /// 回滚
        /// </summary>
        public void RollBackTransaction()
        {
            this.dao.RollBackTransaction();
            this.dao.Dispose();
        }

        /// <summary>
        /// 回滚
        /// </summary>
        /// <param name="closeConnection">关闭连接</param>
        public void RollBackTransaction(bool closeConnection)
        {
            this.dao.RollBackTransaction(closeConnection);
            this.dao.Dispose();
        }

        #endregion crud
    }
}