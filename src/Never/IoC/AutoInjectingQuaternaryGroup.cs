namespace Never.IoC
{
    /// <summary>
    /// 自动注入分组
    /// </summary>
    public class AutoInjectingQuaternaryGroup : AutoInjectingTurpeGroup
    {
        /// <summary>
        /// 注入Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 注入生命周期
        /// </summary>
        public ComponentLifeStyle LifeStyle { get; set; }
    }
}