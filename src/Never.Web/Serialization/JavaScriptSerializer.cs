#if !NET461
#else

using Never.Reflection;
using Never.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace Never.Web.Serialization
{
    /// <summary>
    /// javascript对象序列化
    /// </summary>
    public struct JavaScriptSerializer : IJsonSerializer
    {
        #region field

#if NET35
        private readonly static Hashtable typeAction = new Hashtable(200);
#endif

        /// <summary>
        /// The ser
        /// </summary>
        private readonly static System.Web.Script.Serialization.JavaScriptSerializer ser = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes static members of the <see cref="JavaScriptSerializer"/> class.
        /// </summary>
        static JavaScriptSerializer()
        {
            ser = new System.Web.Script.Serialization.JavaScriptSerializer();
        }

        #endregion ctor

        #region 序列化

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="object">源对象</param>
        /// <returns></returns>
        public string SerializeObject(object @object)
        {
            if (@object == null)
            {
                return string.Empty;
            }

            return ser.Serialize(@object);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="object">源对象</param>
        /// <returns></returns>
        public string Serialize<T>(T @object)
        {
            if (@object == null)
            {
                return string.Empty;
            }

            return ser.Serialize(@object);
        }

        #endregion 序列化

        #region 反序列化

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="input">源字符串</param>
        /// <returns></returns>
        public T Deserialize<T>(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return default(T);
            }

            return (T)this.DeserializeObject(input, typeof(T));
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public object DeserializeObject(string input, Type targetType)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

#if NET35
            var action = typeAction[targetType] as Func<JavaScriptSerializer, string, object>;
            if (action == null)
            {
                var members = typeof(JavaScriptConverter).GetMembers(BindingFlags.Instance | BindingFlags.Public);
                foreach (var member in members)
                {
                    if (member.MemberType != MemberTypes.Method)
                        continue;

                    if (member.Name != "Deserialize")
                        continue;

                    var method = (MethodInfo)member;
                    if (!method.IsGenericMethod)
                        continue;

                    var emit = EasyEmitBuilder<Func<JavaScriptSerializer, string, object>>.NewDynamicMethod();
                    emit.LoadArgument(0);
                    emit.LoadArgument(1);
                    emit.Call(method.MakeGenericMethod(targetType));
                    emit.Return();
                    typeAction[targetType] = action = emit.CreateDelegate();

                    return action.Invoke(this, input);
                }
            }

            return action.Invoke(this, input);
#else
            return ser.Deserialize(input, targetType);
#endif
        }

        #endregion 反序列化
    }
}

#endif