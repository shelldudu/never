using Never.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// sql参数，只接受key-value这种形式的对象，如果是value文本参数，请传入<see cref="KeyValueEasySqlParameter{T}"/>对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EasySqlParameter<T>
    {
        #region ctor

        /// <summary>
        /// sql参数，只接受key-value这种形式的对象，如果是value文本参数，请传入<see cref="KeyValueEasySqlParameter{T}"/>对象
        /// </summary>
        /// <param name="object"></param>
        protected EasySqlParameter(T @object)
        {
            this.Object = @object;
            this.IsIEnumerable = this.Object is IEnumerable;
            if (this.IsIEnumerable)
            {
                if (this.Object is ICollection)
                {
                    this.IsICollection = true;
                    this.IsIDictionary = this.Object is IDictionary;
                    this.IsIList = this.Object is IList;
                    this.Count = ((ICollection)this.Object).Count;
                }
            }
        }

        #endregion ctor

        #region prop

        /// <summary>
        /// 是否为迭代参数
        /// </summary>
        public bool IsIEnumerable { get; private set; }

        /// <summary>
        /// 是否为ICollection接口
        /// </summary>
        public bool IsICollection { get; private set; }

        /// <summary>
        /// 是否为IList接口
        /// </summary>
        public bool IsIList { get; private set; }

        /// <summary>
        /// 是否为IsIDictionary接口
        /// </summary>
        public bool IsIDictionary { get; private set; }

        /// <summary>
        /// 参数个数
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// T对象
        /// </summary>
        public T @Object { get; private set; }

        #endregion prop

        #region convey

        /// <summary>
        /// 转换参数
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<KeyValueTuple<string, object>> Convert()
        {
            return this.Convert(this.@Object, typeof(T));
        }

        /// <summary>
        /// 转换参数
        /// </summary>
        /// <param name="target"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected virtual IReadOnlyList<KeyValueTuple<string, object>> Convert(T target, Type objectType)
        {
            if (this.IsIDictionary)
            {
                var ator = target as IDictionary;
                var diction = new List<KeyValueTuple<string, object>>(ator.Count);
                foreach (var key in ator.Keys)
                {
                    if (key == null)
                        continue;

                    diction.Add(new KeyValueTuple<string, object>(key.ToString(), ator[key]));
                }

                if (this.Count <= 0)
                    this.Count = diction.Count;

                return diction.AsReadOnly();
            }
            else if (this.IsIList)
            {
                return new List<KeyValueTuple<string, object>>(0).AsReadOnly();
                //var ator = target as IList;
                //list = new List<KeyValueTuple<string, object>>(ator.Count);
                //foreach (var key in ator.Keys)
                //{
                //    if (key == null)
                //        continue;

                //    list.Add(new KeyValueTuple<string, object>(key.ToString(), ator[key]));
                //}

                //return list.AsReadOnly();
            }
            else if (this.IsICollection)
            {
                return new List<KeyValueTuple<string, object>>(0).AsReadOnly();
                //var ator = target as ICollection;
                //list = new List<KeyValueTuple<string, object>>(ator.Count);
                //foreach (var key in ator.Keys)
                //{
                //    if (key == null)
                //        continue;

                //    list.Add(new KeyValueTuple<string, object>(key.ToString(), ator[key]));
                //}

                //return list.AsReadOnly();
            }

            var members = objectType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            var list = new List<KeyValueTuple<string, object>>(members.Count());
            foreach (var member in members)
            {
                switch (member.MemberType)
                {
                    case System.Reflection.MemberTypes.Property:
                        {
                            try
                            {
                                var p = member as System.Reflection.PropertyInfo;
                                if (!p.CanRead)
                                    continue;

                                var value = p.GetValue(target);
                                var attributes = p.GetCustomAttributes(typeof(TypeHandlerAttribute), true);
                                if (attributes.IsNullOrEmpty())
                                {
                                    list.Add(new KeyValueTuple<string, object>(member.Name, value));
                                    continue;
                                }

                                foreach (TypeHandlerAttribute attribute in attributes)
                                {
                                    if (attribute.TypeHandler.IsAssignableFromType(typeof(ICastingValueToParameterTypeHandler<>)))
                                    {
                                        list.Add(new KeyValueTuple<string, object>(member.Name, attribute.GetOnInitingParameterCallBack()(attribute, value)));
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                                throw;
                            }
                        }
                        break;

                    case System.Reflection.MemberTypes.TypeInfo:
                        {
                            try
                            {
                                var f = member as System.Reflection.FieldInfo;
                                var value = f.GetValue(target);
                                var attributes = f.GetCustomAttributes(typeof(TypeHandlerAttribute), true);
                                if (attributes.IsNullOrEmpty())
                                {
                                    list.Add(new KeyValueTuple<string, object>(member.Name, value));
                                    continue;
                                }

                                foreach (TypeHandlerAttribute attribute in attributes)
                                {
                                    if (attribute.TypeHandler.IsAssignableFromType(typeof(ICastingValueToParameterTypeHandler<>)))
                                    {
                                        list.Add(new KeyValueTuple<string, object>(member.Name, attribute.GetOnInitingParameterCallBack()(attribute, value)));
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                                throw;
                            }
                        }
                        break;
                }
            }

            if (this.Count <= 0)
                this.Count = list.Count;

            return list.AsReadOnly();
        }

        #endregion convey
    }
}