using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// update操作语法
    /// </summary>
    public struct UpdateGrammar<Parameter>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal UpdateContext<Parameter> Context { get; set; }

        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        internal UpdateGrammar<Parameter> StartSetColumn()
        {
            this.Context.StartSetColumn();
            return this;
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColumn<TMember>(Expression<Func<Parameter, TMember>> expression)
        {
            this.Context.Set<TMember>(expression);
            return this;
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColumnWithFunc<TMember>(Expression<Func<Parameter, TMember>> expression, string value)
        {
            this.Context.SetFunc<TMember>(expression, value);
            return this;
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColumnWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value)
        {
            this.Context.SetValue<TMember>(expression, value);
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public int GetResult()
        {
            return this.Context.GetResult();
        }

        /// <summary>
        /// where
        /// </summary>
        public UpdateWhereGrammar<Parameter> Where()
        {
            this.Context.Where();
            return new UpdateWhereGrammar<Parameter>() { Context = this.Context };
        }

        /// <summary>
        /// where
        /// </summary>
        public UpdateWhereGrammar<Parameter> Where(Expression<Func<Parameter, object>> expression)
        {
            this.Context.Where(expression);
            return new UpdateWhereGrammar<Parameter>() { Context = this.Context };
        }
    }

    /// <summary>
    /// update的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    public struct UpdateJoinGrammar<Parameter, Table1>
    {
        internal UpdateGrammar<Parameter> update { get; set; }
        private readonly string @as;
        private readonly JoinOption option;
        private readonly List<Context.JoinInfo> joins;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        public UpdateJoinGrammar(string @as, JoinOption option) : this()
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
        public UpdateJoinGrammar<Parameter, Table1> On(Expression<Func<Parameter, Table1, bool>> expression)
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
        public UpdateJoinGrammar<Parameter, Table1> And(Expression<Func<Parameter, Table1, bool>> expression)
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
        public UpdateJoinGrammar<Parameter, Table1, Table2> Join<Table2>(string @as)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new UpdateJoinGrammar<Parameter, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.joins) { update = this.update };
        }


        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2> InnerJoin<Table2>(string @as)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new UpdateJoinGrammar<Parameter, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.joins) { update = this.update };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2> LeftJoin<Table2>(string @as)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new UpdateJoinGrammar<Parameter, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.joins) { update = this.update };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2> RightJoin<Table2>(string @as)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new UpdateJoinGrammar<Parameter, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.joins) { update = this.update };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateGrammar<Parameter> ToUpdate()
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.update.StartSetColumn();
            this.update.Context.JoinOnUpdate(this.joins);
            return this.update;
        }
    }

    /// <summary>
    /// update的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    public struct UpdateJoinGrammar<Parameter, Table1, Table2>
    {
        internal UpdateGrammar<Parameter> update { get; set; }
        private readonly List<string> @as;
        private readonly JoinOption option;
        private readonly List<Context.JoinInfo> joins;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="joins"></param>
        public UpdateJoinGrammar(List<string> @as, JoinOption option, List<Context.JoinInfo> joins) : this()
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
        public UpdateJoinGrammar<Parameter, Table1, Table2> On(Expression<Func<Parameter, Table1, Table2, bool>> expression)
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
        public UpdateJoinGrammar<Parameter, Table1, Table2> And(Expression<Func<Parameter, Table1, Table2, bool>> expression)
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
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3> Join<Table3>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3>(this.@as, JoinOption.Join, this.joins) { update = this.update };
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3> InnerJoin<Table3>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3>(this.@as, JoinOption.InnerJoin, this.joins) { update = this.update };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3> LeftJoin<Table3>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3>(this.@as, JoinOption.LeftJoin, this.joins) { update = this.update };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3> RightJoin<Table3>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3>(this.@as, JoinOption.RightJoin, this.joins) { update = this.update };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateGrammar<Parameter> ToUpdate()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.update.StartSetColumn();
            this.update.Context.JoinOnUpdate(this.joins);
            return this.update;
        }
    }

    /// <summary>
    /// update的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    /// <typeparam name="Table3"></typeparam>
    public struct UpdateJoinGrammar<Parameter, Table1, Table2, Table3>
    {
        internal UpdateGrammar<Parameter> update { get; set; }
        private readonly List<string> @as;
        private readonly JoinOption option;
        private readonly List<Context.JoinInfo> joins;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="joins"></param>
        public UpdateJoinGrammar(List<string> @as, JoinOption option, List<Context.JoinInfo> joins) : this()
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
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3> On(Expression<Func<Parameter, Table1, Table2, Table3, bool>> expression)
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
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3> And(Expression<Func<Parameter, Table1, Table2, Table3, bool>> expression)
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
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4> Join<Table4>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4>(this.@as, JoinOption.Join, this.joins) { update = this.update };
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4> InnerJoin<Table4>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4>(this.@as, JoinOption.InnerJoin, this.joins) { update = this.update };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4> LeftJoin<Table4>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4>(this.@as, JoinOption.LeftJoin, this.joins) { update = this.update };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4> RightJoin<Table4>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4>(this.@as, JoinOption.RightJoin, this.joins) { update = this.update };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateGrammar<Parameter> ToUpdate()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.update.StartSetColumn();
            this.update.Context.JoinOnUpdate(this.joins);
            return this.update;
        }
    }

    /// <summary>
    /// update的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    /// <typeparam name="Table3"></typeparam>
    /// <typeparam name="Table4"></typeparam>
    public struct UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4>
    {
        internal UpdateGrammar<Parameter> update { get; set; }
        private readonly List<string> @as;
        private readonly JoinOption option;
        private readonly List<Context.JoinInfo> joins;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="joins"></param>
        public UpdateJoinGrammar(List<string> @as, JoinOption option, List<Context.JoinInfo> joins) : this()
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
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4> On(Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> expression)
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
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4> And(Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.joins.Last().And = expression;
            return this;
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateGrammar<Parameter> ToUpdate()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.update.StartSetColumn();
            this.update.Context.JoinOnUpdate(this.joins);
            return this.update;
        }
    }

    /// <summary>
    /// update的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    public struct UpdateWhereExistsGrammar<Parameter, Table1>
    {
        internal UpdateWhereGrammar<Parameter> where { get; set; }
        private readonly string @as;
        private readonly Context.WhereExistsInfo exists;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="flag">只有n(not)和e(exists)</param>
        public UpdateWhereExistsGrammar(string @as, AndOrOption option, char flag) : this()
        {
            this.@as = @as;
            this.exists = new Context.WhereExistsInfo()
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
        public UpdateWhereExistsGrammar<Parameter, Table1> Where(Expression<Func<Parameter, Table1, bool>> expression)
        {
            this.exists.Where = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1> And(Expression<Func<Parameter, Table1, bool>> expression)
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
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2> Join<Table2>(string @as)
        {
            if (this.exists.Where == null && this.exists.And == null)
                throw new Exception("please use Where or And method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.exists) { where = this.where };
        }


        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2> InnerJoin<Table2>(string @as)
        {
            if (this.exists.Where == null && this.exists.And == null)
                throw new Exception("please use Where or And method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.exists) { where = this.where };
        }


        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2> LeftJoin<Table2>(string @as)
        {
            if (this.exists.Where == null && this.exists.And == null)
                throw new Exception("please use Where or And method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.exists) { where = this.where };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2> RightJoin<Table2>(string @as)
        {
            if (this.exists.Where == null && this.exists.And == null)
                throw new Exception("please use Where or And method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.exists) { where = this.where };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
        {
            if (this.exists.Where == null && this.exists.And == null)
                throw new Exception("please use Where or And method first;");

            this.where.Context.JoinOnWhereExists(this.exists);
            return this.where;
        }
    }

    /// <summary>
    /// update的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    public struct UpdateWhereExistsGrammar<Parameter, Table1, Table2>
    {
        internal UpdateWhereGrammar<Parameter> where { get; set; }
        private readonly List<string> @as;
        private readonly Context.WhereExistsInfo exists;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="joinOption"></param>
        /// <param name="exists"></param>
        public UpdateWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
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
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2> On(Expression<Func<Parameter, Table1, Table2, bool>> expression)
        {
            this.exists.Joins.Last().On = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2> And(Expression<Func<Parameter, Table1, Table2, bool>> expression)
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
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3> Join<Table3>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3>(this.@as, JoinOption.Join, this.exists)
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
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3> InnerJoin<Table3>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3>(this.@as, JoinOption.InnerJoin, this.exists)
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
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3> LeftJoin<Table3>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3>(this.@as, JoinOption.LeftJoin, this.exists)
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
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3> RightJoin<Table3>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3>(this.@as, JoinOption.RightJoin, this.exists)
            {
                where = this.where,
            };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
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
    /// update的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    /// <typeparam name="Table3"></typeparam>
    public struct UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3>
    {
        internal UpdateWhereGrammar<Parameter> where { get; set; }
        private readonly List<string> @as;
        private readonly Context.WhereExistsInfo exists;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="exists"></param>
        /// <param name="joinOption"></param>
        public UpdateWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
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
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3> On(Expression<Func<Parameter, Table1, Table2, Table3, bool>> expression)
        {
            this.exists.Joins.Last().On = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3> And(Expression<Func<Parameter, Table1, Table2, Table3, bool>> expression)
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
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4> Join<Table4>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4>(this.@as, JoinOption.Join, this.exists)
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
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4> InnerJoin<Table4>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4>(this.@as, JoinOption.InnerJoin, this.exists)
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
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4> LeftJoin<Table4>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4>(this.@as, JoinOption.LeftJoin, this.exists)
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
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4> RightJoin<Table4>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4>(this.@as, JoinOption.RightJoin, this.exists)
            {
                where = this.where,
            };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
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
    /// update的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    /// <typeparam name="Table3"></typeparam>
    /// <typeparam name="Table4"></typeparam>
    public struct UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4>
    {
        internal UpdateWhereGrammar<Parameter> where { get; set; }
        private readonly List<string> @as;
        private readonly Context.WhereExistsInfo exists;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="exists"></param>
        /// <param name="joinOption"></param>
        public UpdateWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
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
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4> On(Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> expression)
        {
            this.exists.Joins.Last().On = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4> And(Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> expression)
        {
            if (this.exists.Joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.exists.Joins.Last().And = expression;
            return this;
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
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
    /// update的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    public struct UpdateWhereInGrammar<Parameter, Table1>
    {
        internal UpdateWhereGrammar<Parameter> where { get; set; }
        private readonly string @as;
        private readonly Context.WhereInInfo @in;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="flag">只有n(not)和i(in)</param>
        public UpdateWhereInGrammar(string @as, AndOrOption option, char flag) : this()
        {
            this.@as = @as;
            this.@in = new Context.WhereInInfo()
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
        public UpdateWhereInGrammar<Parameter, Table1> Field(Expression<Func<Parameter, Table1, bool>> expression)
        {
            this.@in.Field = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1> Where(Expression<Func<Parameter, Table1, bool>> expression)
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
        public UpdateWhereInGrammar<Parameter, Table1, Table2> Join<Table2>(string @as)
        {
            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new UpdateWhereInGrammar<Parameter, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.@in) { where = this.where };
        }


        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1, Table2> InnerJoin<Table2>(string @as)
        {
            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new UpdateWhereInGrammar<Parameter, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.@in) { where = this.where };
        }


        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1, Table2> LeftJoin<Table2>(string @as)
        {
            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new UpdateWhereInGrammar<Parameter, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.@in) { where = this.where };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1, Table2> RightJoin<Table2>(string @as)
        {
            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new UpdateWhereInGrammar<Parameter, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.@in) { where = this.where };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
        {
            if (this.@in.Field == null)
                throw new Exception("please use On Field first;");

            this.where.Context.JoinOnWhereIn(this.@in);
            return this.where;
        }
    }

    /// <summary>
    /// update的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    public struct UpdateWhereInGrammar<Parameter, Table1, Table2>
    {
        internal UpdateWhereGrammar<Parameter> where { get; set; }
        private readonly List<string> @as;
        private readonly Context.WhereInInfo @in;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="joinOption"></param>
        /// <param name="in"></param>
        public UpdateWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
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
        public UpdateWhereInGrammar<Parameter, Table1, Table2> On(Expression<Func<Parameter, Table1, Table2, bool>> expression)
        {
            this.@in.Joins.Last().On = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1, Table2> And(Expression<Func<Parameter, Table1, Table2, bool>> expression)
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
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3> Join<Table3>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3>(this.@as, JoinOption.Join, this.@in)
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
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3> InnerJoin<Table3>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3>(this.@as, JoinOption.InnerJoin, this.@in)
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
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3> LeftJoin<Table3>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3>(this.@as, JoinOption.LeftJoin, this.@in)
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
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3> RightJoin<Table3>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3>(this.@as, JoinOption.RightJoin, this.@in)
            {
                where = this.where,
            };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
        {
            if (this.@in.Joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.where.Context.JoinOnWhereIn(this.@in);
            return this.where;
        }
    }

    /// <summary>
    /// update的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    /// <typeparam name="Table3"></typeparam>
    public struct UpdateWhereInGrammar<Parameter, Table1, Table2, Table3>
    {
        internal UpdateWhereGrammar<Parameter> where { get; set; }
        private readonly List<string> @as;
        private readonly Context.WhereInInfo @in;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="joinOption"></param>
        /// <param name="in"></param>
        public UpdateWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
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
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3> On(Expression<Func<Parameter, Table1, Table2, Table3, bool>> expression)
        {
            this.@in.Joins.Last().On = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3> And(Expression<Func<Parameter, Table1, Table2, Table3, bool>> expression)
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
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4> Join<Table4>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4>(this.@as, JoinOption.Join, this.@in)
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
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4> InnerJoin<Table4>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4>(this.@as, JoinOption.InnerJoin, this.@in)
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
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4> LeftJoin<Table4>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4>(this.@as, JoinOption.LeftJoin, this.@in)
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
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4> RightJoin<Table4>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4>(this.@as, JoinOption.RightJoin, this.@in)
            {
                where = this.where,
            };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
        {
            if (this.@in.Joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.where.Context.JoinOnWhereIn(this.@in);
            return this.where;
        }
    }

    /// <summary>
    /// update的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    /// <typeparam name="Table3"></typeparam>
    /// <typeparam name="Table4"></typeparam>
    public struct UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4>
    {
        internal UpdateWhereGrammar<Parameter> where { get; set; }
        private readonly List<string> @as;
        private readonly Context.WhereInInfo @in;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="joinOption"></param>
        /// <param name="in"></param>
        public UpdateWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
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
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4> On(Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> expression)
        {
            this.@in.Joins.Last().On = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4> And(Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> expression)
        {
            if (this.@in.Joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.@in.Joins.Last().And = expression;
            return this;
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
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
    /// <typeparam name="Parameter">查询参数</typeparam>
    public struct UpdateWhereGrammar<Parameter>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal UpdateContext<Parameter> Context { get; set; }

        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table> AndExists<Table>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateWhereExistsGrammar<Parameter, Table>(@as, AndOrOption.and, 'e') { where = this };
        }

        /// <summary>
        /// 不存在
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table> AndNotExists<Table>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateWhereExistsGrammar<Parameter, Table>(@as, AndOrOption.and, 'n') { where = this };
        }
        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table> OrExists<Table>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateWhereExistsGrammar<Parameter, Table>(@as, AndOrOption.or, 'e') { where = this };
        }

        /// <summary>
        /// 不存在
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table> OrNotExists<Table>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateWhereExistsGrammar<Parameter, Table>(@as, AndOrOption.or, 'n') { where = this };
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table> AndIn<Table>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateWhereInGrammar<Parameter, Table>(@as, AndOrOption.and, 'i') { where = this };
        }

        /// <summary>
        /// 不存在
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table> AndNotIn<Table>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateWhereInGrammar<Parameter, Table>(@as, AndOrOption.and, 'n') { where = this };
        }
        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table> OrIn<Table>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateWhereInGrammar<Parameter, Table>(@as, AndOrOption.or, 'i') { where = this };
        }

        /// <summary>
        /// 不存在
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table> OrNotIn<Table>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new UpdateWhereInGrammar<Parameter, Table>(@as, AndOrOption.or, 'n') { where = this };
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
        public UpdateWhereGrammar<Parameter> AndNotExists(string expression)
        {
            this.Context.Where(AndOrOption.and, expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
        public UpdateWhereGrammar<Parameter> OrNotExists(string expression)
        {
            this.Context.Where(AndOrOption.or, expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
        public UpdateWhereGrammar<Parameter> AndIn(string expression)
        {
            this.Context.Where(AndOrOption.and, expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
        public UpdateWhereGrammar<Parameter> AndNotIn(string expression)
        {
            this.Context.Where(AndOrOption.and, expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
        public UpdateWhereGrammar<Parameter> OrIn(string expression)
        {
            this.Context.Where(AndOrOption.or, expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
        public UpdateWhereGrammar<Parameter> OrNotIn(string expression)
        {
            this.Context.Where(AndOrOption.or, expression);
            return this;
        }

        /// <summary>
        /// 字符串
        /// </summary>
        /// <param name="sql">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
        public UpdateWhereGrammar<Parameter> Append(string sql)
        {
            this.Context.Append(sql);
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public int GetResult()
        {
            return this.Context.GetResult();
        }
    }
}
