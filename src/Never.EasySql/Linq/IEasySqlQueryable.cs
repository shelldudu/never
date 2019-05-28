using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// sql的定义
    /// </summary>
    public interface IEasySqlQueryable<Table, Parameter> where Parameter : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        IEasySqlContext Context { get; }

        /// <summary>
        ///Sql参数
        /// </summary>
        Parameter SqlParameter { get; }
    }

    /// <summary>
    /// sql的定义
    /// </summary>
    public interface IEasySqlQueryable<Table1, Table2, Parameter> where Parameter : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        IEasySqlContext Context { get; }

        /// <summary>
        ///Sql参数
        /// </summary>
        Parameter SqlParameter { get; }
    }

    /// <summary>
    /// sql的定义
    /// </summary>
    public interface IEasySqlQueryable<Table1, Table2, Table3, Parameter> where Parameter : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        IEasySqlContext Context { get; }

        /// <summary>
        ///Sql参数
        /// </summary>
        Parameter SqlParameter { get; }
    }

    /// <summary>
    /// sql的定义
    /// </summary>
    public interface IEasySqlQueryable<Table1, Table2, Table3, Table4, Parameter> where Parameter : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        IEasySqlContext Context { get; }

        /// <summary>
        ///Sql参数
        /// </summary>
        Parameter SqlParameter { get; }
    }

    /// <summary>
    /// sql的定义
    /// </summary>
    public interface IEasySqlQueryable<Table1, Table2, Table3, Table4, Table5, Parameter> where Parameter : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        IEasySqlContext Context { get; }

        /// <summary>
        ///Sql参数
        /// </summary>
        Parameter SqlParameter { get; }
    }

    /// <summary>
    ///  sql的定义
    /// </summary>
    public interface IEasySqlQueryable<Table1, Table2, Table3, Table4, Table5, Table6, Parameter> where Parameter : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        IEasySqlContext Context { get; }

        /// <summary>
        ///Sql参数
        /// </summary>
        Parameter SqlParameter { get; }
    }

    /// <summary>
    /// sql的定义
    /// </summary>
    public interface IEasySqlQueryable<Table1, Table2, Table3, Table4, Table5, Table6, Table7, Parameter> where Parameter : class
    {
        /// <summary>
        /// 上下文
        /// </summary>
        IEasySqlContext Context { get; }

        /// <summary>
        ///Sql参数
        /// </summary>
        Parameter SqlParameter { get; }
    }
}