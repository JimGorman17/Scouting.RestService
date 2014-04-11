using System;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using Funq;
using Scouting.DataLayer;
using Scouting.DataLayer.Models;
using Scouting.DataLayer.Repositories;
using Scouting.RestService.Api;
using Scouting.RestService.App_Start;
using Scouting.RestService.Dtos;
using ServiceStack;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;

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

            //SqlServerTypes.Utilities.LoadNativeAssemblies(Server.MapPath("~/bin")); The NuGet documentation said that this might be necessary.

            Mapper.CreateMap<GooglePlusLoginDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ForMember(dest => dest.GoogleId, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.GivenName, opt => opt.MapFrom(src => src.given_name))
                .ForMember(dest => dest.FamilyName, opt => opt.MapFrom(src => src.family_name))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.link))
                .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.picture))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.gender))
                .ForMember(dest => dest.Locale, opt => opt.MapFrom(src => src.locale))
                .ForMember(dest => dest.FavoriteTeamId, opt => opt.Ignore())
                .ForMember(dest => dest.IsAdmin, opt => opt.Ignore());

            Mapper.AssertConfigurationIsValid();

            new AppHost().Init();
        }

        public class AppHost : AppHostBase
        {
            public AppHost() : base("Scouting.RestService", typeof(TeamService).Assembly)
            {
                
            }

            public override void Configure(Container container)
            {
                SetConfig(new EndpointHostConfig { ServiceStackHandlerFactoryPath = "api" });

                container.Register(c => new Database("localDB"));
                container.RegisterAutoWired<CommentRepository>();
                container.RegisterAutoWired<PlayerRepository>();
                container.RegisterAutoWired<Repository<Team>>();
                container.RegisterAutoWired<UserRepository>();
                container.RegisterAutoWired<AuthTokenRepository>();
                container.RegisterAutoWired<Repository<ErrorLog>>();
                container.RegisterAutoWired<FlaggedCommentRepository>();
                
                ServiceExceptionHandler = (req, request, exception) =>
                    {
                        var errorLog = new ErrorLog
                            {
                                Application = "Scouting.RestService",
                                CreateDate = DateTimeOffset.Now,
                                Message = exception.Message,
                                StackTrace = exception.StackTrace
                            };
                        container.Resolve<Repository<ErrorLog>>().Add(errorLog);

                        return DtoUtils.CreateErrorResponse(request, exception, new ResponseStatus(HttpStatusCode.InternalServerError.ToString()));
                    };

                //Handle Unhandled Exceptions occurring outside of Services
                //E.g. Exceptions during Request binding or in filters:
                ExceptionHandler = (req, res, operationName, ex) =>
                {
                    var errorLog = new ErrorLog
                    {
                        Application = "Scouting.RestService",
                        CreateDate = DateTimeOffset.Now,
                        Message = ex.Message,
                        StackTrace = ex.StackTrace
                    };
                    container.Resolve<Repository<ErrorLog>>().Add(errorLog);

                    res.Write("Error: {0}: {1}".Fmt(ex.GetType().Name, ex.Message));
                    res.EndRequest(skipHeaders: true);
                };
            }
        }
    }
}