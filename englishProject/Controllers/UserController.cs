﻿using AutoMapper;
using englishProject.Infrastructure;
using englishProject.Infrastructure.Users;
using englishProject.Infrastructure.ViewModel;
using englishProject.Models;
using Facebook;
using log4net.Repository.Hierarchy;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace englishProject.Controllers
{
    [Authorize]
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
            ViewBag.Title = "Anasayfa";
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
            ViewBag.Title = level.levelNumber + ".Seviye sınavı";
            Modul m = (Modul)Enum.Parse(typeof(Modul), level.levelModul.ToString(CultureInfo.InvariantCulture));
            ModulSubLevel sub = (ModulSubLevel)Enum.Parse(typeof(ModulSubLevel), subLevel.ToString());

            ViewBag.level = level;
            ViewBag.nextLevel = operations.GetNextLevel(level);
            ViewBag.modul = m;
            ViewBag.UserDetail = operations.GetUserDetail();
            switch (m)
            {
                case Modul.WordModul:

                    ViewBag.exam = operations.GetWordModul(sub, levelId).Item1;

                    break;

                case Modul.PictureWordModul:

                    ViewBag.exam = operations.GetPictureWordModul(sub, levelId).Item1;

                    break;

                case Modul.SynonymWordModul:
                    ViewBag.exam = operations.GetSynonymWordModul(sub, levelId).Item1;
                    break;
            }

            return View();
        }

        public ActionResult commentsLevel(int id)
        {
            ViewBag.userProfilView = operations.GetUserProfilViewMenu();
            ViewBag.comments = operations.GetComment(id);

            Level l = operations.GetLevel(id);
            ViewBag.level = l;
            ViewBag.Title = l.levelNumber + ". seviye yorumları";

            return View();
        }

        public ActionResult comments()
        {
            ViewBag.Title = "Yorumlar";
            ViewBag.userProfilView = operations.GetUserProfilViewMenu();
            ViewBag.comments = operations.GetComment();
            return View();
        }

        public ActionResult RemenderCard(int id)
        {
            ViewBag.userProfilView = operations.GetUserProfilViewMenu();

            Level level = operations.GetLevel(1);
            ViewBag.Title = level.levelNumber + ". seviye hatırlatma kartları";
            return View(level);
        }

        public ActionResult Settings(int id = 0)
        {
            ViewBag.Title = "Ayarlar";
            UserApp userApp = operations.getProfil();

            ViewBag.user = Mapper.Map<UserViewModel>(userApp);

            ViewBag.City = new SelectList(new HelperMethod().GetCityListItems(), "Value", "Text", userApp.City.ToString());
            ViewBag.DailyTargetScore = operations.GetUserDetail().DailyTargetScore;
            ViewBag.userProfilView = operations.GetUserProfilViewMenu();
            ViewBag.userApp = operations.getProfil();
            ViewBag.index = id;
            ViewBag.chart = operations.GetScoreChart();
            ViewBag.UserDetail = operations.GetUserDetail();
            return View();
        }

        public ActionResult Profil()
        {
            ViewBag.Title = "Profil";
            return View();
        }

        public ActionResult ScoreTable()
        {
            ViewBag.userProfilView = operations.GetUserProfilViewMenu();
            ViewBag.ScoreTableBox = OperationDirect.GetScoreTable();
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult PageFound(string aspxerrorpath)
        {
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType).Error(aspxerrorpath);
            return View();
        }

        public ActionResult PageError(string aspxerrorpath)
        {
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)
                .Error(aspxerrorpath);

            return View();
        }

        public ActionResult deneme()
        {
            //ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            //return View(ident.Claims.ToList());

            ViewBag.chart = operations.GetScoreChart();

            return View();
        }

        public ActionResult deneme2()
        {
            return View();
        }

        public ActionResult SignOut()
        {
            Authen.SignOut();

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

                user = new UserApp { Email = infoEmail.email, UserName = info.DefaultUserName, PicturePath = profilImage, createTime = DateTime.Now };

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
            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTime.Now.AddDays(120) }, identity);

            //return Redirect(ReturnUrl ?? "/User/Index");
            return RedirectToAction("Index");
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
                user = new UserApp { Email = info.Email, UserName = info.DefaultUserName, PicturePath = "/Content/images/user.png", createTime = DateTime.Now };

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

            HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTime.Now.AddDays(120) }, identity);

            //return Redirect(ReturnUrl ?? "/User/Index");

            return RedirectToAction("Index");
        }

        #endregion Social Login

        #region ajax

        public JsonResult GetMessage(int key)
        {
            return Json(HelperMethod.GetMessage(key), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ChartAjax()
        {
            return PartialView("Templates/SettingsChartPartial", operations.GetScoreChart());
        }

        [HttpGet]
        public ActionResult LevelExamStartAjax(int levelId)
        {
            return PartialView("Templates/ExamStart", operations.GetExamLevelStart(levelId));
        }

        public JsonResult GetMessageHide(string key)
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