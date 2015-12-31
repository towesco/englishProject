using englishProject.Infrastructure;
using englishProject.Infrastructure.Users;
using englishProject.Infrastructure.ViewModel;
using englishProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
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

            return Content(HttpStatusCode.OK, new Operations().GetWordModul(s, levelId).Item1);
        }

        [System.Web.Http.ActionName("PictureWordModulSubLevelQuestions")]
        public IHttpActionResult GetPictureWordModulSubLevelQuestions(int subLevel, int levelId)
        {
            ModulSubLevel s = (ModulSubLevel)Enum.Parse(typeof(ModulSubLevel), subLevel.ToString());

            return Content(HttpStatusCode.OK, new Operations().GetPictureWordModul(s, levelId).Item1);
        }

        [System.Web.Http.ActionName("UpdateUserProgress")]
        public IHttpActionResult POSTUpdateUserProgress(levelUserProgress userProgress)
        {
            return Content(HttpStatusCode.OK, new Operations().UpdateUserProggress(userProgress));
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
    }
}