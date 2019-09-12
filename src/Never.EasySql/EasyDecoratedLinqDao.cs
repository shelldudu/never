using Never.EasySql.Linq;
using Never.EasySql.Xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// 对难用的语法进行一下装饰查询，更好的使用Idao接口，该对象每次执行一次都会释放IDao接口，请不要重复使用
    /// </summary>
    public class EasyDecoratedLinqDao : EasyDecoratedDao, IDao, IDisposable
    {
        #region field

        private readonly IDao dao = null;

        #endregion field

        #region ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dao"></param>
        public EasyDecoratedLinqDao(IDao dao) : base(dao)
        {
            this.dao = dao;
        }

        #endregion ctor

        #region crud

        public Update<T> Update<T>(T info)
        {
            return new Update<T>();
        }

        public Delete<T> Delete<T>(T info)
        {
            return new Delete<T>();
        }

        public Insert<T> Insert<T>(T info)
        {
            return new Insert<T>();
        }

        public Select<T> Select<T>(T info)
        {
            return new Select<T>();
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <param name="callmode"></param>
        /// <returns></returns>
        public object Call<Parameter>(string sql, Parameter @parameter, CallMode callmode)
        {
            var sqlTag = EasyDecoratedTextDaoHelper.SelectSqlTag(sql, this.dao);
            if (this.dao.CurrentSession != null)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Call(sqlTag, new KeyValueEasySqlParameter<Parameter>(parameter), callmode);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Call(sqlTag, new KeyValueEasySqlParameter<Parameter>(parameter), callmode);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }

            using (this.dao)
            {
                var baseDao = this.dao as BaseDao;
                if (baseDao != null)
                {
                    return baseDao.Call(sqlTag, new KeyValueEasySqlParameter<Parameter>(parameter), callmode);
                }

                var sqlDao = this.dao as ISqlTagDao;
                if (sqlDao != null)
                {
                    return baseDao.Call(sqlTag, new KeyValueEasySqlParameter<Parameter>(parameter), callmode);
                }

                throw new NotSupportedException("the dao must impl the ISqlTagDao interface");
            }
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <param name="callmode"></param>
        /// <returns></returns>
        public object Call<Parameter>(Action<Parameter, StringBuilder> sql, Parameter @parameter, CallMode callmode = CallMode.ExecuteScalar | CallMode.CommandText)
        {
            var sb = new StringBuilder();
            sql(parameter, sb);
            return this.Call(sb.ToString(), @parameter, callmode);
        }

        #endregion crud
    }
}
