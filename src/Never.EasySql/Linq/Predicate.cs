using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    public class Predicate<Parameter> where Parameter : class
    {
        private readonly Parameter parameter = null;

        public bool Contains<T>(Expression<IFunc<Parameter, T>> selector, IEnumerable<T> values) where T : IConvertible
        {
        }
    }
}