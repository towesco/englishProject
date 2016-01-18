using englishProject.Infrastructure;
using englishProject.Infrastructure.Users;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;
using WebGrease.Activities;

namespace englishProject.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Operations operations;

        public HomeController()
        {
            operations = new Operations();
        }

        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return View();
            }
        }

        public ActionResult deneme()
        {
            logger.Error("hata oldu");
            logger.Fatal("ülümcül hata");
            logger.Info(("bilgilendirme mesajı"));

            return View();
        }
    }
}