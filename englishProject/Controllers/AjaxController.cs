using englishProject.Infrastructure;
using englishProject.Infrastructure.HelperClass;
using englishProject.Infrastructure.Users;
using englishProject.Infrastructure.ViewModel;
using englishProject.Models;
using log4net.Repository.Hierarchy;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace englishProject.Controllers
{
    public class AjaxController : ApiController
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public UserAppManager usermanager
        {
            get { return HttpContext.Current.GetOwinContext().GetUserManager<UserAppManager>(); }
        }

        public IAuthenticationManager Authen
        {
            get { return HttpContext.Current.GetOwinContext().Authentication; }
        }

        /// <summary>
        /// True dönerse login başarılı false dönerse başarısız
        /// </summary>
        /// <param name="UserSignInVM"></param>
        /// <returns></returns>
        [System.Web.Http.ActionName("SignIn")]
        public async Task<IHttpActionResult> POSTUserLogin(UserSignInVM UserSignInVM)
        {
            bool result = false;
            UserApp user = await usermanager.FindByEmailAsync(UserSignInVM.Email);

            try
            {
                if (user != null)
                {
                    var isUser = await usermanager.FindAsync(user.UserName, UserSignInVM.Password);

                    if (isUser != null)
                    {
                        ClaimsIdentity identity = await usermanager.CreateIdentityAsync(user,
                            DefaultAuthenticationTypes.ApplicationCookie);

                        Authen.SignOut();
                        Authen.SignIn(new AuthenticationProperties() { IsPersistent = UserSignInVM.MeRemember }, identity);
                        result = true;
                    }
                }

                return Content(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                logger.Error("SignIn", ex);
                return BadRequest();
            }
        }

        /// <summary>
        /// SignUp metodu geriye 0: dönerse başarısız :1 dönerse başarılı 2: dönerse email adresi kayıtlı
        /// </summary>
        /// <param name="UserSignUpVM"></param>
        /// <returns></returns>
        [System.Web.Http.ActionName("SignUp")]
        public async Task<IHttpActionResult> POSTUserSignUp(UserSignUpVM UserSignUpVM)
        {
            int result = 0;

            var userEmail = await usermanager.FindByEmailAsync(UserSignUpVM.Email);
            if (userEmail == null)
            {
                try
                {
                    UserApp user = new UserApp() { UserName = UserSignUpVM.Email, Email = UserSignUpVM.Email, PicturePath = "/Content/images/user.png" };

                    IdentityResult identResult = await usermanager.CreateAsync(user, UserSignUpVM.Password);
                    if (identResult.Succeeded)
                    {
                        result = 1;

                        ClaimsIdentity identity = await usermanager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

                        Authen.SignOut();
                        Authen.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);

                        OperationDirect.UpdateTargetDailyTargetScore(100, user.Id, false);
                    }
                    else
                    {
                        result = 0;
                    }
                }
                catch (Exception ex)
                {
                    result = 0;
                    logger.Error("SignUp", ex);
                }
            }
            else
            {
                result = 2;
            }

            return Content(HttpStatusCode.OK, result);
        }

        [System.Web.Http.ActionName("WordModulSubLevelQuestions")]
        public IHttpActionResult GetWordModulSubLevelQuestions(int subLevel, int levelId)
        {
            ModulSubLevel s = (ModulSubLevel)Enum.Parse(typeof(ModulSubLevel), subLevel.ToString());

            try
            {
                return Content(HttpStatusCode.OK, new Operations().GetWordModul(s, levelId).Item1);
            }
            catch (Exception ex)
            {
                logger.Error("WordModulSubLevelQuestions", ex);
                return NotFound();
            }
        }

        [System.Web.Http.ActionName("PictureWordModulSubLevelQuestions")]
        public IHttpActionResult GetPictureWordModulSubLevelQuestions(int subLevel, int levelId)
        {
            ModulSubLevel s = (ModulSubLevel)Enum.Parse(typeof(ModulSubLevel), subLevel.ToString());

            try
            {
                return Content(HttpStatusCode.OK, new Operations().GetPictureWordModul(s, levelId).Item1);
            }
            catch (Exception ex)
            {
                logger.Error("PictureWordModulSubLevelQuestions", ex);
                return BadRequest();
            }
        }

        [System.Web.Http.ActionName("UpdateUserProgress")]
        public IHttpActionResult POSTUpdateUserProgress(levelUserProgress userProgress)
        {
            try
            {
                return Content(HttpStatusCode.OK, new Operations().UpdateUserProggress(userProgress));
            }
            catch (Exception ex)
            {
                logger.Error("UpdateUserProgress", ex);
                return BadRequest();
            }
        }

        [System.Web.Http.ActionName("UpdateUser")]
        public IHttpActionResult POSTUpdateUser(UserViewModel user)
        {
            return Content(HttpStatusCode.OK, new Operations().UpdateUser(user));
        }

        [System.Web.Http.ActionName("UpdateUserPassword")]
        public IHttpActionResult POSTUpdateUserPassword(UserPasswordViewModel user)
        {
            return Content(HttpStatusCode.OK, new Operations().UpdateUserPassword(user));
        }

        [System.Web.Http.ActionName("UpdateDailyTargetScore")]
        public IHttpActionResult GETUpdateDailyTargetScore(int score = 100)
        {
            return Content(HttpStatusCode.OK, OperationDirect.UpdateTargetDailyTargetScore(score, Operations.GetUserId));
        }

        [System.Web.Http.ActionName("CommentIssueSave")]
        public IHttpActionResult POSTCommentIssueSave(CommentIssueVM viewModel)
        {
            return Content(HttpStatusCode.OK, OperationDirect.CommentIssueSave(viewModel));
        }

        [System.Web.Http.ActionName("GetChart")]
        public JsonResult<List<ScoreChart>> GETGetChart()
        {
            return Json(new Operations().GetScoreChart());
        }
    }
}