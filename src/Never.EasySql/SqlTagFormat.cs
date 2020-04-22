using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.EasySql
{
    /// <summary>
    /// sqltag格式化
    /// </summary>
    public class SqlTagFormat
    {
        /// <summary>
        /// 读写器
        /// </summary>
        protected StringBuilder Writer { get; }

        /// <summary>
        /// 所有参数
        /// </summary>
        public List<KeyValuePair<string, object>> Parameters { get; }

        /// <summary>
        /// 所有参数Key
        /// </summary>
        public List<string> Keys { get; }

        /// <summary>
        /// 返回类型
        /// </summary>
        public string ReturnType { get; set; }

        /// <summary>
        /// 是否有if标签
        /// </summary>
        public bool IfContainer { get; set; }

        /// <summary>
        /// 所有参数
        /// </summary>
        public int IfThenCount { get; set; }

        /// <summary>
        /// 当前Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlTagFormat"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="parameterCounts">The parameter counts.</param>
        public SqlTagFormat(int capacity, int parameterCounts)
        {
            this.Writer = new StringBuilder(capacity);
            this.Parameters = new List<KeyValuePair<string, object>>(parameterCounts);
            this.Keys = new List<string>(parameterCounts);
        }

        #region parameter

        /// <summary>
        /// 新加参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="object"></param>
        internal void AddParameter(string key, object @object)
        {
            if (this.Keys.Any(o => o == key))
                return;

            this.Keys.Add(key);
            this.Parameters.Add(new KeyValuePair<string, object>(key, @object));
        }

        /// <summary>
        /// 新加参数
        /// </summary>
        /// <param name="parameter"></param>
        internal void AddParameter(KeyValueTuple<string, object> parameter)
        {
            if (this.Keys.Any(o => o == parameter.Key))
                return;

            if (parameter.Value is IReferceNullableParameter)
            {
                this.Keys.Add(parameter.Key);
                var value = ((IReferceNullableParameter)parameter.Value).Value;
                this.Parameters.Add(new KeyValuePair<string, object>(parameter.Key, value));
            }
            else
            {
                this.Keys.Add(parameter.Key);
                this.Parameters.Add(new KeyValuePair<string, object>(parameter.Key, parameter.Value));
            }
        }

        /// <summary>
        /// 是否文本参数
        /// </summary>
        /// <param name="parameterPosition"></param>
        /// <returns></returns>
        public virtual bool IfTextParameter(SqlTagParameterPosition parameterPosition)
        {
            return parameterPosition.TextParameter;
        }

        #endregion parameter

        #region write

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="value"></param>
        public virtual void Write(char value)
        {
            this.Writer.Append(value);
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="value"></param>
        public virtual void WriteOnTextMode(char value)
        {
            this.Writer.Append(value);
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="value"></param>
        public virtual void Write(string value)
        {
            this.Writer.Append(value);
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="value"></param>
        public virtual void WriteOnTextMode(string value)
        {
            this.Writer.Append(value);
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        public virtual void Write(string value, int startIndex, int count)
        {
            this.Writer.Append(value, startIndex, count);
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        public virtual void WriteOnTextMode(string value, int startIndex, int count)
        {
            this.Writer.Append(value, startIndex, count);
        }

        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Writer.ToString();
        }

        #endregion write

        #region text

        internal class TextSqlTagFormat : SqlTagFormat
        {
            public TextSqlTagFormat(int capacity, int parameterCounts) : base(capacity, parameterCounts)
            {
            }

            public override bool IfTextParameter(SqlTagParameterPosition parameterPosition)
            {
                return true;
            }
        }

        #endregion text
    }
}