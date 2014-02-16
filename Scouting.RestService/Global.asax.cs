using System.Web.Mvc;
using System.Web.Routing;
using Funq;
using Scouting.RestService.Api;
using Scouting.RestService.App_Start;
using ServiceStack;

namespace Scouting.RestService
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            new AppHost().Init();
        }

        public class AppHost : AppHostBase
        {
            public AppHost() : base("Hello Web Services", typeof(TeamService).Assembly)
            {
                
            }

            public override void Configure(Container container)
            {
                SetConfig(new HostConfig { HandlerFactoryPath = "api" });
            }
        }
    }
}