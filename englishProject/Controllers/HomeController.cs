using englishProject.Infrastructure;
using System.Web;
using System.Web.Mvc;

namespace englishProject.Controllers
{
    public class HomeController : Controller
    {
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
            return View();
        }
    }
}