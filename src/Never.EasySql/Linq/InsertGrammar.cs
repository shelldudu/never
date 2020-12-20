using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// insert 语法
    /// </summary>
    public struct SingleInsertGrammar<Parameter, Table>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal InsertContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 入口
        /// </summary>
        internal SingleInsertGrammar<Parameter, Table> StartInsertRecord()
        {
            this.Context.SetSingle().StartEntrance();
            return this;
        }

        /// <summary>
        /// 插入的所有字段名
        /// </summary>
        public InsertAllGrammar InsertAll()
        {
            this.Context.InsertAll();
            return new InsertAllGrammar() { Context = this.Context };
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public SingleInsertGrammar<Parameter, Table> Colum(Expression<Func<Table, object>> keyValue)
        {
            this.Context.Colum(keyValue);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public SingleInsertGrammar<Parameter, Table> Colum(Expression<Func<Table, object>> key, Expression<Func<Parameter, object>> value)
        {
            this.Context.Colum(key, value);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public SingleInsertGrammar<Parameter, Table> ColumWithFunc(Expression<Func<Table, object>> key, string value)
        {
            this.Context.ColumWithFunc(key, value);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public SingleInsertGrammar<Parameter, Table> ColumWithValue<TMember>(Expression<Func<Table, TMember>> key, TMember value)
        {
            this.Context.ColumWithValue(key, value);
            return this;
        }

        /// <summary>
        /// 返回最后插入语句
        /// </summary>
        public InsertLastInsertIdGrammar<ReturnType> LastInsertId<ReturnType>()
        {
            this.Context.InsertLastInsertId<ReturnType>();
            return new InsertLastInsertIdGrammar<ReturnType>() { Context = this.Context };
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public void GetResult()
        {
            this.Context.GetResult();
            return;
        }

        /// <summary>
        /// 插入全部
        /// </summary>
        public struct InsertAllGrammar
        {
            /// <summary>
            /// 上下文
            /// </summary>
            internal InsertContext<Parameter, Table> Context { get; set; }

            /// <summary>
            /// 返回最后插入语句
            /// </summary>
            public InsertLastInsertIdGrammar<ReturnType> LastInsertId<ReturnType>()
            {
                this.Context.InsertLastInsertId<ReturnType>();
                return new InsertLastInsertIdGrammar<ReturnType>() { Context = this.Context };
            }
            /// <summary>
            /// 获取结果
            /// </summary>
            public void GetResult()
            {
                this.Context.GetResult();
                return;
            }
        }

        /// <summary>
        /// 自增Id
        /// </summary>
        public struct InsertLastInsertIdGrammar<ReturnType>
        {
            /// <summary>
            /// 上下文
            /// </summary>
            internal InsertContext<Parameter, Table> Context { get; set; }

            /// <summary>
            /// 获取结果
            /// </summary>
            public ReturnType GetResult()
            {
                return this.Context.GetResult<ReturnType>();
            }
        }
    }

    /// <summary>
    /// insert 语法
    /// </summary>
    public struct BulkInsertGrammar<Parameter, Table>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal InsertContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 入口
        /// </summary>
        public BulkInsertGrammar<Parameter, Table> StartInsertRecord()
        {
            this.Context.SetBulk().StartEntrance();
            return this;
        }

        /// <summary>
        /// 插入的所有字段名
        /// </summary>
        public InsertAllGrammar InsertAll()
        {
            this.Context.InsertAll();
            return new InsertAllGrammar() { Context = this.Context };
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public BulkInsertGrammar<Parameter, Table> Colum(Expression<Func<Table, object>> keyValue)
        {
            this.Context.Colum(keyValue);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public BulkInsertGrammar<Parameter, Table> Colum<UnitParameter>(Expression<Func<Table, object>> key, Expression<Func<UnitParameter, object>> value)
        {
            string columnName = this.Context.FindColumnName(key, Linq.Context.FindTableInfo<Table>(), out _);
            string parameterName = this.Context.FindColumnName(value, Linq.Context.FindTableInfo<UnitParameter>(), out _);
            this.Context.InsertColumn(columnName, parameterName, false, false);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public BulkInsertGrammar<Parameter, Table> ColumWithFunc(Expression<Func<Table, object>> key, string value)
        {
            this.Context.ColumWithFunc(key, value);
            return this;
        }

        /// <summary>
        /// 插入的字段名
        /// </summary>
        public BulkInsertGrammar<Parameter, Table> ColumWithValue<TMember>(Expression<Func<Table, TMember>> key, TMember value)
        {
            this.Context.ColumWithValue(key, value);
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public void GetResult()
        {
            this.Context.GetResult();
            return;
        }

        /// <summary>
        /// 插入全部
        /// </summary>
        public struct InsertAllGrammar
        {
            /// <summary>
            /// 上下文
            /// </summary>
            internal InsertContext<Parameter, Table> Context { get; set; }

            /// <summary>
            /// 获取结果
            /// </summary>
            public void GetResult()
            {
                this.Context.GetResult();
                return;
            }
        }
    }
}
