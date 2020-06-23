using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// select的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    public struct SelectJoinGrammar<Parameter, Table, Table1>
    {
        internal SelectContext<Parameter, Table> Context { get; set; }
        private readonly string @as;
        private readonly JoinOption option;
        private readonly List<Context.JoinInfo> joins;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        public SelectJoinGrammar(string @as, JoinOption option) : this()
        {
            this.@as = @as;
            this.option = option;
            this.joins = new List<Context.JoinInfo>(4);
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1> On(Expression<Func<Parameter, Table, Table1, bool>> expression)
        {
            this.joins.Add(new Context.JoinInfo()
            {
                On = expression,
                AsName = this.@as,
                JoinOption = this.option,
                Types = new[] { typeof(Parameter), typeof(Table1) },
            });

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1> And(Expression<Func<Parameter, Table, Table1, bool>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.joins.Last().And = expression;
            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2> Join<Table2>(string @as)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new SelectJoinGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.joins) { Context = this.Context };
        }


        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2> InnerJoin<Table2>(string @as)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new SelectJoinGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.joins) { Context = this.Context };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2> LeftJoin<Table2>(string @as)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new SelectJoinGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.joins) { Context = this.Context };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2> RightJoin<Table2>(string @as)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new SelectJoinGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.joins) { Context = this.Context };
        }

        /// <summary>
        /// then
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1> ToSingle()
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.Context.SetSingle().StartSelectColumn();
            this.Context.JoinSelect(this.joins);
            return new SingleSelectGrammar<Parameter, Table, Table1>() { Context = this.Context };
        }

        /// <summary>
        /// then
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1> ToEnumerable()
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.Context.SetPage().StartSelectColumn();
            this.Context.JoinSelect(this.joins);
            return new EnumerableSelectGrammar<Parameter, Table, Table1>() { Context = this.Context };
        }
    }

    /// <summary>
    /// select的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    public struct SelectJoinGrammar<Parameter, Table, Table1, Table2>
    {
        internal SelectContext<Parameter, Table> Context { get; set; }
        private readonly List<string> @as;
        private readonly JoinOption option;
        private readonly List<Context.JoinInfo> joins;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="joins"></param>
        public SelectJoinGrammar(List<string> @as, JoinOption option, List<Context.JoinInfo> joins) : this()
        {
            this.@as = @as;
            this.option = option;
            this.joins = joins;
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2> On(Expression<Func<Parameter, Table, Table1, Table2, bool>> expression)
        {
            this.joins.Add(new Context.JoinInfo()
            {
                On = expression,
                AsName = this.@as.Last(),
                JoinOption = this.option,
                Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2) },
            });
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2> And(Expression<Func<Parameter, Table, Table1, Table2, bool>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.joins.Last().And = expression;
            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3> Join<Table3>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.Join, this.joins) { Context = this.Context };
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3> InnerJoin<Table3>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.InnerJoin, this.joins) { Context = this.Context };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3> LeftJoin<Table3>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.LeftJoin, this.joins) { Context = this.Context };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3> RightJoin<Table3>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.RightJoin, this.joins) { Context = this.Context };
        }

        /// <summary>
        /// then
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2> ToSingle()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.Context.SetSingle().StartSelectColumn();
            this.Context.JoinSelect(this.joins);
            return new SingleSelectGrammar<Parameter, Table, Table1, Table2>() { Context = this.Context };
        }

        /// <summary>
        /// then
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2> ToEnumerable()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.Context.SetSingle().StartSelectColumn();
            this.Context.JoinSelect(this.joins);
            return new EnumerableSelectGrammar<Parameter, Table, Table1, Table2>() { Context = this.Context };
        }
    }

    /// <summary>
    /// select的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    /// <typeparam name="Table3"></typeparam>
    public struct SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3>
    {
        internal SelectContext<Parameter, Table> Context { get; set; }
        private readonly List<string> @as;
        private readonly JoinOption option;
        private readonly List<Context.JoinInfo> joins;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="joins"></param>
        public SelectJoinGrammar(List<string> @as, JoinOption option, List<Context.JoinInfo> joins) : this()
        {
            this.@as = @as;
            this.option = option;
            this.joins = joins;
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3> On(Expression<Func<Parameter, Table, Table1, Table2, Table3, bool>> expression)
        {
            this.joins.Add(new Context.JoinInfo()
            {
                On = expression,
                AsName = this.@as.Last(),
                JoinOption = this.option,
                Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3) },
            });
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3> And(Expression<Func<Parameter, Table, Table1, Table2, Table3, bool>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.joins.Last().And = expression;
            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> Join<Table4>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.Join, this.joins) { Context = this.Context };
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> InnerJoin<Table4>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.InnerJoin, this.joins) { Context = this.Context };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> LeftJoin<Table4>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.LeftJoin, this.joins) { Context = this.Context };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> RightJoin<Table4>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.RightJoin, this.joins) { Context = this.Context };
        }

        /// <summary>
        /// then
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3> ToSingle()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.Context.SetSingle().StartSelectColumn();
            this.Context.JoinSelect(this.joins);
            return new SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3>() { Context = this.Context };
        }

        /// <summary>
        /// then
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3> ToEnumerable()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.Context.SetSingle().StartSelectColumn();
            this.Context.JoinSelect(this.joins);
            return new EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3>() { Context = this.Context };
        }
    }

    /// <summary>
    /// select的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    /// <typeparam name="Table3"></typeparam>
    /// <typeparam name="Table4"></typeparam>
    public struct SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4>
    {
        internal SelectContext<Parameter, Table> Context { get; set; }
        private readonly List<string> @as;
        private readonly JoinOption option;
        private readonly List<Context.JoinInfo> joins;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="joins"></param>
        public SelectJoinGrammar(List<string> @as, JoinOption option, List<Context.JoinInfo> joins) : this()
        {
            this.@as = @as;
            this.option = option;
            this.joins = joins;
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> On(Expression<Func<Parameter, Table, Table1, Table2, Table3, Table4, bool>> expression)
        {
            this.joins.Add(new Context.JoinInfo()
            {
                On = expression,
                AsName = this.@as.Last(),
                JoinOption = this.option,
                Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3), typeof(Table4) },
            });
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> And(Expression<Func<Parameter, Table, Table1, Table2, Table3, Table4, bool>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.joins.Last().And = expression;
            return this;
        }

        /// <summary>
        /// then
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> ToSingle()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.Context.SetSingle().StartSelectColumn();
            this.Context.JoinSelect(this.joins);
            return new SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4>() { Context = this.Context };
        }

        /// <summary>
        /// then
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> ToEnumerable()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.Context.SetSingle().StartSelectColumn();
            this.Context.JoinSelect(this.joins);
            return new EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4>() { Context = this.Context };
        }
    }

    /// <summary>
    /// 单条查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="Table">查询结果对象</typeparam>
    public struct SingleSelectGrammar<Parameter, Table>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal SelectContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        internal SingleSelectGrammar<Parameter, Table> StartSelectColumn()
        {
            this.Context.SetSingle().StartSelectColumn();
            return this;
        }

        #region linq

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table> SelectAll()
        {
            this.Context.SelectAll();
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            this.Context.Select(expression);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table> Select(string func, string @as)
        {
            this.Context.Select(func, @as);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            this.Context.Select(expression, @as);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table> OrderBy(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table> OrderByDescending(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where()
        {
            this.Context.Where();
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where(Expression<Func<Parameter, Table, object>> expression)
        {
            this.Context.Where(expression);
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// 返回执行结果
        /// </summary>
        public Table GetResult()
        {
            return this.Context.GetResult();
        }

        #endregion

        #region where

        /// <summary>
        /// where 条件
        /// </summary>
        public struct SelectWhereGrammar
        {
            /// <summary>
            /// 上下文
            /// </summary>
            internal SelectContext<Parameter, Table> Context { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderBy(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescending(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }
            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// and 拼字符串
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndAppend(string expression)
            {
                this.Context.Append(string.Concat("and ", expression));
                return this;
            }

            /// <summary>
            /// or 拼字符串
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrAppend(string expression)
            {
                this.Context.Append(string.Concat("or ", expression));
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Append(string expression)
            {
                this.Context.Append(expression);
                return this;
            }

            /// <summary>
            /// 获取结果
            /// </summary>
            public Table GetResult()
            {
                return this.Context.GetResult();
            }
        }

        #endregion

        #region select where

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereExistsGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和e(exists)</param>
            public SelectWhereExistsGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.exists = new Context.WhereExistsInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotExists = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                this.exists.Where = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> And(Expression<Func<Table5, bool>> expression)
            {
                if (this.exists.Where == null)
                    throw new Exception("please use Where method first;");

                this.exists.And = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.exists) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.exists) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="exists"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table3> Join<Table3>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table3>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereInGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereInInfo @in;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和i(in)</param>
            public SelectWhereInGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.@in = new Context.WhereInInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotIn = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Field(Expression<Func<Table5, bool>> expression)
            {
                this.@in.Field = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.@in.Where = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.@in) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.@in) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> Join<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        #endregion
    }

    /// <summary>
    /// 单条查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="Table">查询结果对象</typeparam>
    /// <typeparam name="Table1"></typeparam>
    public struct SingleSelectGrammar<Parameter, Table, Table1>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal SelectContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        internal SingleSelectGrammar<Parameter, Table, Table1> StartSelectColumn()
        {
            this.Context.SetSingle().StartSelectColumn();
            return this;
        }

        #region linq

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1> SelectAll()
        {
            this.Context.SelectAll();
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            this.Context.Select(expression);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1> Select(string func, string @as)
        {
            this.Context.Select(func, @as);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            this.Context.Select(expression, @as);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1> OrderBy(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1> OrderByDescending(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1> OrderByTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1> OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where()
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where(Expression<Func<Parameter, Table, object>> expression)
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// 返回执行结果
        /// </summary>
        public Table GetResult()
        {
            return this.Context.GetResult();
        }

        #endregion

        #region where

        /// <summary>
        /// where 条件
        /// </summary>
        public struct SelectWhereGrammar
        {
            /// <summary>
            /// 上下文
            /// </summary>
            internal SelectContext<Parameter, Table> Context { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderBy(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescending(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }
            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// and 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndAppend(string sql)
            {
                this.Context.Append(string.Concat("and ", sql));
                return this;
            }

            /// <summary>
            /// or 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrAppend(string sql)
            {
                this.Context.Append(string.Concat("or ", sql));
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Append(string sql)
            {
                this.Context.Append(sql);
                return this;
            }

            /// <summary>
            /// sql
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Last(string sql)
            {
                this.Context.Last(sql);
                return this;
            }

            /// <summary>
            /// 获取结果
            /// </summary>
            public Table GetResult()
            {
                return this.Context.GetResult();
            }
        }

        #endregion

        #region select where

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereExistsGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和e(exists)</param>
            public SelectWhereExistsGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.exists = new Context.WhereExistsInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotExists = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                this.exists.Where = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> And(Expression<Func<Table5, bool>> expression)
            {
                if (this.exists.Where == null)
                    throw new Exception("please use Where method first;");

                this.exists.And = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.exists) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.exists) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="exists"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table3> Join<Table3>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table3>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereInGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereInInfo @in;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和i(in)</param>
            public SelectWhereInGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.@in = new Context.WhereInInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotIn = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Field(Expression<Func<Table5, bool>> expression)
            {
                this.@in.Field = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.@in.Where = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.@in) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.@in) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> Join<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        #endregion
    }

    /// <summary>
    /// 单条查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="Table">查询结果对象</typeparam>
    /// <typeparam name="Table1">join的表</typeparam>
    /// <typeparam name="Table2">join的表</typeparam>
    public struct SingleSelectGrammar<Parameter, Table, Table1, Table2>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal SelectContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        internal SingleSelectGrammar<Parameter, Table, Table1, Table2> StartSelectColumn()
        {
            this.Context.SetSingle().StartSelectColumn();
            return this;
        }

        #region linq

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2> SelectAll()
        {
            this.Context.SelectAll();
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            this.Context.Select(expression);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2> Select(string func, string @as)
        {
            this.Context.Select(func, @as);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            this.Context.Select(expression, @as);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2> OrderBy(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2> OrderByDescending(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2> OrderByTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2> OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2> OrderByTable2(Expression<Func<Table2, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2> OrderByDescendingTable2(Expression<Func<Table2, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where()
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where(Expression<Func<Parameter, Table, object>> expression)
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// 返回执行结果
        /// </summary>
        public Table GetResult()
        {
            return this.Context.GetResult();
        }

        #endregion

        #region where

        /// <summary>
        /// where 条件
        /// </summary>
        public struct SelectWhereGrammar
        {
            /// <summary>
            /// 上下文
            /// </summary>
            internal SelectContext<Parameter, Table> Context { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderBy(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescending(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable2(Expression<Func<Table2, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable2(Expression<Func<Table2, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }
            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// and 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndAppend(string sql)
            {
                this.Context.Append(string.Concat("and ", sql));
                return this;
            }

            /// <summary>
            /// or 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrAppend(string sql)
            {
                this.Context.Append(string.Concat("or ", sql));
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Append(string sql)
            {
                this.Context.Append(sql);
                return this;
            }

            /// <summary>
            /// sql
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Last(string sql)
            {
                this.Context.Last(sql);
                return this;
            }

            /// <summary>
            /// 获取结果
            /// </summary>
            public Table GetResult()
            {
                return this.Context.GetResult();
            }
        }

        #endregion

        #region select where

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereExistsGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和e(exists)</param>
            public SelectWhereExistsGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.exists = new Context.WhereExistsInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotExists = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                this.exists.Where = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> And(Expression<Func<Table5, bool>> expression)
            {
                if (this.exists.Where == null)
                    throw new Exception("please use Where method first;");

                this.exists.And = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.exists) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.exists) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="exists"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table3> Join<Table3>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table3>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereInGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereInInfo @in;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和i(in)</param>
            public SelectWhereInGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.@in = new Context.WhereInInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotIn = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Field(Expression<Func<Table5, bool>> expression)
            {
                this.@in.Field = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.@in.Where = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.@in) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.@in) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> Join<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        #endregion
    }

    /// <summary>
    /// 单条查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="Table">查询结果对象</typeparam>
    /// <typeparam name="Table1">join的表</typeparam>
    /// <typeparam name="Table2">join的表</typeparam>
    /// <typeparam name="Table3">join的表</typeparam>
    public struct SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal SelectContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        internal SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3> StartSelectColumn()
        {
            this.Context.SetSingle().StartSelectColumn();
            return this;
        }

        #region linq

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3> SelectAll()
        {
            this.Context.SelectAll();
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            this.Context.Select(expression);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3> Select(string func, string @as)
        {
            this.Context.Select(func, @as);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            this.Context.Select(expression, @as);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderBy(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderByDescending(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderByTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderByTable2(Expression<Func<Table2, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderByDescendingTable2(Expression<Func<Table2, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderByTable3(Expression<Func<Table3, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderByDescendingTable3(Expression<Func<Table3, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where()
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where(Expression<Func<Parameter, Table, object>> expression)
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// 返回执行结果
        /// </summary>
        public Table GetResult()
        {
            return this.Context.GetResult();
        }

        #endregion

        #region where

        /// <summary>
        /// where 条件
        /// </summary>
        public struct SelectWhereGrammar
        {
            /// <summary>
            /// 上下文
            /// </summary>
            internal SelectContext<Parameter, Table> Context { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderBy(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescending(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable2(Expression<Func<Table2, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable2(Expression<Func<Table2, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable3(Expression<Func<Table3, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable3(Expression<Func<Table3, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }
            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// and 拼字符串
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndAppend(string expression)
            {
                this.Context.Append(string.Concat("and ", expression));
                return this;
            }

            /// <summary>
            /// or 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrAppend(string sql)
            {
                this.Context.Append(string.Concat("or ", sql));
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Append(string sql)
            {
                this.Context.Append(sql);
                return this;
            }

            /// <summary>
            /// sql
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Last(string sql)
            {
                this.Context.Last(sql);
                return this;
            }

            /// <summary>
            /// 获取结果
            /// </summary>
            public Table GetResult()
            {
                return this.Context.GetResult();
            }
        }

        #endregion

        #region select where

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereExistsGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和e(exists)</param>
            public SelectWhereExistsGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.exists = new Context.WhereExistsInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotExists = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                this.exists.Where = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> And(Expression<Func<Table5, bool>> expression)
            {
                if (this.exists.Where == null)
                    throw new Exception("please use Where method first;");

                this.exists.And = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.exists) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.exists) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="exists"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> Join<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereInGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereInInfo @in;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和i(in)</param>
            public SelectWhereInGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.@in = new Context.WhereInInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotIn = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Field(Expression<Func<Table5, bool>> expression)
            {
                this.@in.Field = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.@in.Where = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.@in) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.@in) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> Join<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        #endregion
    }

    /// <summary>
    /// 单条查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="Table">查询结果对象</typeparam>
    /// <typeparam name="Table1">join的表</typeparam>
    /// <typeparam name="Table2">join的表</typeparam>
    /// <typeparam name="Table3">join的表</typeparam>
    /// <typeparam name="Table4">join的表</typeparam>
    public struct SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal SelectContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        internal SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> StartSelectColumn()
        {
            this.Context.SetSingle().StartSelectColumn();
            return this;
        }

        #region linq

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> SelectAll()
        {
            this.Context.SelectAll();
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            this.Context.Select(expression);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> Select(string func, string @as)
        {
            this.Context.Select(func, @as);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            this.Context.Select(expression, @as);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderBy(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByDescending(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByTable2(Expression<Func<Table2, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByDescendingTable2(Expression<Func<Table2, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByTable3(Expression<Func<Table3, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SingleSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByDescendingTable3(Expression<Func<Table3, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where()
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where(Expression<Func<Parameter, Table, object>> expression)
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// 返回执行结果
        /// </summary>
        public Table GetResult()
        {
            return this.Context.GetResult();
        }

        #endregion

        #region where

        /// <summary>
        /// where 条件
        /// </summary>
        public struct SelectWhereGrammar
        {
            /// <summary>
            /// 上下文
            /// </summary>
            internal SelectContext<Parameter, Table> Context { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderBy(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescending(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable2(Expression<Func<Table2, object>> expression)
            {
                this.Context.OrderBy(expression, 1);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable2(Expression<Func<Table2, object>> expression)
            {
                this.Context.OrderBy(expression, 1);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable3(Expression<Func<Table3, object>> expression)
            {
                this.Context.OrderBy(expression, 2);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable4(Expression<Func<Table4, object>> expression)
            {
                this.Context.OrderBy(expression, 2);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable4(Expression<Func<Table4, object>> expression)
            {
                this.Context.OrderBy(expression, 3);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable3(Expression<Func<Table4, object>> expression)
            {
                this.Context.OrderBy(expression, 3);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// and 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndAppend(string sql)
            {
                this.Context.Append(string.Concat("and ", sql));
                return this;
            }

            /// <summary>
            /// or 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrAppend(string sql)
            {
                this.Context.Append(string.Concat("or ", sql));
                return this;
            }

            /// <summary>
            /// sql
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Append(string sql)
            {
                this.Context.Append(sql);
                return this;
            }


            /// <summary>
            /// sql
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Last(string sql)
            {
                this.Context.Last(sql);
                return this;
            }

            /// <summary>
            /// 获取结果
            /// </summary>
            public Table GetResult()
            {
                return this.Context.GetResult();
            }
        }

        #endregion

        #region select where

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereExistsGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和e(exists)</param>
            public SelectWhereExistsGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.exists = new Context.WhereExistsInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotExists = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                this.exists.Where = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> And(Expression<Func<Table5, bool>> expression)
            {
                if (this.exists.Where == null)
                    throw new Exception("please use Where method first;");

                this.exists.And = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.exists) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.exists) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="exists"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> Join<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereInGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereInInfo @in;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和i(in)</param>
            public SelectWhereInGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.@in = new Context.WhereInInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotIn = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Field(Expression<Func<Table5, bool>> expression)
            {
                this.@in.Field = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.@in.Where = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.@in) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.@in) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> Join<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        #endregion
    }

    /// <summary>
    /// 多条查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="Table">查询结果对象</typeparam>
    public struct EnumerableSelectGrammar<Parameter, Table>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal SelectContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        internal EnumerableSelectGrammar<Parameter, Table> StartSelectColumn()
        {
            this.Context.SetSingle().StartSelectColumn();
            return this;
        }

        #region linq

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table> SelectAll()
        {
            this.Context.SelectAll();
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            this.Context.Select(expression);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table> Select(string func, string @as)
        {
            this.Context.Select(func, @as);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            this.Context.Select(expression, @as);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table> OrderBy(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table> OrderByDescending(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where()
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where(Expression<Func<Parameter, Table, object>> expression)
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// 返回执行结果
        /// </summary>
        public IEnumerable<Table> GetResult(int startIndex, int endIndex)
        {
            return this.Context.GetResults(startIndex, endIndex);
        }

        #endregion

        #region where

        /// <summary>
        /// where 条件
        /// </summary>
        public struct SelectWhereGrammar
        {
            /// <summary>
            /// 上下文
            /// </summary>
            internal SelectContext<Parameter, Table> Context { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderBy(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescending(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }
            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// and 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndAppend(string sql)
            {
                this.Context.Append(string.Concat("and ", sql));
                return this;
            }

            /// <summary>
            /// or 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrAppend(string sql)
            {
                this.Context.Append(string.Concat("or ", sql));
                return this;
            }


            /// <summary>
            /// sql
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Last(string sql)
            {
                this.Context.Last(sql);
                return this;
            }


            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Append(string expression)
            {
                this.Context.Append(expression);
                return this;
            }

            /// <summary>
            /// 获取结果
            /// </summary>
            public IEnumerable<Table> GetResult(int startIndex, int endIndex)
            {
                return this.Context.GetResults(startIndex, endIndex);
            }
        }

        #endregion

        #region select where

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereExistsGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和e(exists)</param>
            public SelectWhereExistsGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.exists = new Context.WhereExistsInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotExists = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                this.exists.Where = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> And(Expression<Func<Table5, bool>> expression)
            {
                if (this.exists.Where == null)
                    throw new Exception("please use Where method first;");

                this.exists.And = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.exists) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.exists) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="exists"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table3> Join<Table3>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table3>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereInGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereInInfo @in;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和i(in)</param>
            public SelectWhereInGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.@in = new Context.WhereInInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotIn = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Field(Expression<Func<Table5, bool>> expression)
            {
                this.@in.Field = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.@in.Where = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.@in) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.@in) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> Join<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        #endregion
    }

    /// <summary>
    /// 单条查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="Table">查询结果对象</typeparam>
    /// <typeparam name="Table1"></typeparam>
    public struct EnumerableSelectGrammar<Parameter, Table, Table1>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal SelectContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        internal EnumerableSelectGrammar<Parameter, Table, Table1> StartSelectColumn()
        {
            this.Context.SetSingle().StartSelectColumn();
            return this;
        }

        #region linq

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1> SelectAll()
        {
            this.Context.SelectAll();
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            this.Context.Select(expression);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1> Select(string func, string @as)
        {
            this.Context.Select(func, @as);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            this.Context.Select(expression, @as);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1> OrderBy(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1> OrderByDescending(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1> OrderByTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1> OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where()
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where(Expression<Func<Parameter, Table, object>> expression)
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// 返回执行结果
        /// </summary>
        public IEnumerable<Table> GetResult(int startIndex, int endIndex)
        {
            return this.Context.GetResults(startIndex, endIndex);
        }

        #endregion

        #region where

        /// <summary>
        /// where 条件
        /// </summary>
        public struct SelectWhereGrammar
        {
            /// <summary>
            /// 上下文
            /// </summary>
            internal SelectContext<Parameter, Table> Context { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderBy(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescending(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }
            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// and 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndAppend(string sql)
            {
                this.Context.Append(string.Concat("and ", sql));
                return this;
            }

            /// <summary>
            /// or 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrAppend(string sql)
            {
                this.Context.Append(string.Concat("or ", sql));
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Append(string sql)
            {
                this.Context.Append(sql);
                return this;
            }


            /// <summary>
            /// sql
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Last(string sql)
            {
                this.Context.Last(sql);
                return this;
            }

            /// <summary>
            /// 返回执行结果
            /// </summary>
            public IEnumerable<Table> GetResult(int startIndex, int endIndex)
            {
                return this.Context.GetResults(startIndex, endIndex);
            }
        }

        #endregion

        #region select where

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereExistsGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和e(exists)</param>
            public SelectWhereExistsGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.exists = new Context.WhereExistsInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotExists = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                this.exists.Where = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> And(Expression<Func<Table5, bool>> expression)
            {
                if (this.exists.Where == null)
                    throw new Exception("please use Where method first;");

                this.exists.And = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.exists) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.exists) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="exists"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table3> Join<Table3>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table3>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereInGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereInInfo @in;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和i(in)</param>
            public SelectWhereInGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.@in = new Context.WhereInInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotIn = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Field(Expression<Func<Table5, bool>> expression)
            {
                this.@in.Field = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.@in.Where = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.@in) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.@in) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> Join<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        #endregion
    }

    /// <summary>
    /// 单条查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="Table">查询结果对象</typeparam>
    /// <typeparam name="Table1">join的表</typeparam>
    /// <typeparam name="Table2">join的表</typeparam>
    public struct EnumerableSelectGrammar<Parameter, Table, Table1, Table2>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal SelectContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        internal EnumerableSelectGrammar<Parameter, Table, Table1, Table2> StartSelectColumn()
        {
            this.Context.SetSingle().StartSelectColumn();
            return this;
        }

        #region linq

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2> SelectAll()
        {
            this.Context.SelectAll();
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            this.Context.Select(expression);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2> Select(string func, string @as)
        {
            this.Context.Select(func, @as);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            this.Context.Select(expression, @as);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2> OrderBy(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2> OrderByDescending(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2> OrderByTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2> OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2> OrderByTable2(Expression<Func<Table2, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2> OrderByDescendingTable2(Expression<Func<Table2, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where()
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where(Expression<Func<Parameter, Table, object>> expression)
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// 返回执行结果
        /// </summary>
        public IEnumerable<Table> GetResult(int startIndex, int endIndex)
        {
            return this.Context.GetResults(startIndex, endIndex);
        }

        #endregion

        #region where

        /// <summary>
        /// where 条件
        /// </summary>
        public struct SelectWhereGrammar
        {
            /// <summary>
            /// 上下文
            /// </summary>
            internal SelectContext<Parameter, Table> Context { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderBy(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescending(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable2(Expression<Func<Table2, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable2(Expression<Func<Table2, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }
            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// and 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndAppend(string sql)
            {
                this.Context.Append(string.Concat("and ", sql));
                return this;
            }

            /// <summary>
            /// or 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrAppend(string sql)
            {
                this.Context.Append(string.Concat("or ", sql));
                return this;
            }

            /// <summary>
            /// sql
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Append(string sql)
            {
                this.Context.Append(sql);
                return this;
            }

            /// <summary>
            /// sql
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Last(string sql)
            {
                this.Context.Last(sql);
                return this;
            }

            /// <summary>
            /// 返回执行结果
            /// </summary>
            public IEnumerable<Table> GetResult(int startIndex, int endIndex)
            {
                return this.Context.GetResults(startIndex, endIndex);
            }
        }

        #endregion

        #region select where

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereExistsGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和e(exists)</param>
            public SelectWhereExistsGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.exists = new Context.WhereExistsInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotExists = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                this.exists.Where = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> And(Expression<Func<Table5, bool>> expression)
            {
                if (this.exists.Where == null)
                    throw new Exception("please use Where method first;");

                this.exists.And = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.exists) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.exists) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="exists"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table3> Join<Table3>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table3>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereInGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereInInfo @in;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和i(in)</param>
            public SelectWhereInGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.@in = new Context.WhereInInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotIn = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Field(Expression<Func<Table5, bool>> expression)
            {
                this.@in.Field = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.@in.Where = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.@in) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.@in) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> Join<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        #endregion
    }

    /// <summary>
    /// 单条查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="Table">查询结果对象</typeparam>
    /// <typeparam name="Table1">join的表</typeparam>
    /// <typeparam name="Table2">join的表</typeparam>
    /// <typeparam name="Table3">join的表</typeparam>
    public struct EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal SelectContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        internal EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3> StartSelectColumn()
        {
            this.Context.SetSingle().StartSelectColumn();
            return this;
        }

        #region linq

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3> SelectAll()
        {
            this.Context.SelectAll();
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            this.Context.Select(expression);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3> Select(string func, string @as)
        {
            this.Context.Select(func, @as);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            this.Context.Select(expression, @as);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderBy(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderByDescending(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderByTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderByTable2(Expression<Func<Table2, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderByDescendingTable2(Expression<Func<Table2, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderByTable3(Expression<Func<Table3, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3> OrderByDescendingTable3(Expression<Func<Table3, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where()
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where(Expression<Func<Parameter, Table, object>> expression)
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// 返回执行结果
        /// </summary>
        public IEnumerable<Table> GetResult(int startIndex, int endIndex)
        {
            return this.Context.GetResults(startIndex, endIndex);
        }

        #endregion

        #region where

        /// <summary>
        /// where 条件
        /// </summary>
        public struct SelectWhereGrammar
        {
            /// <summary>
            /// 上下文
            /// </summary>
            internal SelectContext<Parameter, Table> Context { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderBy(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescending(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable2(Expression<Func<Table2, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable2(Expression<Func<Table2, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable3(Expression<Func<Table3, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable3(Expression<Func<Table3, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }
            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// and 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndAppend(string sql)
            {
                this.Context.Append(string.Concat("and ", sql));
                return this;
            }

            /// <summary>
            /// or 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrAppend(string sql)
            {
                this.Context.Append(string.Concat("or ", sql));
                return this;
            }

            /// <summary>
            /// sql
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Append(string sql)
            {
                this.Context.Append(sql);
                return this;
            }

            /// <summary>
            /// sql
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Last(string sql)
            {
                this.Context.Last(sql);
                return this;
            }

            /// <summary>
            /// 返回执行结果
            /// </summary>
            public IEnumerable<Table> GetResult(int startIndex, int endIndex)
            {
                return this.Context.GetResults(startIndex, endIndex);
            }
        }

        #endregion

        #region select where

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereExistsGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和e(exists)</param>
            public SelectWhereExistsGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.exists = new Context.WhereExistsInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotExists = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                this.exists.Where = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> And(Expression<Func<Table5, bool>> expression)
            {
                if (this.exists.Where == null)
                    throw new Exception("please use Where method first;");

                this.exists.And = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.exists) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.exists) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="exists"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> Join<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereInGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereInInfo @in;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和i(in)</param>
            public SelectWhereInGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.@in = new Context.WhereInInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotIn = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Field(Expression<Func<Table5, bool>> expression)
            {
                this.@in.Field = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.@in.Where = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.@in) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.@in) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> Join<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        #endregion
    }

    /// <summary>
    /// 单条查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="Table">查询结果对象</typeparam>
    /// <typeparam name="Table1">join的表</typeparam>
    /// <typeparam name="Table2">join的表</typeparam>
    /// <typeparam name="Table3">join的表</typeparam>
    /// <typeparam name="Table4">join的表</typeparam>
    public struct EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal SelectContext<Parameter, Table> Context { get; set; }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        internal EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> StartSelectColumn()
        {
            this.Context.SetSingle().StartSelectColumn();
            return this;
        }

        #region linq

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> SelectAll()
        {
            this.Context.SelectAll();
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            this.Context.Select(expression);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> Select(string func, string @as)
        {
            this.Context.Select(func, @as);
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
            this.Context.Select(expression, @as);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderBy(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByDescending(Expression<Func<Table, object>> expression)
        {
            this.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByTable2(Expression<Func<Table2, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByDescendingTable2(Expression<Func<Table2, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByTable3(Expression<Func<Table3, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByDescendingTable3(Expression<Func<Table3, object>> expression)
        {
            this.Context.OrderBy(expression, 0);
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where()
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public SelectWhereGrammar Where(Expression<Func<Parameter, Table, object>> expression)
        {
            return new SelectWhereGrammar()
            {
                Context = this.Context,
            };
        }

        /// <summary>
        /// 返回执行结果
        /// </summary>
        public IEnumerable<Table> GetResult(int startIndex, int endIndex)
        {
            return this.Context.GetResults(startIndex, endIndex);
        }

        #endregion

        #region where

        /// <summary>
        /// where 条件
        /// </summary>
        public struct SelectWhereGrammar
        {
            /// <summary>
            /// 上下文
            /// </summary>
            internal SelectContext<Parameter, Table> Context { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderBy(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescending(Expression<Func<Table, object>> expression)
            {
                this.Context.OrderBy(expression);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable1(Expression<Func<Table1, object>> expression)
            {
                this.Context.OrderBy(expression, 0);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable2(Expression<Func<Table2, object>> expression)
            {
                this.Context.OrderBy(expression, 1);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable2(Expression<Func<Table2, object>> expression)
            {
                this.Context.OrderBy(expression, 1);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable3(Expression<Func<Table3, object>> expression)
            {
                this.Context.OrderBy(expression, 2);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable4(Expression<Func<Table4, object>> expression)
            {
                this.Context.OrderBy(expression, 2);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByTable4(Expression<Func<Table4, object>> expression)
            {
                this.Context.OrderBy(expression, 3);
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereGrammar OrderByDescendingTable3(Expression<Func<Table4, object>> expression)
            {
                this.Context.OrderBy(expression, 3);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> AndNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> OrNotExists<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> AndNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.and, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table5"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> OrNotIn<Table5>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table5>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// and 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndAppend(string sql)
            {
                this.Context.Append(string.Concat("and ", sql));
                return this;
            }

            /// <summary>
            /// or 拼字符串
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrAppend(string sql)
            {
                this.Context.Append(string.Concat("or ", sql));
                return this;
            }

            /// <summary>
            /// sql
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Append(string sql)
            {
                this.Context.Append(sql);
                return this;
            }

            /// <summary>
            /// sql
            /// </summary>
            /// <param name="sql">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar Last(string sql)
            {
                this.Context.Last(sql);
                return this;
            }

            /// <summary>
            /// 返回执行结果
            /// </summary>
            public IEnumerable<Table> GetResult(int startIndex, int endIndex)
            {
                return this.Context.GetResults(startIndex, endIndex);
            }
        }

        #endregion

        #region select where

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereExistsGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和e(exists)</param>
            public SelectWhereExistsGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.exists = new Context.WhereExistsInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotExists = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                this.exists.Where = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5> And(Expression<Func<Table5, bool>> expression)
            {
                if (this.exists.Where == null)
                    throw new Exception("please use Where method first;");

                this.exists.And = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.exists) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.exists) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="exists"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> Join<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereExistsGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExistsInfo exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        public struct SelectWhereInGrammar<Table5>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereInInfo @in;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和i(in)</param>
            public SelectWhereInGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.@in = new Context.WhereInInfo()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotIn = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table5) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Field(Expression<Func<Table5, bool>> expression)
            {
                this.@in.Field = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5> Where(Expression<Func<Table5, bool>> expression)
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.@in.Where = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> Join<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.@in) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> InnerJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.@in) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> LeftJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table6"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> RightJoin<Table6>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table5, Table6>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> On(Expression<Func<Table5, Table6, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6> And(Expression<Func<Table5, Table6, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> Join<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> InnerJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> LeftJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table7"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> RightJoin<Table7>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> On(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7> And(Expression<Func<Table5, Table6, Table7, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> Join<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> InnerJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> LeftJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table8"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> RightJoin<Table8>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table5, Table6, Table7, Table8>(this.@as, JoinOption.RightJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table5"></typeparam>
        /// <typeparam name="Table6"></typeparam>
        /// <typeparam name="Table7"></typeparam>
        /// <typeparam name="Table8"></typeparam>
        public struct SelectWhereInGrammar<Table5, Table6, Table7, Table8>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereInInfo @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table5), typeof(Table6), typeof(Table7), typeof(Table8) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> On(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table5, Table6, Table7, Table8> And(Expression<Func<Table5, Table6, Table7, Table8, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.where.Context.AppenInWhereIn(this.@in);
                return this.where;
            }
        }

        #endregion
    }
}
