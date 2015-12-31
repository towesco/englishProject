using AutoMapper;
using englishProject.Areas.Admin.Infrastructure;
using englishProject.Infrastructure.Users;
using englishProject.Infrastructure.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace englishProject
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Mapper.CreateMap<UserApp, UserViewModel>()
                .ForMember(dest => dest.BirthDay,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.BirthDay.HasValue
                                    ? ((DateTime)src.BirthDay).ToShortDateString()
                                    : src.BirthDay.ToString())).ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.PasswordHash));

            BundleTable.EnableOptimizations = true;
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DependencyResolver.SetResolver((IDependencyResolver)new NinjectLanguageResolver());
        }
    }
}