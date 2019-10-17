using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.MySql
{
    internal sealed class Select<Parameter, T> : Linq.Select<Parameter, T>
    {
        #region ctor
        public Select(IDao dao, EasySqlParameter<Parameter> sqlParameter) : this(dao, sqlParameter, null) { }
        public Select(IDao dao, EasySqlParameter<Parameter> sqlParameter, string cacheId) : base(dao, sqlParameter, cacheId) { }
        #endregion


        #region build
        protected override SqlTag Build()
        {
            var sqlTag = base.Build();
            if (sqlTag != null)
                return sqlTag;

            sqlTag = new SqlTag();
            if (this.cacheId != null)
                sqlTag.Id = this.cacheId;
            else
                sqlTag.Id = Guid.NewGuid().ToString();

            var labels = 
        }
        #endregion
    }
}
