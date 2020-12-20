using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Never.SqlClient
{
    /// <summary>
    /// 构造者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataEmitBuilder<T>
    {
        #region netsted

        /// <summary>
        /// 成员信息
        /// </summary>
        public struct DataMemberInfo
        {
            /// <summary>
            /// 成员
            /// </summary>
            public MemberInfo Member { get; set; }

            /// <summary>
            /// 主键配置
            /// </summary>
            public ColumnAttribute Column { get; set; }

            /// <summary>
            /// 类型处理
            /// </summary>
            public TypeHandlerAttribute TypeHandler { get; set; }

            /// <summary>
            /// 获取名字
            /// </summary>
            /// <returns></returns>
            public string GetMemberName()
            {
                if (this.Column != null && this.Column.Name.IsNotNullOrWhiteSpace())
                    return this.Column.Name;

                return this.Member.Name;
            }
        }

        #endregion

        /// <summary>
        /// 获取成员
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public List<DataMemberInfo> GetMembers(Type targetType)
        {
            return this.GetMembers(targetType, BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// 获取成员
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public List<DataMemberInfo> GetMembers(Type targetType, params BindingFlags[] bindingFlags)
        {
            BindingFlags flag = BindingFlags.Default;
            foreach (var f in bindingFlags)
                flag = flag | f;

            return this.GetMembers(targetType, flag);
        }

        /// <summary>
        /// 获取成员
        /// </summary>
        public virtual List<DataMemberInfo> GetMembers(Type targetType, BindingFlags bindingFlags)
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
                    if (p.CanWrite)
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
                    if (f.IsInitOnly)
                        continue;

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
    }
}
