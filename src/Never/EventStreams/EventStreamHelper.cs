using System.Text.RegularExpressions;

namespace Never.EventStreams
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class EventStreamHelper
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="operaEvent"></param>
        /// <returns></returns>
        public static EventStreamMessage ConvertTo(IOperateEvent operaEvent)
        {
            return ConvertTo(operaEvent, Never.Serialization.SerializeEnvironment.JsonSerializer);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="operaEvent"></param>
        /// <param name="jsonSerializer"></param>
        /// <returns></returns>
        public static EventStreamMessage ConvertTo(IOperateEvent operaEvent, Serialization.IJsonSerializer jsonSerializer)
        {
            var type = operaEvent.Event.GetType();
            var message = new EventStreamMessage()
            {
                AggregateType = operaEvent.AggregateType.FullName,
                AppDomain = operaEvent.AppDomain,
                CreateDate = operaEvent.CreateDate,
                Creator = operaEvent.Creator,
                EventContent = jsonSerializer.SerializeObject(operaEvent.Event),
                EventName = type.Name,
                EventType = Regex.Replace(type.AssemblyQualifiedName, "version=(.*?),", "", RegexOptions.IgnoreCase),
                HashCode = operaEvent.HashCode,
                Increment = operaEvent.Increment,
                AggregateId = operaEvent.AggregateId,
                AggregateIdType = Regex.Replace(operaEvent.AggregateIdType.AssemblyQualifiedName, "version=(.*?),", "", RegexOptions.IgnoreCase),
                Version = operaEvent.Version,
            };

            return message;
        }
    }
}