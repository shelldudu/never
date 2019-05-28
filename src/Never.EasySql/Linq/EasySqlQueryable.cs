using Never.EasySql.Linq.MySql;
using Never.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    public class EasySqlQueryable<Table, Parameter> : IEasySqlQueryable<Table, Parameter> where Parameter : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        public IEasySqlContext Context { get; private set; }

        /// <summary>
        ///Sql参数
        /// </summary>
        public Parameter SqlParameter { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sqlParameter"></param>
        public EasySqlQueryable(IEasySqlContext context, Parameter sqlParameter)
        {
            this.Context = context;
            this.SqlParameter = sqlParameter;
        }
    }

    public class EasySqlQueryable<Table1, Table2, Parameter> : IEasySqlQueryable<Table1, Table2, Parameter> where Parameter : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        public IEasySqlContext Context { get; private set; }

        /// <summary>
        ///Sql参数
        /// </summary>
        public Parameter SqlParameter { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sqlParameter"></param>
        public EasySqlQueryable(IEasySqlContext context, Parameter sqlParameter)
        {
            this.Context = context;
            this.SqlParameter = sqlParameter;
        }
    }

    public class EasySqlQueryable<Table1, Table2, Table3, Parameter> : IEasySqlQueryable<Table1, Table2, Table3, Parameter> where Parameter : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        public IEasySqlContext Context { get; private set; }

        /// <summary>
        ///Sql参数
        /// </summary>
        public Parameter SqlParameter { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sqlParameter"></param>
        public EasySqlQueryable(IEasySqlContext context, Parameter sqlParameter)
        {
            this.Context = context;
            this.SqlParameter = sqlParameter;
        }
    }

    public class EasySqlQueryable<Table1, Table2, Table3, Table4, Parameter> : IEasySqlQueryable<Table1, Table2, Table3, Table4, Parameter> where Parameter : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        public IEasySqlContext Context { get; private set; }

        /// <summary>
        ///Sql参数
        /// </summary>
        public Parameter SqlParameter { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sqlParameter"></param>
        public EasySqlQueryable(IEasySqlContext context, Parameter sqlParameter)
        {
            this.Context = context;
            this.SqlParameter = sqlParameter;
        }
    }

    public class EasySqlQueryable<Table1, Table2, Table3, Table4, Table5, Parameter> : IEasySqlQueryable<Table1, Table2, Table3, Table4, Table5, Parameter> where Parameter : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        public IEasySqlContext Context { get; private set; }

        /// <summary>
        ///Sql参数
        /// </summary>
        public Parameter SqlParameter { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sqlParameter"></param>
        public EasySqlQueryable(IEasySqlContext context, Parameter sqlParameter)
        {
            this.Context = context;
            this.SqlParameter = sqlParameter;
        }
    }

    public class EasySqlQueryable<Table1, Table2, Table3, Table4, Table5, Table6, Parameter> : IEasySqlQueryable<Table1, Table2, Table3, Table4, Table5, Table6, Parameter> where Parameter : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        public IEasySqlContext Context { get; private set; }

        /// <summary>
        ///Sql参数
        /// </summary>
        public Parameter SqlParameter { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sqlParameter"></param>
        public EasySqlQueryable(IEasySqlContext context, Parameter sqlParameter)
        {
            this.Context = context;
            this.SqlParameter = sqlParameter;
        }
    }

    public class EasySqlQueryable<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Parameter> : IEasySqlQueryable<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Parameter> where Parameter : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        public IEasySqlContext Context { get; private set; }

        /// <summary>
        ///Sql参数
        /// </summary>
        public Parameter SqlParameter { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sqlParameter"></param>
        public EasySqlQueryable(IEasySqlContext context, Parameter sqlParameter)
        {
            this.Context = context;
            this.SqlParameter = sqlParameter;
        }
    }
}