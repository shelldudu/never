namespace Never.IoC
{
    /// <summary>
    /// 组件生命周期
    /// </summary>
    public enum ComponentLifeStyle
    {
        /// <summary>
        /// 单例:任何情况下返回相同实例
        /// </summary>
        Singleton = 0,

        /// <summary>
        /// 嵌套:每个作用域内只会产生一个实例
        /// </summary>
        Scoped = 1,

        /// <summary>
        /// 短暂:每次请求都会返回单独的实例
        /// </summary>
        Transient = 2
    }
}