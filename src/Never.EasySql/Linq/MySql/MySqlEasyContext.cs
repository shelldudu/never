using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq.MySql
{
    public abstract class MySqlEasyContext : EasySqlContext
    {
    }

    public class MySqlEasyContext<Table> : MySqlEasyContext, IEasySqlContext
    {
        #region IEasySqlContext

        string IEasySqlContext.SqlId => base.SqlId;

        void IEasySqlContext.Build()
        {
            base.BeginBuilding();
        }

        #endregion IEasySqlContext

        public IEasySqlQueryable<Table, Parameter> Build<Parameter>(Parameter sqlParameter) where Parameter : class
        {
            return new EasySqlQueryable<Table, Parameter>(this, sqlParameter);
        }
    }

    public class MySqlEasyContext<Table1, Table2> : MySqlEasyContext, IEasySqlContext
    {
        #region IEasySqlContext

        string IEasySqlContext.SqlId => base.SqlId;

        void IEasySqlContext.Build()
        {
            base.BeginBuilding();
        }

        #endregion IEasySqlContext

        public IEasySqlQueryable<Table1, Table2, Parameter> Build<Parameter>(Parameter sqlParameter) where Parameter : class
        {
            return new EasySqlQueryable<Table1, Table2, Parameter>(this, sqlParameter);
        }
    }

    public class MySqlEasyContext<Table1, Table2, Table3> : MySqlEasyContext, IEasySqlContext
    {
        #region IEasySqlContext

        string IEasySqlContext.SqlId => base.SqlId;

        void IEasySqlContext.Build()
        {
            base.BeginBuilding();
        }

        #endregion IEasySqlContext

        public IEasySqlQueryable<Table1, Table2, Table3, Parameter> Build<Parameter>(Parameter sqlParameter) where Parameter : class
        {
            return new EasySqlQueryable<Table1, Table2, Table3, Parameter>(this, sqlParameter);
        }
    }

    public class MySqlEasyContext<Table1, Table2, Table3, Table4> : MySqlEasyContext, IEasySqlContext
    {
        #region IEasySqlContext

        string IEasySqlContext.SqlId => base.SqlId;

        void IEasySqlContext.Build()
        {
            base.BeginBuilding();
        }

        #endregion IEasySqlContext

        public IEasySqlQueryable<Table1, Table2, Table3, Table4, Parameter> Build<Parameter>(Parameter sqlParameter) where Parameter : class
        {
            return new EasySqlQueryable<Table1, Table2, Table3, Table4, Parameter>(this, sqlParameter);
        }
    }

    public class MySqlEasyContext<Table1, Table2, Table3, Table4, Table5> : MySqlEasyContext, IEasySqlContext
    {
        #region IEasySqlContext

        string IEasySqlContext.SqlId => base.SqlId;

        void IEasySqlContext.Build()
        {
            base.BeginBuilding();
        }

        #endregion IEasySqlContext

        public IEasySqlQueryable<Table1, Table2, Table3, Table4, Table5, Parameter> Build<Parameter>(Parameter sqlParameter) where Parameter : class
        {
            return new EasySqlQueryable<Table1, Table2, Table3, Table4, Table5, Parameter>(this, sqlParameter);
        }
    }

    public class MySqlEasyContext<Table1, Table2, Table3, Table4, Table5, Table6> : MySqlEasyContext, IEasySqlContext
    {
        #region IEasySqlContext

        string IEasySqlContext.SqlId => base.SqlId;

        void IEasySqlContext.Build()
        {
            base.BeginBuilding();
        }

        #endregion IEasySqlContext

        public IEasySqlQueryable<Table1, Table2, Table3, Table4, Table5, Table6, Parameter> Build<Parameter>(Parameter sqlParameter) where Parameter : class
        {
            return new EasySqlQueryable<Table1, Table2, Table3, Table4, Table5, Table6, Parameter>(this, sqlParameter);
        }
    }

    public class MySqlEasyContext<Table1, Table2, Table3, Table4, Table5, Table6, Table7> : MySqlEasyContext, IEasySqlContext
    {
        #region IEasySqlContext

        string IEasySqlContext.SqlId => base.SqlId;

        void IEasySqlContext.Build()
        {
            base.BeginBuilding();
        }

        #endregion IEasySqlContext

        public IEasySqlQueryable<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Parameter> Build<Parameter>(Parameter sqlParameter) where Parameter : class
        {
            return new EasySqlQueryable<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Parameter>(this, sqlParameter);
        }
    }
}