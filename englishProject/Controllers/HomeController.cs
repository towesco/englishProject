using englishProject.Infrastructure;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace englishProject.Controllers
{
    public class HomeController : Controller
    {
        private Operations operations;

        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            if (filterContext.Principal.Identity.IsAuthenticated)
            {
                Response.Redirect("/User/index");
            }

            base.OnAuthentication(filterContext);
        }

        public HomeController()
        {
            operations = new Operations();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult deneme()
        {
            return View();
        }
    }
}