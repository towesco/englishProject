using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new SetInitializerDbContext());
            base.OnModelCreating(modelBuilder);
        }
    }

    public class SetInitializerDbContext : DropCreateDatabaseIfModelChanges<UserAppDbContext>
    {
    }
}