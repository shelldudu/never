using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Never.SqlClient
{
    /// <summary>
    /// sql语句拼接
    /// </summary>
    public class SqlParamerBuilder
    {
        #region field

        /// <summary>
        /// 当前语句
        /// </summary>
        private string sql = string.Empty;

        /// <summary>
        /// 分析sql语句中的参数
        /// </summary>
        private readonly static Regex rxParamers = new Regex(@"(?<prefix>(?<![?@:])[?@:](?![?@:]))(?<name>\w+)", RegexOptions.Compiled);

        /// <summary>
        /// 记录参数
        /// </summary>
        private List<KeyValuePair<string, object>> parameters = null;

        /// <summary>
        /// 已经构建了
        /// </summary>
        private bool builded = false;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlParamerBuilder"/> class.
        /// </summary>
        internal SqlParamerBuilder(string sql)
        {
            this.sql = sql ?? string.Empty;
        }

        #endregion ctor

        #region init

        /// <summary>
        /// 记录参数
        /// </summary>
        public KeyValuePair<string, object>[] Paramters
        {
            get
            {
                return this.parameters == null ? new KeyValuePair<string, object>[0] : this.parameters.ToArray();
            }
        }

        /// <summary>
        /// 准备参数
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public SqlParamerBuilder Build(string prefix, object @parameter)
        {
            if (string.IsNullOrEmpty(this.sql))
                return this;

            if (this.builded)
                return this;

            var table = PreParameters(@parameter);
            if (table == null || table.Count == 0)
            {
                this.parameters = new List<KeyValuePair<string, object>>();
                return this;
            }

            this.parameters = new List<KeyValuePair<string, object>>(table.Count);
            var keys = new List<string>(table.Count);
            sql = rxParamers.Replace(sql, m =>
            {
                var name = m.Groups["name"].Value;
                if (!table.ContainsKey(name))
                {
                    var i = m.Groups["name"].Index;
                    var count = 0;
                    while (i >= 0)
                    {
                        if (this.sql[i] == '\'')
                        {
                            count = 1;
                            break;
                        }
                        i--;
                    }

                    if (count > 0)
                    {
                        i = m.Groups["name"].Index;
                        while (i < this.sql.Length)
                        {
                            if (this.sql[i] == '\'')
                            {
                                count = 2;
                                break;
                            }
                            i++;
                        }
                    }

                    if (count != 2)
                        throw new Exception(string.Format("当前在sql语句中参数为{0}的值在所提供的参数列表中找不到", name));

                    return string.Concat(this.sql[m.Groups["name"].Index - 1], name);
                }

                var stValue = table[name] as string;
                if (stValue != null)
                {
                    if (!keys.Contains(name))
                    {
                        keys.Add(name);
                        parameters.Add(new KeyValuePair<string, object>(name, table[name]));
                    }

                    return string.Concat(prefix, name);
                }

                var value = table[name] as IEnumerable;
                if (value != null)
                {
                    int totalCount = 0;
                    var ator = value.GetEnumerator();
                    var newNameList = new List<string>(50);
                    while (ator.MoveNext())
                    {
                        totalCount++;
                        var newkey = string.Format("{0}{1}x{2}z", prefix, name, totalCount);
                        newNameList.Add(newkey);
                        parameters.Add(new KeyValuePair<string, object>(newkey, ator.Current));
                    }

                    return string.Concat(string.Join(",", newNameList.ToArray()));
                }

                if (!keys.Contains(name))
                {
                    keys.Add(name);
                    parameters.Add(new KeyValuePair<string, object>(name, table[name]));
                }

                return string.Concat(prefix, name);
            });

            this.builded = true;
            keys.Clear();
            return this;
        }

        /// <summary>
        /// 准备参数
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public SqlParamerBuilder Build(string prefix, KeyValuePair<string, object>[] @parameter)
        {
            if (string.IsNullOrEmpty(this.sql))
                return this;

            if (this.builded)
                return this;

            if (@parameter == null || @parameter.Length == 0)
            {
                this.parameters = new List<KeyValuePair<string, object>>();
                return this;
            }

            this.parameters = new List<KeyValuePair<string, object>>(@parameter.Length);
            var keys = new List<string>(@parameter.Length);
            sql = rxParamers.Replace(sql, m =>
            {
                var name = m.Groups["name"].Value;
                foreach (var k in @parameter)
                {
                    if (k.Key == name)
                    {
                        var stValue = k.Value as string;
                        if (stValue != null)
                        {
                            if (!keys.Contains(name))
                            {
                                keys.Add(name);
                                parameters.Add(new KeyValuePair<string, object>(name, k.Value));
                            }

                            return string.Concat(prefix, name);
                        }

                        var value = k.Value as IEnumerable;
                        if (value != null)
                        {
                            int totalCount = 0;
                            var ator = value.GetEnumerator();
                            var newNameList = new List<string>(0);
                            while (ator.MoveNext())
                            {
                                totalCount++;
                                var newkey = string.Format("{0}{1}x{2}z", prefix, name, totalCount);
                                newNameList.Add(newkey);
                                parameters.Add(new KeyValuePair<string, object>(newkey, ator.Current));
                            }

                            return string.Concat(string.Join(",", newNameList.ToArray()));
                        }

                        if (!keys.Contains(name))
                        {
                            keys.Add(name);
                            parameters.Add(new KeyValuePair<string, object>(name, k.Value));
                            return string.Concat(prefix, name);
                        }
                    }
                }

                var i = m.Groups["name"].Index;
                var count = 0;
                while (i >= 0)
                {
                    if (this.sql[i] == '\'')
                    {
                        count = 1;
                        break;
                    }
                    i--;
                }

                if (count > 0)
                {
                    i = m.Groups["name"].Index;
                    while (i < this.sql.Length)
                    {
                        if (this.sql[i] == '\'')
                        {
                            count = 2;
                            break;
                        }
                        i++;
                    }
                }

                if (count != 2)
                    throw new Exception(string.Format("当前在sql语句中参数为{0}的值在所提供的参数列表中找不到", name));

                return string.Concat(this.sql[m.Groups["name"].Index - 1], name);
            });

            this.builded = true;
            keys.Clear();
            return this;
        }

        /// <summary>
        /// 准备参数
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static Hashtable PreParameters(object @parameter)
        {
            if (parameter == null)
                return null;

            var table = @parameter as Hashtable;
            if (table != null)
                return table;

            table = new Hashtable();
            var pi = @parameter.GetType().GetProperties();
            if (pi != null)
            {
                foreach (var p in pi)
                {
                    var value = p.GetValue(@parameter, null);
                    var attributes = p.GetCustomAttributes(typeof(TypeHandlerAttribute), true);
                    if (attributes.IsNullOrEmpty())
                    {
                        table[p.Name] = value;
                        continue;
                    }

                    foreach (TypeHandlerAttribute attribute in attributes)
                    {
                        if (attribute.TypeHandler.IsAssignableFromType(typeof(ICastingValueToParameterTypeHandler<>)))
                        {
                            table[p.Name] = attribute.GetOnInitingParameterCallBack()(attribute, value);
                            break;
                        }
                    }
                }
            }

            return table;
        }

        #endregion init

        #region tostring

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.sql;
        }

        #endregion tostring
    }
}