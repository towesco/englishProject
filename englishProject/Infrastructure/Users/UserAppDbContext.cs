using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Infrastructure.Users
{
    public class UserAppDbContext : IdentityDbContext<UserApp>
    {
        public UserAppDbContext()
            : base("EnglishProjectUserDB")
        {
        }

        public System.Data.Entity.DbSet<englishProject.Infrastructure.Users.UserApp> UserApps { get; set; }
    }
}