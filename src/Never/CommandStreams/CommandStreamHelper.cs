using System.Text.RegularExpressions;

namespace Never.CommandStreams
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class CommandStreamHelper
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="operaCommand"></param>
        /// <returns></returns>
        public static CommandStreamMessage ConvertTo(IOperateCommand operaCommand)
        {
            return ConvertTo(operaCommand, Never.Serialization.SerializeEnvironment.JsonSerializer);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="operaCommand"></param>
        /// <param name="jsonSerializer"></param>
        /// <returns></returns>
        public static CommandStreamMessage ConvertTo(IOperateCommand operaCommand, Serialization.IJsonSerializer jsonSerializer)
        {
            var type = operaCommand.Command.GetType();
            var message = new CommandStreamMessage()
            {
                AppDomain = operaCommand.AppDomain,
                CreateDate = operaCommand.CreateDate,
                Creator = operaCommand.Creator,
                CommandContent = jsonSerializer.SerializeObject(operaCommand.Command),
                CommandName = type.Name,
                CommandType = Regex.Replace(type.AssemblyQualifiedName, "version=(.*?),", "", RegexOptions.IgnoreCase),
                HashCode = operaCommand.HashCode,
                Increment = operaCommand.Increment,
                AggregateId = operaCommand.AggregateId,
                AggregateIdType = Regex.Replace(operaCommand.AggregateIdType.AssemblyQualifiedName, "version=(.*?),", "", RegexOptions.IgnoreCase),
                Version = operaCommand.Version,
            };

            return message;
        }
    }
}