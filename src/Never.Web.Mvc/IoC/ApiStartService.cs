using System.Linq;

namespace Never.Web.Mvc.IoC
{
    internal class ApiStartService : Never.Startups.IStartupService
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        public void OnStarting(Startups.StartupContext context)
        {
            context.ServiceRegister.RegisterMvcControllers(context.FilteringAssemblyProvider.GetAssemblies().ToArray());
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