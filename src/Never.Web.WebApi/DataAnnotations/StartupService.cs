using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Never.IoC;

namespace Never.Web.WebApi.DataAnnotations
{
    /// <summary>
    ///
    /// </summary>
    internal class StartupService : Never.Startups.IStartupService
    {
        #region ctor

        /// <summary>
        ///
        /// </summary>
        public StartupService()
        {
        }

        #endregion ctor

        #region IStartupService

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        void Startups.IStartupService.OnStarting(Startups.StartupContext context)
        {
            context.ProcessType(new TypeProcessor());
        }

        /// <summary>
        ///
        /// </summary>
        int Startups.IStartupService.Order
        {
            get { return 120; }
        }

        #endregion IStartupService
    }
}