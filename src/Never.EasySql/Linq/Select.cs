using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql.Linq
{
    /// <summary>
    /// 单个表的查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="T">查询结果对象</typeparam>
    public struct Select<Parameter, T>
    {
        /// <summary>
        /// where 条件
        /// </summary>
        public NWhere<Parameter, T> Where(Expression<Func<Parameter, T, object>> expression)
        {
            return new NWhere<Parameter, T>();
        }

        /// <summary>
        /// where 条件
        /// </summary>
        /// <typeparam name="NParameter">查询参数</typeparam>
        /// <typeparam name="NT">查询结果对象</typeparam>
        public struct NWhere<NParameter, NT>
        {
            /// <summary>
            /// 返回列表
            /// </summary>
            public NToList<T> ToList(int pageSize, int pageNow)
            {
                return new NToList<T>();
            }

            /// <summary>
            /// 返回单条
            /// </summary>
            public NToSingle<T> ToSingle()
            {
                return new NToSingle<T>();
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public Exists<T> Exists<T1>(Expression<Func<Parameter, T, T1, object>> expression)
            {
                return new Exists<T>();
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NotExists<T> NotExists<T1>(Expression<Func<Parameter, T, T1, object>> expression)
            {
                return new NotExists<T>();
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public In<T> In<T1>(Expression<Func<Parameter, T, T1, object>> expression)
            {
                return new In<T>();
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NotIn<T> NotIn<T1>(Expression<Func<Parameter, T, T1, object>> expression)
            {
                return new NotIn<T>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT">查询结果对象</typeparam>
        public struct NToList<NT>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public IEnumerable<T> GetResult()
            {
                return System.Linq.Enumerable.Empty<T>();
            }
        }

        /// <summary>
        /// 返回单条
        /// </summary>
        /// <typeparam name="NT">查询结果对象</typeparam>
        public struct NToSingle<NT>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public T GetResult()
            {
                return default(T);
            }
        }

        /// <summary>
        /// 存在
        /// </summary>
        public struct Exists<NT>
        {
            /// <summary>
            /// 返回列表
            /// </summary>
            public NToList<T> ToList(int pageSize, int pageNow)
            {
                return new NToList<T>();
            }

            /// <summary>
            /// 返回单条
            /// </summary>
            public NToSingle<T> ToSingle()
            {
                return new NToSingle<T>();
            }
        }

        /// <summary>
        /// 不存在
        /// </summary>
        public struct NotExists<NT>
        {
            /// <summary>
            /// 返回列表
            /// </summary>
            public NToList<T> ToList(int pageSize, int pageNow)
            {
                return new NToList<T>();
            }

            /// <summary>
            /// 返回单条
            /// </summary>
            public NToSingle<T> ToSingle()
            {
                return new NToSingle<T>();
            }
        }

        /// <summary>
        /// 存在
        /// </summary>
        public struct In<NT>
        {
            /// <summary>
            /// 返回列表
            /// </summary>
            public NToList<T> ToList(int pageSize, int pageNow)
            {
                return new NToList<T>();
            }

            /// <summary>
            /// 返回单条
            /// </summary>
            public NToSingle<T> ToSingle()
            {
                return new NToSingle<T>();
            }
        }

        /// <summary>
        /// 不存在
        /// </summary>
        public struct NotIn<NT>
        {
            /// <summary>
            /// 返回列表
            /// </summary>
            public NToList<T> ToList(int pageSize, int pageNow)
            {
                return new NToList<T>();
            }

            /// <summary>
            /// 返回单条
            /// </summary>
            public NToSingle<T> ToSingle()
            {
                return new NToSingle<T>();
            }
        }
    }

    /// <summary>
    /// 2个表的联合查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="T1">查询结果对象1</typeparam>
    /// <typeparam name="T2">查询结果对象2</typeparam>
    public struct Select<Parameter, T1, T2>
    {
        /// <summary>
        /// join
        /// </summary>
        /// <param name="expression"></param>
        public Select<Parameter, T1, T2> Join(Expression<Func<Parameter, T1, T2, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="expression"></param>
        public Select<Parameter, T1, T2> LeftJoin(Expression<Func<Parameter, T1, T2, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="expression"></param>
        public Select<Parameter, T1, T2> RightJoin(Expression<Func<Parameter, T1, T2, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="expression"></param>
        public Select<Parameter, T1, T2> InnerJoin(Expression<Func<Parameter, T1, T2, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public NWhere<Parameter, T1, T2> Where(Expression<Func<Parameter, T1, T2, object>> expression)
        {
            return new NWhere<Parameter, T1, T2>();
        }

        /// <summary>
        /// where 条件
        /// </summary>
        /// <typeparam name="NParameter">查询参数</typeparam>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        public struct NWhere<NParameter, NT1, NT2>
        {
            /// <summary>
            /// 返回第一个表的查询列表
            /// </summary>
            public NToList<NT1, NT2> ToList(int pageSize, int pageNow)
            {
                return new NToList<NT1, NT2>();
            }

            /// <summary>
            /// 返回第一个表的查询对象
            /// </summary>
            public NToSingle<NT1, NT2> ToSingle()
            {
                return new NToSingle<NT1, NT2>();
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public Exists<NT1, NT2> Exists<T1>(Expression<Func<Parameter, T, T1, object>> expression)
            {
                return new Exists<NT1, NT2>();
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NotExists<NT1, NT2> NotExists<T1>(Expression<Func<Parameter, T, T1, object>> expression)
            {
                return new NotExists<NT1, NT2>();
            }

            /// <summary>
            /// 存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public In<NT1, NT2> In<T1>(Expression<Func<Parameter, T, T1, object>> expression)
            {
                return new In<NT1, NT2>();
            }

            /// <summary>
            /// 不存在
            /// </summary>
            /// <typeparam name="T1">另外的表中</typeparam>
            public NotIn<NT1, NT2> NotIn<T1>(Expression<Func<Parameter, T, T1, object>> expression)
            {
                return new NotIn<NT1, NT2>();
            }
        }

        /// <summary>
        /// 存在
        /// </summary>
        public struct Exists<NT1, NT2>
        {
            /// <summary>
            /// 返回第一个表的查询列表
            /// </summary>
            public NToList<NT1, NT2> ToList(int pageSize, int pageNow)
            {
                return new NToList<NT1, NT2>();
            }

            /// <summary>
            /// 返回第一个表的查询对象
            /// </summary>
            public NToSingle<NT1, NT2> ToSingle()
            {
                return new NToSingle<NT1, NT2>();
            }
        }

        /// <summary>
        /// 不存在
        /// </summary>
        public struct NotExists<NT1, NT2>
        {
            /// <summary>
            /// 返回第一个表的查询列表
            /// </summary>
            public NToList<NT1, NT2> ToList(int pageSize, int pageNow)
            {
                return new NToList<NT1, NT2>();
            }

            /// <summary>
            /// 返回第一个表的查询对象
            /// </summary>
            public NToSingle<NT1, NT2> ToSingle()
            {
                return new NToSingle<NT1, NT2>();
            }
        }

        /// <summary>
        /// 存在
        /// </summary>
        public struct In<NT1, NT2>
        {
            /// <summary>
            /// 返回第一个表的查询列表
            /// </summary>
            public NToList<NT1, NT2> ToList(int pageSize, int pageNow)
            {
                return new NToList<NT1, NT2>();
            }

            /// <summary>
            /// 返回第一个表的查询对象
            /// </summary>
            public NToSingle<NT1, NT2> ToSingle()
            {
                return new NToSingle<NT1, NT2>();
            }
        }

        /// <summary>
        /// 不存在
        /// </summary>
        public struct NotIn<NT1, NT2>
        {
            /// <summary>
            /// 返回第一个表的查询列表
            /// </summary>
            public NToList<NT1, NT2> ToList(int pageSize, int pageNow)
            {
                return new NToList<NT1, NT2>();
            }

            /// <summary>
            /// 返回第一个表的查询对象
            /// </summary>
            public NToSingle<NT1, NT2> ToSingle()
            {
                return new NToSingle<NT1, NT2>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        public struct NToList<NT1, NT2>
        {
            /// <summary>
            /// 返回第二个表的查询列表
            /// </summary>
            public NNToListToList<NT1, NT2> ToList(int pageSize, int pageNow)
            {
                return new NNToListToList<NT1, NT2>();
            }

            /// <summary>
            /// 返回第二个表的查询对象
            /// </summary>
            public NNToListToSingle<NT1, NT2> ToSingle()
            {
                return new NNToListToSingle<NT1, NT2>();
            }
        }

        /// <summary>
        /// 返回单条
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        public struct NToSingle<NT1, NT2>
        {
            /// <summary>
            /// 返回第二个表的查询列表
            /// </summary>
            public NNToSingleToList<NT1, NT2> ToList(int pageSize, int pageNow)
            {
                return new NNToSingleToList<NT1, NT2>();
            }

            /// <summary>
            /// 返回第二个表的查询对象
            /// </summary>
            public NNToSingleToSingle<NT1, NT2> ToSingle()
            {
                return new NNToSingleToSingle<NT1, NT2>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        public struct NNToListToList<NT1, NT2>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, IEnumerable<T2>> GetResult()
            {
                return Tuple.Create(System.Linq.Enumerable.Empty<T1>(), System.Linq.Enumerable.Empty<T2>());
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        public struct NNToListToSingle<NT1, NT2>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, T2> GetResult()
            {
                return Tuple.Create(System.Linq.Enumerable.Empty<T1>(), default(T2));
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        public struct NNToSingleToList<NT1, NT2>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<T1, IEnumerable<T2>> GetResult()
            {
                return Tuple.Create(default(T1), System.Linq.Enumerable.Empty<T2>());
            }
        }

        /// <summary>
        /// 返回单条
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        public struct NNToSingleToSingle<NT1, NT2>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<T1, T2> GetResult()
            {
                return Tuple.Create(default(T1), default(T2));
            }
        }
    }

    /// <summary>
    /// 3个表的联合查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="T1">查询结果对象1</typeparam>
    /// <typeparam name="T2">查询结果对象2</typeparam>
    /// <typeparam name="T3">查询结果对象3</typeparam>
    public struct Select<Parameter, T1, T2, T3>
    {
        /// <summary>
        /// join
        /// </summary>
        /// <param name="expression"></param>
        public Select<Parameter, T1, T2, T3> Join(Expression<Func<Parameter, T1, T2, T3, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="expression"></param>
        public Select<Parameter, T1, T2, T3> LeftJoin(Expression<Func<Parameter, T1, T2, T3, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="expression"></param>
        public Select<Parameter, T1, T2, T3> RightJoin(Expression<Func<Parameter, T1, T2, T3, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="expression"></param>
        public Select<Parameter, T1, T2, T3> InnerJoin(Expression<Func<Parameter, T1, T2, T3, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public NWhere<Parameter, T1, T2, T3> Where(Expression<Func<Parameter, T1, T2, T3, object>> expression)
        {
            return new NWhere<Parameter, T1, T2, T3>();
        }

        /// <summary>
        /// where 条件
        /// </summary>
        /// <typeparam name="NParameter">查询参数</typeparam>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        public struct NWhere<NParameter, NT1, NT2, NT3>
        {
            /// <summary>
            /// 返回第一个表的查询列表
            /// </summary>
            public NToList<NT1, NT2, NT3> ToList(int pageSize, int pageNow)
            {
                return new NToList<NT1, NT2, NT3>();
            }

            /// <summary>
            /// 返回第一个表的查询对象
            /// </summary>
            public NToSingle<NT1, NT2, NT3> ToSingle()
            {
                return new NToSingle<NT1, NT2, NT3>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        public struct NToList<NT1, NT2, NT3>
        {
            /// <summary>
            /// 返回第二个表的查询列表
            /// </summary>
            public NNToListToList<NT1, NT2, NT3> ToList(int pageSize, int pageNow)
            {
                return new NNToListToList<NT1, NT2, NT3>();
            }

            /// <summary>
            /// 返回第二个表的查询对象
            /// </summary>
            public NNToListToSingle<NT1, NT2, NT3> ToSingle()
            {
                return new NNToListToSingle<NT1, NT2, NT3>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        public struct NNToListToList<NT1, NT2, NT3>
        {
            /// <summary>
            /// 返回第三个表的查询列表
            /// </summary>
            public NNNToListToListToList<NT1, NT2, NT3> ToList(int pageSize, int pageNow)
            {
                return new NNNToListToListToList<NT1, NT2, NT3>();
            }

            /// <summary>
            /// 返回第三个表的查询对象
            /// </summary>
            public NNNToListToListToSingle<NT1, NT2, NT3> ToSingle()
            {
                return new NNNToListToListToSingle<NT1, NT2, NT3>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        public struct NNNToListToListToList<NT1, NT2, NT3>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>());
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        public struct NNNToListToListToSingle<NT1, NT2, NT3>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, IEnumerable<T2>, T3> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), default(T3));
            }
        }

        /// <summary>
        /// 返回对象
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        public struct NNToListToSingle<NT1, NT2, NT3>
        {
            /// <summary>
            /// 返回第三个表的查询列表
            /// </summary>
            public NNNToListToSingleToList<NT1, NT2, NT3> ToList(int pageSize, int pageNow)
            {
                return new NNNToListToSingleToList<NT1, NT2, NT3>();
            }

            /// <summary>
            /// 返回第三个表的查询对象
            /// </summary>
            public NNNToListToSingleToSingle<NT1, NT2, NT3> ToSingle()
            {
                return new NNNToListToSingleToSingle<NT1, NT2, NT3>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        public struct NNNToListToSingleToList<NT1, NT2, NT3>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, T2, IEnumerable<T3>> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), default(T2), Enumerable.Empty<T3>());
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        public struct NNNToListToSingleToSingle<NT1, NT2, NT3>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, T2, T3> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), default(T2), default(T3));
            }
        }

        /// <summary>
        /// 返回对象
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        public struct NToSingle<NT1, NT2, NT3>
        {
            /// <summary>
            /// 返回第二个表的查询列表
            /// </summary>
            public NNToSingleToList<NT1, NT2, NT3> ToList(int pageSize, int pageNow)
            {
                return new NNToSingleToList<NT1, NT2, NT3>();
            }

            /// <summary>
            /// 返回第二个表的查询对象
            /// </summary>
            public NNToSingleToSingle<NT1, NT2, NT3> ToSingle()
            {
                return new NNToSingleToSingle<NT1, NT2, NT3>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        public struct NNToSingleToList<NT1, NT2, NT3>
        {
            /// <summary>
            /// 返回第三个表的查询列表
            /// </summary>
            public NNNToSingleToListToList<NT1, NT2, NT3> ToList(int pageSize, int pageNow)
            {
                return new NNNToSingleToListToList<NT1, NT2, NT3>();
            }

            /// <summary>
            /// 返回第三个表的查询对象
            /// </summary>
            public NNNToSingleToListToSingle<NT1, NT2, NT3> ToSingle()
            {
                return new NNNToSingleToListToSingle<NT1, NT2, NT3>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        public struct NNNToSingleToListToList<NT1, NT2, NT3>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<T1, IEnumerable<T2>, IEnumerable<T3>> GetResult()
            {
                return Tuple.Create(default(T1), Enumerable.Empty<T2>(), Enumerable.Empty<T3>());
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        public struct NNNToSingleToListToSingle<NT1, NT2, NT3>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<T1, IEnumerable<T2>, T3> GetResult()
            {
                return Tuple.Create(default(T1), Enumerable.Empty<T2>(), default(T3));
            }
        }

        /// <summary>
        /// 返回对象
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        public struct NNToSingleToSingle<NT1, NT2, NT3>
        {
            /// <summary>
            /// 返回第三个表的查询列表
            /// </summary>
            public NNNToSingleToSingleToList<NT1, NT2, NT3> ToList(int pageSize, int pageNow)
            {
                return new NNNToSingleToSingleToList<NT1, NT2, NT3>();
            }

            /// <summary>
            /// 返回第三个表的查询对象
            /// </summary>
            public NNNToSingleToSingleToSingle<NT1, NT2, NT3> ToSingle()
            {
                return new NNNToSingleToSingleToSingle<NT1, NT2, NT3>();
            }
        }

        /// <summary>
        /// 返回对象
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        public struct NNNToSingleToSingleToList<NT1, NT2, NT3>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<T1, T2, IEnumerable<T3>> GetResult()
            {
                return Tuple.Create(default(T1), default(T2), Enumerable.Empty<T3>());
            }
        }

        /// <summary>
        /// 返回对象
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        public struct NNNToSingleToSingleToSingle<NT1, NT2, NT3>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<T1, T2, T3> GetResult()
            {
                return Tuple.Create(default(T1), default(T2), default(T3));
            }
        }
    }

    /// <summary>
    /// 4个表的联合查询
    /// </summary>
    /// <typeparam name="Parameter">查询参数</typeparam>
    /// <typeparam name="T1">查询结果对象1</typeparam>
    /// <typeparam name="T2">查询结果对象2</typeparam>
    /// <typeparam name="T3">查询结果对象3</typeparam>
    /// <typeparam name="T4">查询结果对象3</typeparam>
    public struct Select<Parameter, T1, T2, T3, T4>
    {
        /// <summary>
        /// join
        /// </summary>
        /// <param name="expression"></param>
        public Select<Parameter, T1, T2, T3, T4> Join(Expression<Func<Parameter, T1, T2, T3, T4, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="expression"></param>
        public Select<Parameter, T1, T2, T3, T4> LeftJoin(Expression<Func<Parameter, T1, T2, T3, T4, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="expression"></param>
        public Select<Parameter, T1, T2, T3, T4> RightJoin(Expression<Func<Parameter, T1, T2, T3, T4, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="expression"></param>
        public Select<Parameter, T1, T2, T3, T4> InnerJoin(Expression<Func<Parameter, T1, T2, T3, T4, object>> expression)
        {
            return this;
        }

        /// <summary>
        /// where 条件
        /// </summary>
        public NWhere<Parameter, T1, T2, T3, T4> Where(Expression<Func<Parameter, T1, T2, T3, T4, object>> expression)
        {
            return new NWhere<Parameter, T1, T2, T3, T4>();
        }

        /// <summary>
        /// where 条件
        /// </summary>
        /// <typeparam name="NParameter">查询参数</typeparam>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NWhere<NParameter, NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回第一个表的查询列表
            /// </summary>
            public NToList<NT1, NT2, NT3, NT4> ToList(int pageSize, int pageNow)
            {
                return new NToList<NT1, NT2, NT3, NT4>();
            }

            /// <summary>
            /// 返回第一个表的查询对象
            /// </summary>
            public NToSingle<NT1, NT2, NT3, NT4> ToSingle()
            {
                return new NToSingle<NT1, NT2, NT3, NT4>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NToList<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回第二个表的查询列表
            /// </summary>
            public NNToListToList<NT1, NT2, NT3, NT4> ToList(int pageSize, int pageNow)
            {
                return new NNToListToList<NT1, NT2, NT3, NT4>();
            }

            /// <summary>
            /// 返回第二个表的查询对象
            /// </summary>
            public NNToListToSingle<NT1, NT2, NT3, NT4> ToSingle()
            {
                return new NNToListToSingle<NT1, NT2, NT3, NT4>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNToListToList<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回第三个表的查询列表
            /// </summary>
            public NNNToListToListToList<NT1, NT2, NT3, NT4> ToList(int pageSize, int pageNow)
            {
                return new NNNToListToListToList<NT1, NT2, NT3, NT4>();
            }

            /// <summary>
            /// 返回第三个表的查询对象
            /// </summary>
            public NNNToListToListToSingle<NT1, NT2, NT3, NT4> ToSingle()
            {
                return new NNNToListToListToSingle<NT1, NT2, NT3, NT4>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNToListToListToList<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回第四个表的查询列表
            /// </summary>
            public NNNNToListToListToListToList<NT1, NT2, NT3, NT4> ToList(int pageSize, int pageNow)
            {
                return new NNNNToListToListToListToList<NT1, NT2, NT3, NT4>();
            }

            /// <summary>
            /// 返回第四个表的查询对象
            /// </summary>
            public NNNNToListToListToListToSingle<NT1, NT2, NT3, NT4> ToSingle()
            {
                return new NNNNToListToListToListToSingle<NT1, NT2, NT3, NT4>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToListToListToListToList<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>());
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToListToListToListToSingle<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, T4> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), default(T4));
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNToListToListToSingle<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回第四个表的查询列表
            /// </summary>
            public NNNNToListToListToSingleToList<NT1, NT2, NT3, NT4> ToList(int pageSize, int pageNow)
            {
                return new NNNNToListToListToSingleToList<NT1, NT2, NT3, NT4>();
            }

            /// <summary>
            /// 返回第四个表的查询对象
            /// </summary>
            public NNNNToListToListToSingleToSingle<NT1, NT2, NT3, NT4> ToSingle()
            {
                return new NNNNToListToListToSingleToSingle<NT1, NT2, NT3, NT4>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToListToListToSingleToList<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, IEnumerable<T2>, T3, IEnumerable<T4>> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), default(T3), Enumerable.Empty<T4>());
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToListToListToSingleToSingle<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, IEnumerable<T2>, T3, T4> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), default(T3), default(T4));
            }
        }

        /// <summary>
        /// 返回对象
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNToListToSingle<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回第三个表的查询列表
            /// </summary>
            public NNNToListToSingleToList<NT1, NT2, NT3, NT4> ToList(int pageSize, int pageNow)
            {
                return new NNNToListToSingleToList<NT1, NT2, NT3, NT4>();
            }

            /// <summary>
            /// 返回第三个表的查询对象
            /// </summary>
            public NNNToListToSingleToSingle<NT1, NT2, NT3, NT4> ToSingle()
            {
                return new NNNToListToSingleToSingle<NT1, NT2, NT3, NT4>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNToListToSingleToList<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回第四个表的查询列表
            /// </summary>
            public NNNNToListToSingleToListToList<NT1, NT2, NT3, NT4> ToList(int pageSize, int pageNow)
            {
                return new NNNNToListToSingleToListToList<NT1, NT2, NT3, NT4>();
            }

            /// <summary>
            /// 返回第四个表的查询对象
            /// </summary>
            public NNNNToListToSingleToListToSingle<NT1, NT2, NT3, NT4> ToSingle()
            {
                return new NNNNToListToSingleToListToSingle<NT1, NT2, NT3, NT4>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToListToSingleToListToList<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, T2, IEnumerable<T3>> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), default(T2), Enumerable.Empty<T3>());
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToListToSingleToListToSingle<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, T2, T3> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), default(T2), default(T3));
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNToListToSingleToSingle<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回第四个表的查询列表
            /// </summary>
            public NNNNToListToSingleToSingleToList<NT1, NT2, NT3, NT4> ToList(int pageSize, int pageNow)
            {
                return new NNNNToListToSingleToSingleToList<NT1, NT2, NT3, NT4>();
            }

            /// <summary>
            /// 返回第四个表的查询对象
            /// </summary>
            public NNNNToListToSingleToSingleToSingle<NT1, NT2, NT3, NT4> ToSingle()
            {
                return new NNNNToListToSingleToSingleToSingle<NT1, NT2, NT3, NT4>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToListToSingleToSingleToList<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, T2, T3, IEnumerable<T4>> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), default(T2), default(T3), Enumerable.Empty<T4>());
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToListToSingleToSingleToSingle<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, T2, T3, T4> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), default(T2), default(T3), default(T4));
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NToSingle<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回第二个表的查询列表
            /// </summary>
            public NNToSingleToList<NT1, NT2, NT3, NT4> ToList(int pageSize, int pageNow)
            {
                return new NNToSingleToList<NT1, NT2, NT3, NT4>();
            }

            /// <summary>
            /// 返回第二个表的查询对象
            /// </summary>
            public NNToSingleToSingle<NT1, NT2, NT3, NT4> ToSingle()
            {
                return new NNToSingleToSingle<NT1, NT2, NT3, NT4>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNToSingleToList<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回第三个表的查询列表
            /// </summary>
            public NNNToSingleToListToList<NT1, NT2, NT3, NT4> ToList(int pageSize, int pageNow)
            {
                return new NNNToSingleToListToList<NT1, NT2, NT3, NT4>();
            }

            /// <summary>
            /// 返回第三个表的查询对象
            /// </summary>
            public NNNToSingleToListToSingle<NT1, NT2, NT3, NT4> ToSingle()
            {
                return new NNNToSingleToListToSingle<NT1, NT2, NT3, NT4>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNToSingleToListToList<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回第四个表的查询列表
            /// </summary>
            public NNNNToSingleToListToListToList<NT1, NT2, NT3, NT4> ToList(int pageSize, int pageNow)
            {
                return new NNNNToSingleToListToListToList<NT1, NT2, NT3, NT4>();
            }

            /// <summary>
            /// 返回第四个表的查询对象
            /// </summary>
            public NNNNToSingleToListToListToSingle<NT1, NT2, NT3, NT4> ToSingle()
            {
                return new NNNNToSingleToListToListToSingle<NT1, NT2, NT3, NT4>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToSingleToListToListToList<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>());
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToSingleToListToListToSingle<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, T4> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), default(T4));
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNToSingleToListToSingle<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回第四个表的查询列表
            /// </summary>
            public NNNNToSingleToListToSingleToList<NT1, NT2, NT3, NT4> ToList(int pageSize, int pageNow)
            {
                return new NNNNToSingleToListToSingleToList<NT1, NT2, NT3, NT4>();
            }

            /// <summary>
            /// 返回第四个表的查询对象
            /// </summary>
            public NNNNToSingleToListToSingleToSingle<NT1, NT2, NT3, NT4> ToSingle()
            {
                return new NNNNToSingleToListToSingleToSingle<NT1, NT2, NT3, NT4>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToSingleToListToSingleToList<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, IEnumerable<T2>, T3, IEnumerable<T4>> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), default(T3), Enumerable.Empty<T4>());
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToSingleToListToSingleToSingle<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, IEnumerable<T2>, T3, T4> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), default(T3), default(T4));
            }
        }

        /// <summary>
        /// 返回对象
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNToSingleToSingle<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回第三个表的查询列表
            /// </summary>
            public NNNToSingleToSingleToList<NT1, NT2, NT3, NT4> ToList(int pageSize, int pageNow)
            {
                return new NNNToSingleToSingleToList<NT1, NT2, NT3, NT4>();
            }

            /// <summary>
            /// 返回第三个表的查询对象
            /// </summary>
            public NNNToSingleToSingleToSingle<NT1, NT2, NT3, NT4> ToSingle()
            {
                return new NNNToSingleToSingleToSingle<NT1, NT2, NT3, NT4>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNToSingleToSingleToList<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回第四个表的查询列表
            /// </summary>
            public NNNNToSingleToSingleToListToList<NT1, NT2, NT3, NT4> ToList(int pageSize, int pageNow)
            {
                return new NNNNToSingleToSingleToListToList<NT1, NT2, NT3, NT4>();
            }

            /// <summary>
            /// 返回第四个表的查询对象
            /// </summary>
            public NNNNToSingleToSingleToListToSingle<NT1, NT2, NT3, NT4> ToSingle()
            {
                return new NNNNToSingleToSingleToListToSingle<NT1, NT2, NT3, NT4>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToSingleToSingleToListToList<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, T2, IEnumerable<T3>> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), default(T2), Enumerable.Empty<T3>());
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToSingleToSingleToListToSingle<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, T2, T3> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), default(T2), default(T3));
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNToSingleToSingleToSingle<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回第四个表的查询列表
            /// </summary>
            public NNNNToSingleToSingleToSingleToList<NT1, NT2, NT3, NT4> ToList(int pageSize, int pageNow)
            {
                return new NNNNToSingleToSingleToSingleToList<NT1, NT2, NT3, NT4>();
            }

            /// <summary>
            /// 返回第四个表的查询对象
            /// </summary>
            public NNNNToSingleToSingleToSingleToSingle<NT1, NT2, NT3, NT4> ToSingle()
            {
                return new NNNNToSingleToSingleToSingleToSingle<NT1, NT2, NT3, NT4>();
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToSingleToSingleToSingleToList<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, T2, T3, IEnumerable<T4>> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), default(T2), default(T3), Enumerable.Empty<T4>());
            }
        }

        /// <summary>
        /// 返回列表
        /// </summary>
        /// <typeparam name="NT1">查询结果对象1</typeparam>
        /// <typeparam name="NT2">查询结果对象2</typeparam>
        /// <typeparam name="NT3">查询结果对象3</typeparam>
        /// <typeparam name="NT4">查询结果对象4</typeparam>
        public struct NNNNToSingleToSingleToSingleToSingle<NT1, NT2, NT3, NT4>
        {
            /// <summary>
            /// 返回执行结果
            /// </summary>
            public Tuple<IEnumerable<T1>, T2, T3, T4> GetResult()
            {
                return Tuple.Create(Enumerable.Empty<T1>(), default(T2), default(T3), default(T4));
            }
        }
    }
}
