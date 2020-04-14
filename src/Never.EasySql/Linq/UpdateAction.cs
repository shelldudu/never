using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 更新操作
    /// </summary>
    /// <typeparam name="Parameter"></typeparam>
    public struct UpdateAction<Parameter>
    {
        /// <summary>
        /// 上下文
        /// </summary>
        internal _UpdateContext<Parameter> Context { get; set; }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateAction<Parameter> As(string table)
        {
            this.Context.AsTable(table);
            return this;
        }

        /// <summary>
        /// 从哪一张表更新
        /// </summary>
        public UpdateAction<Parameter> From(string table)
        {
            this.Context.From(table);
            return this;
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColum<TMember>(Expression<Func<Parameter, TMember>> expression)
        {
            return new UpdateGrammar<Parameter>() { Context = this.Context }.Entrance().SetColumn(expression);
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColumWithFunc<TMember>(Expression<Func<Parameter, TMember>> expression, string value)
        {
            return new UpdateGrammar<Parameter>() { Context = this.Context }.Entrance().SetColumnWithFunc(expression, value);
        }

        /// <summary>
        /// 更新的字段名
        /// </summary>
        public UpdateGrammar<Parameter> SetColumWithValue<TMember>(Expression<Func<Parameter, TMember>> expression, TMember value)
        {
            return new UpdateGrammar<Parameter>() { Context = this.Context }.Entrance().SetColumnWithValue(expression, value);
        }
    }
}
