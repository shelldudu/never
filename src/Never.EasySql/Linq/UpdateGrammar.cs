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
            this.Context.SetColumn<TMember>(expression);
            return this;
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColumnWithFunc<TMember>(Expression<Func<Parameter, TMember>> expression, string value)
        {
            this.Context.SetColumnWithFunc<TMember>(expression, value);
            return this;
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColumnWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value)
        {
            this.Context.SetColumnWithValue<TMember>(expression, value);
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
        internal readonly string @as;
        internal readonly JoinOption option;
        private object[] operation;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        public UpdateJoinGrammar(string @as, JoinOption option) : this()
        {
            this.@as = @as;
            this.option = option;
            this.operation = new object[2];
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1> On(Expression<Func<Parameter, Table1, bool>> expression)
        {
            this.operation[0] = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1> And(Expression<Func<Table1, bool>> expression)
        {
            if (this.operation[0] == null)
                throw new Exception("please use On method first;");
            if (this.operation[1] != null)
                return this;

            this.operation[1] = expression ?? (object)"a";
            switch (option)
            {
                case JoinOption.Join:
                    {
                        this.update.Context.JoinOnUpdate(this.@as, (Expression<Func<Parameter, Table1, bool>>)this.operation[0], expression);
                    }
                    break;
                case JoinOption.InnerJoin:
                    {
                        this.update.Context.InnerJoinOnUpdate(this.@as, (Expression<Func<Parameter, Table1, bool>>)this.operation[0], expression);
                    }
                    break;
                case JoinOption.RightJoin:
                    {
                        this.update.Context.RightJoinOnUpdate(this.@as, (Expression<Func<Parameter, Table1, bool>>)this.operation[0], expression);
                    }
                    break;
                case JoinOption.LeftJoin:
                    {
                        this.update.Context.LeftJoinOnUpdate(this.@as, (Expression<Func<Parameter, Table1, bool>>)this.operation[0], expression);
                    }
                    break;
            }
            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2> NextJoin<Table2>(string @as)
        {
            this.And(null);
            return new UpdateJoinGrammar<Parameter, Table1, Table2>(@as, JoinOption.Join) { update = this.update };
        }


        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2> NextInnerJoin<Table2>(string @as)
        {
            this.And(null);
            return new UpdateJoinGrammar<Parameter, Table1, Table2>(@as, JoinOption.InnerJoin) { update = this.update };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2> NextLeftJoin<Table2>(string @as)
        {
            this.And(null);
            return new UpdateJoinGrammar<Parameter, Table1, Table2>(@as, JoinOption.LeftJoin) { update = this.update };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2> NextRightJoin<Table2>(string @as)
        {
            this.And(null);
            return new UpdateJoinGrammar<Parameter, Table1, Table2>(@as, JoinOption.RightJoin) { update = this.update };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateGrammar<Parameter> ToUpdate()
        {
            return this.update.StartSetColumn();
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
        internal readonly string @as;
        internal readonly JoinOption option;
        private object[] operation;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        public UpdateJoinGrammar(string @as, JoinOption option) : this()
        {
            this.@as = @as;
            this.option = option;
            this.operation = new object[2];
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2> On(Expression<Func<Parameter, Table1, Table2, bool>> expression)
        {
            this.operation[0] = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2> And(Expression<Func<Table1, Table2, bool>> expression)
        {
            if (this.operation[0] == null)
                throw new Exception("please use On method first;");
            if (this.operation[1] != null)
                return this;
            this.operation[1] = expression ?? (object)"a";
            switch (option)
            {
                case JoinOption.Join:
                    {
                        this.update.Context.JoinOnUpdate(this.@as, (Expression<Func<Parameter, Table1, Table2, bool>>)this.operation[0], expression);
                    }
                    break;
                case JoinOption.InnerJoin:
                    {
                        this.update.Context.InnerJoinOnUpdate(this.@as, (Expression<Func<Parameter, Table1, Table2, bool>>)this.operation[0], expression);
                    }
                    break;
                case JoinOption.RightJoin:
                    {
                        this.update.Context.RightJoinOnUpdate(this.@as, (Expression<Func<Parameter, Table1, Table2, bool>>)this.operation[0], expression);
                    }
                    break;
                case JoinOption.LeftJoin:
                    {
                        this.update.Context.LeftJoinOnUpdate(this.@as, (Expression<Func<Parameter, Table1, Table2, bool>>)this.operation[0], expression);
                    }
                    break;
            }

            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3> NextJoin<Table3>(string @as)
        {
            this.And(null);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3>(@as, JoinOption.Join) { update = this.update };
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3> NextInnerJoin<Table3>(string @as)
        {
            this.And(null);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3>(@as, JoinOption.InnerJoin) { update = this.update };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3> NextLeftJoin<Table3>(string @as)
        {
            this.And(null);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3>(@as, JoinOption.LeftJoin) { update = this.update };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table3"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3> NextRightJoin<Table3>(string @as)
        {
            this.And(null);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3>(@as, JoinOption.RightJoin) { update = this.update };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateGrammar<Parameter> ToUpdate()
        {
            return this.update.StartSetColumn();
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
        internal readonly string @as;
        internal readonly JoinOption option;
        private object[] operation;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        public UpdateJoinGrammar(string @as, JoinOption option) : this()
        {
            this.@as = @as;
            this.option = option;
            this.operation = new object[2];
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3> On(Expression<Func<Parameter, Table1, Table2, Table3, bool>> expression)
        {
            this.operation[0] = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3> And(Expression<Func<Table1, Table2, Table3, bool>> expression)
        {
            if (this.operation[0] == null)
                throw new Exception("please use On method first;");
            if (this.operation[1] != null)
                return this;

            this.operation[1] = expression ?? (object)"a";
            switch (option)
            {
                case JoinOption.Join:
                    {
                        this.update.Context.JoinOnUpdate(this.@as, (Expression<Func<Parameter, Table1, Table2, Table3, bool>>)this.operation[0], expression);
                    }
                    break;
                case JoinOption.InnerJoin:
                    {
                        this.update.Context.InnerJoinOnUpdate(this.@as, (Expression<Func<Parameter, Table1, Table2, Table3, bool>>)this.operation[0], expression);
                    }
                    break;
                case JoinOption.RightJoin:
                    {
                        this.update.Context.RightJoin(this.@as, (Expression<Func<Parameter, Table1, Table2, Table3, bool>>)this.operation[0], expression);
                    }
                    break;
                case JoinOption.LeftJoin:
                    {
                        this.update.Context.LeftJoin(this.@as, (Expression<Func<Parameter, Table1, Table2, Table3, bool>>)this.operation[0], expression);
                    }
                    break;
            }
            return this;
        }

        /// <summary>
        /// join
        /// </summary>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4> NextJoin<Table4>(string @as)
        {
            this.And(null);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4>(@as, JoinOption.Join) { update = this.update };
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4> NextInnerJoin<Table4>(string @as)
        {
            this.And(null);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4>(@as, JoinOption.InnerJoin) { update = this.update };
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4> NextLeftJoin<Table4>(string @as)
        {
            this.And(null);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4>(@as, JoinOption.LeftJoin) { update = this.update };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table4"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4> NextRightJoin<Table4>(string @as)
        {
            this.And(null);
            return new UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4>(@as, JoinOption.RightJoin) { update = this.update };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateGrammar<Parameter> ToUpdate()
        {
            this.And(null);
            return this.update.StartSetColumn();
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
        internal readonly string @as;
        internal readonly JoinOption option;
        private object[] operation;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        public UpdateJoinGrammar(string @as, JoinOption option) : this()
        {
            this.@as = @as;
            this.option = option;
            this.operation = new object[2];
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4> On(Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> expression)
        {
            this.operation[0] = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateJoinGrammar<Parameter, Table1, Table2, Table3, Table4> And(Expression<Func<Table1, Table2, Table3, Table4, bool>> expression)
        {
            if (this.operation[0] == null)
                throw new Exception("please use On method first;");
            if (this.operation[1] != null)
                return this;

            this.operation[1] = expression ?? (object)"a";
            switch (option)
            {
                case JoinOption.Join:
                    {
                        this.update.Context.JoinOnUpdate(this.@as, (Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>>)this.operation[0], expression);
                    }
                    break;
                case JoinOption.InnerJoin:
                    {
                        this.update.Context.InnerJoin(this.@as, (Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>>)this.operation[0], expression);
                    }
                    break;
                case JoinOption.RightJoin:
                    {
                        this.update.Context.RightJoin(this.@as, (Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>>)this.operation[0], expression);
                    }
                    break;
                case JoinOption.LeftJoin:
                    {
                        this.update.Context.LeftJoin(this.@as, (Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>>)this.operation[0], expression);
                    }
                    break;
            }
            return this;
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateGrammar<Parameter> ToUpdate()
        {
            this.And(null);
            return this.update.StartSetColumn();
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
        internal readonly string @as;
        internal readonly AndOrOption option;
        internal readonly char @flag;
        private object[] operation;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="flag">只有n和e</param>
        public UpdateWhereExistsGrammar(string @as, AndOrOption option, char flag) : this()
        {
            this.@as = @as;
            this.option = option;
            this.flag = flag;
            this.operation = new object[2];
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1> Where(Expression<Func<Parameter, Table1, bool>> expression)
        {
            this.operation[0] = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1> And(Expression<Func<Table1, bool>> expression)
        {
            if (this.operation[0] == null)
                throw new Exception("please use Where method first;");
            if (this.operation[1] != null)
                return this;
            this.operation[1] = expression ?? (object)"a";
            switch (this.flag)
            {
                case 'n':
                    {
                        this.where.Context.NotExists(this.option, this.@as, (Expression<Func<Parameter, Table1, bool>>)this.operation[0], expression);
                    }
                    break;
                default:
                    {
                        this.where.Context.Exists(this.option, this.@as, (Expression<Func<Parameter, Table1, bool>>)this.operation[0], expression);
                    }
                    break;
            }

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
            this.And(null);
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2>(@as, JoinOption.Join) { where = this.where };
        }


        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2> InnerJoin<Table2>(string @as)
        {
            this.And(null);
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2>(@as, JoinOption.InnerJoin) { where = this.where };
        }


        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2> LeftJoin<Table2>(string @as)
        {
            this.And(null);
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2>(@as, JoinOption.LeftJoin) { where = this.where };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2> RightJoin<Table2>(string @as)
        {
            this.And(null);
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2>(@as, JoinOption.RightJoin) { where = this.where };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
        {
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
        internal UpdateWhereExistsGrammar<Parameter, Table1> prev { get; set; }
        internal readonly string @as;
        internal readonly JoinOption joinOption;
        private object[] operation;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="joinOption"></param>
        public UpdateWhereExistsGrammar(string @as, JoinOption joinOption) : this()
        {
            this.@as = @as;
            this.joinOption = joinOption;
            this.operation = new object[2];
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2> On(Expression<Func<Parameter, Table1, Table2, bool>> expression)
        {
            this.operation[0] = expression;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2> And(Expression<Func<Parameter, Table1, Table2, bool>> expression)
        {
            if (this.operation[0] == null)
                throw new Exception("please use On method first;");
            if (this.operation[1] != null)
                return this;
            this.operation[1] = expression ?? (object)"a";
            this.where.Context.Exists(this.option, this.@as, (Expression<Func<Parameter, Table1, bool>>)this.operation[0], expression);
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
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3>(@as, AndOrOption.and, JoinOption.Join, 'e')
            {
                where = this.where,
                prev = this,
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
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3>(@as, AndOrOption.and, JoinOption.InnerJoin, 'n')
            {
                where = this.where,
                prev = this,
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
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3>(@as, AndOrOption.or, JoinOption.LeftJoin, 'e')
            {
                where = this.where,
                prev = this,
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
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3>(@as, AndOrOption.or, JoinOption.RightJoin, 'n')
            {
                where = this.where,
                prev = this,
            };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
        {
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
        internal UpdateWhereExistsGrammar<Parameter, Table1, Table2> prev { get; set; }
        internal readonly string @as;
        internal readonly AndOrOption option;
        internal readonly JoinOption joinOption;
        internal readonly char flag;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="joinOption"></param>
        /// <param name="flag"></param>
        public UpdateWhereExistsGrammar(string @as, AndOrOption option, JoinOption joinOption, char flag) : this()
        {
            this.@as = @as;
            this.option = option;
            this.joinOption = joinOption;
            this.flag = flag;
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3> On(Expression<Func<Parameter, Table1, Table2, Table3, bool>> expression)
        {
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3> And(Expression<Func<Parameter, Table1, Table2, Table3, bool>> expression)
        {
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
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4>(@as, AndOrOption.and, JoinOption.Join, 'e')
            {
                where = this.where,
                prev = this,
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
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4>(@as, AndOrOption.and, JoinOption.InnerJoin, 'n')
            {
                where = this.where,
                prev = this,
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
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4>(@as, AndOrOption.or, JoinOption.LeftJoin, 'e')
            {
                where = this.where,
                prev = this,
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
            return new UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4>(@as, AndOrOption.or, JoinOption.RightJoin, 'n')
            {
                where = this.where,
                prev = this,
            };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
        {
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
        internal UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3> prev { get; set; }
        internal readonly string @as;
        internal readonly AndOrOption option;
        internal readonly JoinOption joinOption;
        internal readonly char flag;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="joinOption"></param>
        /// <param name="flag"></param>
        public UpdateWhereExistsGrammar(string @as, AndOrOption option, JoinOption joinOption, char flag) : this()
        {
            this.@as = @as;
            this.option = option;
            this.joinOption = joinOption;
            this.flag = flag;
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4> On(Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> expression)
        {
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereExistsGrammar<Parameter, Table1, Table2, Table3, Table4> And(Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> expression)
        {
            return this;
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
        {
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
        internal readonly string @as;
        internal readonly AndOrOption option;
        internal readonly char @flag;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="flag">只有n和e</param>
        public UpdateWhereInGrammar(string @as, AndOrOption option, char flag) : this()
        {
            this.@as = @as;
            this.option = option;
            this.flag = flag;
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1> Field(Expression<Func<Parameter, Table1, bool>> expression)
        {
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1> Where(Expression<Func<Parameter, Table1, bool>> expression)
        {
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
            return new UpdateWhereInGrammar<Parameter, Table1, Table2>(@as, AndOrOption.and, JoinOption.Join, 'e') { where = this.where };
        }


        /// <summary>
        /// inner join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1, Table2> InnerJoin<Table2>(string @as)
        {
            return new UpdateWhereInGrammar<Parameter, Table1, Table2>(@as, AndOrOption.and, JoinOption.InnerJoin, 'n')
            {
                where = this.where,
                prev = this,
            };
        }


        /// <summary>
        /// left join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1, Table2> LeftJoin<Table2>(string @as)
        {
            return new UpdateWhereInGrammar<Parameter, Table1, Table2>(@as, AndOrOption.or, JoinOption.LeftJoin, 'e')
            {
                where = this.where,
                prev = this,
            };
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <typeparam name="Table2"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1, Table2> RightJoin<Table2>(string @as)
        {
            return new UpdateWhereInGrammar<Parameter, Table1, Table2>(@as, AndOrOption.or, JoinOption.RightJoin, 'n')
            {
                where = this.where,
                prev = this,
            };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
        {
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
        internal UpdateWhereInGrammar<Parameter, Table1> prev { get; set; }
        internal readonly string @as;
        internal readonly AndOrOption option;
        internal readonly JoinOption joinOption;
        internal readonly char flag;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="joinOption"></param>
        /// <param name="flag"></param>
        public UpdateWhereInGrammar(string @as, AndOrOption option, JoinOption joinOption, char flag) : this()
        {
            this.@as = @as;
            this.option = option;
            this.joinOption = joinOption;
            this.flag = flag;
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1, Table2> On(Expression<Func<Parameter, Table1, Table2, bool>> expression)
        {
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1, Table2> And(Expression<Func<Parameter, Table1, Table2, bool>> expression)
        {
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
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3>(@as, AndOrOption.and, JoinOption.Join, 'e')
            {
                where = this.where,
                prev = this,
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
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3>(@as, AndOrOption.and, JoinOption.InnerJoin, 'n')
            {
                where = this.where,
                prev = this,
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
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3>(@as, AndOrOption.or, JoinOption.LeftJoin, 'e')
            {
                where = this.where,
                prev = this,
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
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3>(@as, AndOrOption.or, JoinOption.RightJoin, 'n')
            {
                where = this.where,
                prev = this,
            };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
        {
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
        internal UpdateWhereInGrammar<Parameter, Table1, Table2> prev { get; set; }
        internal readonly string @as;
        internal readonly AndOrOption option;
        internal readonly JoinOption joinOption;
        internal readonly char flag;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="joinOption"></param>
        /// <param name="flag"></param>
        public UpdateWhereInGrammar(string @as, AndOrOption option, JoinOption joinOption, char flag) : this()
        {
            this.@as = @as;
            this.option = option;
            this.joinOption = joinOption;
            this.flag = flag;
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3> On(Expression<Func<Parameter, Table1, Table2, Table3, bool>> expression)
        {
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3> And(Expression<Func<Parameter, Table1, Table2, Table3, bool>> expression)
        {
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
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4>(@as, AndOrOption.and, JoinOption.Join, 'e')
            {
                where = this.where,
                prev = this,
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
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4>(@as, AndOrOption.and, JoinOption.InnerJoin, 'n')
            {
                where = this.where,
                prev = this,
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
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4>(@as, AndOrOption.or, JoinOption.LeftJoin, 'e')
            {
                where = this.where,
                prev = this,
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
            return new UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4>(@as, AndOrOption.or, JoinOption.RightJoin, 'n')
            {
                where = this.where,
                prev = this,
            };
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
        {
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
        internal UpdateWhereInGrammar<Parameter, Table1, Table2, Table3> prev { get; set; }
        internal readonly string @as;
        internal readonly AndOrOption option;
        internal readonly JoinOption joinOption;
        internal readonly char flag;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="as"></param>
        /// <param name="option"></param>
        /// <param name="joinOption"></param>
        /// <param name="flag"></param>
        public UpdateWhereInGrammar(string @as, AndOrOption option, JoinOption joinOption, char flag) : this()
        {
            this.@as = @as;
            this.option = option;
            this.joinOption = joinOption;
            this.flag = flag;
        }

        /// <summary>
        /// on
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4> On(Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> expression)
        {
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table1, Table2, Table3, Table4> And(Expression<Func<Parameter, Table1, Table2, Table3, Table4, bool>> expression)
        {
            return this;
        }

        /// <summary>
        /// then
        /// </summary>
        public UpdateWhereGrammar<Parameter> ToWhere()
        {
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
            return new UpdateWhereExistsGrammar<Parameter, Table>(@as, AndOrOption.or, 'n') { where = this };
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
        public UpdateWhereGrammar<Parameter> AndNotExists(string expression)
        {
            this.Context.NotExists(AndOrOption.and, expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如select 0 from table2 inner join table3 on table2.Id = table3.Id and table2.Name = table.UserName，其中table的名字由参数Tableinfo传递</param>
        public UpdateWhereGrammar<Parameter> OrNotExists(string expression)
        {
            this.Context.NotExists(AndOrOption.or, expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table> AndIn<Table>(string @as)
        {
            return new UpdateWhereInGrammar<Parameter, Table>(@as, AndOrOption.and, 'e') { where = this };
        }

        /// <summary>
        /// 不存在
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table> AndNotIn<Table>(string @as)
        {
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
            return new UpdateWhereInGrammar<Parameter, Table>(@as, AndOrOption.or, 'e') { where = this };
        }

        /// <summary>
        /// 不存在
        /// </summary>
        /// <typeparam name="Table"></typeparam>
        /// <param name="as"></param>
        /// <returns></returns>
        public UpdateWhereInGrammar<Parameter, Table> OrNotIn<Table>(string @as)
        {
            return new UpdateWhereInGrammar<Parameter, Table>(@as, AndOrOption.or, 'n') { where = this };
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
        public UpdateWhereGrammar<Parameter> AndIn(string expression)
        {
            this.Context.In(AndOrOption.and, expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
        public UpdateWhereGrammar<Parameter> AndNotIn(string expression)
        {
            this.Context.NotIn(AndOrOption.and, expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如table.UserName in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
        public UpdateWhereGrammar<Parameter> OrIn(string expression)
        {
            this.Context.In(AndOrOption.or, expression);
            return this;
        }

        /// <summary>
        /// 存在
        /// </summary>
        /// <param name="expression">自己写的sql语法，比如table.UserName not in (select table2.Name from table2 inner join table3 on table2.Id = table3.Id)，其中table的名字由参数Tableinfo传递</param>
        public UpdateWhereGrammar<Parameter> OrNotIn(string expression)
        {
            this.Context.NotIn(AndOrOption.or, expression);
            return this;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public int GetResult()
        {
            return this.GetResult();
        }
    }
}
