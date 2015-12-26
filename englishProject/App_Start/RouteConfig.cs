using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace englishProject
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "signin-google",
                url: "signin-google",
                defaults: new { controller = "User", action = "GoogleLogin" });

            routes.MapRoute(
      name: "levelQuiz",
      url: "{controller}/{action}/{levelId}/{subLevel}",
      defaults: new { controller = "User", action = "levelQuiz" }, namespaces: new[] { "englishProject.Controllers" }

  );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }, namespaces: new[] { "englishProject.Controllers" }

            );
        }
    }
}