namespace Never.IoC
{
    /// <summary>
    /// 生命周期的自动注入属性
    /// </summary>
    public abstract class LifeStyleAutoInjectingAttribute : BaseAutoInjectingAttribute
    {
        /// <summary>
        /// 注入的Key
        /// </summary>
        public virtual string Key { get; }

        /// <summary>
        /// 声明生命周期
        /// </summary>
        public abstract ComponentLifeStyle Declare { get; }
    }
}