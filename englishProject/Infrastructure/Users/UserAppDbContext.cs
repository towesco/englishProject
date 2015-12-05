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
    }
}