using englishProject.Infrastructure;
using englishProject.Infrastructure.Users;
using englishProject.Models;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace englishProject.Controllers
{
    public class UserController : Controller
    {
        private readonly Operations operations;

        public UserController()
        {
            operations = new Operations();
        }

        #region Identity Gets

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

        #endregion Identity Gets

        // GET: User
        public ActionResult Index()
        {
            ViewBag.boxs = operations.GetBoxs(Kind.English);
            ViewBag.userProfilView = operations.GetUserProfilViewMenu();
            ViewBag.boxMenu = operations.GetBoxMenu();

            return View();
        }

        public ActionResult levelExam(int levelNumber, int kind, int? subLevel)
        {
            Level level = operations.GetLevel(levelNumber, kind);
            Modul m = (Modul)Enum.Parse(typeof(Modul), level.levelModul.ToString(CultureInfo.InvariantCulture));
            ViewBag.level = level;
            ViewBag.modul = m;
            switch (m)
            {
                case Modul.WordModul:
                    WordModulSubLevel sub = (WordModulSubLevel)Enum.Parse(typeof(WordModulSubLevel), subLevel.ToString());
                    var result = operations.GetWordModul(sub, levelNumber, kind);
                    ViewBag.exam = result.Item1;
                    break;

                case Modul.IrregularVerbModul:
                    break;
            }

            return View();
        }

        public ActionResult deneme()
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;

            operations.GetWordModul(WordModulSubLevel.Temel, 1, 1);

            return View(ident.Claims.ToList());
        }

        [HttpGet]
        public ActionResult LevelExamStartAjax(int levelNumber, int kind)
        {
            Level level = operations.GetLevel(levelNumber, kind);
            string partialView = "Modul/" + Enum.GetName(typeof(Modul), level.levelModul) + "Start";
            return PartialView(partialView, operations.GetExamLevelStart(levelNumber, kind));
        }

        #region Social Login

        [HttpPost]
        [AllowAnonymous]
        public ActionResult FacebookLogin(string ReturnUrl)
        {
            AuthenticationProperties properties = new AuthenticationProperties
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

                string profilImage = "http://graph.facebook.com/" + userId + "/picture?type=large";

                user = new UserApp { Email = infoEmail.email, UserName = info.DefaultUserName, PicturePath = profilImage };

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
            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);
            return Redirect(ReturnUrl ?? "/");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GoogleLogin(string ReturnUrl)
        {
            AuthenticationProperties properties = new AuthenticationProperties
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
                user = new UserApp { Email = info.Email, UserName = info.DefaultUserName };

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

            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);
            return Redirect(ReturnUrl ?? "/");
        }

        #endregion Social Login
    }
}