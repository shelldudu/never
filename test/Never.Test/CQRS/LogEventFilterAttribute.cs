using Never.Aop.DomainFilters;
using Never.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Never.Test
{
    public class LogEventFilterAttribute1 : Aop.DomainFilters.EventHandlerFilterAttribute
    {
        public override void OnActionExecuting(IEventContext context, IEvent e1)
        {
            Console.WriteLine("action_handler_one_param");
        }
    }

    public class LogEventFilterAttribute : Aop.DomainFilters.EventHandlerFilterAttribute
    {
        public override void OnActionExecuting(IEventContext context, IEvent @event)
        {
            Console.WriteLine("action_handler_two_param");
        }
    }
}