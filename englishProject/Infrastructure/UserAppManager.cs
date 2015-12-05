using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

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
    }
}