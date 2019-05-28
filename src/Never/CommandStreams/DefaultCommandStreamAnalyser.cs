using Never.Aop.DomainFilters;
using Never.Commands;
using Never.Domains;
using System;
using System.Text.RegularExpressions;

namespace Never.CommandStreams
{
    /// <summary>
    /// 命令分析者
    /// </summary>
    public class DefaultCommandStreamAnalyser : ICommandStreamAnalyser
    {
        /// <summary>
        /// 分析命令
        /// </summary>
        /// <typeparam name="TCommand">命令类型</typeparam>
        /// <param name="context">命令上下文</param>
        /// <param name="command">命令</param>
        /// <returns></returns>
        public IOperateCommand Analyse<TCommand>(ICommandContext context, TCommand command) where TCommand : ICommand
        {
            var attributes = CommandBehaviorStorager.Default.GetAttributes(typeof(TCommand));
            var ignoreAttr = ObjectExtension.GetAttribute<IgnoreStoreCommandAttribute>(attributes);
            if (ignoreAttr != null)
                return null;

            var attrbute = ObjectExtension.GetAttribute<CommandDomainAttribute>(attributes);
            var guid = command as IAggregateCommand<Guid>;
            if (guid != null)
            {
                return new DefaultOperateCommand()
                {
                    AggregateId = guid.AggregateId.ToString(),
                    AppDomain = attrbute == null ? "" : attrbute.Domain,
                    Command = command,
                    CommandType = Regex.Replace(typeof(TCommand).AssemblyQualifiedName, "version=(.*?),", "", RegexOptions.IgnoreCase),
                    CommandTypeFullName = typeof(TCommand).FullName,
                    CreateDate = DateTime.Now,
                    AggregateIdType = typeof(Guid),
                    Creator = ObjectExtension.GetWorkerName(context.Worker),
                    HashCode = command.GetHashCode(),
                    Version = guid.Version,
                    Increment = DefaultOperateCommand.NextIncrement,
                };
            }

            var str = command as IAggregateCommand<string>;
            if (str != null)
            {
                return new DefaultOperateCommand()
                {
                    AggregateId = str.AggregateId.ToString(),
                    AppDomain = attrbute == null ? "" : attrbute.Domain,
                    Command = command,
                    CommandType = Regex.Replace(typeof(TCommand).AssemblyQualifiedName, "version=(.*?),", "", RegexOptions.IgnoreCase),
                    CommandTypeFullName = typeof(TCommand).FullName,
                    CreateDate = DateTime.Now,
                    AggregateIdType = typeof(string),
                    Creator = ObjectExtension.GetWorkerName(context.Worker),
                    HashCode = command.GetHashCode(),
                    Version = str.Version,
                    Increment = DefaultOperateCommand.NextIncrement,
                };
            }

            var @long = command as IAggregateCommand<long>;
            if (@long != null)
            {
                return new DefaultOperateCommand()
                {
                    AggregateId = @long.AggregateId.ToString(),
                    AppDomain = attrbute == null ? "" : attrbute.Domain,
                    Command = command,
                    CommandType = Regex.Replace(typeof(TCommand).AssemblyQualifiedName, "version=(.*?),", "", RegexOptions.IgnoreCase),
                    CommandTypeFullName = typeof(TCommand).FullName,
                    CreateDate = DateTime.Now,
                    AggregateIdType = typeof(long),
                    Creator = ObjectExtension.GetWorkerName(context.Worker),
                    HashCode = command.GetHashCode(),
                    Version = @long.Version,
                    Increment = DefaultOperateCommand.NextIncrement,
                };
            }

            var @int = command as IAggregateCommand<int>;
            if (@int != null)
            {
                return new DefaultOperateCommand()
                {
                    AggregateId = @int.AggregateId.ToString(),
                    AppDomain = attrbute == null ? "" : attrbute.Domain,
                    Command = command,
                    CommandType = Regex.Replace(typeof(TCommand).AssemblyQualifiedName, "version=(.*?),", "", RegexOptions.IgnoreCase),
                    CommandTypeFullName = typeof(TCommand).FullName,
                    CreateDate = DateTime.Now,
                    AggregateIdType = typeof(int),
                    Creator = ObjectExtension.GetWorkerName(context.Worker),
                    HashCode = command.GetHashCode(),
                    Version = @int.Version,
                    Increment = DefaultOperateCommand.NextIncrement,
                };
            }
            return null;
        }
    }
}