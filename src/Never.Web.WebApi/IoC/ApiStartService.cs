using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.Web.WebApi.IoC
{
    internal class ApiStartService : Never.Startups.IStartupService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnStarting(Startups.StartupContext context)
        {
            context.ServiceRegister.RegisterApiControllers(context.FilteringAssemblyProvider.GetAssemblies().ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        public int Order
        {
            get { return 35; }
        }
    }
}
