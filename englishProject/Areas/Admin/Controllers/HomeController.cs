using englishProject.Areas.Admin.Infrastructure;
using englishProject.Infrastructure;
using englishProject.Infrastructure.Users;
using englishProject.Infrastructure.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace englishProject.Areas.Admin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public UserAppManager usermanager
        {
            get { return System.Web.HttpContext.Current.GetOwinContext().GetUserManager<UserAppManager>(); }
        }

        public IAuthenticationManager Authen
        {
            get { return System.Web.HttpContext.Current.GetOwinContext().Authentication; }
        }

        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SpeedWord(int subLevel = 1, int levelId = 1)
        {
            Operations operations = new Operations();

            return View(operations.GetWordModul((ModulSubLevel)subLevel, levelId));
        }

        public ActionResult GetCoordinates()
        {
            return View();
        }

        public ActionResult Log()
        {
            return View(OperationsAdmin.GetLogs());
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Authen.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult login(string txtUserName, string txtPassword)
        {
            var user = usermanager.Find(txtUserName, txtPassword);

            if (user != null)
            {
                ClaimsIdentity identity = usermanager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                Authen.SignOut();
                Authen.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış");
                return View();
            }
        }
    }
}