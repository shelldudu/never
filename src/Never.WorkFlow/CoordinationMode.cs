namespace Never.WorkFlow
{
    /// <summary>
    /// 协同方式，多个工作流是同时完成还是单一完成
    /// </summary>
    public enum CoordinationMode : byte
    {
        /// <summary>
        /// 任何一种
        /// </summary>
        Any = 0,

        /// <summary>
        /// 同时完成
        /// </summary>
        Meanwhile = 1,
    }
}