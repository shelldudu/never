using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    public abstract class Insert<T>
    {
        internal Context Context { get; set; }

        public _LastInsertId LastInsertId()
        {
            return new _LastInsertId() { Parent = this };
        }

        public Result AsObject<Result>()
        {
            return default(Result);
        }

        public object ToChange()
        {
            return null;
        }

        public Result ToChange<Result>()
        {
            return default(Result);
        }

        public struct _LastInsertId
        {
            internal Insert<T> Parent { get; set; }

            public object ToChange()
            {
                return this.Parent.ToChange();
            }

            public Result ToChange<Result>()
            {
                return this.Parent.ToChange<Result>();
            }
        }
    }

    public abstract class Insert<T1, T2>
    {
        internal Context Context { get; set; }

        public _Join Join()
        {
            return this;
        }

        public _LeftJoin LeftJoin()
        {
            return this;
        }

        public _InnerJoin InnerJoin()
        {
            return this;
        }

        public _RightJoin RightJoin()
        {
            return this;
        }

        public _LastInsertId LastInsertId()
        {
            return new _InsertLastInsertId() { Parent = this };
        }

        public object ToChange()
        {
            return null;
        }

        public Result ToChange<Result>()
        {
            return default(Result);
        }

        public struct _Join
        {
            internal Insert<T1, T2> Parent { get; set; }

            public object ToChange()
            {
                return this.Parent.ToChange();
            }

            public Result ToChange<Result>()
            {
                return this.Parent.ToChange<Result>();
            }
        }

        public struct _LeftJoin
        {
            internal Insert<T1, T2> Parent { get; set; }

            public object ToChange()
            {
                return this.Parent.ToChange();
            }

            public Result ToChange<Result>()
            {
                return this.Parent.ToChange<Result>();
            }
        }

        public struct _InnerJoin
        {
            internal Insert<T1, T2> Parent { get; set; }

            public object ToChange()
            {
                return this.Parent.ToChange();
            }

            public Result ToChange<Result>()
            {
                return this.Parent.ToChange<Result>();
            }
        }

        public struct _RightJoin
        {
            internal Insert<T1, T2> Parent { get; set; }

            public object ToChange()
            {
                return this.Parent.ToChange();
            }

            public Result ToChange<Result>()
            {
                return this.Parent.ToChange<Result>();
            }
        }

        public struct _LastInsertId
        {
            internal Insert<T1, T2> Parent { get; set; }

            public object ToChange()
            {
                return this.Parent.ToChange();
            }

            public Result ToChange<Result>()
            {
                return this.Parent.ToChange<Result>();
            }
        }
    }
}
