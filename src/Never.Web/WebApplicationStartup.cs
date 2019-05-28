using Never.IoC;
using Never.Web.IoC;
using Never.Web.IoC.Providers;

namespace Never.Web
{    /// <summary>
     /// Web程序宿主环境配置服务
     /// </summary>
    public class WebApplicationStartup : ApplicationStartup
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApplicationStartup"/> class.
        /// </summary>
        public WebApplicationStartup()
            : this(WebDomainAssemblyProvider.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApplicationStartup"/> class.
        /// </summary>
        /// <param name="assemblyProvider">程序集提供者</param>
        public WebApplicationStartup(IAssemblyProvider assemblyProvider)
            : base(assemblyProvider, (x) => { return new WebEasyContainer(x); })
        {

        }

        #endregion ctor

        /// <summary>
        /// 当前是否为web host环境
        /// </summary>
        public override bool IsWebHosted
        {
            get
            {
#if !NET461
                return false;
#else
                return System.Web.Hosting.HostingEnvironment.IsHosted;
#endif
            }
        }
    }
}