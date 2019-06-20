using System;

namespace Never.IoC
{
    /// <summary>
    /// 作用域自动注入属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
    public class ScopedAutoInjectingAttribute : AutoInjectingAttribute
    {
        /// <summary>
        /// 声明生命周期
        /// </summary>
        public override ComponentLifeStyle Declare { get { return ComponentLifeStyle.Scoped; } }

        /// <summary>
        /// 环境
        /// </summary>
        public string Env { get; }

        /// <summary>
        /// 注入类型
        /// </summary>
        public override Type ServiceType { get; }

        /// <summary>
        /// 注入的Key
        /// </summary>
        public override string Key { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="env"></param>
        /// <param name="serviceType"></param>
        public ScopedAutoInjectingAttribute(string env, Type serviceType) : this(env, serviceType, null)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="env"></param>
        /// <param name="serviceType"></param>
        /// <param name="key"></param>
        public ScopedAutoInjectingAttribute(string env, Type serviceType, string key)
        {
            this.Env = env;
            this.ServiceType = serviceType;
            this.Key = key;
        }
    }
}