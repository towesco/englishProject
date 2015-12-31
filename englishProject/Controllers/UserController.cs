using AutoMapper;
using AutoMapper.Internal;
using englishProject.Infrastructure;
using englishProject.Infrastructure.Users;
using englishProject.Infrastructure.ViewModel;
using englishProject.Models;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
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
            ViewBag.boxs = operations.GetBoxs();
            ViewBag.userProfilView = operations.GetUserProfilViewMenu();
            ViewBag.boxMenu = operations.GetBoxMenu();
            ViewBag.userDetail = operations.GetUserDetail();
            ViewBag.userTargetPercent = operations.GetUserTargetPercent();
            return View();
        }

        public ActionResult levelExam(int levelId, int? subLevel = 1)
        {
            Level level = operations.GetLevel(levelId);
            Modul m = (Modul)Enum.Parse(typeof(Modul), level.levelModul.ToString(CultureInfo.InvariantCulture));
            ModulSubLevel sub = (ModulSubLevel)Enum.Parse(typeof(ModulSubLevel), subLevel.ToString());

            ViewBag.level = level;
            ViewBag.nextLevel = operations.GetNextLevel(level);
            ViewBag.modul = m;
            switch (m)
            {
                case Modul.WordModul:

                    ViewBag.exam = operations.GetWordModul(sub, levelId).Item1;

                    break;

                case Modul.PictureWordModul:

                    ViewBag.exam = operations.GetPictureWordModul(sub, levelId).Item1;

                    break;
            }

            return View();
        }

        public ActionResult RemenderCard(int id)
        {
            ViewBag.userProfilView = operations.GetUserProfilViewMenu();

            Level level = operations.GetLevel(1);

            return View(level);
        }

        public ActionResult Settings(int id = 0)
        {
            UserApp userApp = operations.getProfil();

            ViewBag.user = Mapper.Map<UserViewModel>(userApp);

            ViewBag.City = new SelectList(new HelperMethod().GetCityListItems(), "Value", "Text", userApp.City.ToString());
            ViewBag.DailyTargetScore = operations.GetUserDetail().DailyTargetScore;
            ViewBag.index = id;

            return View();
        }

        public ActionResult deneme()
        {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;

            operations.GetWordModul(ModulSubLevel.Temel, 1);
            return View(ident.Claims.ToList());
        }

        public ActionResult SignOut()
        {
            Authen.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
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
                    OperationDirect.UpdateTargetDailyTargetScore(100, user.Id, false);
                }
                else
                {
                    //hata mesajı yazılacak
                }
            }

            ClaimsIdentity identity = await usermanager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            identity.AddClaims(info.ExternalIdentity.Claims);
            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);

            return Redirect(ReturnUrl ?? "/User/Index");
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
                    OperationDirect.UpdateTargetDailyTargetScore(100, user.Id, false);
                }
                else
                {
                    //hata mesajı hazdırılıacak
                }
            }

            ClaimsIdentity identity = await usermanager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            identity.AddClaims(info.ExternalIdentity.Claims);

            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);

            return Redirect(ReturnUrl ?? "/User/Index");
        }

        #endregion Social Login

        #region ajax

        [HttpGet]
        public ActionResult LevelExamStartAjax(int levelId)
        {
            return PartialView("Templates/ExamStart", operations.GetExamLevelStart(levelId));
        }

        public JsonResult MessageHide(string key)
        {
            return Json(HelperMethod.GetMessageHide(key), JsonRequestBehavior.AllowGet);
        }

        public ContentResult UploadUserPicture(HttpPostedFileBase fileUpload)
        {
            return Content(operations.GetUpdateUserPicture(fileUpload));
        }

        #endregion ajax
    }
}