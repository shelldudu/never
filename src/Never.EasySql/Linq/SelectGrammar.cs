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
        internal SingleSelectGrammar<Parameter, Table> select { get; set; }
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

            return new SelectJoinGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.joins) { select = this.select };
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

            return new SelectJoinGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.joins) { select = this.select };
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

            return new SelectJoinGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.joins) { select = this.select };
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

            return new SelectJoinGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.joins) { select = this.select };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1> OrderBy(Expression<Func<Table, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1> OrderBy1(Expression<Func<Table1, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderBy(expression, this.@as);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1> OrderByDescending(Expression<Func<Table, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderByDescending(expression, this.@as);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1> OrderByDescending1(Expression<Func<Table1, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderByDescending(expression, this.@as);
            return this;
        }

        /// <summary>
        /// then
        /// </summary>
        public SingleSelectGrammar<Parameter, Table> ToSingle()
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.StartSelectColumn();
            this.select.Context.JoinOnSelect(this.joins);
            return this.select;
        }

        /// <summary>
        /// then
        /// </summary>
        public SingleSelectGrammar<Parameter, Table> ToEnumerable()
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.StartSelectColumn();
            this.select.Context.JoinOnSelect(this.joins);
            return this.select;
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
        internal SingleSelectGrammar<Parameter, Table> select { get; set; }
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
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.Join, this.joins) { select = this.select };
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
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.InnerJoin, this.joins) { select = this.select };
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
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.LeftJoin, this.joins) { select = this.select };
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
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.RightJoin, this.joins) { select = this.select };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2> OrderBy(Expression<Func<Table, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2> OrderBy1(Expression<Func<Table1, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderBy(expression, this.@as[0]);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2> OrderBy2(Expression<Func<Table2, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderBy(expression, this.@as[1]);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2> OrderByDescending(Expression<Func<Table, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderByDescending(expression, this.@as[0]);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2> OrderByDescending1(Expression<Func<Table1, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderByDescending(expression, this.@as[1]);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2> OrderByDescending2(Expression<Func<Table2, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderByDescending(expression, this.@as[2]);
            return this;
        }

        /// <summary>
        /// then
        /// </summary>
        public SingleSelectGrammar<Parameter, Table> ToSingle()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.StartSelectColumn();
            this.select.Context.JoinOnSelect(this.joins);
            return this.select;
        }

        /// <summary>
        /// then
        /// </summary>
        public SingleSelectGrammar<Parameter, Table> ToEnumerable()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.StartSelectColumn();
            this.select.Context.JoinOnSelect(this.joins);
            return this.select;
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
        internal SingleSelectGrammar<Parameter, Table> select { get; set; }
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
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.Join, this.joins) { select = this.select };
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
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.InnerJoin, this.joins) { select = this.select };
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
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.LeftJoin, this.joins) { select = this.select };
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
            return new SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.RightJoin, this.joins) { select = this.select };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3> OrderBy(Expression<Func<Table, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3> OrderBy1(Expression<Func<Table1, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderBy(expression, this.@as[0]);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3> OrderBy2(Expression<Func<Table2, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderBy(expression, this.@as[1]);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3> OrderBy3(Expression<Func<Table3, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderBy(expression, this.@as[2]);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3> OrderByDescending(Expression<Func<Table, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderByDescending(expression, this.@as[0]);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3> OrderByDescending1(Expression<Func<Table1, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderByDescending(expression, this.@as[1]);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3> OrderByDescending2(Expression<Func<Table2, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderByDescending(expression, this.@as[2]);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3> OrderByDescending3(Expression<Func<Table3, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderByDescending(expression, this.@as[3]);
            return this;
        }

        /// <summary>
        /// then
        /// </summary>
        public SingleSelectGrammar<Parameter, Table> ToSingle()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.StartSelectColumn();
            this.select.Context.JoinOnSelect(this.joins);
            return this.select;
        }

        /// <summary>
        /// then
        /// </summary>
        public SingleSelectGrammar<Parameter, Table> ToEnumerable()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.StartSelectColumn();
            this.select.Context.JoinOnSelect(this.joins);
            return this.select;
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
        internal SingleSelectGrammar<Parameter, Table> select { get; set; }
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
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderBy(Expression<Func<Table, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderBy(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderBy1(Expression<Func<Table1, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderBy(expression,this.@as[0]);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderBy2(Expression<Func<Table2, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderBy(expression, this.@as[1]);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderBy3(Expression<Func<Table3, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderBy(expression, this.@as[2]);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderBy4(Expression<Func<Table4, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderBy(expression, this.@as[3]);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByDescending(Expression<Func<Table, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderByDescending(expression, this.@as[0]);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByDescending1(Expression<Func<Table1, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderByDescending(expression, this.@as[1]);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByDescending2(Expression<Func<Table2, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderByDescending(expression, this.@as[2]);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByDescending3(Expression<Func<Table3, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderByDescending(expression, this.@as[3]);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SelectJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> OrderByDescending4(Expression<Func<Table4, object>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.Context.OrderByDescending(expression, this.@as[4]);
            return this;
        }

        /// <summary>
        /// then
        /// </summary>
        public SingleSelectGrammar<Parameter, Table> ToSingle()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.StartSelectColumn();
            this.select.Context.JoinOnSelect(this.joins);
            return this.select;
        }

        /// <summary>
        /// then
        /// </summary>
        public SingleSelectGrammar<Parameter, Table> ToEnumerable()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.select.StartSelectColumn();
            this.select.Context.JoinOnSelect(this.joins);
            return this.select;
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

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        public struct SelectWhereExistsGrammar<Table1>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereExists exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和e(exists)</param>
            public SelectWhereExistsGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.exists = new Context.WhereExists()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotExists = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table1) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1> Where(Expression<Func<Table1, bool>> expression)
            {
                this.exists.Where = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1> And(Expression<Func<Table1, bool>> expression)
            {
                if (this.exists.Where == null)
                    throw new Exception("please use Where method first;");

                this.exists.And = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2> Join<Table2>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.exists) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2> InnerJoin<Table2>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.exists) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2> LeftJoin<Table2>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2> RightJoin<Table2>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                this.where.Context.JoinOnWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        public struct SelectWhereExistsGrammar<Table1, Table2>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExists exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="exists"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExists exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2> On(Expression<Func<Table1, Table2, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2> And(Expression<Func<Table1, Table2, bool>> expression)
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
            public SelectWhereExistsGrammar<Table1, Table2, Table3> Join<Table3>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3> InnerJoin<Table3>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3> LeftJoin<Table3>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3> RightJoin<Table3>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3>(this.@as, JoinOption.RightJoin, this.exists)
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

                this.where.Context.JoinOnWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        public struct SelectWhereExistsGrammar<Table1, Table2, Table3>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExists exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExists exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3> On(Expression<Func<Table1, Table2, Table3, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3> And(Expression<Func<Table1, Table2, Table3, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3, Table4> Join<Table4>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3, Table4> InnerJoin<Table4>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3, Table4> LeftJoin<Table4>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3, Table4> RightJoin<Table4>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.RightJoin, this.exists)
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

                this.where.Context.JoinOnWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <typeparam name="Table4"></typeparam>
        public struct SelectWhereExistsGrammar<Table1, Table2, Table3, Table4>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExists exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExists exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3), typeof(Table4) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3, Table4> On(Expression<Func<Table1, Table2, Table3, Table4, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3, Table4> And(Expression<Func<Table1, Table2, Table3, Table4, bool>> expression)
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

                this.where.Context.JoinOnWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        public struct SelectWhereInGrammar<Table1>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereIn @in;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和i(in)</param>
            public SelectWhereInGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.@in = new Context.WhereIn()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotIn = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table1) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1> Field(Expression<Func<Table1, bool>> expression)
            {
                this.@in.Field = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1> Where(Expression<Func<Table1, bool>> expression)
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.@in.Where = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2> Join<Table2>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.@in) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2> InnerJoin<Table2>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.@in) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2> LeftJoin<Table2>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2> RightJoin<Table2>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.where.Context.JoinOnWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        public struct SelectWhereInGrammar<Table1, Table2>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereIn @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereIn @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2> On(Expression<Func<Table1, Table2, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2> And(Expression<Func<Table1, Table2, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3> Join<Table3>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3> InnerJoin<Table3>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3> LeftJoin<Table3>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3> RightJoin<Table3>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3>(this.@as, JoinOption.RightJoin, this.@in)
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

                this.where.Context.JoinOnWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        public struct SelectWhereInGrammar<Table1, Table2, Table3>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereIn @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereIn @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3> On(Expression<Func<Table1, Table2, Table3, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3> And(Expression<Func<Table1, Table2, Table3, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3, Table4> Join<Table4>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3, Table4> InnerJoin<Table4>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3, Table4> LeftJoin<Table4>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3, Table4> RightJoin<Table4>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.RightJoin, this.@in)
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

                this.where.Context.JoinOnWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <typeparam name="Table4"></typeparam>
        public struct SelectWhereInGrammar<Table1, Table2, Table3, Table4>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereIn @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereIn @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3), typeof(Table4) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3, Table4> On(Expression<Func<Table1, Table2, Table3, Table4, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3, Table4> And(Expression<Func<Table1, Table2, Table3, Table4, bool>> expression)
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

                this.where.Context.JoinOnWhereIn(this.@in);
                return this.where;
            }
        }

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
            /// 存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1> AndExists<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table1>(@as, AndOrOption.and, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1> AndNotExists<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table1>(@as, AndOrOption.and, 'n') { where = this };
            }
            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1> OrExists<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table1>(@as, AndOrOption.or, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1> OrNotExists<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table1>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1> AndIn<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table1>(@as, AndOrOption.and, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1> AndNotIn<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table1>(@as, AndOrOption.and, 'n') { where = this };
            }
            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1> OrIn<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table1>(@as, AndOrOption.or, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1> OrNotIn<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table1>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndNotExists(string expression)
            {
                this.Context.Where(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrNotExists(string expression)
            {
                this.Context.Where(AndOrOption.or, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndIn(string expression)
            {
                this.Context.Where(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndNotIn(string expression)
            {
                this.Context.Where(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrIn(string expression)
            {
                this.Context.Where(AndOrOption.or, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrNotIn(string expression)
            {
                this.Context.Where(AndOrOption.or, expression);
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
            this.Context.SetPage().StartSelectColumn();
            return this;
        }

        #region linq

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
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public EnumerableSelectGrammar<Parameter, Table> SelectAll()
        {
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression)
        {
            return this;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public EnumerableSelectGrammar<Parameter, Table> Select<TMember>(Expression<Func<Table, TMember>> expression, string @as)
        {
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
        public IEnumerable<Table> GetResults(PagedSearch paged)
        {
            return this.Context.GetResults(paged);
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        public struct SelectWhereExistsGrammar<Table1>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereExists exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和e(exists)</param>
            public SelectWhereExistsGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.exists = new Context.WhereExists()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotExists = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table1) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1> Where(Expression<Func<Table1, bool>> expression)
            {
                this.exists.Where = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1> And(Expression<Func<Table1, bool>> expression)
            {
                if (this.exists.Where == null)
                    throw new Exception("please use Where method first;");

                this.exists.And = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2> Join<Table2>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.exists) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2> InnerJoin<Table2>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.exists) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2> LeftJoin<Table2>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2> RightJoin<Table2>(string @as)
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereExistsGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.exists) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.exists.Where == null && this.exists.And == null)
                    throw new Exception("please use Where or And method first;");

                this.where.Context.JoinOnWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        public struct SelectWhereExistsGrammar<Table1, Table2>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExists exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="exists"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExists exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2> On(Expression<Func<Table1, Table2, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2> And(Expression<Func<Table1, Table2, bool>> expression)
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
            public SelectWhereExistsGrammar<Table1, Table2, Table3> Join<Table3>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3> InnerJoin<Table3>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3> LeftJoin<Table3>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3> RightJoin<Table3>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3>(this.@as, JoinOption.RightJoin, this.exists)
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

                this.where.Context.JoinOnWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        public struct SelectWhereExistsGrammar<Table1, Table2, Table3>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExists exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExists exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3> On(Expression<Func<Table1, Table2, Table3, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3> And(Expression<Func<Table1, Table2, Table3, bool>> expression)
            {
                if (this.exists.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.exists.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3, Table4> Join<Table4>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.Join, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3, Table4> InnerJoin<Table4>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.InnerJoin, this.exists)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3, Table4> LeftJoin<Table4>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.LeftJoin, this.exists)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3, Table4> RightJoin<Table4>(string @as)
            {
                if (this.@as.Count != this.exists.Joins.Count + 1)
                    throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereExistsGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.RightJoin, this.exists)
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

                this.where.Context.JoinOnWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <typeparam name="Table4"></typeparam>
        public struct SelectWhereExistsGrammar<Table1, Table2, Table3, Table4>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereExists exists;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="exists"></param>
            /// <param name="joinOption"></param>
            public SelectWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExists exists) : this()
            {
                this.@as = @as;
                this.exists = exists;
                this.exists.Joins.Add(new Context.JoinInfo()
                {
                    JoinOption = joinOption,
                    AsName = @as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3), typeof(Table4) },
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3, Table4> On(Expression<Func<Table1, Table2, Table3, Table4, bool>> expression)
            {
                this.exists.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1, Table2, Table3, Table4> And(Expression<Func<Table1, Table2, Table3, Table4, bool>> expression)
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

                this.where.Context.JoinOnWhereExists(this.exists);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        public struct SelectWhereInGrammar<Table1>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly string @as;
            private readonly Context.WhereIn @in;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="option"></param>
            /// <param name="flag">只有n(not)和i(in)</param>
            public SelectWhereInGrammar(string @as, AndOrOption option, char flag) : this()
            {
                this.@as = @as;
                this.@in = new Context.WhereIn()
                {
                    AsName = @as,
                    AndOrOption = option,
                    NotIn = flag == 'n',
                    Types = new[] { typeof(Parameter), typeof(Table1) },
                    Joins = new List<Context.JoinInfo>(4),
                };
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1> Field(Expression<Func<Table1, bool>> expression)
            {
                this.@in.Field = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1> Where(Expression<Func<Table1, bool>> expression)
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.@in.Where = expression;
                return this;
            }

            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2> Join<Table2>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.@in) { where = this.where };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2> InnerJoin<Table2>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.@in) { where = this.where };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2> LeftJoin<Table2>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table2"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2> RightJoin<Table2>(string @as)
            {
                if (this.@as == @as)
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                return new SelectWhereInGrammar<Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.@in) { where = this.where };
            }

            /// <summary>
            /// then
            /// </summary>
            public SelectWhereGrammar ToWhere()
            {
                if (this.@in.Field == null)
                    throw new Exception("please use On Field first;");

                this.where.Context.JoinOnWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        public struct SelectWhereInGrammar<Table1, Table2>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereIn @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereIn @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2> On(Expression<Func<Table1, Table2, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2> And(Expression<Func<Table1, Table2, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3> Join<Table3>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3> InnerJoin<Table3>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3> LeftJoin<Table3>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table3"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3> RightJoin<Table3>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3>(this.@as, JoinOption.RightJoin, this.@in)
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

                this.where.Context.JoinOnWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        public struct SelectWhereInGrammar<Table1, Table2, Table3>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereIn @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereIn @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3> On(Expression<Func<Table1, Table2, Table3, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3> And(Expression<Func<Table1, Table2, Table3, bool>> expression)
            {
                if (this.@in.Joins.Last().On == null)
                    throw new Exception("please use On method first;");

                this.@in.Joins.Last().And = expression;
                return this;
            }


            /// <summary>
            /// join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3, Table4> Join<Table4>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.Join, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// inner join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3, Table4> InnerJoin<Table4>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.InnerJoin, this.@in)
                {
                    where = this.where,
                };
            }


            /// <summary>
            /// left join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3, Table4> LeftJoin<Table4>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.LeftJoin, this.@in)
                {
                    where = this.where,
                };
            }

            /// <summary>
            /// right join
            /// </summary>
            /// <typeparam name="Table4"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3, Table4> RightJoin<Table4>(string @as)
            {
                if (this.@as.Any(ta => ta == @as))
                    throw new Exception(string.Format("the alias name {0} is already exists", @as));

                this.@as.Add(@as);
                return new SelectWhereInGrammar<Table1, Table2, Table3, Table4>(this.@as, JoinOption.RightJoin, this.@in)
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

                this.where.Context.JoinOnWhereIn(this.@in);
                return this.where;
            }
        }

        /// <summary>
        /// select的join语法
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <typeparam name="Table2"></typeparam>
        /// <typeparam name="Table3"></typeparam>
        /// <typeparam name="Table4"></typeparam>
        public struct SelectWhereInGrammar<Table1, Table2, Table3, Table4>
        {
            internal SelectWhereGrammar where { get; set; }
            private readonly List<string> @as;
            private readonly Context.WhereIn @in;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="as"></param>
            /// <param name="joinOption"></param>
            /// <param name="in"></param>
            public SelectWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereIn @in) : this()
            {
                this.@as = @as;
                this.@in = @in;
                this.@in.Joins.Add(new Context.JoinInfo()
                {
                    AsName = this.@as.Last(),
                    Types = new[] { typeof(Parameter), typeof(Table1), typeof(Table2), typeof(Table3), typeof(Table4) },
                    JoinOption = joinOption
                });
            }

            /// <summary>
            /// on
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3, Table4> On(Expression<Func<Table1, Table2, Table3, Table4, bool>> expression)
            {
                this.@in.Joins.Last().On = expression;
                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1, Table2, Table3, Table4> And(Expression<Func<Table1, Table2, Table3, Table4, bool>> expression)
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

                this.where.Context.JoinOnWhereIn(this.@in);
                return this.where;
            }
        }

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
            /// 存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1> AndExists<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table1>(@as, AndOrOption.and, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1> AndNotExists<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table1>(@as, AndOrOption.and, 'n') { where = this };
            }
            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1> OrExists<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table1>(@as, AndOrOption.or, 'e') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereExistsGrammar<Table1> OrNotExists<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereExistsGrammar<Table1>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1> AndIn<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table1>(@as, AndOrOption.and, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1> AndNotIn<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table1>(@as, AndOrOption.and, 'n') { where = this };
            }
            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1> OrIn<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table1>(@as, AndOrOption.or, 'i') { where = this };
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="Table1"></typeparam>
            /// <param name="as"></param>
            /// <returns></returns>
            public SelectWhereInGrammar<Table1> OrNotIn<Table1>(string @as)
            {
                this.Context.CheckTableNameIsExists(@as);
                return new SelectWhereInGrammar<Table1>(@as, AndOrOption.or, 'n') { where = this };
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndNotExists(string expression)
            {
                this.Context.Where(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrNotExists(string expression)
            {
                this.Context.Where(AndOrOption.or, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndIn(string expression)
            {
                this.Context.Where(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar AndNotIn(string expression)
            {
                this.Context.Where(AndOrOption.and, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrIn(string expression)
            {
                this.Context.Where(AndOrOption.or, expression);
                return this;
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
            public SelectWhereGrammar OrNotIn(string expression)
            {
                this.Context.Where(AndOrOption.or, expression);
                return this;
            }

            /// <summary>
            /// 获取结果
            /// </summary>
            public IEnumerable<Table> GetResults(PagedSearch paged)
            {
                return this.Context.GetResults(paged);
            }
        }

        #endregion
    }
}
