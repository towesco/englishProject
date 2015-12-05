using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Infrastructure.Users
{
    public class RoleAppManager : RoleManager<RoleApp>
    {
        public RoleAppManager(IRoleStore<RoleApp, string> store)
            : base(store)
        {
        }

        public static RoleAppManager Create(IdentityFactoryOptions<RoleAppManager> identityFactoryOptions, IOwinContext owinContext)
        {
            RoleAppManager manager = new RoleAppManager(new RoleStore<RoleApp>(owinContext.Get<UserAppDbContext>()));
            return manager;
        }
    }
}