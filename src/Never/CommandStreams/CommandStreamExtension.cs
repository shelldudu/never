namespace Never.CommandStreams
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class CommandStreamExtension
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="operaCommand"></param>
        /// <returns></returns>
        public static CommandStreamMessage ConvertTo(this IOperateCommand operaCommand)
        {
            return CommandStreamHelper.ConvertTo(operaCommand);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="operaCommand"></param>
        /// <param name="jsonSerializer"></param>
        /// <returns></returns>
        public static CommandStreamMessage ConvertTo(this IOperateCommand operaCommand, Serialization.IJsonSerializer jsonSerializer)
        {
            return CommandStreamHelper.ConvertTo(operaCommand, jsonSerializer);
        }
    }
}