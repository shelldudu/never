namespace Never.EventStreams
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class EventStreamExtension
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="operaEvent"></param>
        /// <returns></returns>
        public static EventStreamMessage ConvertTo(this IOperateEvent operaEvent)
        {
            return EventStreamHelper.ConvertTo(operaEvent);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="operaEvent"></param>
        /// <param name="jsonSerializer"></param>
        /// <returns></returns>
        public static EventStreamMessage ConvertTo(this IOperateEvent operaEvent, Serialization.IJsonSerializer jsonSerializer)
        {
            return EventStreamHelper.ConvertTo(operaEvent, jsonSerializer);
        }
    }
}