using englishProject.Infrastructure.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace englishProject.Infrastructure
{
    public class UserAppManager : UserManager<UserApp>
    {
        public UserAppManager(IUserStore<UserApp> store)
            : base(store)
        {
        }

        public UserAppManager user()
        {
            return new UserAppManager(new UserStore<UserApp>());
        }

        public static UserAppManager Create(IdentityFactoryOptions<UserAppManager> identityFactoryOptions, IOwinContext owinContext)
        {
            UserAppManager manager = new UserAppManager(new UserStore<UserApp>(new UserAppDbContext()));

            return manager;
        }
    }
}