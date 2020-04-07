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
    public abstract class EasySqlParameter<T> : DataEmitBuilder<T>
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

            this.Members = new List<KeyValueTuple<string, object>>();
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

        /// <summary>
        /// 参数，用于过滤或者增加一些自己的参数
        /// </summary>
        public IEnumerable<KeyValueTuple<string, object>> Members { get; }

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
                var dictionary = new List<KeyValueTuple<string, object>>(ator.Count);
                foreach (var key in ator.Keys)
                {
                    if (key == null)
                        continue;

                    dictionary.Add(new KeyValueTuple<string, object>(key.ToString(), ator[key]));
                }

                foreach (var member in this.Members)
                {
                    if (dictionary.Any(ta => ta.Key.IsEquals(member.Key, StringComparison.OrdinalIgnoreCase)))
                    {
                        dictionary.RemoveAll(ta => ta.Key.IsEquals(member.Key, StringComparison.OrdinalIgnoreCase));
                        dictionary.Add(member);
                        continue;
                    }

                    dictionary.Add(member);
                }

                if (this.Count <= 0)
                    this.Count = dictionary.Count;

                return dictionary.AsReadOnly();
            }
            else if (this.IsIList)
            {
                return this.Members.AsReadOnly();
            }
            else if (this.IsICollection)
            {
                return this.Members.AsReadOnly();
            }

            var members = this.GetMembers(objectType);
            var list = new List<KeyValueTuple<string, object>>(members.Count());
            foreach (var member in members)
            {
                switch (member.Member.MemberType)
                {
                    case System.Reflection.MemberTypes.Property:
                        {
                            try
                            {
                                var p = member.Member as System.Reflection.PropertyInfo;
                                var value = p.GetValue(target);
                                if (member.TypeHandler == null || member.TypeHandler.TypeHandler == null)
                                {
                                    {
                                        list.Add(new KeyValueTuple<string, object>(member.GetMemberName(), value));
                                        continue;
                                    }
                                }
                                if (member.TypeHandler.TypeHandler.IsAssignableFromType(typeof(ICastingValueToParameterTypeHandler<>)))
                                {
                                    list.Add(new KeyValueTuple<string, object>(member.GetMemberName(), member.TypeHandler.GetOnInitingParameterCallBack()(member.TypeHandler, value)));
                                    continue;
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
                                var f = member.Member as System.Reflection.FieldInfo;
                                var value = f.GetValue(target);
                                if (member.TypeHandler == null || member.TypeHandler.TypeHandler == null)
                                {
                                    {
                                        list.Add(new KeyValueTuple<string, object>(member.GetMemberName(), value));
                                        continue;
                                    }
                                }
                                if (member.TypeHandler.TypeHandler.IsAssignableFromType(typeof(ICastingValueToParameterTypeHandler<>)))
                                {
                                    list.Add(new KeyValueTuple<string, object>(member.GetMemberName(), member.TypeHandler.GetOnInitingParameterCallBack()(member.TypeHandler, value)));
                                    continue;
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

            foreach (var member in this.Members)
            {
                if (list.Any(ta => ta.Key.IsEquals(member.Key, StringComparison.OrdinalIgnoreCase)))
                {
                    list.RemoveAll(ta => ta.Key.IsEquals(member.Key, StringComparison.OrdinalIgnoreCase));
                    list.Add(member);
                    continue;
                }

                list.Add(member);
            }

            if (this.Count <= 0)
                this.Count = list.Count;

            return list.AsReadOnly();
        }

        /// <summary>
        /// 获取成员
        /// </summary>
        public override List<DataMemberInfo> GetMembers(Type targetType, BindingFlags bindingFlags)
        {
            var members = targetType.GetMembers(bindingFlags);
            if (members == null || members.Length == 0)
                return new List<DataMemberInfo>(0);

            var list = new List<DataMemberInfo>(members.Length);
            foreach (var member in members)
            {
                var column = member.GetAttribute<ColumnAttribute>();
                var typehandler = member.GetAttribute<TypeHandlerAttribute>();
                if (member.MemberType == MemberTypes.Property)
                {
                    var p = (PropertyInfo)member;
                    if (p.CanRead)
                    {
                        list.Add(new DataMemberInfo()
                        {
                            Member = member,
                            Column = column,
                            TypeHandler = typehandler,
                        });
                    }
                }
                else if (member.MemberType == MemberTypes.Field)
                {
                    var f = (FieldInfo)member;
                    list.Add(new DataMemberInfo()
                    {
                        Member = member,
                        Column = column,
                        TypeHandler = typehandler,
                    });
                }
            }

            return list;
        }

        /// <summary>
        /// 新加一些参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        internal EasySqlParameter<T> ReplaceParameter(string key, object parameter)
        {
            return this.ReplaceParameter(new KeyValueTuple<string, object>(key, parameter));
        }

        /// <summary>
        /// 新加一些参数
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        internal EasySqlParameter<T> ReplaceParameter(KeyValueTuple<string, object> parameter)
        {
            ((List<KeyValueTuple<string, object>>)this.Members).RemoveAll(t => t.Key.IsEquals(parameter.Key));
            ((List<KeyValueTuple<string, object>>)this.Members).Add(parameter);
            return this;
        }

        #endregion convey
    }
}