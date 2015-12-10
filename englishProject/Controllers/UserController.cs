using englishProject.Infrastructure;
using englishProject.Infrastructure.Users;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace englishProject.Controllers
{
    public class UserController : Controller
    {
        public UserAppManager usermanager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<UserAppManager>(); }
        }

        public RoleAppManager rolemanager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<RoleAppManager>();
            }
        }

        public IAuthenticationManager Authen
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        // GET: User
        public ActionResult Index()
        {
            ViewBag.boxs = Operations.GetBoxs(Kind.English);
            ViewBag.user = Operations.getProfil();
            return View();
        }

        public ActionResult deneme()
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;

            return View(ident.Claims.ToList());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult FacebookLogin(string ReturnUrl)
        {
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("facebookcallback", new { ReturnUrl = ReturnUrl })
            };

            Authen.Challenge(properties, "Facebook");
            return new HttpUnauthorizedResult();
        }

        [AllowAnonymous]
        public async Task<ActionResult> facebookcallback(string ReturnUrl)
        {
            ExternalLoginInfo info = await Authen.GetExternalLoginInfoAsync();

            var user = await usermanager.FindAsync(info.Login);

            if (user == null)
            {
                var token = HttpContext.GetOwinContext()
                                  .Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ExternalCookie).FindFirstValue("FacebookAccessToken");
                var fb = new FacebookClient(token);

                dynamic infoEmail = fb.Get("/me?fields=email");

                var me = fb.Get("me") as JsonObject;
                var userId = me["id"];

                ////user picture:  http://graph.facebook.com/10206530076964065/picture?type=large

                string profilImage = "http://graph.facebook.com/" + userId.ToString() + "/picture?type=large";

                user = new UserApp() { Email = infoEmail.email, UserName = info.DefaultUserName, PicturePath = profilImage };

                var result = await usermanager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await usermanager.AddLoginAsync(user.Id, info.Login);
                }
                else
                {
                    //hata mesajı yazılacak
                }
            }

            ClaimsIdentity identity = await usermanager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            identity.AddClaims(info.ExternalIdentity.Claims);
            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);
            return Redirect(ReturnUrl ?? "/");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GoogleLogin(string ReturnUrl)
        {
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("googlecallback", new { ReturnUrl = ReturnUrl })
            };
            Authen.Challenge(properties, "Google");

            return new HttpUnauthorizedResult();
        }

        [AllowAnonymous]
        public async Task<ActionResult> googlecallback(string ReturnUrl)
        {
            ExternalLoginInfo info = HttpContext.GetOwinContext().Authentication.GetExternalLoginInfo();

            UserApp user = await usermanager.FindAsync(info.Login);

            if (user == null)
            {
                user = new UserApp() { Email = info.Email, UserName = info.DefaultUserName };

                IdentityResult result = await usermanager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await usermanager.AddLoginAsync(user.Id, info.Login);
                }
                else
                {
                    //hata mesajı hazdırılıacak
                }
            }

            ClaimsIdentity identity = await usermanager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            identity.AddClaims(info.ExternalIdentity.Claims);

            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);
            return Redirect(ReturnUrl ?? "/");
        }
    }
}