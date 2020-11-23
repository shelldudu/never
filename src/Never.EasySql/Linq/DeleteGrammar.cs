using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// delete语法
    /// </summary>
    public struct DeleteGrammar<Parameter, Table>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal DeleteContext<Parameter, Table> Context { get; set; }


        /// <summary>
        /// 入口
        /// </summary>
        /// <returns></returns>
        internal DeleteGrammar<Parameter, Table> StartDeleteRecord()
        {
            this.Context.StartEntrance();
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public int GetResult()
        {
            return Context.GetResult();
        }

        /// <summary>
        /// where
        /// </summary>
        public DeleteWhereGrammar<Parameter, Table> Where()
        {
            this.Context.Where();
            return new DeleteWhereGrammar<Parameter, Table>() { Context = this.Context };
        }

        /// <summary>
        /// where
        /// </summary>
        public DeleteWhereGrammar<Parameter, Table> Where(Expression<Func<Parameter, Table, bool>> expression)
        {
            this.Context.Where(expression);
            return new DeleteWhereGrammar<Parameter, Table>() { Context = this.Context };
        }
    }

    /// <summary>
    /// Delete的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    public struct DeleteJoinGrammar<Parameter, Table, Table1>
    {
        internal DeleteGrammar<Parameter, Table> delete { get; set; }
        private readonly string @as;
        private readonly JoinOption option;
        private readonly List<Context.JoinInfo> joins;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        public DeleteJoinGrammar(string @as, JoinOption option) : this()
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
        public DeleteJoinGrammar<Parameter, Table, Table1> On(Expression<Func<Parameter, Table, Table1, bool>> expression)
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
        public DeleteJoinGrammar<Parameter, Table, Table1> And(Expression<Func<Parameter, Table, Table1, bool>> expression)
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
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2> Join<Table2>(string @as)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new DeleteJoinGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.joins) { delete = this.delete };
        }


        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2> InnerJoin<Table2>(string @as)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new DeleteJoinGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.joins) { delete = this.delete };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2> LeftJoin<Table2>(string @as)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new DeleteJoinGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.joins) { delete = this.delete };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2> RightJoin<Table2>(string @as)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new DeleteJoinGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.joins) { delete = this.delete };
        }

        /// <summary>
        /// then
        /// </summary>
        public DeleteGrammar<Parameter, Table> ToDelete()
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.delete.StartDeleteRecord();
            this.delete.Context.JoinOnDelete(this.joins);
            return this.delete;
        }
    }

    /// <summary>
    /// Delete的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    public struct DeleteJoinGrammar<Parameter, Table, Table1, Table2>
    {
        internal DeleteGrammar<Parameter, Table> delete { get; set; }
        private readonly List<string> @as;
        private readonly JoinOption option;
        private readonly List<Context.JoinInfo> joins;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="joins"></param>
        public DeleteJoinGrammar(List<string> @as, JoinOption option, List<Context.JoinInfo> joins) : this()
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
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2> On(Expression<Func<Parameter, Table, Table1, Table2, bool>> expression)
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
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2> And(Expression<Func<Parameter, Table, Table1, Table2, bool>> expression)
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
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3> Join<Table3>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.Join, this.joins) { delete = this.delete };
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3> InnerJoin<Table3>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.InnerJoin, this.joins) { delete = this.delete };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3> LeftJoin<Table3>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.LeftJoin, this.joins) { delete = this.delete };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3> RightJoin<Table3>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.RightJoin, this.joins) { delete = this.delete };
        }

        /// <summary>
        /// then
        /// </summary>
        public DeleteGrammar<Parameter, Table> ToDelete()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.delete.StartDeleteRecord();
            this.delete.Context.JoinOnDelete(this.joins);
            return this.delete;
        }
    }

    /// <summary>
    /// Delete的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    /// <typeparam name="Table3"></typeparam>
    public struct DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3>
    {
        internal DeleteGrammar<Parameter, Table> delete { get; set; }
        private readonly List<string> @as;
        private readonly JoinOption option;
        private readonly List<Context.JoinInfo> joins;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="joins"></param>
        public DeleteJoinGrammar(List<string> @as, JoinOption option, List<Context.JoinInfo> joins) : this()
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
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3> On(Expression<Func<Parameter, Table, Table1, Table2, Table3, bool>> expression)
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
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3> And(Expression<Func<Parameter, Table, Table1, Table2, Table3, bool>> expression)
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
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> Join<Table4>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.Join, this.joins) { delete = this.delete };
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> InnerJoin<Table4>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.InnerJoin, this.joins) { delete = this.delete };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> LeftJoin<Table4>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.LeftJoin, this.joins) { delete = this.delete };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> RightJoin<Table4>(string @as)
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.RightJoin, this.joins) { delete = this.delete };
        }

        /// <summary>
        /// then
        /// </summary>
        public DeleteGrammar<Parameter, Table> ToDelete()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.delete.StartDeleteRecord();
            this.delete.Context.JoinOnDelete(this.joins);
            return this.delete;
        }
    }

    /// <summary>
    /// Delete的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    /// <typeparam name="Table3"></typeparam>
    /// <typeparam name="Table4"></typeparam>
    public struct DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4>
    {
        internal DeleteGrammar<Parameter, Table> delete { get; set; }
        private readonly List<string> @as;
        private readonly JoinOption option;
        private readonly List<Context.JoinInfo> joins;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="joins"></param>
        public DeleteJoinGrammar(List<string> @as, JoinOption option, List<Context.JoinInfo> joins) : this()
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
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> On(Expression<Func<Parameter, Table, Table1, Table2, Table3, Table4, bool>> expression)
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
        public DeleteJoinGrammar<Parameter, Table, Table1, Table2, Table3, Table4> And(Expression<Func<Parameter, Table, Table1, Table2, Table3, Table4, bool>> expression)
        {
            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.joins.Last().And = expression;
            return this;
        }

        /// <summary>
        /// then
        /// </summary>
        public DeleteGrammar<Parameter, Table> ToDelete()
        {
            if (this.@as.Count != this.joins.Count)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.delete.StartDeleteRecord();
            this.delete.Context.JoinOnDelete(this.joins);
            return this.delete;
        }
    }

    /// <summary>
    /// Delete的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    public struct DeleteWhereExistsGrammar<Parameter, Table, Table1>
    {
        internal DeleteWhereGrammar<Parameter, Table> where { get; set; }
        private readonly string @as;
        private readonly Context.WhereExistsInfo exists;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="flag">只有n(not)和e(exists)</param>
        public DeleteWhereExistsGrammar(string @as, AndOrOption option, char flag) : this()
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
        public DeleteWhereExistsGrammar<Parameter, Table, Table1> Where(Expression<Func<Parameter, Table, Table1, bool>> expression)
        {
            this.exists.Where = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DeleteWhereExistsGrammar<Parameter, Table, Table1> And(Expression<Func<Parameter, Table, Table1, bool>> expression)
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
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2> Join<Table2>(string @as)
        {
            if (this.exists.Where == null && this.exists.And == null)
                throw new Exception("please use Where or And method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.exists) { where = this.where };
        }


        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2> InnerJoin<Table2>(string @as)
        {
            if (this.exists.Where == null && this.exists.And == null)
                throw new Exception("please use Where or And method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.exists) { where = this.where };
        }


        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2> LeftJoin<Table2>(string @as)
        {
            if (this.exists.Where == null && this.exists.And == null)
                throw new Exception("please use Where or And method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.exists) { where = this.where };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2> RightJoin<Table2>(string @as)
        {
            if (this.exists.Where == null && this.exists.And == null)
                throw new Exception("please use Where or And method first;");

            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.exists) { where = this.where };
        }

        /// <summary>
        /// then
        /// </summary>
        public DeleteWhereGrammar<Parameter, Table> ToWhere()
        {
            if (this.exists.Where == null && this.exists.And == null)
                throw new Exception("please use Where or And method first;");

            this.where.Context.JoinOnWhereExists(this.exists);
            return this.where;
        }
    }

    /// <summary>
    /// Delete的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    public struct DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2>
    {
        internal DeleteWhereGrammar<Parameter, Table> where { get; set; }
        private readonly List<string> @as;
        private readonly Context.WhereExistsInfo exists;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="joinOption"></param>
        /// <param name="exists"></param>
        public DeleteWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
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
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2> On(Expression<Func<Parameter, Table, Table1, Table2, bool>> expression)
        {
            this.exists.Joins.Last().On = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2> And(Expression<Func<Parameter, Table, Table1, Table2, bool>> expression)
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
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3> Join<Table3>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.Join, this.exists)
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
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3> InnerJoin<Table3>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.InnerJoin, this.exists)
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
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3> LeftJoin<Table3>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.LeftJoin, this.exists)
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
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3> RightJoin<Table3>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.RightJoin, this.exists)
            {
                where = this.where,
            };
        }

        /// <summary>
        /// then
        /// </summary>
        public DeleteWhereGrammar<Parameter, Table> ToWhere()
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
    /// Delete的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    /// <typeparam name="Table3"></typeparam>
    public struct DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3>
    {
        internal DeleteWhereGrammar<Parameter, Table> where { get; set; }
        private readonly List<string> @as;
        private readonly Context.WhereExistsInfo exists;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="exists"></param>
        /// <param name="joinOption"></param>
        public DeleteWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
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
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3> On(Expression<Func<Parameter, Table, Table1, Table2, Table3, bool>> expression)
        {
            this.exists.Joins.Last().On = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3> And(Expression<Func<Parameter, Table, Table1, Table2, Table3, bool>> expression)
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
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3, Table4> Join<Table4>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.Join, this.exists)
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
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3, Table4> InnerJoin<Table4>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.InnerJoin, this.exists)
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
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3, Table4> LeftJoin<Table4>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.LeftJoin, this.exists)
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
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3, Table4> RightJoin<Table4>(string @as)
        {
            if (this.@as.Count != this.exists.Joins.Count + 1)
                throw new Exception(string.Format("please use {0} On method first;", this.@as.Last()));

            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.RightJoin, this.exists)
            {
                where = this.where,
            };
        }

        /// <summary>
        /// then
        /// </summary>
        public DeleteWhereGrammar<Parameter, Table> ToWhere()
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
    /// Delete的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    /// <typeparam name="Table3"></typeparam>
    /// <typeparam name="Table4"></typeparam>
    public struct DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3, Table4>
    {
        internal DeleteWhereGrammar<Parameter, Table> where { get; set; }
        private readonly List<string> @as;
        private readonly Context.WhereExistsInfo exists;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="exists"></param>
        /// <param name="joinOption"></param>
        public DeleteWhereExistsGrammar(List<string> @as, JoinOption joinOption, Context.WhereExistsInfo exists) : this()
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
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3, Table4> On(Expression<Func<Parameter, Table, Table1, Table2, Table3, Table4, bool>> expression)
        {
            this.exists.Joins.Last().On = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DeleteWhereExistsGrammar<Parameter, Table, Table1, Table2, Table3, Table4> And(Expression<Func<Parameter, Table, Table1, Table2, Table3, Table4, bool>> expression)
        {
            if (this.exists.Joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.exists.Joins.Last().And = expression;
            return this;
        }

        /// <summary>
        /// then
        /// </summary>
        public DeleteWhereGrammar<Parameter, Table> ToWhere()
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
    /// Delete的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    public struct DeleteWhereInGrammar<Parameter, Table, Table1>
    {
        internal DeleteWhereGrammar<Parameter, Table> where { get; set; }
        private readonly string @as;
        private readonly Context.WhereInInfo @in;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="flag">只有n(not)和i(in)</param>
        public DeleteWhereInGrammar(string @as, AndOrOption option, char flag) : this()
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
        public DeleteWhereInGrammar<Parameter, Table, Table1> Field(Expression<Func<Parameter, Table, Table1, bool>> expression)
        {
            this.@in.Field = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DeleteWhereInGrammar<Parameter, Table, Table1> Where(Expression<Func<Parameter, Table, Table1, bool>> expression)
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
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2> Join<Table2>(string @as)
        {
            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new DeleteWhereInGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.Join, this.@in) { where = this.where };
        }


        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2> InnerJoin<Table2>(string @as)
        {
            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new DeleteWhereInGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.InnerJoin, this.@in) { where = this.where };
        }


        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2> LeftJoin<Table2>(string @as)
        {
            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new DeleteWhereInGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.LeftJoin, this.@in) { where = this.where };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2> RightJoin<Table2>(string @as)
        {
            if (this.@as == @as)
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            return new DeleteWhereInGrammar<Parameter, Table, Table1, Table2>(new List<string>(4) { this.@as, @as }, JoinOption.RightJoin, this.@in) { where = this.where };
        }

        /// <summary>
        /// then
        /// </summary>
        public DeleteWhereGrammar<Parameter, Table> ToWhere()
        {
            if (this.@in.Field == null)
                throw new Exception("please use On Field first;");

            this.where.Context.JoinOnWhereIn(this.@in);
            return this.where;
        }
    }

    /// <summary>
    /// Delete的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    public struct DeleteWhereInGrammar<Parameter, Table, Table1, Table2>
    {
        internal DeleteWhereGrammar<Parameter, Table> where { get; set; }
        private readonly List<string> @as;
        private readonly Context.WhereInInfo @in;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="joinOption"></param>
        /// <param name="in"></param>
        public DeleteWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
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
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2> On(Expression<Func<Parameter, Table, Table1, Table2, bool>> expression)
        {
            this.@in.Joins.Last().On = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2> And(Expression<Func<Parameter, Table, Table1, Table2, bool>> expression)
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
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3> Join<Table3>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.Join, this.@in)
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
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3> InnerJoin<Table3>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.InnerJoin, this.@in)
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
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3> LeftJoin<Table3>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.LeftJoin, this.@in)
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
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3> RightJoin<Table3>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3>(this.@as, JoinOption.RightJoin, this.@in)
            {
                where = this.where,
            };
        }

        /// <summary>
        /// then
        /// </summary>
        public DeleteWhereGrammar<Parameter, Table> ToWhere()
        {
            if (this.@in.Joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.where.Context.JoinOnWhereIn(this.@in);
            return this.where;
        }
    }

    /// <summary>
    /// Delete的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    /// <typeparam name="Table3"></typeparam>
    public struct DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3>
    {
        internal DeleteWhereGrammar<Parameter, Table> where { get; set; }
        private readonly List<string> @as;
        private readonly Context.WhereInInfo @in;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="joinOption"></param>
        /// <param name="in"></param>
        public DeleteWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
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
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3> On(Expression<Func<Parameter, Table, Table1, Table2, Table3, bool>> expression)
        {
            this.@in.Joins.Last().On = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3> And(Expression<Func<Parameter, Table, Table1, Table2, Table3, bool>> expression)
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
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3, Table4> Join<Table4>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.Join, this.@in)
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
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3, Table4> InnerJoin<Table4>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.InnerJoin, this.@in)
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
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3, Table4> LeftJoin<Table4>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.LeftJoin, this.@in)
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
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3, Table4> RightJoin<Table4>(string @as)
        {
            if (this.@as.Any(ta => ta == @as))
                throw new Exception(string.Format("the alias name {0} is already exists", @as));

            this.@as.Add(@as);
            return new DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3, Table4>(this.@as, JoinOption.RightJoin, this.@in)
            {
                where = this.where,
            };
        }

        /// <summary>
        /// then
        /// </summary>
        public DeleteWhereGrammar<Parameter, Table> ToWhere()
        {
            if (this.@in.Joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.where.Context.JoinOnWhereIn(this.@in);
            return this.where;
        }
    }

    /// <summary>
    /// Delete的join语法
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    /// <typeparam name="Table"></typeparam>
    /// <typeparam name="Table1"></typeparam>
    /// <typeparam name="Table2"></typeparam>
    /// <typeparam name="Table3"></typeparam>
    /// <typeparam name="Table4"></typeparam>
    public struct DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3, Table4>
    {
        internal DeleteWhereGrammar<Parameter, Table> where { get; set; }
        private readonly List<string> @as;
        private readonly Context.WhereInInfo @in;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="joinOption"></param>
        /// <param name="in"></param>
        public DeleteWhereInGrammar(List<string> @as, JoinOption joinOption, Context.WhereInInfo @in) : this()
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
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3, Table4> On(Expression<Func<Parameter, Table, Table1, Table2, Table3, Table4, bool>> expression)
        {
            this.@in.Joins.Last().On = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DeleteWhereInGrammar<Parameter, Table, Table1, Table2, Table3, Table4> And(Expression<Func<Parameter, Table, Table1, Table2, Table3, Table4, bool>> expression)
        {
            if (this.@in.Joins.Last().On == null)
                throw new Exception("please use On method first;");

            this.@in.Joins.Last().And = expression;
            return this;
        }

        /// <summary>
        /// then
        /// </summary>
        public DeleteWhereGrammar<Parameter, Table> ToWhere()
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
    /// <typeparam name="Table">目标表</typeparam>
    public struct DeleteWhereGrammar<Parameter, Table>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal DeleteContext<Parameter, Table> Context { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DeleteWhereGrammar<Parameter, Table> And(Expression<Func<Parameter, Table, bool>> expression)
        {
            this.Context.Where(expression);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DeleteWhereGrammar<Parameter, Table> Or(Expression<Func<Parameter, Table, bool>> expression)
        {
            this.Context.Where(expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteWhereExistsGrammar<Parameter, Table, Table1> AndExists<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new DeleteWhereExistsGrammar<Parameter, Table, Table1>(@as, AndOrOption.and, 'e') { where = this };
        }

        /// <summary>
        /// 不存在
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteWhereExistsGrammar<Parameter, Table, Table1> AndNotExists<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new DeleteWhereExistsGrammar<Parameter, Table, Table1>(@as, AndOrOption.and, 'n') { where = this };
        }
        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteWhereExistsGrammar<Parameter, Table, Table1> OrExists<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new DeleteWhereExistsGrammar<Parameter, Table, Table1>(@as, AndOrOption.or, 'e') { where = this };
        }

        /// <summary>
        /// 不存在
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteWhereExistsGrammar<Parameter, Table, Table1> OrNotExists<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new DeleteWhereExistsGrammar<Parameter, Table, Table1>(@as, AndOrOption.or, 'n') { where = this };
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteWhereInGrammar<Parameter, Table, Table1> AndIn<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new DeleteWhereInGrammar<Parameter, Table, Table1>(@as, AndOrOption.and, 'i') { where = this };
        }

        /// <summary>
        /// 不存在
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteWhereInGrammar<Parameter, Table, Table1> AndNotIn<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new DeleteWhereInGrammar<Parameter, Table, Table1>(@as, AndOrOption.and, 'n') { where = this };
        }
        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteWhereInGrammar<Parameter, Table, Table1> OrIn<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new DeleteWhereInGrammar<Parameter, Table, Table1>(@as, AndOrOption.or, 'i') { where = this };
        }

        /// <summary>
        /// 不存在
        /// </summary>
        /// <typeparam name="Table1"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public DeleteWhereInGrammar<Parameter, Table, Table1> OrNotIn<Table1>(string @as)
        {
            this.Context.CheckTableNameIsExists(@as);
            return new DeleteWhereInGrammar<Parameter, Table, Table1>(@as, AndOrOption.or, 'n') { where = this };
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
        public DeleteWhereGrammar<Parameter, Table> AndNotExists(string expression)
        {
            this.Context.Where(AndOrOption.and, expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
        public DeleteWhereGrammar<Parameter, Table> OrNotExists(string expression)
        {
            this.Context.Where(AndOrOption.or, expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
        public DeleteWhereGrammar<Parameter, Table> AndIn(string expression)
        {
            this.Context.Where(AndOrOption.and, expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
        public DeleteWhereGrammar<Parameter, Table> AndNotIn(string expression)
        {
            this.Context.Where(AndOrOption.and, expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
        public DeleteWhereGrammar<Parameter, Table> OrIn(string expression)
        {
            this.Context.Where(AndOrOption.or, expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
        public DeleteWhereGrammar<Parameter, Table> OrNotIn(string expression)
        {
            this.Context.Where(AndOrOption.or, expression);
            return this;
        }

        /// <summary>
        /// 字符串
        /// </summary>
        /// <param name="sql">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
        public DeleteWhereGrammar<Parameter, Table> Append(string sql)
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
