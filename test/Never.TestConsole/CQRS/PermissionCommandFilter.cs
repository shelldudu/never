using Never.Commands;
using Never.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Never.TestConsole.CQRS
{
    public class PermissionCommandFilter : Aop.DomainFilters.CommandHandlerFilterAttribute
    {
        public override void OnActionExecuting(ICommandContext context, ICommand command)
        {
            Console.WriteLine("prm_handler_act_param");
        }
    }
}