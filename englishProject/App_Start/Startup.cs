using englishProject.Infrastructure;
using englishProject.Infrastructure.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(englishProject.Startup))]

namespace englishProject
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<UserAppDbContext>(() => new UserAppDbContext());
            app.CreatePerOwinContext<UserAppManager>(UserAppManager.Create);
            app.CreatePerOwinContext<RoleAppManager>(RoleAppManager.Create);
        }
    }
}